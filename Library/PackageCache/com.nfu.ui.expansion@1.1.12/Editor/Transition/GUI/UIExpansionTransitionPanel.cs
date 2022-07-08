using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace ND.UI
{
    public class UIExpansionTransitionPanel : UIExpansionPanelBase
    {
        private TransitionTimeRulerScrollView _timeRulerView;

        private TransitionEventTimelineScrollView _eventTimelineView;

        private TransitionHeaderScrollView _headerView;

        private TransitionTimelineScrollView _timelineView;

        private Rect _previousPanelArea = default(Rect);

        private Rect _toolbarArea = default(Rect);

        private Rect _borderControlArea = default(Rect);

        private Rect _stateControlArea = default(Rect);

        private Rect _timeRulerArea = default(Rect);

        private Rect _eventToolbarArea = default(Rect);

        private Rect _eventTimelineArea = default(Rect);

        private Rect _headerArea = default(Rect);

        private Rect _timelineArea = default(Rect);

        private Rect _horizontalSliderBarArea = default(Rect);

        private Rect _verticalSliderBarArea = default(Rect);

        private float _headerAreaWidth = 256f;

        private float _resizeOffset = 0f;

        public UIExpansionTransitionPanel(UIExpansionWindow window) : base(window)
        {
            _panelName = "Transition";
            _timeRulerView = new TransitionTimeRulerScrollView();
            _eventTimelineView = new TransitionEventTimelineScrollView();
            _headerView = new TransitionHeaderScrollView();
            _timelineView = new TransitionTimelineScrollView();
        }

        public override void OnGUI(Rect panelArea)
        {
            UpdatePanelLayout(panelArea);
            DrawToolbar();
            if (UIExpansionManager.Instance.CurTransitionWrapper != null)
            {
                UpdateWrapperData();
                DrawStateControl();
                DrawEventToolbar();
                DrawBorderControlLine();
                _timeRulerView.OnGUI(_timeRulerArea);
                _eventTimelineView.OnGUI(_eventTimelineArea);
                _headerView.OnGUI(_headerArea);
                _timelineView.OnGUI(_timelineArea);
            }
        }

        private void UpdatePanelLayout(Rect panelArea)
        {
            _toolbarArea = new Rect(
                panelArea.x,
                panelArea.y,
                panelArea.width,
                UIExpansionEditorUtility.SINGLELINE_HEIGHT);
            _borderControlArea = new Rect(
                _headerAreaWidth,
                panelArea.y + UIExpansionEditorUtility.SINGLELINE_SPACING * 3,
                UIExpansionEditorUtility.BORDER_CONTROL_WIDTH,
                panelArea.height - UIExpansionEditorUtility.SINGLELINE_SPACING * 3);

            if (UpdateHeaderAreaWidth() || (panelArea != _previousPanelArea))
            {
                float timelineAreaPosX = panelArea.x + _headerAreaWidth + UIExpansionEditorUtility.BORDER_CONTROL_WIDTH;
                float timelineAreaWidth = panelArea.width - _headerAreaWidth - UIExpansionEditorUtility.BORDER_CONTROL_WIDTH - UIExpansionEditorUtility.SLIDER_BAR_WIDTH;

                _stateControlArea = new Rect(
                    panelArea.x,
                    panelArea.y + UIExpansionEditorUtility.SINGLELINE_SPACING,
                    _headerAreaWidth,
                    UIExpansionEditorUtility.SINGLELINE_HEIGHT);

                _timeRulerArea = new Rect(
                    timelineAreaPosX,
                    panelArea.y + UIExpansionEditorUtility.SINGLELINE_SPACING,
                    timelineAreaWidth,
                    UIExpansionEditorUtility.SINGLELINE_HEIGHT);

                _eventToolbarArea = new Rect(
                    panelArea.x,
                    panelArea.y + UIExpansionEditorUtility.SINGLELINE_SPACING * 2,
                    _headerAreaWidth,
                    UIExpansionEditorUtility.SINGLELINE_HEIGHT);

                _eventTimelineArea = new Rect(
                    timelineAreaPosX,
                    panelArea.y + UIExpansionEditorUtility.SINGLELINE_SPACING * 2,
                    timelineAreaWidth,
                    UIExpansionEditorUtility.SINGLELINE_SPACING);

                _headerArea = new Rect(
                    panelArea.x,
                    panelArea.y + UIExpansionEditorUtility.SINGLELINE_SPACING * 3,
                    _headerAreaWidth,
                    panelArea.height - (UIExpansionEditorUtility.SINGLELINE_SPACING) * 3 - UIExpansionEditorUtility.SLIDER_BAR_WIDTH);

                _timelineArea = new Rect(
                    timelineAreaPosX,
                    panelArea.y + (UIExpansionEditorUtility.SINGLELINE_SPACING) * 3,
                    timelineAreaWidth,
                    panelArea.height - UIExpansionEditorUtility.SINGLELINE_SPACING * 3 - UIExpansionEditorUtility.SLIDER_BAR_WIDTH);

                _horizontalSliderBarArea = new Rect(
                    timelineAreaPosX,
                    panelArea.yMax - UIExpansionEditorUtility.SLIDER_BAR_WIDTH,
                    timelineAreaWidth,
                    UIExpansionEditorUtility.SLIDER_BAR_WIDTH);

                _verticalSliderBarArea = new Rect(
                    panelArea.xMax - UIExpansionEditorUtility.SLIDER_BAR_WIDTH,
                    panelArea.y + (UIExpansionEditorUtility.SINGLELINE_SPACING),
                    UIExpansionEditorUtility.SLIDER_BAR_WIDTH,
                    panelArea.height - (UIExpansionEditorUtility.SINGLELINE_SPACING) - UIExpansionEditorUtility.SLIDER_BAR_WIDTH);
            }
            _previousPanelArea = panelArea;
        }

        private bool UpdateHeaderAreaWidth()
        {
            bool widthChanged = false;
            EditorGUIUtility.AddCursorRect(_borderControlArea, MouseCursor.ResizeHorizontal);
            int controlID = GUIUtility.GetControlID("TransitionPanelHeaderWidthChange".GetHashCode(), FocusType.Passive, _borderControlArea);
            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (_borderControlArea.Contains(Event.current.mousePosition) && Event.current.button == 0)
                    {
                        _resizeOffset = Event.current.mousePosition.x - _headerAreaWidth;
                        GUIUtility.hotControl = controlID;
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                    }
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        float tempWidth = Event.current.mousePosition.x - _resizeOffset;
                        _headerAreaWidth = Mathf.Clamp(tempWidth, 200, 400);
                        UIExpansionManager.Instance.NeedRepaint = true;
                        widthChanged = true;
                    }
                    break;
            }
            return widthChanged;
        }

        private void CreateNewTransitionWrapper()
        {
            UIExpansionManager.Instance.TransitionSettings.InCreateNewWrapperState = true;
            Rect buttonRect = new Rect(0, _toolbarArea.y, 200, UIExpansionEditorUtility.SINGLELINE_SPACING);
            // PopupWindow.Show(buttonRect, new ControllerChangeNamePopupWindow());
            PopupWindow.Show(buttonRect, new TransitionNamePopupWindow());
        }

        private void ChangeTransitionWrapperName()
        {
            UIExpansionManager.Instance.TransitionSettings.InChangeWrapperNameState = true;
            Rect buttonRect = new Rect(0, _toolbarArea.y, 200, UIExpansionEditorUtility.SINGLELINE_SPACING);
            // PopupWindow.Show(buttonRect, new ControllerChangeNamePopupWindow());
            PopupWindow.Show(buttonRect, new TransitionNamePopupWindow());
        }

        private void DrawToolbar()
        {
            GUI.Box(_toolbarArea, string.Empty, EditorStyles.toolbar);
            float leftX = _toolbarArea.x;
            float rightX = _toolbarArea.xMax - UIExpansionEditorUtility.SLIDER_BAR_WIDTH;
            float tempItemWidth = 30;

            tempItemWidth = 202;
            if (UIExpansionManager.Instance.TransitionSettings.CurTransitionIndex == -1)
            {
                if (GUI.Button(new Rect(leftX, _toolbarArea.y, tempItemWidth, 20), "Create New Transition", EditorStyles.toolbarButton))
                {
                    CreateNewTransitionWrapper();
                }
            }
            else
            {
                if (GUI.Button(new Rect(leftX, _toolbarArea.y, tempItemWidth, 20), UIExpansionManager.Instance.CurTransitionWrapper.Name, EditorStyles.toolbarDropDown))
                {
                    OnTransitionSelectBtnClick();
                }
            }
            leftX += tempItemWidth;
            if (UIExpansionManager.Instance.CurTransitionWrapper != null)
            {
            tempItemWidth = 120;
            UIExpansionManager.Instance.TransitionSettings.InRecordingState = GUI.Toggle(new Rect(leftX, _toolbarArea.y, tempItemWidth, UIExpansionEditorUtility.SINGLELINE_SPACING), UIExpansionManager.Instance.TransitionSettings.InRecordingState, "Recording", EditorStyles.toolbarButton);
            leftX += tempItemWidth;

            tempItemWidth = 120;
            GUI.Toggle(new Rect(leftX, _toolbarArea.y, tempItemWidth, 20), UIExpansionManager.Instance.TransitionSettings.InPreviewMode, "Preview", EditorStyles.toolbarButton);
            // UIExpansionManager.Instance.TransitionSettings.TransitionInPreviewMode = GUI.Toggle(new Rect(leftX, _toolbarArea.y, tempItemWidth, 20), UIExpansionManager.Instance.TransitionSettings.TransitionInPreviewMode, "Preview", EditorStyles.toolbarButton);
            leftX += tempItemWidth;

            /*
            tempItemWidth = 80;
            Color tempColor = GUI.color;
            GUI.color = UIExpansionEditorData.Instance.InPreview ? new Color(0, 1, 0, 1) : tempColor;
            UIExpansionEditorData.Instance.InPreview = GUI.Toggle(new Rect(leftX, _toolbarArea.y, tempItemWidth, 20), UIExpansionEditorData.Instance.InPreview, "PreView", EditorStyles.toolbarButton);
            GUI.color = tempColor;
            leftX += tempItemWidth;
            */
        
                tempItemWidth = 145;
                if (GUI.Button(new Rect(rightX - tempItemWidth, _toolbarArea.y, tempItemWidth, 20), "TimeLine Length: " + UIExpansionManager.Instance.TransitionSettings.NeedDrawTime.ToString().PadLeft(2, ' ') + "S", EditorStyles.toolbarDropDown))
                {
                    int[] lengthArray = new int[5] { 1, 2, 3, 5, 10 };
                    GenericMenu genericMenu = new GenericMenu();
                    for (int i = 0; i < lengthArray.Length; i++)
                    {
                        genericMenu.AddItem(new GUIContent(lengthArray[i].ToString() + "S"), lengthArray[i] == UIExpansionManager.Instance.TransitionSettings.NeedDrawTime, SelectTimeLineNeedShow, lengthArray[i]);
                    }
                    genericMenu.DropDown(new Rect(rightX - tempItemWidth + 20, _toolbarArea.y + 21, 0, 0));
                }
                rightX -= tempItemWidth;

                if (UIExpansionManager.Instance.CurTransitionWrapper.AutoPlay)
                {
                    rightX -= 10;
                    tempItemWidth = 30;
                    UIExpansionManager.Instance.CurTransitionWrapper.AutoPlayTimes = EditorGUI.IntField(new Rect(rightX - tempItemWidth, _toolbarArea.y, tempItemWidth, 20), UIExpansionManager.Instance.CurTransitionWrapper.AutoPlayTimes);
                    rightX -= tempItemWidth;

                    tempItemWidth = 70;
                    EditorGUI.LabelField(new Rect(rightX - tempItemWidth, _toolbarArea.y, tempItemWidth, 20), "Loop Time:");
                    rightX -= tempItemWidth + 10;

                    tempItemWidth = 40;
                    float tempDelayTime = EditorGUI.FloatField(new Rect(rightX - tempItemWidth, _toolbarArea.y, tempItemWidth, 20), UIExpansionManager.Instance.CurTransitionWrapper.AutoPlayDelay);
                    UIExpansionManager.Instance.CurTransitionWrapper.AutoPlayDelay = Mathf.Max(tempDelayTime, 0);
                    rightX -= tempItemWidth;

                    tempItemWidth = 75;
                    EditorGUI.LabelField(new Rect(rightX - tempItemWidth, _toolbarArea.y, tempItemWidth, 20), "Delay Time:");
                    rightX -= tempItemWidth + 10;
                }
                tempItemWidth = 80;
                UIExpansionManager.Instance.CurTransitionWrapper.AutoPlay = GUI.Toggle(new Rect(rightX - tempItemWidth, _toolbarArea.y, tempItemWidth, 20), UIExpansionManager.Instance.CurTransitionWrapper.AutoPlay, "Auto Play", EditorStyles.toolbarButton);
                rightX -= tempItemWidth;
            }
        }

        private void SelectTimeLineNeedShow(object indexObj)
        {
            int tempNeedShowIndex = (int)indexObj;
            if ((float)UIExpansionManager.Instance.CurTransitionWrapper.TotalFrameNum / (float)UIExpansionManager.Instance.TransitionSettings.FPS > tempNeedShowIndex)
            {
                return;
            }
            UIExpansionManager.Instance.TransitionSettings.NeedDrawTime = tempNeedShowIndex;
        }

        private void OnTransitionSelectBtnClick()
        {
            GenericMenu transitionListMenu = new GenericMenu();
            if (UIExpansionManager.Instance.CurUIExpansionWrapper.TransitionWrapperList.Count > 0)
            {
                for (int i = 0; i < UIExpansionManager.Instance.CurUIExpansionWrapper.TransitionWrapperList.Count; i++)
                {
                    var tw = UIExpansionManager.Instance.CurUIExpansionWrapper.TransitionWrapperList[i];
                    transitionListMenu.AddItem(new GUIContent(tw.Name), i == UIExpansionManager.Instance.TransitionSettings.CurTransitionIndex, OnSelectWrapperItem, i);
                }
                transitionListMenu.AddSeparator(string.Empty);
            }
            transitionListMenu.AddItem(new GUIContent("Create New"), false, CreateNewTransitionWrapper);
            transitionListMenu.AddItem(new GUIContent("Change Name"), false, ChangeTransitionWrapperName);
            transitionListMenu.AddItem(new GUIContent("Delete This"), false, UIExpansionManager.Instance.DeleteCurTransitionWrapper);

            transitionListMenu.DropDown(new Rect(20, 41, 0, 0));
        }

        private void OnSelectWrapperItem(object indexObj)
        {
            int wrapperIndex = (int)indexObj;
            UIExpansionManager.Instance.TransitionSettings.CurTransitionIndex = wrapperIndex;
        }

        private void UpdateWrapperData()
        {
            float needShowWidth = UIExpansionEditorUtility.TRANSITION_TIMELINE_MARGIN_LEFT +
                UIExpansionEditorUtility.TRANSITION_TIMELINE_MARGIN_RIGHT +
                UIExpansionManager.Instance.TransitionSettings.FPS * UIExpansionManager.Instance.TransitionSettings.FrameWidth * UIExpansionManager.Instance.TransitionSettings.NeedDrawTime;

            float horizontalSliderBarMax = 0;

            float horizontalSliderBarItemWidth = 0;

            if (needShowWidth < _timeRulerArea.width)
            {
                UIExpansionManager.Instance.TransitionSettings.PanelOffsetX = 0;
                horizontalSliderBarMax = _timeRulerArea.width;
                horizontalSliderBarItemWidth = _timeRulerArea.width;
            }
            else
            {
                UIExpansionManager.Instance.TransitionSettings.PanelOffsetX = Mathf.Max(_timeRulerArea.width - needShowWidth, UIExpansionManager.Instance.TransitionSettings.PanelOffsetX);
                horizontalSliderBarMax = needShowWidth;
                horizontalSliderBarItemWidth = _timeRulerArea.width;
            }
            UIExpansionManager.Instance.TransitionSettings.PanelOffsetX = -GUI.HorizontalScrollbar(
                _horizontalSliderBarArea,
                -UIExpansionManager.Instance.TransitionSettings.PanelOffsetX,
                horizontalSliderBarItemWidth, 0, horizontalSliderBarMax);

            float showTreeHeight = UIExpansionManager.Instance.CurTransitionWrapper.ShowTreeHeight + 30;
            float verticalSliderBarMax = 0;
            float verticalSliderBarItemHeight = 0;
            if (showTreeHeight < _timelineArea.height)
            {
                UIExpansionManager.Instance.TransitionSettings.PanelOffsetY = 0;
                verticalSliderBarMax = _timelineArea.height;
                verticalSliderBarItemHeight = _timelineArea.height;
            }
            else
            {
                UIExpansionManager.Instance.TransitionSettings.PanelOffsetY = Mathf.Max(_timelineArea.height - showTreeHeight, UIExpansionManager.Instance.TransitionSettings.PanelOffsetY);
                verticalSliderBarMax = showTreeHeight;
                verticalSliderBarItemHeight = _timelineArea.height;
            }
            
            Event e = Event.current;
            var delta = 0f;
            if (e.isScrollWheel)
            {
                delta = e.delta.y * UIExpansionManager.Instance.WindowSettings.MouseScrollWheelSteps;
            }
            
            UIExpansionManager.Instance.TransitionSettings.PanelOffsetY = -GUI.VerticalScrollbar(
                _verticalSliderBarArea,
                -UIExpansionManager.Instance.TransitionSettings.PanelOffsetY + delta,
                verticalSliderBarItemHeight, 0, verticalSliderBarMax);
        }

        private void DrawStateControl()
        {

            Rect backgroundArea = new Rect(_stateControlArea);
            backgroundArea.width += UIExpansionEditorUtility.BORDER_CONTROL_WIDTH;
            GUI.Box(backgroundArea, string.Empty, "TE Toolbar");

            float leftX = _stateControlArea.x;
            float rightX = _stateControlArea.xMax;
            float tempItemWidth = 24;

            if (GUI.Button(new Rect(leftX, _stateControlArea.y, tempItemWidth, 20), UIExpansionEditorUtility.FrameBackTexture, EditorStyles.toolbarButton))
            {
                UIExpansionManager.Instance.TransitionSettings.MoveToLeftRrame();
                // UIExpansionEditorData.Instance.MoveToLeftRrame();
            }
            leftX += tempItemWidth;

            bool showPlayIcon = UIExpansionManager.Instance.TransitionSettings.InPlayingMode == false || (UIExpansionManager.Instance.TransitionSettings.InPlayingMode && UIExpansionManager.Instance.TransitionSettings.InPauseState);
            if (GUI.Button(new Rect(leftX, _stateControlArea.y, tempItemWidth, 20), showPlayIcon ? UIExpansionEditorUtility.PlayButtonTexture : UIExpansionEditorUtility.PauseButtonTexture, EditorStyles.toolbarButton))
            {
                UIExpansionManager.Instance.TransitionSettings.OnPlayStateChange();
                // UIExpansionEditorData.Instance.OnPlayStateChange();
            }
            leftX += tempItemWidth;

            if (GUI.Button(new Rect(leftX, _stateControlArea.y, tempItemWidth, 20), UIExpansionEditorUtility.StopButtonTexture, EditorStyles.toolbarButton))
            {
                UIExpansionManager.Instance.TransitionSettings.StopPlay();
                // UIExpansionEditorData.Instance.StopPlay(true);
            }
            leftX += tempItemWidth;

            if (GUI.Button(new Rect(leftX, _stateControlArea.y, tempItemWidth, 20), UIExpansionEditorUtility.FrameForwardTexture, EditorStyles.toolbarButton))
            {
                // UIExpansionEditorData.Instance.MoveToRightFrame();
                UIExpansionManager.Instance.TransitionSettings.MoveToRightFrame();
            }

            string totalFrameStr = UIExpansionManager.Instance.CurTransitionWrapper.TotalFrameNum.ToString();
            tempItemWidth = (totalFrameStr.Length) * 10 + 15;
            EditorGUI.LabelField(new Rect(rightX - tempItemWidth, _stateControlArea.y, tempItemWidth, 20), " /  " + totalFrameStr);
            rightX -= tempItemWidth;


            if (UIExpansionManager.Instance.TransitionSettings.InPlayingMode)
            {
                tempItemWidth = 30;
                EditorGUI.LabelField(new Rect(rightX - tempItemWidth, _stateControlArea.y + 1, tempItemWidth, 18), (UIExpansionManager.Instance.TransitionSettings.CurPlayIndex * UIExpansionManager.Instance.TransitionSettings.FPS).ToString("0.00"));
                rightX -= tempItemWidth;
            }
            else
            {
                tempItemWidth = 30;
                int tempFrameValue = EditorGUI.IntField(new Rect(rightX - tempItemWidth, _stateControlArea.y + 1, tempItemWidth, 18), UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
                tempFrameValue = Mathf.Clamp(tempFrameValue, 0, UIExpansionManager.Instance.TransitionSettings.NeedDrawTime * UIExpansionManager.Instance.TransitionSettings.FPS);
                UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex = tempFrameValue;
                rightX -= tempItemWidth;
            }

        }

        private void DrawEventToolbar()
        {
            Rect backgroundArea = new Rect(_eventToolbarArea);
            backgroundArea.width += UIExpansionEditorUtility.BORDER_CONTROL_WIDTH;
            GUI.Box(backgroundArea, string.Empty, EditorStyles.toolbar);
            /*
            if (GUI.Button(new Rect(_eventToolbarArea.width - 22, _eventToolbarArea.y, 24, 20), UIExpansionEditorUtility.AddIconTex, EditorStyles.toolbarButton))
            {

            }
            */
            float leftX = _eventToolbarArea.x;
            float rightX = _eventToolbarArea.xMax;
            float tempItemWidth = 0;
            TimelineKeyFrameItem frameItem = UIExpansionManager.Instance.CurTransitionWrapper.EventLineTreeItem.GetFrameItem(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
            if (frameItem != null)
            {
                tempItemWidth = 160;
                string tempName = GUI.TextField(new Rect(leftX, _eventToolbarArea.y, tempItemWidth, UIExpansionEditorUtility.SINGLELINE_SPACING), frameItem.GetStrValue());
                if (tempName != frameItem.GetStrValue() && (!string.IsNullOrEmpty(tempName)))
                {
                    frameItem.StrValue = tempName;
                }

                if (GUI.Button(new Rect(_eventToolbarArea.width - 22, _eventToolbarArea.y, 24, 20), UIExpansionEditorUtility.MinusIconTex, EditorStyles.toolbarButton))
                {
                    UIExpansionManager.Instance.CurTransitionWrapper.EventLineTreeItem.RemoveFrame(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
                }
            }
            else
            {
                tempItemWidth = 160;
                UIExpansionManager.Instance.TransitionSettings.TempAddEventName = GUI.TextField(new Rect(leftX, _eventToolbarArea.y, tempItemWidth, UIExpansionEditorUtility.SINGLELINE_SPACING), UIExpansionManager.Instance.TransitionSettings.TempAddEventName);

                if (GUI.Button(new Rect(_eventToolbarArea.width - 22, _eventToolbarArea.y, 24, 20), UIExpansionEditorUtility.AddIconTex, EditorStyles.toolbarButton))
                {
                    if (!string.IsNullOrEmpty(UIExpansionManager.Instance.TransitionSettings.TempAddEventName))
                    {
                        UIExpansionManager.Instance.CurTransitionWrapper.EventLineTreeItem.AddFrame(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
                    }
                }
            }
        }

        private void DrawBorderControlLine()
        {
            GUI.DrawTexture(_borderControlArea, UIExpansionEditorUtility.CutlineTex);
        }

        public override void OnCurDealUIChanged()
        {
           
        }

        public override void OnEnter()
        {
          
        }

        public override void OnExit()
        {
        
        }

        public override void OnUpdate(float deltaTime)
        {
            TweenManager.Instance.EditorUpdate(deltaTime);
            UIExpansionManager.Instance.TransitionSettings.OnUpdate(deltaTime);
            if (UIExpansionManager.Instance.TransitionSettings.InRecordingState)
            {
                int[] selecteds = Selection.instanceIDs;
                for (int i = 0; i < selecteds.Length; i++)
                {
                    UIExpansionManager.Instance.CurTransitionWrapper.MonitorGameObjectChange(selecteds[i]);
                }
                UIExpansionManager.Instance.NeedRepaint = true;
            }
        }
    }
}