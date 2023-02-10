
using System.Collections.Generic;

namespace AppFramework
{
    public class FSMManager
    {
        private List<IFSM> m_FSMList = new List<IFSM>();

        public void Register(IFSM fsm) {
            CheckFSMIsValid(fsm);
            lock (m_FSMList) {
                m_FSMList.Add(fsm);
            }
        }
        public void Remove(IFSM fsm) {
            CheckFSMIsValid(fsm);
            lock (m_FSMList) {
                for (int i = 0; i < m_FSMList.Count; i++) {
                    if (ReferenceEquals(m_FSMList[i], fsm)) {
                        m_FSMList.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        private void CheckFSMIsValid(IFSM fsm) {
            if (fsm == null) {
                throw new System.Exception("fms为空！");
            }
        }
        public void OnUpdate() {
            for (int i = 0; i < m_FSMList.Count; i++)
            {
                if (m_FSMList[i].IsRunning)
                {
                    m_FSMList[i].OnUpdate();
                }
            }
        }
    }
}
