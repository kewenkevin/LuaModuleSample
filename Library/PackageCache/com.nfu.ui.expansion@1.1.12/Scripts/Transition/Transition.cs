using System.Collections.Generic;
using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;
using UnityEngine.Events;

namespace ND.UI
{
    public class Transition : ITransition
    {
        private string _name;

        private UIExpansion _owner;

        private bool _autoPlay;

        private int _autoPlayTimes;

        private float _autoPlayDelay;

        private List<KeyFrameBase> _frameList;

        private List<TransitionActionBase> _actionList;

        private bool _playing;

        private bool _paused;

        private bool _reversed;

        private int _totalTaskNum = 0;

        private int _playTimes = 0;

        private float _length;

        private Tweener _delayTweener;

        private UnityEngine.Events.UnityAction<string> _onStringEvent;

        private UnityEngine.Events.UnityAction _onCompleteEvent;

        public UIExpansion Owner { get => _owner; set => _owner = value; }
        public string Name { get => _name;  }
        public float Length { get => _length; }
        public bool Paused { get => _paused; }
        public UnityAction<string> OnStringEvent { get => _onStringEvent; }

        public Transition(UIExpansion owner, TransitionConfig config)
        {
            _owner = owner;
            _name = config.name;
            _autoPlay = config.autoPlay;
            _autoPlayTimes = config.autoPlayTimes;
            _autoPlayDelay = config.autoPlayDelay;
            _frameList = new List<KeyFrameBase>(config.keyFrameConfigs.Length);
            _actionList = new List<TransitionActionBase>();
            EaseType tweenEaseType = EaseType.None;
            for (int i = 0; i < config.keyFrameConfigs.Length; i++)
            {
                KeyFrameBase keyframe = UIExpansionUtility.GetKeyFrame(this, config.keyFrameConfigs[i]);
                if(keyframe.FrameTime> _length)
                {
                    _length = keyframe.FrameTime;
                }
                _frameList.Add(keyframe);
                if (tweenEaseType != EaseType.None)
                {
                    _actionList.Add(new TransitionTweenAction(_frameList[i - 1], _frameList[i], tweenEaseType));
                    // continue;
                }

                //适配1.0.17(含)之前的动效 老版本动效没有动画曲线 导致解析错误
                if (owner.StoredAnimationCurves == null || owner.StoredAnimationCurves.Length<=0)
                {
                    tweenEaseType = (EaseType)owner.StoredInts[config.keyFrameConfigs[i].dataList[config.keyFrameConfigs[i].dataList.Length - 1]];
                }
                else
                {
                    switch (config.keyFrameConfigs[i].lineType)
                    {
                        //如果是支持自定义动画曲线的动效轴，则反序列化时easeType存放位不为最后一项
                        case LineTypeState.Position:
                        case LineTypeState.Rotation:
                        case LineTypeState.Scale:
                        case LineTypeState.SizeDelta:
                        case LineTypeState.ImageColor:
                        case LineTypeState.TextColor:
                            tweenEaseType = (EaseType)owner.StoredInts[config.keyFrameConfigs[i].dataList[config.keyFrameConfigs[i].dataList.Length - 2]];
                            break;
                        //其他不支持自定义动画曲线的动效轴，，则反序列化时easeType存放位为最后一项
                        default:
                            tweenEaseType = (EaseType)owner.StoredInts[config.keyFrameConfigs[i].dataList[config.keyFrameConfigs[i].dataList.Length - 1]];
                            break;
                    }
                }
                
                // tweenEaseType = (EaseType)owner.StoredInts[config.keyFrameConfigs[i].dataList[config.keyFrameConfigs[i].dataList.Length - 1]];
                if (tweenEaseType == EaseType.None)
                {
                    _actionList.Add(new TransitionKeyFrameAction(_frameList[i]));
                }
            }
        }

        public void CheckAutoPlay()
        {
            if (_autoPlay)
            {
                Play(_autoPlayTimes, _autoPlayDelay);
            }
        }

        public void Play(int times = 1, float delay = 0, UnityEngine.Events.UnityAction onComplete = null,  bool reverse = false)
        {
            Stop(true, true);
            _playTimes = times;
            _reversed = reverse;
            _playing = true;
            _paused = false;
            _onCompleteEvent = onComplete;
            delay = Mathf.Max(delay, 0);
            if (delay == 0)
            {
                StartUp();
            }
            else
            {
                _delayTweener = TweenManager.Instance.CreateTweener().SetDelay(delay).OnComplete(StartUp);
            }
        }
        
