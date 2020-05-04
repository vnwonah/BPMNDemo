using Engine.Interfaces;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class ProcessNode
    {
        public string NodeName { get; set; }
        public string NodeType { get; set; }
        public ProcessInstance ProcessInstance { get; set; }
        public IImmutableDictionary<string, object> InputParameters { get; set; }
        public IImmutableDictionary<string, object> OutputParameters { get; set; }
        public INodeHandler NodeHandler { get; set; }
        public ICollection<ProcessNode> NextNodes { get; set; }
        public ICollection<ProcessNode> PreviousNodes { get; set; }
        private Task Task { get; set; }
        public string Expression { get; set; }

        public ProcessNode()
        {
        }

        public ProcessNode(INodeHandler nodeHandler)
        {
            NodeHandler = nodeHandler;
        }

        public ProcessNode(string name, string type)
        {
            NodeName = name;
            NodeType = type;
        }

        public void Execute(ProcessNode processNode, ProcessNode previousNode)
        {
            NodeHandler = ProcessInstance.NodeHandlers[NodeType];
            if (processNode.InputParameters == null) processNode.InputParameters = ProcessInstance.InputParameters;
            Task = new Task(() => NodeHandler.Execute(processNode, previousNode));
            Task.Start();
        }
        public void Done()
        {
            foreach (var node in NextNodes)
            {
                //to replace with variable resolution
                //for each node retrieve input parameters defined in BPMN
                //retrieve from node.OutputParameters (results of previous node)
                //retrieve missing necessary input from process variables
                node.InputParameters = OutputParameters;
                node.Execute(node, this);
            }
        }
    }

}
