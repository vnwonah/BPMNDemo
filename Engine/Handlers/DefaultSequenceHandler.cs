using Engine.Interfaces;
using Engine.Models;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Handlers
{
    internal class DefaultSequenceHandler : INodeHandler
    {
        void INodeHandler.Execute(ProcessNode processNode, ProcessNode previousNode)
        {
            Console.WriteLine(processNode.NodeName + " Executing Sequence");
            bool result = true;
            if (processNode.Expression != null)
            {
                Console.WriteLine(processNode.NodeName + " Conditional Sequence");
                Console.WriteLine("Condition: " + processNode.Expression);
                var globals = new Globals(processNode.InputParameters.ToDictionary(e => e.Key, e => e.Value));
                try
                {
                    result = CSharpScript.EvaluateAsync<bool>(processNode.Expression, globals: globals).Result;
                    Console.WriteLine("Condition result: " + result.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            if (result)
            {
                processNode.Done();
            }
        }
    }
}
