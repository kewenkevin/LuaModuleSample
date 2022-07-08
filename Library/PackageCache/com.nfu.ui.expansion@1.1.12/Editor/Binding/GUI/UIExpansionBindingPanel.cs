using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;


namespace ND.UI
{
    public class UIExpansionBindingPanel : UIExpansionPanelBase
    {
        private BindingDetailScrollView _detailView;

        private BindingColumnsView _columnsView;

        private Rect _previousPanelArea = default(Rect);

        private Rect _toolbarArea = default(Rect);

        private Rect _detailArea = default(Rect);

        private Rect _columnArea = default(Rect);

        private Rect _verticalSliderBarArea = default(Rect);

        private Rect _horizontalSliderBarArea = default(Rect);

        public UIExpansionBindingPanel(UIExpansionWindow window) : base(window)
        {
            _panelName = "Binding";
            _detailView = new BindingDetailScrollView();
            _columnsView = new BindingColumnsView();
        }

        public override void OnCurDealUIChanged()
        {
            
        }

        public override void OnEnter()
        {
            UIExpansionManager.Instance.CurBindingWrapper.CheckAllLabel();
        }

        public override void OnExit()
        {
            
        }

        public override void OnGUI(Rect panelArea)
        {
            UpdatePanelLayout(panelArea);
            DrawToolbarArea();
            if (UIExpansionManager.Instance.CurBindingWrapper != null)
            {
                switch (UIExpansionManager.Instance.BindingSettings.WorkType)
                {
                    case BindingPanelWorkTypeState.Edit:
                        _columnsView.OnGUI(_columnArea);
                        break;
                    case BindingPanelWorkTypeState.Display:
                        UpdateWrapperData();
                        _detailView.OnGUI(_detailArea);
                        break;
                    /*
                    case 
                        _columnsView.OnGUI(_columnArea);
                        break;
                    case BindingPanelShowTypeState.Detail:
                        UpdateWrapperData();
                        _detailView.OnGUI(_detailArea);
                        break;*/
                }
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            
        }

        private void UpdatePanelLayout(Rect panelArea)
        {
            _toolbarArea = new Rect(
                panelArea.x,
                panelArea.y,
                panelArea.width,
                UIExpansionEditorUtility.SINGLELINE_HEIGHT);

            if (panelArea != _previousPanelArea)
            {
                _detailArea = new Rect(
                    panelArea.x,
                    panelArea.y + UIExpansionEditorUtility.SINGLELINE_SPACING,
                    panelArea.width - UIExpansionEditorUtility.SLIDER_BAR_WIDTH,
                    panelArea.height - UIExpansionEditorUtility.SINGLELINE_SPACING - UIExpansionEditorUtility.SLIDER_BAR_WIDTH);

                _columnArea = new Rect(
                    panelArea.x,
                    panelArea.y + UIExpansionEditorUtility.SINGLELINE_SPACING,
                    panelArea.width,
                    panelArea.height - UIExpansionEditorUtility.SINGLELINE_SPACING);

                _horizontalSliderBarArea = new Rect(
                  panelArea.x,
                  panelArea.yMax - UIExpansionEditorUtility.SLIDER_BAR_WIDTH,
                  panelArea.width - UIExpansionEditorUtility.SLIDER_BAR_WIDTH,
                  UIExpansionEditorUtility.SLIDER_BAR_WIDTH);

                _verticalSliderBarArea = new Rect(
                    panelArea.xMax - UIExpansionEditorUtility.SLIDER_BAR_WIDTH,
                    panelArea.y + UIExpansionEditorUtility.SINGLELINE_SPACING,
                    UIExpansionEditorUtility.SLIDER_BAR_WIDTH,
                    panelArea.height - UIExpansionEditorUtility.SINGLELINE_SPACING - UIExpansionEditorUtility.SLIDER_BAR_WIDTH);
            }
            _previousPanelArea = panelArea;
        }

        private void UpdateWrapperData()
        {
            float titleListRealWidth = 0; // todo...
            float horizontalSliderBarMax = 0;
            float horizontalSliderBarItemWidth = 0;
            if (titleListRealWidth < _detailArea.width)
            {
                UIExpansionManager.Instance.BindingSettings.PanelOffsetX = 0;
                horizontalSliderBarMax = _detailArea.width;
                horizontalSliderBarItemWidth = _detailArea.width;
            }
            else
            {
                UIExpansionManager.Instance.BindingSettings.PanelOffsetX = Mathf.Max(_detailArea.width - titleListRealWidth, UIExpansionManager.Instance.BindingSettings.PanelOffsetX);
                horizontalSliderBarMax = titleListRealWidth;
                horizontalSliderBarItemWidth = _detailArea.width;
            }
            UIExpansionManager.Instance.BindingSettings.PanelOffsetX = -GUI.HorizontalScrollbar(
                _horizontalSliderBarArea,
                -UIExpansionManager.Instance.BindingSettings.PanelOffsetX,
                horizontalSliderBarItemWidth, 0, horizontalSliderBarMax);


            float showTreeHeight = UIExpansionManager.Instance.CurBindingWrapper.ShowTreeHeight;
            float verticalSliderBarMax = 0;
            float verticalSliderBarItemHeight = 0;
            if (showTreeHeight < _detailArea.height)
            {
                UIExpansionManager.Instance.BindingSettings.PanelOffsetY = 0;
                verticalSliderBarMax = _detailArea.height;
                verticalSliderBarItemHeight = _detailArea.height;
            }
            else
            {
                UIExpansionManager.Instance.BindingSettings.PanelOffsetY = Mathf.Max(_detailArea.height - showTreeHeight, UIExpansionManager.Instance.BindingSettings.PanelOffsetY);
                verticalSliderBarMax = showTreeHeight;
                verticalSliderBarItemHeight = _detailArea.height;
            }
            
            Event e = Event.current;
            var delta = 0f;
            if (e.isScrollWheel)
            {
                delta = e.delta.y * UIExpansionManager.Instance.WindowSettings.MouseScrollWheelSteps;
            }
            
            UIExpansionManager.Instance.BindingSettings.PanelOffsetY = -GUI.VerticalScrollbar(
                _verticalSliderBarArea,
                -UIExpansionManager.Instance.BindingSettings.PanelOffsetY + delta,
                verticalSliderBarItemHeight, 0, verticalSliderBarMax);
        }

        private void DrawToolbarArea()
        {
            GUI.Box(_toolbarArea, string.Empty, EditorStyles.toolbar);
            float leftX = _toolbarArea.x;
            float rightX = _toolbarArea.xMax;
            float tempItemWidth = 0;

            tempItemWidth = 100;
            bool inEdit = UIExpansionManager.Instance.BindingSettings.WorkType == BindingPanelWorkTypeState.Edit;
            inEdit = GUI.Toggle(new Rect(leftX, _toolbarArea.y, tempItemWidth, UIExpansionEditorUtility.SINGLELINE_SPACING), inEdit, "Edit Mode", EditorStyles.toolbarButton);
            if (inEdit != (UIExpansionManager.Instance.BindingSettings.WorkType == BindingPanelWorkTypeState.Edit))
            {
                UIExpansionManager.Instance.BindingSettings.WorkType = BindingPanelWorkTypeState.Edit;
            }
            leftX += tempItemWidth;

            tempItemWidth = 100;
            bool inDisplay = UIExpansionManager.Instance.BindingSettings.WorkType == BindingPanelWorkTypeState.Display;
            inDisplay = GUI.Toggle(new Rect(leftX, _toolbarArea.y, tempItemWidth, UIExpansionEditorUtility.SINGLELINE_SPACING), inDisplay, "Display Mode", EditorStyles.toolbarButton);
            if (inDisplay != (UIExpansionManager.Instance.BindingSettings.WorkType == BindingPanelWorkTypeState.Display))
            {
                UIExpansionManager.Instance.BindingSettings.WorkType = BindingPanelWorkTypeState.Display;
            }
            leftX += tempItemWidth;

            //搜索标签功能
            if (inDisplay)
            {
                tempItemWidth = 300;
                UIExpansionManager.Instance.BindingSettings.SearchStr = GUI.TextField(new Rect(leftX, _toolbarArea.y, tempItemWidth, UIExpansionEditorUtility.SINGLELINE_SPACING),
                    UIExpansionManager.Instance.BindingSettings.SearchStr);
            
                leftX += tempItemWidth;
            
                tempItemWidth = 100;
                if (GUI.Button(new Rect(leftX, _toolbarArea.y, tempItemWidth, UIExpansionEditorUtility.SINGLELINE_SPACING),"Search"))
                {
                    if (string.IsNullOrEmpty(UIExpansionManager.Instance.BindingSettings.SearchStr))
                    {
                        UIExpansionManager.Instance.BindingSettings.IsSearching = BindingPanelIsSearchingState.False;
                        return;
                    }
                    UIExpansionManager.Instance.BindingSettings.IsSearching = BindingPanelIsSearchingState.True;
                    Debug.Log($"<color=#ADFF2F>{UIExpansionManager.Instance.BindingSettings.SearchStr}</color>");
                }
                leftX += tempItemWidth;
            
                tempItemWidth = 100;
                if (GUI.Button(new Rect(leftX, _toolbarArea.y, tempItemWidth, UIExpansionEditorUtility.SINGLELINE_SPACING),"Clear"))
                {
                    UIExpansionManager.Instance.BindingSettings.IsSearching = BindingPanelIsSearchingState.False;
                    UIExpansionManager.Instance.BindingSettings.SearchStr = "";
                    Debug.Log($"<color=#ADFF2F>{UIExpansionManager.Instance.BindingSettings.SearchStr}</color>");
                }
                leftX += tempItemWidth;
            }
            
            leftX += 60;
       
            if(UIExpansionManager.Instance.CurBindingWrapper.IllegalLabelTreeItemList.Count > 0|| UIExpansionManager.Instance.CurBindingWrapper.RepeatLabelNum > 0|| UIExpansionManager.Instance.CurBindingWrapper.IllegalMethodLabelTreeItemList.Count > 0 )
            {
                Color tempColor = GUI.color;
                GUI.color = new Color(1, 0, 0, 1);
                tempItemWidth = 300;
                StringBuilder sb = new StringBuilder();
                if(UIExpansionManager.Instance.CurBindingWrapper.IllegalLabelTreeItemList.Count > 0)
                {
                    sb.Append("含有非法字符的Label数：" + UIExpansionManager.Instance.CurBindingWrapper.IllegalLabelTreeItemList.Count + "  " );
                }
                if(UIExpansionManager.Instance.CurBindingWrapper.IllegalMethodLabelTreeItemList.Count > 0)
                {
                    sb.Append("被重复绑定的Methods数：" + UIExpansionManager.Instance.CurBindingWrapper.IllegalMethodLabelTreeItemList.Count + "  " );
                }
                if (UIExpansionManager.Instance.CurBindingWrapper.RepeatLabelNum > 0)
                {
                    sb.Append("重复的Label数：" + UIExpansionManager.Instance.CurBindingWrapper.RepeatLabelNum);
                }
                GUI.Label(new Rect(leftX, _toolbarArea.y, tempItemWidth, UIExpansionEditorUtility.SINGLELINE_SPACING), sb.ToString(),EditorStyles.boldLabel);
                GUI.color = tempColor;
            }
            /*
            leftX += 60;
            tempItemWidth = 100;
            bool inUsed = UIExpansionManager.Instance.BindingSettings.ShowState ==  BindingTreeItemState.Use;
            inUsed = GUI.Toggle(new Rect(leftX, _toolbarArea.y, tempItemWidth, UIExpansionEditorUtility.SINGLELINE_SPACING), inUsed, "Use", EditorStyles.toolbarButton);
            if (inUsed != (UIExpansionManager.Instance.BindingSettings.ShowState == BindingTreeItemState.Use))
            {
                UIExpansionManager.Instance.BindingSettings.ShowState = BindingTreeItemState.Use;
            }
            leftX += tempItemWidth;

            tempItemWidth = 100;
            bool inTotal = UIExpansionManager.Instance.BindingSettings.ShowState == BindingTreeItemState.Total;
            inTotal = GUI.Toggle(new Rect(leftX, _toolbarArea.y, tempItemWidth, UIExpansionEditorUtility.SINGLELINE_SPACING), inTotal, "Total", EditorStyles.toolbarButton);
            if (inTotal != (UIExpansionManager.Instance.BindingSettings.ShowState == BindingTreeItemState.Total))
            {
                UIExpansionManager.Instance.BindingSettings.ShowState = BindingTreeItemState.Total;
            }
            leftX += tempItemWidth;
            */
        }
    }
}