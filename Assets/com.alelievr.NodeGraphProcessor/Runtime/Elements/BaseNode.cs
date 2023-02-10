﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using Unity.Jobs;
using System.Linq;

namespace GraphProcessor
{
    /// <summary>
    /// 处理端口的自定义方法，给定线，返回显示的端口数据。
    /// 定义标签为[CustomPortBehavior]，针对的是指定的字段。
    /// </summary>
	public delegate IEnumerable<PortData> CustomPortBehaviorDelegate(List<SerializableEdge> edges);
    /// <summary>
    /// 处理端口的自定义方法，给定字段参数，返回显示的端口数据。
    /// 定义标签为[CustomPortTypeBehavior],针对的是指定的类型。
    /// </summary>
	public delegate IEnumerable< PortData > CustomPortTypeBehaviorDelegate(string fieldName, string displayName, object value);

	[Serializable]
	public abstract partial class BaseNode
	{
		[SerializeField]
		internal string nodeCustomName = null; // The name of the node in case it was renamed by a user

		/// <summary>
		/// Name of the node, it will be displayed in the title section
		/// </summary>
		/// <returns></returns>
		public virtual string       name => GetType().Name;
		
		///// <summary>
		///// The accent color of the node
		///// </summary>
		//public virtual Color color => Color.clear;
		
		/// <summary>
		/// Set a custom uss file for the node. We use a Resources.Load to get the stylesheet so be sure to put the correct resources path
		/// https://docs.unity3d.com/ScriptReference/Resources.Load.html
		/// </summary>
        public virtual string       layoutStyle => string.Empty;

		/// <summary>
		/// If the node can be locked or not
		/// </summary>
        public virtual bool         unlockable => true; 

		/// <summary>
		/// Is the node is locked (if locked it can't be moved)
		/// </summary>
        public virtual bool         isLocked => nodeLock; 

        //id
        public string				GUID;

		public int					computeOrder = -1;

		/// <summary>Tell wether or not the node can be processed. Do not check anything from inputs because this step happens before inputs are sent to the node</summary>
		public virtual bool			canProcess => true;

		/// <summary>Show the node controlContainer only when the mouse is over the node</summary>
		public virtual bool			showControlsOnHover => false;

		/// <summary>True if the node can be deleted, false otherwise</summary>
		public virtual bool			deletable => true;

		/// <summary>
		/// Container of input ports
		/// </summary>
		[NonSerialized]
		public readonly NodeInputPortContainer	inputPorts;
		/// <summary>
		/// Container of output ports
		/// </summary>
		[NonSerialized]
		public readonly NodeOutputPortContainer	outputPorts;

		//Node view datas
		public Rect					position;
		/// <summary>
		/// Is the node expanded
		/// </summary>
		public bool					expanded;
		/// <summary>
		/// Is debug visible
		/// </summary>
		public bool					debug;
		/// <summary>
		/// Node locked state
		/// </summary>
        public bool                 nodeLock;

        public delegate void		ProcessDelegate();

		/// <summary>
		/// Triggered when the node is processes
		/// </summary>
		public event ProcessDelegate	onProcessed;
		public event Action< string, NodeMessageType >	onMessageAdded;
		public event Action< string >					onMessageRemoved;
		/// <summary>
		/// Triggered after an edge was connected on the node
		/// </summary>
		public event Action< SerializableEdge >			onAfterEdgeConnected;
		/// <summary>
		/// Triggered after an edge was disconnected on the node
		/// </summary>
		public event Action< SerializableEdge >			onAfterEdgeDisconnected;

		/// <summary>
		/// Triggered after a single/list of port(s) is updated, the parameter is the field name
        /// 端口列表数据有变化时触发，参数是端口关联的字段名称
		/// </summary>
		public event Action< string >					onPortsUpdated;

		[NonSerialized]
		bool _needsInspector = false;

		/// <summary>
		/// Does the node needs to be visible in the inspector (when selected).
		/// </summary>
		public virtual bool			needsInspector => _needsInspector;

		/// <summary>
		/// Can the node be renamed in the UI. By default a node can be renamed by double clicking it's name.
		/// </summary>
		public virtual bool			isRenamable => false;

		/// <summary>
		/// Is the node created from a duplicate operation (either ctrl-D or copy/paste).
		/// </summary>
		public bool					createdFromDuplication {get; internal set; } = false;

