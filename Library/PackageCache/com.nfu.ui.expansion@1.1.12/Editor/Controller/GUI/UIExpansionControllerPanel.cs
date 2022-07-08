using UnityEngine;
using UnityEditor;

namespace ND.UI
{
    public class UIExpansionControllerPanel : UIExpansionPanelBase
    {
        private ControllerPageScrollView _pageView;

        private ControllerHeaderScrollView _headerView;

        private ControllerDetailScrollView _detailView;

        private Rect _previousPanelArea = default(Rect);

        private Rect _borderControlArea = default(Rect);

        private Rect _toolbarArea = default(Rect);

        private Rect _secondToolbarArea = default(Rect);

        private Rect _headerArea = default(Rect);

        private Rect _pageNameArea = default(Rect);

        private Rect _detailArea = default(Rect);

        private Rect _horizontalSliderBarArea = default(Rect);

        private Rect _verticalSliderBarArea = default(Rect);

        private string _tempControllerWrapperName;

        private string _newPageName;

        private float _headerAreaWidth = 198;

        private float _headerAreaMinWidth = 198;

        private float _headerAreaMaxWidth = 298;

        private float _borderResizeOffsetX = 0f;

        public UIExpansionControllerPanel(UIExpansionWindow window) : base(window)
        {
            _panelName = "Controller";
            _pageView = new ControllerPageScrollView();
            _headerView = new ControllerHeaderScrollView();
            _detailView = new ControllerDetailScrollView();
        }

        public override void OnCurDealUIChanged()
        {
            
        }

        public override void OnEnter()
        {
            
        }

        public override void OnExit()
        {
            UIExpansionManager.Instance.ControllerSettings.ExitAllState();
        }

