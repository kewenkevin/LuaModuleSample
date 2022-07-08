using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;

namespace ND.UI
{
    public class OverallAlphaGear : GearBase
    {
        private GameObject _target;

        private float[] _values;

        public OverallAlphaGear(Controller parent, GearConfig config) : base(parent, config)
        {
        }

        public override void Init(GearConfig config)
        {
            _target = Owner.GetStoredGameObject(config.StoredGameObjectIndex);
            _active = (_target != null);
            if (!_active)
            {
                return;
            }
            _values = new float[config.dataArray.Length - 1];
            for (int i = 1; i < config.dataArray.Length; i++)
            {
                _values[i - 1] = Owner.StoredFloats[config.dataArray[i]];
            }
        }

        public override void Apply()
        {
            base.Apply();
            var canvasGroup = _target.GetComponent<CanvasGroup>();
            if (!canvasGroup)
            {
                canvasGroup = _target.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = _values[_parent.SelectedIndex];
        }
        
    }
}