		/// <summary>
		/// True only when the node was created from a duplicate operation and is inside a group that was also duplicated at the same time. 
		/// </summary>
		public bool					createdWithinGroup {get; internal set; } = false;

        /// <summary>
        /// 带有input或ouput标签的字段信息，包含自定义的behavior方法。
        /// </summary>
		[NonSerialized]
		internal Dictionary< string, NodeFieldInformation >	nodeFields = new Dictionary< string, NodeFieldInformation >();

        /// <summary>
        /// 带有CustomPortTypeBehavior标签的方法，用来快速查找字段是否有自定义Behavior
        /// </summary>
		[NonSerialized]
		internal Dictionary< Type, CustomPortTypeBehaviorDelegate> customPortTypeBehaviorMap = new Dictionary<Type, CustomPortTypeBehaviorDelegate>();

		[NonSerialized]
		List< string >				messages = new List<string>();

		[NonSerialized]
		protected BaseGraph			graph;

		internal class NodeFieldInformation
		{
			public string						name;
			public string						fieldName;
			public FieldInfo					info;
			public bool							input;
			public bool							isMultiple;
			public string						tooltip;
			public CustomPortBehaviorDelegate	behavior;
			public bool							vertical;

			public NodeFieldInformation(FieldInfo info, string name, bool input, bool isMultiple, string tooltip, bool vertical, CustomPortBehaviorDelegate behavior)
			{
				this.input = input;
				this.isMultiple = isMultiple;
				this.info = info;
				this.name = name;
				this.fieldName = info.Name;
				this.behavior = behavior;
				this.tooltip = tooltip;
				this.vertical = vertical;
			}
		}

		struct PortUpdate
		{
			public List<string>	fieldNames;
			public BaseNode		node;

			public void Deconstruct(out List<string> fieldNames, out BaseNode node)
			{
				fieldNames = this.fieldNames;
				node = this.node;
			}
		}

		// Used in port update algorithm
		Stack<PortUpdate> fieldsToUpdate = new Stack<PortUpdate>();
		HashSet<PortUpdate> updatedFields = new HashSet<PortUpdate>();

		/// <summary>
		/// Creates a node of type T at a certain position
		/// </summary>
		/// <param name="position">position in the graph in pixels</param>
		/// <typeparam name="T">type of the node</typeparam>
		/// <returns>the node instance</returns>
		public static T CreateFromType< T >(Vector2 position) where T : BaseNode
		{
			return CreateFromType(typeof(T), position) as T;
		}

		/// <summary>
		/// Creates a node of type nodeType at a certain position
		/// </summary>
		/// <param name="position">position in the graph in pixels</param>
		/// <typeparam name="nodeType">type of the node</typeparam>
		/// <returns>the node instance</returns>
		public static BaseNode CreateFromType(Type nodeType, Vector2 position)
		{
			if (!nodeType.IsSubclassOf(typeof(BaseNode)))
				return null;

			var node = Activator.CreateInstance(nodeType) as BaseNode;

			node.position = new Rect(position, new Vector2(100, 100));

			ExceptionToLog.Call(() => node.OnNodeCreated());

			return node;
		}

		#region Initialization

		// called by the BaseGraph when the node is added to the graph
		public void Initialize(BaseGraph graph)
		{
			this.graph = graph;

			ExceptionToLog.Call(() => Enable());

			InitializePorts();
		}
        /// <summary>
        /// 查找所有包含CustomPortTypeBehavior标签的方法，保存在customPortTypeBehaviorMap
        /// </summary>
		void InitializeCustomPortTypeMethods()
		{
			MethodInfo[] methods = new MethodInfo[0];
			Type baseType = GetType();
			while (true)
			{
				methods = baseType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
				foreach (var method in methods)
				{
                    //针对某个类型的自定义处理方法
					var typeBehaviors = method.GetCustomAttributes<CustomPortTypeBehavior>().ToArray();

					if (typeBehaviors.Length == 0)
						continue;

					CustomPortTypeBehaviorDelegate deleg = null;
					try
					{
						deleg = Delegate.CreateDelegate(typeof(CustomPortTypeBehaviorDelegate), this, method) as CustomPortTypeBehaviorDelegate;
					} catch (Exception e)
					{
						Debug.LogError(e);
						Debug.LogError($"Cannot convert method {method} to a delegate of type {typeof(CustomPortTypeBehaviorDelegate)}");
					}

					foreach (var typeBehavior in typeBehaviors)
						customPortTypeBehaviorMap[typeBehavior.type] = deleg;
				}

				// Try to also find private methods in the base class
				baseType = baseType.BaseType;
				if (baseType == null)
					break;
			}
		}

