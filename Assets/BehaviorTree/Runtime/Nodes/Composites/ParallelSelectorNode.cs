using GraphProcessor;

namespace MyBehaviorTree
{
    [System.Serializable]
    [NodeName("并行选择 \nParallel Selector")]
    [NodeIcon("DarkParallelSelectorIcon.png")]
    [TaskNodeMenuItem("Composite/Parallel Selector")]
    public class ParallelSelectorNode : BaseCompositeNode
    {
        /// <summary>
        /// 按队列顺序执行子Task，先执行完所有子Task，再检查所有执行完的子Task。
        /// 有一个子Task返回Success时返回Success。
        /// 没有子Task返回Success时返回Failure。
        /// </summary>
        public override TaskStatus Tick()
        {
            TaskStatus result = TaskStatus.Failure;
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
                    if (status == TaskStatus.Running)
                    {
                        return TaskStatus.Running;
                    }
                    else if (status == TaskStatus.Success)
                    {
                        result = TaskStatus.Success;
                    }
                }
            }
            return result;
        }
    }
}