        // public void EditorPlay(int times = 1, float delay = 0, UnityEngine.Events.UnityAction onComplete = null,
        //     bool reverse = false)
        // {
        //     Stop(true, true);
        //     _playTimes = times;
        //     _reversed = reverse;
        //     _playing = true;
        //     _paused = false;
        //     _onCompleteEvent = onComplete;
        //     delay = Mathf.Max(delay, 0);
        // }

        public void AddStringEvent(UnityEngine.Events.UnityAction<string> onString)
        {
            _onStringEvent = onString;
        }

        public void SetPaused(bool paused)
        {
            if (_playing == false || _paused == paused)
            {
                return;
            }
            _paused = paused;
            if (_delayTweener != null)
            {
                _delayTweener.Paused = paused;
                return;
            }
            for (int i = 0; i < _actionList.Count; i++)
            {
                _actionList[i].SetPaused(paused);
            }
        }

        private void StartUp()
        {
            if (_delayTweener != null)
            {
                _delayTweener = null;
            }
            BeforeStartUp();
            _playing = _totalTaskNum > 0;
            if (!_playing)
            {
                if (_onCompleteEvent != null)
                {
                    UnityEngine.Events.UnityAction func = _onCompleteEvent;
                    _onCompleteEvent = null;
                    func();
                }
                if (_onStringEvent != null)
                {
                    _onStringEvent = null;
                }
            }
        }

        private void BeforeStartUp()
        {
            _totalTaskNum = 0;
            if (!_reversed)
            {
                for (int i = 0; i < _actionList.Count; i++)
                {
                    if (_actionList[i].Active)
                    {
                        _totalTaskNum += _actionList[i].Play() ? 1 : 0;
                    }
                }
            }
            else
            {
                for (int i = _actionList.Count - 1; i >= 0; i--)
                {
                    if (_actionList[i].Active)
                    {
                        _totalTaskNum += _actionList[i].PlayReversed() ? 1 : 0;
                    }
                }
            }
        }

        public void Stop(bool setToComplete = true, bool processCallback = false)
        {
            if (!_playing)
            {
                return;
            }
            if (_delayTweener != null)
            {
                _delayTweener.Kill();
                _delayTweener = null;
            }
            _playing = false;
            _totalTaskNum = 0;
            _playTimes = 0;
            if (_onStringEvent != null && processCallback == false)
            {
                _onStringEvent = null;
            }
            if (!_reversed)
            {
                for (int i = 0; i < _actionList.Count; i++)
                {
                    TransitionActionBase action = _actionList[i];
                    if (action.Active && action.HasTweener)
                    {
                        action.Stop(setToComplete);
                    }
                }
            }
            else
            {
                for (int i = _actionList.Count - 1; i >= 0; i--)
                {
                    TransitionActionBase action = _actionList[i];
                    if (action.Active && action.HasTweener)
                    {
                        action.Stop(setToComplete);
                    }
                }
            }
            if (_onCompleteEvent != null)
            {
                if (processCallback)
                {
                    UnityEngine.Events.UnityAction func = _onCompleteEvent;
                    _onCompleteEvent = null;
                    func();
                }
                else
                {
                    _onCompleteEvent = null;
                }
            }
            _onStringEvent = null;
        }

        public void OnActionCompleted()
        {
            _totalTaskNum--;
            CheckAllActionCompleted();
        }

        private void CheckAllActionCompleted()
        {
            if (_playing && _totalTaskNum == 0)
            {
                if (_playTimes < 0)
                {
                    BeforeStartUp();
                }
                else
                {
                    if (--_playTimes > 0)
                    {
                        BeforeStartUp();
                    }
                    else
                    {
                        _playing = false;
                        if (_onCompleteEvent != null)
                        {
                            UnityEngine.Events.UnityAction func = _onCompleteEvent;
                            _onCompleteEvent = null;
                            func();
                        }
                        if (_onStringEvent != null)
                        {
                            _onStringEvent = null;
                        }
                    }
                }
            }
        }
    }
}