		/// <summary>
		/// Use this function to initialize anything related to ports generation in your node
		/// This will allow the node creation menu to correctly recognize ports that can be connected between nodes
        /// 初始化port，这个方法可以让创建节点菜单正确识别出端口可以连接的节点。
		/// </summary>
		public virtual void InitializePorts()
		{
            // 查找所有包含CustomPortTypeBehavior标签的方法
            InitializeCustomPortTypeMethods();
            //重新排序过再遍历
			foreach (var key in OverrideFieldOrder(nodeFields.Values.Select(k => k.info)))
			{
				var nodeField = nodeFields[key.Name];

				if (HasCustomBehavior(nodeField))
				{
                    //更新Graph中所有关联的node
					UpdatePortsForField(nodeField.fieldName, sendPortUpdatedEvent: false);
				}
				else
				{
					// If we don't have a custom behavior on the node, we just have to create a simple port
                    //如果没有自定义behavior，那么创建一个默认的端口。
					AddPort(nodeField.input, nodeField.fieldName, new PortData { acceptMultipleEdges = nodeField.isMultiple, displayName = nodeField.name, tooltip = nodeField.tooltip, vertical = nodeField.vertical });
				}
			}
		}

		/// <summary>
		/// Override the field order inside the node. It allows to re-order all the ports and field in the UI.
        /// 排列节点内的字段，这样可以在UI上保证字段的排序。
		/// </summary>
		/// <param name="fields">List of fields to sort</param>
		/// <returns>Sorted list of fields</returns>
		public virtual IEnumerable<FieldInfo> OverrideFieldOrder(IEnumerable<FieldInfo> fields)
		{
			long GetFieldInheritanceLevel(FieldInfo f)
			{
                //最优先，先找到是否在最弗雷定义
				int level = 0;
				var t = f.DeclaringType;
				while (t != null)
				{
					t = t.BaseType;
					level++;
				}

				return level;
			}

            // Order by MetadataToken and inheritance level to sync the order with the port order (make sure FieldDrawers are next to the correct port)
            //将端口根据继承深度和MetadataToken排序，保证字段绘制下一个是顺序是固定的。
            return fields.OrderByDescending(f => (long)(((GetFieldInheritanceLevel(f) << 32)) | (long)f.MetadataToken));
		}

		protected BaseNode()
		{
            inputPorts = new NodeInputPortContainer(this);
            outputPorts = new NodeOutputPortContainer(this);

			InitializeInOutDatas();
		}

		/// <summary>
		/// Update all ports of the node
        /// 更新所有节点
		/// </summary>
		public bool UpdateAllPorts()
		{
			bool changed = false;

			foreach (var key in OverrideFieldOrder(nodeFields.Values.Select(k => k.info)))
			{
				var field = nodeFields[key.Name];
				changed |= UpdatePortsForField(field.fieldName);
			}

			return changed;
		}

		/// <summary>
		/// Update all ports of the node without updating the connected ports. Only use this method when you need to update all the nodes ports in your graph.
        /// 更新本地所有节点
		/// </summary>
		public bool UpdateAllPortsLocal()
		{
			bool changed = false;

			foreach (var key in OverrideFieldOrder(nodeFields.Values.Select(k => k.info)))
			{
				var field = nodeFields[key.Name];
				changed |= UpdatePortsForFieldLocal(field.fieldName);
			}

			return changed;
		}


