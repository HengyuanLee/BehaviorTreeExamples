using System;
using System.Collections.Generic;
using UnityEngine;

namespace AppFramework
{
    public class AppMain : MonoBehaviour
    {

        public static AppMain Instance { get; private set; }
        public DataModule Data => GetModule<DataModule>();
        public EventModule Event => GetModule<EventModule>();
        public FSMModule FSM => GetModule<FSMModule>();
        public UIModule UI => GetModule<UIModule>();

        private AppModuleManager m_AppModuleManager = new AppModuleManager();

        private void Awake()
        {
            var modules = GetComponentsInChildren<AppModuleBase>();
            for (int i = 0; i < modules.Length; i++) {
                Debug.Log("加载模块："+modules[i].GetType().FullName);
                m_AppModuleManager.Register(modules[i]);
            }
            Instance = this;
            OnInit();
        }
        public T GetModule<T>() where T : IAppModule {
            return m_AppModuleManager.Get<T>();
        }

        public void OnInit()
        {
            m_AppModuleManager.OnInit();
        }
        void Update() {
            m_AppModuleManager.OnUpdate();
        }
    }
}
