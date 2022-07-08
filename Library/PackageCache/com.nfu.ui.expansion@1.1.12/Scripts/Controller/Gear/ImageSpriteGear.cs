using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;
using UnityEngine.UI;


namespace ND.UI
{
    public class ImageSpriteGear : GearBase
    {
        private Image _target;

        private Sprite[] _values;

        public ImageSpriteGear(Controller parent, GearConfig config) : base(parent, config)
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
            _target = go.GetComponent<Image>();
            _active = _target != null;
            if (!_active)
            {
                return;
            }
            _values = new Sprite[config.dataArray.Length - 1];
            for (int i = 1; i < config.dataArray.Length; i ++)
            {
                _values[i - 1] = Owner.StoredSprites[config.dataArray[i]];
            }
        }

        public override void Apply()
        {
            base.Apply();
            _target.sprite = _values[_parent.SelectedIndex];
        }
    }
}