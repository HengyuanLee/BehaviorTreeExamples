namespace AppFramework
{
    public class DataModule : AppModuleBase
    {
        private DataManager m_DataManager = new DataManager();

        public void Register<T>(T dataProxy) where T : IDataProxy<IData> {
            m_DataManager.Register(dataProxy);
        }
        public void Remove<T>(T dataProxy) where T : IDataProxy<IData> {
            m_DataManager.Register(dataProxy);
        }
        public void Get<T>() where T : IDataProxy<IData> {
            m_DataManager.Get<T>();
        }
    }
}