		/// <summary>
		/// Update the ports related to one C# property field (only for this node)
        /// 本地节点单点更新
        /// 更新端口与C#字段的关联
		/// </summary>
		/// <param name="fieldName"></param>
		public bool UpdatePortsForFieldLocal(string fieldName, bool sendPortUpdatedEvent = true)
		{
			bool changed = false;

			if (!nodeFields.ContainsKey(fieldName))
				return false;

			var fieldInfo = nodeFields[fieldName];

			if (!HasCustomBehavior(fieldInfo))
				return false;

            //正在显示的端口
			List< string > finalPorts = new List< string >();

			var portCollection = fieldInfo.input ? (NodePortContainer)inputPorts : outputPorts;

			// Gather all fields for this port (before to modify them)
            //获取这个字段的所有端口（在他们修改前）
			var nodePorts = portCollection.Where(p => p.fieldName == fieldName);
			// Gather all edges connected to these fields:
            //获取这些端口的所有的线
			var edges = nodePorts.SelectMany(n => n.GetEdges()).ToList();

			if (fieldInfo.behavior != null)
			{
                //优先一、
                //执行自定义字段的带[CustomPortBehavior]标签的方法,
                //从这里看得出，CustomPortBehavior标签的方法就是以为edges为参数，获得返回的端口数据PortData。
                foreach (var portData in fieldInfo.behavior(edges))
					AddPortData(portData);
			}
			else
			{
                //优先二、
                //执行自定义类型的带[CustomPortTypeBehavior]标签的方法
                //这里看的出，CustomPortTypeBehavior标签的方法就是以给定的类型信息为参数，获得返回的端口数据PortData。
                var customPortTypeBehavior = customPortTypeBehaviorMap[fieldInfo.info.FieldType];

				foreach (var portData in customPortTypeBehavior(fieldName, fieldInfo.name, fieldInfo.info.GetValue(this)))
					AddPortData(portData);
			}
            //添加要显示的portData
			void AddPortData(PortData portData)
			{
				var port = nodePorts.FirstOrDefault(n => n.portData.identifier == portData.identifier);
				// Guard using the port identifier so we don't duplicate identifiers
                //id需要固定，所以不能复制id
				if (port == null)
				{
					AddPort(fieldInfo.input, fieldName, portData);
					changed = true;
				}
				else
				{
					// in case the port type have changed for an incompatible type, we disconnect all the edges attached to this port
                    //如果端口类型变得不兼容，那么断掉所有连接到这个端口的线
					if (!BaseGraph.TypesAreConnectable(port.portData.displayType, portData.displayType))
					{
						foreach (var edge in port.GetEdges().ToList())
							graph.Disconnect(edge.GUID);
					}

					// patch the port data
                    //修复正端口数据
					if (port.portData != portData)
					{
						port.portData.CopyFrom(portData);
						changed = true;
					}
				}

				finalPorts.Add(portData.identifier);
			}

			// TODO
			// Remove only the ports that are no more in the list
            //将节点不需要的端口移除
			if (nodePorts != null)
			{
				var currentPortsCopy = nodePorts.ToList();
				foreach (var currentPort in currentPortsCopy)
				{
					// If the current port does not appear in the list of final ports, we remove it
                    //移除不显示的ports
					if (!finalPorts.Any(id => id == currentPort.portData.identifier))
					{
						RemovePort(fieldInfo.input, currentPort);
						changed = true;
					}
				}
			}

            // Make sure the port order is correct:
            //节点端口容器根据finalPorts节点的顺序排序
            portCollection.Sort((p1, p2) => {
				int p1Index = finalPorts.FindIndex(id => p1.portData.identifier == id);
				int p2Index = finalPorts.FindIndex(id => p2.portData.identifier == id);

				if (p1Index == -1 || p2Index == -1)
					return 0;

				return p1Index.CompareTo(p2Index);
			});

			if (sendPortUpdatedEvent)
				onPortsUpdated?.Invoke(fieldName);

			return changed;
		}
        /// <summary>
        /// 是否包含有自定义的behavior，字段的或者类型的都可以
        /// </summary>
		bool HasCustomBehavior(NodeFieldInformation info)
		{
			if (info.behavior != null)
				return true;

			if (customPortTypeBehaviorMap.ContainsKey(info.info.FieldType))
				return true;
			
			return false;
		}

