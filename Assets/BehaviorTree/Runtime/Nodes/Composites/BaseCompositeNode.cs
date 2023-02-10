
using GraphProcessor;

namespace MyBehaviorTree
{
    [System.Serializable]
    [NodeColor(0.1f, 0.3f, 0.7f)]
    public abstract class BaseCompositeNode : BaseMultipleOutNode
    {
        public AbortType abortType;
        [Input("", false), Vertical]
        public TaskStatus inTaskStatus;

        [System.NonSerialized]
        protected int runningChildIndex = 0;

        protected ITaskNode CurrentNode => ChildAtIndex(runningChildIndex);

        protected override void BeforeTick()
        {
            if (LastReturnTaskStatus == TaskStatus.Running)
            {
                //���ͬ��LowerPriority������ͬ�����child��ִֻ�ж���AbortType��Composite�ڵ㡣
                for (int i = 0; i < runningChildIndex; i++)
                {
                    var childNode = ChildAtIndex(i);
                    //������Ϊfalse
                    (childNode as BaseTaskNode).canDoTick = false;
                    if (childNode is BaseCompositeNode compositeNode)
                    {
                        //Switch����child��AbortType��
                        switch (compositeNode.abortType)
                        {
                            case AbortType.LowerPriority://���Դ��ͬ����
                            case AbortType.Both:
                                compositeNode.canDoTick = true;
                                break;
                        }
                    }
                    //Switch���ǵ�ǰ��Ͻڵ��AbortType��
                    switch (abortType)
                    {
                        case AbortType.Self://�Լ��ڲ����Դ��ͬ����
                        case AbortType.Both:
                            (childNode as BaseTaskNode).canDoTick = true;
                            break;
                    }
                }
            }
            runningChildIndex = 0;
        }
        protected override void AfterTick()
        {
            if (LastReturnTaskStatus != TaskStatus.Running)
            {
                //�ϴν������Running��˵��ִ�����ˣ��������ò����ˡ�
                for (int i = 0; i < ChildCount; i++)
                {
                    var child = ChildAtIndex(i);
                    if (child is BaseTaskNode childNode)
                    {
                        //childNode.ResetStatus();
                        childNode.canDoTick = true;
                    }
                }
                runningChildIndex = 0;
            }
        }
    }
}
