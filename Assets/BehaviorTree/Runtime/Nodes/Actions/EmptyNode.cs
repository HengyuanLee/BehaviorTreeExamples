using GraphProcessor;
using UnityEngine;

namespace MyBehaviorTree
{
    [System.Serializable]
    [TaskNodeMenuItem("Action/Empty")]
    //[NodeIcon("DarkLogIcon.png")]
    [NodeName("�սڵ�")]
    public class EmptyNode : BaseActionNode
    {
        public TaskStatus returnTaskStatus = TaskStatus.Success;

        public override TaskStatus Tick()
        {
            return returnTaskStatus;
        }
    }
}
