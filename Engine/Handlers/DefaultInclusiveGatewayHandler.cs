using Engine.Interfaces;
using Engine.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Engine.Handlers
{
    internal class DefaultInclusiveGatewayHandler : INodeHandler
    {
        ConcurrentDictionary<ProcessNode, ICollection<ProcessNode>> sequenceWait = new ConcurrentDictionary<ProcessNode, ICollection<ProcessNode>>();

        void INodeHandler.Execute(ProcessNode processNode, ProcessNode previousNode)
        {
            Console.WriteLine(processNode.NodeName);
            sequenceWait.GetOrAdd(processNode, new List<ProcessNode>(processNode.PreviousNodes));
            lock (sequenceWait[processNode])
            {
                sequenceWait[processNode].Remove(previousNode);
            }
            if (sequenceWait[processNode].Count == 0)
            {
                processNode.Done();
            }
        }
    }
}
