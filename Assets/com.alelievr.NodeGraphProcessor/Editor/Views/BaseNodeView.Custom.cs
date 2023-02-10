using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System;

namespace GraphProcessor
{
    public partial class BaseNodeView
    {
        protected Image taskIconImage = new Image();
        protected virtual string[] IconFolders => null;
        protected Sprite GetIcon(string iconName)
        {
            Sprite icon = null;
            if (IconFolders != null && IconFolders.Length > 0)
            {
                foreach (var iconFolder in IconFolders)
                {
                    string file = iconFolder + "/" + iconName;
                    icon = AssetDatabase.LoadAssetAtPath<Sprite>(file);
                    if (icon != null)
                    {
                        break;
                    }
                }
            }
            if (icon == null)
            {
                Texture2D tex = new Texture2D(50,50);
                icon = Sprite.Create(tex, new Rect(0, 0, 0, 45), Vector2.zero);
            }
            return icon;
        }
        public virtual void CustomInitialize()
        {
            if (owner == null)
            {
                return;
            }
            var iconAttrs = nodeTarget.GetType().GetCustomAttributes(typeof(NodeIconAttribute),true).Cast<NodeIconAttribute>().ToArray();
            string iconName = iconAttrs.Length > 0 ? iconAttrs[0].icon : null;
            //����icon
            mainContainer.Insert(2, taskIconImage);
            taskIconImage.sprite = GetIcon(iconName);
            taskIconImage.style.left = 0;
            taskIconImage.style.height = 60;
            Color backgoundColor = new Color(0.5f, 0.5f, 0.5f, 0.75f);
            var colorAttrs = nodeTarget.GetType().GetCustomAttributes(typeof(NodeColorAttribute), true).Cast<NodeColorAttribute>().ToArray();
            if (colorAttrs.Length > 0)
            {
                //����color
                backgoundColor = colorAttrs[0].color;
                backgoundColor.a *= 0.6f;
            }
            taskIconImage.style.backgroundColor = backgoundColor;
                //this.style.backgroundImage = new StyleBackground(icon);
            var nameAttrs = nodeTarget.GetType().GetCustomAttributes(typeof(NodeNameAttribute), false).Cast<NodeNameAttribute>().ToArray();
            if (nameAttrs.Length > 0)
            {
                //����name
                var attr = nameAttrs[0];
                nodeTarget.SetCustomName(attr.name);
                UpdateTitle();
            }
            var sizeAttrs = nodeTarget.GetType().GetCustomAttributes(typeof(NodeSizeAttribute), true).Cast<NodeSizeAttribute>().ToArray();
            if (sizeAttrs.Length > 0)
            {
                //����size
                var attr = sizeAttrs[0];
                if(attr.width > 0) this.style.width = attr.width;
                if(attr.height > 0) this.style.height = attr.height;
            }
            //var colorAttrs = nodeTarget.GetType().GetCustomAttributes(typeof(NodeColorAttribute), true).Cast<NodeColorAttribute>().ToArray();
            if (colorAttrs.Length > 0)
            {
                //����size
                var attr = colorAttrs[0];
                SetNodeColor(attr.color);
            }
        }
        
        public override void OnSelected()
        {

            if (owner == null)
            {
                return;
            }
            Type nodeInspectorViewType = null;
            Type nodeType = nodeTarget.GetType();
            while (nodeInspectorViewType == null && nodeType != null)
            {
                //�Ҳ�������˳�����������ҡ�
                nodeInspectorViewType = NodeProvider.GetNodeInspectorViewTypeFromType(nodeType);
                nodeType = nodeType.BaseType;
            }
            bool isCloseOtherOnShowNodeType = false;
            if (nodeInspectorViewType != null)
            {
                Rect lastRect = BaseNodeInspectorView.DEFAULT_POSITION;
                //�ر��������ڴ򿪵�inspector view����ֻ֤��һ���ڴ�
                if (owner.pinnedElements != null)
                {
                    for (int i = 0; i < owner.pinnedElements.Count; i++)
                    {
                        var e = owner.pinnedElements.ElementAt(i);
                        if (e.Value is BaseNodeInspectorView inspectorView)
                        {
                            if (e.Value.GetType() != nodeInspectorViewType)
                            {
                                //������ǵ�ǰnode��Ӧ���͵���壬��ô���ر�
                                lastRect = inspectorView.GetPosition();
                                isCloseOtherOnShowNodeType = true;
                                owner.ClosePinned(e.Key, e.Value);
                            }
                        }
                    }
                }
                if (isCloseOtherOnShowNodeType)
                {
                    //��ͬnode����֮����л���˳��ԭ����λ�ý���ȥ��
                    var view = owner.OpenPinned(nodeInspectorViewType);
                    view.SetPosition(lastRect);
                }

                if (owner.pinnedElements.TryGetValue(nodeInspectorViewType, out PinnedElementView pinnedElementView))
                {
                    //�����ڣ�˵��δ��ʾ����
                    if (pinnedElementView is BaseNodeInspectorView nodeInspectorView)
                    {
                        //������˵������ʾ��
                        if (owner.Contains(nodeInspectorView))
                        {
                            nodeInspectorView.OnSelectNodeView(this);
                        }
                    }
                }
            }
        }
        public override void OnUnselected()
        {
            ClearInsepctorView();
        }
        /// <summary>
        /// ֻ��ѡ��һ����inspector�������ݡ�����0�����������ʾ��
        /// </summary>
        private void ClearInsepctorView()
        {
            if (owner.pinnedElements != null)
            {
                for (int i = 0; i < owner.pinnedElements.Count; i++)
                {
                    var e = owner.pinnedElements.ElementAt(i);
                    if (e.Value is BaseNodeInspectorView view)
                    {
                        view.OnNodeUnSelect();
                    }
                }
            }
        }
    }
}