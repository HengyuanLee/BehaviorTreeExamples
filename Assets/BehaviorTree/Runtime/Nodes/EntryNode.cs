using GraphProcessor;
using GraphProcessor.Custom;
using UnityEngine;

namespace MyBehaviorTree
{
    [System.Serializable]
    [TaskNodeMenuItem("Entry")]
    [NodeIcon("DarkEntryIcon.png")]
    [NodeName("Èë¿Ú")]
    [NodeColor(0f, 1f, 0f)]
    public class EntryNode : BaseSingleOutNode, IEntryNode
    {
        public override TaskStatus Tick()
        {
            if (Child != null && Child.CanDoTick())
            {
                return Child.DoTick();
            }
            return TaskStatus.Success;
        }
    }
}