		/// <summary>
		/// Update the ports related to one C# property field and all connected nodes in the graph
        /// 更新Graph中所有关联的节点的C#字段
		/// </summary>
		/// <param name="fieldName"></param>
		public bool UpdatePortsForField(string fieldName, bool sendPortUpdatedEvent = true)
		{
			bool changed  = false;

			fieldsToUpdate.Clear();
			updatedFields.Clear();

			fieldsToUpdate.Push(new PortUpdate{fieldNames = new List<string>(){fieldName}, node = this});

			// Iterate through all the ports that needs to be updated, following graph connection when the 
			// port is updated. This is required ton have type propagation multiple nodes that changes port types
			// are connected to each other (i.e. the relay node)
            //如果这个端口需要更新，那么顺着更新所有关联的节点
			while (fieldsToUpdate.Count != 0)
			{
				var (fields, node) = fieldsToUpdate.Pop();

				// Avoid updating twice a port
				if (updatedFields.Any((t) => t.node == node && fields.SequenceEqual(t.fieldNames)))
					continue;
				updatedFields.Add(new PortUpdate{fieldNames = fields, node = node});

				foreach (var field in fields)
				{
					if (node.UpdatePortsForFieldLocal(field, sendPortUpdatedEvent))
					{
                        //本地节点有变化
						foreach (var port in node.IsFieldInput(field) ? (NodePortContainer)node.inputPorts : node.outputPorts)
						{
							if (port.fieldName != field)
								continue;

							foreach(var edge in port.GetEdges())
							{
                                //顺着线更新所有的节点
								var edgeNode = (node.IsFieldInput(field)) ? edge.outputNode : edge.inputNode;
								var fieldsWithBehavior = edgeNode.nodeFields.Values.Where(f => HasCustomBehavior(f)).Select(f => f.fieldName).ToList();
								fieldsToUpdate.Push(new PortUpdate{fieldNames = fieldsWithBehavior, node = edgeNode});
							}
						}
						changed = true;
					}
				}
			}

			return changed;
		}

		HashSet<BaseNode> portUpdateHashSet = new HashSet<BaseNode>();

		internal void DisableInternal()
		{
			// port containers are initialized in the OnEnable
			inputPorts.Clear();
			outputPorts.Clear();

			ExceptionToLog.Call(() => Disable());
		}

		internal void DestroyInternal() => ExceptionToLog.Call(() => Destroy());

		/// <summary>
		/// Called only when the node is created, not when instantiated
		/// </summary>
		public virtual void	OnNodeCreated() => GUID = Guid.NewGuid().ToString();

		public virtual FieldInfo[] GetNodeFields()
			=> GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// 初始化输入输出字段数据
        /// </summary>
		void InitializeInOutDatas()
		{
			var fields = GetNodeFields();
			var methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (var field in fields)
			{
				var inputAttribute = field.GetCustomAttribute< InputAttribute >();
				var outputAttribute = field.GetCustomAttribute< OutputAttribute >();
				var tooltipAttribute = field.GetCustomAttribute< TooltipAttribute >();
				var showInInspector = field.GetCustomAttribute< ShowInInspector >();
				var vertical = field.GetCustomAttribute< VerticalAttribute >();
				bool isMultiple = false;
				bool input = false;
				string name = field.Name;
				string tooltip = null;

				if (showInInspector != null)
					_needsInspector = true;

				if (inputAttribute == null && outputAttribute == null)
					continue ;

				//check if field is a collection type
				isMultiple = (inputAttribute != null) ? inputAttribute.allowMultiple : (outputAttribute.allowMultiple);
				input = inputAttribute != null;
				tooltip = tooltipAttribute?.tooltip;

				if (!String.IsNullOrEmpty(inputAttribute?.name))
					name = inputAttribute.name;
				if (!String.IsNullOrEmpty(outputAttribute?.name))
					name = outputAttribute.name;

				// By default we set the behavior to null, if the field have a custom behavior, it will be set in the loop just below
                //默认behavior为空，如果字段有自定义的behavior，那么在下一步处理。
				nodeFields[field.Name] = new NodeFieldInformation(field, name, input, isMultiple, tooltip, vertical != null, null);
			}

			foreach (var method in methods)
			{
				var customPortBehaviorAttribute = method.GetCustomAttribute< CustomPortBehaviorAttribute >();
				CustomPortBehaviorDelegate behavior = null;

				if (customPortBehaviorAttribute == null)
					continue ;

				// Check if custom port behavior function is valid
				try {
					var referenceType = typeof(CustomPortBehaviorDelegate);
					behavior = (CustomPortBehaviorDelegate)Delegate.CreateDelegate(referenceType, this, method, true);
				} catch {
					Debug.LogError("The function " + method + " cannot be converted to the required delegate format: " + typeof(CustomPortBehaviorDelegate));
				}

				if (nodeFields.ContainsKey(customPortBehaviorAttribute.fieldName))
                    //给字段添加behavior
					nodeFields[customPortBehaviorAttribute.fieldName].behavior = behavior;
				else
					Debug.LogError("Invalid field name for custom port behavior: " + method + ", " + customPortBehaviorAttribute.fieldName);
			}
		}

