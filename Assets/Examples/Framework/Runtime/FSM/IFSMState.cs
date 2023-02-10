namespace AppFramework
{
    /// <summary>
    /// 状态机状态
    /// </summary>
    public interface IFSMState
    {
        void OnInit();
        void OnAdd(IFSM belongFms);
        void OnRemove();
        void OnEnter();
        void OnUpdate();
        void OnExit();
        /// <summary>
        /// 所属状态机
        /// </summary>
        IFSM OwnerFSM { get; }
    }
}
