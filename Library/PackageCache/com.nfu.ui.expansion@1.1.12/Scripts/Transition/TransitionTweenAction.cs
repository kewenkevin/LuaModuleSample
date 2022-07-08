
using System.Collections.Generic;


namespace ND.UI
{
    public class TransitionTweenAction :TransitionActionBase
{
        private ValuesKeyFrameBase _leftFrame;
        private ValuesKeyFrameBase _rightFrame;
        private EaseType _easeType;

        public TransitionTweenAction(KeyFrameBase leftKeyFrame, KeyFrameBase rightKeyFrame, EaseType easeType)
        {

            _leftFrame = leftKeyFrame as ValuesKeyFrameBase;
            _rightFrame = rightKeyFrame as ValuesKeyFrameBase;
            _easeType = easeType;
            _active = leftKeyFrame.Active && rightKeyFrame.Active;
        }

        public List<bool> Actives
        {
            get
            {
                List<bool> actives = new List<bool>();
                for (int i = 0; i < _leftFrame.Actives.Length; i++)
                {
                    actives.Add(_leftFrame.Actives[i] || _rightFrame.Actives[i]);
                }
                return actives;
            }
        }

        public TweenValue Value
        {
            get
            {
                return _tweener.Value;
            }
        }

        public override void OnTweenComplete()
        {
            // _tweener.Kill(true);
            // _tweener = null;
            _leftFrame.Parent.OnActionCompleted();
            _tweener = null;
        }

        public override void OnTweenStart()
        {
      
        }

        public override void OnTweenUpdate()
        {
            if (!_leftFrame.ApplyValue(this))
            {
                Stop(false);
            }
        }

        public override bool Play()
        {
            float delayTime = _leftFrame.FrameTime;
            float durationTime = _rightFrame.FrameTime - _leftFrame.FrameTime;
            this._tweener = TweenManager.Instance.CreateTweener().To(_leftFrame.Values, _rightFrame.Values, _leftFrame.ValueSize, durationTime);
            this._tweener.SetDelay(delayTime)
                 .SetEase(_easeType,_leftFrame.Curve)
                 .SetTimeScale(1)
                 .SetIgnoreEngineTimeScale(true)
                 .SetTarget(this)
                 .SetListener(this);
            return true;
        }

        public override bool PlayReversed()
        {
            float delayTime = _leftFrame.Parent.Length - _rightFrame.FrameTime;
            float durationTime = _rightFrame.FrameTime - _leftFrame.FrameTime;
            this._tweener = TweenManager.Instance.CreateTweener().To(_rightFrame.Values, _leftFrame.Values, _leftFrame.ValueSize, durationTime);
            this._tweener.SetDelay(delayTime)
                 .SetEase(_easeType,_leftFrame.Curve)
                 .SetTimeScale(1)
                 .SetIgnoreEngineTimeScale(true)
                 .SetTarget(this)
                 .SetListener(this);
            return true;
        }

        public override void SetPaused(bool paused)
        {
            if (_tweener != null)
            {
                _tweener.Paused = paused;
            }
        }

        public override void Stop(bool setToComplete)
        {
            if (_tweener != null)
            {
                _tweener.Kill(setToComplete);
                _tweener = null;
            }
        }
    }
}