		#endregion

		#region Events and Processing

		public void OnEdgeConnected(SerializableEdge edge)
		{
			bool input = edge.inputNode == this;
			NodePortContainer portCollection = (input) ? (NodePortContainer)inputPorts : outputPorts;

			portCollection.Add(edge);

			UpdateAllPorts();

			onAfterEdgeConnected?.Invoke(edge);
		}

		protected virtual bool CanResetPort(NodePort port) => true;

        /// <summary>
        /// 断开连接
        /// </summary>
		public void OnEdgeDisconnected(SerializableEdge edge)
		{
			if (edge == null)
				return ;

			bool input = edge.inputNode == this;
			NodePortContainer portCollection = (input) ? (NodePortContainer)inputPorts : outputPorts;

			portCollection.Remove(edge);

			// Reset default values of input port:
            // 重置所有值
			bool haveConnectedEdges = edge.inputNode.inputPorts.Where(p => p.fieldName == edge.inputFieldName).Any(p => p.GetEdges().Count != 0);
			if (edge.inputNode == this && !haveConnectedEdges && CanResetPort(edge.inputPort))
				edge.inputPort?.ResetToDefault();

			UpdateAllPorts();

			onAfterEdgeDisconnected?.Invoke(edge);
		}

		public void OnProcess()
		{
			inputPorts.PullDatas();

			ExceptionToLog.Call(() => Process());

			InvokeOnProcessed();

			outputPorts.PushDatas();
		}

		public void InvokeOnProcessed() => onProcessed?.Invoke();

		/// <summary>
		/// Called when the node is enabled
		/// </summary>
		protected virtual void Enable() {}
		/// <summary>
		/// Called when the node is disabled
		/// </summary>
		protected virtual void Disable() {}
		/// <summary>
		/// Called when the node is removed
		/// </summary>
		protected virtual void Destroy() {}

		/// <summary>
		/// Override this method to implement custom processing
		/// </summary>
		protected virtual void Process() {}

		#endregion

		#region API and utils

		/// <summary>
		/// Add a port
		/// </summary>
		/// <param name="input">is input port</param>
		/// <param name="fieldName">C# field name</param>
		/// <param name="portData">Data of the port</param>
		public void AddPort(bool input, string fieldName, PortData portData)
		{
			// Fixup port data info if needed:
			if (portData.displayType == null)
				portData.displayType = nodeFields[fieldName].info.FieldType;

			if (input)
				inputPorts.Add(new NodePort(this, fieldName, portData));
			else
				outputPorts.Add(new NodePort(this, fieldName, portData));
		}

		/// <summary>
		/// Remove a port
		/// </summary>
		/// <param name="input">is input port</param>
		/// <param name="port">the port to delete</param>
		public void RemovePort(bool input, NodePort port)
		{
			if (input)
				inputPorts.Remove(port);
			else
				outputPorts.Remove(port);
		}

		/// <summary>
		/// Remove port(s) from field name
		/// </summary>
		/// <param name="input">is input</param>
		/// <param name="fieldName">C# field name</param>
		public void RemovePort(bool input, string fieldName)
		{
			if (input)
				inputPorts.RemoveAll(p => p.fieldName == fieldName);
			else
				outputPorts.RemoveAll(p => p.fieldName == fieldName);
		}

		/// <summary>
		/// Get all the nodes connected to the input ports of this node
		/// 获取所有左边的节点。
		/// </summary>
		/// <returns>an enumerable of node</returns>
		public IEnumerable< BaseNode > GetInputNodes()
		{
			foreach (var port in inputPorts)
				foreach (var edge in port.GetEdges())
					yield return edge.outputNode;
		}

