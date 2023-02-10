
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphProcessor
{
    [NodeCustomEditor(typeof(BaseNode))]
    [WindowTitle("Node Inspector")]
    public class BaseNodeInspectorView : PinnedElementView
    {
        public static readonly Rect DEFAULT_POSITION = new Rect(0, 30, 400, 800);

        protected BaseNodeView nodeView { get; private set; }
        protected BaseGraphView graphView { get; private set; }

        private Dictionary<string, PropertyField> showPropertyFields = new Dictionary<string, PropertyField>();

        private Label subTitleLabel;
        protected override void Initialize(BaseGraphView graphView)
        {
            //this.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0.95f));
            subTitleLabel = new Label();
            this.graphView = graphView;
            //初次打开，显示默认的。
            int onSelectNodeCount = graphView.selection.Count(e => e is BaseNodeView);
            if (onSelectNodeCount == 1)
            {
                graphView.selection.ForEach(e => {
                    if (e is BaseNodeView nodeView)
                    {
                        OnSelectNodeView(nodeView);
                    }
                });
            }
            else {
                content.Clear();
                content.Add(subTitleLabel);
                SetOnSelectNodeCount();
            }
        }
        public void OnSelectNodeView(BaseNodeView nodeView)
        {
            if (graphView.selection.Count > 1)
            {
                //选中多个，清空显示。
                OnNodeUnSelect();
                return;
            }
            this.nodeView = nodeView;
            showPropertyFields.Clear();
            content.Clear();
            content.Add(subTitleLabel);
            OnGUI();
        }
        /// <summary>
        /// 选中空白，取消inspector view的node显示。
        /// </summary>
        public void OnNodeUnSelect()
        {
            nodeView = null;
            showPropertyFields.Clear();
            content.Clear();
            content.Add(subTitleLabel);

            //因为选中一个节点时，会取消另外一个节点的选中触发这个回调。
            //为了保证先触发选中再触发未选中顺序，延迟10ms。
            schedule.Execute(() =>
            {
                SetOnSelectNodeCount();
            }).ExecuteLater(10);
        }
        private void SetOnSelectNodeCount()
        {
            int count = graphView.selection.Count(e => e is BaseNodeView);
            if (count != 1)
            {
                subTitleLabel.text = $"Select a node to view its properties. \nCurrent select node count : {count}.";
            }
        }
        protected virtual void OnGUI()
        {
            var attrs = nodeView.nodeTarget.GetType().GetCustomAttributes(typeof(NodeNameAttribute), true).Cast<NodeNameAttribute>().ToArray();
            if (attrs.Length > 0)
            {
                subTitleLabel.text = attrs[0].name.Replace("\n", string.Empty);
            }
            else
            {
                subTitleLabel.text = nodeView.nodeTarget.name;
            }
            //===========================================
            // Filter fields from the BaseNode type since we are only interested in user-defined fields
            // (better than BindingFlags.DeclaredOnly because we keep any inherited user-defined fields) 
            var fields = nodeView.nodeTarget.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.DeclaringType != typeof(BaseNode));
            fields = nodeView.nodeTarget.OverrideFieldOrder(fields).Reverse();

            foreach (var field in fields)
            {
                if (field.GetCustomAttribute(typeof(SettingAttribute)) != null)
                {
                    continue;
                }

                bool serializeField = field.GetCustomAttribute(typeof(SerializeField)) != null;
                if ((!field.IsPublic && !serializeField) || field.IsNotSerialized)
                {
                    continue;
                }

                bool hasInputAttribute = field.GetCustomAttribute(typeof(InputAttribute)) != null;
                bool hasInputOrOutputAttribute = hasInputAttribute || field.GetCustomAttribute(typeof(OutputAttribute)) != null;
                //bool showAsDrawer = fromInspector && field.GetCustomAttribute(typeof(ShowAsDrawer)) != null;
                if (!serializeField && hasInputOrOutputAttribute)// && !showAsDrawer)
                {
                    continue;
                }

                if (field.GetCustomAttribute(typeof(System.NonSerializedAttribute)) != null || field.GetCustomAttribute(typeof(HideInInspector)) != null)
                {
                    continue;
                }
                var showInInspector = field.GetCustomAttribute<ShowInInspector>();
                if (!serializeField && showInInspector != null && !showInInspector.showInNode)
                {
                    continue;
                }

                var showInputDrawer = field.GetCustomAttribute(typeof(InputAttribute)) != null && field.GetCustomAttribute(typeof(SerializeField)) != null;
                showInputDrawer |= field.GetCustomAttribute(typeof(InputAttribute)) != null && field.GetCustomAttribute(typeof(ShowAsDrawer)) != null;
                //showInputDrawer &= !fromInspector; // We can't show a drawer in the inspector
                showInputDrawer &= !typeof(IList).IsAssignableFrom(field.FieldType);

                DrawField(field);
            }
        }

        protected void DrawField(FieldInfo field)
        {
            string displayName = ObjectNames.NicifyVariableName(field.Name);
            var inspectorNameAttribute = field.GetCustomAttribute<InspectorNameAttribute>();
            if (inspectorNameAttribute != null)
                displayName = inspectorNameAttribute.displayName;

            var propertyField = new PropertyField(FindSerializedProperty(nodeView, field.Name), displayName);
            propertyField.Bind(nodeView.owner.serializedGraph);

            if (propertyField != null)
            {
                content.Add(propertyField);
                propertyField.name = displayName;
                showPropertyFields.Add(field.Name, propertyField);
            }
        }
        protected PropertyField GetPropertyField(string fieldName)
        {
            if (showPropertyFields.TryGetValue(fieldName, out PropertyField propertyField))
            {
                return propertyField;
            }
            return null;
        }
        protected SerializedProperty FindSerializedProperty(BaseNodeView nodeView, string fieldName)
        {
            int i = nodeView.owner.graph.nodes.FindIndex(n => n == nodeView.nodeTarget);
            return nodeView.owner.serializedGraph.FindProperty("nodes").GetArrayElementAtIndex(i).FindPropertyRelative(fieldName);
        }
    }
}