        public override void OnGUI(Rect panelArea)
        {
            UpdatePanelLayout(panelArea);
            DrawToolbarArea();
            if (UIExpansionManager.Instance.CurControllerWrapper != null)
            {
                UpdateWrapperData();
                DrawSecondToolbar();
                DrawBorderLine();
                _pageView.OnGUI(_pageNameArea);
                _headerView.OnGUI(_headerArea);
                _detailView.OnGUI(_detailArea);
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            if (UIExpansionManager.Instance.ControllerSettings.InRecordingState)
            {
                int[] selecteds = Selection.instanceIDs;
                for (int i = 0; i < selecteds.Length; i++)
                {
                    UIExpansionManager.Instance.CurControllerWrapper.MonitorGameObjectChange(selecteds[i]);
                }
            }
        }

        private void UpdateWrapperData()
        {
            float pageNameListRealWidth = UIExpansionManager.Instance.CurControllerWrapper.PageNameList.Count * UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH;
            float horizontalSliderBarMax = 0;
            float horizontalSliderBarItemWidth = 0;
            if (pageNameListRealWidth < _pageNameArea.width)
            {
                UIExpansionManager.Instance.ControllerSettings.PanelOffsetX = 0;
                horizontalSliderBarMax = _pageNameArea.width;
                horizontalSliderBarItemWidth = _pageNameArea.width;
            }
            else
            {
                UIExpansionManager.Instance.ControllerSettings.PanelOffsetX = Mathf.Max(_pageNameArea.width - pageNameListRealWidth, UIExpansionManager.Instance.ControllerSettings.PanelOffsetX);
                horizontalSliderBarMax = pageNameListRealWidth;
                horizontalSliderBarItemWidth = _pageNameArea.width;
            }
            UIExpansionManager.Instance.ControllerSettings.PanelOffsetX = -GUI.HorizontalScrollbar(
                _horizontalSliderBarArea,
                -UIExpansionManager.Instance.ControllerSettings.PanelOffsetX,
                horizontalSliderBarItemWidth, 0, horizontalSliderBarMax);

            // UIExpansionEditorData.Instance.ControllerWrapper.RebuildDisplayTree();
            float showTreeHeight = UIExpansionManager.Instance.CurControllerWrapper.ShowTreeHeight + 30;
            float verticalSliderBarMax = 0;
            float verticalSliderBarItemHeight = 0;
            if (showTreeHeight < _detailArea.height)
            {
                UIExpansionManager.Instance.ControllerSettings.PanelOffsetY = 0;
                verticalSliderBarMax = _detailArea.height;
                verticalSliderBarItemHeight = _detailArea.height;
            }
            else
            {
                UIExpansionManager.Instance.ControllerSettings.PanelOffsetY = Mathf.Max(_detailArea.height - showTreeHeight, UIExpansionManager.Instance.ControllerSettings.PanelOffsetY);
                verticalSliderBarMax = showTreeHeight;
                verticalSliderBarItemHeight = _detailArea.height;
            }
            
            Event e = Event.current;
            var delta = 0f;
            if (e.isScrollWheel)
            {
                delta = e.delta.y * UIExpansionManager.Instance.WindowSettings.MouseScrollWheelSteps;
            }
            
            UIExpansionManager.Instance.ControllerSettings.PanelOffsetY = -GUI.VerticalScrollbar(
                _verticalSliderBarArea,
                -UIExpansionManager.Instance.ControllerSettings.PanelOffsetY + delta,
                verticalSliderBarItemHeight, 0, verticalSliderBarMax);
        }

        private void DrawSecondToolbar()
        {
            Rect backgroundArea = new Rect(_secondToolbarArea);
            backgroundArea.width += UIExpansionEditorUtility.BORDER_CONTROL_WIDTH;
            GUI.Box(backgroundArea, string.Empty, "TE Toolbar");

            float leftX = _secondToolbarArea.x;
            float rightX = _secondToolbarArea.xMax;
            float tempItemWidth = 30;
            /*
            tempItemWidth = 20;
            if (GUI.Button(new Rect(rightX - tempItemWidth, _secondToolbarArea.y, tempItemWidth, UIExpansionEditorUtility.SINGLELINE_SPACING), UIExpansionEditorUtility.AddIconTex, EditorStyles.toolbarButton))
            {
                UIExpansionManager.Instance.CurControllerWrapper.AddNewPage(_newPageName);
                _newPageName = string.Empty;
            }
            rightX -= tempItemWidth;

            tempItemWidth = 160;
            _newPageName = GUI.TextField(new Rect(leftX, _secondToolbarArea.y, rightX, UIExpansionEditorUtility.SINGLELINE_SPACING), _newPageName);
            leftX += tempItemWidth;
            */
        }

        private void UpdatePanelLayout(Rect panelArea)
        {
            _toolbarArea = new Rect(
                panelArea.x, 
                panelArea.y, 
                panelArea.width, 
                UIExpansionEditorUtility.SINGLELINE_SPACING);

            _borderControlArea = new Rect(
                panelArea.x + _headerAreaWidth,
                panelArea.y + UIExpansionEditorUtility.SINGLELINE_SPACING,
                UIExpansionEditorUtility.BORDER_CONTROL_WIDTH,
                panelArea.height - UIExpansionEditorUtility.SINGLELINE_SPACING);

            if (UpdateHeaderAreaWidth() || panelArea != _previousPanelArea)
            {
                float rightAreaPosX = panelArea.x + _headerAreaWidth + UIExpansionEditorUtility.BORDER_CONTROL_WIDTH;
                float rightAreaWidth = panelArea.xMax - rightAreaPosX - UIExpansionEditorUtility.SLIDER_BAR_WIDTH;

                _secondToolbarArea = new Rect(
                    panelArea.x,
                    panelArea.y + UIExpansionEditorUtility.SINGLELINE_SPACING,
                    _headerAreaWidth,
                    UIExpansionEditorUtility.SINGLELINE_SPACING);

                _headerArea = new Rect(
                    panelArea.x,
                    panelArea.y + UIExpansionEditorUtility.SINGLELINE_SPACING * 2,
                    _headerAreaWidth,
                    panelArea.height - UIExpansionEditorUtility.SINGLELINE_SPACING * 2 - UIExpansionEditorUtility.SINGLELINE_SPACING);

                _pageNameArea = new Rect(
                    rightAreaPosX,
                    panelArea.y + UIExpansionEditorUtility.SINGLELINE_SPACING,
                    rightAreaWidth,
                    UIExpansionEditorUtility.SINGLELINE_SPACING);

                _detailArea = new Rect(
                    rightAreaPosX,
                    panelArea.y + UIExpansionEditorUtility.SINGLELINE_SPACING * 2,
                    rightAreaWidth,
                    panelArea.height - UIExpansionEditorUtility.SINGLELINE_SPACING * 2 - UIExpansionEditorUtility.SINGLELINE_SPACING);

                _horizontalSliderBarArea = new Rect(
                    rightAreaPosX,
                    panelArea.yMax - UIExpansionEditorUtility.SLIDER_BAR_WIDTH,
                    rightAreaWidth,
                    UIExpansionEditorUtility.SLIDER_BAR_WIDTH);

                _verticalSliderBarArea = new Rect(
                    panelArea.xMax - UIExpansionEditorUtility.SLIDER_BAR_WIDTH,
                    panelArea.y + UIExpansionEditorUtility.SINGLELINE_SPACING,
                    UIExpansionEditorUtility.SLIDER_BAR_WIDTH,
                    panelArea.height - UIExpansionEditorUtility.SINGLELINE_SPACING - UIExpansionEditorUtility.SLIDER_BAR_WIDTH);
            }
            _previousPanelArea = panelArea;
        }

        private bool UpdateHeaderAreaWidth()
        {
            bool widthChanged = false;
            EditorGUIUtility.AddCursorRect(_borderControlArea, MouseCursor.ResizeHorizontal);
            int controlID = GUIUtility.GetControlID("Controller Panel Header Width Change".GetHashCode(), FocusType.Passive, _borderControlArea);
            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (_borderControlArea.Contains(Event.current.mousePosition) && Event.current.button == 0)
                    {
                        _borderResizeOffsetX = Event.current.mousePosition.x - _headerAreaWidth;
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
                        float tempWidth = Event.current.mousePosition.x - _borderResizeOffsetX;
                        _headerAreaWidth = Mathf.Clamp(Event.current.mousePosition.x, _headerAreaMinWidth, _headerAreaMaxWidth);
                        UIExpansionManager.Instance.NeedRepaint = true;
                        widthChanged = true;
                    }
                    break;
            }
            return widthChanged;
        }

