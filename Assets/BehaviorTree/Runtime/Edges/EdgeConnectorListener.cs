using GraphProcessor;

namespace MyBehaviorTree
{
    public class EdgeConnectorListener : BaseEdgeConnectorListener
    {
        /// <summary>
        /// ���Ʋ��ܴ����ߴ���ͨ�ýڵ����ͣ�ֻ�ܴ�����Ϊ��ר���ģ���
        /// </summary>
        protected override bool CreateIncludeGenericNodes => false;

        public EdgeConnectorListener(BaseGraphView graphView) : base(graphView) { }
    }
}