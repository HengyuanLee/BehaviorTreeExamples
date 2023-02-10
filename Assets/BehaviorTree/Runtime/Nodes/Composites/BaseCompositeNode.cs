
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
                //针对同级LowerPriority，对于同级别的child，只执行对于AbortType的Composite节点。
                for (int i = 0; i < runningChildIndex; i++)
                {
                    var childNode = ChildAtIndex(i);
                    //先重置为false
                    (childNode as BaseTaskNode).canDoTick = false;
                    if (childNode is BaseCompositeNode compositeNode)
                    {
                        //Switch的是child的AbortType。
                        switch (compositeNode.abortType)
                        {
                            case AbortType.LowerPriority://可以打断同级的
                            case AbortType.Both:
                                compositeNode.canDoTick = true;
                                break;
                        }
                    }
                    //Switch的是当前组合节点的AbortType。
                    switch (abortType)
                    {
                        case AbortType.Self://自己内部可以打断同级的
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
                //上次结果不是Running，说明执行完了，可以重置参数了。
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
