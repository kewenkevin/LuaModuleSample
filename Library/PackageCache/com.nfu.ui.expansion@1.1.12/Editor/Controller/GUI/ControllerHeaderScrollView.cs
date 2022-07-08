﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace ND.UI
{
    public class ControllerHeaderScrollView : EditorScrollViewBase
    {
        private float _addGearPanelOffsetY;

        private bool _needShowVerticalSliderBar = false;

        private Rect _addGearPanelArea = default(Rect);

        private Rect _verticalSilderBarArea = default(Rect);

        private float _addGearPanelWidth;

        private float _addGearPanelHeight;

        public override void OnGUI(Rect viewArea)
        {
            base.OnGUI(viewArea);
            if (UIExpansionManager.Instance.ControllerSettings.InAddNewGearState)
            {
                UpdateLayout();
                GUI.BeginGroup(_addGearPanelArea, string.Empty);
                DrawAddGearItems();
                GUI.EndGroup();
                DrawAddGearSilderBarArea();
                DrawBackToShowListButton();
            }
            else
            {
                GUI.BeginGroup(_viewArea, string.Empty);
                DrawHeaderItems();
                DrawAddNewBtn();
                GUI.EndGroup();
            }
        }

        private void UpdateLayout()
        {
            _addGearPanelHeight = _viewArea.height - 30;
            _addGearPanelWidth = _viewArea.width;
            _needShowVerticalSliderBar = UIExpansionManager.Instance.CurControllerWrapper.AddGearTreeHeight > _addGearPanelHeight;
            if (_needShowVerticalSliderBar)
            {
                // if (_addGearPanelOffsetY < _addGearPanelHeight -
                //     UIExpansionManager.Instance.CurControllerWrapper.AddGearTreeHeight)
                // {
                //     Debug.Log("hhhh");
                // }
                _addGearPanelOffsetY = Mathf.Clamp(_addGearPanelOffsetY, _addGearPanelHeight - UIExpansionManager.Instance.CurControllerWrapper.AddGearTreeHeight, 0);
                _addGearPanelWidth -= UIExpansionEditorUtility.SLIDER_BAR_WIDTH;
                _verticalSilderBarArea = new Rect(
                    _viewArea.xMax - UIExpansionEditorUtility.SLIDER_BAR_WIDTH,
                    _viewArea.y,
                    UIExpansionEditorUtility.SLIDER_BAR_WIDTH,
                    _addGearPanelHeight);
            }
            else
            {
                _addGearPanelOffsetY = 0;
            }
            _addGearPanelArea = new Rect(_viewArea.x, _viewArea.y, _addGearPanelWidth, _addGearPanelHeight);
        }

        private void DrawAddGearItems()
        {
            float height = _addGearPanelOffsetY;
            float itemWidth = _addGearPanelWidth;
            if (UIExpansionManager.Instance.CurControllerWrapper.AddGearTreeList.Count == 0)
            {
                return;
            }
            for (int i = 0; i < UIExpansionManager.Instance.CurControllerWrapper.AddGearTreeList.Count; i++)
            {
                float itemHeight = UIExpansionManager.Instance.CurControllerWrapper.AddGearTreeList[i].AddGearHeight;
                Rect itemArea = new Rect(
                    0,
                    height,
                    itemWidth,
                    itemHeight);
                height += itemHeight;
                UIExpansionManager.Instance.CurControllerWrapper.AddGearTreeList[i].OnAddGearGUI(i, itemArea);
            }
        }


        private void DrawAddGearSilderBarArea()
        {
            if (_needShowVerticalSliderBar)
            {
                Event e = Event.current;
                var delta = 0f;
                if (e.isScrollWheel)
                {
                    delta = e.delta.y * UIExpansionManager.Instance.WindowSettings.MouseScrollWheelSteps;
                }
                
                _addGearPanelOffsetY = Mathf.Max(_addGearPanelHeight - UIExpansionManager.Instance.CurControllerWrapper.AddGearTreeHeight, _addGearPanelOffsetY);
                _addGearPanelOffsetY = -GUI.VerticalScrollbar(
                    _verticalSilderBarArea,
                    -_addGearPanelOffsetY + delta, 
                    _addGearPanelHeight + 20, 0, UIExpansionManager.Instance.CurControllerWrapper.AddGearTreeHeight);
            }
        }

        private void DrawBackToShowListButton()
        {
            Rect buttonArea = new Rect(
                _viewArea.width / 2 - 80,
                _viewArea.yMax - 13,
                160,
                EditorGUIUtility.singleLineHeight);
            if (GUI.Button(buttonArea, "Back To Editor"))
            {
                _addGearPanelOffsetY = 0;
                UIExpansionManager.Instance.ControllerSettings.InAddNewGearState = false;
            }
        }


        private void DrawHeaderItems()
        {
            float height = UIExpansionManager.Instance.ControllerSettings.PanelOffsetY;
            float itemWidth = _viewArea.width;
            if (UIExpansionManager.Instance.CurControllerWrapper.ShowTreeList.Count == 0)
            {
                return;
            }
            for (int i = 0; i < UIExpansionManager.Instance.CurControllerWrapper.ShowTreeList.Count; i++)
            {
                float itemHeight = UIExpansionManager.Instance.CurControllerWrapper.ShowTreeList[i].Height;
                Rect itemArea = new Rect(
                    0,
                    height,
                    itemWidth,
                    itemHeight);
                
                Rect priorityArea = new Rect(
                    itemWidth - 70,
                    height,
                    70,
                    itemHeight);
                height += itemHeight;
                UIExpansionManager.Instance.CurControllerWrapper.ShowTreeList[i].OnHeaderGUI(i, itemArea);
                UIExpansionManager.Instance.CurControllerWrapper.ShowTreeList[i].OnHeaderPriorityGUI(i, priorityArea);
                
            }
        }

        private void DrawAddNewBtn()
        {
            Rect buttonArea = new Rect(
                _viewArea.width / 2 - 80,
                UIExpansionManager.Instance.CurControllerWrapper.ShowTreeHeight + UIExpansionManager.Instance.ControllerSettings.PanelOffsetY + 4,
                160,
                20);
            if (GUI.Button(buttonArea, "Add New Gear"))
            {
                UIExpansionManager.Instance.CurControllerWrapper.RebuildAddGearTreeList();
                UIExpansionManager.Instance.ControllerSettings.InAddNewGearState = true;
            }
        }
    }
}