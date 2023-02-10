using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using GraphProcessor.Custom;
using UnityEditor.Searcher;

namespace GraphProcessor
{
    /// <summary>
    /// Base class to write your own edge handling connection system
    /// </summary>
    public class BaseEdgeConnectorListener : IEdgeConnectorListener
    {
        protected virtual bool CreateIncludeGenericNodes => true;

        public readonly BaseGraphView graphView;

        Dictionary< Edge, PortView >    edgeInputPorts = new Dictionary< Edge, PortView >();
        Dictionary< Edge, PortView >    edgeOutputPorts = new Dictionary< Edge, PortView >();

        static CreateNodeMenuWindow     edgeNodeCreateMenuWindow;

        public BaseEdgeConnectorListener(BaseGraphView graphView)
        {
            this.graphView = graphView;
        }

        public virtual void OnDropOutsidePort(Edge edge, Vector2 position)
        {
			this.graphView.RegisterCompleteObjectUndo("Disconnect edge");

			//If the edge was already existing, remove it
			if (!edge.isGhostEdge)
				graphView.Disconnect(edge as EdgeView);

            // when on of the port is null, then the edge was created and dropped outside of a port
            if (edge.input == null || edge.output == null)
                ShowNodeCreationMenuFromEdge(edge as EdgeView, position);
        }

        public virtual void OnDrop(GraphView graphView, Edge edge)
        {
			var edgeView = edge as EdgeView;
            bool wasOnTheSamePort = false;

			if (edgeView?.input == null || edgeView?.output == null)
				return ;

			//If the edge was moved to another port
			if (edgeView.isConnected)
			{
                if (edgeInputPorts.ContainsKey(edge) && edgeOutputPorts.ContainsKey(edge))
                    if (edgeInputPorts[edge] == edge.input && edgeOutputPorts[edge] == edge.output)
                        wasOnTheSamePort = true;

                if (!wasOnTheSamePort)
                    this.graphView.Disconnect(edgeView);
			}

            if (edgeView.input.node == null || edgeView.output.node == null)
                return;

            edgeInputPorts[edge] = edge.input as PortView;
            edgeOutputPorts[edge] = edge.output as PortView;
            try
            {
                this.graphView.RegisterCompleteObjectUndo("Connected " + edgeView.input.node.name + " and " + edgeView.output.node.name);
                if (!this.graphView.Connect(edge as EdgeView, autoDisconnectInputs: !wasOnTheSamePort))
                    this.graphView.Disconnect(edge as EdgeView);
            } catch (System.Exception)
            {
                this.graphView.Disconnect(edge as EdgeView);
            }
        }

        void ShowNodeCreationMenuFromEdge(EdgeView edgeView, Vector2 screenMousePosition)
        {
            //if (edgeNodeCreateMenuWindow == null)
            //    edgeNodeCreateMenuWindow = ScriptableObject.CreateInstance<CreateNodeMenuWindow>();

            //edgeNodeCreateMenuWindow.Initialize(graphView, EditorWindow.focusedWindow, edgeView);
            //SearchWindow.Open(new SearchWindowContext(screenMousePosition + EditorWindow.focusedWindow.position.position), edgeNodeCreateMenuWindow);

            SearcherProvider m_SearcherProvider = ScriptableObject.CreateInstance<SearcherProvider>();
            m_SearcherProvider.Initialize(graphView, EditorWindow.focusedWindow, edgeView);
            var searcherData = m_SearcherProvider.CreateEdgeNodeMenu(CreateIncludeGenericNodes);
            //对齐鼠标左上角
            var windowAlignment = new SearcherWindow.Alignment(SearcherWindow.Alignment.Vertical.Top, SearcherWindow.Alignment.Horizontal.Left);
            SearcherWindow.Show(
                EditorWindow.focusedWindow,
                searcherData,
                item => m_SearcherProvider.OnSearcherWindowSelect(screenMousePosition, item),
                screenMousePosition,
                null,
                windowAlignment
                );
        }
    }
}