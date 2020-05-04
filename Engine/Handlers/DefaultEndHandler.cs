using Engine.Interfaces;
using Engine.Models;
using System;

namespace Engine.Handlers
{
    internal class DefaultEndHandler : INodeHandler
    {
        void INodeHandler.Execute(ProcessNode processNode, ProcessNode previousNode)
        {
            Console.WriteLine(processNode.NodeName + " Executing End");
            processNode.ProcessInstance.SetOutputParameters(processNode);
            processNode.Done();
        }
    }
}
