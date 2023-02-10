using GraphProcessor;
using System;

namespace MyBehaviorTree
{
    public enum SortChildrenType
    {
        Default,
        Priority,
        Random
    }

    public class BaseMultipleOutNode : BaseTaskNode
    {
        [Output("", true), Vertical]
        public TaskStatus outTaskStatus;
        protected virtual SortChildrenType SortChildrenType => SortChildrenType.Default;
        protected int ChildCount {
            get {
                if (outputPorts.Count > 0)
                {
                    return outputPorts[0].GetEdges().Count;
                }
                return 0;
            }
        }

        protected ITaskNode ChildAtIndex(int index)
        {
            if (outputPorts.Count > 0)
            {
                var edges = outputPorts[0].GetEdges();
                if (edges.Count > index)
                {
                    return edges[index].inputNode as ITaskNode;
                }
            }
            return null;
        }
        public override void Awake()
        {
            SortChildren(SortChildrenType);
        }
        private void SortChildren(SortChildrenType sort)
        {
            if (outputPorts.Count > 0)
            {
                var edges = outputPorts[0].GetEdges();
                switch (sort)
                {
                    case SortChildrenType.Default:
                        edges.Sort((e1, e2) => e1.inputNode.position.x < e2.inputNode.position.x ? -1 : 1);
                        break;
                    case SortChildrenType.Priority:
                        edges.Sort((e1, e2) => {
                            var n1 = e1.inputNode as ITaskNode;
                            var n2 = e2.inputNode as ITaskNode;
                            if (n1.Priority != n2.Priority)
                            {
                                return -n1.Priority.CompareTo(n2.Priority);
                            }
                            else
                            {
                                return e1.inputNode.position.x.CompareTo(e2.inputNode.position.x);
                            }
                            });
                        break;
                    case SortChildrenType.Random:
                        int[] indices = new int[] { -1, 1};
                        edges.Sort((e1, e2) =>  UnityEngine.Random.Range(0, indices.Length));
                        break;
                }
            }
        }
    }
}
