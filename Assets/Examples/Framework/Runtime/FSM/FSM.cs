
using System;
using System.Collections.Generic;

namespace AppFramework
{
    public class FSM : IFSM
    {
        public string Name => "";
        public bool IsRunning { get; set; }

        public FSMManager BelongFSMManager { get; private set; }

        private Dictionary<Type, IFSMState> m_StateDict = new Dictionary<Type, IFSMState>();
        private IFSMState m_CurState;

        public void AddState<T>()where T : IFSMState, new() {
            lock (m_StateDict) {
                if (m_StateDict.ContainsKey(typeof(T)) == false)
                {
                    T state = new T();
                    m_StateDict.Add(typeof(T), state);
                    state.OnAdd(this);
                    state.OnInit();
                }
                else {
                    throw new Exception("已存在的FSM状态: "+typeof(T));
                }
            }
        }
        public void RemoveState<T>()where T :IFSMState{
            lock (m_StateDict) {
                if (m_StateDict.ContainsKey(typeof(T)))
                {
                    m_StateDict[typeof(T)].OnRemove();
                    m_StateDict.Remove(typeof(T));
                }
                else {
                    throw new Exception("不存在的状态机："+typeof(T));
                }
            }
        }
        public void EnterState<T>() where T : IFSMState
        {
            if (m_CurState != null) {
                m_CurState.OnExit();
            }
            m_CurState = null;
            if (m_StateDict.TryGetValue(typeof(T), out IFSMState outState))
            {
                m_CurState = outState;
                outState.OnEnter();
            }
            else {
                throw new Exception("不存在状态："+typeof(T));
            }
        }

        public void OnUpdate()
        {
            if (m_CurState != null) {
                m_CurState.OnUpdate();
            }
        }

        public void Shutdown()
        {
            IsRunning = false;
        }

        public void Startup()
        {
            IsRunning = true;
        }

        public void OnRegister(FSMManager fsmManager)
        {
            BelongFSMManager = fsmManager;
        }

        public void OnRemove()
        {
        }
    }
}
