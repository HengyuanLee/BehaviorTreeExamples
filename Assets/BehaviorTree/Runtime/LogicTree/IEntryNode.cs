
namespace MyBehaviorTree
{

    public interface IEntryNode : ITaskNode
    {
        ITaskNode Child { get; }
    }
}
