using GraphProcessor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MyBehaviorTree
{

    public class BehaviorTreeWindow : BaseGraphWindow
    {
        protected override void InitializeWindow(BaseGraph graph)
        {
            titleContent = new GUIContent("ÐÐÎªÊ÷");
            if (graphView == null)
            {
                graphView = new BehaviorTreeGraphView(this);
                graphView.AddToolbarView(new BTToolbarView(graphView));

                //var cameraMenu = new ToolbarMenu() { name = "toolb" };
                ////cameraMenu.variant = ToolbarMenu.Variant.Popup;
                //var cameraToggle = new ToolbarToggle() { name = "cam" };
                //cameraToggle.value = true;
                //cameraToggle.tooltip = "??";
                //cameraToggle.RegisterCallback<MouseUpEvent>(evt =>
                //{
                //    Debug.LogError("mouse dow...");
                //});
                //cameraToggle.RegisterValueChangedCallback(evt => Debug.LogError("cb..."));;
                //string texName = "d_winbtn_win_close@2x";
                //var image = EditorGUIUtility.FindTexture(texName);


                //string texName2 = "d_Prefab On Icon";
                //var image2 = EditorGUIUtility.FindTexture(texName2);

                //var cameraSeparator = new ToolbarToggle() {  };
                //cameraSeparator.Add(new Image() { image = image2});
                //cameraToggle.Add(new Image() { image = image });
                ////cameraToggle.Add(new Image() { image = image });

                //cameraMenu.Add(cameraSeparator);
                //cameraMenu.Add(cameraToggle);
                //cameraMenu.menu.AppendAction("s",
                //    (DropdownMenuAction a) => { Debug.LogError("zhix ..."); },
                //    DropdownMenuAction.AlwaysEnabled);

                //var toolbar = new Toolbar() { name = "tb" };
                //toolbar.Add(new ToolbarSpacer() { flex = true });
                ////toolbar.Add(new ToolbarSpacer());
                //toolbar.Add(cameraMenu);
                //rootVisualElement.Add(toolbar);
            }
            rootView.Add(graphView);
        }
        protected override void InitializeGraphView(BaseGraphView view)
        {
            base.InitializeGraphView(view);
        }
    }
}