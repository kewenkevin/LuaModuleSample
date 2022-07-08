using System.Collections.Generic;
using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;
using UnityEditor;

namespace ND.UI
{
    public class ActiveGearTreeItem : ControllerGearTreeItemBase
    { 
        private bool _monitorCheckValue;

        private List<bool> _values;

        public List<bool> Values
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

        public ActiveGearTreeItem() : base(GearTypeState.Active)
        {
            _values = new List<bool>();
        }

        public override void OnDetailGUI(int index, Rect itemArea)
        {
            base.OnDetailGUI(index, itemArea);
            for (int i = 0; i < _values.Count; i++)
            {
                Rect valueArea = new Rect(
                    itemArea.x + UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH * i + (UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH - EditorGUIUtility.singleLineHeight) / 2.0f,
                    itemArea.y + 1,
                    EditorGUIUtility.singleLineHeight,
                    EditorGUIUtility.singleLineHeight);
                bool tempValue = GUI.Toggle(valueArea, _values[i], "");
                if (tempValue != _values[i])
                {
                    _values[i] = tempValue;
                    this.OnValueChanged();
                }
            }
        }

        public override void ApplyValue(int pageIndex)
        {
            Target.SetActive(_values[pageIndex]);
        }

        public override GearConfig BuildConfig(UIExpansionStoredDataBuilder dataBuilder)
        {
            return new GearConfig(_gear, dataBuilder.BuildDataList(Target, _values));
        }

        public override void LoadConfig(GearConfig config)
        {
            _values = new List<bool>();
            for (int i = 1; i < config.dataArray.Length; i++)
            {
                _values.Add(config.dataArray[i] == UIExpansionUtility.TrueValue);
            }
            _state = ControllerTreeItemState.Show;
            State = ControllerTreeItemState.Show;
           
        }

        public override void MonitorValueChange()
        {
            if (Target.activeSelf == _monitorCheckValue)
            {
                return;
            }
            bool stateChanged = _state == ControllerTreeItemState.Hide;
            if (stateChanged)
            {
                _values = new List<bool>();
                _state = ControllerTreeItemState.Show;
                State = ControllerTreeItemState.Show;
                for (int i = 0; i < ControllerWrapper.PageNameList.Count; i++)
                {
                    if (i == ControllerWrapper.SelectedIndex)
                    {
                        _values.Add(Target.activeSelf);
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
                _values[SelectedIndex] = Target.activeSelf;
            }
            _monitorCheckValue = Target.activeSelf;
            this.OnValueChanged();
        }

        public override void OnRemovePage(int pageIndex)
        {
            if(_state == ControllerTreeItemState.Show)
            {
                _values.RemoveAt(pageIndex);
            }
            base.OnRemovePage(pageIndex);
        }

        public override void RecordValue(int pageIndex = NEW_PAGE_INDEX)
        {
            if (pageIndex == MONITOR_INDEX)
            {
                _monitorCheckValue = Target.activeSelf;
            }
            else if (pageIndex == NEW_PAGE_INDEX)
            {
                _values.Add(Target.activeSelf);
            }
        }

        public override void RefreshMonitorValue()
        {
            if(_state == ControllerTreeItemState.Show)
            {
                _monitorCheckValue = _values[SelectedIndex];
            }
            else
            {
                RecordValue(MONITOR_INDEX);
            }
        }
    }
}