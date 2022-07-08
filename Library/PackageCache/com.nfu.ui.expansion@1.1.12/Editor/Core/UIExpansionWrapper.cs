using System.Collections.Generic;
using ND.UI.Core.Model;
using UnityEditor;


namespace ND.UI
{
    public class UIExpansionWrapper 
    {
        private List<UIExpansionControllerWrapper> _controllerWrapperList = new List<UIExpansionControllerWrapper>();

        private List<UIExpansionTransitionWrapper> _transitionWrapperList = new List<UIExpansionTransitionWrapper>();

        private UIExpansionBindingWrapper _bindingWrapper;

        public List<UIExpansionControllerWrapper> ControllerWrapperList
        {
            get
            {
                return _controllerWrapperList;
            }

            set
            {
                _controllerWrapperList = value;
            }
        }

        public List<UIExpansionTransitionWrapper> TransitionWrapperList { get => _transitionWrapperList; set => _transitionWrapperList = value; }
      
        public UIExpansionBindingWrapper BindingWrapper
        {
            get
            {
                return _bindingWrapper;
            }

            set
            {
                _bindingWrapper = value;
            }
        }
        public UIExpansionWrapper(UIExpansion uiExpansion)
        {
            Init(uiExpansion);
        }

        public void Refresh()
        {
            for(int i = 0;i < _controllerWrapperList.Count;i++)
            {
                _controllerWrapperList[i].Refresh();
            }
            _bindingWrapper.Refresh();
        }

        private void Init(UIExpansion uiExpansion)
        {
            _controllerWrapperList = new List<UIExpansionControllerWrapper>();
            if (uiExpansion.ControllerConfigs != null && uiExpansion.ControllerConfigs.Length > 0)
            {
                for (int i = 0; i < uiExpansion.ControllerConfigs.Length; i++)
                {
                    _controllerWrapperList.Add(new UIExpansionControllerWrapper(uiExpansion.ControllerConfigs[i]));
                }
            }
            
            _transitionWrapperList = new List<UIExpansionTransitionWrapper>();
            if (uiExpansion.TransitionConfigs != null && uiExpansion.TransitionConfigs.Length > 0)
            {
                for (int i = 0; i < uiExpansion.TransitionConfigs.Length; i++)
                {
                    _transitionWrapperList.Add(new UIExpansionTransitionWrapper(uiExpansion.TransitionConfigs[i]));
                }
            }

            _bindingWrapper = new UIExpansionBindingWrapper(uiExpansion.BindingConfig);
        }

        public bool CheckCanSave(bool needCheckAllLabel ,out string failedStr)
        {
            failedStr = "";
            if (needCheckAllLabel)
            {
                UIExpansionManager.Instance.CurBindingWrapper.CheckAllLabel();
            }
            if (UIExpansionManager.Instance.CurBindingWrapper.IllegalLabelTreeItemList.Count > 0 
                || UIExpansionManager.Instance.CurBindingWrapper.RepeatLabelNum > 0 
                || UIExpansionManager.Instance.CurBindingWrapper.IllegalMethodLabelTreeItemList.Count > 0)
            {
                if (UIExpansionManager.Instance.WindowSettings.AutoSave)
                {
                    UIExpansionManager.Instance.WindowSettings.AutoSave = false;
                }
                if(UIExpansionManager.Instance.CurBindingWrapper.IllegalLabelTreeItemList.Count > 0)
                {
                    failedStr = "Binding Panel Label 含有非法字符。";
                }
                if(UIExpansionManager.Instance.CurBindingWrapper.IllegalMethodLabelTreeItemList.Count > 0)
                {
                    failedStr = "Binding Panel 有Methods被多次綁定了。";
                }
                if(UIExpansionManager.Instance.CurBindingWrapper.RepeatLabelNum > 0)
                {
                    failedStr = "Binding Panel 有重复的Label。";
                }
                return false;
            }
            return true;
        }

        public void Save()
        {
            if (UIExpansionManager.Instance.CurUIExpansion == null)
            {
                return;
            }
            UIExpansionManager.Instance.CurUIExpansion.ClearStoredDatas();
            UIExpansionStoredDataBuilder dataBuilder = new UIExpansionStoredDataBuilder();
            // 保存控制器信息
            if (_controllerWrapperList.Count > 0)
            {
                UIExpansionManager.Instance.CurUIExpansion.ControllerConfigs = new ControllerConfig[_controllerWrapperList.Count];
                for (int i = 0; i < _controllerWrapperList.Count; i++)
                {
                    UIExpansionManager.Instance.CurUIExpansion.ControllerConfigs[i] = BuildControllerConfig(i, dataBuilder);
                }
            }
            // 保存绑定信息
            UIExpansionManager.Instance.CurUIExpansion.BindingConfig = BuildBindingConfig(dataBuilder);
            // 保存动效信息
            if (_transitionWrapperList.Count > 0)
            {
                UIExpansionManager.Instance.CurUIExpansion.TransitionConfigs = new TransitionConfig[_transitionWrapperList.Count];
                for (int i = 0; i < _transitionWrapperList.Count; i++)
                {
                    UIExpansionManager.Instance.CurUIExpansion.TransitionConfigs[i] = BuildTransitionConfig(i, dataBuilder);
                }
            }
            //将处理好的临时数据存储保存回CurUIExpansion上 p.s ref.CurUIExpansion.gameObjects = data.gameObjects;
            dataBuilder.Store(UIExpansionManager.Instance.CurUIExpansion);
            //通知Unity将数据持久化保存下来
            EditorUtility.SetDirty(UIExpansionManager.Instance.CurUIExpansion);
        }

        private ControllerConfig BuildControllerConfig(int wrapperIndex, UIExpansionStoredDataBuilder dataBuilder)
        {
            UIExpansionControllerWrapper wrapper = _controllerWrapperList[wrapperIndex];
            ControllerConfig config = new ControllerConfig();
            config.name = wrapper.Name;
            config.pageNames = wrapper.PageNameList.ToArray();
            //标签备注
            config.pageTips = wrapper.PageTipsList.ToArray();
            config.selectedIndex = wrapper.SelectedIndex;
            config.gearConfigs = wrapper.RebuildGearConfigList(dataBuilder).ToArray();
            config.SortGears();
            return config;
        }

        public TransitionConfig BuildTransitionConfig(int wapperIndex, UIExpansionStoredDataBuilder dataBuilder)
        {
            UIExpansionTransitionWrapper wrapper = _transitionWrapperList[wapperIndex];
            TransitionConfig config = new TransitionConfig();
            config.name = wrapper.Name;
            config.autoPlay = wrapper.AutoPlay;
            if (config.autoPlay)
            {
                config.autoPlayDelay = wrapper.AutoPlayDelay;
                config.autoPlayTimes = wrapper.AutoPlayTimes;
            }
            config.keyFrameConfigs = wrapper.RebuildKeyFrameConfigList(dataBuilder).ToArray();
            return config;
        }

        private BindingConfig BuildBindingConfig(UIExpansionStoredDataBuilder dataBuilder)
        {
            BindingConfig config = new BindingConfig();
            config.linkerConfigs = _bindingWrapper.RebuildLinkerConfigList(dataBuilder).ToArray();
            return config;
        }
    }
}