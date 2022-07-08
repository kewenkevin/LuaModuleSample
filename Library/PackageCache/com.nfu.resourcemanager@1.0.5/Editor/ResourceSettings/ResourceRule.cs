using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{

    /// <summary>
    /// 资源规则，筛选资源进行处理
    /// </summary>
    [Serializable]
    public class ResourceRule : ISerializationCallbackReceiver
    {
        public string include;
        public string exclude;

        [SerializeField]
        public string addressableTypeName;

        private System.Type addressableType;
        private IAddressableProvider addressableProvider;

        /// <summary>
        /// 寻址器类型
        /// </summary>
        public System.Type AddressableType
        {
            get
            {
                return addressableType;
            }
            set
            {
                if (value != addressableType)
                {

                    if (value != null && typeof(IAddressableProvider).IsAssignableFrom(value))
                    {
                        addressableType = value;
                    }
                    else
                    {
                        addressableType = null;
                    }
                    if (addressableType != null)
                    {
                        addressableProvider = Activator.CreateInstance(addressableType) as IAddressableProvider;
                        if (addressableProvider != null)
                        {
                            if (addressableProvider is ISerializationCallbackReceiver)
                                ((ISerializationCallbackReceiver)addressableProvider).OnAfterDeserialize();
                        }
                    }
                    else
                    {
                        addressableProvider = null;
                    }
                }
            }
        }

        public IAddressableProvider AddressableProvider { get => addressableProvider; }

        public bool IsMatch(string assetPath)
        {
            if (!EditorUtilityx.IncludeExclude(assetPath, include, exclude))
                return false;

            if (EditorResourceSettings.IsExcludeAssetPath(assetPath))
                return false;

            var provider = AddressableProvider;
            if (provider != null && !provider.IsAssetPathMatch(assetPath))
                return false;

            return true;
        }

        public void OnAfterDeserialize()
        {
            addressableType = null;
            if (!string.IsNullOrEmpty(addressableTypeName))
            {
                AddressableType = System.Type.GetType(addressableTypeName);
            }
        }

        public void OnBeforeSerialize()
        {
            addressableTypeName = null;
            if (addressableType != null)
                addressableTypeName = addressableType.AssemblyQualifiedName;
        }
    }
}