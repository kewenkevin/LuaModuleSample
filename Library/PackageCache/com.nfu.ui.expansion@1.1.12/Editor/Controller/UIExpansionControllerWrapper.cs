using System;
using System.Collections.Generic;
using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEditor;
using UnityEngine;

namespace ND.UI
{
    public class UIExpansionControllerWrapper
    {
        private string _name;

        private List<string> _pageNameList;

        private List<string> _pageTipsList;

        private int _selectedIndex = 0;

        private ControllerRootTreeItem _treeRoot;

        private Dictionary<int, ControllerGameObjectTreeItem> _goTreeItemDic;

        private List<ControllerTreeItemBase> _showTreeList;
        
        private float _showTreeHeight;

        private List<ControllerTreeItemBase> _addGearTreeList;

        private float _addGearTreeHeight;

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public List<string> PageNameList
        {
            get
            {
                return _pageNameList;
            }

            set
            {
                _pageNameList = value;
            }
        }
        
        public List<string> PageTipsList
        {
            get
            {
                return _pageTipsList;
            }

            set
            {
                _pageTipsList = value;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }

            set
            {
                _selectedIndex = value;
            }
        }

        public List<ControllerTreeItemBase> ShowTreeList
        {
            get
            {
                return _showTreeList;
            }

            set
            {
                _showTreeList = value;
            }
        }

        public float ShowTreeHeight
        {
            get
            {
                return _showTreeHeight;
            }

            set
            {
                _showTreeHeight = value;
            }
        }

        public List<ControllerTreeItemBase> AddGearTreeList
        {
            get
            {
                return _addGearTreeList;
            }

            set
            {
                _addGearTreeList = value;
            }
        }

        public float AddGearTreeHeight
        {
            get
            {
                return _addGearTreeHeight;
            }

            set
            {
                _addGearTreeHeight = value;
            }
        }

        public UIExpansionControllerWrapper(ControllerConfig config)
        {
            RebuildTree();
            AnalyzeControllerConfig(config);
            RebuildShowTreeList();
        }

        public void AnalyzeControllerConfig(ControllerConfig config)
        {
            _name = config.name;
            _pageNameList = new List<string>();
            _pageTipsList = new List<string>();
            for (int i = 0; i < config.pageNames.Length; i++)
            {
                _pageNameList.Add(config.pageNames[i]);
            }

            if (config.pageTips == null || config.pageTips.Length != config.pageNames.Length)
            {
                //标签备注
                for (int i = 0; i < config.pageNames.Length; i++)
                {
                    _pageTipsList.Add("");
                }
            }
            else
            {
                //标签备注
                for (int i = 0; i < config.pageTips.Length; i++)
                {
                    _pageTipsList.Add(config.pageTips[i]);
                }
            }
            
            
            _selectedIndex = config.selectedIndex;
            if (config.gearConfigs == null)
            {
                return;
            }
            for (int i = 0; i < config.gearConfigs.Length; i++)
            {
                GearConfig gearConfig = config.gearConfigs[i];
                GameObject target = UIExpansionManager.Instance.CurUIExpansion.GetStoredGameObject(gearConfig.StoredGameObjectIndex);
                if (target == null)
                {
                    continue;
                }
                ControllerGameObjectTreeItem goTreeItem = null;
                if (!_goTreeItemDic.TryGetValue(target.GetInstanceID(), out goTreeItem))
                {
                    continue;
                }
                ControllerGearTreeItemBase gearTreeItem = null;
                switch (gearConfig.gearType)
                {
                    case GearTypeState.Controller:
                        gearTreeItem = goTreeItem.GetTargetGear(gearConfig.gearType, UIExpansionManager.Instance.CurUIExpansion.StoredStrings[gearConfig.dataArray[1]]);
                        break;
                    default:
                        gearTreeItem = goTreeItem.GetTargetGear(gearConfig.gearType);
                        break;
                }

                if (gearTreeItem == null)
                {
                    continue;
                }
                gearTreeItem.LoadGearConfig(gearConfig);
            }
        }

        public UIExpansionControllerWrapper(string name,List<string> pageNameList, List<string> pageTipsList)
        {
            _name = name;
            _pageNameList = pageNameList;
            _pageTipsList = pageTipsList;
            RebuildTree();
            RebuildShowTreeList();
        }

        public void RebuildTree()
        {
            if (UIExpansionManager.Instance.CurUIExpansion.gameObject == null)
            {
                return;
            }
            ControllerRootTreeItem newTreeRoot = new ControllerRootTreeItem(UIExpansionManager.Instance.CurUIExpansion.gameObject);
            Dictionary<int, ControllerGameObjectTreeItem> newGOTreeItemDic = new Dictionary<int, ControllerGameObjectTreeItem>();

            newGOTreeItemDic.Add(UIExpansionManager.Instance.CurUIExpansion.gameObject.GetInstanceID(), newTreeRoot);
            newTreeRoot.AddGearItems();

            foreach (Transform child in UIExpansionManager.Instance.CurUIExpansion.gameObject.transform)
            {
                AddChildToTree(child, newTreeRoot, newGOTreeItemDic, false);
            }

            _treeRoot = newTreeRoot;
            _goTreeItemDic = newGOTreeItemDic;
        }

