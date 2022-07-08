//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Collections;
using ND.Managers.ResourceMgr.Framework;

namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{
    /// <summary>
    /// 资源。
    /// </summary>
    public sealed class Resource : IEnumerable<Asset>
    {
        private readonly Dictionary<string, Asset> m_Assets;
        private readonly List<string> m_ResourceGroups;
        //缓存路径提高性能
        private string[] m_PathNames;
        private SortedDictionary<string, Asset> m_SoredAssets;
        private Dictionary<string, Asset> m_LowerAssets;

        private Resource(string name, string variant, string fileSystem, LoadType loadType, bool packed, string[] resourceGroups)
        {
            m_Assets = new Dictionary<string, Asset>();
            m_ResourceGroups = new List<string>();
            m_SoredAssets = new SortedDictionary<string, Asset>();
            m_LowerAssets = new Dictionary<string, Asset>();

            Name = name;
            Variant = variant;
            AssetType = AssetType.Unknown;
            FileSystem = fileSystem;
            LoadType = loadType;
            Packed = packed;
            m_PathNames = name.Split('/');

            foreach (string resourceGroup in resourceGroups)
            {
                AddResourceGroup(resourceGroup);
            }
        }

        public string Name
        {
            get;
            private set;
        }

        public string Variant
        {
            get;
            private set;
        }

        public string FullName
        {
            get
            {
                return Variant != null ? Utility.Text.Format("{0}.{1}", Name, Variant) : Name;
            }
        }

        public AssetType AssetType
        {
            get;
            private set;
        }

        public bool IsLoadFromBinary
        {
            get
            {
                return LoadType == LoadType.LoadFromBinary || LoadType == LoadType.LoadFromBinaryAndQuickDecrypt || LoadType == LoadType.LoadFromBinaryAndDecrypt;
            }
        }

        public string FileSystem
        {
            get;
            set;
        }

        public LoadType LoadType
        {
            get;
            set;
        }

        public bool Packed
        {
            get;
            set;
        }

        public string[] PathNames
        {
            get => m_PathNames;
        }

        /// <summary>
        /// Asset 数量
        /// </summary>
        public int Count
        {
            get => m_Assets.Count;
        }

        public static Resource Create(string name, string variant, string fileSystem, LoadType loadType, bool packed, string[] resourceGroups)
        {
            return new Resource(name, variant, fileSystem, loadType, packed, resourceGroups ?? new string[0]);
        }

        public Asset[] GetAssets()
        {
            return m_SoredAssets.Values.ToArray();
        }


        public IEnumerator<Asset> GetEnumerator()
        {
            return m_SoredAssets.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public Asset GetFirstAsset()
        {
            return m_SoredAssets.Values.FirstOrDefault();
        }
        public Asset GetAssetByGuid(string guid)
        {
            foreach (var item in m_Assets)
            {
                if (item.Value.Guid == guid)
                    return item.Value;
            }
            return null;
        }
        public Asset GetAssetByAssetPath(string assetPath)
        {
            Asset asset;
            if (m_Assets.TryGetValue(assetPath, out asset))
                return asset;
            return null;
        }

        public Asset GetAssetByAssetPathIgnoreCase(string assetPath)
        {
            Asset asset;
            if (m_LowerAssets.TryGetValue(assetPath.ToLower(), out asset))
                return asset;
            return null;
        }

        public void Rename(string name, string variant)
        {
            Name = name;
            Variant = variant;
            m_PathNames = name.Split('/');
        }

        public void AssignAsset(Asset asset, bool isScene)
        {
            if (asset.Resource != null)
            {
                asset.Resource.UnassignAsset(asset);
            }

            AssetType = isScene ? AssetType.Scene : AssetType.Asset;
            asset.Resource = this;
            m_Assets.Add(asset.AssetPath, asset);
            m_SoredAssets.Add(asset.AssetPath, asset);
            m_LowerAssets.Add(asset.AssetPathLower, asset);
        }

        public void UnassignAsset(Asset asset)
        {
            asset.Resource = null;
            m_Assets.Remove(asset.AssetPath);
            m_SoredAssets.Remove(asset.AssetPath);
            m_LowerAssets.Remove(asset.AssetPathLower);
            if (m_Assets.Count <= 0)
            {
                AssetType = AssetType.Unknown;
            }
        }

        public string[] GetResourceGroups()
        {
            return m_ResourceGroups.ToArray();
        }

        public bool HasResourceGroup(string resourceGroup)
        {
            if (string.IsNullOrEmpty(resourceGroup))
            {
                return false;
            }

            return m_ResourceGroups.Contains(resourceGroup);
        }

        public void AddResourceGroup(string resourceGroup)
        {
            if (string.IsNullOrEmpty(resourceGroup))
            {
                return;
            }

            if (m_ResourceGroups.Contains(resourceGroup))
            {
                return;
            }

            m_ResourceGroups.Add(resourceGroup);
            m_ResourceGroups.Sort();
        }

        public bool RemoveResourceGroup(string resourceGroup)
        {
            if (string.IsNullOrEmpty(resourceGroup))
            {
                return false;
            }

            return m_ResourceGroups.Remove(resourceGroup);
        }

        public void Clear()
        {
            foreach (Asset asset in m_Assets.Values)
            {
                asset.Resource = null;
            }

            m_Assets.Clear();
            m_SoredAssets.Clear();
            m_LowerAssets.Clear();
            m_ResourceGroups.Clear();
        }


        public override string ToString()
        {
            return $"FullName: {this.FullName}";
        }
    }
}
