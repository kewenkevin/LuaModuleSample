using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;

namespace ND.UI
{
    public class PositionGear : GearBase
    {
        private RectTransform _target;

        private Vector3[] _values;

        public PositionGear(Controller parent, GearConfig config) : base(parent, config)
        {
        }

        public override void Init(GearConfig config)
        {
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
            _values = new Vector3[(config.dataArray.Length - 1) / 3];
            for (int i = 1; i < config.dataArray.Length; i += 3)
            {
                _values[(i - 1) / 3] = new Vector3(
                    Owner.StoredFloats[config.dataArray[i]],
                    Owner.StoredFloats[config.dataArray[i + 1]],
                    Owner.StoredFloats[config.dataArray[i + 2]]);
            }
        }

        public override void Apply()
        {
            base.Apply();
            _target.localPosition = _values[_parent.SelectedIndex];
        }
    }
}