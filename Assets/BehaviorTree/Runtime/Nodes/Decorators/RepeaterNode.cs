using GraphProcessor;
namespace MyBehaviorTree
{
    [System.Serializable]
    [NodeName("÷ÿ∏¥ \nRepeater")]
    [NodeIcon("DarkRepeaterIcon.png")]
    [TaskNodeMenuItem("Decorator/Repeater")]
    public class RepeaterNode : BaseDecoratorNode
    {
        public override TaskStatus Tick()
        {
            if (Child != null)
            {
                Child.DoTick();
            }
            return TaskStatus.Running;
        }
    }
}


