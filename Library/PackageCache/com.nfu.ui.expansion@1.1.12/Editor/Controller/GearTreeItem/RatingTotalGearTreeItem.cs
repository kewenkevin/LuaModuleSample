using System.Collections.Generic;
using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;
using UnityEditor;

namespace ND.UI
{
    public class RatingTotalGearTreeItem : ControllerGearTreeItemBase
    { 
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

        public RatingTotalGearTreeItem() : base(GearTypeState.RatingTotal)
        {
            _values = new List<int>();
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
                    EditorGUIUtility.singleLineHeight);
                
                var tempValue = EditorGUI.IntField(valueArea, _values[i], GUI.skin.textField);
                tempValue = Mathf.Clamp(tempValue, 0, int.MaxValue);
                
                if (tempValue != _values[i])
                {
                    _values[i] = tempValue;
                    this.OnValueChanged();
                }
            }
        }

        public override void ApplyValue(int pageIndex)
        {
            var yRating = Target.GetComponent<IRating>();
            yRating.total = _values[pageIndex];
        }

        public override GearConfig BuildConfig(UIExpansionStoredDataBuilder dataBuilder)
        {
            return new GearConfig(_gear, dataBuilder.BuildDataList(Target, _values));
        }

        public override void LoadConfig(GearConfig config)
        {
            _values = new List<int>();
            for (int i = 1; i < config.dataArray.Length; i++)
            {
                _values.Add(UIScript.StoredInts[config.dataArray[i]]);
            }
            _state = ControllerTreeItemState.Show;
            State = ControllerTreeItemState.Show;
        }

        public override void MonitorValueChange()
        {
            
            if ( Target.GetComponent<IRating>().total == _monitorCheckValue )
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
                        _values.Add(Target.GetComponent<IRating>().total);
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
                _values[SelectedIndex] = Target.GetComponent<IRating>().total;
            }
            _monitorCheckValue = Target.GetComponent<IRating>().total;
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
                _monitorCheckValue = Target.GetComponent<IRating>().total;
            }
            else if (pageIndex == NEW_PAGE_INDEX)
            {
                _values.Add(Target.GetComponent<IRating>().total);
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