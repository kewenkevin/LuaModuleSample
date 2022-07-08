using System.Collections;
using System.Collections.Generic;
using ND.UI;
using UnityEngine;
using UnityEditor;


namespace ND.UI
{
    public class UIExpansionWindowSettings
    {
        private UIExpansionPanelBase[] _panels;

        private int _curPanelIndex = 0;

        private bool _autoSave = false;

        private float _scrollWheelSteps = 4;

        private UIExpansionControllerPanelSettings _controllerSettings;

        private UIExpansionTransitionPanelSettings _transitionSettings;

        private UIExpansionBindingPanelSettings _bindingSettings;

        public int CurPanelIndex
        {
            get
            {
                return _curPanelIndex;
            }

            set
            {
                if (_curPanelIndex == value)
                {
                    return;
                }
                _curPanelIndex = value;
                EditorPrefs.SetInt("UIExpansionWindowSettings.CurPanelIndex", _curPanelIndex);
                UIExpansionManager.Instance.NeedRepaint = true;
            }
        }

        public UIExpansionPanelBase[] Panels
        {
            get
            {
                return _panels;
            }

            set
            {
                _panels = value;
            }
        }

        public UIExpansionControllerPanelSettings ControllerSettings
        {
            get
            {
                return _controllerSettings;
            }

            set
            {
                _controllerSettings = value;
            }
        }

        public UIExpansionBindingPanelSettings BindingSettings
        {
            get
            {
                return _bindingSettings;
            }

            set
            {
                _bindingSettings = value;
            }
        }

        public bool AutoSave { get => _autoSave; set => _autoSave = value; }

        public float MouseScrollWheelSteps
        {
            get => _scrollWheelSteps;
            set => _scrollWheelSteps = value;
        }

        public UIExpansionTransitionPanelSettings TransitionSettings { get => _transitionSettings; set => _transitionSettings = value; }

        public UIExpansionWindowSettings()
        {
            _panels = new UIExpansionPanelBase[]
            {
                new UIExpansionControllerPanel(null),
                new UIExpansionTransitionPanel(null),
                //new UIExpansionBindingPanel(null),    // 双向绑定先屏蔽掉
            };
            //if (EditorPrefs.HasKey("UIExpansionWindowSettings.CurPanelIndex"))
            //{
            //    _curPanelIndex = EditorPrefs.GetInt("UIExpansionWindowSettings.CurPanelIndex");
            //}
            //else
            {
                _curPanelIndex = 0;
            }
            _controllerSettings = new UIExpansionControllerPanelSettings();
            _transitionSettings = new UIExpansionTransitionPanelSettings();
            _bindingSettings = new UIExpansionBindingPanelSettings();
            Init();
            _panels[_curPanelIndex].OnEnter();
        }

        public void Init()
        {
            _controllerSettings.Init();
            _transitionSettings.Init();
        }

        public void Refresh()
        {
            _controllerSettings.Refresh();
            _transitionSettings.Refresh();
        }
    }
}