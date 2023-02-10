using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.IO;
using System.Reflection;
using UnityEditor.Experimental.GraphView;

namespace GraphProcessor
{
	public static class NodeProvider
	{
		public struct PortDescription
		{
			public Type nodeType;
			public Type portType;
			public bool isInput;
			public string portFieldName;
			public string portIdentifier;
			public string portDisplayName;
		}

		static Dictionary< Type, MonoScript >	nodeViewScripts = new Dictionary< Type, MonoScript >();
		static Dictionary< Type, MonoScript >	nodeScripts = new Dictionary< Type, MonoScript >();
		static Dictionary< Type, Type >			nodeViewPerType = new Dictionary< Type, Type >();
		static Dictionary< Type, Type >			nodeInspectorViewPerType = new Dictionary< Type, Type >();//node的Inspector面板显示

        public class NodeDescriptions
		{
			public Dictionary< string, Type >		nodePerMenuTitle = new Dictionary< string, Type >();
			public List< Type >						slotTypes = new List< Type >();
			public List< PortDescription >			nodeCreatePortDescription = new List<PortDescription>();
		}

		public struct NodeSpecificToGraph
		{
			public Type				nodeType;
			public List<MethodInfo>	isCompatibleWithGraph;
			public Type				compatibleWithGraphType;
		} 
		/// <summary>
		/// 针对每个graph，将所有对这个graph可用的node填充进去。
		/// </summary>
		static Dictionary<BaseGraph, NodeDescriptions>	specificNodeDescriptions = new Dictionary<BaseGraph, NodeDescriptions>();
		/// <summary>
		/// 指定某个graph可创建的node。
		/// </summary>
		static List<NodeSpecificToGraph>				specificNodes = new List<NodeSpecificToGraph>();
		/// <summary>
		/// 通用的node，未指定哪个graph，所有的graph可用。
		/// </summary>
		static NodeDescriptions							genericNodes = new NodeDescriptions();

		/// <summary>
		/// 默认触发
		/// </summary>
		static NodeProvider()
		{
			BuildScriptCache();
			BuildGenericNodeCache();
		}

		public static void LoadGraph(BaseGraph graph)
		{
			// Clear old graph data in case there was some
			specificNodeDescriptions.Remove(graph);
			var descriptions = new NodeDescriptions();
			specificNodeDescriptions.Add(graph, descriptions);

			var graphType = graph.GetType();
			foreach (var nodeInfo in specificNodes)
			{
				//这个node是否指定这个graph可创建
				bool compatible = nodeInfo.compatibleWithGraphType == null || nodeInfo.compatibleWithGraphType == graphType;

				if (nodeInfo.isCompatibleWithGraph != null)
				{
					foreach (var method in nodeInfo.isCompatibleWithGraph)
						compatible &= (bool)method?.Invoke(null, new object[]{ graph });
				}

				if (compatible)
					BuildCacheForNode(nodeInfo.nodeType, descriptions, graph);
			}
		}

		public static void UnloadGraph(BaseGraph graph)
		{
			specificNodeDescriptions.Remove(graph);
		}

		static void BuildGenericNodeCache()
		{
			foreach (var nodeType in TypeCache.GetTypesDerivedFrom<BaseNode>())
			{
				if (!IsNodeAccessibleFromMenu(nodeType))//排除抽象类
					continue;

				if (IsNodeSpecificToGraph(nodeType))//排除指定与某个类
					continue;

				BuildCacheForNode(nodeType, genericNodes);
			}
		}
		/// <summary>
		/// 将node的显示信息保存到targetDescription
		/// </summary>
		/// <param name="nodeType"></param>
		/// <param name="targetDescription"></param>
		/// <param name="graph"></param>
		static void BuildCacheForNode(Type nodeType, NodeDescriptions targetDescription, BaseGraph graph = null)
		{
			var attrs = nodeType.GetCustomAttributes(typeof(NodeMenuItemAttribute), true) as NodeMenuItemAttribute[];

			if (attrs != null && attrs.Length > 0)
			{
				foreach (var attr in attrs)
				{
					if (attr.menuTitle == string.Empty)
					{
						//空白的不填就不显示了
						//targetDescription.nodePerMenuTitle[nodeType.Name] = nodeType;
					}
					else
					{
						targetDescription.nodePerMenuTitle[attr.menuTitle] = nodeType;
					}
				}
			}

			foreach (var field in nodeType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (field.GetCustomAttribute<HideInInspector>() == null && field.GetCustomAttributes().Any(c => c is InputAttribute || c is OutputAttribute))
					targetDescription.slotTypes.Add(field.FieldType);
			}

			ProvideNodePortCreationDescription(nodeType, targetDescription, graph);
		}
		/// <summary>
		/// 排除abstract类
		/// </summary>
		static bool IsNodeAccessibleFromMenu(Type nodeType)
		{
			if (nodeType.IsAbstract)
				return false;

			return nodeType.GetCustomAttributes<NodeMenuItemAttribute>().Count() > 0;
		}

