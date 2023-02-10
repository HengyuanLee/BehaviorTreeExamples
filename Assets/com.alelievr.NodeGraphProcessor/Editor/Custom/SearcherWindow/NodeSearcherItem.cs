using System;
using UnityEditor.Searcher;

namespace GraphProcessor.Custom
{
    /// <summary>
    /// ���ڱ���SearcherWindow������
    /// </summary>
    public class StandardNodeSearcherItem : SearcherItem
    {
        private Type m_NodeType;
        public Type NodeType => m_NodeType;
        public StandardNodeSearcherItem(string name, Type nodeType = null) : base(name)
        {
            m_NodeType = nodeType;
        }
    }

    public class EdgeNodeSearcherItem : UnityEditor.Searcher.SearcherItem
    {
        private NodeProvider.PortDescription m_PortDescription;
        public NodeProvider.PortDescription PortDescription => m_PortDescription;

        public EdgeNodeSearcherItem(string name, NodeProvider.PortDescription portDescription = default) : base(name)
        {
            m_PortDescription = portDescription;
        }
    }
}
