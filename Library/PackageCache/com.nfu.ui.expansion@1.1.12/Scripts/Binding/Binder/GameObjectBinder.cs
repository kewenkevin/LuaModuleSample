using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ND.UI
{
    public class GameObjectBinder : BinderBase
    {
        public enum AttributeType : int
        {
            Active,
        }

        private GameObject _target;

        public GameObjectBinder(UIExpansion owner, LinkerConfig config) : base(owner, config)
        {
        }

        public override void Init(LinkerConfig config)
        {
            _target = _owner.GetStoredGameObject(config.StoredGameObjectIndex);
            _active = _target != null;
        }

        public override void SetBoolean(bool value)
        {
            switch ((AttributeType)Enum.Parse(typeof(AttributeType), _linkerType))
            {
                case AttributeType.Active:
                    _target.SetActive(value);
                    break;
                default:
                    break;
            }
            base.SetBoolean(value);
        }
    }
}
