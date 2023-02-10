using GraphProcessor;
namespace MyBehaviorTree
{
    [System.Serializable]
    [NodeName("���سɹ� \nReturn Success")]
    [NodeIcon("DarkReturnSuccessIcon.png")]
    [TaskNodeMenuItem("Decorator/Return Success")]
    public class ReturnSuccessNode : BaseDecoratorNode
    {
        public override TaskStatus Tick()
        {
            if (Child != null)
            {
                Child.DoTick();
            }
            return TaskStatus.Success;
        }
    }
}


