using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Engine.Models
{
    public class Process
    {
        internal IEnumerable<Property> Properties { get; set; }
        public XElement ProcessXML { get; set; }
        public XNamespace NS { get; set; }
        private Process()
        {
        }

        public Process(Stream bpmnStream)
        {
            XDocument doc = XDocument.Load(bpmnStream);
            NS = @"http://www.omg.org/spec/BPMN/20100524/MODEL";
            ProcessXML = doc.Root.Element(NS + "process");
            Properties = PropertyInitializer(ProcessXML, NS);
        }

        public ProcessInstance NewProcessInstance()
        {
            var current = ProcessXML.Element(NS + "startEvent");
            var node = new ProcessNode(current.Attribute("id").Value, current.Name.LocalName);
            var nodes = BuildNodes(ProcessXML);
            var processInstance = new ProcessInstance(this);
            BuildLinkedNodes(current, ref node, nodes, processInstance);
            processInstance.Id = Guid.NewGuid().ToString();
            processInstance.StartNode = node;
            processInstance.Nodes = nodes.ToImmutableDictionary();

            return processInstance;
        }

        private IDictionary<string, ProcessNode> BuildNodes(XElement processXML)
        {
            var nodes = processXML.Elements().ToDictionary(e => e.Attribute("id").Value, e => new ProcessNode(e.Attribute("id").Value, e.Name.LocalName));
            nodes.Where(e => e.Value.NodeType == "property").Select(e => e.Key).ToList().ForEach(k => nodes.Remove(k));
            var scripts = processXML.Elements().Elements(NS + "script")
                .Select(s => new { id = s.Parent.Attribute("id").Value, expression = s.Value });
            foreach (var s in scripts) nodes[s.id].Expression = s.expression;

            var conditionExpressions = processXML.Elements().Elements(NS + "conditionExpression")
                .Select(c => new { id = c.Parent.Attribute("id").Value, expression = c.Value });
            foreach (var c in conditionExpressions) nodes[c.id].Expression = c.expression;

            //Quick fix for zmq example
            //TODO Proper process var/assignment to node var mapping
            var taskExpressions = processXML.Elements(NS + "task").Elements(NS + "dataInputAssociation").Elements(NS + "assignment").Elements(NS + "from")
                .Select(e => new { id = e.Parent.Parent.Parent.Attribute("id").Value, expression = e.Value });
            foreach (var e in taskExpressions) nodes[e.id].Expression = e.expression;

            return nodes;
        }

        private Func<XElement, XElement, XNamespace, IEnumerable<XElement>> NextSequences =
            (e, ProcessXML, NS) => ProcessXML.Elements(NS + "sequenceFlow")?
            .Where(s => s.Attribute("sourceRef")?.Value == e.Attribute("id").Value);

        private Func<XElement, XElement, IEnumerable<XElement>> NextElement =
            (s, ProcessXML) => ProcessXML.Elements()
            .Where(e => e.Attribute("id").Value == s.Attribute("targetRef")?.Value);

        private void BuildLinkedNodes(XElement current, ref ProcessNode node, IDictionary<string, ProcessNode> nodes, ProcessInstance processInstance)
        {
            node.ProcessInstance = processInstance;
            var seq = NextSequences(current, ProcessXML, NS);
            var next = (seq.Any() ? seq : NextElement(current, ProcessXML));
            node.NextNodes = new List<ProcessNode>();

            foreach (var n in next)
            {
                var nextNode = nodes[n.Attribute("id").Value];
                if (nextNode.PreviousNodes == null) nextNode.PreviousNodes = new List<ProcessNode>();
                if (!nextNode.PreviousNodes.Contains(node)) nextNode.PreviousNodes.Add(node);
                node.NextNodes.Add(nextNode);
                BuildLinkedNodes(n, ref nextNode, nodes, processInstance);
            }
        }

        internal string GetAssociation(string nodeId, string nodeVariableName)
        {
            var node = ProcessXML.Elements().Where(e => e.Attribute("id").Value == nodeId);
            var inputId = node.Elements(NS + "ioSpecification").Elements(NS + "dataInput")
                .Where(e => e.Attribute("name").Value == nodeVariableName).FirstOrDefault().Attribute("id").Value;
            var propertyId = node.Elements(NS + "dataInputAssociation")
                .Where(d => d.Element(NS + "targetRef").Value == inputId).Elements(NS + "sourceRef").FirstOrDefault().Value;
            var propertyName = ProcessXML.Elements(NS + "property")
                .Where(e => e.Attribute("id").Value == propertyId).Attributes("name").FirstOrDefault().Value;
            return propertyName;
        }

        private IEnumerable<Property> PropertyInitializer(XElement process, XNamespace ns)
        {
            var itemDefinitions = process.Parent.Elements(ns + "itemDefinition");
            var properties = process.Elements(ns + "property").ToList();
            var propertyList = new List<Property>();
            foreach (var property in properties)
            {
                string id = property.Attribute("id").Value;
                string name = property.Attribute("name").Value;
                string itemSubjectRef = property.Attribute("itemSubjectRef").Value;
                string structureRef = itemDefinitions
                    .Where(i => i.Attribute("id").Value == itemSubjectRef)
                    .FirstOrDefault()
                    .Attribute("structureRef")
                    .Value;
                bool isCollection = Convert.ToBoolean(itemDefinitions
                    .Where(i => i.Attribute("id").Value == itemSubjectRef)
                    .FirstOrDefault()
                    .Attribute("isCollection")
                    .Value);
                propertyList.Add(new Property(id, name, structureRef, isCollection));
            }

            return propertyList;
        }
    }
}
