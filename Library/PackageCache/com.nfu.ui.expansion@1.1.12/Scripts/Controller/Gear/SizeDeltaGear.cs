using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;

namespace ND.UI
{
    public class SizeDeltaGear : GearBase
    {
        private RectTransform _target;

        private Vector2[] _values;

        public SizeDeltaGear(Controller parent, GearConfig config) : base(parent, config)
        {
        }

        public override void Init(GearConfig config)
        {
            base.Apply();
            GameObject go = Owner.GetStoredGameObject(config.StoredGameObjectIndex);
            if (go == null)
            {
                _active = false;
                return;
            }
            _target = go.GetComponent<RectTransform>();
            _active = _target != null;
            if (!_active)
            {
                return;
            }
            _values = new Vector2[(config.dataArray.Length - 1) / 2];
            for (int i = 1; i < config.dataArray.Length; i += 2)
            {
                _values[(i - 1) / 2] = new Vector2(
                    Owner.StoredFloats[config.dataArray[i]],
                    Owner.StoredFloats[config.dataArray[i + 1]]);
            }
        }

        public override void Apply()
        {
            base.Apply();
            _target.sizeDelta = _values[_parent.SelectedIndex];
        }
    }
}