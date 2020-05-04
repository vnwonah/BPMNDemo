using Engine.Interfaces;
using Engine.Models;
using System;

namespace Engine.Handlers
{
    internal class DefaultExclusiveGatewayHandler : INodeHandler
    {
        void INodeHandler.Execute(ProcessNode processNode, ProcessNode previousNode)
        {
            Console.WriteLine(processNode.NodeName);
            processNode.Done();
        }
    }
}
