

using GraphProcessor;

namespace MyBehaviorTree
{

    public abstract class BaseSingleOutNode : BaseTaskNode
    {
        [Output("", false), Vertical]
        public TaskStatus outTaskStatus;

        public ITaskNode Child {
            get {
                if (outputPorts.Count > 0)
                {
                    var edges = outputPorts[0].GetEdges();
                    if (edges.Count > 0)
                    {
                        return edges[0].inputNode as ITaskNode;
                    }
                }
                return null;
            }
        }
    }
}
