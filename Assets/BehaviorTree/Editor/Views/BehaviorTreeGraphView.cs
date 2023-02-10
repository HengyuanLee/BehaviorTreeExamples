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
            //false:�ų�����ͨ�ýڵ�
            foreach (var nodeMenuItem in NodeProvider.GetNodeMenuEntries(graph, false))
                yield return nodeMenuItem;
        }
        protected override BaseEdgeConnectorListener CreateEdgeConnectorListener()
        {
            //���ƽڵ㴴������
            return new EdgeConnectorListener(this);
        }
        public override EdgeView CreateEdgeView()
        {
            //��ֱ�ߡ�
            return new BTEdgeView();
        }
    }
}