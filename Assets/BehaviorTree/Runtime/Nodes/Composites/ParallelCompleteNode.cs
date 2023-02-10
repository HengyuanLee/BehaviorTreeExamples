
using GraphProcessor;

namespace MyBehaviorTree
{
    [System.Serializable]
    [NodeName("并行完成 \nParallel Complete")]
    [NodeIcon("DarkParallelCompleteIcon.png")]
    [TaskNodeMenuItem("Composite/Parallel Complete")]
    public class ParallelCompleteNode : BaseCompositeNode
    {
        /// <summary>
        /// 按队列顺序执行子Task，先执行完所有子Task，再检查所有执行完的子Task。
        /// 从左到右检查，遇到Failure或Success就直接返回Failure或Success。
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
                    if (runningChildIndex == 0)
                    {
                        if (status == TaskStatus.Success || status == TaskStatus.Failure)
                        {
                            //记录第一个Success或Failure的结果。
                            result = status;
                        }
                    }
                }
            }
            return result;
        }
    }
}