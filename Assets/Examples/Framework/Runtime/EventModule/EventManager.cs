using System;
using System.Collections.Generic;

namespace AppFramework
{
    public class EventManager
    {
        private readonly Dictionary<EventDef, Delegate> m_ActionDict = new Dictionary<EventDef, Delegate>();

        public void Send(EventDef eventDef) {
            (SendTryGet<Action>(eventDef))?.Invoke();
        }
        public void Send<T>(EventDef eventDef, T t) {
            (SendTryGet<Action<T>>(eventDef))?.Invoke(t);
        }
        public void Send<T1, T2>(EventDef eventDef, T1 t1, T2 t2)
        {
            (SendTryGet<Action<T1, T2>>(eventDef))?.Invoke(t1, t2);
        }
        public void Send<T1, T2, T3>(EventDef eventDef, T1 t1, T2 t2, T3 t3)
        {
            (SendTryGet<Action<T1, T2, T3>>(eventDef))?.Invoke(t1, t2, t3);
        }

        private T SendTryGet<T>(EventDef eventDef)  where T : Delegate{
            if (m_ActionDict.TryGetValue(eventDef, out Delegate outDelegate))
            {
                if (outDelegate is T)
                {
                    return outDelegate as T;
                }
                else if (outDelegate == null) {
                    return default;
                }
                else
                {
                    throw new Exception($"�¼������쳣��Send�¼� id ��{eventDef} Send����Register�Ĳ�����ƥ�䣬��ǰ���ͣ�{typeof(T)}, �Ѵ������ͣ�{outDelegate.GetType()}");
                }
            }
            else {
                return default;
            }
        }

        public void Register(EventDef eventDef, Delegate d)
        {
            if (m_ActionDict.TryGetValue(eventDef, out Delegate outDelegate))
            {
                if (d.GetType() != outDelegate.GetType())
                {
                    throw new Exception($"�¼�ע���쳣��Register�¼� id��{eventDef} ע���������ע����Ĳ�����ƥ�䣬��ǰ���� : {d.GetType()}���Ѵ������ͣ�{outDelegate.GetType()}");
                }
                else
                {
                    m_ActionDict[eventDef] = Delegate.Combine(outDelegate, d);
                }
            }
            else
            {
                m_ActionDict.Add(eventDef, d);
            }
        }
        public void RemoveListener(EventDef eventDef, Delegate target) {
            if (m_ActionDict.TryGetValue(eventDef, out Delegate outDelegate))
            {
                if (outDelegate == null) {
                    m_ActionDict.Remove(eventDef);
                }else if (outDelegate.GetType() != target.GetType())
                {
                    throw new Exception($"�¼��Ƴ��쳣��Send�¼� id ��{eventDef} Remove����Register�Ĳ�����ƥ�䣬remove���ͣ�{target.GetType()}, ��ע�����ͣ�{outDelegate.GetType()}");
                }
                else
                {
                    m_ActionDict[eventDef] = Delegate.Remove(outDelegate, target);
                }
            }
        }
    }
}