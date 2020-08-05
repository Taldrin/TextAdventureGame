using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace InterfurCreations.AdventureGames.MXVisualGraph
{
    public class MXGraphXMLGenerator
    {
        public string Generate(MXGraph graph)
        {
            XmlDocument xmlDocument = new XmlDocument();
            var rootElement = SetupDocument(xmlDocument);
            RecursivelyCreateNodes(xmlDocument, rootElement, graph.RootNode, new Dictionary<MXNode, XmlElement>(), new Dictionary<MXNodeConnection, XmlElement>());

            return GenerateXmlString(xmlDocument);
        }

        private string RecursivelyCreateNodes(XmlDocument document, XmlElement rootElement, MXNode currentNode, Dictionary<MXNode, XmlElement> existingNodes, Dictionary<MXNodeConnection, XmlElement> existingConnections)
        {
            XmlElement newElement = null;
            string id = null;
            if (existingNodes.TryGetValue(currentNode, out var existingElement))
            {
                newElement = existingElement;
                id = existingElement.GetAttribute("id");
                currentNode.Connections.ForEach(a =>
                {
                    string connectionId = null;
                    XmlElement newConnection = null;
                    if (existingConnections.TryGetValue(a, out var existingConnection))
                    {
                        connectionId = existingConnection.GetAttribute("id");
                        newConnection = existingConnection;
                    }
                    else
                    {
                        connectionId = Guid.NewGuid().ToString();
                        newConnection = CreateMXCellConnection(document, connectionId, a.ConnectionText, "", id);
                        existingConnections.Add(a, newConnection);
                        var createdNode = RecursivelyCreateNodes(document, rootElement, a.ResultNode, existingNodes, existingConnections);
                        newConnection.SetAttribute("target", createdNode);
                        rootElement.AppendChild(newConnection);
                    }
                });
            }
            else
            {
                id = Guid.NewGuid().ToString();
                newElement = CreateMXCellNode(document, currentNode.NodeText, id, currentNode.x, currentNode.y);
                rootElement.AppendChild(newElement);
                existingNodes.Add(currentNode, newElement);

                currentNode.Connections.ForEach(a =>
                {
                    var connectionId = Guid.NewGuid().ToString();
                    var newConnection = CreateMXCellConnection(document, connectionId, a.ConnectionText, "", id);
                    var createdNode = RecursivelyCreateNodes(document, rootElement, a.ResultNode, existingNodes, existingConnections);
                    newConnection.SetAttribute("target", createdNode);
                    rootElement.AppendChild(newConnection);
                });
            }

            return id;
        }

        private XmlElement CreateMXCellConnection(XmlDocument document, string connectionId, string text, string targetId, string sourceId)
        {
            var newElement = CreateElement(document, "mxCell", new List<(string attributeName, string attributeValue)>
            {
                ("id", connectionId),
                ("parent", "1"),
                ("edge", "1"),
                ("target", targetId),
                ("source", sourceId),
            });
            var geometryElement = CreateElement(document, "mxGeometry", new List<(string attributeName, string attributeValue)>
            {
                ("relative", "1"),
                ("as", "geometry"),
            });

            var point = CreateElement(document, "mxPoint", new List<(string attributeName, string attributeValue)>
            {
                ("relative", "1"),
                ("as", "offset"),
            });
            newElement.AppendChild(geometryElement);

            return newElement;
        }

        private XmlElement CreateMXCellNode(XmlDocument document, string text, string id, int x, int y)
        {
            var newElement = CreateElement(document, "mxCell", new List<(string attributeName, string attributeValue)>
            {
                ("id", id),
                ("value", text),
                ("parent", "1"),
                ("vertex", "1"),
                ("style", "rounded=0;whiteSpace=wrap;html=1;"),
            });
            var geometryElement = CreateElement(document, "mxGeometry", new List<(string attributeName, string attributeValue)>
            {
                ("x", "" + x),
                ("y", "" + y),
                ("width", "350"),
                ("height", "170"),
                ("as", "geometry"),
            });
            newElement.AppendChild(geometryElement);
            return newElement;
        }

        private string GenerateXmlString(XmlDocument xmlDocument)
        {
            using var stringWriter = new StringWriter();
            using var xmlTextWriter = XmlWriter.Create(stringWriter);
            xmlDocument.WriteTo(xmlTextWriter);
            xmlTextWriter.Flush();

            return stringWriter.GetStringBuilder().ToString();
        }

        private XmlElement SetupDocument(XmlDocument xml)
        {
            var mxFileElement = CreateElement(xml, "mxfile", new List<(string attributeName, string attributeValue)>
            {
                ("version", "0.0.1"),
                ("pages", "1"),
                ("host", "InterfurCreations"),
            });
            xml.AppendChild(mxFileElement);

            var diagramElement = CreateElement(xml, "diagram", new List<(string attributeName, string attributeValue)>
            {
                ("id", "0001"),
                ("name", "Page-1")
            });
            mxFileElement.AppendChild(diagramElement);

            var graphModelElement = CreateElement(xml, "mxGraphModel", new List<(string attributeName, string attributeValue)>
            {
                ("dx", "1000"),
                ("dy", "1000"),
                ("grid", "1"),
                ("gridSize", "10"),
                ("guides", "1"),
                ("connect", "1"),
                ("tooltips", "1"),
                ("arrows", "1"),
                ("fold", "1"),
                ("page", "1"),
                ("pageScale", "1"),
                ("pageWidth", "1000"),
                ("pageHeight", "1300"),
                ("math", "0"),
                ("shadow", "0"),
            });
            diagramElement.AppendChild(graphModelElement);

            var rootElement = CreateElement(xml, "root", new List<(string attributeName, string attributeValue)>());
            graphModelElement.AppendChild(rootElement);

            var firstCell = CreateElement(xml, "mxCell", new List<(string attributeName, string attributeValue)>
            {
                ("id", "0"),
            });
            var secondCell = CreateElement(xml, "mxCell", new List<(string attributeName, string attributeValue)>
            {
                ("id", "1"),
                ("parent", "0")
            });

            rootElement.AppendChild(firstCell);
            rootElement.AppendChild(secondCell);

            return rootElement;
        }

        private XmlElement CreateElement(XmlDocument document, string elementName, List<(string attributeName, string attributeValue)> attributes)
        {
            var newElement = document.CreateElement(elementName);
            attributes.ForEach(a => CreateAttr(document, newElement, a.attributeName, a.attributeValue));
            return newElement;
        }

        private void CreateAttr(XmlDocument xml, XmlElement element, string name, string value)
        {
            var attr = xml.CreateAttribute(name);
            attr.Value = value;
            element.Attributes.Append(attr);
        }

    }
}
