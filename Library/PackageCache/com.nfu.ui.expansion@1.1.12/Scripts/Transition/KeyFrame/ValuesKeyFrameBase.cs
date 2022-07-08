using ND.UI.Core.Model;
using UnityEngine;

namespace ND.UI
{
    public abstract class ValuesKeyFrameBase : KeyFrameBase
    {
        protected int _valueSize;

        protected float[] _values;

        protected bool[] _actives;

        protected AnimationCurve _curve;

        public int ValueSize { get => _valueSize; set => _valueSize = value; }
        public float[] Values { get => _values; set => _values = value; }
        public bool[] Actives { get => _actives; set => _actives = value; }
        public AnimationCurve Curve { get => _curve; set => _curve = value; }

        public ValuesKeyFrameBase(Transition parent, KeyFrameConfig config) : base(parent, config)
        {
        
        }

        public virtual bool ApplyValue(TransitionTweenAction tweenAction)
        {
            return true;
        }
    }
}