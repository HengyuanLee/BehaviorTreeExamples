using GraphProcessor;

namespace MyBehaviorTree
{
    [System.Serializable]
    [NodeName("���ѡ�� \nRandom Selector")]
    [NodeIcon("DarkRandomSelectorIcon.png")]
    [TaskNodeMenuItem("Composite/Random Selector")]
    public class RandomSelectorNode : SelectorNode
    {
        protected override SortChildrenType SortChildrenType => SortChildrenType.Random;
    }
}