		/// <summary>
		/// Get all the nodes connected to the output ports of this node
		/// 获取所有接受输出的节点。获取所有连接到这个node的ouput port的nodes，也就是node的右边的nodes。
		/// </summary>
		/// <returns>an enumerable of node</returns>
		public IEnumerable< BaseNode > GetOutputNodes()
		{
			foreach (var port in outputPorts)
				foreach (var edge in port.GetEdges())
					yield return edge.inputNode;
		}

		/// <summary>
		/// Return a node matching the condition in the dependencies of the node
		/// </summary>
		/// <param name="condition">Condition to choose the node</param>
		/// <returns>Matched node or null</returns>
		public BaseNode FindInDependencies(Func<BaseNode, bool> condition)
		{
			Stack<BaseNode> dependencies = new Stack<BaseNode>();

			dependencies.Push(this);

			int depth = 0;
			while (dependencies.Count > 0)
			{
				var node = dependencies.Pop();

				// Guard for infinite loop (faster than a HashSet based solution)
				depth++;
				if (depth > 2000)
					break;

				if (condition(node))
					return node;
				
				foreach (var dep in node.GetInputNodes())
					dependencies.Push(dep);
			}
			return null;
		}

		/// <summary>
		/// Get the port from field name and identifier
		/// </summary>
		/// <param name="fieldName">C# field name</param>
		/// <param name="identifier">Unique port identifier</param>
		/// <returns></returns>
		public NodePort	GetPort(string fieldName, string identifier)
		{
			return inputPorts.Concat(outputPorts).FirstOrDefault(p => {
				var bothNull = String.IsNullOrEmpty(identifier) && String.IsNullOrEmpty(p.portData.identifier);
				return p.fieldName == fieldName && (bothNull || identifier == p.portData.identifier);
			});
		}

		/// <summary>
		/// Return all the ports of the node
		/// </summary>
		/// <returns></returns>
		public IEnumerable<NodePort> GetAllPorts()
		{
			foreach (var port in inputPorts)
				yield return port;
			foreach (var port in outputPorts)
				yield return port;
		}

		/// <summary>
		/// Return all the connected edges of the node
		/// </summary>
		/// <returns></returns>
		public IEnumerable<SerializableEdge> GetAllEdges()
		{
			foreach (var port in GetAllPorts())
				foreach (var edge in port.GetEdges())
					yield return edge;
		}

		/// <summary>
		/// Is the port an input
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public bool IsFieldInput(string fieldName) => nodeFields[fieldName].input;

		/// <summary>
		/// Add a message on the node
		/// </summary>
		/// <param name="message"></param>
		/// <param name="messageType"></param>
		public void AddMessage(string message, NodeMessageType messageType)
		{
			if (messages.Contains(message))
				return;

			onMessageAdded?.Invoke(message, messageType);
			messages.Add(message);
		}

		/// <summary>
		/// Remove a message on the node
		/// </summary>
		/// <param name="message"></param>
		public void RemoveMessage(string message)
		{
			onMessageRemoved?.Invoke(message);
			messages.Remove(message);
		}

		/// <summary>
		/// Remove a message that contains
		/// </summary>
		/// <param name="subMessage"></param>
		public void RemoveMessageContains(string subMessage)
		{
			string toRemove = messages.Find(m => m.Contains(subMessage));
			messages.Remove(toRemove);
			onMessageRemoved?.Invoke(toRemove);
		}

		/// <summary>
		/// Remove all messages on the node
		/// </summary>
		public void ClearMessages()
		{
			foreach (var message in messages)
				onMessageRemoved?.Invoke(message);
			messages.Clear();
		}

		/// <summary>
		/// Set the custom name of the node. This is intended to be used by renamable nodes.
		/// This custom name will be serialized inside the node.
		/// </summary>
		/// <param name="customNodeName">New name of the node.</param>
		public void SetCustomName(string customName) => nodeCustomName = customName;

		/// <summary>
		/// Get the name of the node. If the node have a custom name (set using the UI by double clicking on the node title) then it will return this name first, otherwise it returns the value of the name field.
		/// </summary>
		/// <returns>The name of the node as written in the title</returns>
		public string GetCustomName() => String.IsNullOrEmpty(nodeCustomName) ? name : nodeCustomName; 

		#endregion
	}
}
