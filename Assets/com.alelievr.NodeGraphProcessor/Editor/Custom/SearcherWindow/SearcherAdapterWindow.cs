
namespace GraphProcessor.Custom
{

    public class SearcherAdapterWindow : UnityEditor.Searcher.SearcherAdapter
    {
        public override bool HasDetailsPanel => false;

        public SearcherAdapterWindow(string title) : base(title)
        {
        }
    }
}
