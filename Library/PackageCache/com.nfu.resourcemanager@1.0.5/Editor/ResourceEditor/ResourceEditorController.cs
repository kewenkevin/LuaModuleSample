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
using System.Xml;
using ND.Managers.ResourceMgr.Editor.ResourceDependencyAnalyzer;
using ND.Managers.ResourceMgr.Framework;
using ND.Managers.ResourceMgr.Runtime;
using UnityEditor;

namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{
    /// <summary>
    /// 资源分析控制器，配置文件：ResourceEditor.xml
    /// </summary>
    public sealed class ResourceEditorController
    {
        private const string DefaultSourceAssetRootPath = "Assets";

        private readonly string m_ConfigurationPath;
        private readonly ResourceCollection m_ResourceCollection;
        private readonly List<string> m_SourceAssetSearchPaths;
        private readonly List<string> m_SourceAssetSearchRelativePaths;
        private readonly Dictionary<string, SourceAsset> m_SourceAssets;
        private SourceFolder m_SourceAssetRoot;
        private string m_SourceAssetRootPath;
        private string m_SourceAssetUnionTypeFilter;
        private string m_SourceAssetUnionLabelFilter;
        private string m_SourceAssetExceptTypeFilter;
        private string m_SourceAssetExceptLabelFilter;
        private AssetSorterType m_AssetSorter;
        private bool m_AutoSave;

        public ResourceEditorController()
        {
            m_ConfigurationPath = Type.GetConfigurationPath<ResourceEditorConfigPathAttribute>() ?? Utility.Path.GetRegularPath(Path.GetFullPath(
                "Packages/com.nfu.resourcemanager/Configs/ResourceEditor.xml"));
            m_ResourceCollection = new ResourceCollection();
            m_ResourceCollection.OnLoadingResource += delegate (int index, int count)
            {
                if (OnLoadingResource != null)
                {
                    OnLoadingResource(index, count);
                }
            };

            m_ResourceCollection.OnLoadingAsset += delegate (int index, int count)
            {
                if (OnLoadingAsset != null)
                {
                    OnLoadingAsset(index, count);
                }
            };

            m_ResourceCollection.OnLoadCompleted += delegate ()
            {
                if (OnLoadCompleted != null)
                {
                    OnLoadCompleted();
                }
            };

            m_SourceAssetSearchPaths = new List<string>();
            m_SourceAssetSearchRelativePaths = new List<string>();
            m_SourceAssets = new Dictionary<string, SourceAsset>(StringComparer.Ordinal);
            m_SourceAssetRoot = null;
            m_SourceAssetRootPath = null;
            m_SourceAssetUnionTypeFilter = null;
            m_SourceAssetUnionLabelFilter = null;
            m_SourceAssetExceptTypeFilter = null;
            m_SourceAssetExceptLabelFilter = null;
            m_AssetSorter = AssetSorterType.Path;

            SourceAssetRootPath = DefaultSourceAssetRootPath;
        }

        public int ResourceCount
        {
            get
            {
                return m_ResourceCollection.ResourceCount;
            }
        }

        public int AssetCount
        {
            get
            {
                return m_ResourceCollection.AssetCount;
            }
        }

        public string AnalysisHandlerName
        {
            get;
            set;
        }

        public SourceFolder SourceAssetRoot
        {
            get
            {
                return m_SourceAssetRoot;
            }
        }

        public string SourceAssetRootPath
        {
            get
            {
                return m_SourceAssetRootPath;
            }
            set
            {
                if (m_SourceAssetRootPath == value)
                {
                    return;
                }

                m_SourceAssetRootPath = value.Replace('\\', '/');
                m_SourceAssetRoot = new SourceFolder(m_SourceAssetRootPath, null);
                RefreshSourceAssetSearchPaths();
            }
        }

        public string SourceAssetUnionTypeFilter
        {
            get
            {
                return m_SourceAssetUnionTypeFilter;
            }
            set
            {
                if (m_SourceAssetUnionTypeFilter == value)
                {
                    return;
                }

                m_SourceAssetUnionTypeFilter = value;
            }
        }

        public string SourceAssetUnionLabelFilter
        {
            get
            {
                return m_SourceAssetUnionLabelFilter;
            }
            set
            {
                if (m_SourceAssetUnionLabelFilter == value)
                {
                    return;
                }

                m_SourceAssetUnionLabelFilter = value;
            }
        }

        public string SourceAssetExceptTypeFilter
        {
            get
            {
                return m_SourceAssetExceptTypeFilter;
            }
            set
            {
                if (m_SourceAssetExceptTypeFilter == value)
                {
                    return;
                }

                m_SourceAssetExceptTypeFilter = value;
            }
        }

        public string SourceAssetExceptLabelFilter
        {
            get
            {
                return m_SourceAssetExceptLabelFilter;
            }
            set
            {
                if (m_SourceAssetExceptLabelFilter == value)
                {
                    return;
                }

                m_SourceAssetExceptLabelFilter = value;
            }
        }

        public AssetSorterType AssetSorter
        {
            get
            {
                return m_AssetSorter;
            }
            set
            {
                if (m_AssetSorter == value)
                {
                    return;
                }

                m_AssetSorter = value;
            }
        }
        /// <summary>
        /// 清理或分析结束自动保存
        /// </summary>
        public bool AutoSave
        {
            get => m_AutoSave;
            set => m_AutoSave = value;
        }

        public ResourceCollection ResourceCollection { get => m_ResourceCollection; }

        public event GameFrameworkAction<int, int> OnLoadingResource = null;

        public event GameFrameworkAction<int, int> OnLoadingAsset = null;

        public event GameFrameworkAction OnLoadCompleted = null;

        public event GameFrameworkAction<SourceAsset[]> OnAssetAssigned = null;

        public event GameFrameworkAction<SourceAsset[]> OnAssetUnassigned = null;

        /// <summary>
        /// 加载分析配置
        /// </summary> 
        public bool LoadConfig()
        {
            if (!File.Exists(m_ConfigurationPath))
            {
                return false;
            }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(m_ConfigurationPath);
                XmlNode xmlRoot = xmlDocument.SelectSingleNode("UnityGameFramework");
                XmlNode xmlEditor = xmlRoot.SelectSingleNode("ResourceEditor");
                XmlNode xmlSettings = xmlEditor.SelectSingleNode("Settings");

                XmlNodeList xmlNodeList = null;
                XmlNode xmlNode = null;

                xmlNodeList = xmlSettings.ChildNodes;
                for (int i = 0; i < xmlNodeList.Count; i++)
                {
                    xmlNode = xmlNodeList.Item(i);
                    switch (xmlNode.Name)
                    {
                        case "SourceAssetRootPath":
                            SourceAssetRootPath = xmlNode.InnerText;
                            break;

                        case "SourceAssetSearchPaths":
                            m_SourceAssetSearchRelativePaths.Clear();
                            XmlNodeList xmlNodeListInner = xmlNode.ChildNodes;
                            XmlNode xmlNodeInner = null;
                            for (int j = 0; j < xmlNodeListInner.Count; j++)
                            {
                                xmlNodeInner = xmlNodeListInner.Item(j);
                                if (xmlNodeInner.Name != "SourceAssetSearchPath")
                                {
                                    continue;
                                }

                                m_SourceAssetSearchRelativePaths.Add(xmlNodeInner.Attributes.GetNamedItem("RelativePath").Value);
                            }
                            break;

                        case "SourceAssetUnionTypeFilter":
                            SourceAssetUnionTypeFilter = xmlNode.InnerText;
                            break;

                        case "SourceAssetUnionLabelFilter":
                            SourceAssetUnionLabelFilter = xmlNode.InnerText;
                            break;

                        case "SourceAssetExceptTypeFilter":
                            SourceAssetExceptTypeFilter = xmlNode.InnerText;
                            break;

                        case "SourceAssetExceptLabelFilter":
                            SourceAssetExceptLabelFilter = xmlNode.InnerText;
                            break;
                        case "AnalysisHandler":
                            AnalysisHandlerName = xmlNode.InnerText;
                            break;

                        case "AssetSorter":
                            AssetSorter = (AssetSorterType)Enum.Parse(typeof(AssetSorterType), xmlNode.InnerText);
                            break;
                        case "AutoSave":
                            AutoSave = bool.Parse(xmlNode.InnerText);
                            break;
                    }
                }

                RefreshSourceAssetSearchPaths();
            }
            catch
            {
                File.Delete(m_ConfigurationPath);
                return false;
            }
            return true;
        }

        public bool Load()
        {

            if (!LoadConfig())
                return false;

            ScanSourceAssets();

            return true;
        }

        /// <summary>
        /// 保存分析配置和分析结果
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

                XmlElement xmlEditor = xmlDocument.CreateElement("ResourceEditor");
                xmlRoot.AppendChild(xmlEditor);

                XmlElement xmlSettings = xmlDocument.CreateElement("Settings");
                xmlEditor.AppendChild(xmlSettings);

                XmlElement xmlElement = null;
                XmlAttribute xmlAttribute = null;

                xmlElement = xmlDocument.CreateElement("SourceAssetRootPath");
                xmlElement.InnerText = SourceAssetRootPath.ToString();
                xmlSettings.AppendChild(xmlElement);

                xmlElement = xmlDocument.CreateElement("SourceAssetSearchPaths");
                xmlSettings.AppendChild(xmlElement);

                foreach (string sourceAssetSearchRelativePath in m_SourceAssetSearchRelativePaths)
                {
                    XmlElement xmlElementInner = xmlDocument.CreateElement("SourceAssetSearchPath");
                    xmlAttribute = xmlDocument.CreateAttribute("RelativePath");
                    xmlAttribute.Value = sourceAssetSearchRelativePath;
                    xmlElementInner.Attributes.SetNamedItem(xmlAttribute);
                    xmlElement.AppendChild(xmlElementInner);
                }

                xmlElement = xmlDocument.CreateElement("AnalysisHandler");
                xmlElement.InnerText = AnalysisHandlerName;
                xmlSettings.AppendChild(xmlElement);



                xmlElement = xmlDocument.CreateElement("SourceAssetUnionTypeFilter");
                xmlElement.InnerText = SourceAssetUnionTypeFilter ?? string.Empty;
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("SourceAssetUnionLabelFilter");
                xmlElement.InnerText = SourceAssetUnionLabelFilter ?? string.Empty;
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("SourceAssetExceptTypeFilter");
                xmlElement.InnerText = SourceAssetExceptTypeFilter ?? string.Empty;
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("SourceAssetExceptLabelFilter");
                xmlElement.InnerText = SourceAssetExceptLabelFilter ?? string.Empty;
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("AssetSorter");
                xmlElement.InnerText = AssetSorter.ToString();
                xmlSettings.AppendChild(xmlElement);

                xmlSettings.SetNodeValue("AutoSave", AutoSave.ToString());

                string configurationDirectoryName = Path.GetDirectoryName(m_ConfigurationPath);
                if (!Directory.Exists(configurationDirectoryName))
                {
                    Directory.CreateDirectory(configurationDirectoryName);
                }

                xmlDocument.Save(m_ConfigurationPath);
                AssetDatabase.Refresh();
            }
            catch
            {
                if (File.Exists(m_ConfigurationPath))
                {
                    File.Delete(m_ConfigurationPath);
                }

                return false;
            }

            return m_ResourceCollection.Save();
        }
        /// <summary>
        /// 获取所有资源包
        /// </summary>
        public Resource[] GetResources()
        {
            return m_ResourceCollection.GetResources();
        }
        /// <summary>
        /// 获取资源包
        /// </summary>
        public Resource GetResource(string name, string variant)
        {
            return m_ResourceCollection.GetResource(name, variant);
        }
        /// <summary>
        /// 是否存在资源包
        /// </summary>
        public bool HasResource(string name, string variant)
        {
            return m_ResourceCollection.HasResource(name, variant);
        }
        /// <summary>
        /// 添加资源包
        /// </summary>
        public bool AddResource(string name, string variant, string fileSystem, LoadType loadType, bool packed, bool toLower = true)
        {
            return m_ResourceCollection.AddResource(name, variant, fileSystem, loadType, packed, toLower: toLower);
        }
        /// <summary>
        /// 重命名资源包
        /// </summary>
        public bool RenameResource(string oldName, string oldVariant, string newName, string newVariant)
        {
            return m_ResourceCollection.RenameResource(oldName, oldVariant, newName, newVariant);
        }
        /// <summary>
        /// 移除资源包
        /// </summary>
        public bool RemoveResource(string name, string variant)
        {
            Asset[] assetsToRemove = m_ResourceCollection.GetAssets(name, variant);
            if (m_ResourceCollection.RemoveResource(name, variant))
            {
                List<SourceAsset> unassignedSourceAssets = new List<SourceAsset>();
                foreach (Asset asset in assetsToRemove)
                {
                    SourceAsset sourceAsset = GetSourceAsset(asset.Guid);
                    if (sourceAsset != null)
                    {
                        unassignedSourceAssets.Add(sourceAsset);
                    }
                }

                if (OnAssetUnassigned != null)
                {
                    OnAssetUnassigned(unassignedSourceAssets.ToArray());
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 设置资源加载类型
        /// </summary>
        public bool SetResourceLoadType(string name, string variant, LoadType loadType)
        {
            return m_ResourceCollection.SetResourceLoadType(name, variant, loadType);
        }

        public bool SetResourcePacked(string name, string variant, bool packed)
        {
            return m_ResourceCollection.SetResourcePacked(name, variant, packed);
        }
        /// <summary>
        /// 移除资源数量为0的资源包
        /// </summary>
        public int RemoveUnusedResources()
        {
            List<Resource> resources = new List<Resource>(m_ResourceCollection.GetResources());
            List<Resource> removeResources = resources.FindAll(resource => GetAssets(resource.Name, resource.Variant).Length <= 0);
            foreach (Resource removeResource in removeResources)
            {
                m_ResourceCollection.RemoveResource(removeResource.Name, removeResource.Variant);
            }

            return removeResources.Count;
        }
        /// <summary>
        /// 获取所有资源
        /// </summary>
        public Asset[] GetAssets(string name, string variant)
        {
            List<Asset> assets = new List<Asset>(m_ResourceCollection.GetAssets(name, variant));
            switch (AssetSorter)
            {
                case AssetSorterType.Path:
                    assets.Sort(AssetPathComparer);
                    break;

                case AssetSorterType.Name:
                    assets.Sort(AssetNameComparer);
                    break;

                case AssetSorterType.Guid:
                    assets.Sort(AssetGuidComparer);
                    break;
            }

            return assets.ToArray();
        }
        /// <summary>
        /// guid获取资源
        /// </summary>
        public Asset GetAsset(string guid)
        {
            return m_ResourceCollection.GetAsset(guid);
        }
        /// <summary>
        /// 添加资源
        /// </summary>
        public bool AssignAsset(string guid, string name, string variant)
        {
            if (m_ResourceCollection.AssignAsset(guid, name, variant))
            {
                if (OnAssetAssigned != null)
                {
                    OnAssetAssigned(new SourceAsset[] { GetSourceAsset(guid) });
                }

                return true;
            }

            return false;
        }
        /// <summary>
        /// 移除资源
        /// </summary>
        public bool UnassignAsset(string guid)
        {
            if (m_ResourceCollection.UnassignAsset(guid))
            {
                SourceAsset sourceAsset = GetSourceAsset(guid);
                if (sourceAsset != null)
                {
                    if (OnAssetUnassigned != null)
                    {
                        OnAssetUnassigned(new SourceAsset[] { sourceAsset });
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除 missing 资源
        /// </summary>
        public int RemoveUnknownAssets()
        {
            List<Asset> assets = new List<Asset>(m_ResourceCollection.GetAssets());
            List<Asset> removeAssets = assets.FindAll(asset => GetSourceAsset(asset.Guid) == null);
            foreach (Asset asset in removeAssets)
            {
                m_ResourceCollection.UnassignAsset(asset.Guid);
            }

            return removeAssets.Count;
        }

        public SourceAsset[] GetSourceAssets()
        {
            int count = 0;
            SourceAsset[] sourceAssets = new SourceAsset[m_SourceAssets.Count];
            foreach (KeyValuePair<string, SourceAsset> sourceAsset in m_SourceAssets)
            {
                sourceAssets[count++] = sourceAsset.Value;
            }

            return sourceAssets;
        }

        public SourceAsset GetSourceAsset(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }

            SourceAsset sourceAsset = null;
            if (m_SourceAssets.TryGetValue(guid, out sourceAsset))
            {
                return sourceAsset;
            }

            return null;
        }
        /// <summary>
        /// 清除资源列表
        /// </summary>
        public void ClearRecord()
        {
            m_ResourceCollection.Clear();
        }
        /// <summary>
        /// 分析前查找所有使用到的资源
        /// </summary>
        public void ScanSourceAssets()
        {
            m_SourceAssets.Clear();
            m_SourceAssetRoot.Clear();

            string[] sourceAssetSearchPaths = m_SourceAssetSearchPaths.ToArray();
            HashSet<string> tempGuids = new HashSet<string>();
            tempGuids.UnionWith(AssetDatabase.FindAssets(SourceAssetUnionTypeFilter, sourceAssetSearchPaths));
            tempGuids.UnionWith(AssetDatabase.FindAssets(SourceAssetUnionLabelFilter, sourceAssetSearchPaths));
            tempGuids.ExceptWith(AssetDatabase.FindAssets(SourceAssetExceptTypeFilter, sourceAssetSearchPaths));
            tempGuids.ExceptWith(AssetDatabase.FindAssets(SourceAssetExceptLabelFilter, sourceAssetSearchPaths));



            string[] guids = new List<string>(tempGuids).ToArray();
            foreach (string guid in guids)
            {
                string fullPath = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetDatabase.IsValidFolder(fullPath))
                {
                    // Skip folder.
                    continue;
                }

                string assetPath = fullPath.Substring(SourceAssetRootPath.Length + 1);
                string[] splitedPath = assetPath.Split('/');
                SourceFolder folder = m_SourceAssetRoot;
                for (int i = 0; i < splitedPath.Length - 1; i++)
                {
                    SourceFolder subFolder = folder.GetFolder(splitedPath[i]);
                    folder = subFolder == null ? folder.AddFolder(splitedPath[i]) : subFolder;
                }

                SourceAsset asset = folder.AddAsset(guid, fullPath, splitedPath[splitedPath.Length - 1]);
                m_SourceAssets.Add(asset.Guid, asset);
            }


            var rawAssetPath = ResourceSettings.RawAssetsPath;
            if (!Directory.Exists(rawAssetPath))
            {
                return;
            }
            var files = Directory.GetFiles(rawAssetPath, "*.*", System.IO.SearchOption.AllDirectories);
            

            for (int j = 0; j < files.Length; j++)
            {
                string fullPath = Utility.Path.GetRegularPath(files[j]);
                if (AssetDatabase.IsValidFolder(fullPath))
                {
                    continue;
                }
                if (fullPath.EndsWith(".meta")) continue;

                string guid = AssetDatabase.AssetPathToGUID(fullPath);

                if (guids.Contains(guid)) continue;

                string assetPath = fullPath.Substring(SourceAssetRootPath.Length + 1);
                string[] splitedPath = assetPath.Split('/');
                SourceFolder folder = m_SourceAssetRoot;
                for (int i = 0; i < splitedPath.Length - 1; i++)
                {
                    SourceFolder subFolder = folder.GetFolder(splitedPath[i]);
                    folder = subFolder == null ? folder.AddFolder(splitedPath[i]) : subFolder;
                }

                SourceAsset asset = folder.AddAsset(guid, fullPath, splitedPath[splitedPath.Length - 1]);
                m_SourceAssets.Add(asset.Guid, asset);
            }
        }

        public void RefreshSourceAssetSearchPaths()
        {
            m_SourceAssetSearchPaths.Clear();

            if (string.IsNullOrEmpty(m_SourceAssetRootPath))
            {
                SourceAssetRootPath = DefaultSourceAssetRootPath;
            }

            if (m_SourceAssetSearchRelativePaths.Count > 0)
            {
                foreach (string sourceAssetSearchRelativePath in m_SourceAssetSearchRelativePaths)
                {
                    m_SourceAssetSearchPaths.Add(Utility.Path.GetRegularPath(Path.Combine(m_SourceAssetRootPath, sourceAssetSearchRelativePath)));
                }
            }
            else
            {
                m_SourceAssetSearchPaths.Add(m_SourceAssetRootPath);
            }
        }
        /// <summary>
        /// 排序根据 AssetPath
        /// </summary>
        private int AssetPathComparer(Asset a, Asset b)
        {
            SourceAsset sourceAssetA = GetSourceAsset(a.Guid);
            SourceAsset sourceAssetB = GetSourceAsset(b.Guid);

            if (sourceAssetA != null && sourceAssetB != null)
            {
                return sourceAssetA.Path.CompareTo(sourceAssetB.Path);
            }

            if (sourceAssetA == null && sourceAssetB == null)
            {
                return a.Guid.CompareTo(b.Guid);
            }

            if (sourceAssetA == null)
            {
                return -1;
            }

            if (sourceAssetB == null)
            {
                return 1;
            }

            return 0;
        }
        /// <summary>
        /// 排序根据 Name
        /// </summary>
        private int AssetNameComparer(Asset a, Asset b)
        {
            SourceAsset sourceAssetA = GetSourceAsset(a.Guid);
            SourceAsset sourceAssetB = GetSourceAsset(b.Guid);

            if (sourceAssetA != null && sourceAssetB != null)
            {
                return sourceAssetA.Name.CompareTo(sourceAssetB.Name);
            }

            if (sourceAssetA == null && sourceAssetB == null)
            {
                return a.Guid.CompareTo(b.Guid);
            }

            if (sourceAssetA == null)
            {
                return -1;
            }

            if (sourceAssetB == null)
            {
                return 1;
            }

            return 0;
        }
        /// <summary>
        /// 排序根据 Guid
        /// </summary>
        private int AssetGuidComparer(Asset a, Asset b)
        {
            SourceAsset sourceAssetA = GetSourceAsset(a.Guid);
            SourceAsset sourceAssetB = GetSourceAsset(b.Guid);

            if (sourceAssetA != null && sourceAssetB != null || sourceAssetA == null && sourceAssetB == null)
            {
                return a.Guid.CompareTo(b.Guid);
            }

            if (sourceAssetA == null)
            {
                return -1;
            }

            if (sourceAssetB == null)
            {
                return 1;
            }

            return 0;
        }


        public IAssetBundleAnalysisHandler GetAnalysisHandler()
        {

            IAssetBundleAnalysisHandler handler = null;
            if (string.IsNullOrEmpty(AnalysisHandlerName))
            {
                handler = new DefaultAssetBundleAnalysisHandler();
            }
            else
            {

                var cType = typeof(IAssetBundleAnalysisHandler);
                foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                    .Referenced(cType.Assembly)
                    .SelectMany(o => o.GetTypes()))
                {
                    if (type.IsSubclassOf(cType))
                    {
                        if (type.Name == AnalysisHandlerName)
                        {
                            handler = Activator.CreateInstance(type) as IAssetBundleAnalysisHandler;
                            break;
                        }
                    }
                }

                if (handler == null)
                    throw new Exception("Not found type: " + AnalysisHandlerName);
            }
            return handler;
        }
    }
}
