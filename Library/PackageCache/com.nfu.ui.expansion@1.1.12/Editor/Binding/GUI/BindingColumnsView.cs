using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ND.UI
{
    public class BindingColumnsView : EditorScrollViewBase 
    {
        private float _gameObjectColumnWidth = 200;

        private float _gameObjectColumnMinWidth = 100;

        private float _gameObjectColumnMaxWidth = 300;

        private float _binderColumnWidth = 200;

        private float _binderColumnMinWidth = 100;

        private float _binderColumnMaxWidth = 300;

        private float _linkerColumnWidth = 400;

        private float _linkerColumnMinWidth = 200;

        private float _linkerColumnMaxWidth = 600;

        public float BinderColumnWidth
        {
            get { return _gameObjectColumnWidth + _binderColumnWidth + UIExpansionEditorUtility.BORDER_CONTROL_WIDTH; }
            set
            {
                _binderColumnWidth = value - _gameObjectColumnWidth - UIExpansionEditorUtility.BORDER_CONTROL_WIDTH;
            }
        }



        private Rect _previousViewArea = default(Rect);

        private Rect _gameObjectColumnArea = default(Rect);

        private Rect _binderColumnArea = default(Rect);

        private Rect _linkerColumnArea = default(Rect);

        private Rect _horizontalSliderBarArea = default(Rect);

        private bool _needShowHorizontalSliderBar = false;

        private Rect _gameObjectBorderControlArea = default(Rect);

        private Rect _binderBorderControlArea = default(Rect);

        private float _borderResizeOffsetX = 0f;

        private float _viewOffsetX = 0;

        private float _columnAreaHeight = 0;

        private Rect _realViewArea = default(Rect);

        public BindingColumnsView()
        {

        }

        public override void OnGUI(Rect viewArea)
        {
            base.OnGUI(viewArea);
            UpdateViewLayout();
            GUI.BeginGroup(_realViewArea);
            DrawGameObjectColumnArea();
            DrawBinderColumnArea();
            DrawLinkerColumnArea();
            GUI.EndGroup();
            DrawBorderLine();

        }

        private bool _showGameObjectColumnSlider = false;

        private float _GameObjectColumnOffsetY = 0;

        public bool ShowGameObjectColumnSlider
        {
            get => _showGameObjectColumnSlider;
            set
            {
                if (_showGameObjectColumnSlider == value)
                {
                    return;
                }
                _GameObjectColumnOffsetY = 0;
                _showGameObjectColumnSlider = value;
            }
        }

        private void DrawGameObjectColumnArea()
        {
            float totalHeight = UIExpansionManager.Instance.CurBindingWrapper.ColumnGOTreeList.Count * (UIExpansionEditorUtility.SINGLELINE_HEIGHT + 2);
            ShowGameObjectColumnSlider = totalHeight > _columnAreaHeight;
            if (ShowGameObjectColumnSlider)
            {
                Event e = Event.current;
                var delta = 0f;
                if (e.isScrollWheel)
                {
                    delta = e.delta.y * UIExpansionManager.Instance.WindowSettings.MouseScrollWheelSteps;
                }
                Rect verticalSliderBarArea = new Rect(
                    _gameObjectColumnArea.x + _viewOffsetX + _gameObjectColumnArea.width - 20,
                    0,
                    20,
                    _columnAreaHeight);
                
                _GameObjectColumnOffsetY = -GUI.VerticalScrollbar(verticalSliderBarArea, -_GameObjectColumnOffsetY + delta, _columnAreaHeight, 0, totalHeight);
            }

            float height = _GameObjectColumnOffsetY;
            float itemWidth = ShowGameObjectColumnSlider ? _gameObjectColumnArea.width - 20 : _gameObjectColumnArea.width; // todo...
            if (UIExpansionManager.Instance.CurBindingWrapper.ColumnGOTreeList.Count == 0)
            {
                return;
            }
            for (int i = 0; i < UIExpansionManager.Instance.CurBindingWrapper.ColumnGOTreeList.Count; i++)
            {
                float itemHeight = UIExpansionManager.Instance.CurBindingWrapper.ColumnGOTreeList[i].Height;
                Rect itemArea = new Rect(
                    _viewOffsetX,
                    height,
                    itemWidth,
                    itemHeight);
                height += itemHeight;
                UIExpansionManager.Instance.CurBindingWrapper.ColumnGOTreeList[i].OnColumnGUI(i, itemArea);
            }
        }

        private void DrawBinderColumnArea()
        {
            float height = 0;
            float itemWidth = Mathf.Max(_binderColumnArea.width, 0); // todo...
            if (UIExpansionManager.Instance.BindingSettings.ColumnSelectedGOId == 0)
            {
                return;
            }
            BindingGameObjectTreeItem targetGameObject = null;
            if (!UIExpansionManager.Instance.CurBindingWrapper.GoTreeItemDic.TryGetValue(UIExpansionManager.Instance.BindingSettings.ColumnSelectedGOId, out targetGameObject))
            {
                UIExpansionManager.Instance.BindingSettings.ColumnSelectedGOId = 0;
                return;
            }
            
            List<BindingBinderTreeItemBase> binderList = targetGameObject.GetBinderList();
            if (binderList.Count == 0)
            {
                return;
            }
            for (int i = 0; i < binderList.Count; i++)
            {
                float itemHeight = binderList[i].Height;
                Rect itemArea = new Rect(
                    _binderColumnArea.x + _viewOffsetX,
                    height,
                    itemWidth,
                    itemHeight);
                height += itemHeight;
                // GUI.DrawTexture(itemArea, Texture2D.whiteTexture);
                binderList[i].OnColumnGUI(i, itemArea);
            }
        }

        private bool _showLinkerColumnSlider = false;

        private float _linkerColumnOffsetY = 0;

        public bool ShowLinkerColumnSlider
        {
            get => _showLinkerColumnSlider;
            set
            {
                if (_showLinkerColumnSlider == value)
                {
                    return;
                }
                _linkerColumnOffsetY = 0;
                _showLinkerColumnSlider = value;
            }
        }


        private void DrawLinkerColumnArea()
        {
            if (UIExpansionManager.Instance.BindingSettings.ColumnSelectedGOId == 0)
            {
                return;
            }
            BindingGameObjectTreeItem targetGameObject = null;
            if (!UIExpansionManager.Instance.CurBindingWrapper.GoTreeItemDic.TryGetValue(UIExpansionManager.Instance.BindingSettings.ColumnSelectedGOId, out targetGameObject))
            {
                UIExpansionManager.Instance.BindingSettings.ColumnSelectedGOId = 0;
                return;
            }
            if (string.IsNullOrEmpty(UIExpansionManager.Instance.BindingSettings.BinderType))
            {
                return;
            }
            BindingBinderTreeItemBase targetBinder = targetGameObject.GetTargetBinder(UIExpansionManager.Instance.BindingSettings.BinderType);
            if (targetBinder == null)
            {
                return;
            }

            UIExpansion ue = targetGameObject.Target.GetComponent<UIExpansion>();
            bool bModule =  ue != null && UIExpansionManager.Instance.CurUIExpansion != ue;
            float totalHeight = targetBinder.ChildrenList.Count * (UIExpansionEditorUtility.SINGLELINE_HEIGHT + 2);
            ShowLinkerColumnSlider = totalHeight > _columnAreaHeight;
            if (ShowLinkerColumnSlider)
            {
                Event e = Event.current;
                var delta = 0f;
                if (e.isScrollWheel)
                {
                    delta = e.delta.y * UIExpansionManager.Instance.WindowSettings.MouseScrollWheelSteps;
                }
                
                Rect verticalSliderBarArea = new Rect(
                            _linkerColumnArea.x + _viewOffsetX + _linkerColumnArea.width - 20,
                            0,
                            20,
                            _columnAreaHeight);

                _linkerColumnOffsetY = -GUI.VerticalScrollbar(
                    verticalSliderBarArea, 
                    -_linkerColumnOffsetY + delta, 
                    _columnAreaHeight, 
                    0, totalHeight);
            }

            float height = _linkerColumnOffsetY;
            float itemWidth = ShowLinkerColumnSlider ? _linkerColumnArea.width - 20 : _linkerColumnArea.width; // todo...

            for (int i = 0; i < targetBinder.ChildrenList.Count; i++)
            {
                BindingLinkerTreeItem linkerTreeItem = targetBinder.ChildrenList[i] as BindingLinkerTreeItem;
                float itemHeight = linkerTreeItem.Height;
                Rect itemArea = new Rect(
                    _linkerColumnArea.x + _viewOffsetX,
                    height,
                    itemWidth,
                    itemHeight);
                height += itemHeight;
                linkerTreeItem.OnColumnGUI(i, itemArea, bModule);
            }
        }


        private void DrawBorderLine()
        {
            GUI.DrawTexture(_binderBorderControlArea, UIExpansionEditorUtility.CutlineTex);
            GUI.DrawTexture(_gameObjectBorderControlArea, UIExpansionEditorUtility.CutlineTex);
        }

        private void UpdateViewLayout()
        {
            float realViewWidth = _gameObjectColumnWidth + _binderColumnWidth + _linkerColumnWidth + UIExpansionEditorUtility.BORDER_CONTROL_WIDTH * 2;
            _needShowHorizontalSliderBar = realViewWidth > _viewArea.width;
            _columnAreaHeight = _needShowHorizontalSliderBar ? _viewArea.height - UIExpansionEditorUtility.SLIDER_BAR_WIDTH : _viewArea.height;
            _realViewArea = new Rect(
                _viewArea.x,
                _viewArea.y,
                _viewArea.width,
                _columnAreaHeight);

            _gameObjectBorderControlArea = new Rect(
                _viewArea.x + _gameObjectColumnWidth + _viewOffsetX,
                _viewArea.y,
                UIExpansionEditorUtility.BORDER_CONTROL_WIDTH,
                _columnAreaHeight);

            _binderBorderControlArea = new Rect(
                _viewArea.x + _gameObjectColumnWidth + _binderColumnWidth + UIExpansionEditorUtility.BORDER_CONTROL_WIDTH + _viewOffsetX,
                _viewArea.y,
                UIExpansionEditorUtility.BORDER_CONTROL_WIDTH,
                _columnAreaHeight);

            if (UpdateGameObjectColumnWidth() || UpdateBinderColumnWidth() || _viewArea != _previousViewArea)
            {
                _gameObjectColumnArea = new Rect(
                    _viewArea.x,
                    _viewArea.y,
                    _gameObjectColumnWidth,
                    _columnAreaHeight);

                _binderColumnArea = new Rect(
                    _viewArea.x + _gameObjectColumnWidth + UIExpansionEditorUtility.BORDER_CONTROL_WIDTH,
                    _viewArea.y,
                    _binderColumnWidth,
                    _columnAreaHeight);

                _linkerColumnArea = new Rect(
                    _viewArea.x + _gameObjectColumnWidth + _binderColumnWidth + UIExpansionEditorUtility.BORDER_CONTROL_WIDTH * 2,
                    _viewArea.y,
                    Mathf.Max(_linkerColumnWidth, _viewArea.width - (_gameObjectColumnWidth + _binderColumnWidth + UIExpansionEditorUtility.BORDER_CONTROL_WIDTH * 2)),
                    _columnAreaHeight);
            }
            if (_needShowHorizontalSliderBar)
            {
                _horizontalSliderBarArea = new Rect(
                    _viewArea.x,
                    _viewArea.y + _columnAreaHeight,
                    _viewArea.width,
                    UIExpansionEditorUtility.SLIDER_BAR_WIDTH);

                _viewOffsetX = -GUI.HorizontalScrollbar(
                    _horizontalSliderBarArea,
                    -_viewOffsetX,
                    _viewArea.width, 0, realViewWidth);
            }
        }

        private bool UpdateGameObjectColumnWidth()
        {
            bool widthChanged = false;
            EditorGUIUtility.AddCursorRect(_gameObjectBorderControlArea, MouseCursor.ResizeHorizontal);
            int controlID = GUIUtility.GetControlID("Controller Panel Header Width Change".GetHashCode(), FocusType.Passive, _gameObjectBorderControlArea);
            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (_gameObjectBorderControlArea.Contains(Event.current.mousePosition) && Event.current.button == 0)
                    {
                        _borderResizeOffsetX = Event.current.mousePosition.x - _gameObjectColumnWidth;
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
                        _gameObjectColumnWidth = Mathf.Clamp(tempWidth, _gameObjectColumnMinWidth, _gameObjectColumnMaxWidth);
                        UIExpansionManager.Instance.NeedRepaint = true;
                        widthChanged = true;
                    }
                    break;
            }
            return widthChanged;
        }

        private bool UpdateBinderColumnWidth()
        {
            bool widthChanged = false;
            EditorGUIUtility.AddCursorRect(_binderBorderControlArea, MouseCursor.ResizeHorizontal);
            int controlID = GUIUtility.GetControlID("Controller Panel Header Width Change".GetHashCode(), FocusType.Passive, _binderBorderControlArea);
            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (_binderBorderControlArea.Contains(Event.current.mousePosition) && Event.current.button == 0)
                    {
                        _borderResizeOffsetX = Event.current.mousePosition.x - BinderColumnWidth;
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
                        BinderColumnWidth = Mathf.Clamp(tempWidth,
                            _gameObjectColumnWidth + _binderColumnMinWidth + UIExpansionEditorUtility.BORDER_CONTROL_WIDTH,
                             _gameObjectColumnWidth + _binderColumnMaxWidth + UIExpansionEditorUtility.BORDER_CONTROL_WIDTH);
                        UIExpansionManager.Instance.NeedRepaint = true;
                        widthChanged = true;
                    }
                    break;
            }
            return widthChanged;
        }
    }
}