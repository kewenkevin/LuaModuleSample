using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;


namespace ND.UI
{
    public class UIExpansionBindingWrapper
    {
        private BindingRootTreeItem _treeRoot;
        
        private UIExpansionGameObjectTreeItem _uiExpansionGoTreeRoot;

        private Dictionary<int, BindingGameObjectTreeItem> _goTreeItemDic;
        
        private Dictionary<int, UIExpansionGameObjectTreeItem> _uiExpansionGoTreeItemDic = new Dictionary<int, UIExpansionGameObjectTreeItem>();

        private List<BindingTreeItemBase> _showTreeList;

        private float _showTreeHeight;

        private List<BindingGameObjectTreeItem> _columnGOTreeList;

        private float _columnGOTreeHeight;

        private Dictionary<string, List<BindingLinkerTreeItem>> _labelCheckDic = new Dictionary<string, List<BindingLinkerTreeItem>>();

        private List<BindingLinkerTreeItem> _illegalLabelTreeItemList = new List<BindingLinkerTreeItem>();
        
        private List<BindingLinkerTreeItem> _illegalMethodLabelTreeItemList = new List<BindingLinkerTreeItem>();

        private int _repeatLabelNum = 0;

        private List<string> _oldConfigLabelList = new List<string>();

        public UIExpansionGameObjectTreeItem UIExpansionGoTreeRoot
        {
            get => _uiExpansionGoTreeRoot;
        }
        public float ShowTreeHeight { get => _showTreeHeight; set => _showTreeHeight = value; }
        public List<BindingTreeItemBase> ShowTreeList { get => _showTreeList; set => _showTreeList = value; }
        public List<BindingGameObjectTreeItem> ColumnGOTreeList { get => _columnGOTreeList; set => _columnGOTreeList = value; }
        public float ColumnGOTreeHeight { get => _columnGOTreeHeight; set => _columnGOTreeHeight = value; }
        public Dictionary<int, BindingGameObjectTreeItem> GoTreeItemDic { get => _goTreeItemDic; set => _goTreeItemDic = value; }
        public Dictionary<int, UIExpansionGameObjectTreeItem> UIExpansionGoTreeItemDic { get => _uiExpansionGoTreeItemDic; set => _uiExpansionGoTreeItemDic = value; }

        public Dictionary<string, List<BindingLinkerTreeItem>> LabelCheckDic
        {
            get
            {
                if (_labelCheckDic == null || _labelCheckDic.Count <= 0)
                {
                    // UIExpansionManager.Instance.CurBindingWrapper.RebuildTree();
                }

                return _labelCheckDic;
            }
            set => _labelCheckDic = value;
        }
        public int RepeatLabelNum { get => _repeatLabelNum; set => _repeatLabelNum = value; }
        public List<BindingLinkerTreeItem> IllegalLabelTreeItemList { get => _illegalLabelTreeItemList; set => _illegalLabelTreeItemList = value; }
        public List<BindingLinkerTreeItem> IllegalMethodLabelTreeItemList { get => _illegalMethodLabelTreeItemList; set => _illegalMethodLabelTreeItemList = value; }
        public List<string> OldConfigLabelList { get => _oldConfigLabelList; set => _oldConfigLabelList = value; }

        public bool CheckRepeatLabelLegal(string label)
        {
            List<BindingLinkerTreeItem> linkerTreeItemList = null;
            if (LabelCheckDic.TryGetValue(label, out linkerTreeItemList))
            {
                return CheckRepeatLabelLegal(linkerTreeItemList);
            }
            else
            {
                return false;
            }
        }

        public bool CheckRepeatLabelLegal(List<BindingLinkerTreeItem> linkerTreeItemList)
        {
            if (linkerTreeItemList.Count == 1)
            {
                return true;
            }
            int ValueTypeNum = linkerTreeItemList[0].ValueTypeNum;
            for (int i = 1; i < linkerTreeItemList.Count; i++)
            {
               if( linkerTreeItemList[i].ValueTypeNum != ValueTypeNum)
               {
                    return false;
               }
            }
            return true;
        }

