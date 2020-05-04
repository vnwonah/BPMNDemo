using Engine.Interfaces;
using Engine.Models;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Engine.Handlers
{
    internal class DefaultScriptTaskHandler : INodeHandler
    {
        void INodeHandler.Execute(ProcessNode processNode, ProcessNode previousNode)
        {
            Console.WriteLine(processNode.NodeName + " Executing Script");

            if (processNode.Expression != null)
            {
                Console.WriteLine("Script: " + processNode.Expression);
                var globals = new Globals(processNode.InputParameters.ToDictionary(e => e.Key, e => e.Value));
                try
                {
                    processNode.OutputParameters =
                        CSharpScript.EvaluateAsync<IDictionary<string, object>>(processNode.Expression, globals: globals)
                        .Result.ToImmutableDictionary();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            processNode.Done();
        }
    }
}
