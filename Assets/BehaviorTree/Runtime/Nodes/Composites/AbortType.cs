
namespace MyBehaviorTree
{
    public enum AbortType
    {
        None,
        LowerPriority,//ֻҪ��ǰͬ�ȼ�Running��Task���ҵͣ��ͼ���������
        Self,//ֻҪ��ǰ����ͬ��task����Running���ͼ���������
        Both
    }
}