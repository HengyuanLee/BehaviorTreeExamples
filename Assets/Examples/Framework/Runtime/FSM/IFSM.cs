
namespace AppFramework
{
    public interface IFSM
    {
        string Name { get; }
        bool IsRunning { get; }
        FSMManager BelongFSMManager { get; }
        void OnRegister(FSMManager fsmManager);
        void OnRemove();
        void EnterState<T>()where T : IFSMState;
        void Startup();
        void OnUpdate();
        void Shutdown();
    }
}
