using System.Collections;
using System.Collections.Generic;
using ND.UI;
using UnityEngine;


namespace ND.UI
{
    public class Binding
    {
        private UIExpansion _owner;

        //_bindersMap[StoredGameObjectIndex][binderName]-->Binder
        private Dictionary<ushort, Dictionary<string, BinderBase>> _bindersMap = new Dictionary<ushort, Dictionary<string, BinderBase>>();
        
        //所有binder都在这里
        private List<BinderBase> _bindersList = new List<BinderBase>();
        
        //_binderPropertiesMap[label][GameObjectIndex][binderName]-->LinkerTypeName
        private Dictionary<string, Dictionary<ushort, Dictionary<string, List<string>>>> _binderPropertiesMap =
            new Dictionary<string, Dictionary<ushort, Dictionary<string, List<string>>>>();
        
        
        
        private void tryCreateBinder(UIExpansion owner, LinkerConfig linkerConfig)
        {
            string label = owner.StoredStrings[linkerConfig.LabelIndex];
            GameObject gameObject = owner.StoredGameObjects[linkerConfig.StoredGameObjectIndex];
            string binderName = owner.StoredStrings[linkerConfig.BinderTypeIndex];

            {//这个代码块是整理调用属性的关系
                if (!_binderPropertiesMap.ContainsKey(label))
                {
                    _binderPropertiesMap.Add(label, new Dictionary<ushort, Dictionary<string, List<string>>>());
                }

                if (!_binderPropertiesMap[label].ContainsKey(linkerConfig.StoredGameObjectIndex))
                {
                    _binderPropertiesMap[label].Add(linkerConfig.StoredGameObjectIndex,
                        new Dictionary<string, List<string>>());
                }

                if (!_binderPropertiesMap[label][linkerConfig.StoredGameObjectIndex].ContainsKey(binderName))
                {
                    _binderPropertiesMap[label][linkerConfig.StoredGameObjectIndex].Add(binderName, new List<string>());
                }

                _binderPropertiesMap[label][linkerConfig.StoredGameObjectIndex][binderName]
                    .Add(owner.StoredStrings[linkerConfig.LinkerTypeIndex]);
                
            }
            {//这个代码块将Binders创建了出来
                if (!_bindersMap.ContainsKey(linkerConfig.StoredGameObjectIndex))
                {
                    _bindersMap.Add(linkerConfig.StoredGameObjectIndex,new Dictionary<string, BinderBase>());
                }
                if (!_bindersMap[linkerConfig.StoredGameObjectIndex].ContainsKey(binderName))
                {
                    var binder = UIExpansionUtility.GetLinker(owner, linkerConfig);
                    _bindersMap[linkerConfig.StoredGameObjectIndex].Add(binderName,binder);
                    _bindersList.Add(binder);
                }
            }
        }

        public Dictionary<ushort, Dictionary<string, List<string>>> GetLabel(string Label)
        {
            Dictionary<ushort, Dictionary<string, List<string>>> map;
            if (_binderPropertiesMap.TryGetValue(Label, out map))
            {
                return _binderPropertiesMap[Label];
            }
            else
            {
                return null;
            }
        }

        public BinderBase GetBinder(ushort gameObjectIndex,string binderTypeName)
        {
            Dictionary<string, BinderBase> map;
            if (_bindersMap.TryGetValue(gameObjectIndex, out map))
            {
                BinderBase result;
                if (map.TryGetValue(binderTypeName, out result))
                {
                    return result;
                }
                else
                {
                    Debug.LogError($"not found binderTypeName {binderTypeName}");
                    return null;
                }
            }
            else
            {
                Debug.LogError($"not found gameObjectIndex {_owner.GetStoredGameObject(gameObjectIndex).name}");
                return null;
            }
        }



        public Binding(UIExpansion owner, BindingConfig config)
        {
            if (config.linkerConfigs == null || config.linkerConfigs.Length == 0)
            {
                return;
            }
            _owner = owner;
            for (int i = 0; i < config.linkerConfigs.Length; i++)
            {
                tryCreateBinder(owner,config.linkerConfigs[i]);
            }
        }

        public void RemoveAction(string label)
        {
            var dic = GetLabel(label);
            if (dic == null) return;

            foreach (var kv in dic)
            {
                foreach (var md in kv.Value)
                {
                    var binder = GetBinder(kv.Key, md.Key);
                    if (binder == null) continue;
                    for (int i = 0; i <  md.Value.Count; i++)
                    {
                        binder.linkerType = md.Value[i];
                        binder.RemoveAllAction();
                    }
                }
            }
        }

        public void RemoveAllAction()
        {
            foreach (var lv in _binderPropertiesMap)
            {
                RemoveAction(lv.Key);
            }
        }

        public void Dispose()
        {
            if (_bindersList!=null)
            {
                for (int i = 0; i < _bindersList.Count; i++)
                {
                    _bindersList[i].Dispose();
                }
            }
        }
    }
}