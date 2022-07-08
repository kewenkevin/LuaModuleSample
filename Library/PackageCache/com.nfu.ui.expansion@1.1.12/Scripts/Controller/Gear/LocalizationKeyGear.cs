using ND.UI.Core;
using ND.UI.Core.Model;
using ND.UI.I18n;
using UnityEngine;

namespace ND.UI
{
    public class LocalizationKeyGear : GearBase
    {
        private ILocalizationable _target;

        /// <summary>
        /// 目标翻译gear的类型
        /// </summary>
        private GearTypeState _targetGearType;
        
        private string[] _values;

        public LocalizationKeyGear(Controller parent, GearConfig config) : base(parent, config)
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
            
            _targetGearType = (GearTypeState)config.dataArray[1];
            
            var components = go.GetComponents<ILocalizationable>();
            for (int i = 0; i < components.Length; i++)
            {
                if ((int) _targetGearType == components[i].LocalizationGearType)
                {
                    _target = components[i];
                    break;
                }
            }

            _active = _target != null;
            if (!_active)
            {
                return;
            }

            _values = new string[config.dataArray.Length - 2];
            for (int i = 2; i < config.dataArray.Length; i++)
            {
                _values[i - 2] = Owner.StoredStrings[config.dataArray[i]];
            }
        }

        public override void Apply()
        {
            base.Apply();
            _target.LocalizationKey = _values[_parent.SelectedIndex];
        }
    }
}