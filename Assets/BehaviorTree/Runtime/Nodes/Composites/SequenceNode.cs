using GraphProcessor;

namespace MyBehaviorTree
{
    [System.Serializable]
    [NodeName("队列 \nSequence")]
    [NodeIcon("DarkSequenceIcon.png")]
    [TaskNodeMenuItem("Composite/Sequence")]
    public class SequenceNode : BaseCompositeNode
    {
        /// <summary>
        /// 按队列顺序执行子Task，直到有一个子Task返回Failure时停止，并返回Failure。
        /// 所有子Task执行完没有一个返回Failure时，返回Success。
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
                    if (status == TaskStatus.Failure || status == TaskStatus.Running)
                    {
                        return status;
                    }
                }
            }
            return TaskStatus.Success;
        }
    }
}

