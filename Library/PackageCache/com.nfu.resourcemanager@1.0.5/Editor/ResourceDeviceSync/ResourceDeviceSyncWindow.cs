using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ND.Managers.ResourceMgr.Framework;
using ND.Managers.ResourceMgr.Framework.FileSystem;
using ND.Managers.ResourceMgr.Framework.Resource;
using ND.Managers.ResourceMgr.Runtime;
using UnityEditor;
using UnityEngine;

using static ND.Managers.ResourceMgr.Editor.ResourceTools.ResourceBuilderController;
using static ND.Managers.ResourceMgr.Framework.Resource.ResourceManager;
using Debug = UnityEngine.Debug;


namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{

    public class ResourceDeviceSyncWindow : EditorWindow
    {
        private string[] connectedDevices;
        private bool refresh;
        [SerializeField]
        private string selectedDevice;
        private string resourceVersionName;
        private string[] allResourceVersionNames;
        PackageVersionList? localVersion;
        private bool hasLocalVersion;
        UpdatableVersionList? remoteVersion;
        private bool hasRemoteVersion;
        //总是最新的版本
        private bool alwaysLatestLocalVersion = true;
        private Vector2 scrollPos;
        private bool isInstall;
        private bool isConnected;
        private bool initialized;
        private object checkConnectedLock;
        private List<ResourceInfo> changedResources;
        private List<ResourceInfo> removedResources;
        private List<ResourceInfo> addedResources;
        Dictionary<string, ResourceInfo> localResources;
        Dictionary<string, ResourceInfo> remoteResources;
        ResourceBuilderController builderCtrl;
        static string TmpRoot = "Temp/ResourceSync";
        static string TmpRemotePath = TmpRoot + "/Remote";
        static string TmpLocalPath = TmpRoot + "/Local";

        static string RemoteVersionFileName = ResourceBuilderController.RemoteVersionListFileName;
        static string PackedVersionFileName = ResourceBuilderController.LocalVersionListFileName;


        static string AndroidDevicePersistentDataPath
        {
            get
            {
                return $"/sdcard/android/data/{Application.identifier}/files";
            }
        }


        public string SelectedDevice
        {
            get
            {
                int selectedIndex;
                selectedIndex = Array.FindIndex(connectedDevices, (o) => o == selectedDevice);
                if (selectedIndex == -1)
                    return null;
                return selectedDevice;
            }
        }
        public string LocalResourceRoot
        {
            get => builderCtrl.OutputDirectory;
        }
        public string LocalResourceDirectory
        {
            get
            {
                string platformName = ResourceBuilderController.BuildTargetToPlatform(EditorUserBuildSettings.activeBuildTarget).ToString();
                return $"{LocalResourceRoot}/Package/{resourceVersionName}/{platformName}";
            }
        }
        public string LocalResourceFullDirectory
        {
            get
            {
                string platformName = ResourceBuilderController.BuildTargetToPlatform(EditorUserBuildSettings.activeBuildTarget).ToString();
                return $"{LocalResourceRoot}/Full/{resourceVersionName}/{platformName}";
            }
        }
        public string LocalResourcePackedDirectory
        {
            get
            {
                string platformName = ResourceBuilderController.BuildTargetToPlatform(EditorUserBuildSettings.activeBuildTarget).ToString();
                return $"{LocalResourceRoot}/Packed/{resourceVersionName}/{platformName}";
            }
        }
        string removeDeviceResourceDirectory;
        public string RemoteDeviceResourceDirectory
        {
            get
            {
                if (!string.IsNullOrEmpty(removeDeviceResourceDirectory))
                    return removeDeviceResourceDirectory;
                return $"{AndroidDevicePersistentDataPath}";
            }
            set
            {
                removeDeviceResourceDirectory = value;
            }
        }

        [MenuItem(EditorUtilityx.MenuPrefix + "Device Sync")]
        public static void ShowWindow()
        {
            GetWindow<ResourceDeviceSyncWindow>().Show();
        }
        void RefreshDevices()
        {
            isConnected = false;
            isInstall = false;

            string result = RunADB("devices");
            string[] array = result.Split('\n')
                .Select(o => o.Trim())
                .Where(o => !string.IsNullOrEmpty(o))
                .Skip(1)
                .ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                string[] parts = array[i].Split('\t');
                array[i] = parts[0];
            }

            connectedDevices = array
                .OrderBy(o => o)
                .ToArray();
            if (string.IsNullOrEmpty(selectedDevice) && connectedDevices.Length > 0)
            {
                SelectDevice(connectedDevices[0]);
            }
            else
            {
                CheckConnect();
            }
        }

        void CheckConnect()
        {
            isConnected = connectedDevices.Any(o => o == selectedDevice);
            isInstall = false;
            if (isConnected)
            {
                isInstall = !string.IsNullOrEmpty(GetInstallPath(Application.identifier));
            }
        }

        private void OnEnable()
        {
            EditorUtilityx.InitLogHelper();

            if (changedResources == null)
            {
                changedResources = new List<ResourceInfo>();
                addedResources = new List<ResourceInfo>();
                removedResources = new List<ResourceInfo>();
                localResources = new Dictionary<string, ResourceInfo>();
                remoteResources = new Dictionary<string, ResourceInfo>();
            }

            titleContent = new GUIContent("Resource Device Sync");

            refresh = true;
            checkConnectedLock = new object();
            CheckConnectRunner(checkConnectedLock);
            if (builderCtrl == null)
            {
                builderCtrl = new ResourceBuilderController();
                builderCtrl.Load(false);
                connectedDevices = new string[0];
                if (!initialized)
                {
                    Refresh();
                }
            }

            initialized = true;

        }

        async void CheckConnectRunner(object locked)
        {
            while (checkConnectedLock == locked)
            {
                if (!string.IsNullOrEmpty(selectedDevice))
                {
                    RefreshDevices();
                }
                await Task.Delay(5000);
            }
        }

        private void OnDisable()
        {
            checkConnectedLock = null;
            initialized = false;
        }
        private void Update()
        {
            if (refresh)
            {
                Refresh();
            }
        }


        void SelectDevice(string newDevice)
        {

            if (selectedDevice != newDevice)
            {
                selectedDevice = newDevice;
                CheckConnect();

                GetDeviceVersion();
            }
        }

        string LocalVersionFilePath
        {
            get
            {
                if (!Directory.Exists(LocalResourceDirectory))
                    return null;
                var filePath = Directory.GetFiles(LocalResourceDirectory, RemoteVersionFileName).FirstOrDefault();
                return filePath;
            }
        }
        string RemoteVersionFilePath
        {
            get => Path.Combine(RemoteDeviceResourceDirectory, RemoteVersionFileName).Replace('\\', '/');
        }

        string RemoteVersionListFilePath
        {
            get => Path.Combine(RemoteDeviceResourceDirectory, PackedVersionFileName).Replace('\\', '/');
        }

        void GetLocalVersion()
        {
            localVersion = null;
            hasLocalVersion = false;
            builderCtrl = new ResourceBuilderController();
            builderCtrl.Load(false);

            List<string> versionNames = new List<string>();
            foreach (var dir in Directory.GetDirectories(Path.Combine(LocalResourceRoot, "Package")))
            {
                if (ParseResVersion(dir, out int resVer, out string appVer))
                {
                    versionNames.Add(Path.GetFileName(dir));
                }
            }
            allResourceVersionNames = versionNames.OrderByDescending(o =>
            {
                ParseResVersion(o, out int resVer, out string appVer);
                return resVer;
            }).ToArray();


            if (allResourceVersionNames.Length > 0)
            {
                if (string.IsNullOrEmpty(resourceVersionName) || alwaysLatestLocalVersion)
                {
                    resourceVersionName = allResourceVersionNames[0];
                }
            }

            string versionFile = LocalVersionFilePath;

            if (File.Exists(versionFile))
            {
                try
                {
                    localVersion = ResourceInfo.GetPackageVersionSerializer().Deserialize(versionFile);
                    if (localVersion != null)
                    {
                        hasLocalVersion = true;
                        Log.Info("Load local version ok\n" + versionFile);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            if (localVersion == null)
                Debug.Log(EditorUtilityx.LogPrefix + "local version null");
        }

        void Refresh()
        {
            using (new PerformanceSample("Refresh"))
            {
                refresh = false;
                EditorUtility.DisplayProgressBar("Get local version", "", 0);
                GetLocalVersion();
                EditorUtility.DisplayProgressBar("Refresh device", "", 0.3f);
                RefreshDevices();
                if (!string.IsNullOrEmpty(SelectedDevice))
                {
                    EditorUtility.DisplayProgressBar("Get remote version", "", 0.6f);
                    GetDeviceVersion();
                }
                EditorUtility.ClearProgressBar();
            }
        }

        void GetDeviceVersion()
        {
            using (new PerformanceSample("GetDeviceVersion"))
            {
                remoteVersion = null;
                changedResources.Clear();
                addedResources.Clear();
                removedResources.Clear();
                hasRemoteVersion = false;

                CheckConnect();

                if (!isInstall)
                    return;

                string tmpDir = Path.GetFullPath(TmpRemotePath);
                if (Directory.Exists(tmpDir))
                    Directory.Delete(tmpDir, true);
                if (!Directory.Exists(tmpDir))
                    Directory.CreateDirectory(tmpDir);

                try
                {
                    GetRemotePathReadWritePermission(RemoteDeviceResourceDirectory);
                }
                catch (Exception ex) { }
                try
                {
                    RunADB(GetADBCmdArg($"pull \"{RemoteVersionFilePath}\" \"{tmpDir}\""));
                }
                catch (Exception ex) { }

                try
                {
                    RunADB(GetADBCmdArg($"pull \"{RemoteVersionListFilePath}\" \"{tmpDir}\""));
                }
                catch (Exception ex) { }

                string versionFile = Path.Combine(tmpDir, RemoteVersionFileName);
                if (File.Exists(versionFile))
                {
                    try
                    {
                        remoteVersion = ResourceInfo.GetFullVersionSerializer().Deserialize(versionFile);
                        if (remoteVersion != null)
                        {
                            hasRemoteVersion = true;
                            Log.Info("Load remote version ok\n" + versionFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
                else
                {
                    Debug.Log(EditorUtilityx.LogPrefix + $"not found remote {RemoteVersionFileName} file");
                }

                if (localVersion != null && remoteVersion != null)
                {
                    localResources.Clear();
                    foreach (var item in localVersion.Value.GetResourceInfos())
                    {
                        localResources[item.FullName] = item;
                    }
                    remoteResources.Clear();
                    foreach (var item in remoteVersion.Value.GetResourceInfos())
                    {
                        remoteResources[item.FullName] = item;
                    }
                    ResourceInfo.Compare(localResources.Values.ToArray(), remoteResources.Values.ToArray(), changedResources, addedResources, removedResources);
                }
            }
        }



        private void OnGUI()
        {

            if (string.IsNullOrEmpty(EditorUtilityx.AndroidSDKHome))
                EditorGUILayout.HelpBox("require Android Sdk", MessageType.Error);

            GUIAndroidDeviceList();


            if (isInstall)
            {
                if (!hasRemoteVersion)
                {
                    EditorGUILayout.HelpBox("Cannot get remote resource version information, Please run the game complete initialization", MessageType.Error);
                }

                using (new GUILayout.VerticalScope())
                {
                    using (new GUILayout.VerticalScope("box"))
                    {
                        EditorGUI.indentLevel++;
                        if (remoteVersion != null)
                        {
                            GUIVersion(remoteVersion.Value);
                        }
                        EditorGUI.indentLevel--;
                    }
                }
            }


            using (var sv = new GUILayout.ScrollViewScope(scrollPos))
            {
                scrollPos = sv.scrollPosition;
                GUILayout.Label("Local", GUILayout.ExpandWidth(false));

                if (!hasLocalVersion)
                {
                    EditorGUILayout.HelpBox($"Cannot get local resource version information, build the resource, menu: {EditorUtilityx.MenuPrefix}Build", MessageType.Error);
                }

                using (new GUILayout.VerticalScope("box"))
                {
                    EditorGUI.indentLevel++;
                    if (localVersion != null)
                    {
                        var version = localVersion.Value;

                        var oldLabelWidth = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth += 35;
                        EditorGUILayout.LabelField("Applicable Game Version", version.ApplicableGameVersion, GUILayout.ExpandWidth(false));
                        using (new GUILayout.HorizontalScope())
                        {
                            EditorGUILayout.PrefixLabel("Internal Resource Version");
                            //防止 label 变灰
                            GUILayout.Button(GUIContent.none, GUILayout.Width(0));
                            using (new EditorGUI.DisabledGroupScope(alwaysLatestLocalVersion))
                            {
                                int selectedIndex = Array.FindIndex(allResourceVersionNames, o => o == resourceVersionName);
                                int newSelectedIndex = EditorGUILayout.Popup(selectedIndex, allResourceVersionNames.Select(o => new GUIContent(o.Split('_').Last())).ToArray());
                                if (newSelectedIndex != selectedIndex)
                                {
                                    resourceVersionName = allResourceVersionNames[newSelectedIndex];
                                    GetLocalVersion();
                                }
                            }
                            alwaysLatestLocalVersion = EditorGUILayout.ToggleLeft("Latest", alwaysLatestLocalVersion, GUILayout.Width(70));
                        }

                        EditorGUIUtility.labelWidth -= 50;
                    }
                    EditorGUI.indentLevel--;
                }

                GUIHelp();

                using (new EditorGUI.DisabledGroupScope(string.IsNullOrEmpty(SelectedDevice)))
                {
                    var toal = changedResources.Concat(addedResources).Concat(removedResources);
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Total");
                        GUILayout.Label($"({GetFileCount(toal)})");
                        GUILayout.FlexibleSpace();
                        GUILayout.Label($"{GetDisplaySizeText(toal.Sum(o => o.Length))}");
                    }
                    GUIResourceInfos("Changed", changedResources);
                    GUIResourceInfos("Added", addedResources);
                    GUIResourceInfos("Removed", removedResources);
                }
            }


            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Refresh"))
                {
                    Refresh();
                }

                GUI.enabled = hasLocalVersion && hasRemoteVersion;
                if (GUILayout.Button("Sync"))
                {
                    SyncLocalToRemoteAsync().StartCoroutine();
                }
                GUI.enabled = true;
            }
        }

        void GUIHelp()
        {

            using (var foldout = new EditorGUILayoutx.FoldoutHeaderGroupScope(new GUIContent("Help"), initExpand: false))
            {
                if (foldout.Visiable)
                {
                    EditorGUILayout.LabelField("Android SDK", EditorUtilityx.AndroidSDKHome);
                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PrefixLabel("Package");
                        EditorGUILayout.SelectableLabel(Application.identifier, GUILayout.ExpandWidth(true), GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    }
                    GUILayout.Label("只支持 Android 设备同步");
                    GUILayout.Label("第一次使用同步，需要先运行游戏完成资源管理器初始化配置");
                    GUILayout.Label("开启热更模式 [Resource Mode]=[Updatable]");
                    GUILayout.Label("设置资源版本号增量模式 [Resource Update Check Mode]=[Normal]");
                }
            }
        }

        int GetFileCount(IEnumerable<ResourceInfo> resourceInfos)
        {
            HashSet<string> files = new HashSet<string>();
            foreach (var resource in resourceInfos)
            {
                files.Add(resource.FileName);
            }
            return files.Count;
        }

        void GUIResourceInfos(string label, List<ResourceInfo> resourceInfos)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label(label);
                GUILayout.Label($"({GetFileCount(resourceInfos)})");
                GUILayout.FlexibleSpace();
                int totalSize = 0;
                totalSize = resourceInfos.Sum(o => o.Length);
                GUILayout.Label(GetDisplaySizeText(totalSize), GUILayout.ExpandWidth(false));
            }

            using (var alternate = new EditorGUILayoutx.AlternateScope())
            using (new EditorGUILayoutx.IndentLevelVerticalScope())
            {
                foreach (var g in resourceInfos.GroupBy(o => o.FileName))
                {
                    var first = g.First();

                    Func<ResourceInfo, string> GetHashString = (ResourceInfo localResInfo) =>
                      {
                          ResourceInfo remoteResInfo;

                          string s = "";
                          if (remoteResources.TryGetValue(localResInfo.FullName, out remoteResInfo))
                          {
                              s = remoteResInfo.HashCode.ToString("x8");
                          }
                          if (localResInfo.HashCode != 0)
                              s += " > " + localResInfo.HashCode.ToString("x8");
                          return s;
                      };
                    using (alternate.Horizontal())
                    {
                        GUILayout.Label(g.Key + " " + GetHashString(first));
                        GUILayout.Label(new GUIContent(GetDisplaySizeText(g.Sum(o => o.Length))), GUILayout.ExpandWidth(false));
                    }

                    if (first.UseFileSystem)
                    {
                        using (new EditorGUILayoutx.IndentLevelVerticalScope())
                        {
                            foreach (var resource in g)
                            {
                                using (alternate.Horizontal())
                                {

                                    GUILayout.Label(resource.Name + " " + GetHashString(resource));
                                    GUILayout.FlexibleSpace();
                                    GUILayout.Label(new GUIContent(GetDisplaySizeText(resource.Length)), GUILayout.ExpandWidth(false));
                                }
                            }
                        }
                    }

                }
            }

        }
        ResourceInfo? GetLocalResource(string fullName)
        {
            ResourceInfo resourceInfo;
            if (!localResources.TryGetValue(fullName, out resourceInfo))
                return null;
            return resourceInfo;
        }



        string GetInstallPath(string package)
        {
            string result = null;
            try
            {
                result = RunADB(GetADBCmdArg($"shell pm path {package}"));
                if (string.IsNullOrEmpty(result))
                    return null;
                if (result.StartsWith("package:"))
                    return result.Substring(8);
            }
            catch { }
            return result;
        }
        string GetDisplaySizeText(int size)
        {
            if (size < 1024)
            {
                return $"{size}B";
            }
            else if (size < 1024 * 1024)
            {
                return $"{size / 1024f:0.#}K";
            }
            else if (size < 1024 * 1024 * 1024)
            {
                return $"{size / 1024f / 1024f:0.#}M";
            }
            return $"{size / 1024f / 1024f / 1024f:0.#}G";
        }

        IEnumerator SyncLocalToRemoteAsync()
        {
            using (new PerformanceSample("SyncLocalToRemoteAsync"))
            {
                Debug.Log(EditorUtilityx.LogPrefix + "Resource sync start");

                if (!hasLocalVersion)
                    throw new Exception("not local version");
                if (!hasRemoteVersion)
                    throw new Exception("not remote version");

                GetLocalVersion();
                if (localVersion == null)
                    throw new Exception("not local version");

                string localDir = Path.GetFullPath(LocalResourceDirectory);
                string remoteDir = RemoteDeviceResourceDirectory;
                Log.Info("local dir: " + localDir);
                Log.Info("remote dir: " + remoteDir);

                List<Resource> resources = new List<Resource>();
                string tmpLocalPath = Path.GetFullPath(TmpLocalPath);
                string tmpRemoteDir = Path.GetFullPath(TmpRemotePath);

                if (Directory.Exists(TmpRoot))
                    Directory.Delete(TmpRoot, true);
                if (!Directory.Exists(TmpRoot))
                    Directory.CreateDirectory(TmpRoot);
                if (!Directory.Exists(TmpLocalPath))
                    Directory.CreateDirectory(TmpLocalPath);
                if (!Directory.Exists(TmpRemotePath))
                    Directory.CreateDirectory(TmpRemotePath);


                List<ResourceInfo> changedResources = new List<ResourceInfo>();
                List<ResourceInfo> removedResources = new List<ResourceInfo>();
                List<ResourceInfo> addedResources = new List<ResourceInfo>();

                string remoteFullVersionPath = Path.Combine(tmpRemoteDir, RemoteVersionFileName);
                string remoteReadWriteVersionPath = Path.Combine(tmpRemoteDir, PackedVersionFileName);

                try
                {
                    GetRemotePathReadWritePermission(RemoteDeviceResourceDirectory);
                }
                catch (Exception ex) { }

                EditorUtility.DisplayProgressBar("Get remote file", RemoteVersionFileName, 0f);
                try
                {
                    RunADB(GetADBCmdArg($"pull \"{RemoteVersionFilePath}\" \"{tmpRemoteDir}\""));
                }
                catch (Exception ex) { }

                if (!File.Exists(remoteFullVersionPath))
                    throw new FileNotFoundException("Not found file", RemoteVersionFileName);

                EditorUtility.DisplayProgressBar("Get remote file", PackedVersionFileName, 0f);
                try
                {
                    RunADB(GetADBCmdArg($"pull \"{RemoteDeviceResourceDirectory}/{ PackedVersionFileName}\" \"{tmpRemoteDir}\""));
                }
                catch (Exception ex) { }

                var remoteVersion = ResourceInfo.GetFullVersionSerializer().Deserialize(remoteFullVersionPath);
                Dictionary<string, ResourceInfo> localResources = new Dictionary<string, ResourceInfo>();

                foreach (var res in localVersion.Value.GetResourceInfos())
                {
                    localResources[res.FullName] = res;
                }

                ResourceInfo.Compare(localVersion.Value.GetResourceInfos(), remoteVersion.GetResourceInfos(), changedResources, addedResources, removedResources);

                var newVersionList = GenerateReadWriteVersionList(changedResources, addedResources, removedResources);


                //文件系统文件
                Dictionary<string, ResourceInfo> fileSystemResources = new Dictionary<string, ResourceInfo>();
                HashSet<string> changedFileSystems = new HashSet<string>();

                foreach (var res in changedResources.Concat(addedResources))
                {
                    if (res.FileSystem == null)
                        continue;
                    fileSystemResources[res.FullName] = res;
                    changedFileSystems.Add(res.FileSystem);
                }
                if (changedFileSystems.Count > 0)
                {
                    //补充 FileSystem 关联文件
                    foreach (var newRes in newVersionList.GetResourceInfos())
                    {
                        if (fileSystemResources.ContainsKey(newRes.FullName))
                            continue;

                        ResourceInfo localResInfo;
                        if (!localResources.TryGetValue(newRes.FullName, out localResInfo))
                            throw new Exception("Not found resource: " + newRes.FullName);
                        if (localResInfo.FileSystem == null)
                            continue;
                        //如果这个文件系统没有发生修改，则忽略
                        if (!changedFileSystems.Contains(localResInfo.FileSystem))
                            continue;
                        fileSystemResources.Add(localResInfo.FullName, localResInfo);
                    }
                    //生成文件系统文件
                    IFileSystemManager fsm = null;
                    var fileSystems = CreateFileSystems(fileSystemResources.Values, tmpLocalPath, ref fsm);
                    if (fileSystems.Count > 0)
                    {
                        IFileSystemManager srcFsm = null;
                        MemoryStream ms = new MemoryStream();

                        srcFsm = GameFrameworkEntry.GetModule<IFileSystemManager>();
                        srcFsm.SetFileSystemHelper(new DefaultFileSystemHelper());

                        foreach (var item in fileSystems)
                        {
                            var fs = item.Value;
                            string fileSystem = item.Key;

                            EditorUtility.DisplayProgressBar("Create local fileSystem", fileSystem, 0f);
                            string srcPath = Path.Combine(localDir, fileSystem + "." + DefaultExtension);
                            var srcFs = srcFsm.LoadFileSystem(srcPath, FileSystemAccess.Read);

                            foreach (var res in fileSystemResources.Values)
                            {
                                if (res.FileSystem != fileSystem)
                                    continue;
                                ms.Position = 0;
                                ms.SetLength(0);
                                srcFs.ReadFile(res.WithExtensionName, ms);
                                ms.Position = 0;
                                fs.WriteFile(res.WithExtensionName, ms);
                            }

                            srcFsm.DestroyFileSystem(srcFs, false);
                            fsm.DestroyFileSystem(fs, false);
                        }

                        ms.Dispose();
                    }
                }

                //清理远程文件
                EditorUtility.DisplayProgressBar("Clear Remote files", "", 0f);

                string[] remoteFiles = GetRemoteAllFiles(RemoteDeviceResourceDirectory);

                HashSet<string> files = new HashSet<string>();
                foreach (var resource in changedResources.Concat(addedResources))
                {
                    files.Add(resource.FileName);
                }
                //清理远程文件
                foreach (var remoteFile in remoteFiles)
                {
                    if (remoteFile == PackedVersionFileName || remoteFile == RemoteVersionFileName)
                        continue;

                    if (removedResources.Where(o => o.FileName == remoteFile).Any())
                    // if (files.Where(o => o == remoteFile).Count() == 0 || changedResources.Where(o => GetFileName(o.Name, o.Variant, o.FileSystem) == remoteFile).Count() > 0)
                    {
                        DeleteRemoteFile(Path.Combine(remoteDir, remoteFile));
                    }
                }

                int copyProgress = 0, copyTotal = 0;

                List<string[]> copyFiles = new List<string[]>();

                foreach (var resource in changedResources.Concat(addedResources))
                {
                    string localPath;
                    string remotePath;
                    string fileName;

                    fileName = resource.FileName;
                    remotePath = Path.Combine(remoteDir, fileName).Replace('\\', '/');
                    if (resource.FileSystem != null)
                    {
                        localPath = Path.Combine(tmpLocalPath, fileName);
                    }
                    else
                    {
                        localPath = Path.Combine(LocalResourceDirectory, fileName);
                    }

                    if (copyFiles.Where(o => o[0] == localPath).Any())
                        continue;
                    copyFiles.Add(new string[] { localPath, remotePath, fileName });
                    copyTotal++;
                }
                using (new PerformanceSample("Copy Files To Remote"))
                {
                    foreach (var item in copyFiles)
                    {
                        string localPath = item[0];
                        string remotePath = item[1];
                        string fileName = item[2];
                        EditorUtility.DisplayProgressBar($"Copy Files To Remote", fileName, (copyProgress / (float)copyTotal));
                        CopyToRemoteFile(localPath, remotePath);
                        copyProgress++;
                    }
                }
                //写入最新版本文件
                EditorUtility.DisplayProgressBar("Write version file", "", 0f);
                DeleteRemoteFile(RemoteVersionFilePath);
                DeleteRemoteFile(Path.Combine(RemoteDeviceResourceDirectory, PackedVersionFileName));


                var remoteFileNameDotIdx = RemoteVersionFileName.LastIndexOf('.');
                string searchPattern = RemoteVersionFileName.Substring(0, remoteFileNameDotIdx)+"*" + RemoteVersionFileName.Substring(remoteFileNameDotIdx);
                string localFullVersionPath = Directory.GetFiles(LocalResourceFullDirectory, searchPattern).FirstOrDefault();
                if (!File.Exists(localFullVersionPath))
                {
                    throw new Exception("not found file: " + localFullVersionPath);
                }
                byte[] bytes = File.ReadAllBytes(localFullVersionPath);
                bytes = Utility.ZipUtil.Decompress(bytes);
                string tmpPath = $"Temp/{RemoteVersionFileName}";
                Directory.CreateDirectory(Path.GetDirectoryName(tmpPath));
                File.WriteAllBytes(tmpPath, bytes);
                CopyToRemoteFile(tmpPath, RemoteVersionFilePath);
                File.Delete(tmpPath);

                CopyToRemoteFile(Path.Combine(tmpLocalPath, PackedVersionFileName), Path.Combine(RemoteDeviceResourceDirectory, PackedVersionFileName));

                GetDeviceVersion();


                EditorUtility.ClearProgressBar();

                Debug.Log(EditorUtilityx.LogPrefix + "Resource sync done");
                yield break;
            }
        }

        private string[] GetFileSystemNames(IEnumerable<ResourceInfo> resourceDatas)
        {
            HashSet<string> fileSystemNames = new HashSet<string>();
            foreach (ResourceInfo resourceData in resourceDatas)
            {
                if (resourceData.FileSystem == null)
                {
                    continue;
                }

                fileSystemNames.Add(resourceData.FileSystem);
            }

            return fileSystemNames.OrderBy(x => x).ToArray();
        }
        private int[] GetResourceIndexesFromFileSystem(IEnumerable<ResourceInfo> resourceDatas, string fileSystemName)
        {
            int index = 0;
            List<int> resourceIndexes = new List<int>();
            foreach (ResourceInfo resourceData in resourceDatas)
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
        private Dictionary<string, IFileSystem> CreateFileSystems(IEnumerable<ResourceInfo> resourceDatas, string outputPath, ref IFileSystemManager dstFsm)
        {
            string[] fileSystemNames = GetFileSystemNames(resourceDatas);
            Dictionary<string, IFileSystem> outputFileSystem = new Dictionary<string, IFileSystem>();
            if (fileSystemNames.Length == 0)
                return outputFileSystem;
            if (dstFsm == null)
            {
                dstFsm = GameFrameworkEntry.GetModule<IFileSystemManager>();
                dstFsm.SetFileSystemHelper(new DefaultFileSystemHelper());
            }

            foreach (string fileSystemName in fileSystemNames)
            {
                int fileCount = GetResourceIndexesFromFileSystem(resourceDatas, fileSystemName).Length;
                string fullPath = Utility.Path.GetRegularPath(Path.Combine(outputPath, Utility.Text.Format("{0}.{1}", fileSystemName, ResourceBuilderController.DefaultExtension)));
                string directory = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                IFileSystem fileSystem = dstFsm.CreateFileSystem(fullPath, FileSystemAccess.Write, fileCount, fileCount);
                outputFileSystem.Add(fileSystemName, fileSystem);
            }
            return outputFileSystem;
        }

        void CopyToRemoteFile(string localPath, string remotePath)
        {
            localPath = localPath.Replace('\\', '/');
            remotePath = remotePath.Replace('\\', '/');
            string dir = Path.GetDirectoryName(remotePath);
            dir = dir.Replace('\\', '/');
            string cmdText;
            string localFileName = Path.GetFileName(localPath);
            string remoteFileName = Path.GetFileName(remotePath);
            string tmpPath = null;
            if (localFileName != remoteFileName)
            {
                //本地与远程文件名不同
                tmpPath = "Temp/adb/" + remoteFileName;
                Directory.CreateDirectory(Path.GetDirectoryName(tmpPath));
                if (File.Exists(tmpPath))
                    File.Delete(tmpPath);
                File.Copy(localPath, tmpPath, true);
                localPath = tmpPath;
            }
            DeleteRemoteFile(remotePath);
            CreateRemoteDirecotry(dir);
            Log.Info($"Copy to remote file: <{localPath}> => <{dir}>");
            cmdText = GetADBCmdArg($"push \"{localPath}\" \"{dir}\"");
            string result = RunADB(cmdText);
            if (tmpPath != null && File.Exists(tmpPath))
                File.Delete(tmpPath);

        }

        void CreateRemoteDirecotry(string path)
        {
            try
            {
                string cmd = GetADBCmdArg($"shell mkdir -p \"{path}\"");
                string result = RunADB(cmd);
                Log.Info("Create remote direcotry: " + path + "\n" + result);
            }
            catch { }
        }

        public void GetRemotePathReadWritePermission(string path)
        {
            path = path.Replace('\\', '/');
            RunADB(GetADBCmdArg($"shell su -c 'chmod -R 777 \"{path}\"'"));
        }

        bool DeleteRemoteFile(string remotePath)
        {
            remotePath = remotePath.Replace('\\', '/');
            try
            {
                Log.Info($"Delete remote file: <{remotePath}>");
                string cmdText;
                cmdText = $"shell rm -rf \"{ remotePath}\"";
                cmdText = GetADBCmdArg(cmdText);
                if (string.IsNullOrEmpty(RunADB(cmdText)))
                    return true;
            }
            catch { }
            return false;
        }

        string ADBShell(string cmdText)
        {
            return ADBShell(new string[] { cmdText });
        }
        string ADBShell(IEnumerable<string> cmdTexts)
        {
            return Cmd(EditorUtilityx.AndroidADBPath, new string[] { GetADBCmdArg("shell") }.Concat(cmdTexts));
        }
        void ADBShell(Action<StreamWriter, StreamReader> cmd)
        {
            Cmd(EditorUtilityx.AndroidADBPath, "shell", (w, r) =>
            {
                cmd?.Invoke(w, r);
                w.WriteLine("exit");
            });
        }
        string[] GetRemoteAllFiles(string remoteDir)
        {
            string result = ADBShell(new string[]{
                $"cd \"{remoteDir}\"",
                $"ls -lR"
                });
            string[] parts = result.Split('\n').Select(o => o.Trim()).ToArray();

            bool startDir = false;
            string dir = null;
            List<string> paths = new List<string>();
            Regex regex = new Regex("\\d+:\\d+ (.*)");

            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];
                if (part.IndexOf('#') > 0)
                    continue;

                if (startDir)
                {
                    if (string.IsNullOrEmpty(part))
                    {
                        startDir = false;
                        continue;
                    }

                    if (part.StartsWith("d"))
                        continue;
                    if (part.StartsWith("total"))
                        continue;

                    string str = regex.Match(part).Groups[1].Value;
                    if (dir.Length > 0)
                        paths.Add(dir + "/" + str);
                    else
                        paths.Add(str);
                }
                else
                {
                    int index = part.IndexOf(':');
                    if (index > 0)
                    {
                        dir = part.Substring(0, index);
                        if (dir.StartsWith("./"))
                            dir = dir.Substring(2);
                        else if (dir == ".")
                            dir = "";
                        startDir = true;
                    }
                }
            }
            //Debug.Log(result);
            //Debug.Log(string.Join("\n", paths.ToArray()));

            return paths.ToArray();
        }

        string[] GetRemoteAllFileOrDirectorys(string remoteDir)
        {
            string result = ADBShell(new string[]{
                $"cd \"{remoteDir}\"",
                $"ls -lR"
                });
            string[] parts = result.Split('\n').Select(o => o.Trim()).ToArray();

            bool startDir = false;
            string dir = null;
            List<string> paths = new List<string>();
            Regex regex = new Regex("\\d+:\\d+ (.*)");

            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];
                if (part.IndexOf('#') > 0)
                    continue;

                if (startDir)
                {
                    if (string.IsNullOrEmpty(part))
                    {
                        startDir = false;
                        continue;
                    }

                    if (part.StartsWith("total"))
                        continue;

                    string str = regex.Match(part).Groups[1].Value;
                    if (dir.Length > 0)
                        paths.Add(dir + "/" + str);
                    else
                        paths.Add(str);
                }
                else
                {
                    int index = part.IndexOf(':');
                    if (index > 0)
                    {
                        dir = part.Substring(0, index);
                        if (dir.StartsWith("./"))
                            dir = dir.Substring(2);
                        else if (dir == ".")
                            dir = "";
                        startDir = true;
                    }
                }
            }
            //Debug.Log(result);
            //Debug.Log(string.Join("\n", paths.ToArray()));

            return paths.ToArray();
        }



        void GUIVersion(UpdatableVersionList version)
        {
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth += 30;
            EditorGUILayout.LabelField("Applicable Game Version", version.ApplicableGameVersion);
            EditorGUILayout.LabelField("Internal Resource Version", version.InternalResourceVersion.ToString());
            EditorGUIUtility.labelWidth -= 50;
        }
        void GUIVersion(PackageVersionList version)
        {
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth += 30;
            EditorGUILayout.LabelField("Applicable Game Version", version.ApplicableGameVersion);
            EditorGUILayout.LabelField("Internal Resource Version", version.InternalResourceVersion.ToString());
            EditorGUIUtility.labelWidth -= 50;
        }


        void GUIAndroidDeviceList()
        {
            using (new GUILayout.HorizontalScope())
            {

                int selectedIndex;
                List<string> list = new List<string>(connectedDevices);
                if (!string.IsNullOrEmpty(selectedDevice) && !list.Contains(selectedDevice))
                    list.Add(selectedDevice);
                selectedIndex = list.IndexOf(selectedDevice);
                GUILayout.Label("Device", GUILayout.ExpandWidth(false));
                int index = EditorGUILayout.Popup(selectedIndex, list.ToArray());
                if (selectedIndex != index)
                {
                    string newDevice = null;
                    if (index != -1)
                    {
                        newDevice = list[index];
                    }
                    SelectDevice(newDevice);
                }

            }

            if (!isConnected)
            {
                EditorGUILayout.HelpBox($"Device disconnected", MessageType.Error);
            }
            else if (string.IsNullOrEmpty(selectedDevice))
            {
                EditorGUILayout.HelpBox("No device selected", MessageType.Error);
            }
            else if (!isInstall)
            {
                EditorGUILayout.HelpBox($"No package name '{Application.identifier}' app installed on the device", MessageType.Error);
            }
        }

        bool RefreshButton()
        {
            var style = new GUIStyle(EditorStyles.largeLabel);
            style.fontSize += 4;
            style.padding = new RectOffset(5, 0, 1, 0);
            style.margin = new RectOffset();
            return GUILayout.Button("↻", style, GUILayout.ExpandWidth(false));
        }

        string GetADBCmdArg(string cmdText)
        {
            return GetADBCmdArg(SelectedDevice, cmdText);
        }
        string GetADBCmdArg(string device, string cmdText)
        {
            string _cmdText = cmdText;
            if (!string.IsNullOrEmpty(device))
            {
                _cmdText = "-s " + device + " " + _cmdText;
            }
            return _cmdText;
        }

        static string RunADB(string adbArg)
        {
            return StartProcess(EditorUtilityx.AndroidADBPath, adbArg, null);
        }

        static string StartProcess(string filePath, string argments = null, string workingDirectory = null)
        {
            StringBuilder result = new StringBuilder();
            StringBuilder error = null;
            using (var proc = new Process())
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = filePath;
                startInfo.Arguments = argments;
                if (!string.IsNullOrEmpty(workingDirectory))
                {
                    startInfo.WorkingDirectory = Path.GetFullPath(workingDirectory);
                }
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardError = true;

                proc.StartInfo = startInfo;
                proc.OutputDataReceived += (sender, e) =>
                {
                    result.AppendLine(e.Data);
                };
                proc.ErrorDataReceived += (sender, e) =>
                {
                    if (error == null)
                        error = new StringBuilder();
                    error.AppendLine(e.Data);
                };
                proc.EnableRaisingEvents = true;
                proc.Start();
                proc.BeginErrorReadLine();
                proc.BeginOutputReadLine();
                proc.WaitForExit();

                if (proc.ExitCode != 0)
                {
                    throw new ProcessException(error?.ToString(), filePath, argments, result.ToString());
                }
            }

            Log.Info(filePath + " " + argments + "\nresult: " + result + "\nerror: " + error);

            return result.ToString();
        }

        static string Cmd(string filePath, IEnumerable<string> cmdTexts, string workingDirectory = null)
        {

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = filePath;
            if (cmdTexts.Count() > 0)
                startInfo.Arguments = cmdTexts.First();
            if (!string.IsNullOrEmpty(workingDirectory))
            {
                startInfo.WorkingDirectory = Path.GetFullPath(workingDirectory);
            }
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            //startInfo.StandardOutputEncoding = Encoding.UTF8;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardError = true;
            //startInfo.StandardErrorEncoding = Encoding.UTF8;

            string result;

            using (var p = Process.Start(startInfo))
            {
                foreach (var cmdText in cmdTexts.Skip(1))
                {
                    Log.Info(cmdText);
                    p.StandardInput.WriteLine(cmdText);
                    p.StandardInput.Flush();
                }

                p.StandardInput.WriteLine("exit");
                p.StandardInput.Flush();
                p.StandardInput.Close();

                result = p.StandardOutput.ReadToEnd();

                if (p.ExitCode != 0)
                {
                    Debug.LogError(string.Join("\n", cmdTexts));
                    throw new Exception(result);
                }
            }


            return result;
        }

        static void Cmd(string file, string args, Action<StreamWriter, StreamReader> inputOutput)
        {
            Cmd(file, args, null, inputOutput);
        }
        static void Cmd(string file, string args, string workingDirectory, Action<StreamWriter, StreamReader> inputOutput)
        {

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = file;
            startInfo.Arguments = args;
            if (!string.IsNullOrEmpty(workingDirectory))
            {
                startInfo.WorkingDirectory = Path.GetFullPath(workingDirectory);
            }
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            //startInfo.StandardOutputEncoding = Encoding.UTF8;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardError = true;
            //startInfo.StandardErrorEncoding = Encoding.UTF8;


            using (var p = Process.Start(startInfo))
            {

                inputOutput(p.StandardInput, p.StandardOutput);
                //p.StandardInput.WriteLine("exit");
                //p.StandardInput.Flush();
                //p.StandardInput.Close();

                if (!p.WaitForExit(1000 * 3))
                {
                    Debug.LogError("timeout " + file + " " + args);
                }
                if (p.ExitCode != 0)
                {
                    if (p.HasExited)
                        throw new Exception(p.StandardOutput.ReadToEnd());
                    throw new Exception();
                }
            }
        }


        #region Version File

        private LocalVersionList GenerateReadWriteVersionList(IEnumerable<ResourceInfo> changedResources, IEnumerable<ResourceInfo> addedResources, IEnumerable<ResourceInfo> removedResources)
        {
            SortedDictionary<ResourceManager.ResourceName, ResourceInfo> readWriteResourceInfos;
            readWriteResourceInfos = new SortedDictionary<ResourceManager.ResourceName, ResourceInfo>();
            string tmpRemoteDir = Path.GetFullPath(TmpRemotePath);
            string readWriteVersionPath = Path.Combine(tmpRemoteDir, PackedVersionFileName);

            string newReadWriteVersionPath = Path.Combine(TmpLocalPath, PackedVersionFileName);
            if (File.Exists(newReadWriteVersionPath))
                File.Delete(newReadWriteVersionPath);

            if (File.Exists(readWriteVersionPath))
            {
                foreach (var res in ResourceInfo.GetReadWriteVersionSerializer().Deserialize(readWriteVersionPath).GetResourceInfos())
                {
                    readWriteResourceInfos[res.ResourceName] = res;
                }
            }

            foreach (var item in changedResources.Concat(addedResources))
            {
                var item2 = item;
                item2.Extension = ResourceInfo.GetExtension(item);
                readWriteResourceInfos[item2.ResourceName] = item2;
            }

            foreach (var item in removedResources)
            {
                readWriteResourceInfos.Remove(item.ResourceName);
            }
            LocalVersionList versionList;
            try
            {

                versionList = ResourceInfo.CreateLocalVersionList(readWriteResourceInfos.Values.ToArray());

                using (var fileStream = new FileStream(newReadWriteVersionPath, FileMode.Create, FileAccess.Write))
                {
                    if (!ResourceInfo.GetReadWriteVersionSerializer().Serialize(fileStream, versionList))
                    {
                        throw new GameFrameworkException("Serialize read write version list failure.");
                    }
                }
            }
            catch (Exception exception)
            {
                throw new GameFrameworkException(Utility.Text.Format("Generate read write version list exception '{0}'.", exception.ToString()), exception);
            }
            Log.Info($"Generate read write version, resource: {readWriteResourceInfos.Values.Count}");
            return versionList;
        }

        #endregion

    }


}