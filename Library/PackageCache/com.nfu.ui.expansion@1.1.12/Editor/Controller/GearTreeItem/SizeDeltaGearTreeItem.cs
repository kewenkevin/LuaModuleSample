using System.Collections.Generic;
using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;
using UnityEditor;

namespace ND.UI
{
    public class SizeDeltaGearTreeItem : ControllerGearTreeItemBase
    {

        private Vector2 _monitorCheckValue;

        private List<Vector2> _values;

        public List<Vector2> Values
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

        public SizeDeltaGearTreeItem() : base(GearTypeState.SizeDelta)
        {
            _values = new List<Vector2>();
        }

        public override void OnDetailGUI(int index, Rect itemArea)
        {
            base.OnDetailGUI(index, itemArea);
            float width = UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH - 20;
            for (int i = 0; i < _values.Count; i++)
            {
                Rect valueArea = new Rect(
                    itemArea.x + UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH * i + (UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH - width) / 2,
                    itemArea.y,
                    width,
                    EditorGUIUtility.singleLineHeight);
                var tempValue = EditorGUI.Vector2Field(valueArea, "", _values[i]);
                if (tempValue != _values[i])
                {
                    _values[i] = tempValue;
                    this.OnValueChanged();
                }
            }
        }

        public override void ApplyValue(int pageIndex)
        {
            Target.GetComponent<RectTransform>().sizeDelta = (_values[pageIndex]);
        }

        public override GearConfig BuildConfig(UIExpansionStoredDataBuilder dataBuilder)
        {
            return new GearConfig(_gear, dataBuilder.BuildDataList(Target, _values));
        }

        public override void LoadConfig(GearConfig config)
        {
            _values = new List<Vector2>();
            for (int i = 1; i < config.dataArray.Length; i += 2)
            {
                _values.Add(new Vector2(
                    UIScript.StoredFloats[config.dataArray[i]],
                    UIScript.StoredFloats[config.dataArray[i + 1]]));
            }
            _state = ControllerTreeItemState.Show;
            State = ControllerTreeItemState.Show;
        }

        public override void MonitorValueChange()
        {
            if (Target.GetComponent<RectTransform>().sizeDelta == _monitorCheckValue)
            {
                return;
            }
            bool stateChanged = _state == ControllerTreeItemState.Hide;
            if (stateChanged)
            {
                _values = new List<Vector2>();
                _state = ControllerTreeItemState.Show;
                State = ControllerTreeItemState.Show;
                for (int i = 0; i < ControllerWrapper.PageNameList.Count; i++)
                {
                    if (i == ControllerWrapper.SelectedIndex)
                    {
                        _values.Add(Target.GetComponent<RectTransform>().sizeDelta);
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
                _values[SelectedIndex] = Target.GetComponent<RectTransform>().sizeDelta;
            }
            _monitorCheckValue = Target.GetComponent<RectTransform>().sizeDelta;
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

        public override void RecordValue(int pageIndex = 10000)
        {
            if (pageIndex == MONITOR_INDEX)
            {
                _monitorCheckValue = Target.GetComponent<RectTransform>().sizeDelta;
            }
            else if (pageIndex == NEW_PAGE_INDEX)
            {
                _values.Add(Target.GetComponent<RectTransform>().sizeDelta);
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
    }
}