using Engine.Interfaces;
using Engine.Models;
using System;

namespace Engine.Handlers
{
    internal class DefaultTaskHandler : INodeHandler
    {
        public void Execute(ProcessNode processNode, ProcessNode previousNode)
        {
            Console.WriteLine(processNode.NodeName + " Executing Task");
            processNode.Done();
        }

    }
}
