using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using ND.UI.Core;
using ND.UI.Core.Model;

namespace ND.UI
{
    public abstract class ControllerGearTreeItemBase : ControllerTreeItemBase
    {

        public const int NEW_PAGE_INDEX = 10000;

        public const int MONITOR_INDEX = -2;

        protected GearTypeState _gear;

        public GearTypeState Gear
        {
            get
            {
                return _gear;
            }

            set
            {
                _gear = value;
            }
        }

        public GameObject Target
        {
            get { return (_parent as ControllerGameObjectTreeItem).Target; }
        }

        public int SelectedIndex
        {
            get { return UIExpansionManager.Instance.CurControllerWrapper.SelectedIndex; }
        }

        public UIExpansion UIScript
        {
            get { return UIExpansionManager.Instance.CurUIExpansion; }
        }

        public UIExpansionControllerWrapper ControllerWrapper
        {
            get { return UIExpansionManager.Instance.CurControllerWrapper; }
        }
        
        
        public virtual string GearName
        {
            get { return _gear.ToString(); }
        }

        public ControllerGearTreeItemBase(GearTypeState gearType)
        {
            _type = ControllerTreeItemType.Gear;
            _gear = gearType;
        }

        public override void OnHeaderGUI(int index, Rect itemArea)
        {
            base.OnHeaderGUI(index, itemArea);
            Rect tagArea = new Rect(
                itemArea.x - 6 + DepthValue * 20,
                itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                140,
                EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(tagArea, _gear.ToString());
        }

        public override void OnHeaderPriorityGUI(int index, Rect itemArea)
        {
            var newPriority = (GearPriority) EditorGUI.EnumPopup(new Rect(itemArea.x, itemArea.y+ (_height - EditorGUIUtility.singleLineHeight) / 2, itemArea.width,  EditorGUIUtility.singleLineHeight), Priority);
            if (Priority != newPriority)
            {
                Priority = newPriority;
                UIExpansionManager.Instance.CurControllerWrapper.RebuildShowTreeList();
            }
        }

        public override void OnAddGearGUI(int index, Rect itemArea)
        {
            base.OnAddGearGUI(index, itemArea);
            Rect tagArea = new Rect(
                itemArea.x - 6 + DepthValue * 20,
                itemArea.y + (_addGearHeight - EditorGUIUtility.singleLineHeight) / 2,
                140,
                EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(tagArea, GearName);

            Rect addBtnArea = new Rect(
                itemArea.xMax - 20,
                itemArea.y + (_addGearHeight - EditorGUIUtility.singleLineHeight) / 2,
                20,
                EditorGUIUtility.singleLineHeight);
            if (GUI.Button(addBtnArea, "+", EditorStyles.toolbarButton))
            {
                State = ControllerTreeItemState.Show;
                UIExpansionManager.Instance.CurControllerWrapper.RebuildShowTreeList();
                UIExpansionManager.Instance.ControllerSettings.InAddNewGearState = false;
            }
        }

        protected override void OnSelfStateChange(ControllerTreeItemState state)
        {
            if (state == ControllerTreeItemState.Show)
            {
                Init();
            }
        }

        public override void OnAddNewPage()
        {
            if(_state == ControllerTreeItemState.Show)
            {
                RecordValue();
            }
            base.OnAddNewPage();
        }

        public override void Apply()
        {
            if (_state == ControllerTreeItemState.Show)
            {
                ApplyValue(SelectedIndex);
            }
            base.Apply();
        }

        public virtual void Init()
        {
            for (int i = 0; i < ControllerWrapper.PageNameList.Count; i++)
            {
                RecordValue();
            }
        }

        protected virtual void OnValueChanged()
        {
            if (UIExpansionManager.Instance.ControllerSettings.AutoApply)
            {
                UIExpansionManager.Instance.CurControllerWrapper.Apply();
            }
            if (UIExpansionManager.Instance.WindowSettings.AutoSave)
            {
                string failedStr = null;
                if (UIExpansionManager.Instance.CurUIExpansionWrapper.CheckCanSave(true,out failedStr))
                {
                    List<string> missLabelList = null;
                    if (UIExpansionManager.Instance.CurBindingWrapper.CheckAllOldLabelExist(out missLabelList))
                    {
                        UIExpansionManager.Instance.CurUIExpansionWrapper.Save();
                    }
                    else
                    {
                        Debug.Log(string.Format("<color=#ff0000>{0}</color>", "[ControllerGearTreeItemBase的检测]"));
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("双向绑定界面有部分Label被删除：");
                        for (int i = 0; i < missLabelList.Count; i++)
                        {
                            sb.Append(missLabelList[i]);
                            sb.Append("  ");
                        }
                        sb.AppendLine();
                        sb.Append("是否继续保存？");
                        if (EditorUtility.DisplayDialog("Label丢失确认", sb.ToString(), "保存", "取消"))
                        {
                            UIExpansionManager.Instance.CurUIExpansionWrapper.Save();
                        }
                    }
                }
                else
                {
                    Debug.Log(string.Format("{0}原因: {1}", "保存失败,已退出自动保存模式", failedStr));// EditorUtility.DisplayDialog("保存失败", failedStr, "确认");
                }
            }
            UIExpansionManager.Instance.NeedRepaint = true;
        }

        public override void RebuildGearConfigList(List<GearConfig> gearConfigList, UIExpansionStoredDataBuilder dataBuilder)
        {
            if (_state == ControllerTreeItemState.Hide)
            {
                return;
            }

            var config = BuildConfig(dataBuilder);
            config.priority = Priority;
            gearConfigList.Add(config);
            base.RebuildGearConfigList(gearConfigList, dataBuilder);
        }


        public virtual void LoadGearConfig(GearConfig config)
        {
            LoadConfig(config);
            Priority = config.priority;
        }

        public abstract void LoadConfig(GearConfig config);

        public abstract GearConfig BuildConfig(UIExpansionStoredDataBuilder dataBuilder);

        public abstract void RecordValue(int pageIndex = NEW_PAGE_INDEX);

        public abstract void ApplyValue(int pageIndex);

        public abstract void MonitorValueChange();
    }
}