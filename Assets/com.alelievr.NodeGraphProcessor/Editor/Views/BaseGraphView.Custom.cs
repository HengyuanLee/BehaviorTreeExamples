

namespace GraphProcessor
{

    public partial class BaseGraphView
    {
        public ToolbarView toolbarView { get; private set; }
        public void AddToolbarView(ToolbarView view)
        {
            if (toolbarView != null && Contains(toolbarView))
            {
                Remove(toolbarView);
            }
            toolbarView = view;
            Add(toolbarView);
        }
        private void CustomInitialize()
        {
        }
    }
}
