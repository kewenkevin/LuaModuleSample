using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;

namespace ND.UI
{
    public class RatingTotalGear : GearBase
    {
        private GameObject _target;

        private int[] _values;

        public RatingTotalGear(Controller parent, GearConfig config) : base(parent, config) { }

        public override void Init(GearConfig config)
        {
            _target = Owner.GetStoredGameObject(config.StoredGameObjectIndex);
            _active = (_target != null);
            if (!_active)
            {
                return;
            }
            
            _values = new int[config.dataArray.Length - 1];
            for (int i = 1; i < config.dataArray.Length; i++)
            {
                _values[i - 1] = Owner.StoredInts[config.dataArray[i]];
            }
        }

        public override void Apply()
        {
            base.Apply();
            var rating = _target.GetComponent<IRating>();
            rating.total = _values[_parent.SelectedIndex];
        }
        
    }
}