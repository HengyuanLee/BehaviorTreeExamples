using GraphProcessor;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace MyBehaviorTree
{
    public class BehaviorTreeGraphView : BaseGraphView
    {
        public BehaviorTreeGraphView(EditorWindow window) : base(window)
        {
        }

        public override IEnumerable<(string path, Type type)> FilterCreateNodeMenuEntries()
        {
            //false:排除创建通用节点
            foreach (var nodeMenuItem in NodeProvider.GetNodeMenuEntries(graph, false))
                yield return nodeMenuItem;
        }
        protected override BaseEdgeConnectorListener CreateEdgeConnectorListener()
        {
            //控制节点创建类型
            return new EdgeConnectorListener(this);
        }
        public override EdgeView CreateEdgeView()
        {
            //垂直线。
            return new BTEdgeView();
        }
    }
}