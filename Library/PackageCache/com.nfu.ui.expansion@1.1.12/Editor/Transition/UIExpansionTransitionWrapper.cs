using System;
using System.Collections.Generic;
using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;

namespace ND.UI
{
    public class UIExpansionTransitionWrapper
    {
        private int _totalFrameNum;

        private string _name;

        private bool _autoPlay;

        private int _autoPlayTimes;

        private float _autoPlayDelay;

        private TransitionRootTreeItem _treeRoot;

        private EventLineTreeItem _eventLine;

        private Dictionary<int, TransitionGameObjectTreeItem> _goTreeItemDic;

        private List<TransitionTreeItemBase> _showTreeList;

        private float _showTreeHeight;

        private List<TransitionTreeItemBase> _addLineTreeList;

        private float _addLineTreeHeight;

        public UIExpansionTransitionWrapper(string name)
        {
            _name = name;
            _eventLine = new EventLineTreeItem();
            RebuildTree();
            RebuildShowTreeList();
        }

        public void AnalyzeTransitionConfig(TransitionConfig config)
        {
            _name = config.name;
            _autoPlay = config.autoPlay;
            if (_autoPlay)
            {
                _autoPlayTimes = config.autoPlayTimes;
                _autoPlayDelay = config.autoPlayDelay;
            }
            if (config.keyFrameConfigs == null || config.keyFrameConfigs.Length == 0)
            {
                return;
            }
            for (int i = 0; i < config.keyFrameConfigs.Length; i++)
            {
                KeyFrameConfig keyFrameConfig = config.keyFrameConfigs[i];
                if (config.keyFrameConfigs[i].lineType == LineTypeState.Event)
                {
                    _eventLine.AddFrameByConfig(keyFrameConfig);
                }
                else
                {
                    GameObject target = UIExpansionManager.Instance.CurUIExpansion.GetStoredGameObject(keyFrameConfig.StoredGameObjectIndex);
                    if (target == null)
                    {
                        continue;
                    }
                    TransitionGameObjectTreeItem goTreeItem = null;
                    if (!_goTreeItemDic.TryGetValue(target.GetInstanceID(), out goTreeItem))
                    {
                        continue;
                    }
                    TransitionLineTreeItemBase lineTreeItem = null;
                    switch (keyFrameConfig.lineType)
                    {
                        default:
                            lineTreeItem = goTreeItem.GetLineTreeItem(keyFrameConfig.lineType);
                            break;
                    }
                    if (lineTreeItem == null)
                    {
                        continue;
                    }
                    lineTreeItem.AddFrameByConfig(keyFrameConfig);
                }
            }
        }

        public UIExpansionTransitionWrapper(TransitionConfig config)
        {
            _eventLine = new EventLineTreeItem();
            RebuildTree();
            AnalyzeTransitionConfig(config);
            RefreshTotalFrameNum();
            RebuildShowTreeList();
        }

        public int TotalFrameNum { get => _totalFrameNum; set => _totalFrameNum = value; }
        public string Name { get => _name; set => _name = value; }
        public bool AutoPlay { get => _autoPlay; set => _autoPlay = value; }
        public int AutoPlayTimes { get => _autoPlayTimes; set => _autoPlayTimes = value; }
        public float AutoPlayDelay { get => _autoPlayDelay; set => _autoPlayDelay = value; }
        public List<TransitionTreeItemBase> ShowTreeList { get => _showTreeList; set => _showTreeList = value; }
        public float ShowTreeHeight { get => _showTreeHeight; set => _showTreeHeight = value; }
        public List<TransitionTreeItemBase> AddLineTreeList { get => _addLineTreeList; set => _addLineTreeList = value; }
        public float AddLineTreeHeight { get => _addLineTreeHeight; set => _addLineTreeHeight = value; }
        public EventLineTreeItem EventLineTreeItem { get => _eventLine; set => _eventLine = value; }

        public void RebuildTree()
        {
            if (UIExpansionManager.Instance.CurUIExpansion.gameObject == null)
            {
                return;
            }

            TransitionRootTreeItem newTreeRoot = new TransitionRootTreeItem(UIExpansionManager.Instance.CurUIExpansion.gameObject);// todo...
            Dictionary<int, TransitionGameObjectTreeItem> newGOTreeItemDic = new Dictionary<int, TransitionGameObjectTreeItem>();

            newGOTreeItemDic.Add(UIExpansionManager.Instance.CurUIExpansion.gameObject.GetInstanceID(), newTreeRoot);
            // newTreeRoot.AddGearItems();

            foreach (Transform child in UIExpansionManager.Instance.CurUIExpansion.gameObject.transform)
            {
                AddChildToTree(child, newTreeRoot, newGOTreeItemDic, false);
            }
            _treeRoot = newTreeRoot;
            _goTreeItemDic = newGOTreeItemDic;
        }

