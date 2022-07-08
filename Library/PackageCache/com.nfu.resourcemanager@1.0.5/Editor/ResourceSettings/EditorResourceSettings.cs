using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.StringFormats;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using ND.Managers.ResourceMgr.Editor.ResourceDependencyAnalyzer;
using ND.Managers.ResourceMgr.Framework;
using ND.Managers.ResourceMgr.Runtime;
using UnityEditor;
using UnityEngine;


namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{

    /// <summary>
    /// 资源编辑器配置，配置文件 'ResourceSettings.xml'
    /// </summary>
    public class EditorResourceSettings
    {
        /// <summary>
        /// <see cref="EditorResourceSettings"/>配置文件路径
        /// </summary>
        private static string m_ConfigPath;

        private static bool m_Loaded;
        private static ResourceGroup[] groups = new ResourceGroup[0];
        private static int SettingsFileVersion = 1;

        /// <summary>
        /// 分析结果设置 <see cref="AssetImporter.assetBundleName"/>
        /// </summary>
        public static bool SetAssetBundleName = false;

        public static bool ClearOnRefresh = false;

        /// <summary>
        /// 在构建前先自动分析
        /// </summary>
        public static bool AnalysisOnBuild = false;

        /// <summary>
        /// 输出路径
        /// </summary>
        public static string OutputPath = "";

        /// <summary>
        /// 生成完复制到 StreamingAssets 目录
        /// </summary>
        public static bool CopyToStreamingAssets = false;

        /// <summary>
        /// 性能分析日志
        /// </summary>
        public static bool PerformanceLogEnabled = false;

        /// <summary>
        /// 图集子目录分隔符
        /// </summary>
        public static string AtlasDirectorySeparator = "_";

        private static string variantAssetPath;
        private static string luaAssetPath;


        /// <summary>
        /// 剔除自动依赖，被引用次数小于2的资源
        /// </summary>
        public static bool StripAutoDependency = false;

        public static string[] AllAssetRootPaths { get; private set; }

        /// <summary>
        /// 检查丢失prefab
        /// </summary>
        public static bool CheckMissingPrefab = false;
        public static string AssetBundleNameFormat;
        /// <summary>
        /// 包内排除正则表达式
        /// </summary>
        public static string ResourcePackedExclude;

        public static bool OutputAssetHash = true;

        private const string ASSET_ENTER_FOLDER_NAME = "Assets/ResourcesAssets/";
        private const string DEFAULT_INCLUDE_LABEL = "ResPack";
        public const string ASSET_VARIENT_FOLDER_NAME = "Assets/Variant/";

        private const string FormatArg_BuildTarget = "BuildTarget";
        private const string FormatArg_AssetPath = "AssetPath";
        private const string FormatArg_Platform = "Platform";
        private const string FormatArg_AppVersion = "AppVersion";
        private const string FormatArg_AssetBundle = "AssetBundle";

        static string ConfigPath
        {
            get
            {
                if (m_ConfigPath == null)
                {
                    m_ConfigPath = Type.GetConfigurationPath<ResourceEditorConfigPathAttribute>() ??
                                   Utility.Path.GetRegularPath(Path.GetFullPath(
                                       "Packages/com.nfu.resourcemanager/Configs/ResourceEditor.xml"));
                    if (!string.IsNullOrEmpty(m_ConfigPath))
                    {
                        m_ConfigPath = Path.Combine(Path.GetDirectoryName(m_ConfigPath), "ResourceSettings.xml");
                        m_ConfigPath = GetReleativePath(m_ConfigPath);
                    }
                    else
                    {
                        m_ConfigPath = string.Empty;
                    }
                }

                return m_ConfigPath;
            }
        }

        /// <summary>
        /// 正则表达式格式, 全局排除的AssetPath
        /// </summary>
        public static string[] Excludes { get; set; } = new string[]
        {
        };

        private static string[] InternalExcludes { get; set; } = new string[]
        {
            "^Assets/StreamingAssets/",
            //"/Resources/",
            "\\.(cs|meta|ds_store|tpsheet|dll)$"
        };

        public static ResourceGroup[] Groups
        {
            get
            {
                EnsureLoadConfig();
                return groups;
            }
            set
            {
                groups = value;
                if (groups == null)
                    groups = new ResourceGroup[0];
            }
        }

        /// <summary>
        ///  路径 'Assets/StreamingAssets' 开始
        /// </summary>
        public static string EditorStreamingAssetsPath
        {
            get
            {
                string dir = "Assets/StreamingAssets";
                string subDir = ResourceSettings.StreamingAssetsPath;
                if (!string.IsNullOrEmpty(subDir))
                    dir = dir + "/" + subDir;
                return dir;
            }
        }




        static EditorResourceSettings()
        {
            EnsureLoadConfig();
        }


        /// <summary>
        /// 资源主目录
        /// </summary>
        public static string AssetRootPath;

        public static string AllAssetRootPath;

        public static string AssetIncludeLabel;
        public static string AllAssetIncludeLabel;

        public static string[] AssetIncludeLabels
        {
            get
            {
                if (string.IsNullOrEmpty(AssetIncludeLabel)) return new string[0];
                return AssetIncludeLabel.Split('|');
            }
        }

        public static string[] AllAssetIncludeLabels
        {
            get
            {
                if (string.IsNullOrEmpty(AllAssetIncludeLabel)) return new string[0];
                return AllAssetIncludeLabel.Split('|');
            }
        }



        public static string AllAssetIncludeLabelFilter;

        /// <summary>
        /// 变体根路径
        /// </summary>
        public static string VariantAssetPath
        {
            get
            {
                if (variantAssetPath == null)
                    variantAssetPath = (Type.GetConfigurationPath<AssetVariantPathAttribute>() ??
                                        ASSET_VARIENT_FOLDER_NAME);
                return variantAssetPath;
            }
        }
        
        public static string LuaAssetPath
        {
            get
            {
                if (luaAssetPath == null)
                {
                    EnsureLoadConfig();
                    foreach (var group in Groups)
                    {
                        if (group.PreprocessBuilds != null)
                        {
                            foreach (var preBuild in group.PreprocessBuilds)
                            {
                                if (preBuild.PreprocessBuild != null && preBuild.PreprocessBuild is LuaPreprocessBuild)
                                {
                                    var luaPreBuild = preBuild.PreprocessBuild as LuaPreprocessBuild;
                                    luaAssetPath = luaPreBuild.outputPath;
                                }
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(luaAssetPath))
                    {
                        luaAssetPath = "Assets/Lua";
                    }
                }

                return luaAssetPath;
            }
        }

        #region Serialization

        #region Load

        public static void LoadConfig()
        {
            if (!File.Exists(ConfigPath))
            {
                SaveConfig();
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(ConfigPath);
            XmlNode xmlRoot = doc.SelectSingleNode("UnityGameFramework");
            XmlNode settingsNode = xmlRoot.SelectSingleNode("ResourceSettings");

            XmlNodeList xmlNodeList = null;
            XmlNode node = null;

            xmlNodeList = settingsNode.ChildNodes;
            Groups = null;
            AssetRootPath = null;
            AllAssetRootPath = null;
            AssetIncludeLabel = null;

            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                node = xmlNodeList.Item(i);
                switch (node.Name)
                {
                    case "Excludes":
                        Excludes = node.ReadArray(o => o.InnerText);
                        break;
                    case "SetAssetBundleName":
                        SetAssetBundleName = node.GetNodeValue<bool>();
                        break;
                    case "AutoAnalysisOnBuild":
                        AnalysisOnBuild = node.GetNodeValue<bool>();
                        break;
                    case "ResourceGroups":
                        groups = node.ReadArray(ReadGroup);
                        break;
                    case "CopyToStreamingAssets":
                        CopyToStreamingAssets = node.GetNodeValue<bool>();
                        break;
                    case "PerformanceLogEnabled":
                        PerformanceLogEnabled = node.GetNodeValue<bool>();
                        break;
                    case "AtlasDirectorySeparator":
                        AtlasDirectorySeparator = node.InnerText;
                        break;
                    case "StripAutoDependency":
                        StripAutoDependency = node.GetNodeValue<bool>();
                        break;
                    case "AssetRootPath":
                        AllAssetRootPath = node.GetNodeValue<string>();
                        if (!string.IsNullOrEmpty(AllAssetRootPath))
                        {
                            AssetRootPath = AllAssetRootPath.Split('|')[0];
                        }

                        break;
                    case "AssetIncludeLabel":
                        AllAssetIncludeLabel = node.GetNodeValue<string>();
                        if (!string.IsNullOrEmpty(AllAssetIncludeLabel))
                        {
                            AssetIncludeLabel = AllAssetIncludeLabel.Split('|')[0];
                        }

                        break;
                    case "CheckMissingPrefab":
                        CheckMissingPrefab = node.GetNodeValue<bool>();
                        break;
                    case "AssetBundleNameFormat":
                        AssetBundleNameFormat = node.GetNodeValue<string>();
                        break;
                    case "ResourcePackedExclude":
                        ResourcePackedExclude = node.GetNodeValue<string>();
                        break;
                    case "OutputAssetHash":
                        OutputAssetHash = node.GetNodeValue<bool>();
                        break;
                }
            }

            if (groups == null)
                groups = new ResourceGroup[0];

            if (string.IsNullOrEmpty(AssetRootPath))
            {
                AssetRootPath = Type.GetConfigurationPath<AssetBundleAnalysisRootPathAttribute>() ??
                                ASSET_ENTER_FOLDER_NAME;
            }

            if (!string.IsNullOrEmpty(AssetRootPath) && !AssetRootPath.EndsWith("/"))
            {
                AssetRootPath += "/";
            }

            if (string.IsNullOrEmpty(AssetIncludeLabel))
            {
                AssetIncludeLabel = Type.GetConfigurationPath<AssetBundleAnalysisIncludeLabelAttribute>() ??
                                    DEFAULT_INCLUDE_LABEL;
            }

            AllAssetIncludeLabelFilter = null;


            AllAssetRootPaths = GetAllAssetRootPaths();


            m_Loaded = true;
        }

        static ResourceGroup ReadGroup(XmlNode groupNode)
        {
            ResourceGroup group = new ResourceGroup();
            foreach (XmlNode node in groupNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "PreprocessBuilds":

                        group.PreprocessBuilds = new List<ResourcePreprocessBuild>(node.ReadArray(itemNode =>
                        {
                            ResourcePreprocessBuild item = new ResourcePreprocessBuild();

                            foreach (XmlNode childNode in itemNode.ChildNodes)
                            {
                                switch (childNode.Name)
                                {
                                    case "PreprocessBuildTypeName":
                                        item.preprocessBuildTypeName = childNode.InnerText;
                                        break;
                                    case "PreprocessBuildData":
                                        item.preprocessBuildData = childNode.InnerText;
                                        break;
                                }
                            }

                            item.OnAfterDeserialize();

                            return item;
                        }));

                        break;
                    case "Rules":
                        var itemsNode = node;
                        group.Rules = new List<ResourceRule>(itemsNode.ReadArray(itemNode =>
                        {
                            ResourceRule item = new ResourceRule();
                            string addressableProviderData = null;
                            foreach (XmlNode childNode in itemNode.ChildNodes)
                            {
                                switch (childNode.Name)
                                {
                                    case "Include":
                                        item.include = childNode.InnerText;
                                        break;
                                    case "Exclude":
                                        item.exclude = childNode.InnerText;
                                        break;
                                    case "AddressableTypeName":
                                        item.addressableTypeName = childNode.InnerText;
                                        break;
                                    case "AddressableProvider":
                                        addressableProviderData = childNode.InnerText;
                                        break;
                                }
                            }

                            item.OnAfterDeserialize();

                            if (!string.IsNullOrEmpty(addressableProviderData))
                            {
                                if (item.AddressableProvider != null)
                                {
                                    JsonUtility.FromJsonOverwrite(addressableProviderData, item.AddressableProvider);
                                }
                            }

                            return item;
                        }));
                        break;
                }
            }

            return group;
        }

        #endregion

        #region Save

        public static void SaveConfig()
        {
            if (File.Exists(ConfigPath))
            {
                EnsureLoadConfig();
            }


            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));

            XmlElement xmlRoot = (XmlElement)doc.AppendChild(doc.CreateElement("UnityGameFramework"));
            var settingsNode = xmlRoot.AppendChild("ResourceSettings");
            settingsNode.SetAttribute("version", SettingsFileVersion.ToString());

            settingsNode.AppendChild("Excludes").WriteArray("Item", Excludes);
            settingsNode.SetNodeValue("SetAssetBundleName", SetAssetBundleName.ToString());
            settingsNode.SetNodeValue("AutoAnalysisOnBuild", AnalysisOnBuild.ToString());
            settingsNode.SetNodeValue("CopyToStreamingAssets", CopyToStreamingAssets.ToString());
            settingsNode.SetNodeValue("PerformanceLogEnabled", PerformanceLogEnabled.ToString());
            settingsNode.SetNodeValue("AtlasDirectorySeparator", AtlasDirectorySeparator);
            settingsNode.SetNodeValue("StripAutoDependency", StripAutoDependency.ToString());
            settingsNode.SetNodeValue("AssetRootPath", AllAssetRootPath);
            settingsNode.SetNodeValue("CheckMissingPrefab", CheckMissingPrefab.ToString());
            settingsNode.SetNodeValue("AssetIncludeLabel", AllAssetIncludeLabel);
            settingsNode.SetNodeValue("AssetBundleNameFormat", AssetBundleNameFormat);
            settingsNode.SetNodeValue("ResourcePackedExclude", ResourcePackedExclude);
            settingsNode.SetNodeValue("OutputAssetHash", OutputAssetHash.ToString());

            var groupsNode = settingsNode.AppendChild("ResourceGroups");
            foreach (var group in groups)
            {
                WriteGroup(groupsNode, group);
            }

            if (Directory.Exists(Path.GetDirectoryName(ConfigPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath));

            doc.Save(ConfigPath);
            AssetDatabase.ImportAsset(ConfigPath);
        }

        static void WriteGroup(XmlNode parent, ResourceGroup group)
        {
            var groupNode = parent.AppendChild("ResourceGroup");
            var preprocessBuildsNode = groupNode.AppendChild("PreprocessBuilds");

            if (group.PreprocessBuilds != null)
            {
                for (int i = 0; i < group.PreprocessBuilds.Count; i++)
                {
                    var item = group.PreprocessBuilds[i];
                    item.OnBeforeSerialize();
                    var itemNode = preprocessBuildsNode.AppendChild("PreprocessBuild");
                    itemNode.SetNodeValue("PreprocessBuildTypeName", item.preprocessBuildTypeName);
                    itemNode.SetNodeValue("PreprocessBuildData", item.preprocessBuildData);
                }
            }

            var rulesNode = groupNode.AppendChild("Rules");
            if (group.Rules != null)
            {
                for (int i = 0; i < group.Rules.Count; i++)
                {
                    var item = group.Rules[i];
                    item.OnBeforeSerialize();
                    var itemNode = rulesNode.AppendChild("Rule");
                    itemNode.SetNodeValue("Include", item.include);
                    itemNode.SetNodeValue("Exclude", item.exclude);
                    itemNode.SetNodeValue("AddressableTypeName", item.addressableTypeName);
                    string addressableProviderData = null;
                    if (item.AddressableProvider != null)
                    {
                        addressableProviderData = JsonUtility.ToJson(item.AddressableProvider);
                    }

                    itemNode.SetNodeValue("AddressableProvider", addressableProviderData);
                }
            }

        }

        #endregion

        #endregion

        public static void EnsureLoadConfig()
        {
            if (!m_Loaded)
            {
                LoadConfig();
            }

            if (groups == null)
                groups = new ResourceGroup[0];

            if (groups.Length == 0)
            {
                var group = new ResourceGroup();
                //group.Rules.Add(new ResourceRule()
                //{
                //    include = "Assets/ResourcesAssets",
                //    AddressableType = typeof(AssetPathAddressableProvider)
                //});
                groups = new ResourceGroup[] { group };
            }
        }


        static string GetReleativePath(string path)
        {
            if (!Path.IsPathRooted(path))
                return path;
            string fullPath = Path.GetFullPath(path);
            string retativeToPath = Path.GetFullPath(".");
            retativeToPath = retativeToPath.Replace('\\', '/');
            fullPath = fullPath.Replace('\\', '/');
            string retativePath;
            if (!retativeToPath.EndsWith("/"))
                retativeToPath += "/";

            if (fullPath.StartsWith(retativeToPath, StringComparison.InvariantCultureIgnoreCase))
                retativePath = fullPath.Substring(retativeToPath.Length);
            else
                retativePath = path;

            return retativePath;
        }

        public static IEnumerable<string> GetExcludes()
        {
            if (InternalExcludes != null)
            {
                foreach (var item in InternalExcludes)
                {
                    yield return item;
                }
            }

            if (Excludes != null)
            {
                foreach (var item in Excludes)
                {
                    yield return item;
                }
            }

        }


        /// <summary>
        /// 是否为排除的资源
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static bool IsExcludeAssetPath(string assetPath)
        {
            bool isExclude = false;

            //排除手动和程序导入的白名单资源
            if (cachedIncludeAssetPaths.Contains(assetPath))
                return false;

            foreach (var exclude in GetExcludes())
            {
                if (EditorUtilityx.GetOrCacheRegex(exclude).IsMatch(assetPath))
                {
                    isExclude = true;
                    break;
                }
            }

            return isExclude;
        }

        /// <summary>
        /// 格式化字符串
        /// </summary>
        public static string FormatString(string input, string assetPath, Dictionary<string, object> values = null)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            if (values == null)
            {
                values = new Dictionary<string, object>();
            }

            GetAssetPathFormatValues(assetPath, values);
            try
            {
                input = input.FormatStringWithKey(values);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return input;
        }

        public static void GetAssetPathFormatValues(string assetPath, Dictionary<string, object> values)
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            values[FormatArg_AssetPath] = assetPath;
            values[FormatArg_BuildTarget] = buildTarget.ToString();
            values[FormatArg_Platform] = GetPlatformName(buildTarget);
            values[FormatArg_AppVersion] = Application.version;
        }


        public static string FormatAssetBundleName(string assetBundleName, Dictionary<string, object> values = null)
        {
            string input = AssetBundleNameFormat;
            if (string.IsNullOrEmpty(input))
                return assetBundleName;
            if (values == null)
            {
                values = new Dictionary<string, object>();
            }

            values[FormatArg_AssetBundle] = assetBundleName;

            try
            {
                input = input.FormatStringWithKey(values);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return input;
        }



        public static string GetPlatformName(BuildTarget buildTarget)
        {
            return GetPlatformName(ResourceBuilderController.BuildTargetToPlatform(buildTarget));
        }

        public static string GetPlatformName(Platform platform)
        {
            string platformName;
            switch (platform)
            {
                case Platform.Windows:
                    platformName = "Windows";
                    break;
                case Platform.Windows64:
                    platformName = "Windows64";
                    break;
                case Platform.IOS:
                    if (ResourceSettings.PlatformNameStyle == ResourceSettings.PlatformNameStyles.Invariant)
                    {
                        return "iOS";
                    }
                    else
                    {
                        platformName = platform.ToString();
                    }
                    break;
                default:
                    platformName = platform.ToString();
                    break;
            }

            if (ResourceSettings.PlatformNameStyle == ResourceSettings.PlatformNameStyles.Lower)
                platformName = platformName.ToLower();
            return platformName;
        }



        /// <summary>
        /// 获取资源包名
        /// </summary>
        public static bool GetAddressable(string assetPath, out string bundleName, out string variant,
        out string assetName)
        {
            bundleName = null;
            variant = null;
            assetName = null;
            for (int i = Groups.Length - 1; i >= 0; i--)
            {
                var group = Groups[i];
                var rule = group.FindRule(assetPath);
                if (rule != null)
                {
                    if (rule.AddressableProvider != null)
                    {
                        if (rule.AddressableProvider.GetAddressableName(assetPath, out bundleName, out variant,
                            out assetName))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    break;
                }
            }

            return false;
        }


        public static string[] GetAllAssetRootPaths()
        {
            HashSet<string> paths = new HashSet<string>();
            Action<string> add = (path) =>
            {
                if (string.IsNullOrEmpty(path))
                    return;
                path = path.Replace('\\', '/');
                if (!path.EndsWith("/"))
                    path += "/";
                paths.Add(path);
            };
            if (!string.IsNullOrEmpty(AllAssetRootPath))
            {
                foreach (var path in AllAssetRootPath.Split('|'))
                {
                    add(path);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(AssetRootPath))
                    add(AssetRootPath);
            }

            return paths.ToArray();
        }


        public static string GetAllAssetIncludeLabelsFilter()
        {
            if (AllAssetIncludeLabelFilter == null)
            {
                StringBuilder filter = new StringBuilder();
                Action<string> add = (label) =>
                {
                    if (string.IsNullOrEmpty(label))
                        return;
                    filter.Append($"l:{label} ");
                };

                if (!string.IsNullOrEmpty(AllAssetIncludeLabel))
                {
                    foreach (var path in AllAssetIncludeLabel.Split('|'))
                    {
                        add(path);
                    }
                }
                else if (!string.IsNullOrEmpty(AssetIncludeLabel))
                {
                    foreach (var path in AssetIncludeLabel.Split('|'))
                    {
                        add(path);
                    }
                }

                AllAssetIncludeLabelFilter = filter.ToString();
            }

            return AllAssetIncludeLabelFilter;
        }



        private static HashSet<string> cachedIncludeAssetPaths = new HashSet<string>();

        public static void ResetCachedIncludeAssetPaths()
        {
            cachedIncludeAssetPaths.Clear();
        }

        public static void AddCachedIncludeAssetPaths(string assetName)
        {
            cachedIncludeAssetPaths.Add(assetName);
        }

        public static bool IsAssetRootPath(string assetPath)
        {
            var paths = AllAssetRootPaths;
            for (int i = 0; i < paths.Length; i++)
            {
                if (assetPath.StartsWith(paths[i]))
                    return true;
            }

            if (cachedIncludeAssetPaths.Contains(assetPath))
                return true;

            var label = EditorResourceSettings.GetAllAssetIncludeLabelsFilter();
            if (string.IsNullOrEmpty(label))
                return false;

            return false;
        }

        public static bool IsPackedResource(string resourceName)
        {
            if (!string.IsNullOrEmpty(ResourcePackedExclude) && Regex.IsMatch(resourceName, ResourcePackedExclude, RegexOptions.IgnoreCase))
                return false;
            return true;
        }

    }
}