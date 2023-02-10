using GraphProcessor;

namespace MyBehaviorTree
{
    [System.Serializable]
    [NodeName("效能选择 \nUtility Selector")]
    [NodeIcon("DarkUtilitySelectorIcon.png")]
    [TaskNodeMenuItem("Composite/UtilitySelector")]
    public class UtilitySelectorNode : BaseCompositeNode
    {
        public override TaskStatus Tick()
        {
            SortChildrenByUtility();
            for (int i = 0; i < ChildCount; i++)
            {
                var child = ChildAtIndex(i);
                if (!child.CanDoTick())
                {
                    continue;
                }
                return child.Tick();
            }
            return TaskStatus.Success;
        }
        /// <summary>
        /// 总是将Utility最高的放前面。
        /// </summary>
        private void SortChildrenByUtility()
        {
            if (outputPorts.Count > 0)
            {
                var edges = outputPorts[0].GetEdges();
                edges.Sort((e1, e2) => {
                    if (e1 is IActionNode a1 && e2 is IActionNode a2)
                    {
                        return -a1.GetUtility().CompareTo(a2.GetUtility());
                    }
                    else if (e1 is IActionNode)
                    {
                        return -1;
                    }
                    else if (e2 is IActionNode)
                    {
                        return 1;
                    }
                    else {
                        return e1.inputNode.position.x.CompareTo(e2.inputNode.position.x);
                    }
                });
            }
        }
    }
}