        public void AddChildToTree(Transform target, TransitionTreeItemBase parent, Dictionary<int, TransitionGameObjectTreeItem> goTreeItemDic, bool refresh)
        {
            TransitionGameObjectTreeItem gameObjectTreeItem = new TransitionGameObjectTreeItem(target.gameObject);
            parent.AddChild(gameObjectTreeItem);
            goTreeItemDic.Add(target.gameObject.GetInstanceID(), gameObjectTreeItem);
            foreach (LineTypeState lineType in Enum.GetValues(typeof(LineTypeState)))
            {
                if (TransitionGameObjectTreeItem.CheckCanAddLine(target.gameObject, lineType))
                {
                    switch (lineType)
                    {
                        /*
                        case GearTypeState.Controller:
                            for (int i = 0; i < target.gameObject.GetComponent<UIExpansion>().ControllerConfigs.Length; i++)
                            {
                                if (refresh)
                                {
                                    TransitionGameObjectTreeItem oldGameObjectTreeItem = null;
                                    if (_goTreeItemDic.TryGetValue(target.gameObject.GetInstanceID(), out oldGameObjectTreeItem))
                                    {
                                        ControllerGearTreeItemBase oldGearTreeItem = oldGameObjectTreeItem.GetTargetGear(lineType, target.gameObject.GetComponent<UIExpansion>().ControllerConfigs[i].name);
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
                            */
                        default:
                            if (refresh)
                            {
                                /*
                                TransitionGameObjectTreeItem oldGameObjectTreeItem = null;
                                if (_goTreeItemDic.TryGetValue(target.gameObject.GetInstanceID(), out oldGameObjectTreeItem))
                                {
                                    ControllerGearTreeItemBase oldGearTreeItem = oldGameObjectTreeItem.GetTargetGear(lineType);
                                    if (oldGearTreeItem != null)
                                    {
                                        gameObjectTreeItem.AddChild(oldGearTreeItem);
                                        continue;
                                    }
                                }
                                */
                            }
                            string lineClassName = "ND.UI." + lineType.ToString() + "LineTreeItem";
                            Type lineClassType = Type.GetType(lineClassName);
                            TransitionLineTreeItemBase lineTreeItem = Activator.CreateInstance(lineClassType, true) as TransitionLineTreeItemBase;
                            gameObjectTreeItem.AddChild(lineTreeItem);
                            break;
                    }
                }
            }

            foreach (Transform child in target)
            {
                AddChildToTree(child, gameObjectTreeItem, goTreeItemDic, refresh);
            }
        }

        public void RebuildShowTreeList()
        {
            _showTreeList = new List<TransitionTreeItemBase>();
            _showTreeHeight = _treeRoot.RebuildShowTreeList(_showTreeList);
        }

        public void RebuildAddGearTreeList()
        {
            _addLineTreeList = new List<TransitionTreeItemBase>();
            _addLineTreeHeight = _treeRoot.RebuildAddLineTreeList(_addLineTreeList);
        }

        public List<KeyFrameConfig> RebuildKeyFrameConfigList(UIExpansionStoredDataBuilder dataBuilder)
        {
            List<KeyFrameConfig> keyFrameConfigList = new List<KeyFrameConfig>();
            _eventLine.RebuildKeyFrameConfigList(keyFrameConfigList, dataBuilder);
            _treeRoot.RebuildKeyFrameConfigList(keyFrameConfigList, dataBuilder);
            return keyFrameConfigList;
        }

        public void RefreshTotalFrameNum()
        {
            int tempTotalFrameNum = 0;
            List<TransitionTreeItemBase> lineTreeItemList = new List<TransitionTreeItemBase>();
            _treeRoot.GetTypeLineTreeItems(TransitionTreeItemType.Line, lineTreeItemList);
            foreach (TransitionTreeItemBase lineTreeItem in lineTreeItemList)
            {
                foreach (var frame in (lineTreeItem as TransitionLineTreeItemBase).FrameList)
                {
                    if (frame.FrameIndex > tempTotalFrameNum)
                    {
                        tempTotalFrameNum = frame.FrameIndex;
                    }
                }
            }
            if (tempTotalFrameNum != _totalFrameNum)
            {
                _totalFrameNum = tempTotalFrameNum;
            }
            UIExpansionManager.Instance.NeedRepaint = true;
        }

        public void MonitorGameObjectChange(int instanceID)
        {
            TransitionGameObjectTreeItem gameObjectTreeItem = null;
            if (_goTreeItemDic.TryGetValue(instanceID, out gameObjectTreeItem))
            {
                gameObjectTreeItem.MonitorLineChange();
            }
        }

        public void RefreshMonitorValue()
        {
            _treeRoot.RefreshMonitorValue();
        }

        private bool _playing = false;

        private bool _paused = false;

        private int _totalTasks = 0;

        public void Play()
        {
            Stop();
            StartUp();
        }

        public void SetPauseState(bool pauseState)
        {

        }

        private void StartUp()
        {
            BeforeStartUp();
            _paused = false;
            _playing = _totalTasks > 0;
        }

        private void BeforeStartUp()
        {
            _totalTasks = _treeRoot.Play();
        }

        public void OnItemTweenComplete()
        {
            _totalTasks--;
            CheckAllComplete();
        }

        private void CheckAllComplete()
        {
            if (_totalTasks == 0)
            {
                _playing = false;
                UIExpansionManager.Instance.TransitionSettings.StopPlay();
            }
        }

        public void Stop()
        {
            _treeRoot.Stop();
        }

        public void Apply(int frameIndex)
        {
            _treeRoot.Apply(frameIndex);
        }

        public void OnPreviewStateChange(bool state)
        {
            _treeRoot.OnPreviewStateChange(state);
        }
    }
}