using Engine.Interfaces;
using Engine.Models;
using System;

namespace Engine.Handlers
{
    internal class DefaultStartHandler : INodeHandler
    {
        void INodeHandler.Execute(ProcessNode processNode, ProcessNode previousNode)
        {
            Console.WriteLine(processNode.NodeName + " Executing Start");
            processNode.Done();
        }
    }
}
