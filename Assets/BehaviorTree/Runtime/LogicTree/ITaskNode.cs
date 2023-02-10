

namespace MyBehaviorTree
{
    public interface ITaskNode
    {
        TaskStatus DoTick();
        void Awake();
        void Start();
        bool CanDoTick();
        TaskStatus Tick();
        float Priority { get; }
    }
}
