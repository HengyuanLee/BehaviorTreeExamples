
using GraphProcessor;

namespace MyBehaviorTree
{
    [NodeColor(0.4f, 0.8f, 0.4f)]
    public abstract class BaseActionNode : BaseTaskNode, IActionNode
    {
        [Input, Vertical]
        public TaskStatus status;


        public virtual float GetUtility()
        {
            return 0;
        }
    }
}