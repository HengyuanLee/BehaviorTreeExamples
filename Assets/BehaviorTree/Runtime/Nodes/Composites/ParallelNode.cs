
using GraphProcessor;

namespace MyBehaviorTree
{
    [System.Serializable]
    [NodeName("并行 \nParallel")]
    [NodeIcon("DarkParallelIcon.png")]
    [TaskNodeMenuItem("Composite/Parallel")]
    public class ParallelNode : BaseCompositeNode
    {
        /// <summary>
        /// 按队列顺序执行子Task，先执行完所有子Task，再检查所有执行完的子Task。
        /// 有一个子Task返回Failure时返回Failure。
        /// 没有子Task返回Failure时返回Success。
        /// </summary>
        public override TaskStatus Tick()
        {
            TaskStatus result = TaskStatus.Success;
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
                    else if (status == TaskStatus.Failure)
                    {
                        result = TaskStatus.Failure;
                    }
                }
            }
            return result;
        }
    }
}