using System.Collections;
using System.Collections.Generic;
using ND.UI;
using UnityEngine;

namespace ND.UI
{
    public class ModuleData
    {
        private string _label;

        // private string _prefabPath;

        private UIExpansion _ui;

        public ModuleData()
        {

        }

        public ModuleData(UIExpansion owner, LinkerConfig config)
        {
            Init(owner, config);
        }

        public ModuleData(UIExpansion ui, string prefabPath)
        {
            _ui = ui;
        }

        public string Label { get => _label; }
        public string PrefabPath { get => string.Empty; }
        public UIExpansion UI { get => _ui; }

        public bool Init(UIExpansion owner, LinkerConfig config)
        {
            _label = owner.StoredStrings[config.LabelIndex];
            _ui = owner.GetStoredGameObject(config.StoredGameObjectIndex).GetComponent<UIExpansion>();
            // _gameObjectId = config.StoredGameObjectIndex;
            // _binderId = owner.StoredInts[config.BinderTypeIndex];
            // _linkerId = owner.StoredInts[config.LinkerTypeIndex];
            //owner.StoredStrings[config.PrefabPathIndex];
            return true;
        }
    }
}