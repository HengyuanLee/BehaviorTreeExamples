
using GraphProcessor;

namespace MyBehaviorTree
{
    [System.Serializable]
    [NodeName("������� \nParallel Complete")]
    [NodeIcon("DarkParallelCompleteIcon.png")]
    [TaskNodeMenuItem("Composite/Parallel Complete")]
    public class ParallelCompleteNode : BaseCompositeNode
    {
        /// <summary>
        /// ������˳��ִ����Task����ִ����������Task���ټ������ִ�������Task��
        /// �����Ҽ�飬����Failure��Success��ֱ�ӷ���Failure��Success��
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
                            //��¼��һ��Success��Failure�Ľ����
                            result = status;
                        }
                    }
                }
            }
            return result;
        }
    }
}