        private void DrawToolbarArea()
        {
            GUI.Box(_toolbarArea, string.Empty, EditorStyles.toolbar);
            float leftX = _toolbarArea.x;
            float rightX = _toolbarArea.xMax;
            float tempItemWidth = 0;

            /*
            if (UIExpansionManager.Instance.ControllerSettings.InChangeNameState)
            {
                tempItemWidth = 160;
                _tempControllerWrapperName = GUI.TextField(new Rect(leftX, _toolbarArea.y, tempItemWidth, UIExpansionEditorUtility.SINGLELINE_SPACING), _tempControllerWrapperName);
                leftX += tempItemWidth;

                tempItemWidth = 20;
                if (GUI.Button(new Rect(leftX, _toolbarArea.y, tempItemWidth, UIExpansionEditorUtility.SINGLELINE_SPACING), "+", EditorStyles.toolbarButton))
                {
                    UIExpansionManager.Instance.CurControllerWrapper.ChangeCurControllerName(_tempControllerWrapperName);
                    UIExpansionManager.Instance.ControllerSettings.InChangeNameState = false;
                }
                leftX += tempItemWidth;

                tempItemWidth = 20;
                if (GUI.Button(new Rect(leftX, _toolbarArea.y, tempItemWidth, UIExpansionEditorUtility.SINGLELINE_SPACING), "-", EditorStyles.toolbarButton))
                {

                    UIExpansionManager.Instance.ControllerSettings.InChangeNameState = false;
                }
                leftX += tempItemWidth;
            }
            else
            {
                tempItemWidth = 200;
                if (UIExpansionManager.Instance.ControllerSettings.CurControllerIndex == -1)
                {
                    if (GUI.Button(new Rect(leftX, _toolbarArea.y, tempItemWidth, UIExpansionEditorUtility.SINGLELINE_SPACING), "Create New Controller", EditorStyles.toolbarButton))
                    {
                        UIExpansionManager.Instance.CreateNewControllerWrapper();
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(leftX, _toolbarArea.y, tempItemWidth, UIExpansionEditorUtility.SINGLELINE_SPACING), UIExpansionManager.Instance.CurControllerWrapper.Name, EditorStyles.toolbarDropDown))
                    {
                        OnSelectControllerBtnClick();
                    }
                }
                leftX += tempItemWidth;
            }
            */

            tempItemWidth = 200;
            if (UIExpansionManager.Instance.ControllerSettings.CurControllerIndex == -1)
            {
                if (GUI.Button(new Rect(leftX, _toolbarArea.y, tempItemWidth, UIExpansionEditorUtility.SINGLELINE_SPACING), "Create New Controller", EditorStyles.toolbarButton))
                {

                    // PopupWindow.Show(buttonRect, new ControllerChangeNamePopupWindow());
                    // UIExpansionManager.Instance.CreateNewControllerWrapper();
                    CreateNewWrapper();
                }
            }
            else
            {
                if (GUI.Button(new Rect(leftX, _toolbarArea.y, tempItemWidth, UIExpansionEditorUtility.SINGLELINE_SPACING), UIExpansionManager.Instance.CurControllerWrapper.Name, EditorStyles.toolbarDropDown))
                {
                    OnSelectControllerBtnClick();
                }
            }
            leftX += tempItemWidth;

            if (UIExpansionManager.Instance.CurControllerWrapper == null)
            {
                return;
            }
            Color tempColor = GUI.color;
            
            if (UIExpansionManager.Instance.ControllerSettings.InRecordingState)
            {
                GUI.color = new Color(0.5f, 1, 1, 1f);
            }
            tempItemWidth = 120;
            UIExpansionManager.Instance.ControllerSettings.InRecordingState = GUI.Toggle(new Rect(leftX, _toolbarArea.y, tempItemWidth, UIExpansionEditorUtility.SINGLELINE_SPACING), UIExpansionManager.Instance.ControllerSettings.InRecordingState, "Recording", EditorStyles.toolbarButton);
            leftX += tempItemWidth;// + 10;
            GUI.color = tempColor;

            if (UIExpansionManager.Instance.ControllerSettings.AutoApply)
            {
                GUI.color = new Color(0.5f, 1, 1, 1f);
            }
            tempItemWidth = 120;
            UIExpansionManager.Instance.ControllerSettings.AutoApply = GUI.Toggle(new Rect(leftX, _toolbarArea.y, tempItemWidth, UIExpansionEditorUtility.SINGLELINE_SPACING), UIExpansionManager.Instance.ControllerSettings.AutoApply, "Auto Apply", EditorStyles.toolbarButton);
            leftX += tempItemWidth + 10;
            GUI.color = tempColor;
            if (!UIExpansionManager.Instance.ControllerSettings.AutoApply && !UIExpansionManager.Instance.ControllerSettings.InRecordingState)
            {
                tempItemWidth = 80;
                if (GUI.Button(new Rect(leftX, _toolbarArea.y, tempItemWidth, UIExpansionEditorUtility.SINGLELINE_SPACING), "Apply", EditorStyles.toolbarButton))
                {

                    UIExpansionManager.Instance.CurControllerWrapper.Apply();
                }
                leftX += tempItemWidth;
            }
        }

