using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;


namespace ND.UI
{
    public class PercentPositionGear : GearBase
    {
        private RectTransform _target;

        private Vector3[] _values;
        
        public PercentPositionGear(Controller parent, GearConfig config) : base(parent, config)
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
            float width = 1920;
            float height = 1080;
            UIExpansionUtility.GetCanvasSize(out width, out height);
            _target.localPosition = new Vector3(_values[_parent.SelectedIndex].x*width,
                _values[_parent.SelectedIndex].y*height,_target.localPosition.z);
        }
    }
}