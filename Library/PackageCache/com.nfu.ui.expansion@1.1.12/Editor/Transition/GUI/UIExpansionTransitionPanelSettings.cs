using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ND.UI
{
    public class UIExpansionTransitionPanelSettings
    {
        private int _curTransitionIndex;

        private int _fps = 30;

        private int _frameWidth = 30;

        private int _needDrawTime = 1;

        private float _panelOffsetX;

        private float _panelOffsetY;

        private int _curDealFrameIndex;

        private bool _inCreateNewWrapperState;

        private bool _inChangeWrapperNameState;

        private bool _inPlayingMode;

        private bool _inPauseState;

        private float _curPlayIndex = 0;

        private bool _inPreviewMode;

        private bool _inAddNewLineState;

        private bool _inRecordingState;

        private TransitionTimelineItemBase _curOperableTimelineItem;

        private string _tempAddEventName;

        public int FPS { get => _fps; set => _fps = value; }
        public int FrameWidth { get => _frameWidth; set => _frameWidth = value; }
        public int NeedDrawTime { get => _needDrawTime; set => _needDrawTime = value; }
        public int CurTransitionIndex { get => _curTransitionIndex; set => _curTransitionIndex = value; }
        public float PanelOffsetX { get => _panelOffsetX; set => _panelOffsetX = value; }
        public float PanelOffsetY { get => _panelOffsetY; set => _panelOffsetY = value; }
        public int CurDealFrameIndex
        {
            get => _curDealFrameIndex;
            set
            {
                _curDealFrameIndex = value;
                if (_inPreviewMode)
                {
                    UIExpansionManager.Instance.CurTransitionWrapper.Apply(_curDealFrameIndex);
                }
            }
        }
        public bool InPlayingMode { get => _inPlayingMode; set => _inPlayingMode = value; }
        public bool InPreviewMode
        {
            get => _inPreviewMode;
            set
            {
                if (_inPreviewMode != value)
                {
                    UIExpansionManager.Instance.CurTransitionWrapper.OnPreviewStateChange(value);
                }
                _inPreviewMode = value;
            }
        }
        public bool InPauseState { get => _inPauseState; set => _inPauseState = value; }
        public float CurPlayIndex { get => _curPlayIndex; set => _curPlayIndex = value; }
        public TransitionTimelineItemBase CurOperableTimelineItem { get => _curOperableTimelineItem; set => _curOperableTimelineItem = value; }
        public bool InAddNewLineState { get => _inAddNewLineState; set => _inAddNewLineState = value; }
        public bool InRecordingState
        {
            get => _inRecordingState;
            set
            {
                if(value == _inRecordingState)
                {
                    return;
                }
                if (value)
                {
                    if (_inPlayingMode)
                    {
                        StopPlay();
                    }
                    UIExpansionManager.Instance.CurTransitionWrapper.RefreshMonitorValue();
                }
                _inRecordingState = value;
            }
        }

        public string TempAddEventName { get => _tempAddEventName; set => _tempAddEventName = value; }
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
        public bool InChangeWrapperNameState { 
            get => _inChangeWrapperNameState;
            set
            {
                if (_inChangeWrapperNameState == value)
                {
                    return;
                }
                if (value)
                {
                    ExitAllState();
                }
                _inChangeWrapperNameState = value;
            }
        }

        public void MoveToLeftRrame()
        {
            InPreviewMode = true;
            int targetFrame = _curDealFrameIndex - 1;
            if (_inPlayingMode)
            {
                targetFrame = Mathf.FloorToInt(_curPlayIndex);
                StopPlay();
            }
            targetFrame = Mathf.Clamp(targetFrame, 0, _fps * _needDrawTime);
            CurDealFrameIndex = targetFrame;
        }

        public void MoveToRightFrame()
        {
            InPreviewMode = true;
            int targetFrame = _curDealFrameIndex + 1;
            if (_inPlayingMode)
            {
                targetFrame = Mathf.CeilToInt(_curPlayIndex);
                StopPlay();
            }
            targetFrame = Mathf.Clamp(targetFrame, 0, _fps * _needDrawTime);
            CurDealFrameIndex = targetFrame;
        }

        public void OnPlayStateChange()
        {
            InPreviewMode = true;

            if (_inPlayingMode)
            {
                InPauseState = !InPauseState;
            }
            else
            {
                InPauseState = false;
                _curPlayIndex = 0;
                InPlayingMode = true;
               UIExpansionManager.Instance.CurTransitionWrapper.Play();
            }
            UIExpansionManager.Instance.NeedRepaint = true;
        }

        public void StopPlay(bool exitPreview = false)
        {
            if (_inPlayingMode)
            {
                UIExpansionManager.Instance.CurTransitionWrapper.Stop();
                UIExpansionManager.Instance.NeedRepaint = true;
                _inPlayingMode = false;
                _curPlayIndex = 0;
                if (exitPreview)
                {
                    InPreviewMode = false;
                    CurDealFrameIndex = 0;
                }
            }
            else
            {
                InPreviewMode = false;
            }
        }

        public void OnUpdate(float deltaTime)
        {
            if (_inPlayingMode && (_inPauseState == false))
            {
                _curPlayIndex += deltaTime; 
                UIExpansionManager.Instance.NeedRepaint = true;
            }
        }

        public void Init()
        {
            _curTransitionIndex = UIExpansionManager.Instance.CurUIExpansionWrapper.TransitionWrapperList.Count > 0 ? 0 : -1;
        }

        public void Refresh()
        {

        }

        public void ExitAllState()
        {
            InCreateNewWrapperState = false;
            InChangeWrapperNameState = false;
            InPlayingMode = false;
            InPauseState = false;
            InPreviewMode = false;
            InAddNewLineState = false;
            InRecordingState = false;
        }
    }
}