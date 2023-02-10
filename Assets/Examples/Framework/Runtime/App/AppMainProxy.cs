using UnityEngine;
namespace AppFramework
{
    /// <summary>
    /// 模块访问辅助器
    /// </summary>
    public sealed class AppMainProxy
    {

        public DataModule Data => m_AppMain.GetModule<DataModule>();
        public EventModule Event => m_AppMain.GetModule<EventModule>();
        public FSMModule FSM => m_AppMain.GetModule<FSMModule>();
        public UIModule UI => m_AppMain.GetModule<UIModule>();

        private static AppMain m_AppMain;
        public AppMainProxy() {
            if (m_AppMain == null)
            {
                m_AppMain = GameObject.Find("AppFramework").GetComponent<AppMain>();
            }
        }
        //private AppMainProxy m_AppMainProxy = new AppMainProxy();
        //public void Send(EventDef eventDef, object data = null)
        //{
        //    m_AppMainProxy.Event.Send(eventDef, data);
        //}

        //public void Send(IEventArgs args)
        //{
        //    m_AppMainProxy.Event.Send(args);
        //}

        //public void AddListener(EventDef eventDef, Action<IEventArgs> action)
        //{
        //    int eventId = (int)eventDef;
        //    m_AppMainProxy.Event.AddListener(eventId, action);
        //}

        //public void RemoveListener(EventDef eventDef, Action<IEventArgs> action)
        //{
        //    int eventId = (int)eventDef;
        //    m_AppMainProxy.Event.RemoveListener(eventId, action);
        //}
    }
}
