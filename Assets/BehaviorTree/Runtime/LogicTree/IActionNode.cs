
namespace MyBehaviorTree
{

    public interface IActionNode : ITaskNode
    {
        float GetUtility();
    }
}
