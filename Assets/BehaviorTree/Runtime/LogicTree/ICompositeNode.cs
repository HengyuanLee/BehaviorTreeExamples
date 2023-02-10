
namespace MyBehaviorTree
{
    public interface ICompositeNode : ITaskNode
    {
        ITaskNode GetChild(int index);
    }
}
