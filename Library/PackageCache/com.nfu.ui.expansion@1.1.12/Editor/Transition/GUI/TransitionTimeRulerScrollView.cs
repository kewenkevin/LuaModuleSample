using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace ND.UI
{
    public class TransitionTimeRulerScrollView : EditorScrollViewBase
    {

        private GUIStyle _timelineTickStyle;

        private float _needShowWidth;

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
            if (tempNeedShowWidth <= _viewArea.width)
            {
                tempNeedShowWidth = _viewArea.width;
            }
            _needShowWidth = tempNeedShowWidth;// - UIExpansionEditorUtility.TRANSITION_TIMELINE_MARGIN_RIGHT;
            UpdateArrowPos();
            GUI.BeginGroup(_viewArea, EditorStyles.toolbar);
            DrawSecondGUI();
            DrawFrameGUI();
            DrawTimeText();
            DrawArrowGUI();
            GUI.EndGroup();
        }

        private void DrawSecondGUI()
        {
            Color lineColor = _timelineTickStyle.normal.textColor;
            lineColor.a = 1;
            Handles.color = lineColor;
            float curCheckValue = UIExpansionEditorUtility.TRANSITION_TIMELINE_MARGIN_LEFT;
            while (curCheckValue <= _needShowWidth)
            {
                Handles.DrawLine(new Vector3(curCheckValue + UIExpansionManager.Instance.TransitionSettings.PanelOffsetX, _viewArea.height - UIExpansionEditorUtility.SINGLELINE_SPACING, 0f),
                    new Vector3(curCheckValue + UIExpansionManager.Instance.TransitionSettings.PanelOffsetX, _viewArea.height, 0f));
                curCheckValue += UIExpansionManager.Instance.TransitionSettings.FPS * UIExpansionManager.Instance.TransitionSettings.FrameWidth;
            }
        }


        private void DrawFrameGUI()
        {
            Color lineColor = _timelineTickStyle.normal.textColor;
            lineColor.a = 0.6f;
            Handles.color = lineColor;
            float curCheckValue = UIExpansionEditorUtility.TRANSITION_TIMELINE_MARGIN_LEFT;
            int curCheckFrame = 0;
            while (curCheckValue <= _needShowWidth)
            {
                if (curCheckFrame % UIExpansionManager.Instance.TransitionSettings.FPS != 0)
                {
                    Handles.DrawLine(new Vector3(curCheckValue + UIExpansionManager.Instance.TransitionSettings.PanelOffsetX, _viewArea.height - UIExpansionEditorUtility.SINGLELINE_SPACING, 0f),
                        new Vector3(curCheckValue + UIExpansionManager.Instance.TransitionSettings.PanelOffsetX, _viewArea.height, 0f));
                }
                curCheckValue += UIExpansionManager.Instance.TransitionSettings.FrameWidth;
                curCheckFrame++;
            }
        }

        private void DrawTimeText()
        {
            Color lineColor = _timelineTickStyle.normal.textColor;
            Handles.color = lineColor;
            float curCheckValue = UIExpansionEditorUtility.TRANSITION_TIMELINE_MARGIN_LEFT;
            int curCheckFrame = 0;

            Color originColor = GUI.color;

            while (curCheckValue <= _needShowWidth)
            {
                if (curCheckFrame == UIExpansionManager.Instance.TransitionSettings.NeedDrawTime * UIExpansionManager.Instance.TransitionSettings.FPS)
                {
                    GUI.color = new Color(1, 0, 0, 2);
                }
                if (curCheckFrame == UIExpansionManager.Instance.CurTransitionWrapper.TotalFrameNum)
                {
                    GUI.color = new Color(0, 1, 0, 2);
                }
                GUI.Label(new Rect(curCheckValue + UIExpansionManager.Instance.TransitionSettings.PanelOffsetX + 3f, -3f, 40f, _viewArea.height), GetFrameText(curCheckFrame), _timelineTickStyle);
                curCheckValue += UIExpansionManager.Instance.TransitionSettings.FrameWidth;
                curCheckFrame++;
                GUI.color = originColor;
            }
        }

        private void DrawArrowGUI()
        {
            if (Event.current.type == EventType.Repaint)
            {

                Handles.color = new Color(1, 1, 1, 1);
                GUI.DrawTexture(new Rect(GetFramePosByIndex(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex) - 8f, 4f, 16f, 16f), UIExpansionEditorUtility.ArrowTexture);
            }
        }

        public void UpdateArrowPos()
        {
            if (UIExpansionManager.Instance.TransitionSettings.InPlayingMode)
            {
                return;
            }
            int controlID = GUIUtility.GetControlID("Dynamic Effect Time Ruler Scroll View".GetHashCode(), FocusType.Passive, _viewArea);
            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (!_viewArea.Contains(Event.current.mousePosition))
                    {
                        return;
                    }
                    GUIUtility.hotControl = controlID;
                    UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex = GetFrameIndexByMousePos(Event.current.mousePosition.x);
                    UIExpansionManager.Instance.NeedRepaint = true;
                    return;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                    }
                    return;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex = GetFrameIndexByMousePos(Event.current.mousePosition.x);
                        UIExpansionManager.Instance.NeedRepaint = true;
                        Event.current.Use();
                    }
                    return;
            }
            if (GUIUtility.hotControl == controlID)
            {
                Vector2 mousePosition = Event.current.mousePosition;
                GUI.FocusControl(null);
            }
        }

        public string GetFrameText(int frame)
        {
            int frameBigNum = frame / UIExpansionManager.Instance.TransitionSettings.FPS;
            int frameSmallNum = frame % UIExpansionManager.Instance.TransitionSettings.FPS;
            if (UIExpansionManager.Instance.TransitionSettings.FPS > 10)
            {
                return string.Format("{0}:{1}", frameBigNum.ToString(), frameSmallNum.ToString().PadLeft(2, '0'));
            }
            else
            {
                return string.Format("{0}:{1}", frameBigNum.ToString(), frameSmallNum.ToString());
            }
        }

        public int GetFrameIndexByMousePos(float mousePosX)
        {
            float mouseRealPos = mousePosX - _viewArea.x - UIExpansionManager.Instance.TransitionSettings.PanelOffsetX - UIExpansionEditorUtility.TRANSITION_TIMELINE_MARGIN_LEFT;
            int frameIndex = Mathf.RoundToInt(mouseRealPos / UIExpansionManager.Instance.TransitionSettings.FrameWidth);
            frameIndex = Mathf.Clamp(frameIndex, 0, UIExpansionManager.Instance.TransitionSettings.NeedDrawTime * UIExpansionManager.Instance.TransitionSettings.FPS);
            return frameIndex;
        }

        public float GetFramePosByIndex(int frameIndex)
        {
            return UIExpansionManager.Instance.TransitionSettings.FrameWidth * frameIndex + UIExpansionEditorUtility.TRANSITION_TIMELINE_MARGIN_LEFT + UIExpansionManager.Instance.TransitionSettings.PanelOffsetX;
        }
    }
}