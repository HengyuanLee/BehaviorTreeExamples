using GraphProcessor;

namespace MyBehaviorTree
{
    [System.Serializable]
    [NodeName("����ѡ�� \nPriority Selector")]
    [NodeIcon("DarkPrioritySelectorIcon.png")]
    [TaskNodeMenuItem("Composite/Priority Selector")]
    public class PrioritySelectroNode : BaseCompositeNode
    {
        //�����������Ҵ�����ִ��˳��ģ��Ȳ����㡣
        //��ʹ�ù����б��߷�����ٷ��ĵ���Priority Selector��Selector Evaluator���������ʹ��ֻ���˸�����˵����δ������˵����Priority������ϸ������
        //�����о���ԭ��ÿ��Task������Priority��������ͨ����дGetPriority()��������Priorityֵ��������������Ϳ��Ը���Task��Priority��������ÿ��Task���������ȼ���
    }
}