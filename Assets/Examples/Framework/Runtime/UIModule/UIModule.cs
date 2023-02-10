using UnityEngine;

namespace AppFramework
{
    public class UIPanelInfo
    {
        public string prefabPath;
        public UIShowType uiShowType;
        public bool isPreload;
    }
    public enum UIShowState
    {
        Unload = 0,
        Show,
        Hide
    }
    public enum UIShowType
    {
        General,
        Pop,
        Message,
        Element
    }
    public partial class UIModule : AppModuleBase
    {
        [SerializeField]
        private Transform m_UIRoot;

        public Transform UIRoot => m_UIRoot;
        private UIManager m_UIManager = new UIManager();

        public void Show<T>() where T : IUIPanelController
        {
            m_UIManager.Show<T>();
        }
        public void Hide<T>() where T : IUIPanelController
        {
            m_UIManager.Hide<T>();
        }
        public T Get<T>() where T : IUIPanelController {
            return m_UIManager.Get<T>();
        }
        public void Register<T>() where T : IUIPanelController, new()
        {
            m_UIManager.Register<T>();
        }
        public void Remove<T>() where T : IUIPanelController
        {
            m_UIManager.Remove<T>();
        }

        public override void OnUpdate()
        {
            m_UIManager.OnUpdate();
        }
    }
}