        public void CheckAllLabel()
        {
            LabelCheckDic = new Dictionary<string, List<BindingLinkerTreeItem>>();
            _illegalLabelTreeItemList = new List<BindingLinkerTreeItem>();
            _illegalMethodLabelTreeItemList = new List<BindingLinkerTreeItem>();
            _repeatLabelNum = 0;
            _treeRoot.CheckAllLabel();
            foreach (var linkerList in LabelCheckDic.Values)
            {
                if (!CheckRepeatLabelLegal(linkerList))
                {
                    _repeatLabelNum += 1;
                }
            }
        }

        public UIExpansionBindingWrapper(BindingConfig config)
        {
            RebuildTree();
            AnalyzeBindingConfig(config);
            RebuildShowTreeList();
            RebuildColumnGOTreeList();
            ReBuildUIExpansionGameObjectTree();
            _oldConfigLabelList = new List<string>();
            Debug.Log(_uiExpansionGoTreeRoot); 
            if (config == null|| config.linkerConfigs==null)
            {
                return;
            }
            foreach (var linkerConfig in config.linkerConfigs)
            {
                if (!_oldConfigLabelList.Contains(UIExpansionManager.Instance.CurUIExpansion.StoredStrings[linkerConfig.LabelIndex]))
                {
                    _oldConfigLabelList.Add(UIExpansionManager.Instance.CurUIExpansion.StoredStrings[linkerConfig.LabelIndex]);
                }
            }
        }

        public bool CheckAllOldLabelExist(out List<string> missLabel)
        {
            bool allExist = true;
            missLabel = new List<string>();
            for (int i = 0;i < _oldConfigLabelList.Count; i++)
            {
                if (!LabelCheckDic.ContainsKey(_oldConfigLabelList[i]))
                {
                    missLabel.Add(_oldConfigLabelList[i]);
                    allExist = false;
                }
            }
            return allExist;
        }

        public void RebuildTree()
        {
            if (UIExpansionManager.Instance.CurUIExpansion.gameObject == null)
            {
                return;
            }
            BindingRootTreeItem newTreeRoot = new BindingRootTreeItem(UIExpansionManager.Instance.CurUIExpansion.gameObject);
            Dictionary<int, BindingGameObjectTreeItem> newGOTreeItemDic = new Dictionary<int, BindingGameObjectTreeItem>();

            newGOTreeItemDic.Add(UIExpansionManager.Instance.CurUIExpansion.gameObject.GetInstanceID(), newTreeRoot);
            List<Type> needAddTypeList = new List<Type>();
            Type binderTreeItemClassType = null;
            foreach (string binderType in UIExpansionUtility.RegisterBinderKeyList)
            {
                if (BindingGameObjectTreeItem.CheckCanAddBindingComponent(newTreeRoot.Target, binderType) && !BindingRootTreeItem.IgnoreBinderType.Contains(binderType))
                {
                    Type binderClassType = BindingGameObjectTreeItem.GetBindingType(newTreeRoot.Target, binderType);
                    bool needAdd = true;
                    for (int i = needAddTypeList.Count - 1; i >= 0; i--)
                    {
                        var checkType = needAddTypeList[i];
                        if (checkType.IsSubclassOf(binderClassType))
                        {
                            needAdd = false;
                            break;
                        }
                        if (binderClassType.IsSubclassOf(checkType))
                        {
                            needAddTypeList.RemoveAt(i);
                            break;
                        }
                        if(binderClassType.Equals(checkType))
                        {
                            needAdd = false;
                            break;
                        }
                    }
                    if (needAdd)
                    {
                        needAddTypeList.Add(binderClassType);
                    }
                }
            }
           
            foreach(var addType in needAddTypeList)
            {
                binderTreeItemClassType = null;
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.Load("Assembly-CSharp-Editor");
                }
                catch (Exception e)
                {

                }
                if (assembly != null)
                {
                    binderTreeItemClassType = assembly.GetType(addType.Name + "BinderTreeItem");
                }
                if (binderTreeItemClassType == null)
                {
                    string binderClassName = "ND.UI." + addType.Name + "BinderTreeItem";
                    binderTreeItemClassType = Type.GetType(binderClassName);

                }
                if (binderTreeItemClassType != null)
                {
                    BindingBinderTreeItemBase binderTreeItem = Activator.CreateInstance(binderTreeItemClassType, true) as BindingBinderTreeItemBase;
                    newTreeRoot.AddChild(binderTreeItem);
                }
            }
           
