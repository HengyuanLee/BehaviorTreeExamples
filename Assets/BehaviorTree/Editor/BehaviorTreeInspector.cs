using GraphProcessor;
using UnityEditor;
using UnityEngine.UIElements;

namespace MyBehaviorTree
{
    [CustomEditor(typeof(BehaviorTreeGraph))]
    public class BehaviorTreeInspector : GraphInspector
    {

        protected override void CreateInspector()
        {
            base.CreateInspector();

            root.Add(new Button(() => EditorWindow.GetWindow<BehaviorTreeWindow>().InitializeGraph(target as BehaviorTreeGraph))
            {
                text = "Open"
            });
        }
    }
}
