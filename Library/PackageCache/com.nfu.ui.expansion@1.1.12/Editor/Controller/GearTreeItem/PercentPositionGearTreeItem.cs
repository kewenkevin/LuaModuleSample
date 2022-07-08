using System.Collections.Generic;
using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEditor;
using UnityEngine;

namespace ND.UI
{
    public class PercentPositionGearTreeItem : ControllerGearTreeItemBase
    {
        private Vector3 _monitorCheckValue;

        private List<Vector3> _values;

        private float _canvasWidth;
        private float _canvasHeight;

        public List<Vector3> Values
        {
            get { return _values; }

            set { _values = value; }
        }

        public PercentPositionGearTreeItem() : base(GearTypeState.PercentPosition)
        {
            _values = new List<Vector3>();
            UIExpansionUtility.GetCanvasSize(out _canvasWidth, out _canvasHeight);
        }

        private Vector3 GetValueByLocalPos(Vector3 localPos)
        {
            return new Vector3(localPos.x / _canvasWidth,
                localPos.y / _canvasHeight, localPos.z);
        }

        private Vector3 GetLocalPosByVector3(Vector3 value)
        {
            return new Vector3(value.x * _canvasWidth,
                value.y * _canvasHeight, value.z);
        }

        public override void OnDetailGUI(int index, Rect itemArea)
        {
            base.OnDetailGUI(index, itemArea);
            float width = UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH - 20;
            for (int i = 0; i < _values.Count; i++)
            {
                Rect valueArea = new Rect(
                    itemArea.x + UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH * i +
                    (UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH - width) / 2,
                    itemArea.y,
                    width,
                    EditorGUIUtility.singleLineHeight);
                Vector3 tempPos = EditorGUI.Vector3Field(valueArea, "", GetLocalPosByVector3(_values[i]));
                Vector3 tempValue = GetValueByLocalPos(tempPos);
                if (tempValue != _values[i])
                {
                    _values[i] = tempValue;
                    this.OnValueChanged();
                }
            }
        }

        public override void ApplyValue(int pageIndex)
        {
            Target.GetComponent<RectTransform>().localPosition = GetLocalPosByVector3(_values[pageIndex]);
        }

        public override GearConfig BuildConfig(UIExpansionStoredDataBuilder dataBuilder)
        {
            return new GearConfig(_gear, dataBuilder.BuildDataList(Target, _values));
        }

        public override void LoadConfig(GearConfig config)
        {
            _values = new List<Vector3>();
            for (int i = 1; i < config.dataArray.Length; i += 3)
            {
                _values.Add(new Vector3(
                    UIScript.StoredFloats[config.dataArray[i]],
                    UIScript.StoredFloats[config.dataArray[i + 1]],
                    UIScript.StoredFloats[config.dataArray[i + 2]]));
            }

            _state = ControllerTreeItemState.Show;
            State = ControllerTreeItemState.Show;
        }

        public override void MonitorValueChange()
        {
            Vector3 curValue = GetValueByLocalPos(Target.GetComponent<RectTransform>().localPosition);
            if (curValue == _monitorCheckValue)
            {
                return;
            }

            bool stateChanged = _state == ControllerTreeItemState.Hide;
            if (stateChanged)
            {
                _values = new List<Vector3>();
                _state = ControllerTreeItemState.Show;
                State = ControllerTreeItemState.Show;
                for (int i = 0; i < ControllerWrapper.PageNameList.Count; i++)
                {
                    if (i == ControllerWrapper.SelectedIndex)
                    {
                        _values.Add(GetValueByLocalPos(Target.GetComponent<RectTransform>().localPosition));
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
                _values[SelectedIndex] = GetValueByLocalPos(Target.GetComponent<RectTransform>().localPosition);
            }

            _monitorCheckValue = GetValueByLocalPos(Target.GetComponent<RectTransform>().localPosition);
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
            if (pageIndex == MONITOR_INDEX)
            {
                _monitorCheckValue = GetValueByLocalPos(Target.GetComponent<RectTransform>().localPosition);
            }
            else if (pageIndex == NEW_PAGE_INDEX)
            {
                _values.Add(GetValueByLocalPos(Target.GetComponent<RectTransform>().localPosition));
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