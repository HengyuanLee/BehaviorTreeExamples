
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphProcessor
{
    public abstract partial class PinnedElementView
    {
        /// <summary>
        /// 不让覆盖掉toolbarview。
        /// </summary>
        public override void UpdatePresenterPosition()
        {
            if (graphView != null && graphView.toolbarView != null && graphView.Contains(graphView.toolbarView))
            {
                //漏出菜单栏
                var pos = GetPosition();
                if (pos.y < graphView.toolbarView.Height+3)
                {
                    SetPosition(new Rect(pos.x, graphView.toolbarView.Height+3, pos.size.x, pos.size.y));
                }
            }
        }
        private void CustomInitializeGraphView()
        {
            var closeBtn = new Button(() => {
                graphView.ClosePinned(GetType(),this);
            });
            closeBtn.style.width = 18;
            closeBtn.style.height = 18;
            closeBtn.style.color = Color.clear;
            closeBtn.style.backgroundColor = Color.clear;
            closeBtn.style.borderRightWidth = 0;
            closeBtn.style.borderLeftWidth = 0;
            closeBtn.style.borderTopWidth = 0;
            closeBtn.style.borderBottomWidth = 0;
            closeBtn.style.borderRightWidth = 0;
            closeBtn.style.borderLeftColor = Color.clear;
            string texName = "d_winbtn_win_close@2x";
            var image = EditorGUIUtility.FindTexture(texName);
            closeBtn.style.backgroundImage = new StyleBackground(image);
            header.Insert(header.childCount, closeBtn);

            RegisterCallback<MouseDownEvent>(e =>
            {
                if (graphView != null)
                {
                    //被选中，保持在最顶端。
                    graphView.Insert(graphView.childCount, this);
                }
            });
        }
    }
}