using GraphProcessor;
namespace MyBehaviorTree
{
    [System.Serializable]
    [NodeName("����ʧ�� \nReturn Failure")]
    [NodeIcon("DarkReturnFailureIcon.png")]
    [TaskNodeMenuItem("Decorator/Return Failure")]
    public class ReturnFailureNode : BaseDecoratorNode
    {
        public override TaskStatus Tick()
        {
            if (Child != null)
            {
                Child.DoTick();
            }
            return TaskStatus.Failure;
        }
    }
}


