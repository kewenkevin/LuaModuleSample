using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;


namespace ND.UI
{
    public class ActiveGear : GearBase
    {
        private GameObject _target;

        private bool[] _values;

        public ActiveGear(Controller parent, GearConfig config) : base(parent, config)
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
            _values = new bool[config.dataArray.Length - 1];
            for (int i = 1; i < config.dataArray.Length; i++)
            {
                _values[i - 1] = config.dataArray[i] == UIExpansionUtility.TrueValue;
            }
        }

        public override void Apply()
        {
            base.Apply();
            if (_target.activeSelf != _values[_parent.SelectedIndex])
            {
                _target.SetActive(_values[_parent.SelectedIndex]);
            }
        }
    }
}