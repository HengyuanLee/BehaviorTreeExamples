using System;
using System.Linq;
using System.Collections.Generic;

namespace GraphProcessor
{
    public static class GraphUtils
    {
        enum State
        {
            White,
            Grey,
            Black,
        }

        class TarversalNode
        {
            public BaseNode node;
            public List<TarversalNode> inputs = new List<TarversalNode>();
            public List<TarversalNode> outputs = new List<TarversalNode>();
            public State    state = State.White;

            public TarversalNode(BaseNode node) { this.node = node; }
        }

        // A structure made for easy graph traversal
        class TraversalGraph
        {
            public List<TarversalNode> nodes = new List<TarversalNode>();
            public List<TarversalNode> outputs = new List<TarversalNode>();
        }
        /// <summary>
        /// ת��graphΪ�ɱ����ģ�����������������㷨��
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        static TraversalGraph ConvertGraphToTraversalGraph(BaseGraph graph)
        {
            TraversalGraph g = new TraversalGraph();
            Dictionary<BaseNode, TarversalNode> nodeMap = new Dictionary<BaseNode, TarversalNode>();

            //������䣬��graph��������䵽�ɱ�����g�С�
            foreach (var node in graph.nodes)
            {
                var tn = new TarversalNode(node);
                g.nodes.Add(tn);
                nodeMap[node] = tn;

                if (graph.graphOutputs.Contains(node))
                    g.outputs.Add(tn);
            }
            //������䣬��node��in/out������䵽�ɱȰ����ڵ�tn�С�
            foreach (var tn in g.nodes)
            {
                tn.inputs = tn.node.GetInputNodes().Where(n => nodeMap.ContainsKey(n)).Select(n => nodeMap[n]).ToList();
                tn.outputs = tn.node.GetOutputNodes().Where(n => nodeMap.ContainsKey(n)).Select(n => nodeMap[n]).ToList();
            }

            return g;
        }

        public static List<BaseNode> DepthFirstSort(BaseGraph g)
        {
            var graph = ConvertGraphToTraversalGraph(g);
            List<BaseNode> depthFirstNodes = new List<BaseNode>();

            foreach (var n in graph.nodes)
                DFS(n);

            void DFS(TarversalNode n)
            {
                if (n.state == State.Black)
                    return;
                
                n.state = State.Grey;

                if (n.node is ParameterNode parameterNode && parameterNode.accessor == ParameterAccessor.Get)
                {
                    foreach (var setter in graph.nodes.Where(x=> 
                        x.node is ParameterNode p &&
                        p.parameterGUID == parameterNode.parameterGUID &&
                        p.accessor == ParameterAccessor.Set))
                    {
                        if (setter.state == State.White)
                            DFS(setter);
                    }
                }
                else
                {
                    foreach (var input in n.inputs)
                    {
                        if (input.state == State.White)
                            DFS(input);
                    }
                }

                n.state = State.Black;

                // Only add the node when his children are completely visited
                depthFirstNodes.Add(n.node);
            }

            return depthFirstNodes;
        }
        /// <summary>
        /// ����ѭ����������ڵ㡣
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cyclicNode"></param>
        public static void FindCyclesInGraph(BaseGraph g, Action<BaseNode> cyclicNode)
        {
            //ת������װ���ɿɽ���DFS�����graph��
            var graph = ConvertGraphToTraversalGraph(g);
            //ѭ���ڵ㡣
            List<TarversalNode> cyclicNodes = new List<TarversalNode>();

            foreach (var n in graph.nodes)
                DFS(n);

            void DFS(TarversalNode n)
            {
                if (n.state == State.Black)//���ʹ��ˣ�������
                    return;
                
                n.state = State.Grey;//�����У���ɫ��

                foreach (var input in n.inputs)//��������ڵ㣬����ߵĽڵ㡣
                {
                    if (input.state == State.White)
                        DFS(input);//û��������ȳ��Դ���
                    else if (input.state == State.Grey)
                        cyclicNodes.Add(n);
                }
                n.state = State.Black;//���Ϊ�Ѵ���
            }

            cyclicNodes.ForEach((tn) => cyclicNode?.Invoke(tn.node));
        }
    }
}