		// Check if node has anything that depends on the graph type or settings
		/// <summary>
		/// 检测节点是否有依赖graph设置
		/// </summary>
		/// <param name="nodeType"></param>
		/// <returns></returns>
		static bool IsNodeSpecificToGraph(Type nodeType)
		{
			//类里带IsCompatibleWithGraph的静态方法
			var isCompatibleWithGraphMethods = nodeType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).Where(m => m.GetCustomAttribute<IsCompatibleWithGraph>() != null);
			//类本身带的NodeMenuItem标签
			var nodeMenuAttributes = nodeType.GetCustomAttributes<NodeMenuItemAttribute>();

			//这个类指定仅仅在哪个graph可创建
			List<Type> compatibleGraphTypes = nodeMenuAttributes.Where(n => n.onlyCompatibleWithGraph != null).Select(a => a.onlyCompatibleWithGraph).ToList();

			List<MethodInfo> compatibleMethods = new List<MethodInfo>();
			foreach (var method in isCompatibleWithGraphMethods)
			{
				// Check if the method is static and have the correct prototype
				var p = method.GetParameters();
				if (method.ReturnType != typeof(bool) || p.Count() != 1 || p[0].ParameterType != typeof(BaseGraph))
					Debug.LogError($"The function '{method.Name}' marked with the IsCompatibleWithGraph attribute either doesn't return a boolean or doesn't take one parameter of BaseGraph type.");
				else
					compatibleMethods.Add(method);
			}

			if (compatibleMethods.Count > 0 || compatibleGraphTypes.Count > 0)
			{
				// We still need to add the element in specificNode even without specific graph
				if (compatibleGraphTypes.Count == 0)
					compatibleGraphTypes.Add(null);

				foreach (var graphType in compatibleGraphTypes)
				{
					specificNodes.Add(new NodeSpecificToGraph{
						nodeType = nodeType,
						isCompatibleWithGraph = compatibleMethods,
						compatibleWithGraphType = graphType
					});
				}
				return true;
			}
			return false;
		}
		/// <summary>
		/// 保存脚本资源
		/// </summary>
		static void BuildScriptCache()
		{
			foreach (var nodeType in TypeCache.GetTypesDerivedFrom<BaseNode>())
			{
				if (!IsNodeAccessibleFromMenu(nodeType))//排除抽象类
					continue;

				AddNodeScriptAsset(nodeType);
			}

			foreach (var nodeViewType in TypeCache.GetTypesDerivedFrom<BaseNodeView>())
			{
				if (!nodeViewType.IsAbstract)
					AddNodeViewScriptAsset(nodeViewType);
			}
            foreach (var viewType in TypeCache.GetTypesDerivedFrom<PinnedElementView>())
            {
                if (typeof(BaseNodeInspectorView).IsAssignableFrom(viewType))
                {
                    if (!viewType.IsAbstract)
                        AddNodeInspectorViewScriptAsset(viewType);
                }
            }
		}

		static FieldInfo SetGraph = typeof(BaseNode).GetField("graph", BindingFlags.NonPublic | BindingFlags.Instance);
		static void ProvideNodePortCreationDescription(Type nodeType, NodeDescriptions targetDescription, BaseGraph graph = null)
		{
			var node = Activator.CreateInstance(nodeType) as BaseNode;//这个node并不保存来使用，仅仅实例化一个出来，以便还原信息来保存。
			try {
				SetGraph.SetValue(node, graph);//自动对node的graph字段赋值
				node.InitializePorts();//初始化
				node.UpdateAllPorts();
			} catch (Exception) { }

			foreach (var p in node.inputPorts)
				AddPort(p, true);
			foreach (var p in node.outputPorts)
				AddPort(p, false);

			void AddPort(NodePort p, bool input)
			{
				targetDescription.nodeCreatePortDescription.Add(new PortDescription{
					nodeType = nodeType,
					portType = p.portData.displayType ?? p.fieldInfo.FieldType,
					isInput = input,
					portFieldName = p.fieldName,
					portDisplayName = p.portData.displayName ?? p.fieldName,
					portIdentifier = p.portData.identifier,
				});
			}
		}

		static void AddNodeScriptAsset(Type type)
		{
			var nodeScriptAsset = FindScriptFromClassName(type.Name);

			// Try find the class name with Node name at the end
			if (nodeScriptAsset == null)
				nodeScriptAsset = FindScriptFromClassName(type.Name + "Node");
			if (nodeScriptAsset != null)
				nodeScripts[type] = nodeScriptAsset;
		}

