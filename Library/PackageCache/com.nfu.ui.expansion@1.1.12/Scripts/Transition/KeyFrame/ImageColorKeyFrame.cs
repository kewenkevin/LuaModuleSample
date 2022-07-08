using ND.UI.Core.Model;
using UnityEngine;
using UnityEngine.UI;

namespace ND.UI
{
    public class ImageColorKeyFrame : ValuesKeyFrameBase
    {
        private Image _target;

        public ImageColorKeyFrame(Transition parent, KeyFrameConfig config) : base(parent, config)
        {
        }

        public override void Init(KeyFrameConfig config)
        {
            GameObject go = Owner.GetStoredGameObject(config.StoredGameObjectIndex);
            if (go == null)
            {
                _active = false;
                return;
            }
            _target = go.GetComponent<Image>();
            _active = _target != null;
            if (!_active)
            {
                return;
            }
            _valueSize = 4;
            _frameTime = (float)Owner.StoredInts[config.dataList[1]] / 30f;
            int checkNum = 2;
            _values = new float[_valueSize];
            for (int i = 0; i < _valueSize; i++)
            {
                _values[i] = Owner.StoredFloats[config.dataList[checkNum++]];
            }
            _actives = new bool[_valueSize];
            for (int i = 0; i < _valueSize; i++)
            {
                _actives[i] = config.dataList[checkNum++] == UIExpansionUtility.TrueValue;
            }
            //适配1.0.17(含)之前的动效 老版本动效没有动画曲线 导致解析错误
            if (Owner.StoredAnimationCurves == null || Owner.StoredAnimationCurves.Length<=0)
            {
                _curve = AnimationCurve.Linear(0, 0, 1, 1);
            }
            else
            {
                _curve = Owner.StoredAnimationCurves[config.dataList[config.dataList.Length - 1]];
            }
        }

        public override void ApplyValue()
        {
            if (_target != null)
            {
                _target.color = new Color(_values[0], _values[1], _values[2], _values[3]);
            }
        }

        public override bool ApplyValue(TransitionTweenAction tweenAction)
        {
            if (_target == null)
            {
                return false;
            }
            _target.color = new Color(tweenAction.Value[0], tweenAction.Value[1], tweenAction.Value[2], tweenAction.Value[3]);
            return true;
        }
    }
}