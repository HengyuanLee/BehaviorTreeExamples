
using System;
using System.Collections.Generic;

namespace AppFramework
{
    public class UIManager {
        private Dictionary<Type, IUIPanelController> m_UIDict = new Dictionary<Type, IUIPanelController>();

        public void Show<T>() where T : IUIPanelController
        {
            if (m_UIDict.TryGetValue(typeof(T), out IUIPanelController uiController)) {
                uiController.Show();
            }
        }
        public void Hide<T>() where T : IUIPanelController
        {
            if (m_UIDict.TryGetValue(typeof(T), out IUIPanelController uiController))
            {
                uiController.Hide();
            }
        }
        public T Get<T>() where T : IUIPanelController
        {
            if (m_UIDict.TryGetValue(typeof(T), out IUIPanelController uiController))
            {
                return (T)uiController;
            }
            return default;
        }
        public void Destroy<T>() where T : IUIPanelController
        {
            if (m_UIDict.TryGetValue(typeof(T), out IUIPanelController uiController))
            {
                uiController.Destroy();
            }
        }
        public void OnUpdate() {
            foreach (var kv in m_UIDict) {
                if (kv.Value.UIShowState == UIShowState.Show) {
                    kv.Value.OnUpdate();
                }
            }
        }
        public void Register<T>() where T : IUIPanelController, new()
        {
            lock (m_UIDict) {
                if (m_UIDict.ContainsKey(typeof(T)) == false)
                {
                    T controller = new T();
                    controller.OnInit();
                    m_UIDict.Add(controller.GetType(), controller);
                }
                else {
                    throw new System.Exception("重复注册："+ typeof(T));
                }
            }
        }
        public void Remove<T>() where T : IUIPanelController
        {
            if (m_UIDict.ContainsKey(typeof(T))) {
                lock (m_UIDict) {
                    m_UIDict.Remove(typeof(T));
                }
            }
        }
    }
}
