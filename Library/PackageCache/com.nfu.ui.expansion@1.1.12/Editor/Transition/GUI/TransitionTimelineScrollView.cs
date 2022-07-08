using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

namespace ND.UI
{
    public class TransitionTimelineScrollView : EditorScrollViewBase
    {
        private GUIStyle _timelineTickStyle;

        private float _needShowWidth;

        private bool _hasDraged = false;

        private int _useButtonIndex = 0;

        private Vector2 _mouseDownPosition;

        private TransitionLineTreeItemBase _dealTreeItem = null;

        private int _dealIndex = 0;

        public override void OnGUI(Rect viewArea)
        {
            base.OnGUI(viewArea);
            if (_timelineTickStyle == null)
            {
                _timelineTickStyle = "AnimationTimelineTick";
            }
            float tempNeedShowWidth = UIExpansionEditorUtility.TRANSITION_TIMELINE_MARGIN_LEFT +
                UIExpansionEditorUtility.TRANSITION_TIMELINE_MARGIN_RIGHT +
                UIExpansionManager.Instance.TransitionSettings.FPS * UIExpansionManager.Instance.TransitionSettings.FrameWidth * UIExpansionManager.Instance.TransitionSettings.NeedDrawTime;
            _needShowWidth = Mathf.Max(_viewArea.width, tempNeedShowWidth);
            GUI.BeginGroup(_viewArea, string.Empty);
            DrawDetailItems();
            DrawFrameGUI();
            GUI.EndGroup();
            HandleInput();
        }

        private void DrawFrameGUI()
        {
            Color lineColor = _timelineTickStyle.normal.textColor;
            lineColor.a = 0.2f;
            Handles.color = lineColor;
            float curCheckValue = UIExpansionEditorUtility.TRANSITION_TIMELINE_MARGIN_LEFT;
            int curCheckFrame = 0;
            while (curCheckValue <= _needShowWidth)
            {
                if (curCheckFrame != UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex &&
                    curCheckFrame != UIExpansionManager.Instance.TransitionSettings.NeedDrawTime * UIExpansionManager.Instance.TransitionSettings.FPS &&
                    curCheckFrame != UIExpansionManager.Instance.CurTransitionWrapper.TotalFrameNum)
                {
                    Handles.DrawLine(new Vector3(curCheckValue + UIExpansionManager.Instance.TransitionSettings.PanelOffsetX, 0, 0f),
                        new Vector3(curCheckValue + UIExpansionManager.Instance.TransitionSettings.PanelOffsetX, _viewArea.height, 0f));
                }
                curCheckValue += UIExpansionManager.Instance.TransitionSettings.FrameWidth;
                curCheckFrame++;
            }

            if (UIExpansionManager.Instance.TransitionSettings.NeedDrawTime * UIExpansionManager.Instance.TransitionSettings.FPS != UIExpansionManager.Instance.CurTransitionWrapper.TotalFrameNum &&
                UIExpansionManager.Instance.TransitionSettings.NeedDrawTime * UIExpansionManager.Instance.TransitionSettings.FPS != UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex)
            {
                lineColor = new Color(1, 0, 0, 1);
                Handles.color = lineColor;
                float needDrawTimePosX = UIExpansionManager.Instance.TransitionSettings.NeedDrawTime * UIExpansionManager.Instance.TransitionSettings.FPS * UIExpansionManager.Instance.TransitionSettings.FrameWidth + UIExpansionManager.Instance.TransitionSettings.PanelOffsetX + UIExpansionEditorUtility.TRANSITION_TIMELINE_MARGIN_LEFT;
                Handles.DrawLine(new Vector3(needDrawTimePosX, 0, 0f), new Vector3(needDrawTimePosX, _viewArea.height, 0f));
            }

            if (UIExpansionManager.Instance.CurTransitionWrapper.TotalFrameNum != UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex)
            {
                lineColor = new Color(0, 1, 0, 1);
                Handles.color = lineColor;
                float totalFramePosX = UIExpansionManager.Instance.CurTransitionWrapper.TotalFrameNum * UIExpansionManager.Instance.TransitionSettings.FrameWidth + UIExpansionManager.Instance.TransitionSettings.PanelOffsetX + UIExpansionEditorUtility.TRANSITION_TIMELINE_MARGIN_LEFT;
                Handles.DrawLine(new Vector3(totalFramePosX, 0, 0f), new Vector3(totalFramePosX, _viewArea.height, 0f));
            }

            lineColor = new Color(1, 1, 1, 1);
            Handles.color = lineColor;
            float framePosX = UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex * UIExpansionManager.Instance.TransitionSettings.FrameWidth + UIExpansionManager.Instance.TransitionSettings.PanelOffsetX + UIExpansionEditorUtility.TRANSITION_TIMELINE_MARGIN_LEFT;
            Handles.DrawLine(new Vector3(framePosX, 0, 0f), new Vector3(framePosX, _viewArea.height, 0f));

            if (UIExpansionManager.Instance.TransitionSettings.InPlayingMode)
            {
                lineColor = new Color(0, 0, 1, 1);
                Handles.color = lineColor;
                float playFramePosX = UIExpansionManager.Instance.TransitionSettings.CurPlayIndex * UIExpansionManager.Instance.TransitionSettings.FrameWidth * UIExpansionManager.Instance.TransitionSettings.FPS + UIExpansionManager.Instance.TransitionSettings.PanelOffsetX + UIExpansionEditorUtility.TRANSITION_TIMELINE_MARGIN_LEFT;
                Handles.DrawLine(new Vector3(playFramePosX, 0, 0f), new Vector3(playFramePosX, _viewArea.height, 0f));
            }
        }

