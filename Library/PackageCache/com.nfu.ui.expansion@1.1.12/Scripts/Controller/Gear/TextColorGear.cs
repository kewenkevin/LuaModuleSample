using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;
using UnityEngine.UI;

namespace ND.UI
{
    public class TextColorGear : GearBase
    {
        private Text _target;

        private Color[] _values;

        public TextColorGear(Controller parent, GearConfig config) : base(parent, config)
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
            _target = go.GetComponent<Text>();
            _active = _target != null;
            if (!_active)
            {
                return;
            }
            _values = new Color[(config.dataArray.Length - 1) / 4];
            for (int i = 1; i < config.dataArray.Length; i += 4)
            {
                _values[(i - 1) / 4] = new Color(
                    Owner.StoredFloats[config.dataArray[i]],
                    Owner.StoredFloats[config.dataArray[i + 1]],
                    Owner.StoredFloats[config.dataArray[i + 2]],
                    Owner.StoredFloats[config.dataArray[i + 3]]);
            }
        }

        public override void Apply()
        {
            base.Apply();
            _target.color = _values[_parent.SelectedIndex];
        }
    }
}