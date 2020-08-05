using InterfurCreations.AdventureGames.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace InterfurCreations.AdventureGames.Graph
{
    public class DrawParser
    {
        public (DrawState game, DrawMetadata metadata) ParseGameFromPath(string path)
        {
            // string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"DrawFiles\" + "DrawIoTest" + ".xml");
            var bytes = File.ReadAllBytes(path);
            return ParseGameFromBytes(bytes);
        }

        public (DrawState game, DrawMetadata metadata) ParseGameFromBytes(byte[] xml)
        {
            // string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"DrawFiles\" + "DrawIoTest" + ".xml");
            XElement rootElement = null;
            try
            {
                using (var ms = new MemoryStream(xml))
                {
                    using (var sr = new StreamReader(ms))
                    {
                        rootElement = XElement.Load(sr);
                    }
                }
            } catch (Exception e) { Log.LogMessage("Error converting XML bytes to XElement!"); return (null, null); }

            var elements = FindRootElement(rootElement).Elements();

            elements = elements.Where(a => a.Name.LocalName == "mxCell" || a.Name.LocalName == "object");

            var startingElement = GetStart(elements);

            var finishedDiagram = Parse(startingElement, elements, new List<DrawState>());

            var metaData = GetMetadata(elements);

            return (finishedDiagram, metaData);
        }

        private XElement FindRootElement(XElement startElement)
        {
            var nextElement = startElement;
            for(int i = 0; i < 100; i++)
            {
                if(nextElement.Name == "root")
                {
                    return nextElement;
                }
                nextElement = nextElement.Elements().First();
            }
            Log.LogMessage("Error finding the root element in he XML file");
            throw new Exception("Error finding the root element in he XML file");
        }

        private DrawState Parse(XElement element, IEnumerable<XElement> elementStore, List<DrawState> existingStates)
        {
            DrawState newState = new DrawState
            {
                Id = element.Attribute("id").Value,
                StateText = element.Attribute("value") != null ? element.Attribute("value").Value : element.Attribute("label").Value,
                XmlElement = element
            };

            if (element.Attribute("style") != null && element.Attribute("style").Value.Contains("rounded=1")
                || element.FirstNode.ToString().Contains("rounded=1"))
            {
                newState.IsImage = true;
            }

            var existingState = existingStates.FirstOrDefault(a => a.Id == newState.Id);
            if (existingState != null) return existingState;

            existingStates.Add(newState);
            var stateLinks = elementStore.Where(a => a.Attribute("source")?.Value == newState.Id);
            var stateTeleports = element.Attribute("teleport")?.Value;
            bool isTeleport = false;
            newState.StateOptions = new List<StateOption>();
            if (!string.IsNullOrEmpty(stateTeleports))
            {
                isTeleport = true;
                var destination = elementStore.FirstOrDefault(c => c.Attribute("id").Value == stateTeleports);
                if (destination == null) throw new Exception($"State teleport was found for state with text: '{newState.StateText}' but the destination with id: '{stateTeleports}' was not found!");
                newState.StateOptions = new List<StateOption>() { new StateOption { IsDirectTransition = true, ResultState = Parse(destination, elementStore, existingStates), StateText = null } };
                new StateOption { ResultState = Parse(destination, elementStore, existingStates), StateText = null, IsDirectTransition = true };
            }

            newState.StateOptions.AddRange(stateLinks.Select(a =>
            {
                if (a.Attribute("target") == null)
                    throw new Exception($"A transition was found for state with option text: {a.Attribute("value")?.Value} and state text: {newState.StateText} - but it had no destination!");
                var targetId = a.Attribute("target").Value;
                var target = elementStore.FirstOrDefault(c => c.Attribute("id").Value == targetId);
                string stateText = null;
                if (!string.IsNullOrEmpty(a.Attribute("value")?.Value))
                    stateText = a.Attribute("value").Value;
                else
                    stateText = elementStore.FirstOrDefault(b => b.Attribute("parent")?.Value == a.Attribute("id").Value)?.Attribute("value").Value;

                if (a.Attribute("style").Value.Contains("dashed"))
                {
                    newState.StateAttachements.Add(new StateAttachment
                    {
                        Id = a.Attribute("id").Value,
                        StateText = elementStore.FirstOrDefault(f => f.Attribute("id").Value == a.Attribute("target").Value).Attribute("value").Value,
                        StateConditional = stateText
                    });
                    return null;
                }

                if (stateText == null)
                {
                    if (isTeleport) { throw new Exception($"State teleport was found for state with text: '{newState.StateText}' but it also had regular state transitions!"); }
                    return new StateOption { Id = a.Attribute("id").Value, ResultState = Parse(target, elementStore, existingStates), StateText = null, IsDirectTransition = true };
                }
                if (isTeleport) { throw new Exception($"State teleport was found for state with text: '{newState.StateText}' but it also had regular state transitions!"); }
                return new StateOption
                {
                    Id = a.Attribute("id").Value,
                    ResultState = Parse(target, elementStore, existingStates),
                    StateText = stateText,
                    XmlElement = a
                };
            }).ToList());
            newState.StateOptions.RemoveAll(a => a == null);
            return newState;
        }

        public XElement GetStart(IEnumerable<XElement> elements)
        {
            var listOfStartObjects = elements.Where(a => a.Name.LocalName == "object" && a.Attribute("Start") != null).ToList();
            if(listOfStartObjects.Count > 1)
            {
                throw new Exception("There is more than 1 Start state defined! Only 1 state may be marked as the Start state");
            }
            if(listOfStartObjects.Count == 0)
            {
                throw new Exception("No Start state was found. Please mark a state as the Start state by adding it as custom data in Draw.io");
            }
            return listOfStartObjects.First();
        }

        public DrawMetadata GetMetadata(IEnumerable<XElement> elements)
        {
            var listOfMetadata = elements.Where(a => a.Name.LocalName == "object" && (a.Attribute("Metadata") != null || a.Attribute("metadata") != null)).ToList();
            if(listOfMetadata.Count > 1)
            {
                throw new Exception("More than 1 metadata state found");
            }

            var metadataState = listOfMetadata.FirstOrDefault();

            if (metadataState == null)
                return new DrawMetadata();

            var metaDataText = metadataState.Attribute("value") != null ? metadataState.Attribute("value").Value : metadataState.Attribute("label").Value;

            if (string.IsNullOrEmpty(metaDataText))
                return new DrawMetadata();

            metaDataText = metaDataText.CleanHtmlTags();

            DrawMetadata metadata = new DrawMetadata();

            var tokenSplit = metaDataText.Split('#');
            foreach(var command in tokenSplit)
            {
                if(command.ToLower().StartsWith("description"))
                {
                    metadata.Description = GetStringFromCommand(command);
                }
                if (command.ToLower().StartsWith("category"))
                {
                    metadata.Category = GetStringFromCommand(command);
                } 
                if(command.ToLower().StartsWith("achievement"))
                {
                    var nameValue = GetStringNameValueFromCommand(command.Substring(11, command.Length - 11));
                    metadata.Achievements.Add(new DrawAchievement
                    {
                        Description = nameValue.value,
                        Name = nameValue.name
                    });
                }
            }

            return metadata;
        }

        private string GetStringFromCommand(string commandString)
        {
            var split = commandString.Split('=');
            if (split.Length < 2)
                throw new Exception("Error whilst parsing metadata state. You may be missing an '='. Tried to parse command: " + commandString);
            var text = split[1];
            return text.Trim();
        }

        private (string name, string value) GetStringNameValueFromCommand(string commandString)
        {
            commandString = commandString.Trim();
            var split = commandString.Split('=');
            if (split.Length < 2)
                throw new Exception("Error whilst parsing metadata state. You may be missing an '='. Tried to parse command: " + commandString);
            var name = split[0];
            var value = split[1];
            return (name.Trim(), value.Trim().Trim('[', ']'));
        }
    }
}
