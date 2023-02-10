
using GraphProcessor;

namespace MyBehaviorTree
{
    [System.Serializable]
    [NodeName("选择 \nSelector")]
    [NodeIcon("DarkSelectorIcon.png")]
    [TaskNodeMenuItem("Composite/Selector")]
    public class SelectorNode : BaseCompositeNode
    {
        /// <summary>
        /// 按队列顺序执行子Task，直到有一个子Task返回Success时停止，并返回Success。
        /// 所有子Task执行完没有一个返回Success时，返回Failure。
        /// </summary>
        public override TaskStatus Tick()
        {
            for (; runningChildIndex < ChildCount; runningChildIndex++)
            {
                var child = ChildAtIndex(runningChildIndex);
                if (child != null)
                {
                    if (!child.CanDoTick())
                    {
                        continue; 
                    }
                    TaskStatus status = child.DoTick();
                    if (status == TaskStatus.Success || status == TaskStatus.Running)
                    {
                        return status;
                    }
                }
            }
            return TaskStatus.Failure;
        }
    }
}

