using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ND.UI
{
    public class TransitionKeyFrameAction : TransitionActionBase
    {
        private KeyFrameBase _frame;

        public TransitionKeyFrameAction(KeyFrameBase frame)
        {
            _frame = frame;
            _active = frame.Active;
        }

        public override void OnTweenComplete()
        {
          
        }

        public override void OnTweenStart()
        {
         
        }

        public override void OnTweenUpdate()
        {
            
        }

        public override bool Play()
        {
            if (_frame.FrameTime == 0)
            {
                _frame.ApplyValue();
                return false;
            }
            this._tweener = TweenManager.Instance.CreateTweener()
                .SetDelay(_frame.FrameTime)
                .SetTimeScale(1)
                .SetIgnoreEngineTimeScale(true)
                .SetTarget(this)
                .OnComplete(OnDelayPlayComplete);
            return true;
        }

        public override bool PlayReversed()
        {
            float delayTime = _frame.Parent.Length - _frame.FrameTime;
            if (delayTime == 0)
            {
                _frame.ApplyValue();
                return false;
            }
            this._tweener = TweenManager.Instance.CreateTweener()
                .SetDelay(delayTime)
                .SetTimeScale(1)
                .SetIgnoreEngineTimeScale(true)
                .SetTarget(this)
                .OnComplete(OnDelayPlayComplete);
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
                _tweener.Kill(false);
                _tweener = null;
            }
        }

        public void OnDelayPlayComplete()
        {
            _frame.ApplyValue();
            if (_tweener != null)
            {
                _tweener.Kill(false);
                _tweener = null;
            }
            _frame.Parent.OnActionCompleted();

        }
    }
}