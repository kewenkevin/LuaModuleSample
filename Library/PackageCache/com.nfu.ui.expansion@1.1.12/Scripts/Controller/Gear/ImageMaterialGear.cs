using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;
using UnityEngine.UI;

namespace ND.UI
{
    public class ImageMaterialGear : GearBase
    {
        private Graphic _target;
        private Material[] _values;

        public ImageMaterialGear(Controller parent, GearConfig config) : base(parent, config)
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
            _target = go.GetComponent<Graphic>();
            _active = _target != null;
            if (!_active)
            {
                return;
            }
            _values = new Material[config.dataArray.Length-1];
            for (int i  = 1; i < config.dataArray.Length; i++)
            {
                _values[i - 1] = Owner.StoredMaterials[config.dataArray[i]];
            }
        }

        public override void Apply()
        {
            base.Apply();
            _target.material = _values[_parent.SelectedIndex];
        }
    }
}