        private void OnSelectControllerBtnClick()
        {
            GenericMenu controllerListMenu = new GenericMenu();
            if (UIExpansionManager.Instance.CurUIExpansionWrapper.ControllerWrapperList.Count > 0)
            {
                for(int i = 0;i < UIExpansionManager.Instance.CurUIExpansionWrapper.ControllerWrapperList.Count; i++)
                {
                    var controllerWrapper = UIExpansionManager.Instance.CurUIExpansionWrapper.ControllerWrapperList[i];
                    controllerListMenu.AddItem(new GUIContent(controllerWrapper.Name), i == UIExpansionManager.Instance.ControllerSettings.CurControllerIndex, OnChangeSelectWrapperBtnClick, i);
                }
                controllerListMenu.AddSeparator(string.Empty);
            }
            controllerListMenu.AddItem(new GUIContent("Create New"), false, CreateNewWrapper);
            controllerListMenu.AddItem(new GUIContent("Edit This"), false, ChangeWrapperInChangeNameState);
            controllerListMenu.AddItem(new GUIContent("Delete This"), false, UIExpansionManager.Instance.DeleteCurControllerWrapper);
            controllerListMenu.ShowAsContext();
        }

        private void CreateNewWrapper()
        {
            UIExpansionManager.Instance.ControllerSettings.InCreateNewWrapperState = true;
            Rect buttonRect = new Rect(0, _toolbarArea.y, 200, UIExpansionEditorUtility.SINGLELINE_SPACING);
            var pos = EditorWindow.GetWindow<UIExpansionWindow>().position.position;
            pos.y += 42;
            EditorWindow.GetWindow<ControllerWrapperDetailPopupWindow>().position = new Rect(pos,new Vector2(500, 190));
            
        }

        private void ChangeWrapperInChangeNameState()
        {
            UIExpansionManager.Instance.ControllerSettings.InEditState = true;
            Rect buttonRect = new Rect(0, _toolbarArea.y, 200, UIExpansionEditorUtility.SINGLELINE_SPACING);
            var pos = EditorWindow.GetWindow<UIExpansionWindow>().position.position;
            pos.y += 42;
            EditorWindow.GetWindow<ControllerWrapperDetailPopupWindow>().position = new Rect(pos,new Vector2(500, 190));
        }

        private void OnChangeSelectWrapperBtnClick(object indexObj)
        {
            int wrapperIndex = (int)indexObj;
            UIExpansionManager.Instance.ControllerSettings.ChangeCurControllerWrapper(wrapperIndex);
        }

        private void DrawBorderLine()
        {
            Rect textureArea = new Rect(_borderControlArea);
            textureArea.y += UIExpansionEditorUtility.SINGLELINE_SPACING;
            textureArea.height -= UIExpansionEditorUtility.SINGLELINE_SPACING;
            GUI.DrawTexture(textureArea, UIExpansionEditorUtility.CutlineTex);
        }
    }
}