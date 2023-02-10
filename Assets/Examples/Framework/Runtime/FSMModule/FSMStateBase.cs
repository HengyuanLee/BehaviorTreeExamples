using System;

namespace AppFramework
{
    public abstract class FSMStateBase : IFSMState
    {
        public IFSM OwnerFSM { get; private set; }

        public void OnAdd(IFSM belongFSM)
        {
            OwnerFSM = belongFSM;
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual void OnInit()
        {
        }

        public virtual void OnRemove()
        {
        }

        public virtual void OnUpdate()
        {
        }
    }
}
