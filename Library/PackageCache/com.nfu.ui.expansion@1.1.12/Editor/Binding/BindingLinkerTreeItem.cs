using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Text;

namespace ND.UI
{
    public class BindingLinkerTreeItem : BindingTreeItemBase
    {
        private string _tag;

        private string _linkerTypeNum;

        private int _valueTypeNum;

        private string _label = "";

        public BindingLinkerTreeItem(string tag, string linkerTypeNum, int valueTypeNum)
        {
            _tag = tag;
            _type = BindingTreeItemType.Linker;
            _linkerTypeNum = linkerTypeNum;
            _valueTypeNum = valueTypeNum;
        }

        public BindingLinkerTreeItem(string tag)
        {
            _tag = tag;
            _linkerTypeNum = UIExpansionBinder.AttributeType.Module.ToString();
        }

        public string Tag
        {
            get => _tag;
        }

        public string Label
        {
            get => _label;
            set
            {
                if (_label == value)
                {
                    return;
                }
                _label = value;
                UIExpansionManager.Instance.CurBindingWrapper.CheckAllLabel();
                if (UIExpansionManager.Instance.WindowSettings.AutoSave)
                {
                    string failedStr = null;
                    if (UIExpansionManager.Instance.CurUIExpansionWrapper.CheckCanSave(false, out failedStr))
                    {
                        List<string> missLabelList = null;
                        if (UIExpansionManager.Instance.CurBindingWrapper.CheckAllOldLabelExist(out missLabelList))
                        {
                            UIExpansionManager.Instance.CurUIExpansionWrapper.Save();
                        }
                        else
                        {
                            Debug.Log(string.Format("<color=#ff0000>{0}</color>", "[BindingLinkerTreeItem的检测]"));
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
            }
        }

        public void ConfigSetLabel(string label)
        {
            _label = label;
        }

        public string LinkerTypeStr { get => _linkerTypeNum; set => _linkerTypeNum = value; }

        public int ValueTypeNum { get => _valueTypeNum; set => _valueTypeNum = value; }

        public override void OnDetailGUI(int index, Rect itemArea)
        {
            base.OnDetailGUI(index, itemArea);
            //是否是搜索模式
            if (UIExpansionManager.Instance.BindingSettings.IsSearching == BindingPanelIsSearchingState.True)
            {
                if (_label == UIExpansionManager.Instance.BindingSettings.SearchStr)
                {
                    Rect tagArea = new Rect(
                        itemArea.x - 6 + DepthValue * 20,
                        itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                        140,
                        UIExpansionEditorUtility.SINGLELINE_HEIGHT);
                    EditorGUI.LabelField(tagArea, _tag);

                    Rect labelArea = new Rect(
                        itemArea.x - 6 + DepthValue * 20 + 150,
                        itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                        140,
                        UIExpansionEditorUtility.SINGLELINE_HEIGHT);
                    Label = EditorGUI.TextField(labelArea, Label);
                    IsMatchedSearch = true;
                    MarkParentIsMatched(Parent, true);
                }
            }
            else
            {
                Rect tagArea = new Rect(
                    itemArea.x - 6 + DepthValue * 20,
                    itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                    140,
                    UIExpansionEditorUtility.SINGLELINE_HEIGHT);
                EditorGUI.LabelField(tagArea, _tag);

                Rect labelArea = new Rect(
                    itemArea.x - 6 + DepthValue * 20 + 150,
                    itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                    140,
                    UIExpansionEditorUtility.SINGLELINE_HEIGHT);
                Label = EditorGUI.TextField(labelArea, Label);
                IsMatchedSearch = false;
                MarkParentIsMatched(Parent, false);
            }
            
            if (string.IsNullOrEmpty(_label)) 
            {
                return;
            }
            if (UIExpansionManager.Instance.CurBindingWrapper.IllegalLabelTreeItemList.Count > 0 && UIExpansionManager.Instance.CurBindingWrapper.IllegalLabelTreeItemList.Contains(this))
            {
                Rect warningLabelArea = new Rect(
                    itemArea.x - 6 + DepthValue * 20 + 300,
                    itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                    140,
                    UIExpansionEditorUtility.SINGLELINE_HEIGHT);
                Color tempColor = GUI.color;
                GUI.color = new Color(1, 0, 0, 1);
                EditorGUI.LabelField(warningLabelArea, "含有非法字符", EditorStyles.boldLabel);
                GUI.color = tempColor;
                return;
            }
            
            if (UIExpansionManager.Instance.CurBindingWrapper.IllegalMethodLabelTreeItemList.Count > 0 && UIExpansionManager.Instance.CurBindingWrapper.IllegalMethodLabelTreeItemList.Contains(this))
            {
                Rect warningLabelArea = new Rect(
                    itemArea.x - 6 + DepthValue * 20 + 314,
                    itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                    140,
                    UIExpansionEditorUtility.SINGLELINE_HEIGHT);
                Color tempColor = GUI.color;
                GUI.color = new Color(1, 0, 0, 1);
                EditorGUI.LabelField(warningLabelArea, "不允许重复绑定同名方法", EditorStyles.boldLabel);
                GUI.color = tempColor;
                return;
            }
            
            if (!UIExpansionManager.Instance.CurBindingWrapper.CheckRepeatLabelLegal(_label))
            {
                Rect warningLabelArea = new Rect(
                        itemArea.x - 6 + DepthValue * 20 + 300,
                        itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                        140,
                        UIExpansionEditorUtility.SINGLELINE_HEIGHT);
                Color tempColor = GUI.color;
                GUI.color = new Color(1, 0, 0, 1);
                EditorGUI.LabelField(warningLabelArea, "重复的标签", EditorStyles.boldLabel);
                GUI.color = tempColor;
                return;
            }
        }

        private void MarkParentIsMatched(EditorTreeItemBase parent, bool isMatched)
        {
            if (parent == null)
            {
                return;
            }
            parent.IsMatchedSearch = isMatched;
            MarkParentIsMatched(parent.Parent, isMatched);
        }

        public void OnColumnGUI(int index, Rect itemArea, bool bModule = false)
        {
            base.OnDetailGUI(index, itemArea);
            Rect tagArea = new Rect(
                itemArea.x + 14,
                itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                140,
                UIExpansionEditorUtility.SINGLELINE_HEIGHT);
            EditorGUI.LabelField(tagArea, _tag);

            Rect labelArea = new Rect(
                itemArea.x + 164,
                itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                140,
                UIExpansionEditorUtility.SINGLELINE_HEIGHT);
            
            if (bModule && this.Target.GetComponent<UIExpansion>() == null) 
            {
                GUI.enabled = false; 
            }
            
            Label = EditorGUI.TextField(labelArea, Label);
            GUI.enabled = true;
            
            if (string.IsNullOrEmpty(_label))
            {
                return;
            }
            if (UIExpansionManager.Instance.CurBindingWrapper.IllegalLabelTreeItemList.Count > 0 && UIExpansionManager.Instance.CurBindingWrapper.IllegalLabelTreeItemList.Contains(this))
            {
                Rect warningLabelArea = new Rect(
                    itemArea.x - 6 + DepthValue * 20 + 314,
                    itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                    140,
                    UIExpansionEditorUtility.SINGLELINE_HEIGHT);
                Color tempColor = GUI.color;
                GUI.color = new Color(1, 0, 0, 1);
                EditorGUI.LabelField(warningLabelArea, "含有非法字符", EditorStyles.boldLabel);
                GUI.color = tempColor;
                return;
            }
            
            if (UIExpansionManager.Instance.CurBindingWrapper.IllegalMethodLabelTreeItemList.Count > 0 && UIExpansionManager.Instance.CurBindingWrapper.IllegalMethodLabelTreeItemList.Contains(this))
            {
                Rect warningLabelArea = new Rect(
                    itemArea.x - 6 + DepthValue * 20 + 314,
                    itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                    140,
                    UIExpansionEditorUtility.SINGLELINE_HEIGHT);
                Color tempColor = GUI.color;
                GUI.color = new Color(1, 0, 0, 1);
                EditorGUI.LabelField(warningLabelArea, "不允许重复绑定同名方法", EditorStyles.boldLabel);
                GUI.color = tempColor;
                return;
            }

            if (!UIExpansionManager.Instance.CurBindingWrapper.CheckRepeatLabelLegal(_label))
            {
                Rect warningLabelArea = new Rect(
                      itemArea.x - 6 + DepthValue * 20 + 314,
                      itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                      140,
                      UIExpansionEditorUtility.SINGLELINE_HEIGHT);
                Color tempColor = GUI.color;
                GUI.color = new Color(1, 0, 0, 1);
                EditorGUI.LabelField(warningLabelArea, "重复的标签", EditorStyles.boldLabel);
                GUI.color = tempColor;
                return;
            }
        }

        public string BinderType
        {
            get
            {
                return (_parent as BindingBinderTreeItemBase).BinderType;
            }
        }

        public GameObject Target
        {
            get
            {
                return (_parent.Parent as BindingGameObjectTreeItem).Target;
            }
        }

        public override void RebuildLinkerConfigList(List<LinkerConfig> linkerConfigList, UIExpansionStoredDataBuilder dataBuilder)
        {
            if (string.IsNullOrEmpty(_label))
            {
                return;
            }
            if ((_parent as BindingBinderTreeItemBase).BinderType == "UIExpansion" && _linkerTypeNum == UIExpansionBinder.AttributeType.Module.ToString())
            {
                linkerConfigList.Add(new LinkerConfig(dataBuilder.BuildDataList(Target, BinderType, _linkerTypeNum, _label)));
            }
            else
            {
                linkerConfigList.Add(new LinkerConfig(dataBuilder.BuildDataList(Target, BinderType, _linkerTypeNum, _label, _valueTypeNum)));
            }
            if (!UIExpansionManager.Instance.CurBindingWrapper.OldConfigLabelList.Contains(_label))
            {
                UIExpansionManager.Instance.CurBindingWrapper.OldConfigLabelList.Add(_label);
            }
            base.RebuildLinkerConfigList(linkerConfigList, dataBuilder);
        }

        public override void CheckAllLabel()
        {
            if (_label == "")
            {
                return;
            }
            if (!IsNumAndEnCh(_label))
            {
                UIExpansionManager.Instance.CurBindingWrapper.IllegalLabelTreeItemList.Add(this);
                return;
            }

            if (IsAnyRepeatedMethods())
            {
                // Debug.Log("[Test][有方法重复啦！！！！！！！！！]");
                UIExpansionManager.Instance.CurBindingWrapper.IllegalMethodLabelTreeItemList.Add(this);
                return;
            }
            List<BindingLinkerTreeItem> linkerTreeItemList = null;
            if (!UIExpansionManager.Instance.CurBindingWrapper.LabelCheckDic.TryGetValue(_label, out linkerTreeItemList))
            {
                linkerTreeItemList = new List<BindingLinkerTreeItem>();
                UIExpansionManager.Instance.CurBindingWrapper.LabelCheckDic[_label] = linkerTreeItemList;
            }
            linkerTreeItemList.Add(this);
        }

        public bool IsNumAndEnCh(string input)
        {
            // string pattern = @"^[A-Za-z_]+[0-9]*$";
            string pattern = @"^[A-Za-z_]+[A-Za-z0-9]*$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        public bool IsAnyRepeatedMethods()
        {
            switch (_valueTypeNum)
            {
                case (int)LinkerValueState.SystemObject:
                case (int)LinkerValueState.UnityEvent:
                case (int)LinkerValueState.UnityEventBoolean:
                case (int)LinkerValueState.UnityEventSingle:
                case (int)LinkerValueState.UnityEventInt32:
                case (int)LinkerValueState.UnityEventString :
                case (int)LinkerValueState.UnityEventVector2:
                case (int)LinkerValueState.UnityEvent2:
                case (int)LinkerValueState.UnityEvent2Boolean:
                case (int)LinkerValueState.UnityEvent2Single:
                case (int)LinkerValueState.UnityEvent2Int32:
                case (int)LinkerValueState.UnityEvent2String:
                case (int)LinkerValueState.UnityEvent2Vector2:
                    if (UIExpansionManager.Instance.CurBindingWrapper.LabelCheckDic.ContainsKey(_label))
                    {
                        return true;
                    }
                    return false;
            }
            return false;
            // UIExpansionManager.Instance.CurBindingWrapper.OldConfigLabelList
        }

        /*
        public override void RebuildModuleConfigList(List<LinkerConfig> moduleConfigList, UIExpansionStoredDataBuilder dataBuilder)
        {
            if (string.IsNullOrEmpty(_label))
            {
                return;
            }
            string assetPath = string.Empty;
            if (PrefabUtility.GetPrefabInstanceStatus(Target) == PrefabInstanceStatus.Connected)
            {
                UnityEngine.Object parentObject = PrefabUtility.GetCorrespondingObjectFromSource(Target);
                assetPath = AssetDatabase.GetAssetPath(parentObject);
            }
            else if (PrefabUtility.GetPrefabAssetType(Target) == PrefabAssetType.Regular ||
                  PrefabUtility.GetPrefabAssetType(Target) == PrefabAssetType.Variant)
            {
                assetPath = AssetDatabase.GetAssetPath(Target);
            }
            if ((_parent as BindingBinderTreeItemBase).BinderType == BinderTypeState.UIExpansion || _linkerTypeNum == (int)UIExpansionBinder.AttributeType.Module)
            {
                moduleConfigList.Add(new LinkerConfig(dataBuilder.BuildDataList(Target, BinderTypeNum, _linkerTypeNum, _label, assetPath)));
            }
            base.RebuildModuleConfigList(moduleConfigList, dataBuilder); 
        }
        */
    }
}