        public void AddChildToTree(Transform target, ControllerTreeItemBase parent, Dictionary<int, ControllerGameObjectTreeItem> goTreeItemDic, bool refresh)
        {
            ControllerGameObjectTreeItem gameObjectTreeItem = new ControllerGameObjectTreeItem(target.gameObject);
            parent.AddChild(gameObjectTreeItem);
            goTreeItemDic.Add(target.gameObject.GetInstanceID(), gameObjectTreeItem);
            foreach (GearTypeState gearType in Enum.GetValues(typeof(GearTypeState)))
            {
                if (ControllerGameObjectTreeItem.CheckCanAddGear(target.gameObject, gearType))
                {
                    switch (gearType)
                    {
                        case GearTypeState.Controller:
                            for (int i = 0; i < target.gameObject.GetComponent<UIExpansion>().ControllerConfigs.Length; i++)
                            {
                                if (refresh)
                                {
                                    ControllerGameObjectTreeItem oldGameObjectTreeItem = null;
                                    if (_goTreeItemDic.TryGetValue(target.gameObject.GetInstanceID(), out oldGameObjectTreeItem))
                                    {
                                        ControllerGearTreeItemBase oldGearTreeItem = oldGameObjectTreeItem.GetTargetGear(gearType, target.gameObject.GetComponent<UIExpansion>().ControllerConfigs[i].name);
                                        if (oldGearTreeItem != null)
                                        {
                                            gameObjectTreeItem.AddChild(oldGearTreeItem);
                                            continue;
                                        }
                                    }
                                }
                                ControllerGearTreeItem controllerGearTreeItem = new ControllerGearTreeItem(target.gameObject.GetComponent<UIExpansion>().ControllerConfigs[i].name,
                                    target.gameObject.GetComponent<UIExpansion>().ControllerConfigs[i].pageNames);
                                gameObjectTreeItem.AddChild(controllerGearTreeItem);
                            }
                            break;
                        case GearTypeState.Transition:

                            break;
                        default:
                            if (refresh)
                            {
                                ControllerGameObjectTreeItem oldGameObjectTreeItem = null;
                                if (_goTreeItemDic.TryGetValue(target.gameObject.GetInstanceID(), out oldGameObjectTreeItem))
                                {
                                    ControllerGearTreeItemBase oldGearTreeItem = oldGameObjectTreeItem.GetTargetGear(gearType);
                                    if (oldGearTreeItem != null)
                                    {
                                        gameObjectTreeItem.AddChild(oldGearTreeItem);
                                        continue;
                                    }
                                }
                            }
                            string gearClassName = "ND.UI." + gearType.ToString() + "GearTreeItem";
                            Type gearClassType = Type.GetType(gearClassName);
                            ControllerGearTreeItemBase gearTreeItem = Activator.CreateInstance(gearClassType, true) as ControllerGearTreeItemBase;
                            gameObjectTreeItem.AddChild(gearTreeItem);
                            break;
                    }
                }
            }

            foreach (Transform child in target)
            {
                AddChildToTree(child, gameObjectTreeItem, goTreeItemDic, refresh);
            }
        }

        public void Refresh()
        {
            // RebuildTree();
            if (UIExpansionManager.Instance.CurUIExpansion.gameObject == null)
            {
                return;
            }
            ControllerRootTreeItem newTreeRoot = new ControllerRootTreeItem(UIExpansionManager.Instance.CurUIExpansion.gameObject);
            Dictionary<int, ControllerGameObjectTreeItem> newGOTreeItemDic = new Dictionary<int, ControllerGameObjectTreeItem>();

            newGOTreeItemDic.Add(UIExpansionManager.Instance.CurUIExpansion.gameObject.GetInstanceID(), newTreeRoot);
            foreach (GearTypeState gearType in Enum.GetValues(typeof(GearTypeState)))
            {
                if (ControllerGameObjectTreeItem.CheckCanAddGear(newTreeRoot.Target, gearType) && !ControllerRootTreeItem.IgnoreGears.Contains(gearType))
                {
                    ControllerGameObjectTreeItem gameObjectTreeItem = null;
                    if (_goTreeItemDic.TryGetValue(newTreeRoot.Target.GetInstanceID(), out gameObjectTreeItem))
                    {
                        ControllerGearTreeItemBase oldGearTreeItem = gameObjectTreeItem.GetTargetGear(gearType);
                        if (oldGearTreeItem != null)
                        {
                            newTreeRoot.AddChild(oldGearTreeItem);
                            continue;
                        }
                    }
                    string gearClassName = "ND.UI." + gearType.ToString() + "GearTreeItem";
                    Type gearClassType = System.Type.GetType(gearClassName);
                    ControllerGearTreeItemBase gearTreeItem = Activator.CreateInstance(gearClassType, true) as ControllerGearTreeItemBase;
                    newTreeRoot.AddChild(gearTreeItem);
                }
            }

            foreach (Transform child in UIExpansionManager.Instance.CurUIExpansion.gameObject.transform)
            {
                AddChildToTree(child, newTreeRoot, newGOTreeItemDic, true);
            }

            _treeRoot = newTreeRoot;
            _goTreeItemDic = newGOTreeItemDic;
        }