            foreach (Transform child in UIExpansionManager.Instance.CurUIExpansion.gameObject.transform)
            {
                AddChildToTree(child, newTreeRoot, newGOTreeItemDic, false);
            }

            _treeRoot = newTreeRoot;
            _goTreeItemDic = newGOTreeItemDic;
        }

        public void ReBuildUIExpansionGameObjectTree()
        {
            if (UIExpansionManager.Instance.CurUIExpansion.gameObject == null)
            {
                return;
            }
            UIExpansionGoTreeItemDic.Clear();
            _uiExpansionGoTreeRoot = new UIExpansionGameObjectTreeItem(UIExpansionManager.Instance.CurUIExpansion.gameObject.GetComponent<UIExpansion>(),null);
            UIExpansionGoTreeItemDic.Add(_uiExpansionGoTreeRoot.UIExpansion.gameObject.GetInstanceID(),_uiExpansionGoTreeRoot);
            foreach (Transform child in _uiExpansionGoTreeRoot.UIExpansion.gameObject.transform)
            {
                AddUIExpansionChildToTree(child, _uiExpansionGoTreeRoot);
            }
        }

        public void AddUIExpansionChildToTree(Transform child, UIExpansionGameObjectTreeItem parent)
        {
            UIExpansion uiExpansion = child.GetComponent<UIExpansion>();
            if (uiExpansion == null)
            {
                return;
            }
            UIExpansionGameObjectTreeItem uiExpansionGameObjectTreeItem = new UIExpansionGameObjectTreeItem(uiExpansion,parent);
            parent.AddChildToTree(uiExpansionGameObjectTreeItem);
            UIExpansionGoTreeItemDic.Add(uiExpansionGameObjectTreeItem.UIExpansion.gameObject.GetInstanceID(), uiExpansionGameObjectTreeItem);
            foreach (Transform subChild in child)
            {
                AddUIExpansionChildToTree(subChild, uiExpansionGameObjectTreeItem);
            }
        }

