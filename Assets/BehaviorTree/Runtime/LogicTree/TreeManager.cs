
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviorTree
{
    public class TreeManager : MonoBehaviour
    {
        public BehaviorTreeGraph graph;
        private IEntryNode entryNode;
        private TaskStatus resultTaskStatus = TaskStatus.Inactive;

        public ulong FrameIndex { get; private set; }//µ±Ç°Ö¡

        private void Awake()
        {
            graph.nodes.ForEach(x => (x as BaseTaskNode).ownerTreeManager = this );
            entryNode = graph.nodes.Find(e=>e is IEntryNode) as IEntryNode;
            if (entryNode != null)
            {
                entryNode.Awake();
            }
        }

        private void Update()
        {
            Excute();
        }

        public void Excute()
        {
            if (entryNode != null && resultTaskStatus == TaskStatus.Inactive || resultTaskStatus == TaskStatus.Running)
            {
                if (entryNode.CanDoTick())
                {
                    resultTaskStatus = entryNode.DoTick();
                    FrameIndex++;
                }
            }
        }
    }
}
