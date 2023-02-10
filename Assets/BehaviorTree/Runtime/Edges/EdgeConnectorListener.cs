using GraphProcessor;

namespace MyBehaviorTree
{
    public class EdgeConnectorListener : BaseEdgeConnectorListener
    {
        /// <summary>
        /// 控制不能从拉线创建通用节点类型（只能创建行为树专属的）。
        /// </summary>
        protected override bool CreateIncludeGenericNodes => false;

        public EdgeConnectorListener(BaseGraphView graphView) : base(graphView) { }
    }
}