using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphProcessor.Custom
{
    public struct NodeEntry
    {
        public string[] title;
        //public AbstractMaterialNode node;
        public int compatibleSlotId;
        public string slotName;
    }
    public class SearcherProvider : ScriptableObject
    {
        private EditorWindow m_EditorWindow;
        private BaseGraphView m_GraphView;
        private EdgeView m_EdgeView;
        private PortView m_InputPortView;
        private PortView m_OutputPortView;

        public List<NodeEntry> currentNodeEntries;
        public VisualElement target { get; internal set; }
        public void Initialize(BaseGraphView graphView, EditorWindow editorWindow, EdgeView edgeFilter = null)
        {
            m_EditorWindow = editorWindow;
            m_GraphView = graphView;
            m_EdgeView = edgeFilter;
            m_InputPortView = edgeFilter?.input as PortView;
            m_OutputPortView = edgeFilter?.output as PortView;
        }
        private void BuildSearchTree()
        {

        }
        public Searcher CreateStandardNodeMenu()
        {
            List<SearcherItem> itemTreeRoot = new List<SearcherItem>();
            var nodeEntries = m_GraphView.FilterCreateNodeMenuEntries().OrderBy(k => k.path);

            foreach (var nodeMenuItem in nodeEntries)
            {
                var nodePath = nodeMenuItem.path;
                var nodeType = nodeMenuItem.type;
                var nodeName = nodePath;

                SearcherItem parent = null;
                SearcherItem item = null;
                string[] subPaths = nodePath.Split("/");
                List<string> fullPath = subPaths.ToList();
                for(int i= 0; i < fullPath.Count; i++)
                {
                    string sub = fullPath[i];
                    if (string.IsNullOrEmpty(sub))
                    {
                        continue;
                    }
                    //先在最顶层文件夹找
                    List<SearcherItem> children = parent != null ? parent.Children : itemTreeRoot;
                    item = children.Find(item => item.Name == sub);
                    //找不到，则新建文件夹
                    if (item == null)
                    {
                        bool isDataNode = i == fullPath.Count - 1;//非路径，最后一个为数据节点。
                        item = new StandardNodeSearcherItem(sub, isDataNode ? nodeType : null);
                        if (parent != null)
                        {
                            parent.AddChild(item);
                        }
                        else
                        {
                            children.Add(item);
                        }
                    }
                    parent = item;

                    if (parent.Depth == 0 && !itemTreeRoot.Contains(parent))
                        itemTreeRoot.Add(parent);

                }
            }
            var dataBase = SearcherDatabase.Create(itemTreeRoot, string.Empty, false);

            string title = "Create Node";
            SearcherAdapterWindow adapterWindow = new SearcherAdapterWindow(title);
            Searcher searcher = new Searcher(dataBase, adapterWindow);
            return searcher;
        }
        public Searcher CreateEdgeNodeMenu(bool includeGenericNodes = true)
        {
            var entries = NodeProvider.GetEdgeCreationNodeMenuEntry((m_EdgeView.input ?? m_EdgeView.output) as PortView, m_GraphView.graph, includeGenericNodes);
            var titlePaths = new HashSet<string>();
            var nodePaths = NodeProvider.GetNodeMenuEntries(m_GraphView.graph);

            List<SearcherItem> itemTreeRoot = new List<SearcherItem>();

            //第一个连接Relay
            var portDescription = new NodeProvider.PortDescription
            {
                nodeType = typeof(RelayNode),
                portType = typeof(System.Object),
                isInput = m_InputPortView != null,
                portFieldName = m_InputPortView != null ? nameof(RelayNode.output) : nameof(RelayNode.input),
                portIdentifier = "0",
                portDisplayName = m_InputPortView != null ? "Out" : "In",
            };
            EdgeNodeSearcherItem relayItem = new EdgeNodeSearcherItem($"Relay", portDescription);



            var sortedMenuItems = entries.Select(port => (port, nodePaths.FirstOrDefault(kp => kp.type == port.nodeType).path)).OrderBy(e => e.path);


            foreach (var nodeMenuItem in sortedMenuItems)
            {

                var nodePath = nodePaths.FirstOrDefault(kp => kp.type == nodeMenuItem.port.nodeType).path;

                // Ignore the node if it's not in the create menu
                if (String.IsNullOrEmpty(nodePath))
                    continue;

                var nodeName = nodePath;
                var parts = nodePath.Split('/');

                SearcherItem parent = null;
                SearcherItem item = null;
                string[] subPaths = nodePath.Split("/");
                if (subPaths.Length > 0)
                {
                    nodeName = subPaths[subPaths.Length - 1];
                }
                List<string> fullPath = subPaths.ToList();
                for (int i=0; i < fullPath.Count; i++)
                {
                    string sub = fullPath[i];
                    if (string.IsNullOrEmpty(sub))
                    {
                        continue;
                    }
                    //先在最顶层文件夹找
                    List<SearcherItem> children = parent != null ? parent.Children : itemTreeRoot;
                    item = children.Find(item => item.Name == sub);
                    //找不到，则新建
                    if (item == null)
                    {
                        bool isDataNode = i == fullPath.Count - 1;//非路径，最后一个为数据节点。
                        if (isDataNode)
                        {
                            //数据节点
                            item = new EdgeNodeSearcherItem($"{sub}: {nodeMenuItem.port.portDisplayName}", nodeMenuItem.port);
                        }
                        else {
                            //路径节点
                            item = new EdgeNodeSearcherItem(sub);
                        }
                        if (parent != null)
                        {
                            parent.AddChild(item);
                        }
                        else
                        {
                            children.Add(item);
                        }
                    }
                    parent = item;

                    if (parent.Depth == 0 && !itemTreeRoot.Contains(parent))
                        itemTreeRoot.Add(parent);

                }
            }
            var dataBase = SearcherDatabase.Create(itemTreeRoot, string.Empty, false);

            string title = "Create Node";
            SearcherAdapterWindow adapterWindow = new SearcherAdapterWindow(title);
            Searcher searcher = new Searcher(dataBase, adapterWindow);
            return searcher;
        }

        public bool OnSearcherWindowSelect(Vector2 screenMousePosition, SearcherItem item)
        {
            // window to graph position
            var windowRoot = m_EditorWindow.rootVisualElement;
            var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, screenMousePosition);
            var graphMousePosition = m_GraphView.contentViewContainer.WorldToLocal(windowMousePosition);

            if (item is StandardNodeSearcherItem standardNodeItem)
            {
                var nodeType = standardNodeItem.NodeType;

                m_GraphView.RegisterCompleteObjectUndo("Added " + nodeType);
                BaseNode newNode = BaseNode.CreateFromType(nodeType, graphMousePosition);
                newNode.createMenuName = item.Name;
                m_GraphView.AddNode(newNode);
                return true;
            }
            if (item is EdgeNodeSearcherItem edgeNodeItem)
            {
                var nodeType = edgeNodeItem.PortDescription.nodeType;
                m_GraphView.RegisterCompleteObjectUndo("Added " + nodeType);
                var newNode = BaseNode.CreateFromType(nodeType, graphMousePosition);
                newNode.createMenuName = item.Name;
                var view = m_GraphView.AddNode(newNode);
                var targetPort = view.GetPortViewFromFieldName(edgeNodeItem.PortDescription.portFieldName, edgeNodeItem.PortDescription.portIdentifier);
                if (m_InputPortView == null)
                {
                    m_GraphView.Connect(targetPort, m_OutputPortView);
                }
                else
                {
                    m_GraphView.Connect(m_InputPortView, targetPort);
                }
                return true;
            }
            return false;
        }
    }
}