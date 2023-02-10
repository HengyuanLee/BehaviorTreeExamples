using GraphProcessor;

namespace MyBehaviorTree
{
    [System.Serializable]
    [NodeName("优先选择 \nPriority Selector")]
    [NodeIcon("DarkPrioritySelectorIcon.png")]
    [TaskNodeMenuItem("Composite/Priority Selector")]
    public class PrioritySelectroNode : BaseCompositeNode
    {
        //就是用来打乱从左到右执行顺序的，先不管你。
        //在使用过程中笔者发现其官方文档对Priority Selector和Selector Evaluator两个组件的使用只做了概念性说明，未对其中说到的Priority进行详细阐述。
        //经过研究，原来每个Task都存在Priority参数，可通过重写GetPriority()函数设置Priority值，以上两个组件就可以根据Task的Priority参数决定每个Task的运行优先级。
    }
}