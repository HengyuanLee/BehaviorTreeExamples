namespace AppFramework
{
    public interface IUIPanelController
    {
        void OnInit();
        void Show();
        void Hide();
        void OnUpdate();
        void Destroy();
        UIShowState UIShowState { get; }
    }
}
