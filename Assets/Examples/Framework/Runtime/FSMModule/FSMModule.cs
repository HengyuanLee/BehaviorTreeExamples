
namespace AppFramework
{
    public partial class FSMModule : AppModuleBase
    {
        private FSMManager m_FSMManager = new FSMManager();


        public void Register(IFSM fsm) {
            m_FSMManager.Register(fsm);
        }
        public void Remove(IFSM fsm) {
            m_FSMManager.Remove(fsm);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            m_FSMManager.OnUpdate();
        }
    }
}
