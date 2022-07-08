using System.Collections.Generic;
using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ND.UI
{
    public class ImageMaterialGearTreeItem : ControllerGearTreeItemBase
    {
        private Material _monitorCheckValue;
        private List<Material> _values;

        public List<Material> Values
        {
            get => _values;
            set => _values = value;
        }

        public ImageMaterialGearTreeItem() : base(GearTypeState.ImageMaterial)
        {
            _values = new List<Material>();
            //_height = 82;
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
                    EditorGUIUtility.singleLineHeight);
                Material tempValue = EditorGUI.ObjectField(valueArea, _values[i], typeof(Material), false) as Material;
                if (tempValue != _values[i])
                {
                    _values[i] = tempValue;
                    this.OnValueChanged();
                }
            }
        }

        public override void ApplyValue(int pageIndex)
        {
            Target.GetComponent<Graphic>().material = _values[pageIndex];
        }
        
        public override GearConfig BuildConfig(UIExpansionStoredDataBuilder dataBuilder)
        {
            return new GearConfig(_gear, dataBuilder.BuildDataList(Target, _values));
        }
        
        public override void LoadConfig(GearConfig config)
        {
            _values = new List<Material>();
            for (int i = 1; i < config.dataArray.Length; i++)
            {
                _values.Add(UIScript.StoredMaterials[config.dataArray[i]]);
            }
            _state = ControllerTreeItemState.Show;
            State = ControllerTreeItemState.Show;
        }
        
        public override void MonitorValueChange()
        {
            if (Target.GetComponent<Graphic>().material == _monitorCheckValue)
            {
                return;
            }
            bool stateChanged = _state == ControllerTreeItemState.Hide;
            if (stateChanged)
            {
                _values = new List<Material>();
                _state = ControllerTreeItemState.Show;
                State = ControllerTreeItemState.Show;
                for (int i = 0; i < ControllerWrapper.PageNameList.Count; i++)
                {
                    if (i == ControllerWrapper.SelectedIndex)
                    {
                        _values.Add(Target.GetComponent<Graphic>().material);
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
                _values[SelectedIndex] = Target.GetComponent<Graphic>().material;
            }
            _monitorCheckValue = Target.GetComponent<Graphic>().material;
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
                _monitorCheckValue = Target.GetComponent<Graphic>().material;
            }
            else if (pageIndex == NEW_PAGE_INDEX)
            {
                _values.Add(Target.GetComponent<Graphic>().material);
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