        public void Refresh()
        {
            if (UIExpansionManager.Instance.CurUIExpansion.gameObject == null)
            {
                return;
            }
            BindingRootTreeItem newTreeRoot = new BindingRootTreeItem(UIExpansionManager.Instance.CurUIExpansion.gameObject);
            Dictionary<int, BindingGameObjectTreeItem> newGOTreeItemDic = new Dictionary<int, BindingGameObjectTreeItem>();

            newGOTreeItemDic.Add(UIExpansionManager.Instance.CurUIExpansion.gameObject.GetInstanceID(), newTreeRoot);
            List<Type> needAddTypeList = new List<Type>();
            foreach (string binderType in UIExpansionUtility.RegisterBinderKeyList)
            {
                if (BindingGameObjectTreeItem.CheckCanAddBindingComponent(newTreeRoot.Target, binderType) && !BindingRootTreeItem.IgnoreBinderType.Contains(binderType))
                {
                    Type binderClassType = BindingGameObjectTreeItem.GetBindingType(newTreeRoot.Target, binderType);
                    bool needAdd = true;
                    for (int i = needAddTypeList.Count - 1; i >= 0; i--)
                    {
                        var checkType = needAddTypeList[i];
                        if (checkType.IsSubclassOf(binderClassType))
                        {
                            needAdd = false;
                            break;
                        }
                        if (binderClassType.IsSubclassOf(checkType))
                        {
                            needAddTypeList.RemoveAt(i);
                            break;
                        }
                        if (binderClassType.Equals(checkType))
                        {
                            needAdd = false;
                            break;
                        }
                    }
                    if (needAdd)
                    {
                        needAddTypeList.Add(binderClassType);
                    }
                }
            }
            foreach (var addType in needAddTypeList)
            {
                Type binderTreeItemClassType = null;
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.Load("Assembly-CSharp-Editor");
                }
                catch (Exception e)
                {

                }
                if (assembly != null)
                {
                    binderTreeItemClassType = assembly.GetType(addType.Name + "BinderTreeItem");
                }
                if (binderTreeItemClassType == null)
                {
                    string binderClassName = "ND.UI." + addType.Name + "BinderTreeItem";
                    binderTreeItemClassType = Type.GetType(binderClassName);

                }
                if (binderTreeItemClassType != null)
                {
                    BindingBinderTreeItemBase binderTreeItem = Activator.CreateInstance(binderTreeItemClassType, true) as BindingBinderTreeItemBase;
                    newTreeRoot.AddChild(binderTreeItem);
                }
            }
            foreach (Transform child in UIExpansionManager.Instance.CurUIExpansion.gameObject.transform)
            {
                AddChildToTree(child, newTreeRoot, newGOTreeItemDic, true);
            }

            _treeRoot = newTreeRoot;
            _goTreeItemDic = newGOTreeItemDic;
            
