using ND.UI.Core.Model;
using UnityEngine;
using UnityEngine.UI;

namespace ND.UI
{
    public class ImageSpriteKeyFrame : KeyFrameBase
    {
        private Image _target;

        private Sprite _sprite;

        public ImageSpriteKeyFrame(Transition parent, KeyFrameConfig config) : base(parent, config)
        {
        }

        public override void Init(KeyFrameConfig config)
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
            _frameTime = (float)Owner.StoredInts[config.dataList[1]] / 30f;
            _sprite = Owner.StoredSprites[config.dataList[2]];
        }

        public override void ApplyValue()
        {
            if (_target != null)
            {
                _target.sprite = _sprite;
            }
        }
    }
}