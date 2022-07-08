using DefaultNamespace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ND.Managers.ResourceMgr.Editor;
using ND.Managers.ResourceMgr.Editor.ResourceTools;
using ND.Managers.ResourceMgr.Framework.Resource;
using ND.Managers.ResourceMgr.Runtime;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{

    /// <summary>
    /// 资源配置窗口，设置 <see cref="EditorResourceSettings"/>
    /// </summary>
    public class EditorResourceSettingsWindow : EditorWindow
    {
        private Vector2 m_ScrollPos;
        private Dictionary<ResourceRule, List<string[]>> m_PreviewAssets;
        private ResourceCollection m_Collection;
        private ResourceBuilderController m_Controller = null;
        private bool m_Reload = false;
        private string[] resourceModeDisplays = new string[] { "Editor", "Package", "Updatable", };
        private static System.Type[] m_AddressableProviderTypes;
        private static System.Type[] m_PreprocessBuildTypes;
        private static string[] variants;
        private bool isShowAdvanced;
        

        System.Type[] AddressableProviderTypes
        {
            get
            {
                if (m_AddressableProviderTypes == null)
                {
                    List<System.Type> types = new List<System.Type>();
                    foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                        .Referenced(typeof(IAddressableProvider).Assembly)
                        .SelectMany(o => o.GetTypes())
                        .Where(o => !o.IsAbstract && typeof(IAddressableProvider).IsAssignableFrom(o)))
                    {
                        types.Add(type);
                    }
                    m_AddressableProviderTypes = types.ToArray();
                }
                return m_AddressableProviderTypes;
            }
        }

        System.Type[] PreprocessBuildTypes
        {
            get
            {
                if (m_PreprocessBuildTypes == null)
                {
                    List<System.Type> types = new List<System.Type>();
                    foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                        .Referenced(typeof(IResourcePreprocessBuild).Assembly)
                        .SelectMany(o => o.GetTypes())
                        .Where(o => !o.IsAbstract && typeof(IResourcePreprocessBuild).IsAssignableFrom(o)))
                    {
                        types.Add(type);
                    }
                    m_PreprocessBuildTypes = types.ToArray();
                }
                return m_PreprocessBuildTypes;
            }
        }


        [MenuItem(EditorUtilityx.MenuPrefix + "Build", priority = -29)]
        static void Menu_Build()
        {
            BuildResources.Run();
        }
        [MenuItem(EditorUtilityx.MenuPrefix + "Analysis", priority = -27)]
        static void Menu_Analysis()
        {
            BuildResources.RunAnalysisByLatestConfig();
        }
        [MenuItem(EditorUtilityx.MenuPrefix + "Settings", priority = -27)]
        static void Menu_Settings()
        {
            GetWindow<EditorResourceSettingsWindow>().Show();
        }
        [MenuItem(EditorUtilityx.MenuPrefix + "Editor Mode", priority = -11)]
        static void Menu_EditorMode()
        {
            ResourceUserSettings.EditorMode = true;
            if (ResourceUserSettings.EditorMode)
            {
                ResourceSceneTools.MarkScene();
            }
            else
            {
                ResourceSceneTools.UnMarkScene();
            }
        }
        [MenuItem(EditorUtilityx.MenuPrefix + "Editor Mode", validate = true)]
        static bool Menu_EditorMode_Validate()
        {
            Menu.SetChecked(EditorUtilityx.MenuPrefix + "Editor Mode", ResourceUserSettings.EditorMode);
            return true;
        }

        [MenuItem(EditorUtilityx.MenuPrefix + "Package Mode", priority = -10)]
        static void Menu_PackageMode()
        {
            ResourceUserSettings.ResourceMode = ResourceMode.Package;
            if (ResourceUserSettings.EditorMode)
            {
                ResourceSceneTools.MarkScene();
            }
            else
            {
                ResourceSceneTools.UnMarkScene();
            }
        }
        [MenuItem(EditorUtilityx.MenuPrefix + "Package Mode", validate = true)]
        static bool Menu_PackageMode_Validate()
        {
            Menu.SetChecked(EditorUtilityx.MenuPrefix + "Package Mode", ResourceUserSettings.ResourceMode == ResourceMode.Package);
            return true;
        }

        [MenuItem(EditorUtilityx.MenuPrefix + "Updatable Mode", priority = -10)]
        static void Menu_UpdatableMode()
        {
            ResourceUserSettings.ResourceMode = ResourceMode.Updatable;
            if (ResourceUserSettings.EditorMode)
            {
                ResourceSceneTools.MarkScene();
            }
            else
            {
                ResourceSceneTools.UnMarkScene();
            }
        }
        [MenuItem(EditorUtilityx.MenuPrefix + "Updatable Mode", validate = true)]
        static bool Menu_UpdatableMode_Validate()
        {
            Menu.SetChecked(EditorUtilityx.MenuPrefix + "Updatable Mode", ResourceUserSettings.ResourceMode == ResourceMode.Updatable);
            return true;
        }

        [MenuItem(EditorUtilityx.AdvancedMenuPrefix + "Clear Local")]
        static void ClearLocalResources()
        {
            if (!EditorUtility.DisplayDialog("Clear", "Clear local resources", "Yes", "No"))
                return;

            string localPath = Application.persistentDataPath;

            foreach (var fileOrDirectory in new string[] { "assets",
                ResourceBuilderController.LocalVersionListFileName,
                ResourceBuilderController.RemoteVersionListFileName })
            {
                string path = Path.Combine(localPath, fileOrDirectory);
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                else if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        private void OnEnable()
        {
            this.titleContent = new GUIContent("Resources Settings");
            if (m_Controller == null)
            {
                m_Controller = new ResourceBuilderController();
            }
            RefreshAllVariants();
            m_Reload = true;
        }

        private void Update()
        {
            if (m_Reload)
            {
                m_Reload = false;
                Load();
            }
        }

        void Load()
        {
            m_Controller.Load(incrementalVersion: false);
        }

        void RefreshAllVariants()
        {
            if (m_Collection == null)
            {
                m_Collection = new ResourceCollection();
                m_Collection.Load();
            }
            HashSet<string> allVariants = new HashSet<string>();
            foreach (string[] item in m_Collection.GetAssetBundleNames().Values)
            {
                string variant = item[1];
                if (!string.IsNullOrEmpty(variant))
                {
                    allVariants.Add(variant);
                }
            }
            if (!allVariants.Contains("none"))
                allVariants.Add("none");
            variants = allVariants.ToArray();
        }

        private void OnGUI()
        {
            EditorResourceSettings.EnsureLoadConfig();
            EditorGUIUtility.labelWidth = 200;

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button(new GUIContent("Analysis", "分析资源"), GUILayout.MinHeight(25)))
                {
                    //Unity button事件有问题，可能会执行两次或多次，所以写成 delayCall
                    EditorApplication.delayCall += () =>
                    {
                        BuildResources.RunAnalysisByLatestConfig();
                        m_Reload = true;
                    };
                }
                if (GUILayout.Button(new GUIContent("Build", "生成资源"), GUILayout.MinHeight(25)))
                {

                    EditorApplication.delayCall += () =>
                    {
                        BuildResources.Run(m_Controller.InternalResourceVersion + 1);
                        m_Reload = true;
                    };

                }
            }
            bool controllerChanged = false, editorResourceSettingsChanged = false;

            using (var checker = new EditorGUI.ChangeCheckScope())
            {
                using (var sv = new GUILayout.ScrollViewScope(m_ScrollPos))
                {
                    m_ScrollPos = sv.scrollPosition;

                    using (var checkerController = new EditorGUI.ChangeCheckScope())
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            //资源版本
                            EditorGUILayout.LabelField(new GUIContent("Resource Version", "资源版本"), new GUIContent($"{Application.version} ({m_Controller.InternalResourceVersion})"));

                            //内部资源版本
                            GUILayout.Label(new GUIContent("Internal Resource Version", "内部资源版本"), GUILayout.ExpandWidth(false));
                            m_Controller.InternalResourceVersion = EditorGUILayout.DelayedIntField(m_Controller.InternalResourceVersion, GUILayout.ExpandWidth(false));


                            ////资源版本
                            //EditorGUILayout.PrefixLabel(new GUIContent("Resource Version", "资源版本"));
                            //GUILayout.Label(new GUIContent($"{Application.version} ("),GUILayout.ExpandWidth(false));

                            ////内部资源版本                            
                            //m_Controller.InternalResourceVersion = EditorGUILayout.IntField(m_Controller.InternalResourceVersion + 1, GUILayout.ExpandWidth(false),GUILayout.Width(30)) - 1;
                            //GUILayout.Label(")", GUILayout.ExpandWidth(false));
                        }

                        GUIResourceMode();

                        using (var checkerUserSettings = new EditorGUI.ChangeCheckScope())
                        {

                            //设置变体
                            string[] variants = EditorResourceSettingsWindow.variants;
                            int variantIndex = -1;
                            if (!string.IsNullOrEmpty(ResourceUserSettings.Variant))
                                variantIndex = Array.IndexOf(variants, ResourceUserSettings.Variant);

                            variantIndex = EditorGUILayout.Popup(new GUIContent("Variant", "设置变体，只对编辑器有效"), variantIndex, variants);
                            if (variantIndex >= 0 && variantIndex < variants.Length)
                            {
                                ResourceUserSettings.Variant = variants[variantIndex].Equals("none")?null:variants[variantIndex];
                                ResourceUserSettings.VariantPrefixUrl = EditorResourceSettings.VariantAssetPath;
                            }

                            if (checkerUserSettings.changed)
                            {
                                GUI.changed = false;
                            }
                        }



                        bool oldGUIChanged = GUI.changed;
                        isShowAdvanced = EditorGUILayout.BeginFoldoutHeaderGroup(isShowAdvanced, new GUIContent("Advanced"));
                        EditorGUILayout.EndFoldoutHeaderGroup();
                        //切换时会导致 changed 改变
                        GUI.changed = oldGUIChanged;

                        if (isShowAdvanced)
                        {
                            using (new EditorGUILayoutx.IndentLevelVerticalScope())
                            {
                                m_Controller.Platforms = (Platform)EditorGUILayout.EnumFlagsField(new GUIContent("Platform", "生成资源平台，建议 'FollowProject' 保持和编辑器一致"), m_Controller.Platforms);

                                using (var checkerEditor = new EditorGUI.ChangeCheckScope())
                                {
                                    EditorResourceSettings.AssetBundleNameFormat = EditorGUILayout.DelayedTextField(new GUIContent("Bundle Name Format", "资源包名称格式"), EditorResourceSettings.AssetBundleNameFormat);
                                    EditorResourceSettings.PerformanceLogEnabled = EditorGUILayout.Toggle(new GUIContent("Editor Performance Log", "资源打包性能日志"), EditorResourceSettings.PerformanceLogEnabled);
                                    EditorResourceSettings.CheckMissingPrefab = EditorGUILayout.Toggle(new GUIContent("Check Missing Prefab", "检查丢失的 prefab，输出错误日志"), EditorResourceSettings.CheckMissingPrefab);
                                    EditorResourceSettings.OutputAssetHash = EditorGUILayout.Toggle(new GUIContent("Output Asset Hash", "Report 是否输出资源哈希，不开启可以提高构建性能"), EditorResourceSettings.OutputAssetHash);

                                    if (checkerEditor.changed)
                                    {
                                        editorResourceSettingsChanged = true;
                                    }
                                }
                                using (var checkerRuntime = new EditorGUI.ChangeCheckScope())
                                {
                                    ResourceSettings.PlatformNameStyle = (ResourceSettings.PlatformNameStyles)EditorGUILayout.EnumPopup(new GUIContent("Platform Name Style", "平台名称样式, 新项目建议选择 'Invariant' \nDefault: 'Platform.IOS'\nInvariant 区分大小写: 'BuildTarget.iOS'\nLower 转小写: 'ios'"), ResourceSettings.PlatformNameStyle);
                                    ResourceSettings.DownloadRequestHeaderGzip = EditorGUILayout.Toggle(new GUIContent("Download Request Header Gzip", "下载请求 Header"), ResourceSettings.DownloadRequestHeaderGzip);
                                    ResourceSettings.EditorModeReloadShader = EditorGUILayout.Toggle(new GUIContent("EditorMode Reload Shader", "编辑器模式加载AssetBundle后 重新加载 Shader"), ResourceSettings.EditorModeReloadShader);

                                    var enabled = (ResourceSettings.Options & ResourceSettings.ResourceOptions.AutoGCStackTrace) != 0;
                                    enabled = EditorGUILayout.Toggle("Auto GC Stack Trace", enabled);
                                    if (enabled)
                                    {
                                        ResourceSettings.Options |= ResourceSettings.ResourceOptions.AutoGCStackTrace;
                                    }
                                    else
                                    {
                                        ResourceSettings.Options &= ~ResourceSettings.ResourceOptions.AutoGCStackTrace;
                                    }

                                    if (checkerRuntime.changed)
                                    {
                                        GUI.changed = false;
                                    }
                                }
                            }
                        }

                        m_Controller.ZipSelected = EditorGUILayout.Toggle("Zip All Resources", m_Controller.ZipSelected);

                        using (new GUILayout.HorizontalScope())
                        {
                            m_Controller.OutputDirectory = new GUIContent("Output Path", "资源输出目录").FolderField(m_Controller.OutputDirectory, "", relativePath: ".");

                            if (string.IsNullOrEmpty(m_Controller.OutputDirectory))
                            {
                                m_Controller.OutputDirectory = "output";

                                GUI.changed = true;
                            }

                            if (GUILayout.Button(new GUIContent("Open", "打开输出目录"), GUILayout.ExpandWidth(false)))
                            {
                                EditorUtilityx.OpenFolder(m_Controller.OutputDirectory);
                            }
                        }

                        if (checkerController.changed)
                        {
                            controllerChanged = true;
                            GUI.changed = false;
                        }

                    }

                    //ResourceSettings.ClearOnRefresh = EditorGUILayout.Toggle(new GUIContent("Clear On Refresh", "分析之前先清理"), ResourceSettings.ClearOnRefresh);

                    //ResourceSettings.OutputPath = EditorGUILayout.DelayedTextField(new GUIContent("Output Path"), ResourceSettings.OutputPath);


                    //using (new EditorGUI.DisabledGroupScope(!ResourceSettings.AutoRefreshOnBuild))
                    //{

                    //}

                    using (var checkerRuntime = new EditorGUI.ChangeCheckScope())
                    { 
                        ResourceSettings.RawAssetsPath = EditorGUILayout.DelayedTextField(new GUIContent("RawAssets Path", "原始资源目录"), ResourceSettings.RawAssetsPath);
                        ResourceSettings.AtlasPath = new GUIContent("Atlas Path", "运行时图集的根目录").FolderField(ResourceSettings.AtlasPath, "", relativePath: ".", directorySeparator: '/');

                        using (var ignoreChanged = new EditorGUI.ChangeCheckScope())
                        {
                            //EditorResourceSettings.AtlasDirectorySeparator = EditorGUILayout.TextField(new GUIContent("Atlas Directory Separator", "图集目录分隔符"), EditorResourceSettings.AtlasDirectorySeparator);
                            if (ignoreChanged.changed)
                            {
                                GUI.changed = false;
                                editorResourceSettingsChanged = true;
                            }
                        }

                        if (checkerRuntime.changed)
                        {
                            GUI.changed = false;
                        }
                    }

                    EditorResourceSettings.SetAssetBundleName = EditorGUILayout.Toggle(new GUIContent("Set AssetBundleName", "分析完成后设置 AssetImporter assetBundleName & assetBundleVariant"), EditorResourceSettings.SetAssetBundleName);
                    EditorResourceSettings.StripAutoDependency = EditorGUILayout.Toggle(new GUIContent("Strip Auto Dependency", "剔除自动依赖，被引用次数小于2的资源"), EditorResourceSettings.StripAutoDependency);
                    EditorResourceSettings.AllAssetRootPath = EditorGUILayout.DelayedTextField(new GUIContent("Asset Root Path", "资源目录"), EditorResourceSettings.AllAssetRootPath);
                    EditorResourceSettings.ResourcePackedExclude = EditorGUILayout.DelayedTextField(new GUIContent("Packed Exclude", "资源包不进安装包正则表达式"), EditorResourceSettings.ResourcePackedExclude);

                    if (EditorResourceSettings.Excludes == null)
                        EditorResourceSettings.Excludes = new string[0];
                    EditorResourceSettings.Excludes = new GUIContent("Exclude", "全局排除的 AssetPath，正则表达式格式，比如扩展名(\\.(cs|meta)$)").ArrayField(EditorResourceSettings.Excludes, (item, index) =>
                     {
                         item = EditorGUILayout.DelayedTextField(item);
                         return item;
                     }, createInstance: () => "", initExpand: true) as string[];


                    using (var foldout = new EditorGUILayoutx.FoldoutHeaderGroupScope(new GUIContent("Before Build", "资源生成之前"), initExpand: true))
                    {
                        if (foldout.Visiable)
                        {
                            using (new EditorGUI.IndentLevelScope())
                            {
                                EditorResourceSettings.AnalysisOnBuild = EditorGUILayout.Toggle(new GUIContent("Auto Analysis", "生成之前重新分析"), EditorResourceSettings.AnalysisOnBuild);
                            }
                        }
                    }

                    using (var foldout = new EditorGUILayoutx.FoldoutHeaderGroupScope(new GUIContent("After Build", "资源生成之后"), initExpand: true))
                    {
                        if (foldout.Visiable)
                        {
                            using (new EditorGUI.IndentLevelScope())
                            {
                                using (var checkerRuntime = new EditorGUI.ChangeCheckScope())
                                {
                                    ResourceStreamingAssetsPathAttribute.GetStreamingAssetsPath(
                                        out var hasStreamingAssetsPathAttr);
                                    if (hasStreamingAssetsPathAttr)
                                    {
                                        using (new EditorGUI.DisabledGroupScope(true))
                                        {
                                            EditorGUILayout.TextField(
                                                new GUIContent("StreamingAssets Subpath", "StreamingAssets 子目录"),
                                                ResourceSettings.StreamingAssetsPath);
                                            EditorGUILayout.HelpBox(
                                                "already used ResourceStreamingAssetsPathAttribute ", MessageType.Info);
                                        }
                                    }
                                    else
                                    {
                                        ResourceSettings.StreamingAssetsPath =
                                            EditorGUILayout.TextField(
                                                new GUIContent("StreamingAssets Subpath", "StreamingAssets 子目录"),
                                                ResourceSettings.StreamingAssetsPath);
                                    }

                                    if (checkerRuntime.changed)
                                    {
                                        GUI.changed = false;
                                    }
                                }

                                using (new GUILayout.HorizontalScope())
                                {
                                    EditorResourceSettings.CopyToStreamingAssets = EditorGUILayout.Toggle(new GUIContent("Copy To StreamingAssets", "资源生成完成后复制到 StreamingAssets 目录"), EditorResourceSettings.CopyToStreamingAssets);

                                    if (GUILayout.Button(new GUIContent("Open", "打开资源 StreamingAssets 目录"), GUILayout.ExpandWidth(false)))
                                    {
                                        if (Directory.Exists(EditorResourceSettings.EditorStreamingAssetsPath))
                                        {
                                            EditorUtilityx.OpenFolder(EditorResourceSettings.EditorStreamingAssetsPath);
                                        }
                                    }

                                    if (GUILayout.Button(new GUIContent("Copy", "复制生成的资源到 StreamingAssets 目录"), GUILayout.ExpandWidth(false)))
                                    {
                                        string workingPath, outputPackagePath, outputFullPath, outputPackedPath;
                                        m_Controller.GetOutputPath(m_Controller.CurrentPlatform, out workingPath, out outputPackagePath, out outputFullPath, out outputPackedPath);
                                        BuildResources.CopyToStreamingAsset(workingPath, outputPackagePath, outputFullPath, outputPackedPath);
                                        AssetDatabase.Refresh();
                                    }

                                    if (GUILayout.Button(new GUIContent("Clear", "清理 StreamingAssets 目录"), GUILayout.ExpandWidth(false)))
                                    {
                                        BuildResources.ClearStreamingAssets();
                                        AssetDatabase.Refresh();
                                    }
                                }
                            }
                        }
                    }

                    if (EditorResourceSettings.Groups == null)
                        EditorResourceSettings.Groups = new ResourceGroup[0];

                    var groups = EditorResourceSettings.Groups;

                    for (int i = 0; i < groups.Length; i++)
                    {
                        var group = groups[i];
                        using (new GUILayout.VerticalScope("box"))
                        {
                            group.PreprocessBuilds = (List<ResourcePreprocessBuild>)new GUIContent("Preprocess Build", "生成资源之前扩展").ArrayField(group.PreprocessBuilds, (item, index) =>
                            {
                                using (new GUILayout.VerticalScope("box"))
                                {
                                    int providerIndex;
                                    string suffix = "PreprocessBuild";
                                    System.Type type = null;
                                    if (item.PreprocessBuild != null)
                                        type = item.PreprocessBuild.GetType();

                                    GUIContent[] providerTypeNames = PreprocessBuildTypes.Select(o => new GUIContent(o.Name.EndsWith(suffix) ? o.Name.Substring(0, o.Name.Length - suffix.Length) : o.Name)).ToArray();
                                    providerIndex = Array.FindIndex(PreprocessBuildTypes, o => o == type);
                                    int newIndex = EditorGUILayout.Popup(new GUIContent("Provider", "IResourcePreprocessBuild 接口"), providerIndex, providerTypeNames);
                                    if (newIndex != providerIndex)
                                    {
                                        providerIndex = newIndex;
                                        if (providerIndex != -1)
                                        {
                                            type = PreprocessBuildTypes[providerIndex];
                                            var instance = Activator.CreateInstance(type) as IResourcePreprocessBuild;
                                            if (instance != null)
                                            {
                                                if (instance is ISerializationCallbackReceiver)
                                                    ((ISerializationCallbackReceiver)instance).OnAfterDeserialize();
                                            }
                                            item.PreprocessBuild = instance;
                                        }
                                    }
                                    if (item.PreprocessBuild != null)
                                    {
                                        EditorUtilityx.GUISerializableObject(item.PreprocessBuild);
                                    }
                                }
                                return item;
                            }, initExpand: true);


                            group.Rules = (List<ResourceRule>)new GUIContent("Rule", "Addressable 路径的规则").ArrayField(group.Rules, (item, index) =>
                             {
                                 using (new GUILayout.VerticalScope("box"))
                                 {
                                     int providerIndex;
                                     string suffix = "AddressableProvider";
                                     GUIContent[] providerTypeNames = AddressableProviderTypes.Select(o => new GUIContent(o.Name.EndsWith(suffix) ? o.Name.Substring(0, o.Name.Length - suffix.Length) : o.Name)).ToArray();
                                     providerIndex = Array.FindIndex(AddressableProviderTypes, o => o == item.AddressableType);
                                     int newIndex = EditorGUILayout.Popup(new GUIContent("Provider", "IAddressableProvider 接口"), providerIndex, providerTypeNames);
                                     if (newIndex != providerIndex)
                                     {
                                         providerIndex = newIndex;
                                         if (providerIndex != -1)
                                         {
                                             item.AddressableType = AddressableProviderTypes[providerIndex];
                                         }
                                     }

                                     item.include = EditorGUILayout.DelayedTextField(new GUIContent("Include", "包含的AssetPath，表达式格式"), item.include);
                                     item.exclude = EditorGUILayout.DelayedTextField(new GUIContent("Exclude", "排除的AssetPath，表达式格式"), item.exclude);

                                     if (item.AddressableProvider != null)
                                     {
                                         EditorUtilityx.GUISerializableObject(item.AddressableProvider);
                                     }

                                     using (var previewFoldout = new EditorGUILayoutx.FoldoutHeaderGroupScope(new GUIContent("Preview"),
                                         initExpand: false,
                                         onShow: () =>
                                         {
                                             m_Collection = new ResourceCollection();
                                             m_Collection.Load();
                                             LoadPreviewAssets(item);
                                         }))
                                     {
                                         if (previewFoldout.Visiable)
                                         {
                                             GUIPreviewAssets(item);
                                         }
                                     }
                                 }
                                 return item;
                             }, initExpand: true);
                        }
                    }
                }

                if (checker.changed || editorResourceSettingsChanged)
                {
                    EditorResourceSettings.SaveConfig();
                    EditorResourceSettings.LoadConfig();
                }

            }
            if (controllerChanged)
            {
                m_Controller.Save();
            }
        }


        void GUIResourceMode()
        {
            using (var checkerUserSettings = new EditorGUI.ChangeCheckScope())
            {

                //资源模式
                int selectedIndex = 0;
                if (ResourceUserSettings.EditorMode)
                {
                    selectedIndex = 0;
                }
                else
                {
                    switch (ResourceUserSettings.ResourceMode)
                    {
                        case ResourceMode.Updatable:
                            selectedIndex = 2;
                            break;
                        case ResourceMode.Package:
                        default:
                            selectedIndex = 1;
                            break;
                    }
                }
                selectedIndex = EditorGUILayout.Popup(new GUIContent("Mode", "资源模式"), selectedIndex, resourceModeDisplays);
                if (checkerUserSettings.changed)
                {
                    GUI.changed = false;
                    switch (selectedIndex)
                    {
                        case 0:
                            ResourceUserSettings.EditorMode = true;
                            break;
                        case 1:
                            ResourceUserSettings.ResourceMode = ResourceMode.Package;
                            break;
                        case 2:
                            ResourceUserSettings.ResourceMode = ResourceMode.Updatable;
                            break;
                    }
                }
            }

        }

        class PreviewAssetsControl
        {
            public string filter;
            public bool showAll;
        }

        void GUIPreviewAssets(ResourceRule item)
        {
            List<string[]> list;
            string assetPath;
            int displayMax = 20;
            if (m_PreviewAssets == null)
                return;

            int ctrlId = GUIUtility.GetControlID(typeof(PreviewAssetsControl).GetHashCode(), FocusType.Passive);
            var ctrl = (PreviewAssetsControl)GUIUtility.GetStateObject(typeof(PreviewAssetsControl), ctrlId);
            using (new GUILayout.HorizontalScope())
            {
                ctrl.showAll = EditorGUILayout.ToggleLeft(new GUIContent("All", "显示所有"), ctrl.showAll, GUILayout.Width(50));
                ctrl.filter = EditorGUILayout.TextField(ctrl.filter, EditorStyles.toolbarSearchField);
            }
            if (ctrl.showAll)
                displayMax = -1;
            GUIStyle h = new GUIStyle();
            h.normal.background = EditorGUIUtility.whiteTexture;

            if (m_PreviewAssets.TryGetValue(item, out list))
            {
                int max = displayMax;
                var array = list;
                bool hasNext = false;
                for (int i = 0, j = 0; i < list.Count; i++)
                {
                    string guid = array[i][0];
                    assetPath = array[i][1];

                    string assetName = null, bundleName = null, variant = null;

                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        if (!EditorResourceSettings.GetAddressable(assetPath, out bundleName, out variant, out assetName))
                        {
                            assetName = assetPath;
                            //var importer = AssetImporter.GetAtPath(assetPath);
                            //bundleName = importer.assetBundleName;
                            //variant = importer.assetBundleVariant;

                            var asset = m_Collection.GetAsset(guid);
                            if (asset != null)
                            {
                                assetName = asset.Name;
                                bundleName = asset.Resource.Name;
                                variant = asset.Resource.Variant;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(ctrl.filter))
                    {
                        bool isFilter = true;

                        if (!string.IsNullOrEmpty(assetPath))
                        {
                            if (assetPath.IndexOf(ctrl.filter, StringComparison.InvariantCultureIgnoreCase) >= 0)
                            {
                                isFilter = false;
                            }
                        }
                        else
                        {
                            isFilter = false;
                        }

                        if (isFilter && !string.IsNullOrEmpty(assetName) && assetName.IndexOf(ctrl.filter, StringComparison.InvariantCultureIgnoreCase) >= 0)
                            isFilter = false;
                        if (isFilter && !string.IsNullOrEmpty(bundleName) && bundleName.IndexOf(ctrl.filter, StringComparison.InvariantCultureIgnoreCase) >= 0)
                            isFilter = false;
                        if (isFilter && !string.IsNullOrEmpty(variant) && variant.IndexOf(ctrl.filter, StringComparison.InvariantCultureIgnoreCase) >= 0)
                            isFilter = false;


                        if (isFilter)
                            continue;
                    }


                    if (max > 0 && j > max)
                    {
                        hasNext = true;
                        break;
                    }
                    Color oldColor = GUI.backgroundColor;
                    if (j % 2 == 1)
                    {
                        GUI.backgroundColor *= new Color(1, 1, 1, 0.03f);
                    }
                    else
                    {
                        GUI.backgroundColor = Color.clear;
                    }
                    using (new GUILayout.HorizontalScope(h))
                    {
                        if (string.IsNullOrEmpty(assetPath))
                        {
                            GUI.color = Color.red;
                            GUILayout.Label(guid + " (missing)");
                            GUI.color = Color.white;
                        }
                        else
                        {
                            if (GUILayout.Button(new GUIContent(assetName, assetPath), "label"))
                            {
                                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object)));
                            }
                            GUILayout.FlexibleSpace();
                            string fullBundleName = bundleName + (string.IsNullOrEmpty(variant) ? "" : "." + variant);
                            if (GUILayout.Button(new GUIContent(fullBundleName, fullBundleName), "label"))
                            {
                                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object)));
                            }
                            j++;
                        }

                    }
                    GUI.backgroundColor = oldColor;
                }
                if (hasNext)
                {
                    GUILayout.Label("...");
                }
            }
        }

        void LoadPreviewAssets(ResourceRule item)
        {
            if (m_PreviewAssets == null)
                m_PreviewAssets = new Dictionary<ResourceRule, List<string[]>>();
            HashSet<string> list = new HashSet<string>();

            using (new PerformanceSample("LoadPreviewAssets"))
            {
                var all = AssetDatabase.GetAllAssetPaths();

                foreach (var assetPath in all)
                {
                    if (AssetDatabase.IsValidFolder(assetPath))
                        continue;
                    if (EditorResourceSettings.IsExcludeAssetPath(assetPath))
                        continue;
                    string guid = AssetDatabase.AssetPathToGUID(assetPath);
                    if (item.IsMatch(assetPath))
                    {
                        list.Add(guid);
                    }
                }


                m_PreviewAssets[item] = list.Select(o => new string[] { o, AssetDatabase.GUIDToAssetPath(o) }).OrderBy(o => o[1]).ToList();
            }
        }
    }



}