using Engine.Models;

namespace Engine.Interfaces
{
    public interface INodeHandler
    {
        void Execute(ProcessNode currentNode, ProcessNode previousNode);
    }
}