		static void	AddNodeViewScriptAsset(Type type)
		{
			var attrs = type.GetCustomAttributes(typeof(NodeCustomEditor), false) as NodeCustomEditor[];

			if (attrs != null && attrs.Length > 0)
			{
				Type nodeType = attrs.First().nodeType;
				nodeViewPerType[nodeType] = type;

				var nodeViewScriptAsset = FindScriptFromClassName(type.Name);
				if (nodeViewScriptAsset == null)
					nodeViewScriptAsset = FindScriptFromClassName(type.Name + "View");
				if (nodeViewScriptAsset == null)
					nodeViewScriptAsset = FindScriptFromClassName(type.Name + "NodeView");

				if (nodeViewScriptAsset != null)
					nodeViewScripts[type] = nodeViewScriptAsset;
			}
		}
        static void AddNodeInspectorViewScriptAsset(Type type)
        {
            var attrs = type.GetCustomAttributes(typeof(NodeCustomEditor), false) as NodeCustomEditor[];

            if (attrs != null && attrs.Length > 0)
            {
                Type nodeType = attrs.First().nodeType;
                nodeInspectorViewPerType[nodeType] = type;
            }
        }

        static MonoScript FindScriptFromClassName(string className)
		{
			var scriptGUIDs = AssetDatabase.FindAssets($"t:script {className}");

			if (scriptGUIDs.Length == 0)
				return null;

			foreach (var scriptGUID in scriptGUIDs)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(scriptGUID);
				var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

				if (script != null && String.Equals(className, Path.GetFileNameWithoutExtension(assetPath), StringComparison.OrdinalIgnoreCase))
					return script;
			}

			return null;
		}

		public static Type GetNodeViewTypeFromType(Type nodeType)
		{
			Type view;

            if (nodeViewPerType.TryGetValue(nodeType, out view))
                return view;

            Type baseType = null;

            // Allow for inheritance in node views: multiple C# node using the same view
            foreach (var type in nodeViewPerType)
            {
                // Find a view (not first fitted view) of nodeType
                if (nodeType.IsSubclassOf(type.Key) && (baseType == null || type.Value.IsSubclassOf(baseType)))
                    baseType = type.Value;
            }

            if (baseType != null)
                return baseType;

            return view;
        }
        /// <summary>
        /// 获取节点Inspector面板的重写类型。
        /// </summary>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        public static Type GetNodeInspectorViewTypeFromType(Type nodeType)
        {
            if (nodeInspectorViewPerType.TryGetValue(nodeType, out Type type))
            {
                return type;
            }
            return null;
        }
		/// <summary>
		/// 创建节点菜单，是定专属graph + anygraph
		/// </summary>
        public static IEnumerable<(string path, Type type)>	GetNodeMenuEntries(BaseGraph graph = null, bool includeGenericNodes = true)
		{
			if (includeGenericNodes)
			{
				//包含不指定哪个graph的通用node。
				foreach (var node in genericNodes.nodePerMenuTitle)
					yield return (node.Key, node.Value);
			}

			if (graph != null && specificNodeDescriptions.TryGetValue(graph, out var specificNodes))
			{
				foreach (var node in specificNodes.nodePerMenuTitle)
					yield return (node.Key, node.Value);
			}
		}

		public static MonoScript GetNodeViewScript(Type type)
		{
			nodeViewScripts.TryGetValue(type, out var script);

			return script;
		}

		public static MonoScript GetNodeScript(Type type)
		{
			nodeScripts.TryGetValue(type, out var script);

			return script;
		}

		public static IEnumerable<Type> GetSlotTypes(BaseGraph graph = null) 
		{
			foreach (var type in genericNodes.slotTypes)
				yield return type;

			if (graph != null && specificNodeDescriptions.TryGetValue(graph, out var specificNodes))
			{
				foreach (var type in specificNodes.slotTypes)
					yield return type;
			}
		}

		public static IEnumerable<PortDescription> GetEdgeCreationNodeMenuEntry(PortView portView, BaseGraph graph = null, bool includeGenericNodes = true)
		{
			if (includeGenericNodes)
			{
				//包含不指定哪个graph的通用node。
				foreach (var description in genericNodes.nodeCreatePortDescription)
				{
					if (!IsPortCompatible(description))
						continue;

					yield return description;
				}
			}

			if (graph != null && specificNodeDescriptions.TryGetValue(graph, out var specificNodes))
			{
				foreach (var description in specificNodes.nodeCreatePortDescription)
				{
					if (!IsPortCompatible(description))
						continue;
					yield return description;
				}
			}

			bool IsPortCompatible(PortDescription description)
			{
				if ((portView.direction == Direction.Input && description.isInput) || (portView.direction == Direction.Output && !description.isInput))
					return false;
	
				if (!BaseGraph.TypesAreConnectable(description.portType, portView.portType))
					return false;
					
				return true;
			}
		}
	}
}
