using System.Collections.Generic;
using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;

namespace ND.UI
{
    public class TextFontStyleGear : GearBase
    {
        private ND.UI.Core.ITextStyleUseAble _target;

        private List<ND.UI.Core.TextStyleBase> _values;

        public TextFontStyleGear(Controller parent, GearConfig config) : base(parent, config)
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
            _target = go.GetComponent<ND.UI.Core.ITextStyleUseAble>();
            _active = _target != null;
            if (!_active)
            {
                return;
            }
            _values = new List<ND.UI.Core.TextStyleBase>();
            for (int i = 1; i < config.dataArray.Length; i++)
            {
                _values.Add(Owner.StoredTextFontStyles[config.dataArray[i]]);
            }
        }

        public override void Apply()
        {
            base.Apply();
            _target.style = _values[_parent.SelectedIndex];
        }
    }
}