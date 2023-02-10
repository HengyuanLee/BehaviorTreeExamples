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
        /// 转换graph为可遍历的，用于适配深度优先算法。
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        static TraversalGraph ConvertGraphToTraversalGraph(BaseGraph graph)
        {
            TraversalGraph g = new TraversalGraph();
            Dictionary<BaseNode, TarversalNode> nodeMap = new Dictionary<BaseNode, TarversalNode>();

            //数据填充，将graph的数据填充到可遍历的g中。
            foreach (var node in graph.nodes)
            {
                var tn = new TarversalNode(node);
                g.nodes.Add(tn);
                nodeMap[node] = tn;

                if (graph.graphOutputs.Contains(node))
                    g.outputs.Add(tn);
            }
            //数据填充，将node的in/out数据填充到可比案例节点tn中。
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
        /// 处理循环输入输出节点。
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cyclicNode"></param>
        public static void FindCyclesInGraph(BaseGraph g, Action<BaseNode> cyclicNode)
        {
            //转换（包装）成可进行DFS运算的graph。
            var graph = ConvertGraphToTraversalGraph(g);
            //循环节点。
            List<TarversalNode> cyclicNodes = new List<TarversalNode>();

            foreach (var n in graph.nodes)
                DFS(n);

            void DFS(TarversalNode n)
            {
                if (n.state == State.Black)//访问过了，跳过。
                    return;
                
                n.state = State.Grey;//访问中，灰色。

                foreach (var input in n.inputs)//对于输入节点，即左边的节点。
                {
                    if (input.state == State.White)
                        DFS(input);//没处理过，先尝试处理
                    else if (input.state == State.Grey)
                        cyclicNodes.Add(n);
                }
                n.state = State.Black;//标记为已处理。
            }

            cyclicNodes.ForEach((tn) => cyclicNode?.Invoke(tn.node));
        }
    }
}