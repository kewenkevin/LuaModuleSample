using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ND.UI
{
    public class UIExpansionControllerPanelSettings
    {
        private int _curControllerIndex;

        private bool _inCreateNewWrapperState = false;

        private bool _inEditState = false;

        private bool _inRecordingState = false;

        private bool _inAddNewGearState = false;

        private float _panelOffsetX;

        private float _panelOffsetY;

        private int _curChangeNamePageIndex = -1;

        private bool _autoApply;

        public int CurControllerIndex
        {
            get
            {
                return _curControllerIndex;
            }

            set
            {
                _curControllerIndex = value;
            }
        }

        public bool InEditState
        {
            get
            {
                return _inEditState;
            }

            set
            {
                if (_inEditState == value)
                {
                    return;
                }
                if (value)
                {
                    ExitAllState();
                }
                _inEditState = value;
            }
        }

        public bool InRecordingState
        {
            get
            {
                return _inRecordingState;
            }

            set
            {
                if (value == _inRecordingState)
                {
                    return;
                }
                
                if (value)
                {
                    ExitAllState();
                    UIExpansionManager.Instance.CurControllerWrapper.Apply();
                    UIExpansionManager.Instance.CurControllerWrapper.RefreshMonitorValue();
                }
                _inRecordingState = value;
            }
        }

        public float PanelOffsetX
        {
            get
            {
                return _panelOffsetX;
            }

            set
            {
                _panelOffsetX = value;
            }
        }

        public float PanelOffsetY
        {
            get
            {
                return _panelOffsetY;
            }

            set
            {
                _panelOffsetY = value;
            }
        }

        public int CurChangeNamePageIndex
        {
            get
            {
                return _curChangeNamePageIndex;
            }

            set
            {
                _curChangeNamePageIndex = value;
            }
        }

        public bool InAddNewGearState
        {
            get
            {
                return _inAddNewGearState;
            }

            set
            {
                if(_inAddNewGearState == value)
                {
                    return;
                }
                if (value)
                {
                    ExitAllState();
                }
                _inAddNewGearState = value;
            }
        }

        public bool AutoApply
        {
            get
            {
                return _autoApply;
            }

            set
            {
                if(_autoApply == value)
                {
                    return;
                }
                if (value)
                {
                    ExitAllState();
                }
                _autoApply = value;
            }
        }

        public bool InCreateNewWrapperState
        {
            get => _inCreateNewWrapperState;
            set
            {
                if (_inCreateNewWrapperState == value)
                {
                    return;
                }
                if (value)
                {
                    ExitAllState();
                }
                _inCreateNewWrapperState = value;
            }
        }

        public void Init()
        {
            _curControllerIndex = UIExpansionManager.Instance.CurUIExpansionWrapper.ControllerWrapperList.Count > 0 ? 0 : -1;
            _inEditState = false;
        }

        public void ChangeCurControllerWrapper(int wrapperIndex)
        {
            if(wrapperIndex == _curControllerIndex)
            {
                return;
            }
            UIExpansionManager.Instance.ControllerSettings.Refresh();
            _curControllerIndex = wrapperIndex;
        }

        public void ExitAllState()
        {
            InEditState = false;
            InRecordingState = false;
            InAddNewGearState = false;
            AutoApply = false;
        }

        public void Refresh()
        {
            InEditState = false;
            InRecordingState = false;
            PanelOffsetX = 0;
            PanelOffsetY = 0;
            CurChangeNamePageIndex = -1;
            InAddNewGearState = false;
            AutoApply = false;
        }
    }
}