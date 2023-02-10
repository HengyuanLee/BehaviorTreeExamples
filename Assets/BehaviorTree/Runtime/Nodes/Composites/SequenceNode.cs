using GraphProcessor;

namespace MyBehaviorTree
{
    [System.Serializable]
    [NodeName("���� \nSequence")]
    [NodeIcon("DarkSequenceIcon.png")]
    [TaskNodeMenuItem("Composite/Sequence")]
    public class SequenceNode : BaseCompositeNode
    {
        /// <summary>
        /// ������˳��ִ����Task��ֱ����һ����Task����Failureʱֹͣ��������Failure��
        /// ������Taskִ����û��һ������Failureʱ������Success��
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

