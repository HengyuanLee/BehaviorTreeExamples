
using GraphProcessor;

namespace MyBehaviorTree
{
    [System.Serializable]
    [NodeName("ѡ�� \nSelector")]
    [NodeIcon("DarkSelectorIcon.png")]
    [TaskNodeMenuItem("Composite/Selector")]
    public class SelectorNode : BaseCompositeNode
    {
        /// <summary>
        /// ������˳��ִ����Task��ֱ����һ����Task����Successʱֹͣ��������Success��
        /// ������Taskִ����û��һ������Successʱ������Failure��
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

