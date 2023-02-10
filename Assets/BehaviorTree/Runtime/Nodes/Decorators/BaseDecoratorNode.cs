
using GraphProcessor;

namespace MyBehaviorTree
{
    [System.Serializable]
    [NodeColor(0.7f, 0.3f, 0.1f)]
    public abstract class BaseDecoratorNode : BaseSingleOutNode
    {
        [Input("", false), Vertical]
        public TaskStatus inTaskStatus;
    }
}