        public void RebuildShowTreeList()
        {
            _showTreeList = new List<ControllerTreeItemBase>();
            _showTreeHeight = _treeRoot.RebuildShowTreeList(_showTreeList);
        }

        public void RebuildAddGearTreeList()
        {
            _addGearTreeList = new List<ControllerTreeItemBase>();
            _addGearTreeHeight = _treeRoot.RebuildAddGearTreeList(_addGearTreeList) + 40; //40 的作用是将列表的总高度撑开。以便能再滚动区域内完整显示
        }

        public List<GearConfig> RebuildGearConfigList(UIExpansionStoredDataBuilder dataBuilder)
        {
            List<GearConfig> gearConfigList = new List<GearConfig>();
            _treeRoot.RebuildGearConfigList(gearConfigList, dataBuilder);
            return gearConfigList;
        }

        public ControllerGearTreeItemBase GetTargetGear(int instanceID, GearTypeState gearType, string name = null)
        {
            if (_goTreeItemDic == null)
            {
                return null;
            }
            ControllerGameObjectTreeItem gameObjectTreeItem = null;
            if (!_goTreeItemDic.TryGetValue(instanceID, out gameObjectTreeItem))
            {
                return null;
            }
            return gameObjectTreeItem.GetTargetGear(gearType, name);
        }

        public void ChangeCurControllerName(string newName)
        {
            _name = newName;
        }

        public void ChangePageName(int index, string newPageName)
        {
            _pageNameList[index] = newPageName;
        }

        public void AddNewPage(string pageName = null)
        {
            if (string.IsNullOrEmpty(pageName))
            {
                pageName = "p" + PageNameList.Count;
            }
            _pageNameList.Add(pageName);
            _pageTipsList.Add("");
            _treeRoot.OnAddNewPage();
        }

        public void RemovePage(int index)
        {
            _pageNameList.RemoveAt(index);
            _pageTipsList.RemoveAt(index);
            _treeRoot.OnRemovePage(index);
            if (index == _selectedIndex)
            {
                _selectedIndex = Mathf.Min(_pageNameList.Count - 1, _selectedIndex);
            }
            else if (index < _selectedIndex)
            {
                _selectedIndex -= 1;
            }
            _selectedIndex = Math.Max(0, _selectedIndex);
        }

        public void RefreshMonitorValue()
        {
            _treeRoot.RefreshMonitorValue();
        }

        public void MonitorGameObjectChange(int instanceID)
        {
            ControllerGameObjectTreeItem gameObjectTreeItem = null;
            if(_goTreeItemDic.TryGetValue(instanceID, out gameObjectTreeItem))
            {
                gameObjectTreeItem.MonitorGearChange();
            }
        }

        public void ChangeSelectedIndex(int index)
        {
            if (index == _selectedIndex)
            {
                return;
            }
            bool inRecordingState = UIExpansionManager.Instance.ControllerSettings.InRecordingState;
            UIExpansionManager.Instance.ControllerSettings.InRecordingState = false;
            _selectedIndex = index;
            if (UIExpansionManager.Instance.ControllerSettings.AutoApply|| inRecordingState)
            {
                Apply();
            }
            if (inRecordingState)
            {
                UIExpansionManager.Instance.ControllerSettings.InRecordingState = true;
            }
        }

        public void ApplyEditData(List<ControllerWrapperEditAction> editActionList, string wrapperName, List<string> pageNameList, List<string> pageTipsList)
        {
            foreach (var actionItem in editActionList)
            {
                if (actionItem.m_editActionState == ControllerWrapperEditAction.EditActionStateType.Remove)
                {
                    RemovePage(actionItem.m_editPageIndex);
                }
                else if (actionItem.m_editActionState == ControllerWrapperEditAction.EditActionStateType.Add)
                {
                    AddNewPage();
                }
            }
            Name = wrapperName;
            for(int i = 0;i< _pageNameList.Count; i++)
            {
                _pageNameList[i] = pageNameList[i];
                _pageTipsList[i] = pageTipsList[i];
            }
        }

        public void Apply()
        {
            _treeRoot.Apply();
            EditorUtility.SetDirty(UIExpansionManager.Instance.CurUIExpansion.gameObject);
        }
    }
}