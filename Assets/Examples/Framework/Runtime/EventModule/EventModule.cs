using System;
using System.Collections.Generic;

namespace AppFramework{
    public interface IEventHandler
    {
        void OnAdd();
        void OnRemove();
    }
    public partial class EventModule : AppModuleBase
    {

        private readonly EventManager m_EventManager = new EventManager();
        private Dictionary<Type, IEventHandler> m_EventHandlers = new Dictionary<Type, IEventHandler>();

        public void AddEventHandler<T>() where T : IEventHandler, new()
        {
            T t = new T();
            m_EventHandlers.Add(typeof(T), t);
            t.OnAdd();
        }
        public void RemoveEventHandler<T>() where T : IEventHandler
        {
            if (m_EventHandlers.TryGetValue(typeof(T), out IEventHandler handler))
            {
                m_EventHandlers.Remove(typeof(T));
                handler.OnRemove();
            }
        }
        public void Send(EventDef eventDef)
        {
            m_EventManager.Send(eventDef);
        }
        public void Send<T>(EventDef eventDef, T t)
        {
            m_EventManager.Send(eventDef, t);
        }
        public void Send<T1, T2>(EventDef eventDef, T1 t1, T2 t2)
        {
            m_EventManager.Send(eventDef, t1, t2);
        }
        public void Send<T1, T2, T3>(EventDef eventDef, T1 t1, T2 t2, T3 t3)
        {
            m_EventManager.Send(eventDef, t1, t2, t3);
        }

        public void AddListener(EventDef eventDef, Action action)
        {
            m_EventManager.Register(eventDef, action);
        }
        public void AddListener<T>(EventDef eventDef, Action<T> action)
        {
            m_EventManager.Register(eventDef, action);
        }
        public void AddListener<T1, T2>(EventDef eventDef, Action<T1, T2> action)
        {
            m_EventManager.Register(eventDef, action);
        }
        public void AddListener<T1, T2, T3>(EventDef eventDef, Action<T1, T2, T3> action)
        {
            m_EventManager.Register(eventDef, action);
        }

        public void RemoveListener(EventDef eventDef, Action action)
        {
            m_EventManager.RemoveListener(eventDef, action);
        }
        public void RemoveListener<T>(EventDef eventDef, Action<T> action)
        {
            m_EventManager.RemoveListener(eventDef, action);
        }
        public void RemoveListener<T1, T2>(EventDef eventDef, Action<T1, T2> action)
        {
            m_EventManager.RemoveListener(eventDef, action);
        }
        public void RemoveListener<T1, T2, T3>(EventDef eventDef, Action<T1, T2, T3> action)
        {
            m_EventManager.RemoveListener(eventDef, action);
        }
    }
}
