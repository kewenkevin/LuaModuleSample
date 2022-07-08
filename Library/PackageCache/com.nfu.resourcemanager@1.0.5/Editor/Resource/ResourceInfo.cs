using System;
using System.Collections.Generic;
using System.Linq;
using ND.Managers.ResourceMgr.Framework;
using ND.Managers.ResourceMgr.Framework.Resource;
using ND.Managers.ResourceMgr.Runtime;

namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{

    /// <summary>
    /// 统一的资源信息
    /// </summary>
    public struct ResourceInfo
    {
        private int m_Length;
        private int m_HashCode;
        private int m_ZipLength;
        private int m_ZipHashCode;
        private Resource resource;

        public ResourceInfo(string name, string variant, string extension, string fileSystem, LoadType loadType, int length, int hashCode, string[] resourceGroups = null)
        {
            this.Extension = extension;
            m_Length = length;
            m_HashCode = hashCode;
            this.resource = Resource.Create(name, variant, fileSystem, loadType, true, resourceGroups ?? new string[0]);
            m_ZipLength = 0;
            m_ZipHashCode = 0;
        }
        public ResourceInfo(Resource resource)
        {
            this.resource = resource;
            this.Extension = null;
            this.m_Length = 0;
            this.m_HashCode = 0;
            m_ZipLength = 0;
            m_ZipHashCode = 0;
        }
        public ResourceInfo(Resource resource, string extension, int length, int hashCode)
        {
            this.resource = resource;
            this.Extension = extension;
            this.m_Length = length;
            this.m_HashCode = hashCode;
            m_ZipLength = 0;
            m_ZipHashCode = 0;
        }
        public Resource Resource
        {
            get => resource;
        }
        public string Name { get => resource.Name; }

        public string Variant { get => resource.Variant; }
        public string Extension { get; set; }

        public bool UseFileSystem
        {
            get
            {
                return !string.IsNullOrEmpty(FileSystem);
            }
        }

        public string FileSystem
        {
            get => resource.FileSystem;
        }

        public LoadType LoadType
        {
            get => resource.LoadType;
        }

        public int Length
        {
            get => m_Length;
            set => m_Length = value;
        }

        public int HashCode
        {
            get => m_HashCode;
            set => m_HashCode = value;
        }


        public int ZipLength { get => m_ZipLength; set => m_ZipLength = value; }

        public int ZipHashCode { get => m_ZipHashCode; set => m_ZipHashCode = value; }

        public string WithExtensionName
        {
            get
            {
                if (!string.IsNullOrEmpty(Extension))
                {
                    return $"{Name}.{Extension}";
                }
                return Name;
            }
        }
        public string FullName
        {
            get => resource.FullName;
        }
        public ResourceManager.ResourceName ResourceName
        {
            get
            {
                return new ResourceManager.ResourceName(Name, Variant, Extension);
            }
        }


        public string FileName
        {
            get
            {
                string fileSystem = FileSystem;

                //RawAssets
                if (LoadType == LoadType.LoadFromBinary && fileSystem == null)
                {
                    var firstAsset = resource.GetFirstAsset();
                    if (firstAsset != null)
                        return firstAsset.AssetPath;
                }

                if (fileSystem != null)
                    return $"{fileSystem}.{ResourceBuilderController.DefaultExtension}";
                if (Variant != null)
                    return $"{Name}.{Variant}.{ResourceBuilderController.DefaultExtension}";
                return $"{Name}.{ResourceBuilderController.DefaultExtension}";
            }
        }

        public override string ToString()
        {
            return ResourceName.ToString();
        }

        public static string GetExtension(ResourceInfo resourceInfo)
        {
            if (resourceInfo.Resource.IsLoadFromBinary)
            {
                if (string.IsNullOrEmpty(resourceInfo.FileSystem) && Utility.Path.DirectoryStartsWith(resourceInfo.Name, ResourceSettings.RawAssetsPath))
                {
                    return null;
                }
                
                var firstAsset = resourceInfo.Resource.GetAssets().FirstOrDefault();
                if (firstAsset == null)
                {
                    //Debug.LogErrorFormat("Resource [{0}] has no asset files", resourceInfo.Name);
                    //索引报错
                    return ResourceBuilderController.DefaultExtension;
                }

                string assetName = firstAsset.AssetPath;
                int position = assetName.LastIndexOf('.');
                if (position >= 0)
                {
                    return assetName.Substring(position + 1);
                }
            }

            return ResourceBuilderController.DefaultExtension;
        }

        internal static int GetAssetIndex(Dictionary<Asset, int> assetIndexs, string assetPath)
        {
            int index = -1;
            foreach (var item in assetIndexs)
            {
                if (item.Key.AssetPath == assetPath)
                {
                    index = item.Value;
                    break;
                }
            }
            if (index == -1)
                throw new Exception("not found index: " + assetPath);
            return index;
        }

        internal static int[] GetAssetIndexes(Dictionary<Asset, int> assetIndexs, string[] assetPaths)
        {
            if (assetPaths == null)
                return new int[0];
            int[] indexs = new int[assetPaths.Length];
            for (int i = 0; i < assetPaths.Length; i++)
            {
                string assetPath = assetPaths[i];
                indexs[i] = GetAssetIndex(assetIndexs, assetPath);
            }
            return indexs;
        }

        internal static List<Asset> GetAssetIndexs(ResourceInfo[] resourceInfos, out Dictionary<Asset, int> assetIndexs)
        {
            List<Asset> assets = new List<Asset>();
            assetIndexs = new Dictionary<Asset, int>();
            foreach (var resInfo in resourceInfos)
            {
                foreach (var asset in resInfo.Resource.GetAssets())
                {
                    assetIndexs[asset] = assets.Count;
                    assets.Add(asset);
                }
            }
            return assets;
        }

        internal static string FixName(string name)
        {
            return ResourceBuilderController.FixName(name);
        }

        internal static SortedDictionary<string, List<int>> GetFileSystemIndexs(ResourceInfo[] resourceInfos)
        {
            SortedDictionary<string, List<int>> fileSystemIndexs = new SortedDictionary<string, List<int>>();
            int index = 0;
            foreach (ResourceInfo resInfo in resourceInfos)
            {
                if (resInfo.UseFileSystem)
                {
                    string fileSystem = resInfo.FileSystem;
                    List<int> resourceIndexes = null;
                    if (!fileSystemIndexs.TryGetValue(fileSystem, out resourceIndexes))
                    {
                        resourceIndexes = new List<int>();
                        fileSystemIndexs.Add(fileSystem, resourceIndexes);
                    }

                    resourceIndexes.Add(index);
                }

                index++;
            }
            return fileSystemIndexs;
        }
        internal static string[] GetResourceGroupNames(ResourceInfo[] resourceInfos)
        {
            HashSet<string> resourceGroupNames = new HashSet<string>();
            for (int i = 0; i < resourceInfos.Length; i++)
            {
                var resInfo = resourceInfos[i];
                foreach (string resourceGroup in resInfo.Resource.GetResourceGroups())
                {
                    resourceGroupNames.Add(resourceGroup);
                }
            }

            return resourceGroupNames.OrderBy(x => x).ToArray();
        }
        internal static int[] GetResourceIndexesFromResourceGroup(ResourceInfo[] resourceInfos, string resourceGroupName)
        {
            List<int> resourceIndexes = new List<int>();
            for (int i = 0; i < resourceInfos.Length; i++)
            {
                var resInfo = resourceInfos[i];
                foreach (string resourceGroup in resInfo.Resource.GetResourceGroups())
                {
                    if (resourceGroup == resourceGroupName)
                    {
                        resourceIndexes.Add(i);
                        break;
                    }
                }
            }

            resourceIndexes.Sort();
            return resourceIndexes.ToArray();
        }

        internal static ResourceInfo[] Sort(IEnumerable<ResourceInfo> resourceInfos)
        {
            SortedDictionary<string, ResourceInfo> resources = new SortedDictionary<string, ResourceInfo>();
            foreach (var item in resourceInfos)
            {
                resources[item.FullName] = item;
            }
            return resources.Values.ToArray();
        }

        #region UpdatableVersionList


        public static UpdatableVersionList CreateUpdatableVersionList(ResourceInfo[] resourceInfos, string applicableGameVersion, int internalResourceVersion, string variantPrefixUrl)
        {
            List<Asset> originAssets;
            Dictionary<Asset, int> originAssetIndexs;

            UpdatableVersionList.Asset[] assets;
            UpdatableVersionList.Resource[] resources;
            UpdatableVersionList.FileSystem[] fileSystems;
            UpdatableVersionList.ResourceGroup[] resourceGroups;

            //Origin
            originAssets = GetAssetIndexs(resourceInfos, out originAssetIndexs);


            //Asset
            assets = new UpdatableVersionList.Asset[originAssets.Count];
            int index = 0;
            foreach (var res in resourceInfos)
            {
                foreach (var asset in res.Resource.GetAssets())
                {
                    assets[index++] = new UpdatableVersionList.Asset(FixName(asset.Name), GetAssetIndexes(originAssetIndexs, asset.DependencyAssetPaths));
                }
            }


            //Resource
            resources = new UpdatableVersionList.Resource[resourceInfos.Length];

            for (int i = 0; i < resourceInfos.Length; i++)
            {
                var resInfo = resourceInfos[i];
                int[] assetIndexs = GetAssetIndexes(originAssetIndexs, resInfo.Resource.GetAssets().Select(o => o.AssetPath).ToArray());
                resources[i] = new UpdatableVersionList.Resource(FixName(resInfo.Name), FixName(resInfo.Variant), GetExtension(resInfo), (byte)resInfo.LoadType, resInfo.Length, resInfo.HashCode, resInfo.ZipLength, resInfo.ZipHashCode, assetIndexs);
            }

            //FileSystem
            SortedDictionary<string, List<int>> fileSystemIndexs = GetFileSystemIndexs(resourceInfos);
            fileSystems = new UpdatableVersionList.FileSystem[fileSystemIndexs.Count];
            index = 0;
            foreach (KeyValuePair<string, List<int>> i in fileSystemIndexs)
            {
                fileSystems[index++] = new UpdatableVersionList.FileSystem(FixName(i.Key), i.Value.ToArray());
            }

            //Group
            string[] resourceGroupNames = GetResourceGroupNames(resourceInfos);
            resourceGroups = new UpdatableVersionList.ResourceGroup[resourceGroupNames.Length];
            for (int i = 0; i < resourceGroups.Length; i++)
            {
                resourceGroups[i] = new UpdatableVersionList.ResourceGroup(resourceGroupNames[i], GetResourceIndexesFromResourceGroup(resourceInfos, resourceGroupNames[i]));
            }

            UpdatableVersionList versionList = new UpdatableVersionList(applicableGameVersion, internalResourceVersion, assets, resources, fileSystems, resourceGroups, variantPrefixUrl);

            return versionList;
        }

        #endregion

        public static LocalVersionList CreateLocalVersionList(ResourceInfo[] resourceInfos)
        {
            SortedDictionary<string, List<int>> fileSystemIndexs;
            LocalVersionList.Resource[] resourceList = new LocalVersionList.Resource[resourceInfos.Length];

            int index = 0;
            for (int i = 0; i < resourceInfos.Length; i++)
            {
                var resInfo = resourceInfos[i];
                //不能使用获取 Extension ，读取的 LocalVersionList 不包含 Asset
                resourceList[index] = new LocalVersionList.Resource(FixName(resInfo.Name), FixName(resInfo.Variant), resInfo.Extension, (byte)resInfo.LoadType, resInfo.Length, resInfo.HashCode);
                index++;
            }
            fileSystemIndexs = GetFileSystemIndexs(resourceInfos);

            LocalVersionList.FileSystem[] fileSystems = new LocalVersionList.FileSystem[fileSystemIndexs.Count];

            index = 0;
            foreach (KeyValuePair<string, List<int>> i in fileSystemIndexs)
            {
                fileSystems[index++] = new LocalVersionList.FileSystem(FixName(i.Key), i.Value.ToArray());
                i.Value.Clear();
            }

            LocalVersionList versionList = new LocalVersionList(resourceList, fileSystems);

            return versionList;
        }

        public static PackageVersionList CreatePackageVersionList(ResourceInfo[] resourceInfos, string applicableGameVersion, int internalResourceVersion, string variantPrefixUrl)
        {
            List<Asset> originAssets;
            Dictionary<Asset, int> originAssetIndexs;

            PackageVersionList.Asset[] assets;
            PackageVersionList.Resource[] resources;
            PackageVersionList.FileSystem[] fileSystems;
            PackageVersionList.ResourceGroup[] resourceGroups;

            //Origin
            originAssets = GetAssetIndexs(resourceInfos, out originAssetIndexs);


            //Asset
            assets = new PackageVersionList.Asset[originAssets.Count];
            int index = 0;
            foreach (var res in resourceInfos)
            {
                foreach (var asset in res.Resource.GetAssets())
                {
                    assets[index++] = new PackageVersionList.Asset(FixName(asset.Name), GetAssetIndexes(originAssetIndexs, asset.DependencyAssetPaths));
                }
            }


            //Resource
            resources = new PackageVersionList.Resource[resourceInfos.Length];
            for (int i = 0; i < resourceInfos.Length; i++)
            {
                var resInfo = resourceInfos[i];
                int[] assetIndexs = GetAssetIndexes(originAssetIndexs, resInfo.Resource.GetAssets().Select(o => o.AssetPath).ToArray());
                resources[i] = new PackageVersionList.Resource(FixName(resInfo.Name), FixName(resInfo.Variant), GetExtension(resInfo), (byte)resInfo.LoadType, resInfo.Length, resInfo.HashCode, assetIndexs);
            }

            //FileSystem
            SortedDictionary<string, List<int>> fileSystemIndexs = GetFileSystemIndexs(resourceInfos);
            fileSystems = new PackageVersionList.FileSystem[fileSystemIndexs.Count];
            index = 0;
            foreach (KeyValuePair<string, List<int>> i in fileSystemIndexs)
            {
                fileSystems[index++] = new PackageVersionList.FileSystem(FixName(i.Key), i.Value.ToArray());
            }

            //Group
            string[] resourceGroupNames = GetResourceGroupNames(resourceInfos);
            resourceGroups = new PackageVersionList.ResourceGroup[resourceGroupNames.Length];
            for (int i = 0; i < resourceGroups.Length; i++)
            {
                resourceGroups[i] = new PackageVersionList.ResourceGroup(resourceGroupNames[i], GetResourceIndexesFromResourceGroup(resourceInfos, resourceGroupNames[i]));
            }

            PackageVersionList versionList = new PackageVersionList(applicableGameVersion, internalResourceVersion, assets, resources, fileSystems, resourceGroups, variantPrefixUrl);

            return versionList;
        }


        /// <summary>
        /// GameFrameworkVersion.dat Full
        /// </summary>
        public static GameFrameworkSerializer<UpdatableVersionList> GetFullVersionSerializer()
        {
            var serializer = new UpdatableVersionListSerializer();
            serializer.RegisterDeserializeCallback(0, BuiltinVersionListSerializer.UpdatableVersionListDeserializeCallback_V0);
            serializer.RegisterDeserializeCallback(1, BuiltinVersionListSerializer.UpdatableVersionListDeserializeCallback_V1);
            serializer.RegisterDeserializeCallback(2, BuiltinVersionListSerializer.UpdatableVersionListDeserializeCallback_V2);

            serializer.RegisterSerializeCallback(0, BuiltinVersionListSerializer.UpdatableVersionListSerializeCallback_V0);
            serializer.RegisterSerializeCallback(1, BuiltinVersionListSerializer.UpdatableVersionListSerializeCallback_V1);
            serializer.RegisterSerializeCallback(2, BuiltinVersionListSerializer.UpdatableVersionListSerializeCallback_V2);

            return serializer;
        }


        /// <summary>
        /// GameFrameworkVersion.dat Package
        /// </summary>
        public static GameFrameworkSerializer<PackageVersionList> GetPackageVersionSerializer()
        {
            PackageVersionListSerializer serializer = new PackageVersionListSerializer();
            serializer.RegisterDeserializeCallback(0, BuiltinVersionListSerializer.PackageVersionListDeserializeCallback_V0);
            serializer.RegisterDeserializeCallback(1, BuiltinVersionListSerializer.PackageVersionListDeserializeCallback_V1);
            serializer.RegisterDeserializeCallback(2, BuiltinVersionListSerializer.PackageVersionListDeserializeCallback_V2);

            serializer.RegisterSerializeCallback(0, BuiltinVersionListSerializer.PackageVersionListSerializeCallback_V0);
            serializer.RegisterSerializeCallback(1, BuiltinVersionListSerializer.PackageVersionListSerializeCallback_V1);
            serializer.RegisterSerializeCallback(2, BuiltinVersionListSerializer.PackageVersionListSerializeCallback_V2);

            return serializer;
        }

        /// <summary>
        /// GameFrameworkList.dat, Packed, streamingAssetsPath
        /// </summary>
        public static GameFrameworkSerializer<LocalVersionList> GetPackedVersionSerializer()
        {
            var serializer = new ReadOnlyVersionListSerializer();
            serializer.RegisterDeserializeCallback(0, BuiltinVersionListSerializer.LocalVersionListDeserializeCallback_V0);
            serializer.RegisterDeserializeCallback(1, BuiltinVersionListSerializer.LocalVersionListDeserializeCallback_V1);
            serializer.RegisterDeserializeCallback(2, BuiltinVersionListSerializer.LocalVersionListDeserializeCallback_V2);

            serializer.RegisterSerializeCallback(0, BuiltinVersionListSerializer.LocalVersionListSerializeCallback_V0);
            serializer.RegisterSerializeCallback(1, BuiltinVersionListSerializer.LocalVersionListSerializeCallback_V1);
            serializer.RegisterSerializeCallback(2, BuiltinVersionListSerializer.LocalVersionListSerializeCallback_V2);
            return serializer;
        }

        /// <summary>
        /// GameFrameworkList.dat, persistentDataPath
        /// </summary>
        /// <returns></returns>
        public static GameFrameworkSerializer<LocalVersionList> GetReadWriteVersionSerializer()
        {
            var serializer = new ReadWriteVersionListSerializer();
            serializer.RegisterSerializeCallback(0, BuiltinVersionListSerializer.LocalVersionListSerializeCallback_V0);
            serializer.RegisterSerializeCallback(1, BuiltinVersionListSerializer.LocalVersionListSerializeCallback_V1);
            serializer.RegisterSerializeCallback(2, BuiltinVersionListSerializer.LocalVersionListSerializeCallback_V2);

            serializer.RegisterDeserializeCallback(0, BuiltinVersionListSerializer.LocalVersionListDeserializeCallback_V0);
            serializer.RegisterDeserializeCallback(1, BuiltinVersionListSerializer.LocalVersionListDeserializeCallback_V1);
            serializer.RegisterDeserializeCallback(2, BuiltinVersionListSerializer.LocalVersionListDeserializeCallback_V2);

            return serializer;
        }

        public static void Compare(IEnumerable<ResourceInfo> items, IEnumerable<ResourceInfo> compareTo, List<ResourceInfo> changed, List<ResourceInfo> added, List<ResourceInfo> removed)
        {
            Dictionary<ResourceManager.ResourceName, ResourceInfo> map = new Dictionary<ResourceManager.ResourceName, ResourceInfo>();
            Dictionary<ResourceManager.ResourceName, ResourceInfo> toMap = new Dictionary<ResourceManager.ResourceName, ResourceInfo>();
            foreach (var resource in items)
            {
                map[resource.ResourceName] = resource;
            }
            foreach (var resource in compareTo)
            {
                toMap[resource.ResourceName] = resource;
            }

            foreach (var item in map)
            {
                var resource = item.Value;
                var key = item.Key;

                if (!toMap.ContainsKey(key))
                {
                    added.Add(resource);
                }
                else if (resource.HashCode != toMap[key].HashCode)
                {
                    changed.Add(resource);
                }
            }

            foreach (var item in toMap)
            {
                var resource = item.Value;
                var key = item.Key;
                if (!map.ContainsKey(key))
                {
                    removed.Add(resource);
                }
            }

        }

    }
}
