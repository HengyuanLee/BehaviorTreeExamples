using GraphProcessor;
using UnityEditor.UIElements;

namespace MyBehaviorTree
{
    [NodeCustomEditor(typeof(BaseTaskNode))]
    public class BaseTaskNodeInspectorView : BaseNodeInspectorView
    {
        protected override void OnGUI()
        {
            base.OnGUI();
            var activePropertyField = GetPropertyField("enable");
            if (activePropertyField != null)
            {
                activePropertyField.RegisterValueChangeCallback(OnEnableValueChange);
            }
            var tintColorPropertyField = GetPropertyField("description");
            if (tintColorPropertyField != null)
            {
                tintColorPropertyField.RegisterValueChangeCallback(OnDescValueChange);
            }
        }
        private void OnEnableValueChange(SerializedPropertyChangeEvent e)
        {
            //bool isActive = e.changedProperty.boolValue;
            if (nodeView is BaseTaskNodeView taskNodeView)
            {
                taskNodeView.OnEnableChange();
            }
        }
        private void OnDescValueChange(SerializedPropertyChangeEvent e)
        {
            //string desc = e.changedProperty.stringValue;
            if (nodeView is BaseTaskNodeView taskNodeView)
            {
                taskNodeView.OnDescTextChange();
            }
        }
    }
}