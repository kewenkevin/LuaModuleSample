using System;
using System.Collections.Generic;
using ND.UI.Core;
using ND.UI.Core.Model;
using ND.UI.Core.TextEditor;
using UnityEngine;

// using Yoozoo.UI.Core.TextEditor;

namespace ND.UI
{
    public class TextFontStyleGearTreeItem : ControllerGearTreeItemBase
    {
        private ND.UI.Core.TextStyleBase _monitorCheckValue;
 
        private List<ND.UI.Core.TextStyleBase> _values;

        public List<ND.UI.Core.TextStyleBase> Values
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

        public TextFontStyleGearTreeItem() : base(GearTypeState.TextFontStyle)
        {
            _values = new List<ND.UI.Core.TextStyleBase>();
        }

        public override void OnDetailGUI(int index, Rect itemArea)
        {
            base.OnDetailGUI(index, itemArea);
            float width = UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH - 20;
            for (int i = 0; i < _values.Count; i++)
            {
                Rect valueArea = new Rect(
                    itemArea.x + UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH * i + (UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH - width) / 2,
                    itemArea.y + 2,
                    width,
                    _height - 4);

                
                
                TextEditorUtility.DrawTextStyleField(valueArea, GUIContent.none, _values[i], i.ToString(),
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
            Target.GetComponent<ND.UI.Core.ITextStyleUseAble>().style = _values[pageIndex];
        }

        public override GearConfig BuildConfig(UIExpansionStoredDataBuilder dataBuilder)
        {
            return new GearConfig(_gear, dataBuilder.BuildDataList(Target, _values));
        }

        public override void LoadConfig(GearConfig config)
        {
            _values = new List<ND.UI.Core.TextStyleBase>();
            for (int i = 1; i < config.dataArray.Length; i++)
            {
                _values.Add(UIScript.StoredTextFontStyles[config.dataArray[i]]);
            }
            _state = ControllerTreeItemState.Show;
            State = ControllerTreeItemState.Show;
        }

        public override void MonitorValueChange()
        {
            if (Target.GetComponent<ND.UI.Core.ITextStyleUseAble>().style == _monitorCheckValue)
            {
                return;
            }
            bool stateChanged = _state == ControllerTreeItemState.Hide;
            if (stateChanged)
            {
                _values = new List<ND.UI.Core.TextStyleBase>();
                _state = ControllerTreeItemState.Show;
                State = ControllerTreeItemState.Show;
                for (int i = 0; i < ControllerWrapper.PageNameList.Count; i++)
                {
                    if (i == ControllerWrapper.SelectedIndex)
                    {
                        _values.Add(Target.GetComponent<ND.UI.Core.ITextStyleUseAble>().style);
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
                _values[SelectedIndex] = Target.GetComponent<ND.UI.Core.ITextStyleUseAble>().style;
            }
            _monitorCheckValue = Target.GetComponent<ND.UI.Core.ITextStyleUseAble>().style;
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
                _monitorCheckValue = Target.GetComponent<ND.UI.Core.ITextStyleUseAble>().style;
            }
            else if (pageIndex == NEW_PAGE_INDEX)
            {
                _values.Add(Target.GetComponent<ND.UI.Core.ITextStyleUseAble>().style);
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