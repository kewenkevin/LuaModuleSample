using System.Collections.Generic;
using ND.UI.Core;
using ND.UI.Core.Model;
using ND.UI.I18n;
using UnityEditor;
using UnityEngine;

namespace ND.UI
{
    public class LocalizationKeyGearTreeItem: ControllerGearTreeItemBase
    {
        private string _monitorCheckValue;

        private List<string> _values;
        private List<string> _tmpValues;
        
        private GearTypeState _targetGearType = GearTypeState.LocalizationKey;
       
        
        public override GearPriority Priority { get; set; } = GearPriority.Lowest;

        public List<string> Values
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

        public LocalizationKeyGearTreeItem() : base(GearTypeState.LocalizationKey)
        {
            _values = new List<string>();
            _tmpValues = new List<string>();
            _height = 60;
        }

        public override void OnDetailGUI(int index, Rect itemArea)
        {
            base.OnDetailGUI(index, itemArea);
            float width = UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH - 20;
            var padding = 2;

            for (int i = 0; i < _values.Count; i++)
            {
                var currentY = itemArea.y + padding;
                var height = (_height - padding * 3) / 3;
                Rect valueArea = new Rect(
                    itemArea.x + UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH * i +
                    (UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH - width) / 2,
                    itemArea.y + padding,
                    width,
                    height * 2);
                currentY += height * 2;
                Rect btnArea = new Rect(
                    itemArea.x + UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH * i +
                    (UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH - width) / 2,
                    currentY,
                    width,
                    height);

                var tempValue = GUI.TextArea(valueArea, _tmpValues[i]);
                bool bValidKey = LocalizationUtils.CheckLocalizationKey(tempValue);
                if (tempValue != _tmpValues[i])
                {
                    if (bValidKey)
                    {
                        _values[i] = tempValue;
                        this.OnValueChanged();
                    }
                    _tmpValues[i] = tempValue;
                }

                if (!bValidKey)
                {
                    GUIStyle guiStyle = new GUIStyle(EditorStyles.label);
                    guiStyle.normal.textColor = Color.red;
                    GUI.TextArea(btnArea, "仅能由小写字母，数字和“-”，“_”组成", guiStyle);
                }

                if (string.IsNullOrEmpty(tempValue))
                {
                    if (GUI.Button(btnArea, "生成Key"))
                    {
                        var p = _parent as ControllerGameObjectTreeItem;

                        for (int j = 0; j < p.ChildrenList.Count; j++)
                        {
                            var item = p.ChildrenList[j] as TextStrGearTreeItem;
                            if (item != null && item.State == ControllerTreeItemState.Show)
                            {
                                Debug.Log("generate");
                                _values[i] = LocalizationUtils.GenerateLocalizationKey(item.Values[i]);
                                _tmpValues[i] = _values[i];
                                this.OnValueChanged();
                            }
                        }
                    }
                }
            }
        }

        public override void ApplyValue(int pageIndex)
        {
            var components = Target.GetComponents<ILocalizationable>();
            for (int i = 0; i < components.Length; i++)
            {
                if ((int) _targetGearType == components[i].LocalizationGearType)
                { 
                    components[i].LocalizationKey = (_values[pageIndex]);
                    break;
                }
            }
        }

        
        public override void OnHeaderGUI(int index, Rect itemArea)
        {
            base.OnHeaderGUI(index, itemArea);

            var target = GetTargetComponent() as UnityEngine.Object;
            Rect GameObjectArea = new Rect(
                itemArea.x - 6 + DepthValue * 20,
                itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2 + EditorGUIUtility.singleLineHeight,
                140,
                EditorGUIUtility.singleLineHeight);
            GUI.enabled = false;
            EditorGUI.ObjectField(GameObjectArea, target, typeof(ILocalizationable), true);
            GUI.enabled = true;
        }
        public override GearConfig BuildConfig(UIExpansionStoredDataBuilder dataBuilder)
        {
            return new GearConfig(_gear, dataBuilder.BuildDataList(Target, _targetGearType,_values));
        }

        public override void LoadConfig(GearConfig config)
        {
            _targetGearType = (GearTypeState)config.dataArray[1];
            _values = new List<string>();
            _tmpValues = new List<string>();
            for (int i = 2; i < config.dataArray.Length; i++)
            {
                _values.Add(UIScript.StoredStrings[config.dataArray[i]]);
                _tmpValues.Add(UIScript.StoredStrings[config.dataArray[i]]);
            }
            _state = ControllerTreeItemState.Show;
            State = ControllerTreeItemState.Show;
        }

        private ILocalizationable GetTargetComponent()
        {
            var components = Target.GetComponents<ILocalizationable>();
            for (int i = 0; i < components.Length; i++)
            {
                if ((int) _targetGearType == components[i].LocalizationGearType)
                { 
                    return components[i];
                }
            }
            return null;
        }

        public override void MonitorValueChange()
        {
            var target = GetTargetComponent();
            if (target.LocalizationKey == _monitorCheckValue)
            {
                return;
            }
            bool stateChanged = _state == ControllerTreeItemState.Hide;
            if (stateChanged)
            {
                _values = new List<string>();
                _tmpValues = new List<string>();
                _state = ControllerTreeItemState.Show;
                State = ControllerTreeItemState.Show;
                for (int i = 0; i < ControllerWrapper.PageNameList.Count; i++)
                {
                    if (i == ControllerWrapper.SelectedIndex)
                    {
                        _values.Add(target.LocalizationKey);
                        _tmpValues.Add(target.LocalizationKey);
                    }
                    else
                    {
                        _values.Add(_monitorCheckValue);
                        _tmpValues.Add(_monitorCheckValue);
                    }
                }
                UIExpansionManager.Instance.CurControllerWrapper.RebuildShowTreeList();
            }
            else
            {
                _values[SelectedIndex] = target.LocalizationKey;
                _tmpValues[SelectedIndex] = target.LocalizationKey;
            }
            _monitorCheckValue = target.LocalizationKey;
            this.OnValueChanged();
        }

        public override void OnRemovePage(int pageIndex)
        {
            if (_state == ControllerTreeItemState.Show)
            {
                _values.RemoveAt(pageIndex);
                _tmpValues.RemoveAt(pageIndex);
            }
            base.OnRemovePage(pageIndex);
        }

        public override void RecordValue(int pageIndex = NEW_PAGE_INDEX)
        {
            if (pageIndex == NEW_PAGE_INDEX && _targetGearType == GearTypeState.LocalizationKey)
            {
                _targetGearType = (GearTypeState)Target.GetComponents<ILocalizationable>()[0].LocalizationGearType;
            }

            var target = GetTargetComponent();
            
            if (pageIndex == MONITOR_INDEX)
            {
                _monitorCheckValue = target.LocalizationKey;
            }
            else if (pageIndex == NEW_PAGE_INDEX)
            {
                _values.Add(target.LocalizationKey);
                _tmpValues.Add(target.LocalizationKey);
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