            // ReBuildUIExpansionGameObjectTree();
        }

        public void AddChildToTree(Transform target, BindingTreeItemBase parent, Dictionary<int, BindingGameObjectTreeItem> goTreeItemDic, bool refresh)
        {
            BindingGameObjectTreeItem gameObjectTreeItem = new BindingGameObjectTreeItem(target.gameObject);
            parent.AddChild(gameObjectTreeItem);
            goTreeItemDic.Add(target.gameObject.GetInstanceID(), gameObjectTreeItem);
            bool needBlock = false;
            List<Type> needAddTypeList = new List<Type>();
            foreach (string binderType in UIExpansionUtility.RegisterBinderKeyList)
            {
                if (BindingGameObjectTreeItem.CheckCanAddBindingComponent(target.gameObject, binderType))
                {
                    Type binderClassType = BindingGameObjectTreeItem.GetBindingType(target.gameObject, binderType);
                    bool needAdd = true;
                    for (int i = needAddTypeList.Count - 1; i >= 0; i--)
                    {
                        var checkType = needAddTypeList[i];
                        if (checkType.IsSubclassOf(binderClassType))
                        {
                            needAdd = false;
                            break;
                        }
                        if (binderClassType.IsSubclassOf(checkType))
                        {
                            needAddTypeList.RemoveAt(i);
                            break;
                        }
                        if (binderClassType.Equals(checkType))
                        {
                            needAdd = false;
                            break;
                        }
                    }
                    if (needAdd)
                    {
                        needAddTypeList.Add(binderClassType);
                    }
                }
            }
            foreach (var addType in needAddTypeList)
            {
                if (refresh)
                {
                    BindingGameObjectTreeItem oldGameObjectTreeItem = null;
                    if (_goTreeItemDic.TryGetValue(target.gameObject.GetInstanceID(), out oldGameObjectTreeItem))
                    {
                        BindingBinderTreeItemBase oldBinderTreeItem = oldGameObjectTreeItem.GetTargetBinder(addType.Name);
                        if (oldBinderTreeItem != null)
                        {
                            gameObjectTreeItem.AddChild(oldBinderTreeItem);
                            continue;
                        }
                    }
                }
                Type binderTreeItemClassType = null;
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.Load("Assembly-CSharp-Editor");
                }
                catch (Exception e)
                {

                }
                if (assembly != null)
                {
                    binderTreeItemClassType = assembly.GetType(addType.Name + "BinderTreeItem");
                }
                if (binderTreeItemClassType == null)
                {
                    string binderClassName = "ND.UI." + addType.Name + "BinderTreeItem";
                    binderTreeItemClassType = Type.GetType(binderClassName);

                }
                if (binderTreeItemClassType != null)
                {
                    BindingBinderTreeItemBase binderTreeItem = Activator.CreateInstance(binderTreeItemClassType, true) as BindingBinderTreeItemBase;
                    gameObjectTreeItem.AddChild(binderTreeItem);
                }
                if (addType.Name == "UIExpansion")
                {
                    needBlock = true;
                }
            }

            if (needBlock)
            {
                return;
            }
            foreach (Transform child in target)
            {
                AddChildToTree(child, gameObjectTreeItem, goTreeItemDic, refresh);
            }
        }

        public void ImplicitSetfoldoutValue(bool foldoutValue)
        {
            _treeRoot.ImplicitSetfoldoutValue(foldoutValue);
        }

        public void AnalyzeBindingConfig(BindingConfig config)
        {
            if (config == null || config.linkerConfigs == null || config.linkerConfigs.Length == 0)
            {
                return;
            }
            for(int i = 0;i < config.linkerConfigs.Length; i++)
            {
                LinkerConfig linkerConfig = config.linkerConfigs[i];
                GameObject target = UIExpansionManager.Instance.CurUIExpansion.GetStoredGameObject(linkerConfig.StoredGameObjectIndex);
                if (target == null)
                {
                    continue;
                }
                BindingGameObjectTreeItem goTreeItem = null;
                if (!_goTreeItemDic.TryGetValue(target.GetInstanceID(), out goTreeItem))
                {
                    continue;
                }
                BindingBinderTreeItemBase BinderTreeItem = null;
                string binderType = UIExpansionManager.Instance.CurUIExpansion.StoredStrings[linkerConfig.BinderTypeIndex];
                switch (binderType)
                {
                    default:
                        BinderTreeItem = goTreeItem.GetTargetBinder(binderType);
                        break;
                }
                if (BinderTreeItem == null)
                {
                    continue;
                }
                BinderTreeItem.LoadConfig(linkerConfig);
            }
        }

        public void RebuildShowTreeList()
        {
            _showTreeList = new List<BindingTreeItemBase>();
            if (UIExpansionManager.Instance.BindingSettings != null)
            {
                _showTreeHeight = _treeRoot.RebuildShowTreeList(_showTreeList, UIExpansionManager.Instance.BindingSettings.ShowState);
            }
            else
            {
                _showTreeHeight = _treeRoot.RebuildShowTreeList(_showTreeList, BindingTreeItemState.Total);
            }
        }

        public void RebuildColumnGOTreeList()
        {
            _columnGOTreeList = new List<BindingGameObjectTreeItem>();
            if (UIExpansionManager.Instance.BindingSettings != null)
            {
                _columnGOTreeHeight = _treeRoot.RebuildColumnGOTreeList(_columnGOTreeList, UIExpansionManager.Instance.BindingSettings.ShowState);
            }
            else
            {
                _columnGOTreeHeight = _treeRoot.RebuildColumnGOTreeList(_columnGOTreeList,  BindingTreeItemState.Total);
            }
        }

        public List<LinkerConfig> RebuildLinkerConfigList(UIExpansionStoredDataBuilder dataBuilder)
        {
            _oldConfigLabelList = new List<string>();
            List<LinkerConfig> linkerConfigList = new List<LinkerConfig>();
            _treeRoot.RebuildLinkerConfigList(linkerConfigList, dataBuilder);
            return linkerConfigList;
        }
    }
}