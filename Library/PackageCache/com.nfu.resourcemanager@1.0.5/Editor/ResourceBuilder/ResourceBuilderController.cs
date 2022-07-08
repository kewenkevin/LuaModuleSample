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
using ND.Managers.ResourceMgr.Framework;
using ND.Managers.ResourceMgr.Framework.FileSystem;
using ND.Managers.ResourceMgr.Framework.Resource;
using ND.Managers.ResourceMgr.Runtime;

using UnityEditor;
using UnityEngine;

namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{

    /// <summary>
    /// 资源生成控制器，配置文件：ResourceBuilder.xml
    /// </summary>
    public sealed partial class ResourceBuilderController
    {
        internal const string RemoteVersionListFileName = ResourceManager.RemoteVersionListFileName;
        internal const string LocalVersionListFileName = ResourceManager.LocalVersionListFileName;
        internal const string DefaultExtension = "dat";
        private const string NoneOptionName = "<None>";
        private static readonly int AssetsStringLength = "Assets".Length;

        private readonly string m_ConfigurationPath;
        private readonly ResourceCollection m_ResourceCollection;
        private readonly ResourceAnalyzerController m_ResourceAnalyzerController;
        private readonly SortedDictionary<string, ResourceData> m_ResourceDatas;
        private readonly Dictionary<string, IFileSystem> m_OutputPackageFileSystems;
        private readonly Dictionary<string, IFileSystem> m_OutputPackedFileSystems;
        private readonly BuildReport m_BuildReport;
        private readonly List<string> m_BuildEventHandlerTypeNames;
        private IResBuildEventHandler m_ResBuildEventHandler;
        private IFileSystemManager m_FileSystemManager;
        private bool loaded;

        public ResourceBuilderController()
        {
            m_ConfigurationPath = Type.GetConfigurationPath<ResourceBuilderConfigPathAttribute>() ??
                                  Utility.Path.GetRegularPath(
                                      Path.GetFullPath("Packages/com.nfu.resourcemanager/Configs/ResourceBuilder.xml"));

            m_ResourceCollection = new ResourceCollection();
            Utility.ZipUtil.SetZipHelper(new DefaultZipHelper());

            m_ResourceCollection.OnLoadingResource += delegate(int index, int count)
            {
                if (OnLoadingResource != null)
                {
                    OnLoadingResource(index, count);
                }
            };

            m_ResourceCollection.OnLoadingAsset += delegate(int index, int count)
            {
                if (OnLoadingAsset != null)
                {
                    OnLoadingAsset(index, count);
                }
            };

            m_ResourceCollection.OnLoadCompleted += delegate()
            {
                if (OnLoadCompleted != null)
                {
                    OnLoadCompleted();
                }
            };

            m_ResourceAnalyzerController = new ResourceAnalyzerController(m_ResourceCollection);

            m_ResourceAnalyzerController.OnAnalyzingAsset += delegate(int index, int count)
            {
                if (OnAnalyzingAsset != null)
                {
                    OnAnalyzingAsset(index, count);
                }
            };

            m_ResourceAnalyzerController.OnAnalyzeCompleted += delegate()
            {
                if (OnAnalyzeCompleted != null)
                {
                    OnAnalyzeCompleted();
                }
            };

            m_ResourceDatas = new SortedDictionary<string, ResourceData>(StringComparer.Ordinal);
            m_OutputPackageFileSystems = new Dictionary<string, IFileSystem>(StringComparer.Ordinal);
            m_OutputPackedFileSystems = new Dictionary<string, IFileSystem>(StringComparer.Ordinal);
            m_BuildReport = new BuildReport();

            m_BuildEventHandlerTypeNames = new List<string>
            {
                NoneOptionName
            };

            m_BuildEventHandlerTypeNames.AddRange(Type.GetEditorTypeNames(typeof(IResBuildEventHandler)));
            m_ResBuildEventHandler = null;
            m_FileSystemManager = null;

            Platforms = Platform.FollowProject;
            ZipSelected = true;
            DeterministicAssetBundleSelected = ChunkBasedCompressionSelected = true;
            UncompressedAssetBundleSelected = DisableWriteTypeTreeSelected = ForceRebuildAssetBundleSelected =
                IgnoreTypeTreeChangesSelected = AppendHashToAssetBundleNameSelected = false;
            OutputPackageSelected = OutputFullSelected = OutputPackedSelected = true;
            BuildEventHandlerTypeName = string.Empty;
            OutputDirectory = string.Empty;
        }

        public string ProductName
        {
            get { return PlayerSettings.productName; }
        }

        public string CompanyName
        {
            get { return PlayerSettings.companyName; }
        }

        public string GameIdentifier
        {
            get
            {
#if UNITY_5_6_OR_NEWER
                return PlayerSettings.applicationIdentifier;
#else
                return PlayerSettings.bundleIdentifier;
#endif
            }
        }

        public string GameFrameworkVersion
        {
            get { return ND.Managers.ResourceMgr.Framework.Version.GameFrameworkVersion; }
        }

        public string UnityVersion
        {
            get { return Application.unityVersion; }
        }

        public string ApplicableGameVersion
        {
            get { return Application.version; }
        }

        public int InternalResourceVersion { get; set; }

        public Platform Platforms { get; set; }

        public Platform SelectedPlatforms
        {
            get
            {
                if (Platforms == Platform.FollowProject)
                {
                    return CurrentPlatform;
                }

                return Platforms;
            }
        }

        public Platform CurrentPlatform
        {
            get => BuildTargetToPlatform(EditorUserBuildSettings.activeBuildTarget);
        }


        public bool ZipSelected { get; set; }

        public bool UncompressedAssetBundleSelected { get; set; }

        public bool DisableWriteTypeTreeSelected { get; set; }

        public bool DeterministicAssetBundleSelected { get; set; }

        public bool ForceRebuildAssetBundleSelected { get; set; }

        public bool IgnoreTypeTreeChangesSelected { get; set; }

        public bool AppendHashToAssetBundleNameSelected { get; set; }

        public bool ChunkBasedCompressionSelected { get; set; }

        public bool OutputPackageSelected { get; set; }

        public bool OutputFullSelected { get; set; }

        public bool OutputPackedSelected { get; set; }

        public string BuildEventHandlerTypeName { get; set; }

        public string OutputDirectory { get; set; }


        public bool IsValidOutputDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(OutputDirectory))
                {
                    return false;
                }

                if (!Directory.Exists(OutputDirectory))
                {
                    Directory.CreateDirectory(OutputDirectory);
                    return true;
                }

                return true;
            }
        }

        public string WorkingPath
        {
            get
            {
                if (!IsValidOutputDirectory)
                {
                    return string.Empty;
                }

                return Utility.Path.GetRegularPath(
                    new DirectoryInfo(Utility.Text.Format("{0}/Working/", OutputDirectory)).FullName);
            }
        }

        public string OutputPackagePath
        {
            get
            {
                if (!IsValidOutputDirectory)
                {
                    return string.Empty;
                }

                return Utility.Path.GetRegularPath(new DirectoryInfo(Utility.Text.Format("{0}/Package/{1}_{2}/",
                        OutputDirectory, ApplicableGameVersion.Replace('.', '_'), InternalResourceVersion.ToString()))
                    .FullName);
            }
        }

        public string OutputFullPath
        {
            get
            {
                if (!IsValidOutputDirectory)
                {
                    return string.Empty;
                }

                return Utility.Path.GetRegularPath(new DirectoryInfo(Utility.Text.Format("{0}/Full/{1}_{2}/",
                        OutputDirectory, ApplicableGameVersion.Replace('.', '_'), InternalResourceVersion.ToString()))
                    .FullName);
            }
        }

        public string OutputPackedPath
        {
            get
            {
                if (!IsValidOutputDirectory)
                {
                    return string.Empty;
                }

                return Utility.Path.GetRegularPath(new DirectoryInfo(Utility.Text.Format("{0}/Packed/{1}_{2}/",
                        OutputDirectory, ApplicableGameVersion.Replace('.', '_'), InternalResourceVersion.ToString()))
                    .FullName);
            }
        }

        public string BuildReportPath
        {
            get
            {
                if (!IsValidOutputDirectory)
                {
                    return string.Empty;
                }

                return Utility.Path.GetRegularPath(new DirectoryInfo(Utility.Text.Format("{0}/BuildReport/{1}_{2}/",
                        OutputDirectory, ApplicableGameVersion.Replace('.', '_'), InternalResourceVersion.ToString()))
                    .FullName);
            }
        }

        public bool IsPartBuild { get; private set; }

        public bool PartBuildTriggerEvent { get; set; }

        public event GameFrameworkAction<int, int> OnLoadingResource = null;

        public event GameFrameworkAction<int, int> OnLoadingAsset = null;

        public event GameFrameworkAction OnLoadCompleted = null;

        public event GameFrameworkAction<int, int> OnAnalyzingAsset = null;

        public event GameFrameworkAction OnAnalyzeCompleted = null;

        public event GameFrameworkFunc<string, float, bool> ProcessingAssetBundle = null;

        public event GameFrameworkFunc<string, float, bool> ProcessingBinary = null;

        public event GameFrameworkAction<Platform> ProcessResourceComplete = null;

        public event GameFrameworkAction<string> BuildResourceError = null;

        public bool Load(bool incrementalVersion = true)
        {

            if (!File.Exists(m_ConfigurationPath))
            {
                Debug.LogWarning(m_ConfigurationPath + " not exists");
                return false;
            }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(m_ConfigurationPath);
                XmlNode xmlRoot = xmlDocument.SelectSingleNode("UnityGameFramework");
                XmlNode xmlEditor = xmlRoot.SelectSingleNode("ResourceBuilder");
                XmlNode xmlSettings = xmlEditor.SelectSingleNode("Settings");

                XmlNodeList xmlNodeList = null;
                XmlNode xmlNode = null;

                xmlNodeList = xmlSettings.ChildNodes;
                for (int i = 0; i < xmlNodeList.Count; i++)
                {
                    xmlNode = xmlNodeList.Item(i);
                    switch (xmlNode.Name)
                    {
                        case "InternalResourceVersion":
                            InternalResourceVersion = int.Parse(xmlNode.InnerText);
                            if (incrementalVersion)
                                InternalResourceVersion += 1;
                            break;

                        case "Platforms":
                            Platforms = (Platform) int.Parse(xmlNode.InnerText);
                            break;

                        case "ZipSelected":
                            ZipSelected = bool.Parse(xmlNode.InnerText);
                            break;

                        case "UncompressedAssetBundleSelected":
                            UncompressedAssetBundleSelected = bool.Parse(xmlNode.InnerText);
                            if (UncompressedAssetBundleSelected)
                            {
                                ChunkBasedCompressionSelected = false;
                            }

                            break;

                        case "DisableWriteTypeTreeSelected":
                            DisableWriteTypeTreeSelected = bool.Parse(xmlNode.InnerText);
                            if (DisableWriteTypeTreeSelected)
                            {
                                IgnoreTypeTreeChangesSelected = false;
                            }

                            break;

                        case "DeterministicAssetBundleSelected":
                            DeterministicAssetBundleSelected = bool.Parse(xmlNode.InnerText);
                            break;

                        case "ForceRebuildAssetBundleSelected":
                            ForceRebuildAssetBundleSelected = bool.Parse(xmlNode.InnerText);
                            break;

                        case "IgnoreTypeTreeChangesSelected":
                            IgnoreTypeTreeChangesSelected = bool.Parse(xmlNode.InnerText);
                            if (IgnoreTypeTreeChangesSelected)
                            {
                                DisableWriteTypeTreeSelected = false;
                            }

                            break;

                        case "AppendHashToAssetBundleNameSelected":
                            AppendHashToAssetBundleNameSelected = false;
                            break;

                        case "ChunkBasedCompressionSelected":
                            ChunkBasedCompressionSelected = bool.Parse(xmlNode.InnerText);
                            if (ChunkBasedCompressionSelected)
                            {
                                UncompressedAssetBundleSelected = false;
                            }

                            break;

                        case "OutputPackageSelected":
                            OutputPackageSelected = bool.Parse(xmlNode.InnerText);
                            break;

                        case "OutputFullSelected":
                            OutputFullSelected = bool.Parse(xmlNode.InnerText);
                            break;

                        case "OutputPackedSelected":
                            OutputPackedSelected = bool.Parse(xmlNode.InnerText);
                            break;

                        case "BuildEventHandlerTypeName":
                            BuildEventHandlerTypeName = xmlNode.InnerText;
                            RefreshBuildEventHandler();
                            break;

                        case "OutputDirectory":
                            OutputDirectory = xmlNode.InnerText;
                            if (!Directory.Exists(OutputDirectory))
                            {
                                var info = Directory.CreateDirectory(Application.dataPath + "/../output");
                                OutputDirectory = info.FullName;
                            }

                            break;
                    }
                }

                loaded = true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                Debug.LogError("delete config: " + m_ConfigurationPath);
                File.Delete(m_ConfigurationPath);
                return false;
            }

            return true;
        }

        public bool Save()
        {

            var performance = new PerformanceSample($"{GetType().Name}.{nameof(Save)}");
            try
            {
                if (!loaded)
                    Load();

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));

                XmlElement xmlRoot = xmlDocument.CreateElement("UnityGameFramework");
                xmlDocument.AppendChild(xmlRoot);

                XmlElement xmlBuilder = xmlDocument.CreateElement("ResourceBuilder");
                xmlRoot.AppendChild(xmlBuilder);

                XmlElement xmlSettings = xmlDocument.CreateElement("Settings");
                xmlBuilder.AppendChild(xmlSettings);

                XmlElement xmlElement = null;

                xmlElement = xmlDocument.CreateElement("InternalResourceVersion");
                xmlElement.InnerText = InternalResourceVersion.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("Platforms");
                xmlElement.InnerText = ((int) Platforms).ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("ZipSelected");
                xmlElement.InnerText = ZipSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("UncompressedAssetBundleSelected");
                xmlElement.InnerText = UncompressedAssetBundleSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("DisableWriteTypeTreeSelected");
                xmlElement.InnerText = DisableWriteTypeTreeSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("DeterministicAssetBundleSelected");
                xmlElement.InnerText = DeterministicAssetBundleSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("ForceRebuildAssetBundleSelected");
                xmlElement.InnerText = ForceRebuildAssetBundleSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("IgnoreTypeTreeChangesSelected");
                xmlElement.InnerText = IgnoreTypeTreeChangesSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("AppendHashToAssetBundleNameSelected");
                xmlElement.InnerText = AppendHashToAssetBundleNameSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("ChunkBasedCompressionSelected");
                xmlElement.InnerText = ChunkBasedCompressionSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("OutputPackageSelected");
                xmlElement.InnerText = OutputPackageSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("OutputFullSelected");
                xmlElement.InnerText = OutputFullSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("OutputPackedSelected");
                xmlElement.InnerText = OutputPackedSelected.ToString();
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("BuildEventHandlerTypeName");
                xmlElement.InnerText = BuildEventHandlerTypeName;
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("OutputDirectory");
                xmlElement.InnerText = OutputDirectory;
                xmlSettings.AppendChild(xmlElement);

                string configurationDirectoryName = Path.GetDirectoryName(m_ConfigurationPath);
                if (!Directory.Exists(configurationDirectoryName))
                {
                    Directory.CreateDirectory(configurationDirectoryName);
                }

                xmlDocument.Save(m_ConfigurationPath);
                performance.Dispose();
                AssetDatabase.Refresh();
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                //if (File.Exists(m_ConfigurationPath))
                //{
                //    File.Delete(m_ConfigurationPath);
                //}
                performance.Dispose();
                return false;
            }
        }

        public void SetBuildEventHandler(IResBuildEventHandler resBuildEventHandler)
        {
            m_ResBuildEventHandler = resBuildEventHandler;
        }

        public string[] GetBuildEventHandlerTypeNames()
        {
            return m_BuildEventHandlerTypeNames.ToArray();
        }

        public bool IsPlatformSelected(Platform platform)
        {
            return (SelectedPlatforms & platform) != 0;
        }

        /// <summary>
        /// 是否为工程当前平台
        /// </summary>
        public bool IsCurrentPlatform(Platform platform)
        {
            return (GetBuildTarget(platform) == EditorUserBuildSettings.activeBuildTarget);
        }

        public void SelectPlatform(Platform platform, bool selected)
        {
            if (selected)
            {
                Platforms |= platform;
            }
            else
            {
                Platforms &= ~platform;
            }
        }

        public bool RefreshBuildEventHandler()
        {
            bool retVal = false;
            if (!string.IsNullOrEmpty(BuildEventHandlerTypeName) &&
                m_BuildEventHandlerTypeNames.Contains(BuildEventHandlerTypeName))
            {
                System.Type buildEventHandlerType = Utility.Assembly.GetType(BuildEventHandlerTypeName);
                if (buildEventHandlerType != null)
                {
                    IResBuildEventHandler resBuildEventHandler =
                        (IResBuildEventHandler) Activator.CreateInstance(buildEventHandlerType);
                    if (resBuildEventHandler != null)
                    {
                        SetBuildEventHandler(resBuildEventHandler);
                        return true;
                    }
                }
            }
            else
            {
                retVal = true;
            }

            BuildEventHandlerTypeName = string.Empty;
            SetBuildEventHandler(null);
            return retVal;
        }

        public bool BuildResources()
        {
            if (!IsValidOutputDirectory)
            {
                return false;
            }

            if (Directory.Exists(OutputPackagePath))
            {
                Directory.Delete(OutputPackagePath, true);
            }

            Directory.CreateDirectory(OutputPackagePath);

            if (Directory.Exists(OutputFullPath))
            {
                Directory.Delete(OutputFullPath, true);
            }

            Directory.CreateDirectory(OutputFullPath);

            if (Directory.Exists(OutputPackedPath))
            {
                Directory.Delete(OutputPackedPath, true);
            }

            Directory.CreateDirectory(OutputPackedPath);

            string buildReportPath = BuildReportPath;
            buildReportPath = Path.Combine(buildReportPath);
            if (Directory.Exists(buildReportPath))
            {
                Directory.Delete(buildReportPath, true);
            }

            Directory.CreateDirectory(buildReportPath);

            IsPartBuild = false;

            BuildAssetBundleOptions buildAssetBundleOptions = GetBuildAssetBundleOptions();
            m_BuildReport.Initialize(buildReportPath, ProductName, CompanyName, GameIdentifier, GameFrameworkVersion,
                UnityVersion, ApplicableGameVersion, InternalResourceVersion,
                SelectedPlatforms, ZipSelected, (int) buildAssetBundleOptions, m_ResourceDatas);

            try
            {
                m_BuildReport.LogInfo("Build Start Time: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                if (m_ResBuildEventHandler != null)
                {
                    m_BuildReport.LogInfo("Execute build event handler 'OnPreprocessAllPlatforms'...");
                    m_ResBuildEventHandler.OnPreprocessAllPlatforms(ProductName, CompanyName, GameIdentifier,
                        GameFrameworkVersion, UnityVersion, ApplicableGameVersion, InternalResourceVersion,
                        buildAssetBundleOptions, ZipSelected, OutputDirectory, WorkingPath, OutputPackageSelected,
                        OutputPackagePath, OutputFullSelected, OutputFullPath, OutputPackedSelected, OutputPackedPath,
                        BuildReportPath);
                }

                m_BuildReport.LogInfo("Start prepare resource collection...");
                if (!m_ResourceCollection.Load())
                {
                    m_BuildReport.LogError(
                        "Can not parse 'ResourceCollection.xml', please use 'Resource Editor' tool first.");

                    if (m_ResBuildEventHandler != null)
                    {
                        m_BuildReport.LogInfo("Execute build event handler 'OnPostprocessAllPlatforms'...");
                        m_ResBuildEventHandler.OnPostprocessAllPlatforms(ProductName, CompanyName, GameIdentifier,
                            GameFrameworkVersion, UnityVersion, ApplicableGameVersion, InternalResourceVersion,
                            buildAssetBundleOptions, ZipSelected, OutputDirectory, WorkingPath, OutputPackageSelected,
                            OutputPackagePath, OutputFullSelected, OutputFullPath, OutputPackedSelected,
                            OutputPackedPath, BuildReportPath);
                    }

                    m_BuildReport.SaveReport();
                    return false;
                }

                //if (Platforms == Platform.Undefined)
                //{
                //    m_BuildReport.LogError("Platform undefined.");

                //    if (m_ResBuildEventHandler != null)
                //    {
                //        m_BuildReport.LogInfo("Execute build event handler 'OnPostprocessAllPlatforms'...");
                //        m_ResBuildEventHandler.OnPostprocessAllPlatforms(ProductName, CompanyName, GameIdentifier, GameFrameworkVersion, UnityVersion, ApplicableGameVersion, InternalResourceVersion, buildAssetBundleOptions, ZipSelected, OutputDirectory, WorkingPath, OutputPackageSelected, OutputPackagePath, OutputFullSelected, OutputFullPath, OutputPackedSelected, OutputPackedPath, BuildReportPath);
                //    }

                //    m_BuildReport.SaveReport();
                //    return false;
                //}

                m_BuildReport.LogInfo("Prepare resource collection complete.");
                m_BuildReport.LogInfo("Start analyze assets dependency...");

                m_ResourceAnalyzerController.Analyze();

                m_BuildReport.LogInfo("Analyze assets dependency complete.");
                m_BuildReport.LogInfo("Start prepare build data...");

                AssetBundleBuild[] assetBundleBuildDatas = null;
                ResourceData[] assetBundleResourceDatas = null;
                ResourceData[] binaryResourceDatas = null;
                if (!PrepareBuildData(out assetBundleBuildDatas, out assetBundleResourceDatas, out binaryResourceDatas))
                {
                    m_BuildReport.LogError("Prepare resource build data failure.");

                    if (m_ResBuildEventHandler != null)
                    {
                        m_BuildReport.LogInfo("Execute build event handler 'OnPostprocessAllPlatforms'...");
                        m_ResBuildEventHandler.OnPostprocessAllPlatforms(ProductName, CompanyName, GameIdentifier,
                            GameFrameworkVersion, UnityVersion, ApplicableGameVersion, InternalResourceVersion,
                            buildAssetBundleOptions, ZipSelected, OutputDirectory, WorkingPath, OutputPackageSelected,
                            OutputPackagePath, OutputFullSelected, OutputFullPath, OutputPackedSelected,
                            OutputPackedPath, BuildReportPath);
                    }

                    m_BuildReport.SaveReport();
                    return false;
                }

                m_BuildReport.LogInfo("Prepare resource build data complete.");
                m_BuildReport.LogInfo("Start build resources for selected platforms...");

                bool watchResult = m_ResBuildEventHandler == null || !m_ResBuildEventHandler.ContinueOnFailure;
                bool isSuccess = false;
                isSuccess = BuildResources(Platform.Windows, assetBundleBuildDatas, buildAssetBundleOptions,
                    assetBundleResourceDatas, binaryResourceDatas);

                if (!watchResult || isSuccess)
                {
                    isSuccess = BuildResources(Platform.Windows64, assetBundleBuildDatas, buildAssetBundleOptions,
                        assetBundleResourceDatas, binaryResourceDatas);
                }

                if (!watchResult || isSuccess)
                {
                    isSuccess = BuildResources(Platform.MacOS, assetBundleBuildDatas, buildAssetBundleOptions,
                        assetBundleResourceDatas, binaryResourceDatas);
                }

                if (!watchResult || isSuccess)
                {
                    isSuccess = BuildResources(Platform.Linux, assetBundleBuildDatas, buildAssetBundleOptions,
                        assetBundleResourceDatas, binaryResourceDatas);
                }

                if (!watchResult || isSuccess)
                {
                    isSuccess = BuildResources(Platform.IOS, assetBundleBuildDatas, buildAssetBundleOptions,
                        assetBundleResourceDatas, binaryResourceDatas);
                }

                if (!watchResult || isSuccess)
                {
                    isSuccess = BuildResources(Platform.Android, assetBundleBuildDatas, buildAssetBundleOptions,
                        assetBundleResourceDatas, binaryResourceDatas);
                }

                if (!watchResult || isSuccess)
                {
                    isSuccess = BuildResources(Platform.WindowsStore, assetBundleBuildDatas, buildAssetBundleOptions,
                        assetBundleResourceDatas, binaryResourceDatas);
                }

                if (!watchResult || isSuccess)
                {
                    isSuccess = BuildResources(Platform.WebGL, assetBundleBuildDatas, buildAssetBundleOptions,
                        assetBundleResourceDatas, binaryResourceDatas);
                }

                if (m_ResBuildEventHandler != null)
                {
                    m_BuildReport.LogInfo("Execute build event handler 'OnPostprocessAllPlatforms'...");
                    m_ResBuildEventHandler.OnPostprocessAllPlatforms(ProductName, CompanyName, GameIdentifier,
                        GameFrameworkVersion, UnityVersion, ApplicableGameVersion, InternalResourceVersion,
                        buildAssetBundleOptions, ZipSelected, OutputDirectory, WorkingPath, OutputPackageSelected,
                        OutputPackagePath, OutputFullSelected, OutputFullPath, OutputPackedSelected, OutputPackedPath,
                        BuildReportPath);
                }

                m_BuildReport.LogInfo("Build resources for selected platforms complete.");
                m_BuildReport.SaveReport();

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                string errorMessage = exception.ToString();
                m_BuildReport.LogFatal(errorMessage);
                m_BuildReport.SaveReport();
                if (BuildResourceError != null)
                {
                    BuildResourceError(errorMessage);
                }

                return false;
            }
            finally
            {
                m_OutputPackageFileSystems.Clear();
                m_OutputPackedFileSystems.Clear();
                if (m_FileSystemManager != null)
                {
                    GameFrameworkEntry.Shutdown();
                    m_FileSystemManager = null;
                }
            }
        }

        public void GetOutputPath(Platform platform, out string workingPath, out string outputPackagePath,
            out string outputFullPath, out string outputPackedPath)
        {
            string platformName = EditorResourceSettings.GetPlatformName(platform);
            workingPath = Utility.Text.Format("{0}{1}/", WorkingPath, platformName);
            outputPackagePath = Utility.Text.Format("{0}{1}/", OutputPackagePath, platformName);
            outputFullPath = Utility.Text.Format("{0}{1}/", OutputFullPath, platformName);
            outputPackedPath = Utility.Text.Format("{0}{1}/", OutputPackedPath, platformName);
        }

        /// <summary>
        /// 生成资源包
        /// </summary>
        private bool BuildResources(Platform platform, AssetBundleBuild[] assetBundleBuildDatas,
            BuildAssetBundleOptions buildAssetBundleOptions, ResourceData[] assetBundleResourceDatas,
            ResourceData[] binaryResourceDatas)
        {
            if (!IsPlatformSelected(platform))
            {
                return true;
            }

            m_OutputPackageFileSystems.Clear();
            m_OutputPackedFileSystems.Clear();
            string platformName = EditorResourceSettings.GetPlatformName(platform);
            m_BuildReport.LogInfo("Start build resources for '{0}'...", platformName);

            string workingPath, outputPackagePath, outputFullPath, outputPackedPath;

            GetOutputPath(platform, out workingPath, out outputPackagePath, out outputFullPath, out outputPackedPath);

            m_BuildReport.LogInfo("Working path is '{0}'.", workingPath);

            // Clean working path
            List<string> validNames = new List<string>();
            foreach (ResourceData assetBundleResourceData in assetBundleResourceDatas)
            {
                validNames.Add(GetResourceFullName(assetBundleResourceData.Name, assetBundleResourceData.Variant)
                    .ToLower());
            }

            if (Directory.Exists(workingPath))
            {
                Uri workingUri = new Uri(workingPath, UriKind.Absolute);
                string[] fileNames = Directory.GetFiles(workingPath, "*", SearchOption.AllDirectories);
                foreach (string fileName in fileNames)
                {
                    if (fileName.EndsWith(".manifest", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    string relativeName = workingUri.MakeRelativeUri(new Uri(fileName, UriKind.Absolute)).ToString();
                    if (!validNames.Contains(relativeName))
                    {
                        File.Delete(fileName);
                    }
                }

                string[] manifestNames = Directory.GetFiles(workingPath, "*.manifest", SearchOption.AllDirectories);
                foreach (string manifestName in manifestNames)
                {
                    if (!File.Exists(manifestName.Substring(0, manifestName.LastIndexOf('.'))))
                    {
                        File.Delete(manifestName);
                    }
                }

                Utility.Path.RemoveEmptyDirectory(workingPath);
            }

            if (!Directory.Exists(workingPath))
            {
                Directory.CreateDirectory(workingPath);
                AssetDatabase.Refresh();
            }

            if (m_ResBuildEventHandler != null)
            {
                m_BuildReport.LogInfo("Execute build event handler 'OnPreprocessPlatform' for '{0}'...", platformName);
                m_ResBuildEventHandler.OnPreprocessPlatform(platform, workingPath, OutputPackageSelected,
                    OutputPackagePath, OutputFullSelected, OutputFullPath, OutputPackedSelected, OutputPackedPath);
            }

            // Build AssetBundles
            m_BuildReport.LogInfo("Unity start build asset bundles for '{0}'... options: {1}", platformName,
                buildAssetBundleOptions);
            Debug.LogFormat("Unity start build asset bundles for '{0}'... options: {1}", platformName,
                buildAssetBundleOptions);

            AssetBundleManifest assetBundleManifest;
            using (new PerformanceSample($"BuildPipeline.BuildAssetBundles"))
            {
                assetBundleManifest = BuildPipeline.BuildAssetBundles(workingPath, assetBundleBuildDatas,
                    buildAssetBundleOptions, GetBuildTarget(platform));
            }

            if (assetBundleManifest == null)
            {
                m_BuildReport.LogError("Build asset bundles for '{0}' failure.", platformName);

                if (m_ResBuildEventHandler != null)
                {
                    m_BuildReport.LogInfo("Execute build event handler 'OnPostprocessPlatform' for '{0}'...",
                        platformName);
                    m_ResBuildEventHandler.OnPostprocessPlatform(platform, workingPath, OutputPackageSelected,
                        OutputPackagePath, OutputFullSelected, OutputFullPath, OutputPackedSelected, OutputPackedPath,
                        false);
                }

                return false;
            }



            if (m_ResBuildEventHandler != null)
            {
                m_BuildReport.LogInfo("Execute build event handler 'OnBuildAssetBundlesComplete' for '{0}'...",
                    platformName);
                m_ResBuildEventHandler.OnBuildAssetBundlesComplete(platform, workingPath, OutputPackageSelected,
                    OutputPackagePath, OutputFullSelected, OutputFullPath, OutputPackedSelected, OutputPackedPath,
                    assetBundleManifest);
            }

            m_BuildReport.LogInfo("Unity build asset bundles for '{0}' complete.", platformName);


            if (EditorResourceSettings.CopyToStreamingAssets && IsCurrentPlatform(platform))
            {
                ResourceTools.BuildResources.ClearStreamingAssets();
            }



            GetOutputPath(platform, out workingPath, out outputPackagePath, out outputFullPath,
                out outputPackedPath);


            if (OutputPackageSelected)
            {
                Directory.CreateDirectory(outputPackagePath);
                m_BuildReport.LogInfo("Output package is selected, path is '{0}'.", outputPackagePath);
            }
            else
            {
                m_BuildReport.LogInfo("Output package is not selected.");
            }

            if (OutputFullSelected)
            {
                Directory.CreateDirectory(outputFullPath);
                m_BuildReport.LogInfo("Output full is selected, path is '{0}'.", outputFullPath);
            }
            else
            {
                m_BuildReport.LogInfo("Output full is not selected.");
            }

            if (OutputPackedSelected)
            {
                Directory.CreateDirectory(outputPackedPath);
                m_BuildReport.LogInfo("Output packed is selected, path is '{0}'.", outputPackedPath);
            }
            else
            {
                m_BuildReport.LogInfo("Output packed is not selected.");
            }

            var resourceDatas = m_ResourceDatas.Values;

            m_OutputPackageFileSystems.Clear();
            m_OutputPackedFileSystems.Clear();

            // Create FileSystems
            m_BuildReport.LogInfo("Start create file system for '{0}'...", platformName);

            if (OutputPackageSelected)
            {
                CreateFileSystems(resourceDatas, outputPackagePath, m_OutputPackageFileSystems);
            }

            if (OutputPackedSelected)
            {
                CreateFileSystems(GetPackedResourceDatas(resourceDatas), outputPackedPath, m_OutputPackedFileSystems);
            }

            m_BuildReport.LogInfo("Create file system for '{0}' complete.", platformName);


            var performanceProcessAssetBundles = new PerformanceSample("Process AssetBundles");

            // Process AssetBundles
            for (int i = 0; i < assetBundleResourceDatas.Length; i++)
            {
                var assetBundleResourceData = assetBundleResourceDatas[i];
                
                string fullName = GetResourceFullName(assetBundleResourceData.Name, assetBundleResourceData.Variant);
                if (ProcessingAssetBundle != null)
                {
                    if (ProcessingAssetBundle(fullName, (float) (i + 1) / assetBundleResourceDatas.Length))
                    {
                        m_BuildReport.LogWarning("The build has been canceled by user.");

                        if (m_ResBuildEventHandler != null)
                        {
                            m_BuildReport.LogInfo("Execute build event handler 'OnPostprocessPlatform' for '{0}'...",
                                platformName);
                            m_ResBuildEventHandler.OnPostprocessPlatform(platform, workingPath,
                                OutputPackageSelected, outputPackagePath, OutputFullSelected, outputFullPath,
                                OutputPackedSelected, outputPackedPath, false);
                        }

                        return false;
                    }
                }

                m_BuildReport.LogInfo("Start process asset bundle '{0}' for '{1}'...", fullName, platformName);

                if (!ProcessAssetBundle(platform, workingPath, outputPackagePath, outputFullPath, outputPackedPath,
                        ZipSelected, assetBundleResourceData.Name, assetBundleResourceData.Variant,
                        assetBundleResourceData.FileSystem))
                {
                    return false;
                }

                EditorUtilityx.DisplayProgressBar("Process AssetBundles", fullName,
                    (i + 1) / (float) assetBundleResourceDatas.Length);
                m_BuildReport.LogInfo("Process asset bundle '{0}' for '{1}' complete.", fullName, platformName);
            }

            EditorUtilityx.ClearProgressBar();
            performanceProcessAssetBundles.Dispose();

            var performanceProcessBinaries = new PerformanceSample("Process Binaries");

            // Process Binaries
            for (int i = 0; i < binaryResourceDatas.Length; i++)
            {
                string fullName = GetResourceFullName(binaryResourceDatas[i].Name, binaryResourceDatas[i].Variant);
                if (ProcessingBinary != null)
                {
                    if (ProcessingBinary(fullName, (float) (i + 1) / binaryResourceDatas.Length))
                    {
                        m_BuildReport.LogWarning("The build has been canceled by user.");

                        if (m_ResBuildEventHandler != null)
                        {
                            m_BuildReport.LogInfo("Execute build event handler 'OnPostprocessPlatform' for '{0}'...",
                                platformName);
                            m_ResBuildEventHandler.OnPostprocessPlatform(platform, workingPath,
                                OutputPackageSelected, outputPackagePath, OutputFullSelected, outputFullPath,
                                OutputPackedSelected, outputPackedPath, false);
                        }

                        return false;
                    }
                }

                m_BuildReport.LogInfo("Start process binary '{0}' for '{1}'...", fullName, platformName);

                if (!ProcessBinary(platform, workingPath, outputPackagePath, outputFullPath, outputPackedPath,
                        ZipSelected, binaryResourceDatas[i].Name, binaryResourceDatas[i].Variant,
                        binaryResourceDatas[i].FileSystem))
                {
                    return false;
                }

                EditorUtilityx.DisplayProgressBar("Process Binaries", fullName, (i + 1) / binaryResourceDatas.Length);
                m_BuildReport.LogInfo("Process binary '{0}' for '{1}' complete.", fullName, platformName);
            }

            EditorUtilityx.ClearProgressBar();
            performanceProcessBinaries.Dispose();

            if (OutputPackageSelected)
            {
                ProcessPackageVersionList(resourceDatas, outputPackagePath, platform);
                m_BuildReport.LogInfo("Process package version list for '{0}' complete.", platformName);
            }

            if (OutputFullSelected)
            {
                VersionListData versionListData = ProcessUpdatableVersionList(resourceDatas, outputFullPath, platform);
                m_BuildReport.LogInfo(
                    "Process updatable version list for '{0}' complete, updatable version list path is '{1}', length is '{2}', hash code is '{3}[0x{3:X8}]', zip length is '{4}', zip hash code is '{5}[0x{5:X8}]'.",
                    platformName, versionListData.Path, versionListData.Length.ToString(), versionListData.HashCode,
                    versionListData.ZipLength.ToString(), versionListData.ZipHashCode);
                if (m_ResBuildEventHandler != null)
                {
                    m_BuildReport.LogInfo("Execute build event handler 'OnOutputUpdatableVersionListData' for '{0}'...",
                        platformName);
                    m_ResBuildEventHandler.OnOutputUpdatableVersionListData(platform, versionListData.Path, versionListData.Length, versionListData.HashCode,
                        versionListData.ZipLength, versionListData.ZipHashCode);
                }
            }

            if (OutputPackedSelected)
            {
                ProcessReadOnlyVersionList(resourceDatas, outputPackedPath, platform);
                m_BuildReport.LogInfo("Process read only version list for '{0}' complete.", platformName);
            }

            if (m_ResBuildEventHandler != null)
            {
                m_BuildReport.LogInfo("Execute build event handler 'OnPostprocessPlatform' for '{0}'...", platformName);
                m_ResBuildEventHandler.OnPostprocessPlatform(platform, workingPath, OutputPackageSelected,
                    outputPackagePath, OutputFullSelected, outputFullPath, OutputPackedSelected, outputPackedPath,
                    true);
            }

            //Copy To StreamingAssets
            //只复制与编辑器平台一致的资源
            if (EditorResourceSettings.CopyToStreamingAssets && IsCurrentPlatform(platform))
            {
                GetOutputPath(platform, out var workingPath2, out var packagePath, out var fullPath,
                    out var packedPath);
                ResourceTools.BuildResources.CopyToStreamingAsset(workingPath, packagePath, fullPath,
                    packedPath);
            }

            if (ProcessResourceComplete != null)
            {
                ProcessResourceComplete(platform);
            }

            m_BuildReport.LogInfo("Build resources for '{0}' success.", platformName);

            //string buildReportPath;
            //buildReportPath = Path.Combine(BuildReportPath,  manifestName);
            //if (!Directory.Exists(buildReportPath))
            //    Directory.CreateDirectory(buildReportPath);
            //m_BuildReport.SetOutputPath(buildReportPath);
            //m_BuildReport.SaveReport();
            return true;
        }


        /// <summary>
        /// 处理生成的 AssetBundle，非<see cref="LoadType.LoadFromBinary"/>资源
        /// </summary>
        private bool ProcessAssetBundle(Platform platform, string workingPath, string outputPackagePath, string outputFullPath, string outputPackedPath, bool zip, string name, string variant, string fileSystem)
        {
            string fullName = GetResourceFullName(name, variant);
            ResourceData resourceData = m_ResourceDatas[fullName];
            string workingName = Utility.Path.GetRegularPath(Path.Combine(workingPath, fullName.ToLower()));

            byte[] bytes = File.ReadAllBytes(workingName);
            int length = bytes.Length;
            int hashCode = Utility.Verifier.GetCrc32(bytes);
            int zipLength = length;
            int zipHashCode = hashCode;

            byte[] hashBytes = Utility.Converter.GetBytes(hashCode);
            if (resourceData.LoadType == LoadType.LoadFromMemoryAndQuickDecrypt)
            {
                bytes = Utility.Encryption.GetQuickXorBytes(bytes, hashBytes);
            }
            else if (resourceData.LoadType == LoadType.LoadFromMemoryAndDecrypt)
            {
                bytes = Utility.Encryption.GetXorBytes(bytes, hashBytes);
            }

            return ProcessOutput(platform, outputPackagePath, outputFullPath, outputPackedPath, zip, name, variant, fileSystem, resourceData, bytes, length, hashCode, zipLength, zipHashCode);
        }

        /// <summary>
        /// 处理二进制资源，<see cref="LoadType.LoadFromBinary"/>类型的资源
        /// </summary>
        private bool ProcessBinary(Platform platform, string workingPath, string outputPackagePath, string outputFullPath, string outputPackedPath, bool zip, string name, string variant, string fileSystem)
        {
            string fullName = GetResourceFullName(name, variant);
            ResourceData resourceData = m_ResourceDatas[fullName];
            string filePath;
            var first = resourceData.GetAssetDatas().FirstOrDefault();
            if (first != null && first.FilePath != null)
            {
                filePath = first.FilePath;
            }
            else
            {
                string assetName = resourceData.GetAssetNames()[0];
                string assetPath = Utility.Path.GetRegularPath(Application.dataPath.Substring(0, Application.dataPath.Length - AssetsStringLength) + assetName);
                filePath = assetPath;
            }
            byte[] bytes = File.ReadAllBytes(filePath);
            int length = bytes.Length;
            int hashCode = Utility.Verifier.GetCrc32(bytes);
            int zipLength = length;
            int zipHashCode = hashCode;

            byte[] hashBytes = Utility.Converter.GetBytes(hashCode);
            if (resourceData.LoadType == LoadType.LoadFromBinaryAndQuickDecrypt)
            {
                bytes = Utility.Encryption.GetQuickXorBytes(bytes, hashBytes);
            }
            else if (resourceData.LoadType == LoadType.LoadFromBinaryAndDecrypt)
            {
                bytes = Utility.Encryption.GetXorBytes(bytes, hashBytes);
            }

            return ProcessOutput(platform, outputPackagePath, outputFullPath, outputPackedPath, zip, name, variant, fileSystem, resourceData, bytes, length, hashCode, zipLength, zipHashCode);
        }

        /// <summary>
        /// 生成版本文件
        /// </summary>
        private void ProcessPackageVersionList(IEnumerable<ResourceData> resourceDatas, string outputPackagePath, Platform platform)
        {
            Asset[] originalAssets = m_ResourceCollection.GetAssets();
            PackageVersionList.Asset[] assets = new PackageVersionList.Asset[originalAssets.Length];
            for (int i = 0; i < assets.Length; i++)
            {
                Asset originalAsset = originalAssets[i];
                assets[i] = new PackageVersionList.Asset(FixName(originalAsset.Name), GetDependencyAssetIndexes(originalAsset.Name));
            }

            int index = 0;
            PackageVersionList.Resource[] resources = new PackageVersionList.Resource[m_ResourceCollection.ResourceCount];
            foreach (ResourceData resourceData in resourceDatas)
            {
                ResourceCode resourceCode = resourceData.GetCode(platform);
                if (resourceCode == null)
                    Debug.LogError("resourceCode null, " + resourceData.Name + ", variant: " + resourceData.Variant);
                resources[index++] = new PackageVersionList.Resource(resourceData.Name, resourceData.Variant, GetExtension(resourceData), (byte)resourceData.LoadType, resourceCode.Length, resourceCode.HashCode, GetAssetIndexes(resourceData));
            }

            string[] fileSystemNames = GetFileSystemNames(resourceDatas);
            PackageVersionList.FileSystem[] fileSystems = new PackageVersionList.FileSystem[fileSystemNames.Length];
            for (int i = 0; i < fileSystems.Length; i++)
            {
                fileSystems[i] = new PackageVersionList.FileSystem(fileSystemNames[i], GetResourceIndexesFromFileSystem(resourceDatas, fileSystemNames[i]));
            }

            string[] resourceGroupNames = GetResourceGroupNames(resourceDatas);
            PackageVersionList.ResourceGroup[] resourceGroups = new PackageVersionList.ResourceGroup[resourceGroupNames.Length];
            for (int i = 0; i < resourceGroups.Length; i++)
            {
                resourceGroups[i] = new PackageVersionList.ResourceGroup(resourceGroupNames[i], GetResourceIndexesFromResourceGroup(resourceDatas, resourceGroupNames[i]));
            }

            PackageVersionList versionList = new PackageVersionList(ApplicableGameVersion, InternalResourceVersion, assets, resources, fileSystems, resourceGroups, EditorResourceSettings.VariantAssetPath);
            PackageVersionListSerializer serializer = new PackageVersionListSerializer();
            serializer.RegisterSerializeCallback(0, BuiltinVersionListSerializer.PackageVersionListSerializeCallback_V0);
            serializer.RegisterSerializeCallback(1, BuiltinVersionListSerializer.PackageVersionListSerializeCallback_V1);
            serializer.RegisterSerializeCallback(2, BuiltinVersionListSerializer.PackageVersionListSerializeCallback_V2);
            string packageVersionListPath = Utility.Path.GetRegularPath(Path.Combine(outputPackagePath, RemoteVersionListFileName));
            using (FileStream fileStream = new FileStream(packageVersionListPath, FileMode.Create, FileAccess.Write))
            {
                if (!serializer.Serialize(fileStream, versionList))
                {
                    throw new GameFrameworkException("Serialize package version list failure.");
                }
            }
        }

        /// <summary>
        /// 生成远端版本文件
        /// </summary>
        private VersionListData ProcessUpdatableVersionList(IEnumerable<ResourceData> resourceDatas, string outputFullPath, Platform platform)
        {
            Asset[] originalAssets = m_ResourceCollection.GetAssets();
            UpdatableVersionList.Asset[] assets = new UpdatableVersionList.Asset[originalAssets.Length];
            for (int i = 0; i < assets.Length; i++)
            {
                Asset originalAsset = originalAssets[i];
                assets[i] = new UpdatableVersionList.Asset(FixName(originalAsset.Name), GetDependencyAssetIndexes(originalAsset.Name));
            }
            
            int index = 0;
            UpdatableVersionList.Resource[] resources = new UpdatableVersionList.Resource[m_ResourceCollection.ResourceCount];
            foreach (ResourceData resourceData in resourceDatas)
            {
                ResourceCode resourceCode = resourceData.GetCode(platform);
                resources[index++] = new UpdatableVersionList.Resource(resourceData.Name, resourceData.Variant, GetExtension(resourceData), (byte)resourceData.LoadType, resourceCode.Length, resourceCode.HashCode, resourceCode.ZipLength, resourceCode.ZipHashCode, GetAssetIndexes(resourceData));
            }

            string[] fileSystemNames = GetFileSystemNames(resourceDatas);
            UpdatableVersionList.FileSystem[] fileSystems = new UpdatableVersionList.FileSystem[fileSystemNames.Length];
            for (int i = 0; i < fileSystems.Length; i++)
            {
                fileSystems[i] = new UpdatableVersionList.FileSystem(fileSystemNames[i], GetResourceIndexesFromFileSystem(resourceDatas, fileSystemNames[i]));
            }

            string[] resourceGroupNames = GetResourceGroupNames(resourceDatas);
            UpdatableVersionList.ResourceGroup[] resourceGroups = new UpdatableVersionList.ResourceGroup[resourceGroupNames.Length];
            for (int i = 0; i < resourceGroups.Length; i++)
            {
                resourceGroups[i] = new UpdatableVersionList.ResourceGroup(resourceGroupNames[i], GetResourceIndexesFromResourceGroup(resourceDatas, resourceGroupNames[i]));
            }

            UpdatableVersionList versionList = new UpdatableVersionList(ApplicableGameVersion, InternalResourceVersion, assets, resources, fileSystems, resourceGroups, EditorResourceSettings.VariantAssetPath);
            UpdatableVersionListSerializer serializer = new UpdatableVersionListSerializer();
            serializer.RegisterSerializeCallback(0, BuiltinVersionListSerializer.UpdatableVersionListSerializeCallback_V0);
            serializer.RegisterSerializeCallback(1, BuiltinVersionListSerializer.UpdatableVersionListSerializeCallback_V1);
            serializer.RegisterSerializeCallback(2, BuiltinVersionListSerializer.UpdatableVersionListSerializeCallback_V2);

            string updatableVersionListPath = Utility.Path.GetRegularPath(Path.Combine(outputFullPath, RemoteVersionListFileName));
            using (FileStream fileStream = new FileStream(updatableVersionListPath, FileMode.Create, FileAccess.Write))
            {
                if (!serializer.Serialize(fileStream, versionList))
                {
                    throw new GameFrameworkException("Serialize updatable version list failure.");
                }
            }

            byte[] bytes = File.ReadAllBytes(updatableVersionListPath);
            int length = bytes.Length;
            int hashCode = Utility.Verifier.GetCrc32(bytes);
            bytes = Utility.ZipUtil.Compress(bytes);
            int zipLength = bytes.Length;
            File.WriteAllBytes(updatableVersionListPath, bytes);
            int zipHashCode = Utility.Verifier.GetCrc32(bytes);
            int dotPosition = RemoteVersionListFileName.LastIndexOf('.');
            string versionListFullNameWithCrc32 = Utility.Text.Format("{0}.{2:x8}.{1}", RemoteVersionListFileName.Substring(0, dotPosition), RemoteVersionListFileName.Substring(dotPosition + 1), hashCode);
            string updatableVersionListPathWithCrc32 = Utility.Path.GetRegularPath(Path.Combine(outputFullPath, versionListFullNameWithCrc32));
            File.Move(updatableVersionListPath, updatableVersionListPathWithCrc32);

            return new VersionListData(updatableVersionListPathWithCrc32, length, hashCode, zipLength, zipHashCode);
        }
        /// <summary>
        /// 生成本地版本文件
        /// </summary>
        private void ProcessReadOnlyVersionList(IEnumerable<ResourceData> resourceDatas, string outputPackedPath, Platform platform)
        {
            ResourceData[] packedResourceDatas = GetPackedResourceDatas(resourceDatas);

            LocalVersionList.Resource[] resources = new LocalVersionList.Resource[packedResourceDatas.Length];
            for (int i = 0; i < resources.Length; i++)
            {
                ResourceData resourceData = packedResourceDatas[i];
                ResourceCode resourceCode = resourceData.GetCode(platform);
                resources[i] = new LocalVersionList.Resource(resourceData.Name, resourceData.Variant, GetExtension(resourceData), (byte)resourceData.LoadType, resourceCode.Length, resourceCode.HashCode);
            }

            string[] packedFileSystemNames = GetFileSystemNames(packedResourceDatas);
            LocalVersionList.FileSystem[] fileSystems = new LocalVersionList.FileSystem[packedFileSystemNames.Length];
            for (int i = 0; i < fileSystems.Length; i++)
            {
                fileSystems[i] = new LocalVersionList.FileSystem(packedFileSystemNames[i], GetResourceIndexesFromFileSystem(packedResourceDatas, packedFileSystemNames[i]));
            }

            LocalVersionList versionList = new LocalVersionList(resources, fileSystems);
            ReadOnlyVersionListSerializer serializer = new ReadOnlyVersionListSerializer();
            serializer.RegisterSerializeCallback(0, BuiltinVersionListSerializer.LocalVersionListSerializeCallback_V0);
            serializer.RegisterSerializeCallback(1, BuiltinVersionListSerializer.LocalVersionListSerializeCallback_V1);
            serializer.RegisterSerializeCallback(2, BuiltinVersionListSerializer.LocalVersionListSerializeCallback_V2);
            string readOnlyVersionListPath = Utility.Path.GetRegularPath(Path.Combine(outputPackedPath, LocalVersionListFileName));
            using (FileStream fileStream = new FileStream(readOnlyVersionListPath, FileMode.Create, FileAccess.Write))
            {
                if (!serializer.Serialize(fileStream, versionList))
                {
                    throw new GameFrameworkException("Serialize read only version list failure.");
                }
            }
        }

        private int[] GetDependencyAssetIndexes(string assetName)
        {
            List<int> dependencyAssetIndexes = new List<int>();
            Asset[] assets = m_ResourceCollection.GetAssets();
            DependencyData dependencyData = m_ResourceAnalyzerController.GetDependencyData(assetName);
            foreach (Asset dependencyAsset in dependencyData.GetDependencyAssets())
            {
                for (int i = 0; i < assets.Length; i++)
                {
                    if (assets[i] == dependencyAsset)
                    {
                        dependencyAssetIndexes.Add(i);
                        break;
                    }
                }
            }

            dependencyAssetIndexes.Sort();
            return dependencyAssetIndexes.ToArray();
        }

        private int[] GetAssetIndexes(ResourceData resourceData)
        {
            Asset[] assets = m_ResourceCollection.GetAssets();
            string[] assetGuids = resourceData.GetAssetGuids();
            int[] assetIndexes = new int[assetGuids.Length];
            for (int i = 0; i < assetGuids.Length; i++)
            {
                assetIndexes[i] = Array.BinarySearch(assets, m_ResourceCollection.GetAsset(assetGuids[i]));
                if (assetIndexes[i] < 0)
                {
                    throw new GameFrameworkException("Asset is invalid.");
                }
            }

            return assetIndexes;
        }
        /// <summary>
        /// 获取所有<see cref="ResourceData.Packed"/>资源包
        /// </summary>
        private ResourceData[] GetPackedResourceDatas(IEnumerable<ResourceData> resourceDatas)
        {
            List<ResourceData> packedResourceDatas = new List<ResourceData>();
            foreach (ResourceData resourceData in resourceDatas)
            {
                if (!resourceData.Packed)
                {
                    continue;
                }

                packedResourceDatas.Add(resourceData);
            }

            return packedResourceDatas.ToArray();
        }

        /// <summary>
        /// 获取所有<see cref="ResourceData.FileSystem"/>资源包
        /// </summary>
        private string[] GetFileSystemNames(IEnumerable<ResourceData> resourceDatas)
        {
            HashSet<string> fileSystemNames = new HashSet<string>();
            foreach (ResourceData resourceData in resourceDatas)
            {
                if (resourceData.FileSystem == null)
                {
                    continue;
                }

                fileSystemNames.Add(resourceData.FileSystem);
            }

            return fileSystemNames.OrderBy(x => x).ToArray();
        }
        /// <summary>
        /// 获取指定<see cref="ResourceData.FileSystem"/>资源包
        /// </summary>
        private int[] GetResourceIndexesFromFileSystem(IEnumerable<ResourceData> resourceDatas, string fileSystemName)
        {
            int index = 0;
            List<int> resourceIndexes = new List<int>();
            foreach (ResourceData resourceData in resourceDatas)
            {
                if (resourceData.FileSystem == fileSystemName)
                {
                    resourceIndexes.Add(index);
                }

                index++;
            }

            resourceIndexes.Sort();
            return resourceIndexes.ToArray();
        }

        private string[] GetResourceGroupNames(IEnumerable<ResourceData> resourceDatas)
        {
            HashSet<string> resourceGroupNames = new HashSet<string>();
            foreach (ResourceData resourceData in resourceDatas)
            {
                foreach (string resourceGroup in resourceData.GetResourceGroups())
                {
                    resourceGroupNames.Add(resourceGroup);
                }
            }

            return resourceGroupNames.OrderBy(x => x).ToArray();
        }

        private int[] GetResourceIndexesFromResourceGroup(IEnumerable<ResourceData> resourceDatas, string resourceGroupName)
        {
            int index = 0;
            List<int> resourceIndexes = new List<int>();
            foreach (ResourceData resourceData in resourceDatas)
            {
                foreach (string resourceGroup in resourceData.GetResourceGroups())
                {
                    if (resourceGroup == resourceGroupName)
                    {
                        resourceIndexes.Add(index);
                        break;
                    }
                }

                index++;
            }

            resourceIndexes.Sort();
            return resourceIndexes.ToArray();
        }

        private void CreateFileSystems(IEnumerable<ResourceData> resourceDatas, string outputPath, Dictionary<string, IFileSystem> outputFileSystem)
        {
            string[] fileSystemNames = GetFileSystemNames(resourceDatas);
            if (fileSystemNames.Length > 0 && m_FileSystemManager == null)
            {
                m_FileSystemManager = GameFrameworkEntry.GetModule<IFileSystemManager>();
                m_FileSystemManager.SetFileSystemHelper(new FileSystemHelper());
            }

            foreach (string fileSystemName in fileSystemNames)
            {
                int fileCount = GetResourceIndexesFromFileSystem(resourceDatas, fileSystemName).Length;
                string fullPath = Utility.Path.GetRegularPath(Path.Combine(outputPath, Utility.Text.Format("{0}.{1}", fileSystemName, DefaultExtension)));
                string directory = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                IFileSystem fileSystem = m_FileSystemManager.CreateFileSystem(fullPath, FileSystemAccess.Write, fileCount, fileCount);
                outputFileSystem.Add(fileSystemName, fileSystem);
            }
        }

        private bool ProcessOutput(Platform platform, string outputPackagePath, string outputFullPath, string outputPackedPath, bool zip, string name, string variant, string fileSystem, ResourceData resourceData, byte[] bytes, int length, int hashCode, int zipLength, int zipHashCode)
        {
            string fullNameWithExtension = Utility.Text.Format("{0}.{1}", GetResourceFullName(name, variant), GetExtension(resourceData));

            //RawAssets 路径
            if (resourceData.IsLoadFromBinary && string.IsNullOrEmpty(fileSystem) && Utility.Path.DirectoryStartsWith(fullNameWithExtension, ResourceSettings.RawAssetsPath))
            {
                fullNameWithExtension = GetResourceFullName(name, variant);
            }

            if (OutputPackageSelected)
            {
                if (string.IsNullOrEmpty(fileSystem))
                {
                    string packagePath = Utility.Path.GetRegularPath(Path.Combine(outputPackagePath, fullNameWithExtension));
                    string packageDirectoryName = Path.GetDirectoryName(packagePath);
                    if (!Directory.Exists(packageDirectoryName))
                    {
                        Directory.CreateDirectory(packageDirectoryName);
                    }

                    File.WriteAllBytes(packagePath, bytes);
                }
                else
                {
                    if (!m_OutputPackageFileSystems[fileSystem].WriteFile(fullNameWithExtension, bytes))
                    {
                        return false;
                    }
                }
            }

            if (OutputPackedSelected && resourceData.Packed)
            {
                if (string.IsNullOrEmpty(fileSystem))
                {
                    string packedPath = Utility.Path.GetRegularPath(Path.Combine(outputPackedPath, fullNameWithExtension));
                    string packedDirectoryName = Path.GetDirectoryName(packedPath);
                    if (!Directory.Exists(packedDirectoryName))
                    {
                        Directory.CreateDirectory(packedDirectoryName);
                    }

                    File.WriteAllBytes(packedPath, bytes);
                }
                else
                {
                    if (!m_OutputPackedFileSystems[fileSystem].WriteFile(fullNameWithExtension, bytes))
                    {
                        return false;
                    }
                }
            }

            if (OutputFullSelected)
            {
                string fullNameWithCrc32AndExtension = variant != null ? Utility.Text.Format("{0}.{1}.{2:x8}.{3}", name, variant, hashCode, DefaultExtension) : Utility.Text.Format("{0}.{1:x8}.{2}", name, hashCode, DefaultExtension);
                string fullPath = Utility.Path.GetRegularPath(Path.Combine(outputFullPath, fullNameWithCrc32AndExtension)).ToLower();
                string fullDirectoryName = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(fullDirectoryName))
                {
                    Directory.CreateDirectory(fullDirectoryName);
                }

                if (zip)
                {
                    byte[] zipBytes = Utility.ZipUtil.Compress(bytes);
                    zipLength = zipBytes.Length;
                    zipHashCode = Utility.Verifier.GetCrc32(zipBytes);
                    File.WriteAllBytes(fullPath, zipBytes);
                }
                else
                {
                    File.WriteAllBytes(fullPath, bytes);
                }
            }

            resourceData.AddCode(platform, length, hashCode, zipLength, zipHashCode);
            return true;
        }

        private BuildAssetBundleOptions GetBuildAssetBundleOptions()
        {
            BuildAssetBundleOptions buildOptions = BuildAssetBundleOptions.None;

            if (UncompressedAssetBundleSelected)
            {
                buildOptions |= BuildAssetBundleOptions.UncompressedAssetBundle;
            }

            if (DisableWriteTypeTreeSelected)
            {
                buildOptions |= BuildAssetBundleOptions.DisableWriteTypeTree;
            }

            if (DeterministicAssetBundleSelected)
            {
                buildOptions |= BuildAssetBundleOptions.DeterministicAssetBundle;
            }

            if (ForceRebuildAssetBundleSelected)
            {
                buildOptions |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
            }

            if (IgnoreTypeTreeChangesSelected)
            {
                buildOptions |= BuildAssetBundleOptions.IgnoreTypeTreeChanges;
            }

            if (AppendHashToAssetBundleNameSelected)
            {
                buildOptions |= BuildAssetBundleOptions.AppendHashToAssetBundleName;
            }

            if (ChunkBasedCompressionSelected)
            {
                buildOptions |= BuildAssetBundleOptions.ChunkBasedCompression;
            }

            return buildOptions;
        }
        /// <summary>
        /// 生成打包的资源包数据
        /// </summary>
        private bool PrepareBuildData(out AssetBundleBuild[] assetBundleBuildDatas, out ResourceData[] assetBundleResourceDatas, out ResourceData[] binaryResourceDatas)
        {
            assetBundleBuildDatas = null;
            assetBundleResourceDatas = null;
            binaryResourceDatas = null;
            m_ResourceDatas.Clear();

            Resource[] resources = m_ResourceCollection.GetResources();
            foreach (Resource resource in resources)
            {
                string fullname;
                if (resource.IsLoadFromBinary && string.IsNullOrEmpty(resource.FileSystem) && Utility.Path.DirectoryStartsWith(resource.Name, ResourceSettings.RawAssetsPath))
                {
                    fullname = resource.FullName;
                }
                else
                {
                    fullname = FixName(resource.FullName);
                }
                m_ResourceDatas.Add(fullname, new ResourceData(resource.Name, resource.Variant, FixName(resource.FileSystem), resource.LoadType, resource.Packed, resource.GetResourceGroups()));
            }

            Asset[] assets = m_ResourceCollection.GetAssets();
            foreach (Asset asset in assets)
            {
                string assetName = asset.Name;
                if (string.IsNullOrEmpty(assetName))
                {
                    m_BuildReport.LogError("Can not find asset by guid '{0}'.", asset.Guid);
                    return false;
                }

                string assetFileFullName = Application.dataPath.Substring(0, Application.dataPath.Length - AssetsStringLength) + assetName;
                if (!File.Exists(assetFileFullName))
                {
                    m_BuildReport.LogError("Can not find asset '{0}'.", assetFileFullName);
                    return false;
                }

                int fileLength = (int)new System.IO.FileInfo(assetFileFullName).Length;
                int assetHashCode = 0;
                if (EditorResourceSettings.OutputAssetHash)
                {
                    byte[] assetBytes = File.ReadAllBytes(assetFileFullName);
                    assetHashCode = Utility.Verifier.GetCrc32(assetBytes);
                }

                List<string> dependencyAssetNames = new List<string>();
                DependencyData dependencyData = m_ResourceAnalyzerController.GetDependencyData(assetName);
                Asset[] dependencyAssets = dependencyData.GetDependencyAssets();
                foreach (Asset dependencyAsset in dependencyAssets)
                {
                    dependencyAssetNames.Add(FixName(dependencyAsset.Name));
                }

                dependencyAssetNames.Sort();

                m_ResourceDatas[asset.Resource.FullName].AddAssetData(asset.Guid, FixName(assetName), fileLength, assetHashCode, dependencyAssetNames.ToArray());
            }

            List<AssetBundleBuild> assetBundleBuildDataList = new List<AssetBundleBuild>();
            List<ResourceData> assetBundleResourceDataList = new List<ResourceData>();
            List<ResourceData> binaryResourceDataList = new List<ResourceData>();
            foreach (ResourceData resourceData in m_ResourceDatas.Values)
            {
                if (resourceData.AssetCount <= 0)
                {
                    m_BuildReport.LogWarning("Resource '{0}' has no asset.", GetResourceFullName(resourceData.Name, resourceData.Variant));
                    //跳过空资源包
                    continue;
                }

                if (resourceData.IsLoadFromBinary)
                {
                    binaryResourceDataList.Add(resourceData);
                }
                else
                {
                    assetBundleResourceDataList.Add(resourceData);

                    AssetBundleBuild build = new AssetBundleBuild();
                    build.assetBundleName = resourceData.Name;
                    build.assetBundleVariant = resourceData.Variant;
                    build.assetNames = resourceData.GetAssetPaths();
                    build.addressableNames = resourceData.GetAssetNames();
                    assetBundleBuildDataList.Add(build);
                }
            }

            assetBundleBuildDatas = assetBundleBuildDataList.ToArray();
            assetBundleResourceDatas = assetBundleResourceDataList.ToArray();
            binaryResourceDatas = binaryResourceDataList.ToArray();
            return true;
        }

        private static string GetResourceFullName(string name, string variant)
        {
            return !string.IsNullOrEmpty(variant) ? Utility.Text.Format("{0}.{1}", name, variant) : name;
        }

        public static BuildTarget GetBuildTarget(Platform platform)
        {
            switch (platform)
            {
                case Platform.Windows:
                    return BuildTarget.StandaloneWindows;

                case Platform.Windows64:
                    return BuildTarget.StandaloneWindows64;

                case Platform.MacOS:
#if UNITY_2017_3_OR_NEWER
                    return BuildTarget.StandaloneOSX;
#else
                    return BuildTarget.StandaloneOSXUniversal;
#endif
                case Platform.Linux:
                    return BuildTarget.StandaloneLinux64;

                case Platform.IOS:
                    return BuildTarget.iOS;

                case Platform.Android:
                    return BuildTarget.Android;

                case Platform.WindowsStore:
                    return BuildTarget.WSAPlayer;

                case Platform.WebGL:
                    return BuildTarget.WebGL;

                default:
                    throw new GameFrameworkException("Platform is invalid.");
            }
        }

        public static Platform BuildTargetToPlatform(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.Android:
                    return Platform.Android;
                case BuildTarget.iOS:
                    return Platform.IOS;
                case BuildTarget.StandaloneWindows:
                    return Platform.Windows;
                case BuildTarget.WebGL:
                    return Platform.WebGL;
                case BuildTarget.StandaloneWindows64:
                    return Platform.Windows64;
                case BuildTarget.WSAPlayer:
                    return Platform.WindowsStore;
                case BuildTarget.StandaloneOSX:
                    return Platform.MacOS;
                case BuildTarget.StandaloneLinux64:
                    return Platform.Linux;
            }
            throw new GameFrameworkException("Platform is invalid. " + buildTarget);
        }

        private static bool IsRawResource(ResourceData data)
        {
            //资源包名包含扩展名
            if (data.IsLoadFromBinary && string.IsNullOrEmpty(data.FileSystem) && Utility.Path.DirectoryStartsWith(data.Name, ResourceSettings.RawAssetsPath))
            {
                return true;
            }
            return false;
        }

        private static string GetExtension(ResourceData data)
        {
            if (data.IsLoadFromBinary)
            {
                if (IsRawResource(data))
                    return null;

                var assetNames = data.GetAssetNames();
                if (assetNames.Length <= 0)
                {
                    Debug.LogErrorFormat("Resource [{0}] has no asset files", data.Name);
                    //索引报错
                    return DefaultExtension;
                }

                string assetName = assetNames[0];
                int position = assetName.LastIndexOf('.');
                if (position >= 0)
                {
                    return assetName.Substring(position + 1);
                }
            }

            return DefaultExtension;
        }

        /// <summary>
        /// Assets开头纯大小写都会有打不出来的问题，所以如果Assets开头，则替换掉。
        /// </summary>
        /// <returns></returns>
        internal static string FixName(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
                return assetName;
            var lowerName = assetName.ToLower();
            //if (lowerName.StartsWith("assets/"))
            //{
            //    return "A"+lowerName.Substring(1);
            //}

            return lowerName;
        }

        public static bool ParseResVersion(string path, out int resVer, out string appVer)
        {
            string dirName = Path.GetFileName(path);
            resVer = -1;
            appVer = null;
            int tmpResVer = -1;
            int index = dirName.LastIndexOf('_');
            if (!int.TryParse(dirName.Substring(index + 1), out tmpResVer))
                return false;
            string tmpAppVer = dirName.Substring(0, index);

            resVer = tmpResVer;
            appVer = tmpAppVer;
            return true;
        }

        public string FindLastestVersionFolderName()
        {
            string lastestDir = null;
            int resVer = -1;

            int tmpResVer = -1;
            string tmpAppVer;
            foreach (var dir in Directory.GetDirectories(Path.Combine(OutputDirectory, "Full")))
            {
                string rootDir = Path.Combine(dir, CurrentPlatform.ToString());
                if (!Directory.Exists(rootDir))
                    continue;
                if (!Directory.GetFiles(rootDir, "GameFrameworkVersion*").Any())
                    continue;

                if (!ParseResVersion(dir, out tmpResVer, out tmpAppVer))
                    continue;

                if (lastestDir != null)
                {
                    if (tmpResVer < resVer)
                        continue;
                }

                lastestDir = dir;
                resVer = tmpResVer;
            }
            if (lastestDir != null)
                return Path.GetFileName(lastestDir);
            return null;
        }


        private bool BuildBinaryResources(Platform platform, ResourceData[] binaryResourceDatas)
        {
            string platformName = EditorResourceSettings.GetPlatformName(platform);
            string workingPath, outputPackagePath, outputFullPath, outputPackedPath;

            GetOutputPath(platform,  out workingPath, out outputPackagePath, out outputFullPath, out outputPackedPath);

            var performanceProcessBinaries = new PerformanceSample("Process Binaries");

            // Process Binaries
            for (int i = 0; i < binaryResourceDatas.Length; i++)
            {
                string fullName = GetResourceFullName(binaryResourceDatas[i].Name, binaryResourceDatas[i].Variant);
                if (ProcessingBinary != null)
                {
                    if (ProcessingBinary(fullName, (float)(i + 1) / binaryResourceDatas.Length))
                    {
                        m_BuildReport.LogWarning("The build has been canceled by user.");

                        if (m_ResBuildEventHandler != null)
                        {
                            m_BuildReport.LogInfo("Execute build event handler 'OnPostprocessPlatform' for '{0}'...", platformName);
                            m_ResBuildEventHandler.OnPostprocessPlatform(platform,  workingPath, OutputPackageSelected, outputPackagePath, OutputFullSelected, outputFullPath, OutputPackedSelected, outputPackedPath, false);
                        }

                        return false;
                    }
                }

                m_BuildReport.LogInfo("Start process binary '{0}' for '{1}'...", fullName, platformName);

                if (!ProcessBinary(platform, workingPath, outputPackagePath, outputFullPath, outputPackedPath, ZipSelected, binaryResourceDatas[i].Name, binaryResourceDatas[i].Variant, binaryResourceDatas[i].FileSystem))
                {
                    return false;
                }
                EditorUtilityx.DisplayProgressBar("Process Binaries", fullName, (i + 1) / binaryResourceDatas.Length);
                m_BuildReport.LogInfo("Process binary '{0}' for '{1}' complete.", fullName, platformName);
            }
            EditorUtilityx.ClearProgressBar();
            performanceProcessBinaries.Dispose();
            return true;
        }

        public bool BuildPartResources(Resource[] partResources, string assetPathRoot)
        {
            assetPathRoot = assetPathRoot.Replace('\\', '/').ToLower();
            string rootPath2 = assetPathRoot;
            if (!assetPathRoot.EndsWith("/"))
                rootPath2 = assetPathRoot + "/";

            var platform = CurrentPlatform;
            if (!IsPlatformSelected(platform))
            {
                return true;
            }


            try
            {
                IsPartBuild = true;

                m_OutputPackageFileSystems.Clear();
                m_OutputPackedFileSystems.Clear();
                m_ResourceCollection.Clear();
                m_ResourceDatas.Clear();
                List<ResourceInfo> updateResourceInfos = new List<ResourceInfo>();
                ResourceData[] binaryResourceDatas;

                string workingPath, outputPackagePath, outputFullPath, outputPackedPath;

                GetOutputPath(platform, out workingPath, out outputPackagePath, out outputFullPath, out outputPackedPath);

                string path;
                //清理 lua 资源包
                if (OutputPackageSelected)
                {
                    path = Path.Combine(outputPackagePath, assetPathRoot);
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }
                }
                if (OutputPackedSelected)
                {
                    path = Path.Combine(outputPackedPath, assetPathRoot);
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }
                }
                if (OutputFullSelected)
                {
                    path = Path.Combine(outputFullPath, assetPathRoot);
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }
                }


                string platformName = EditorResourceSettings.GetPlatformName(platform);

                foreach (Resource resource in partResources)
                {
                    var fullname = resource.FullName;
                    var resourceData = new ResourceData(resource.Name, resource.Variant, FixName(resource.FileSystem), resource.LoadType, resource.Packed, resource.GetResourceGroups());
                    foreach (var asset in resource.GetAssets())
                    {
                        var assetData = resourceData.AddAssetData(null, asset.Name.ToLower(), 0, 0, asset.DependencyAssetPaths == null ? new string[0] : asset.DependencyAssetPaths.Select(o => o.ToLower()).ToArray());
                        assetData.FilePath = asset.FilePath;
                    }
                    m_ResourceDatas.Add(fullname, resourceData);
                    ResourceInfo resourceInfo = new ResourceInfo(resource);
                    updateResourceInfos.Add(resourceInfo);
                }

                binaryResourceDatas = m_ResourceDatas.Values.Where(o => o.IsLoadFromBinary).ToArray();

                BuildAssetBundleOptions buildAssetBundleOptions = GetBuildAssetBundleOptions();
                m_BuildReport.Initialize(BuildReportPath, ProductName, CompanyName, GameIdentifier, GameFrameworkVersion, UnityVersion, ApplicableGameVersion, InternalResourceVersion,
                    SelectedPlatforms, ZipSelected, (int)buildAssetBundleOptions, m_ResourceDatas);

                m_BuildReport.LogInfo("Start build resources for '{0}'...", platform);

                if (PartBuildTriggerEvent && m_ResBuildEventHandler != null)
                {
                    m_BuildReport.LogInfo("Execute build event handler 'OnPreprocessAllPlatforms'...");
                    m_ResBuildEventHandler.OnPreprocessAllPlatforms(ProductName, CompanyName, GameIdentifier, GameFrameworkVersion, UnityVersion, ApplicableGameVersion, InternalResourceVersion, buildAssetBundleOptions, ZipSelected, OutputDirectory, WorkingPath, OutputPackageSelected, OutputPackagePath, OutputFullSelected, OutputFullPath, OutputPackedSelected, OutputPackedPath, BuildReportPath);
                }

                if (OutputPackageSelected)
                {
                    CreateFileSystems(m_ResourceDatas.Values, outputPackagePath, m_OutputPackageFileSystems);
                }

                if (OutputPackedSelected)
                {
                    CreateFileSystems(m_ResourceDatas.Values.Where(o => o.Packed).ToArray(), outputPackedPath, m_OutputPackedFileSystems);
                }

                //生成二进制包
                BuildBinaryResources(platform, binaryResourceDatas);

                for (int i = 0; i < updateResourceInfos.Count; i++)
                {
                    var resourceInfo = updateResourceInfos[i];
                    var fullname = resourceInfo.FullName;
                    var resourceData = m_ResourceDatas[fullname];

                    var code = resourceData.GetCode(platform);
                    resourceInfo.Length = code.Length;
                    resourceInfo.HashCode = code.HashCode;
                    resourceInfo.ZipLength = code.ZipLength;
                    resourceInfo.ZipHashCode = code.ZipHashCode;
                    updateResourceInfos[i] = resourceInfo;
                }

                ResourceInfo[] resourceInfos;
                VersionListData versionListData;
                //生成版本文件
                {

                    string versionFilePath = Directory.GetFiles(outputFullPath, RemoteVersionListFileName.Substring(0, RemoteVersionListFileName.LastIndexOf('.'))+"*").FirstOrDefault();
                    if (versionFilePath == null)
                        throw new Exception("Not found version file: " + outputFullPath);
                    UpdatableVersionList versionList;
                    byte[] bytes = File.ReadAllBytes(versionFilePath);
                    bytes = Utility.ZipUtil.Decompress(bytes);
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        versionList = ResourceInfo.GetFullVersionSerializer().Deserialize(ms);
                        if (!versionList.IsValid)
                        {
                            throw new GameFrameworkException("Deserialize version list failure. " + versionFilePath);
                        }
                    }

                    //版本文件没有存Packed字段，需要对比Full和Packed版本恢复Packed字段
                    string localVersionFilePath = Utility.Path.GetRegularPath(Path.Combine(outputPackedPath, LocalVersionListFileName));
                    LocalVersionList localVersionList = ResourceInfo.GetPackedVersionSerializer().Deserialize(File.ReadAllBytes(localVersionFilePath));
                    HashSet<string> packedFullNames = new HashSet<string>();
                    foreach (var item in localVersionList.GetResources())
                    {
                        string fullName = item.Name;
                        if (!string.IsNullOrEmpty(item.Variant))
                            fullName += "." + item.Variant;
                        packedFullNames.Add(fullName);
                    }

                    SortedDictionary<string, ResourceInfo> newResourceInfos = new SortedDictionary<string, ResourceInfo>();
                    foreach (var item in versionList.GetResourceInfos())
                    {
                        item.Resource.Packed = packedFullNames.Contains(item.FullName);
                        newResourceInfos[item.FullName] = item;
                    }

                    //删除资源包
                    foreach (var item in newResourceInfos.ToArray())
                    {
                        var resInfo = item.Value;
                        if (resInfo.Name.StartsWith(rootPath2, StringComparison.InvariantCultureIgnoreCase))
                        {
                            newResourceInfos.Remove(item.Key);
                        }
                    }
                    //添加变化的资源包
                    foreach (var item in updateResourceInfos)
                        newResourceInfos[item.FullName] = item;
                    resourceInfos = newResourceInfos.Values.ToArray();
                    versionList = ResourceInfo.CreateUpdatableVersionList(resourceInfos, ApplicableGameVersion, InternalResourceVersion, EditorResourceSettings.VariantAssetPath);

                    WriteUpdatableVersionList(versionFilePath, versionList, true, out versionListData);
                    m_BuildReport.LogInfo("Process updatable version list for '{0}' complete, updatable version list path is '{1}', length is '{2}', hash code is '{3}[0x{3:X8}]', zip length is '{4}', zip hash code is '{5}[0x{5:X8}]'.", platformName, versionListData.Path, versionListData.Length.ToString(), versionListData.HashCode, versionListData.ZipLength.ToString(), versionListData.ZipHashCode);

                }

                if (OutputPackageSelected)
                {
                    string versionFilePath = Utility.Path.GetRegularPath(Path.Combine(outputPackagePath, RemoteVersionListFileName));
                    PackageVersionList versionList;

                    versionList = ResourceInfo.CreatePackageVersionList(resourceInfos, ApplicableGameVersion, InternalResourceVersion, EditorResourceSettings.VariantAssetPath);

                    using (FileStream fileStream = new FileStream(versionFilePath, FileMode.Create, FileAccess.Write))
                    {
                        if (!ResourceInfo.GetPackageVersionSerializer().Serialize(fileStream, versionList))
                        {
                            throw new GameFrameworkException("Serialize package version list failure.");
                        }
                    }
                    m_BuildReport.LogInfo("Process package version list for '{0}' complete.", platformName);
                }

                if (OutputPackedSelected)
                {
                    string versionFilePath = Utility.Path.GetRegularPath(Path.Combine(outputPackedPath, LocalVersionListFileName));
                    LocalVersionList versionList;

                    List<ResourceInfo> newResources = new List<ResourceInfo>();
                    foreach (var item in resourceInfos)
                    {
                        var newItem = item;
                        if (!newItem.Resource.Packed)
                            continue;
                        //更新扩展名
                        newItem.Extension = ResourceInfo.GetExtension(newItem);
                        newResources.Add(newItem);
                    }
                    versionList = ResourceInfo.CreateLocalVersionList(newResources.ToArray());

                    using (FileStream fileStream = new FileStream(versionFilePath, FileMode.Create, FileAccess.Write))
                    {
                        if (!ResourceInfo.GetPackedVersionSerializer().Serialize(fileStream, versionList))
                        {
                            throw new GameFrameworkException("Serialize package version list failure.");
                        }
                    }

                    m_BuildReport.LogInfo("Process read only version list for '{0}' complete.", platformName);
                }

                if (PartBuildTriggerEvent && m_ResBuildEventHandler != null)
                {
                    m_BuildReport.LogInfo("Execute build event handler 'OnOutputUpdatableVersionListData' for '{0}'...", platformName);
                    m_ResBuildEventHandler.OnOutputUpdatableVersionListData(platform, versionListData.Path, versionListData.Length, versionListData.HashCode, versionListData.ZipLength, versionListData.ZipHashCode);
                }

                if (PartBuildTriggerEvent && m_ResBuildEventHandler != null)
                {
                    m_BuildReport.LogInfo("Execute build event handler 'OnPostprocessPlatform' for '{0}'...", platformName);
                    m_ResBuildEventHandler.OnPostprocessPlatform(platform, workingPath, OutputPackageSelected, outputPackagePath, OutputFullSelected, outputFullPath, OutputPackedSelected, outputPackedPath, true);
                }

                if (ProcessResourceComplete != null)
                {
                    ProcessResourceComplete(platform);
                }

                m_BuildReport.LogInfo("Build resources for '{0}' success.", platformName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                IsPartBuild = false;
                m_OutputPackageFileSystems.Clear();
                m_OutputPackedFileSystems.Clear();
                if (m_FileSystemManager != null)
                {
                    GameFrameworkEntry.Shutdown();
                    m_FileSystemManager = null;
                }

            }

            return true;
        }

        public static void WriteUpdatableVersionList(string filePath, UpdatableVersionList versionList)
        {
            GameFrameworkSerializer<UpdatableVersionList> serializer = ResourceInfo.GetFullVersionSerializer();

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                if (!serializer.Serialize(fileStream, versionList))
                {
                    throw new GameFrameworkException("Serialize updatable version list failure.");
                }
            }
        }

        static void WriteUpdatableVersionList(string filePath, UpdatableVersionList versionList, bool zip, out VersionListData versionListData)
        {
            versionListData = null;
            WriteUpdatableVersionList(filePath, versionList);

            if (zip)
            {
                byte[] bytes = File.ReadAllBytes(filePath);
                int length = bytes.Length;
                int hashCode = Utility.Verifier.GetCrc32(bytes);
                bytes = Utility.ZipUtil.Compress(bytes);
                int zipLength = bytes.Length;
                File.WriteAllBytes(filePath, bytes);
                int zipHashCode = Utility.Verifier.GetCrc32(bytes);
                string fileName = RemoteVersionListFileName;

                int dotPosition = fileName.LastIndexOf('.');
                string fileNameWithCrc32 = Utility.Text.Format("{0}.{2:x8}.{1}", fileName.Substring(0, dotPosition), fileName.Substring(dotPosition + 1), hashCode);
                string filePathWithCrc32 = Utility.Path.GetRegularPath(Path.Combine(Path.GetDirectoryName(filePath), fileNameWithCrc32));
                File.Move(filePath, filePathWithCrc32);
                versionListData = new VersionListData(filePathWithCrc32, length, hashCode, zipLength, zipHashCode);
            }

        }

    }
}
