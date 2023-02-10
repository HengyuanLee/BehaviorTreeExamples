using System;
using System.Collections.Generic;
using UnityEngine;

namespace AppFramework
{
    public class AppModuleManager
    {
        private Dictionary<Type, IAppModule> m_ModuleDict = new Dictionary<Type, IAppModule>();

        public void Register(AppModuleBase module) {
            Type t = module.GetType();
            if (m_ModuleDict.ContainsKey(t) == false)
            {
                m_ModuleDict.Add(t, module);
            }
            else
            {
                Debug.LogError("不能添加重复的模块：" + t.FullName);
            }
        }
        public T Get<T>() where T : IAppModule{
            if (m_ModuleDict.TryGetValue(typeof(T), out IAppModule outModule)) {
                return (T)outModule;
            }
            return default;
        }
        public void OnInit() {
            foreach (var kv in m_ModuleDict)
            {
                kv.Value.OnInit();
            }
        }
        public void OnUpdate() {
            foreach (var kv in m_ModuleDict) {
                kv.Value.OnUpdate();
            }
        }
    }
}
