using UnityEditor.Experimental.GraphView;
using GraphProcessor;

namespace MyBehaviourTree
{

    public class PerpendicularEdgeControl : EdgeControl
    {
        public EdgeView owner;

        public PerpendicularEdgeControl(EdgeView owner)
        {
            this.owner = owner;
        }

        protected override void ComputeControlPoints()
        {
            base.ComputeControlPoints();

        }
    }
}
