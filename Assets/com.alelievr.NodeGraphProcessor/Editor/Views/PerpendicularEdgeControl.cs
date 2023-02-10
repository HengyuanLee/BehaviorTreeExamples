using UnityEditor.Experimental.GraphView;

namespace GraphProcessor
{
    public class PerpendicularEdgeControl : EdgeControl
    {
        protected override void ComputeControlPoints()
        {
            base.ComputeControlPoints();
            if (controlPoints.Length == 4)
            {
                controlPoints[1].x = controlPoints[0].x;
                controlPoints[2].x = controlPoints[3].x;
                controlPoints[2].y = controlPoints[1].y;
            }
        }
    }
}
