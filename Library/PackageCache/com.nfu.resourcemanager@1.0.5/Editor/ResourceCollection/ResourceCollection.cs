//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using ND.Managers.ResourceMgr.Framework;
using UnityEditor;
using UnityEngine;

namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{
    /// <summary>
    /// 资源集合。保存位置 ResourceCollection.xml
    /// </summary>
    public sealed class ResourceCollection
    {
        private const string SceneExtension = ".unity";
        private static readonly Regex ResourceNameRegex = new Regex(@"^([A-Za-z0-9\._\$-]+/)*[A-Za-z0-9\._\$-]+$");
        private static readonly Regex ResourceVariantRegex = new Regex(@"^[a-z0-9_-]+$");

        private readonly string m_ConfigurationPath;
        private readonly SortedDictionary<string, Resource> m_Resources;
        private readonly SortedDictionary<string, Asset> m_Assets;

        /// <summary>
        /// 是否开启中文字符替换为 '_'
        /// </summary>
        private bool safeNameEnabled = true;

        /// <summary>
        /// 批量一次性保存，提高性能
        /// </summary>
        static bool isSaveAssets;

        public ResourceCollection()
        {
            m_ConfigurationPath = Type.GetConfigurationPath<ResourceCollectionConfigPathAttribute>() ?? Utility.Path.GetRegularPath(Path.GetFullPath(
                "Packages/com.nfu.resourcemanager/Configs/ResourceCollection.xml"));
            m_Resources = new SortedDictionary<string, Resource>(StringComparer.Ordinal);
            m_Assets = new SortedDictionary<string, Asset>(StringComparer.Ordinal);
        }

        public int ResourceCount
        {
            get
            {
                return m_Resources.Count;
            }
        }

        public int AssetCount
        {
            get
            {
                return m_Assets.Count;
            }
        }




        public event GameFrameworkAction<int, int> OnLoadingResource = null;

        public event GameFrameworkAction<int, int> OnLoadingAsset = null;

        public event GameFrameworkAction OnLoadCompleted = null;

        /// <summary>
        /// 清除资源列表
        /// </summary>
        public void Clear()
        {
            m_Resources.Clear();
            m_Assets.Clear();
        }

        /// <summary>
        /// 加载资源列表
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {

            Clear();

            if (!File.Exists(m_ConfigurationPath))
            {
                return false;
            }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(m_ConfigurationPath);
                XmlNode xmlRoot = xmlDocument.SelectSingleNode("UnityGameFramework");
                XmlNode xmlCollection = xmlRoot.SelectSingleNode("ResourceCollection");
                XmlNode xmlResources = xmlCollection.SelectSingleNode("Resources");
                XmlNode xmlAssets = xmlCollection.SelectSingleNode("Assets");

                XmlNodeList xmlNodeList = null;
                XmlNode xmlNode = null;
                int count = 0;

                xmlNodeList = xmlResources.ChildNodes;
                count = xmlNodeList.Count;
                for (int i = 0; i < count; i++)
                {
                    if (OnLoadingResource != null)
                    {
                        OnLoadingResource(i, count);
                    }

                    xmlNode = xmlNodeList.Item(i);
                    if (xmlNode.Name != "Resource")
                    {
                        continue;
                    }

                    string name = xmlNode.Attributes.GetNamedItem("Name").Value;
                    string variant = xmlNode.Attributes.GetNamedItem("Variant") != null ? xmlNode.Attributes.GetNamedItem("Variant").Value : null;
                    string fileSystem = xmlNode.Attributes.GetNamedItem("FileSystem") != null ? xmlNode.Attributes.GetNamedItem("FileSystem").Value : null;
                    byte loadType = 0;
                    if (xmlNode.Attributes.GetNamedItem("LoadType") != null)
                    {
                        byte.TryParse(xmlNode.Attributes.GetNamedItem("LoadType").Value, out loadType);
                    }

                    bool packed = false;
                    if (xmlNode.Attributes.GetNamedItem("Packed") != null)
                    {
                        bool.TryParse(xmlNode.Attributes.GetNamedItem("Packed").Value, out packed);
                    }

                    string[] resourceGroups = xmlNode.Attributes.GetNamedItem("ResourceGroups") != null ? xmlNode.Attributes.GetNamedItem("ResourceGroups").Value.Split(',') : null;
                    if (!AddResource(name, variant, fileSystem, (LoadType)loadType, packed, resourceGroups, toLower: false))
                    {
                        Debug.LogWarning(Utility.Text.Format("Can not add resource '{0}'.", GetResourceFullName(name, variant)));
                        continue;
                    }
                }

                xmlNodeList = xmlAssets.ChildNodes;
                count = xmlNodeList.Count;
                for (int i = 0; i < count; i++)
                {
                    if (OnLoadingAsset != null)
                    {
                        OnLoadingAsset(i, count);
                    }

                    xmlNode = xmlNodeList.Item(i);
                    if (xmlNode.Name != "Asset")
                    {
                        continue;
                    }

                    string guid = xmlNode.Attributes.GetNamedItem("Guid").Value;
                    string name = xmlNode.Attributes.GetNamedItem("ResourceName").Value;
                    string variant = xmlNode.Attributes.GetNamedItem("ResourceVariant") != null ? xmlNode.Attributes.GetNamedItem("ResourceVariant").Value : null;
                    if (!AssignAsset(guid, name, variant))
                    {
                        Debug.LogWarning(Utility.Text.Format("Can not assign asset '{0}' to resource '{1}'.", guid, GetResourceFullName(name, variant)));
                        continue;
                    }
                }

                if (OnLoadCompleted != null)
                {
                    OnLoadCompleted();
                }

                return true;
            }
            catch
            {
                File.Delete(m_ConfigurationPath);
                if (OnLoadCompleted != null)
                {
                    OnLoadCompleted();
                }

                return false;
            }

        }
        /// <summary>
        /// 保存分析结果
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));

                XmlElement xmlRoot = xmlDocument.CreateElement("UnityGameFramework");
                xmlDocument.AppendChild(xmlRoot);

                XmlElement xmlCollection = xmlDocument.CreateElement("ResourceCollection");
                xmlRoot.AppendChild(xmlCollection);

                XmlElement xmlResources = xmlDocument.CreateElement("Resources");
                xmlCollection.AppendChild(xmlResources);

                XmlElement xmlAssets = xmlDocument.CreateElement("Assets");
                xmlCollection.AppendChild(xmlAssets);

                XmlElement xmlElement = null;
                XmlAttribute xmlAttribute = null;

                foreach (Resource resource in m_Resources.Values)
                {
                    xmlElement = xmlDocument.CreateElement("Resource");
                    xmlAttribute = xmlDocument.CreateAttribute("Name");
                    xmlAttribute.Value = resource.Name;
                    xmlElement.Attributes.SetNamedItem(xmlAttribute);

                    if (resource.Variant != null)
                    {
                        xmlAttribute = xmlDocument.CreateAttribute("Variant");
                        xmlAttribute.Value = resource.Variant;
                        xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    }

                    if (resource.FileSystem != null)
                    {
                        xmlAttribute = xmlDocument.CreateAttribute("FileSystem");
                        xmlAttribute.Value = resource.FileSystem;
                        xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    }

                    xmlAttribute = xmlDocument.CreateAttribute("LoadType");
                    xmlAttribute.Value = ((byte)resource.LoadType).ToString();
                    xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    xmlAttribute = xmlDocument.CreateAttribute("Packed");
                    xmlAttribute.Value = resource.Packed.ToString();
                    xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    string[] resourceGroups = resource.GetResourceGroups();
                    if (resourceGroups.Length > 0)
                    {
                        xmlAttribute = xmlDocument.CreateAttribute("ResourceGroups");
                        xmlAttribute.Value = string.Join(",", resourceGroups);
                        xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    }

                    xmlResources.AppendChild(xmlElement);
                }

                foreach (Asset asset in m_Assets.Values)
                {
                    xmlElement = xmlDocument.CreateElement("Asset");
                    xmlAttribute = xmlDocument.CreateAttribute("Guid");
                    xmlAttribute.Value = asset.Guid;
                    xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    xmlAttribute = xmlDocument.CreateAttribute("ResourceName");
                    xmlAttribute.Value = asset.Resource.Name;
                    xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    if (asset.Resource.Variant != null)
                    {
                        xmlAttribute = xmlDocument.CreateAttribute("ResourceVariant");
                        xmlAttribute.Value = asset.Resource.Variant;
                        xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    }

                    xmlAssets.AppendChild(xmlElement);
                }

                string configurationDirectoryName = Path.GetDirectoryName(m_ConfigurationPath);
                if (!Directory.Exists(configurationDirectoryName))
                {
                    Directory.CreateDirectory(configurationDirectoryName);
                }

                xmlDocument.Save(m_ConfigurationPath);
                AssetDatabase.Refresh();
                return true;
            }
            catch
            {
                if (File.Exists(m_ConfigurationPath))
                {
                    File.Delete(m_ConfigurationPath);
                }

                return false;
            }
        }

        /// <summary>
        /// 获取安全的资源名称
        /// </summary>
        public void GetSafeBundleName(ref string bundleName, ref string variant, ref string fileSystem, bool toLower = true)
        {
            if (!safeNameEnabled)
                return;

            if (bundleName != null)
                bundleName = EditorUtilityx.GetSafeBundleName(bundleName, toLower: toLower);
            if (variant != null)
                variant = EditorUtilityx.GetSafeChar(variant);
            if (fileSystem != null)
                fileSystem = EditorUtilityx.GetSafeBundleName(fileSystem, toLower: toLower);
        }

        public void GetSafeBundleName(ref string bundleName, ref string variant, bool toLower = true)
        {
            if (!safeNameEnabled)
                return;
            if (bundleName != null)
                bundleName = EditorUtilityx.GetSafeBundleName(bundleName, toLower: toLower);
            if (variant != null)
                variant = EditorUtilityx.GetSafeChar(variant);
        }
        public void GetSafeBundleName(ref string bundleName, bool toLower = true)
        {
            if (!safeNameEnabled)
                return;
            if (bundleName != null)
                bundleName = EditorUtilityx.GetSafeBundleName(bundleName, toLower: toLower);
        }
        /// <summary>
        /// 获取所有资源包
        /// </summary>
        public Resource[] GetResources()
        {
            return m_Resources.Values.ToArray();
        }
        /// <summary>
        /// 获取资源包
        /// </summary>
        public Resource GetResource(string name, string variant)
        {
            GetSafeBundleName(ref name, ref variant);
            if (!IsValidResourceName(name, variant))
            {
                return null;
            }

            Resource resource = null;
            if (m_Resources.TryGetValue(GetResourceFullName(name, variant).ToLower(), out resource))
            {
                return resource;
            }

            return null;
        }


        /// <summary>
        /// 是否存在资源包
        /// </summary>
        public bool HasResource(string name, string variant)
        {
            GetSafeBundleName(ref name, ref variant);
            if (!IsValidResourceName(name, variant))
            {
                return false;
            }

            return m_Resources.ContainsKey(GetResourceFullName(name, variant).ToLower());
        }
        /// <summary>
        /// 添加资源包
        /// </summary>
        public bool AddResource(string name, string variant, string fileSystem, LoadType loadType, bool packed, bool toLower = true)
        {
            return AddResource(name, variant, fileSystem, loadType, packed, null, toLower: toLower);
        }

        public bool AddResource(string name, string variant, string fileSystem, LoadType loadType, bool packed, string[] resourceGroups, bool toLower = true)
        {
            GetSafeBundleName(ref name, ref variant, ref fileSystem, toLower: toLower);

            if (!IsValidResourceName(name, variant))
            {

                for (int i = 0; i < name.Length; i++)
                {
                    if (name[i] == '/')
                        continue;
                    if (!ResourceNameRegex.IsMatch(name[i].ToString()))
                        Debug.LogWarning($"resource '{name}', index: {i}, char {name[i]}");
                }
                if (variant != null)
                {
                    for (int i = 0; i < variant.Length; i++)
                    {
                        if (!ResourceVariantRegex.IsMatch(variant[i].ToString()))
                            Debug.LogWarning($"variant '{variant}', index: {i}, char {variant[i]}");
                    }
                }
                return false;
            }

            if (!IsAvailableResourceName(name, variant, null))
            {
                return false;
            }

            if (fileSystem != null && !ResourceNameRegex.IsMatch(fileSystem))
            {
                return false;
            }

            Resource resource = Resource.Create(name, variant, fileSystem, loadType, packed, resourceGroups);
            m_Resources.Add(resource.FullName.ToLower(), resource);

            return true;
        }
        /// <summary>
        /// 重命名资源包
        /// </summary>
        public bool RenameResource(string oldName, string oldVariant, string newName, string newVariant)
        {
            GetSafeBundleName(ref oldName, ref oldVariant);
            GetSafeBundleName(ref newName, ref newVariant);

            if (!IsValidResourceName(oldName, oldVariant) || !IsValidResourceName(newName, newVariant))
            {
                return false;
            }

            Resource resource = GetResource(oldName, oldVariant);
            if (resource == null)
            {
                return false;
            }

            if (oldName == newName && oldVariant == newVariant)
            {
                return true;
            }

            if (!IsAvailableResourceName(newName, newVariant, resource))
            {
                return false;
            }

            m_Resources.Remove(resource.FullName.ToLower());
            resource.Rename(newName, newVariant);
            m_Resources.Add(resource.FullName.ToLower(), resource);

            return true;
        }
        /// <summary>
        /// 移除资源包
        /// </summary>
        public bool RemoveResource(string name, string variant)
        {
            GetSafeBundleName(ref name, ref variant);

            if (!IsValidResourceName(name, variant))
            {
                return false;
            }

            Resource resource = GetResource(name, variant);
            if (resource == null)
            {
                return false;
            }

            IEnumerable<Asset> assets = resource.ToArray();
            resource.Clear();
            m_Resources.Remove(resource.FullName.ToLower());
            foreach (Asset asset in assets)
            {
                m_Assets.Remove(asset.Guid);
            }

            return true;
        }
        /// <summary>
        /// 设置资源加载类型
        /// </summary>
        public bool SetResourceLoadType(string name, string variant, LoadType loadType)
        {
            GetSafeBundleName(ref name, ref variant);

            if (!IsValidResourceName(name, variant))
            {
                return false;
            }

            Resource resource = GetResource(name, variant);
            if (resource == null)
            {
                return false;
            }

            if ((loadType == LoadType.LoadFromBinary || loadType == LoadType.LoadFromBinaryAndQuickDecrypt || loadType == LoadType.LoadFromBinaryAndDecrypt) && resource.Count > 1)
            {
                return false;
            }

            resource.LoadType = loadType;
            return true;
        }

        public bool SetResourcePacked(string name, string variant, bool packed)
        {
            GetSafeBundleName(ref name, ref variant);

            if (!IsValidResourceName(name, variant))
            {
                return false;
            }

            Resource resource = GetResource(name, variant);
            if (resource == null)
            {
                return false;
            }

            resource.Packed = packed;
            return true;
        }
        /// <summary>
        /// 获取所有资源
        /// </summary>
        public Asset[] GetAssets()
        {
            return m_Assets.Values.ToArray();
        }
        /// <summary>
        /// 获取资源包内的资源
        /// </summary>
        public Asset[] GetAssets(string name, string variant)
        {
            if (!IsValidResourceName(name, variant))
            {
                return new Asset[0];
            }

            Resource resource = GetResource(name, variant);
            if (resource == null)
            {
                return new Asset[0];
            }

            return resource.ToArray();
        }

        public Asset GetAsset(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }

            Asset asset = null;
            if (m_Assets.TryGetValue(guid, out asset))
            {
                return asset;
            }

            return null;
        }

        public bool HasAsset(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return false;
            }

            return m_Assets.ContainsKey(guid);
        }
        /// <summary>
        /// 添加资源
        /// </summary>
        public bool AssignAsset(string guid, string name, string variant)
        {
            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogWarning("AssignAsset false, guid null, " + guid + ", name: " + name + ", variant: " + variant);
                return false;
            }

            GetSafeBundleName(ref name, ref variant);

            if (!IsValidResourceName(name, variant))
            {
                Debug.LogWarning("AssignAsset false, IsValidResourceName false, " + guid + ", name: " + name + ", variant: " + variant);
                return false;
            }

            Resource resource = GetResource(name, variant);
            if (resource == null)
            {
                Debug.LogWarning("AssignAsset false, Resource null, " + guid + ", name: " + name + ", variant: " + variant);
                return false;
            }

            string assetName = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(assetName))
            {
                Debug.LogWarning("AssignAsset false, assetName null, " + guid + ", name: " + name + ", variant: " + variant);
                return false;
            }

            IEnumerable<Asset> assetsInResource = resource;
            string lowerAssetName = assetName.ToLower();

            if (lowerAssetName != assetName && resource.GetAssetByAssetPathIgnoreCase(lowerAssetName) != null)
            {
                Debug.LogWarning("AssignAsset false, assetInResource.Name.ToLower() == assetName.ToLower() , " + guid + ", name: " + name + ", variant: " + variant);
                return false;
            }


            bool isScene = assetName.EndsWith(SceneExtension, StringComparison.Ordinal);
            if (isScene && resource.AssetType == AssetType.Asset || !isScene && resource.AssetType == AssetType.Scene)
            {
                Debug.LogWarning("AssignAsset false, Asset Type: Scene , " + guid + ", name: " + name + ", variant: " + variant);
                return false;
            }

            Asset asset = GetAsset(guid);
            if (resource.IsLoadFromBinary && resource.Count > 0 && asset != assetsInResource.First())
            {
                Debug.LogWarning("AssignAsset false, IsLoadFromBinary , " + guid + ", name: " + name + ", variant: " + variant);
                return false;
            }

            if (asset == null)
            {
                asset = Asset.Create(guid);
                m_Assets.Add(asset.Guid, asset);
            }

            resource.AssignAsset(asset, isScene);

            return true;
        }

        /// <summary>
        /// 移除资源
        /// </summary>
        public bool UnassignAsset(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return false;
            }

            Asset asset = GetAsset(guid);
            if (asset != null)
            {
                asset.Resource.UnassignAsset(asset);
                m_Assets.Remove(asset.Guid);
            }

            return true;
        }

        private string GetResourceFullName(string name, string variant)
        {
            return !string.IsNullOrEmpty(variant) ? Utility.Text.Format("{0}.{1}", name, variant) : name;
        }
        /// <summary>
        /// 资源包名称是否合规
        /// </summary>
        internal static bool IsValidResourceName(string name, string variant)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (!ResourceNameRegex.IsMatch(name))
            {
                return false;
            }

            if (variant != null && !ResourceVariantRegex.IsMatch(variant))
            {
                return false;
            }

            return true;
        }

        private bool IsAvailableResourceName(string name, string variant, Resource current)
        {
            Resource found = GetResource(name, variant);
            if (found != null && found != current)
            {
                return false;
            }

            string[] foundPathNames = name.Split('/');
            foreach (Resource resource in m_Resources.Values)
            {
                if (current != null && resource == current)
                {
                    continue;
                }

                if (resource.Name == name)
                {
                    //检查变体
                    if (resource.Variant == null && variant != null)
                    {
                        return false;
                    }

                    if (resource.Variant != null && variant == null)
                    {
                        return false;
                    }
                }

                //检查当前资源包名是其它资源包名的路径文件夹
                if (resource.Name.Length > name.Length
                    && resource.Name[name.Length] == '/'
                    && resource.Name.StartsWith(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return false;
                }

                //检查其它资源包名是当前资源包名的路径文件夹
                if (name.Length > resource.Name.Length
                    && name[resource.Name.Length] == '/'
                    && name.StartsWith(resource.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return false;
                }

                string[] pathNames = resource.PathNames;
                for (int i = 0; i < foundPathNames.Length - 1 && i < pathNames.Length - 1; i++)
                {
                    if (foundPathNames[i].ToLower() != pathNames[i].ToLower())
                    {
                        break;
                    }

                    if (foundPathNames[i] != pathNames[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public Dictionary<string, string[]> GetAssetBundleNames()
        {
            Dictionary<string, string[]> assetBundleNames = new Dictionary<string, string[]>();

            foreach (Resource resource in GetResources())
            {
                if (resource.IsLoadFromBinary)
                {
                    continue;
                }

                foreach (Asset asset in resource)
                {
                    string assetPath = asset.Name;
                    string guid = AssetDatabase.AssetPathToGUID(assetPath);
                    AssetImporter assetImporter = AssetImporter.GetAtPath(asset.Name);

                    assetBundleNames[guid] = new string[] { resource.Name, resource.Variant };
                }
            }
            return assetBundleNames;
        }

        public void UpdateAllAssetBundleNameAndVariant()
        {
            using (new PerformanceSample($"{nameof(ResourceCollection)}.{nameof(UpdateAllAssetBundleNameAndVariant)}"))
            {
                Dictionary<string, string[]> assetBundleNames = GetAssetBundleNames();
                UpdateAllAssetBundleNameAndVariant(assetBundleNames);
            }
        }

        public void UpdateAllAssetBundleNameAndVariant(Dictionary<string, string[]> assetBundleNames)
        {
            HashSet<string> used = new HashSet<string>();
            bool changed = false;

            foreach (var item in assetBundleNames)
            {
                string guid = item.Key;
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string bundleName = item.Value[0];
                string variant = item.Value[1];
                used.Add(guid);
                AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
                if (assetImporter == null)
                    throw new Exception($"set AssetImporter assetBundleName error, assetPath: {assetPath}, assetbundle: {bundleName}");
                if (SetAssetBundleNameAndVariant(assetImporter, bundleName, variant))
                {
                    changed = true;
                }
            }

            //移除未使用的
            foreach (var bundleName in AssetDatabase.GetAllAssetBundleNames())
            {
                foreach (var assetPath in AssetDatabase.GetAssetPathsFromAssetBundle(bundleName))
                {
                    string guid = AssetDatabase.AssetPathToGUID(assetPath);
                    if (!used.Contains(guid))
                    {
                        AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
                        if (SetAssetBundleNameAndVariant(assetImporter, null, null))
                        {
                            changed = true;
                        }
                    }
                }
            }

            AssetDatabase.RemoveUnusedAssetBundleNames();

            if (changed)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        bool SetAssetBundleNameAndVariant(AssetImporter assetImporter, string assetBundleName, string variant)
        {
            bool changed = false;
            if (assetBundleName == null)
                assetBundleName = string.Empty;
            if (variant == null)
                variant = string.Empty;
            if (assetImporter.assetBundleName != assetBundleName || assetImporter.assetBundleVariant != variant)
            {
                assetImporter.SetAssetBundleNameAndVariant(assetBundleName, variant);
                changed = true;
                DelaySaveAssets();
            }
            return changed;
        }


        public static void DelaySaveAssets()
        {
            isSaveAssets = true;
            EditorApplication.delayCall += () =>
            {
                if (!isSaveAssets)
                    return;
                isSaveAssets = false;
                AssetDatabase.SaveAssets();
            };
        }

    }
}