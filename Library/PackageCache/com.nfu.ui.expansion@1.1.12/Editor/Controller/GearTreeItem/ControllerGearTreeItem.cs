using System.Collections.Generic;
using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;
using UnityEditor;

namespace ND.UI
{
    public class ControllerGearTreeItem : ControllerGearTreeItemBase
    {
        private string _controllerName;

        private List<string> _pageNameList;
        private int[] _optionValues;

        private int _monitorCheckValue;

        private List<int> _values;

        public List<int> Values
        {
            get
            {
                return _values;
            }

            set
            {
                _values = value;
            }
        }

        public string ControllerName
        {
            get
            {
                return _controllerName;
            }

            set
            {
                _controllerName = value;
            }
        }

        public ControllerGearTreeItem(string controllerName, string[] pageNames) : base(GearTypeState.Controller)
        {
            _controllerName = controllerName;
            _pageNameList = new List<string>();
            _optionValues = new int[pageNames.Length];
            for (int i = 0; i < pageNames.Length; i++)
            {
                _pageNameList.Add($"{i}:{pageNames[i]}");
                _optionValues[i] = i;
            }
            _values = new List<int>();
        }

        public override void OnHeaderGUI(int index, Rect itemArea)
        {
            GUI.DrawTexture(itemArea, index % 2 == 1 ? UIExpansionEditorUtility.TreeItemBG1 : UIExpansionEditorUtility.TreeItemBG2);
            HandleHeaderInput(itemArea);
            Rect tagArea = new Rect(
                itemArea.x - 6 + DepthValue * 20,
                itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                140,
                EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(tagArea, "[C] "+ _controllerName);
        }

        public override void OnDetailGUI(int index, Rect itemArea)
        {
            base.OnDetailGUI(index, itemArea);
            float width = UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH - 20;
            for (int i = 0; i < _values.Count; i++)
            {
                Rect valueArea = new Rect(
                    itemArea.x + UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH * i + (UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH - width) / 2,
                    itemArea.y + 1,
                    width,
                    _height - 2);
                int tempValue = EditorGUI.IntPopup(valueArea, _values[i], _pageNameList.ToArray(), _optionValues);
                if (tempValue != _values[i])
                {
                    _values[i] = tempValue;
                    this.OnValueChanged();
                }
            }
        }


        public override void ApplyValue(int pageIndex)
        {
            Target.GetComponent<UIExpansion>().EditorChangeControllerSelectedIndex(_controllerName, _values[pageIndex]);
        }

        public override GearConfig BuildConfig(UIExpansionStoredDataBuilder dataBuilder)
        {
            return new GearConfig(_gear, dataBuilder.BuildDataList(Target, _controllerName, _values));
        }

        public override void LoadConfig(GearConfig config)
        {
            _values = new List<int>();
            _controllerName = UIScript.StoredStrings[config.dataArray[1]];
            for (int i = 2; i < config.dataArray.Length; i++)
            {
                _values.Add(UIScript.StoredInts[config.dataArray[i]]);
            }
            _state = ControllerTreeItemState.Show;
            State = ControllerTreeItemState.Show;
        }

        public override void MonitorValueChange()
        {
            if (Target.GetComponent<UIExpansion>() == null)
            {
                return;
            }
            ControllerConfig controllerConfig = Target.GetComponent<UIExpansion>().EditorGetControllerConfig(_controllerName);
            if (controllerConfig == null)
            {
                return;
            }
            bool stateChanged = _state == ControllerTreeItemState.Hide;
            if (stateChanged)
            {
                _values = new List<int>();
                _state = ControllerTreeItemState.Show;
                State = ControllerTreeItemState.Show;
                for (int i = 0; i < ControllerWrapper.PageNameList.Count; i++)
                {
                    if (i == ControllerWrapper.SelectedIndex)
                    {
                        _values.Add(controllerConfig.selectedIndex);
                    }
                    else
                    {
                        _values.Add(_monitorCheckValue);
                    }
                }
                UIExpansionManager.Instance.CurControllerWrapper.RebuildShowTreeList();
            }
            else
            {
                _values[SelectedIndex] = controllerConfig.selectedIndex;
            }
            _monitorCheckValue = controllerConfig.selectedIndex;
            this.OnValueChanged();
        }

        public override void OnRemovePage(int pageIndex)
        {
            if (_state == ControllerTreeItemState.Show)
            {
                _values.RemoveAt(pageIndex);
            }
            base.OnRemovePage(pageIndex);
        }

        public override void RecordValue(int pageIndex = NEW_PAGE_INDEX)
        {
            if (Target.GetComponent<UIExpansion>() == null)
            {
                return;
            }
            ControllerConfig controllerConfig = Target.GetComponent<UIExpansion>().EditorGetControllerConfig(_controllerName);
            if (controllerConfig == null)
            {
                return;
            }
            if (pageIndex == MONITOR_INDEX)
            {
                _monitorCheckValue = controllerConfig.selectedIndex;
            }
            else if (pageIndex == NEW_PAGE_INDEX)
            {
                _values.Add(controllerConfig.selectedIndex);
            }
        }

        public override void RefreshMonitorValue()
        {
            if (_state == ControllerTreeItemState.Show)
            {
                _monitorCheckValue = _values[SelectedIndex];
            }
            else
            {
                RecordValue(MONITOR_INDEX);
            }
        }
        
        
        public override string GearName
        {
            get { return $"[C]{_controllerName}"; }
        }
    }
}