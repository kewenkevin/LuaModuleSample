using System;
using System.Collections.Generic;
using ND.UI.Core;
using ND.UI.Core.Model;
using ND.UI.Core.TextEditor;
using UnityEngine;

namespace ND.UI
{
    public class TextColorStyleGearTreeItem : ControllerGearTreeItemBase
    {
        private ND.UI.Core.ColorStyleBase _monitorCheckValue;

        private List<ND.UI.Core.ColorStyleBase> _values;

        public List<ND.UI.Core.ColorStyleBase> Values
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

        public TextColorStyleGearTreeItem() : base(GearTypeState.TextColorStyle)
        {
            _values = new List<ND.UI.Core.ColorStyleBase>();
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
                
                TextEditorUtility.DrawTextColorStyleField(valueArea, GUIContent.none, _values[i],i.ToString(),
                    (style, refPath) =>
                    {
                        var valueIndex = Int32.Parse(refPath); 
                        if(_values[valueIndex] != style) {
                            _values[valueIndex] = style;
                            OnValueChanged();
                        }
                    });
            }
        }

        public override void ApplyValue(int pageIndex)
        {
            Target.GetComponent<ND.UI.Core.IColorStyleUseAble>().colorStyle = _values[pageIndex];
        }

        public override GearConfig BuildConfig(UIExpansionStoredDataBuilder dataBuilder)
        {
            return new GearConfig(_gear, dataBuilder.BuildDataList(Target, _values));
        }

        public override void LoadConfig(GearConfig config)
        {
            _values = new List<ND.UI.Core.ColorStyleBase>();
            for (int i = 1; i < config.dataArray.Length; i++)
            {
                _values.Add(UIScript.StoredTextColorStyles[config.dataArray[i]]);
            }
            _state = ControllerTreeItemState.Show;
            State = ControllerTreeItemState.Show;
        }

        public override void MonitorValueChange()
        {
            if (Target.GetComponent<ND.UI.Core.IColorStyleUseAble>().colorStyle == _monitorCheckValue)
            {
                return;
            }
            bool stateChanged = _state == ControllerTreeItemState.Hide;
            if (stateChanged)
            {
                _values = new List<ND.UI.Core.ColorStyleBase>();
                _state = ControllerTreeItemState.Show;
                State = ControllerTreeItemState.Show;
                for (int i = 0; i < ControllerWrapper.PageNameList.Count; i++)
                {
                    if (i == ControllerWrapper.SelectedIndex)
                    {
                        _values.Add(Target.GetComponent<ND.UI.Core.IColorStyleUseAble>().colorStyle);
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
                _values[SelectedIndex] = Target.GetComponent<ND.UI.Core.IColorStyleUseAble>().colorStyle;
            }
            _monitorCheckValue = Target.GetComponent<ND.UI.Core.IColorStyleUseAble>().colorStyle;
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
                _monitorCheckValue = Target.GetComponent<ND.UI.Core.IColorStyleUseAble>().colorStyle;
            }
            else if (pageIndex == NEW_PAGE_INDEX)
            {
                _values.Add(Target.GetComponent<ND.UI.Core.IColorStyleUseAble>().colorStyle);
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