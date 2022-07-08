using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;

namespace ND.UI
{
    public class ControllerGear : GearBase
    {
        private IController _target;

        private int[] _values;

        public ControllerGear(Controller parent, GearConfig config) : base(parent, config)
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
            UIExpansion uiExpansion = go.GetComponent<UIExpansion>();
            if (uiExpansion == null)
            {
                _active = false;
                return;
            }
            if (!uiExpansion.Ready)
            {
                uiExpansion.Init();
            }
            _target = uiExpansion.GetController(Owner.StoredStrings[config.dataArray[1]]);
            _active = _target != null;
            if (!_active)
            {
                return;
            }
            _values = new int[config.dataArray.Length - 2];
            for (int i = 2; i < config.dataArray.Length; i++)
            {
                _values[i - 2] = Owner.StoredInts[config.dataArray[i]];
            }
        }

        public override void Apply()
        {
            base.Apply();
            _target.SelectedIndex = _values[_parent.SelectedIndex];
        }
    }
}