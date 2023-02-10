
using GraphProcessor;

namespace MyBehaviorTree
{
    [System.Serializable]
    [NodeName("������� \nRandom Sequence")]
    [NodeIcon("DarkRandomSequenceIcon.png")]
    [TaskNodeMenuItem("Composite/Random Sequence")]
    public class RandomSequenceNode : SequenceNode
    {
        protected override SortChildrenType SortChildrenType => SortChildrenType.Random;
    }
}