using GraphProcessor;

namespace MyBehaviorTree
{
    [System.Serializable]
    [NodeName("����ѡ�� \nParallel Selector")]
    [NodeIcon("DarkParallelSelectorIcon.png")]
    [TaskNodeMenuItem("Composite/Parallel Selector")]
    public class ParallelSelectorNode : BaseCompositeNode
    {
        /// <summary>
        /// ������˳��ִ����Task����ִ����������Task���ټ������ִ�������Task��
        /// ��һ����Task����Successʱ����Success��
        /// û����Task����Successʱ����Failure��
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