        private void DrawDetailItems()
        {
            float height = UIExpansionManager.Instance.TransitionSettings.PanelOffsetY;
            if (UIExpansionManager.Instance.CurTransitionWrapper.ShowTreeList.Count == 0)
            {
                return;
            }
            for (int i = 0; i < UIExpansionManager.Instance.CurTransitionWrapper.ShowTreeList.Count; i++)
            {
                float itemHeight = UIExpansionManager.Instance.CurTransitionWrapper.ShowTreeList[i].Height;
                Rect itemArea = new Rect(
                    UIExpansionManager.Instance.TransitionSettings.PanelOffsetX,
                    height,
                    _needShowWidth,
                    itemHeight);
                height += itemHeight;
                UIExpansionManager.Instance.CurTransitionWrapper.ShowTreeList[i].OnDetailGUI(i, itemArea);
            }
        }

        private void HandleInput()
        {
            if (UIExpansionManager.Instance.TransitionSettings.CurOperableTimelineItem != null)
            {
                return;
            }

            int controlID = GUIUtility.GetControlID("Dynamic Effect Timeline Scroll View".GetHashCode(), FocusType.Passive, _viewArea);
            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (!_viewArea.Contains(Event.current.mousePosition))
                    {
                        return;
                    }
                    _mouseDownPosition = Event.current.mousePosition;

                    _useButtonIndex = Event.current.button;
                    GUIUtility.hotControl = controlID;
                    _hasDraged = false;
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        if (!_hasDraged)
                        {
                            switch (_useButtonIndex)
                            {
                                case 0:
                                    OnLeftMouseButtonClick();
                                    break;
                                case 1:
                                    OnRightMouseButtonClick();
                                    break;
                            }
                        }
                    }
                    return;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        // UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex = GetFrameIndexByMousePos(Event.current.mousePosition.x);
                        // UIExpansionManager.Instance.TransitionSettings.NeedRepaint = true;
                        if (Vector2.Distance(_mouseDownPosition, Event.current.mousePosition) > 4)
                        {
                            _hasDraged = true;
                        }
                        Event.current.Use();
                    }
                    return;

            }


        }

        protected void OnLeftMouseButtonClick()
        {
            Debug.Log("DynamicEffectTimelineScrollView.OnLeftMouseButtonClick");
        }

        protected void OnRightMouseButtonClick()
        {
            Debug.Log("DynamicEffectTimelineScrollView.OnRightMouseButtonClick");
            Vector2 offsetVec = new Vector2(_mouseDownPosition.x - _viewArea.x, _mouseDownPosition.y - _viewArea.y);
            if (GetCurDealTreeItemAndFrame(offsetVec))
            {
                TransitionTimelineItemBase timelineItem = _dealTreeItem .GetOperableItem(_dealIndex);
                if (timelineItem == null)
                {
                    GenericMenu genericMenu = new GenericMenu();
                    genericMenu.AddItem(new GUIContent("Add Key Frame"), false, AddKeyFrame);
                    genericMenu.DropDown(new Rect(_mouseDownPosition.x, _mouseDownPosition.y, 0, 0));
                }
            }
        }

        protected void AddKeyFrame()
        {
            TransitionTimelineItemBase timelineItem = _dealTreeItem.GetOperableItem(_dealIndex);
            if (timelineItem != null)
            {
                return;
            }
            _dealTreeItem.AddFrame(_dealIndex);
            if (UIExpansionManager.Instance.WindowSettings.AutoSave)
            {
                string failedStr = null;
                if (UIExpansionManager.Instance.CurUIExpansionWrapper.CheckCanSave(true, out failedStr))
                {
                    List<string> missLabelList = null;
                    if (UIExpansionManager.Instance.CurBindingWrapper.CheckAllOldLabelExist(out missLabelList))
                    {
                        UIExpansionManager.Instance.CurUIExpansionWrapper.Save();
                    }
                    else
                    {
                        Debug.Log(string.Format("<color=#ff0000>{0}</color>", "[TransitionTimelineScrollView的检测]"));
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("双向绑定界面有部分Label被删除：");
                        for (int i = 0; i < missLabelList.Count; i++)
                        {
                            sb.Append(missLabelList[i]);
                            sb.Append("  ");
                        }
                        sb.AppendLine();
                        sb.Append("是否继续保存？");
                        if (EditorUtility.DisplayDialog("Label丢失确认", sb.ToString(), "保存", "取消"))
                        {
                            UIExpansionManager.Instance.CurUIExpansionWrapper.Save();
                        }
                    }
                }
                else
                {
                    Debug.Log(string.Format("{0}原因: {1}", "保存失败,已退出自动保存模式", failedStr));
                }
            }
        }

        public bool GetCurDealTreeItemAndFrame(Vector2 mousePos)
        {
            TransitionTreeItemBase transitionTreeItem = null;
            Vector2 realPos = new Vector2(mousePos.x - UIExpansionManager.Instance.TransitionSettings.PanelOffsetX, mousePos.y - UIExpansionManager.Instance.TransitionSettings.PanelOffsetY);
            float startY = 0;
            for (int i = 0; i < UIExpansionManager.Instance.CurTransitionWrapper.ShowTreeList.Count; i++)
            {
                if (realPos.y <= startY + UIExpansionManager.Instance.CurTransitionWrapper.ShowTreeList[i].Height)
                {
                    transitionTreeItem = UIExpansionManager.Instance.CurTransitionWrapper.ShowTreeList[i];
                    break;
                }
                startY += UIExpansionManager.Instance.CurTransitionWrapper.ShowTreeList[i].Height;
            }

            if (transitionTreeItem == null || transitionTreeItem.Type != TransitionTreeItemType.Line)
            {
                _dealTreeItem = null;
                return false;
            }
            _dealTreeItem = transitionTreeItem as TransitionLineTreeItemBase;
            float checkX = realPos.x - UIExpansionEditorUtility.TRANSITION_TIMELINE_MARGIN_LEFT;
            _dealIndex = Mathf.RoundToInt(checkX / UIExpansionManager.Instance.TransitionSettings.FrameWidth);
            if (_dealIndex > UIExpansionManager.Instance.TransitionSettings.NeedDrawTime * UIExpansionManager.Instance.TransitionSettings.FPS)
            {
                return false;
            }
            return true;
        }
    }
}