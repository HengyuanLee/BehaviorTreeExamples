using GraphProcessor;
using UnityEditor.Experimental.GraphView;

namespace MyBehaviorTree
{
    public class BTEdgeView : EdgeView
    {
        protected override EdgeControl CreateEdgeControl()
        {
            return new PerpendicularEdgeControl();
        }
    }
}
