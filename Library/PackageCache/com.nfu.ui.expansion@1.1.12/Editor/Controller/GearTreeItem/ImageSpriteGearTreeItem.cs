using System.Collections.Generic;
using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace ND.UI
{
    public class ImageSpriteGearTreeItem : ControllerGearTreeItemBase
    {
        private Sprite _monitorCheckValue;

        private List<Sprite> _values;

        public List<Sprite> Values
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

        public ImageSpriteGearTreeItem() : base(GearTypeState.ImageSprite)
        {
            _values = new List<Sprite>();
            _height = 82;
        }

        public override void OnDetailGUI(int index, Rect itemArea)
        {
            base.OnDetailGUI(index, itemArea);
            float length = 80;
            for (int i = 0; i < _values.Count; i++)
            {
                Rect valueArea = new Rect(
                    itemArea.x + UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH * i + (UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH - length) / 2,
                    itemArea.y + 1,
                    length,
                    length);
                Sprite tempValue = EditorGUI.ObjectField(valueArea, _values[i], typeof(Sprite), false) as Sprite;
                if (tempValue != _values[i])
                {
                    _values[i] = tempValue;
                    this.OnValueChanged();
                }
            }
        }

        public override void ApplyValue(int pageIndex)
        {
            Target.GetComponent<Image>().sprite = _values[pageIndex];
        }

        public override GearConfig BuildConfig(UIExpansionStoredDataBuilder dataBuilder)
        {
            return new GearConfig(_gear, dataBuilder.BuildDataList(Target, _values));
        }

        public override void LoadConfig(GearConfig config)
        {
            _values = new List<Sprite>();
            for (int i = 1; i < config.dataArray.Length; i++)
            {
                _values.Add(UIScript.StoredSprites[config.dataArray[i]]);
            }
            _state = ControllerTreeItemState.Show;
            State = ControllerTreeItemState.Show;
        }

        public override void MonitorValueChange()
        {
            if (Target.GetComponent<Image>().sprite == _monitorCheckValue)
            {
                return;
            }
            bool stateChanged = _state == ControllerTreeItemState.Hide;
            if (stateChanged)
            {
                _values = new List<Sprite>();
                _state = ControllerTreeItemState.Show;
                State = ControllerTreeItemState.Show;
                for (int i = 0; i < ControllerWrapper.PageNameList.Count; i++)
                {
                    if (i == ControllerWrapper.SelectedIndex)
                    {
                        _values.Add(Target.GetComponent<Image>().sprite);
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
                _values[SelectedIndex] = Target.GetComponent<Image>().sprite;
            }
            _monitorCheckValue = Target.GetComponent<Image>().sprite;
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
                _monitorCheckValue = Target.GetComponent<Image>().sprite;
            }
            else if (pageIndex == NEW_PAGE_INDEX)
            {
                _values.Add(Target.GetComponent<Image>().sprite);
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