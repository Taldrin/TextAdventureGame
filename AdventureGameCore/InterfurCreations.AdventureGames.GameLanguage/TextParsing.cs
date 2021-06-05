using InterfurCreations.AdventureGames.Core.Services;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.Graph;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services.ImageStore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.GameLanguage
{
    public class TextParsing : ITextParsing
    {
        private readonly IGameDataService _gameDataService;
        private readonly ImageBuildDataTracker _imageBuildTracker;

        public TextParsing(IGameDataService gameDataService, ImageBuildDataTracker imageBuildTracker)
        {
            _gameDataService = gameDataService;
            _imageBuildTracker = imageBuildTracker;
        }

        public string ParseText(PlayerGameSave gameSave, string text)
        {
            try
            {
                return ParseLine(gameSave, text);
            }
            catch (Exception e)
            {
                HandleError(text, e, gameSave);
                return null;
            }
        }

        public ParsedStateOption ResolveOption(PlayerGameSave gameSave, string optionText)
        {
            try
            {
                optionText = CleanText(optionText);
                optionText = optionText.Trim(' ', '\n');
                if (optionText.StartsWith("#if"))
                {
                    var result = ParseLine(gameSave, ParseOptionCommand(gameSave, optionText));
                    ParsedStateOption option = new ParsedStateOption { OptionType = OptionType.Normal, text = result };
                    if (result == "#TRUE")
                    {
                        option.IsDirectTransition = true;
                        option.DirectTransitionCommandResult = true;
                    }
                    else if (result == "#FALSE")
                    {
                        option.IsDirectTransition = true;
                        option.DirectTransitionCommandResult = false;
                    }
                    return option;
                }
                else if (optionText.StartsWith("#fallback"))
                {
                    var result = ParseLine(gameSave, optionText.Replace("#fallback", "").Replace("[", "").Replace("]", "").Trim(' '));
                    if (string.IsNullOrWhiteSpace(result))
                        return new ParsedStateOption { OptionType = OptionType.Fallback, IsDirectTransition = true, DirectTransitionCommandResult = true };
                    else
                        return new ParsedStateOption { OptionType = OptionType.Fallback, text = result };
                }
                else
                {
                    var result = ParseLine(gameSave, optionText);
                    return new ParsedStateOption { OptionType = OptionType.Normal, text = result };
                }
            } catch(Exception e)
            {
                HandleError(optionText, e, gameSave);
                return null;
            }
        }

        public void ParseAttachment(Player player, PlayerGameSave gameSave, StateAttachment stateAttachment)
        {
            try
            {
                var text = stateAttachment.StateText.Replace("<br>", "");
                text = text.Replace("<span>", "");
                var split = text.Split('#').ToList();
                split = split.Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
                split.ForEach(a =>
                {
                    ParseAttachmentCommand(player, gameSave, a);
                });
            } catch (Exception e)
            {
                HandleError(stateAttachment.StateText, e, gameSave);
            }
        }

        public string CleanText(string text)
        {
            text = text.Replace("<br>", " ");
            text = text.Replace("<p>", " ");
            text = text.Replace("<p align=\"left\">", "");
            text = text.Replace("</p>", " ");
            text = text.Replace("<br", " ");
            text = text.Replace("</br>", " ");
            text = text.Replace("<span>", "");
            text = text.Replace("</span>", "");
            text = text.Replace("</div>", " ");
            text = text.Replace("<div>", " ");
            text = text.Replace("<font style=\"font-size: 12px\">", "");
            text = text.Replace("<font style=\"font-size: 11px\">", "");
            text = text.Replace("<font style=\"font-size: 10px\">", "");
            text = text.Replace("<span style=\"white-space: normal\">", "");
            text = text.Replace("style=\"white-space: normal\">", "");
            text = text.Replace("style=\"font-size: 10px\">", "");
            text = text.Replace("style=\"font-size: 11px\">", "");
            text = text.Replace("style=\"font-size: 12px\">", "");
            text = text.Replace("<span style=\"font - size: 12px ; white - space: normal; background - color: rgb(248, 249, 250)\">", "");
            text = text.Replace("</font>", "");
            text = text.Replace("&nbsp;", " ");
            text = text.Replace("\\n", Environment.NewLine);
            text = text.Replace("&gt;", ">");
            text = text.Replace("&lt;", "<");
            text = text.Replace("<span", "");
            text = text.Replace("    ", " ");
            text = text.Replace("   ", " ");
            text = text.Replace("  ", " ");
            if (text.Contains(">") && text.Contains("<"))
                text = Regex.Replace(text, "<.*?>", "");
            return text;
        }

        private void ParseAttachmentCommand(Player player, PlayerGameSave gameSave, string command)
        {
            command = CleanText(command);
            var commandSplit = command.Split(' ');
            if(command.StartsWith("imageBuild"))
            {
                ResolveImageBuildCommand(commandSplit);
            }
            else if (command.StartsWith("save"))
            {
                var dataName = commandSplit[1];
                _gameDataService.SaveData(gameSave, dataName);
            } else if(command.StartsWith("set"))
            {
                var dataName = commandSplit[1];
                if (commandSplit[2] == "=")
                {
                    var valueSet = ResolveValue(ConcatSplitAfterIndex(commandSplit,3), gameSave);
                    _gameDataService.SaveData(gameSave, dataName, valueSet);
                }
                else if (commandSplit[2] == "--")
                    _gameDataService.DecrementData(gameSave, dataName);
                else if (commandSplit[2] == "++")
                    _gameDataService.IncrementData(gameSave, dataName);
                else
                    throw new Exception("Error parsing: " + command + " as an attachment state command");
            } else if(command.StartsWith("delete"))
            {
                var dataName = commandSplit[1];
                _gameDataService.DeleteData(gameSave, dataName);
            } else if(command.StartsWith("achievement"))
            {
                var dataValue = command.Substring(11, command.Length - 11).Trim();
                _gameDataService.SavePermanentData(player, gameSave.GameName, dataValue, PlayerSaveDataType.ACHIEVEMENT);
            }
        }

        private void ResolveImageBuildCommand(string[] commandSplit)
        {
            commandSplit = commandSplit.Where(a => !string.IsNullOrEmpty(a)).ToArray();
            var imageUrl = commandSplit[1].Trim();
            string coordinates;
            string opacity;

            var imageParam = new ImageBuildParameter();

            if (commandSplit.Length > 2)
            {
                coordinates = commandSplit[2];
                var coordParams = coordinates.Trim().Trim('(', ')').Split(',');
                if (coordParams.Length != 2)
                    throw new ApplicationException($"Supplied coordinates for image building are an incorrect format '{coordinates}' from command string '{string.Join(" ", commandSplit)}'");
                if(!int.TryParse(coordParams[0], out var xCord))
                    throw new ApplicationException($"Supplied coordinates for image building are an incorrect format. Could not parse int '{coordParams[0]}' from command string '{string.Join(" ", commandSplit)}'");
                if(!int.TryParse(coordParams[1], out var yCord))
                    throw new ApplicationException($"Supplied coordinates for image building are an incorrect format. Could not parse int '{coordParams[1]}' from command string '{string.Join(" ", commandSplit)}'");
                imageParam.Location = new Vector2(xCord, yCord);
            }
            if (commandSplit.Length > 3)
            {
                opacity = commandSplit[3];
                if(!float.TryParse(opacity, out var parsedOpacity))
                    throw new ApplicationException($"Supplied opacity for image build failed. Could not parse float '{opacity}' from command string '{string.Join(" ", commandSplit)}'");

                imageParam.Opacity = parsedOpacity;
            }
            if(commandSplit.Length > 4)
            {
                string size = commandSplit[4];
                var sizeParams = size.Trim().Trim('(', ')').Split(',');
                if (sizeParams.Length != 2)
                    throw new ApplicationException($"Supplied size for image building are an incorrect format '{size}' from command string '{string.Join(" ", commandSplit)}'");
                if (!int.TryParse(sizeParams[0], out var xCord))
                    throw new ApplicationException($"Supplied size for image building are an incorrect format. Could not parse int '{sizeParams[0]}' from command string '{string.Join(" ", commandSplit)}'");
                if (!int.TryParse(sizeParams[1], out var yCord))
                    throw new ApplicationException($"Supplied size for image building are an incorrect format. Could not parse int '{sizeParams[1]}' from command string '{string.Join(" ", commandSplit)}'");
                imageParam.Size = new Vector2(xCord, yCord);
            }

            imageParam.Image = imageUrl;

            _imageBuildTracker.AddParam(imageParam);
        }

        private string ParseLine(PlayerGameSave gameSave, string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;
            line = CleanText(line);
            var split = line.Split(new string[] { "{{" }, StringSplitOptions.None);
            var lineConstruction = "";
            lineConstruction = split[0];
            for (int i = 1; i < split.Length; i++)
            {
                int indexOfClose = split[i].IndexOf("}}");
                string parseString = split[i].Substring(0, indexOfClose);
                string parsedWord = ParseWord(gameSave, parseString);
                lineConstruction = lineConstruction + parsedWord + split[i].Substring(parseString.Length + 2, split[i].Length - parseString.Length - 2);
            }

            return lineConstruction;
        }

        private string ParseWord(PlayerGameSave gameSave, string word)
        {
            var trimmed = word.Trim(' ');
            if (trimmed.Substring(0, 1) == "#")
            {
                return ParseCommad(gameSave, trimmed);
            }
            return _gameDataService.GetData(gameSave, word);
        }

        private string ParseCommad(PlayerGameSave gameSave, string command)
        {
            var commandSplit = command.Split('[', ']');
            commandSplit = commandSplit.Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();
            for(int i = 0; i < commandSplit.Length; i = i + 2)
            {
                var result = ResolveCommand(gameSave, commandSplit[i]);
                if (result)
                    return commandSplit[i + 1];
            }
            return null;
        }

        private string ParseOptionCommand(PlayerGameSave gameSave, string command)
        {
            if (command.EndsWith("]"))
            {
                var commandSplit = command.Split('[', ']');
                commandSplit = commandSplit.Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();
                var result = ResolveCommand(gameSave, commandSplit[0]);
                if (result)
                    return commandSplit[1];
            } else
            {
                var result = ResolveCommand(gameSave, command);
                if (result)
                    return "#TRUE";
                else
                    return "#FALSE";
            }
            return null;
        }

        public bool ResolveCommand(PlayerGameSave gameSave, string command)
        {
            command = CleanText(command);
            command = command.Trim(' ');
            if (command.StartsWith("#if"))
            {
                return ResolveIf(gameSave, command);
            } 
            if(command.StartsWith("#else"))
            {
                return true;
            }
            throw new ApplicationException("Error parsing: " + command);
        }

        private bool ResolveIf(PlayerGameSave gameSave, string command)
        {
            var split = command.Split(null);
            split = split.Where(a => !string.IsNullOrEmpty(a)).ToArray();
            bool andMore = false;
            bool orMore = false;
            int indexMod = 0;
            bool? initialResult = null;
            if (split.Length > 2 && (split[2] == "=" || split[2] == ">" || split[2] == "<"))
            {
                if (split.Length > 4)
                {
                    andMore = split[4] == "#and";
                    orMore = split[4] == "#or";
                }

                var data1 = ResolveValue(split[1], gameSave);
                var conditional = split[2];
                var data2 = ConcactSplitAfterxUptoCharacter(split, 3, "#");

                var doesData2Exist = _gameDataService.GetData(gameSave, data2);
                if (doesData2Exist != null) data2 = doesData2Exist;

                data2 = ResolveValue(data2, gameSave);

                if (split[0] == "#ifnot")
                {
                    initialResult = !ResolveOperation(data1, data2, conditional, gameSave);
                }
                if (split[0] == "#if")
                {
                    initialResult = ResolveOperation(data1, data2, conditional, gameSave);
                }
                indexMod = 5;
            }
            else
            {
                if (split.Length > 2)
                {
                    andMore = split[2] == "#and";
                    orMore = split[2] == "#or";
                }
                bool exists = _gameDataService.DataExists(gameSave, split[1]);
                if (split[0] == "#ifnot")
                    initialResult = !exists;
                if (split[0] == "#if")
                    initialResult = exists;
                indexMod = 3;
            }

            if (andMore)
                return initialResult.Value && ResolveIf(gameSave, ConcatSplitAfterIndex(split, indexMod));
            else if (orMore)
            {
                return initialResult.Value || ResolveIf(gameSave, ConcatSplitAfterIndex(split, indexMod));
            }
            else
            {
                return initialResult.Value;
            }
            throw new ApplicationException("Error parsing: " + command);
        }

        private string ConcactSplitAfterxUptoCharacter(string[] split, int index, string endingChar)
        {
            var crString = "";
            for(int i = index; i < split.Length; i++)
            {
                if(split[i].StartsWith(endingChar))
                {
                    return crString.TrimEnd(' ');
                }
                crString = crString + split[i] + " ";
            }
            return crString.TrimEnd(' ');
        }

        private string ConcatSplitAfterIndex(string[] split, int index)
        {
            var crString = "";
            foreach (string str in split.Skip(index).ToList())
            {
                crString = crString + " " + str;
            }
            return crString.Trim(' ');
        }

        private void HandleError(string identifyingText, Exception e, PlayerGameSave gameSave = null)
        {
            Log.LogMessage(ErrorMessageHelper.MakeMessage(e, gameSave, "Parsing text: " + identifyingText), LogType.Error);
            throw e;
        }

        private bool ResolveOperation(string data1, string data2, string operat, PlayerGameSave gameSave)
        {
            data1 = ResolveValue(data1, gameSave);
            data2 = ResolveValue(data2, gameSave);
            if (operat == ">")
            {
                if (!int.TryParse(data1, out int data1int))
                    throw new ArgumentException($"{data1} was tried to be operated on in a '>' command, but it is not a number!");
                if (!int.TryParse(data2, out int data2int))
                    throw new ArgumentException($"{data2} was tried to be operated on in a '>' command, but it is not a number!");
                return data1int > data2int;
            }
            if (operat == "<")
            {
                if (!int.TryParse(data1, out int data1int))
                    throw new ArgumentException($"{data1} was tried to be operated on in a '<' command, but it is not a number!");
                if (!int.TryParse(data2, out int data2int))
                    throw new ArgumentException($"{data2} was tried to be operated on in a '<' command, but it is not a number!");
                return data1int < data2int;
            }
            if(operat == "=")
            {
                return data1 == data2;
            }
            throw new ArgumentException($"Found a command with an operator {operat} which is not valid");
        }

        private string ResolveValue(string valueString, PlayerGameSave gameSave)
        {
            var mainSplit = valueString.Split(' ');

            string currentVal = "";

            if (mainSplit[0].StartsWith("rand"))
            {
                var split = valueString.Split(',');
                var fromString = split[0].Split('(')[1];
                var toString = split[1].Split(')')[0];

                if (!int.TryParse(fromString, out int fromValue))
                {
                    throw new ArgumentException("Attempted to create rand from: " + valueString + " But the section: " + fromString + " was not a number");
                }
                if (!int.TryParse(toString, out int toValue))
                {
                    throw new ArgumentException("Attempted to create rand from: " + valueString + " But the section: " + fromString + " was not a number");
                }

                currentVal = "" + (new Random().Next(fromValue, toValue));
            }
            else if (_gameDataService.DataExists(gameSave, mainSplit[0]))
            {
                currentVal = _gameDataService.GetData(gameSave, mainSplit[0]);
            }
            else if (int.TryParse(mainSplit[0], out int valResult))
            {
                currentVal = "" + valResult;
            }
            else
            {
                currentVal = valueString;
            }

            if (mainSplit.Length > 1)
            {
                if (mainSplit[1] == "+")
                {
                    return "" + (int.Parse(currentVal) + int.Parse(ResolveValue(ConcatSplitAfterIndex(mainSplit, 2), gameSave)));
                }
                else if (mainSplit[1] == "-")
                {
                    return "" + (int.Parse(currentVal) - int.Parse(ResolveValue(ConcatSplitAfterIndex(mainSplit, 2), gameSave)));
                }
                else if (mainSplit[1] == "*")
                {
                    return "" + (int.Parse(currentVal) * int.Parse(ResolveValue(ConcatSplitAfterIndex(mainSplit, 2), gameSave)));
                }
                else if (mainSplit[1] == "//")
                {
                    return "" + (int.Parse(currentVal) / int.Parse(ResolveValue(ConcatSplitAfterIndex(mainSplit, 2), gameSave)));
                } else
                {
                    return currentVal;
                }
            }
            else
            {
                return currentVal;
            }
            throw new Exception("Shouldn't be possible");
        }

        public bool ShouldRun(string command, bool onlyRunAfterText, out string restOfCommand)
        {
            command = CleanText(command);
            command = command.Trim(' ');
            bool setRunAfter = false;
            restOfCommand = command;
            if (command.ToLower().StartsWith("#am"))
            {
                restOfCommand = command.Substring(3, command.Length - 3);
                setRunAfter = true;
            } else if(command.ToLower().StartsWith("#bm"))
            {
                restOfCommand = command.Substring(3, command.Length - 3);
                setRunAfter = false;
            }

            if (setRunAfter && onlyRunAfterText)
                return true;
            else if (!setRunAfter && !onlyRunAfterText)
                return true;
            return false;

        }
    }
}
