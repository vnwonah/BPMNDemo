using Engine.Interfaces;
using Engine.Models;
using System;

namespace Engine.Handlers
{
    internal class DefaultBusinessRuleHandler : INodeHandler
    {
        void INodeHandler.Execute(ProcessNode processNode, ProcessNode previousNode)
        {
            Console.WriteLine(processNode.NodeName + " Executing BusinessRule");
            processNode.Done();
        }
    }
}
