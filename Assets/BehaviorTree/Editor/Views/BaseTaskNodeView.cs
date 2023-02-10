using GraphProcessor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MyBehaviorTree
{
    [NodeCustomEditor(typeof(BaseTaskNode))]
    public class BaseTaskNodeView : BaseNodeView
    {
        [System.NonSerialized]
        private bool isRuned;//节点运行过。
        private Label returnLabel;
        private Label descLabel;

        protected override string[] IconFolders => GlobalSettingRefrence.Instance.GlobalSetting?.IconFolders;

        public override void CustomInitialize()
        {
            base.CustomInitialize();
            style.width = 160;
            style.height = 120;
            returnLabel = new Label();
            returnLabel.style.color = Color.black;
            returnLabel.style.fontSize = 22;
            returnLabel.style.alignSelf = new StyleEnum<Align>(Align.Stretch);
            //Add(returnLabel);
            SetNodeRunningColor();
            OnDescTextChange();
            OnEnableChange();
            RegisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
        }
        public void SetHighlightColor(Color color)
        {
            //float radius = 8;
            //style.borderBottomLeftRadius = radius;
            //style.borderBottomRightRadius = radius;
            //style.borderTopLeftRadius = radius;
            //style.borderTopRightRadius = radius;
            //float borderWidth = 4;
            //style.borderRightWidth = borderWidth;
            //style.borderLeftWidth = borderWidth;
            //style.borderTopWidth = borderWidth;
            //style.borderBottomWidth = borderWidth;
            //style.borderBottomColor = color;
            //style.borderLeftColor = color;
            //style.borderRightColor = color;
            //style.borderTopColor = color;
            returnLabel.style.color = color;
            color.a = 0.1f;
            returnLabel.style.backgroundColor = color;
        }
        /// <summary>
        /// 节点在运行时颜色变化
        /// </summary>
        private void SetNodeRunningColor()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            if (EditorApplication.isPlaying)
            {
                //如果打开Graph编辑器时Unity在Play，那么主动设置一下运行时效果。
                OnPlayModeStateChanged(PlayModeStateChange.EnteredPlayMode);
            }
            if (nodeTarget is BaseTaskNode node)
            {
                node.OnDoTickCallback = SetRunningState;
            }
        }
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            {
                if (state == PlayModeStateChange.ExitingPlayMode)
                {
                    Remove(returnLabel);
                    style.height = style.height.value.value - 20;
                    if (isRuned)
                    {
                        isRuned = false;
                        schedule.Execute(() =>
                        {
                            SetHighlightColor(Color.clear);
                            returnLabel.text = "";
                            if (inputPortViews.Count > 0)
                            {
                                var edges = inputPortViews[0].GetEdges();
                                if (edges.Count > 0)
                                {
                                    edges[0].output.portColor = Color.white;
                                    SetLineColorByEnable();
                                }
                            }
                            MarkDirtyRepaint();
                        }).ExecuteLater(50);
                    }
                }
                else if (state == PlayModeStateChange.EnteredPlayMode)
                {
                    Add(returnLabel);
                    style.height = style.height.value.value + 20;
                    SetRunningState();
                }
            };
        }
        private void SetRunningState()
        {
            if (nodeTarget is BaseTaskNode node)
            {
                isRuned = true;
                TaskStatus taskStatus = node.LastReturnTaskStatus;
                Color runColor = Color.clear;
                switch (taskStatus)
                {
                    case TaskStatus.Success:
                        runColor = new Color(0f, 1f, 0f, 1);
                        returnLabel.text = "✔ Success";
                        break;
                    case TaskStatus.Failure:
                        runColor = new Color(1f, 0f, 0f, 1);
                        returnLabel.text = "✘ Failure";
                         break;
                    case TaskStatus.Inactive:
                        runColor = new Color(0.5f, 0.5f, 0.5f, 1);
                        returnLabel.text = "○ Inactive";
                        break;
                    case TaskStatus.Running:
                        runColor = new Color(1f, 1f, 0f, 1);
                        returnLabel.text = "✈ Running";
                        break;
                }

                runColor.a = Random.Range(0.6f, 1f);
                SetHighlightColor(runColor);
                schedule.Execute(() =>
                {
                    if (EditorApplication.isPlaying)
                    {
                        runColor.a = 0.3f;
                        SetHighlightColor(runColor);
                    }
                }).ExecuteLater(500);
                if (inputPortViews.Count > 0)
                {
                    var edges = inputPortViews[0].GetEdges();
                    if (edges.Count > 0)
                    {
                        //float r = Random.Range(0f, 1f);
                        //float g = Random.Range(0f, 1f);
                        //float b = Random.Range(0f, 1f);
                        //Color rColor = new Color(r, g, b, 1);
                        runColor.a = 1;
                        Color rColor = runColor;
                        edges[0].output.portColor = rColor;
                        edges[0].input.portColor = rColor;
                        edges[0].OnSelected();

                        schedule.Execute(() =>
                        {
                            if (EditorApplication.isPlaying)
                            {
                                Color color = new Color(1, 1, 1, 0.2f);
                                edges[0].output.portColor = color;
                                edges[0].input.portColor = color;
                                edges[0].OnSelected();
                            }
                        }).ExecuteLater(500);
                    }
                }
            }
        }
        protected override void DrawDefaultInspector(bool fromInspector = false)
        {
            //为了不在节点下绘制出属性,覆盖掉这个方法。
            //base.DrawDefaultInspector(fromInspector);

            this.expanded = false;
        }
        public void OnDescTextChange()
        {
            if (descLabel == null)
            {
                descLabel = new Label();
                descLabel.style.fontSize = 12;
                Color backgoundColor = Color.gray;
                var colorAttrs = nodeTarget.GetType().GetCustomAttributes(typeof(NodeColorAttribute), true).Cast<NodeColorAttribute>().ToArray();
                if (colorAttrs.Length > 0)
                {
                    //设置color
                    backgoundColor = colorAttrs[0].color;
                }
                backgoundColor.a = 0.3f;
                descLabel.style.backgroundColor = backgoundColor;
                descLabel.style.color = Color.white;//new Color(1f-backgoundColor.r, 1f- backgoundColor.g, 1f- backgoundColor.b);
                descLabel.style.fontSize = 15;
            }
            if (!mainContainer.Contains(descLabel))
            {
                mainContainer.Insert(1, descLabel);
                style.height = style.height.value.value + 20;
            }
            if (!string.IsNullOrEmpty((nodeTarget as BaseTaskNode)?.description))
            {

                descLabel.text = (nodeTarget as BaseTaskNode)?.description;
            }
            else
            {
                if (mainContainer.Contains(descLabel))
                {
                    mainContainer.Remove(descLabel);
                    style.height = style.height.value.value - 20;
                }
            }
        }
        public void OnEnableChange()
        {
            mainContainer.SetEnabled((nodeTarget as BaseTaskNode).enable);
            SetLineColorByEnable();
        }
        protected void OnGeometryChangedEvent(GeometryChangedEvent evt)
        {
            //首次进来更新线的颜色
            SetLineColorByEnable();
        }
        private void SetLineColorByEnable()
        {
            if (inputPortViews.Count > 0)
            {
                var edges = inputPortViews[0]?.GetEdges();
                if (edges?.Count > 0)
                {
                    bool enable = (nodeTarget as BaseTaskNode).enable;
                    Color color = enable ? Color.white : Color.gray;
                    //edges[0].output.portColor = color;
                    edges[0].input.portColor = color;
                    edges[0].OnSelected();
                }
            }
        }
    }
}