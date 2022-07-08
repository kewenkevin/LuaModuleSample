using System.Collections.Generic;
using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;

namespace ND.UI
{
    public class TextColorStyleGear : GearBase
    {
        private ND.UI.Core.IColorStyleUseAble _target;

        private List<ND.UI.Core.ColorStyleBase> _values;

        public TextColorStyleGear(Controller parent, GearConfig config) : base(parent, config)
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
            _target = go.GetComponent<ND.UI.Core.IColorStyleUseAble>();
            _active = _target != null;
            if (!_active)
            {
                return;
            }
            _values = new List<ColorStyleBase>();
            for (int i = 1; i < config.dataArray.Length; i++)
            {
                _values.Add(Owner.StoredTextColorStyles[config.dataArray[i]]);
            }
        }

        public override void Apply()
        {
            base.Apply();
            _target.colorStyle = _values[_parent.SelectedIndex];
        }
    }
}