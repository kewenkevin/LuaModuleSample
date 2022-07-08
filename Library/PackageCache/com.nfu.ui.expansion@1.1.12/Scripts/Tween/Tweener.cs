using System.Collections;
using System.Collections.Generic;
using ND.UI;
using UnityEngine;


namespace ND.UI
{
    public delegate void TweenCallback();

    public class Tweener
    {
        private object _target;

        private bool _killed;

        private bool _paused;

        private float _delay;

        private float _duration;

        private EaseType _easeType;

        private float _elapsedTime;

        private int _ended = 0;

        private bool _started = false;

        private TweenValue _startValue = new TweenValue();
        private TweenValue _endValue = new TweenValue();
        private TweenValue _value = new TweenValue();
        private int _valueSize;

        //Ease相关
        private float _normalizedTime;

        private float _easeOvershootOrAmplitude = 1.70158f;

        private int _easePeriod = 0;

        private bool _ignoreEngineTimeScale;

        private AnimationCurve _curve;

        private TweenCallback _onComplete;

        private ITweenListener _listener;

        public bool Killed { get => _killed; set => _killed = value; }
        public object Target { get => _target; set => _target = value; }
        public bool Paused { get => _paused; set => _paused = value; }
        public TweenValue Value { get => _value; set => _value = value; }

        public void Init()
        {
            _delay = 0;
            _duration = 0;
            _elapsedTime = 0;
            _ended = 0;
            _easeType = EaseType.Linear;
            _curve = null;
            _started = false;
            _paused = false;
            _killed = false;
        }

        public void Kill(bool complete = false)
        {
            if (Killed)
            {
                return;
            }
            if (complete)
            {
                _value = _endValue;
                CallCompleteCallback();
            }
            _killed = true;
        }

        public void Reset()
        {
            _target = null;
            _listener = null;
            _onComplete = null;
        }

        /// <summary>
        /// 主要负责刷新运行的时间
        /// </summary>
        public void Update()
        {
            float dt = 0;
            if (Application.isPlaying)
            {
                dt = Time.deltaTime;
            }
            else
            {
                dt = TweenManager.Instance.DeltaTime;
            }
            _elapsedTime += dt;
            if (_ended != 0)
            {
                if (!_killed)
                {
                    // _value = _endValue;
                    _killed = true;
                    CallCompleteCallback();
                }
                return;
            }
            else
            {
                OnUpdate();  
            }
        }

        private void CallCompleteCallback()
        {
            if (_onComplete != null)
            {
                _onComplete();
            }
            if (_listener != null)
            {
                _listener.OnTweenComplete();
            }

            _ended = 1;
        }



        private void OnUpdate()
        {
            if (_killed)
            {
                _ended = 1;
                return;
            }
            if (_valueSize == 0)
            {
                if (_elapsedTime >= _delay + _duration)
                {
                    _ended = 1;
                }
                return;
            }
            if (!_started)
            {
                if (_elapsedTime < _delay)
                {
                    return;
                }
                _started = true;
                if (_listener != null)
                {
                    _listener.OnTweenStart();
                }
            }
            float runTime = _elapsedTime - _delay;
            if (runTime >= _duration)
            {
                runTime = _duration;
                _ended = 1;
            }
            _normalizedTime = EaseManager.Evaluate(
                                                _easeType, 
                                                runTime, 
                                                _duration, 
                                                _easeOvershootOrAmplitude, 
                                                _easePeriod, 
                                                _curve);

            for (int i = 0; i < _valueSize; i++)
            {
                float n1 = _startValue[i];
                float n2 = _endValue[i];
                float f = n1 + (n2 - n1) * _normalizedTime;
                _value[i] = f;
            }
            if (_listener != null)
            {
                _listener.OnTweenUpdate();
            }
        }

        #region 设置函数

        public Tweener SetEase(EaseType easeType)
        {
            _easeType = easeType;
            return this;
        }
        
        public Tweener SetEase(EaseType easeType, AnimationCurve customCurve)
        {
            _easeType = easeType;
            _curve = customCurve;
            return this;
        }

        public Tweener SetListener(ITweenListener listener)
        {
            _listener = listener;
            return this;
        }

        public Tweener SetDelay(float delay)
        {
            _delay = delay;
            return this;
        }

        private float _timeScale;

        public Tweener SetTimeScale(float value)
        {
            _timeScale = value;
            return this;
        }

        public Tweener SetIgnoreEngineTimeScale(bool value)
        {
            _ignoreEngineTimeScale = value;
            return this;
        }

        public Tweener SetTarget(object target)
        {
            _target = target;
            // _propType = TweenPropType.None;
            return this;
        }
        /*
        public Tweener SetTarget(object target, TweenPropType tweenPropType)
        {
            _target = target;
            _propType = tweenPropType;
            return this;
        }*/

        public Tweener OnComplete(TweenCallback tweenCallback)
        {
            _onComplete = tweenCallback;
            return this;
        }
        #endregion

        #region To函数

        /*
        public Tweener To(TransitionItem startItem, TransitionItem endItem, float duration)
        {
            _startValue = new TweenValue();
            _endValue = new TweenValue();
            _value = new TweenValue();
            _valueSize = startItem.Values.Length;
            for (int i = 0; i < startItem.Values.Length; i++)
            {
                _startValue[i] = startItem.Values[i];
                _endValue[i] = endItem.Values[i];
                _value[i] = startItem.Values[i];
            }
            _duration = duration;
            return this;
        }*/

        public Tweener To(float[] startValues, float[] endValues, int valueSize, float duration)
        {
            _startValue = new TweenValue();
            _endValue = new TweenValue();
            _value = new TweenValue();
            _valueSize = valueSize;
            for (int i = 0; i < valueSize; i++)
            {
                _startValue[i] = startValues[i];
                _endValue[i] = endValues[i];
                _value[i] = startValues[i];
            }
            _duration = duration;
            return this;
        }

        public Tweener To(List<float> startValues, List<float> endValues, int valueSize, float duration)
        {
            _startValue = new TweenValue();
            _endValue = new TweenValue();
            _value = new TweenValue();
            _valueSize = valueSize;
            for (int i = 0; i < valueSize; i++)
            {
                _startValue[i] = startValues[i];
                _endValue[i] = endValues[i];
                _value[i] = startValues[i];
            }
            _duration = duration;
            return this;
        }

        public Tweener To(float start, float end, float duration)
        {
            _valueSize = 1;
            _startValue.x = start;
            _endValue.x = end;
            _value.x = start;
            _duration = duration;
            return this;
        }

        internal Tweener To(Vector2 start, Vector2 end, float duration)
        {
            _valueSize = 2;
            _startValue.vec2 = start;
            _endValue.vec2 = end;
            _value.vec2 = start;
            _duration = duration;
            return this;
        }
        internal Tweener To(Vector3 start, Vector3 end, float duration)
        {
            _valueSize = 3;
            _startValue.vec3 = start;
            _endValue.vec3 = end;
            _value.vec3 = start;
            _duration = duration;
            return this;
        }

        internal Tweener To(Vector4 start, Vector4 end, float duration)
        {
            _valueSize = 4;
            _startValue.vec4 = start;
            _endValue.vec4 = end;
            _value.vec4 = start;
            _duration = duration;
            return this;
        }

        internal Tweener To(Color start, Color end, float duration)
        {
            _valueSize = 4;
            _startValue.color = start;
            _endValue.color = end;
            _value.color = start;
            _duration = duration;
            return this;
        }
        #endregion
    }
}