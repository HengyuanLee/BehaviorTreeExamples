using System;
using System.Collections.Generic;

namespace AppFramework
{
    public class DataManager
    {
        private Dictionary<Type, IDataProxy<IData>> m_DataDict = new Dictionary<Type, IDataProxy<IData>>();

        public void Register<T>(T dataProxy) where T : IDataProxy<IData>
        {
            CheckDataIsValid(dataProxy);
            lock (m_DataDict) {
                if (m_DataDict.ContainsKey(typeof(T)) == false) {
                    m_DataDict.Add(typeof(T), dataProxy);
                }
            }
        }
        public void Remove<T>()where T : IDataProxy<IData>
        {
            lock (m_DataDict) {
                if (m_DataDict.ContainsKey(typeof(T)))
                {
                    m_DataDict.Remove(typeof(T));
                }
            }
        }
        public T Get<T>() where T : IDataProxy<IData> {
            if (m_DataDict.TryGetValue(typeof(T), out IDataProxy<IData> outData)) {
                return (T)outData;
            }
            return default;
        }
        private void CheckDataIsValid(IDataProxy<IData> dataProxy) {
            if (dataProxy == null) {
                throw new System.Exception("dataProxy不能为空！");
            }
        }
        private void CheckDataNameIsValid(string dataName) {
            if (string.IsNullOrEmpty(dataName) == true) {
                throw new System.Exception("dataName不能为空！");
            }
        }
    }
}
