
using GraphProcessor;

namespace MyBehaviorTree
{
    [System.Serializable]
    [NodeName("���� \nParallel")]
    [NodeIcon("DarkParallelIcon.png")]
    [TaskNodeMenuItem("Composite/Parallel")]
    public class ParallelNode : BaseCompositeNode
    {
        /// <summary>
        /// ������˳��ִ����Task����ִ����������Task���ټ������ִ�������Task��
        /// ��һ����Task����Failureʱ����Failure��
        /// û����Task����Failureʱ����Success��
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