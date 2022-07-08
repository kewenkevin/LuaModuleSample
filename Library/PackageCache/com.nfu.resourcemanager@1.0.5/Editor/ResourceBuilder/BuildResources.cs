//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using ND.Managers.ResourceMgr.Framework;
using ND.Managers.ResourceMgr.Runtime;

namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{
    /// <summary>
    /// 生成资源。
    /// </summary>
    public static class BuildResources
    {

        //[MenuItem("Tools/ResourceMgr/资源编辑器立即生成", false, 39)]
        public static void RunAnalysisByLatestConfig()
        {
            var performance = new PerformanceSample($"{nameof(BuildResources)}.{nameof(RunAnalysisByLatestConfig)}");

            ResourceEditorImplate implate = new ResourceEditorImplate();
            implate.Load();
            implate.Run();
            Debug.Log($"Analysis assetbundle complete. ({performance.ElapsedSeconds:0.#}s)");
            performance.Dispose();
        }

        /// <summary>
        /// 运行生成资源。
        /// </summary>
        //[MenuItem("Tools/ResourceMgr/构建资源", false, 40)]
        public static void Run()
        {
            Run(null, null, null, null, null);
        }

        public static void Run(int internalResourceVersion)
        {
            Run((int?)internalResourceVersion, null, null, null, null);
        }

        public static void Run(Platform platforms)
        {
            Run(null, null, platforms, null, null);
        }

        public static void Run(string outputDirectory)
        {
            Run(null, null, null, outputDirectory, null);
        }


        public static void Run(int internalResourceVersion, bool isForceRebuildAssetBundle)
        {
            Run((int?)internalResourceVersion, isForceRebuildAssetBundle, null, null, null);
        }

        public static void Run(Platform platforms, bool isForceRebuildAssetBundle)
        {
            Run(null, isForceRebuildAssetBundle, platforms, null, null);
        }

        public static void Run(string outputDirectory, bool isForceRebuildAssetBundle)
        {
            Run(null, isForceRebuildAssetBundle, null, outputDirectory, null);
        }

        public static void Run(int internalResourceVersion, Platform platforms)
        {
            Run((int?)internalResourceVersion, null, platforms, null, null);
        }

        public static void Run(int internalResourceVersion, string outputDirectory)
        {
            Run((int?)internalResourceVersion, null, null, outputDirectory, null);
        }

        public static void Run(Platform platforms, string outputDirectory)
        {
            Run(null, null, platforms, outputDirectory, null);
        }

        public static void Run(string outputDirectory, string buildEventHandlerTypeName)
        {
            Run(null, null, Platform.FollowProject, outputDirectory, buildEventHandlerTypeName);
        }



        public static void Run(int internalResourceVersion, Platform platforms, bool isForceRebuildAssetBundle)
        {
            Run((int?)internalResourceVersion, isForceRebuildAssetBundle, platforms, null, null);
        }

        public static void Run(int internalResourceVersion, string outputDirectory, bool isForceRebuildAssetBundle)
        {
            Run((int?)internalResourceVersion, isForceRebuildAssetBundle, null, outputDirectory, null);
        }

        public static void Run(Platform platforms, string outputDirectory, bool isForceRebuildAssetBundle)
        {
            Run(null, isForceRebuildAssetBundle, platforms, outputDirectory, null);
        }

        public static void Run(string outputDirectory, string buildEventHandlerTypeName, bool isForceRebuildAssetBundle)
        {
            Run(null, isForceRebuildAssetBundle, null, outputDirectory, buildEventHandlerTypeName);
        }

        public static void Run(int internalResourceVersion, Platform platforms, string outputDirectory)
        {
            Run((int?)internalResourceVersion, null, platforms, outputDirectory, null);
        }

        public static void Run(int internalResourceVersion, string outputDirectory, string buildEventHandlerTypeName)
        {
            Run((int?)internalResourceVersion, null, null, outputDirectory, buildEventHandlerTypeName);
        }

        public static void Run(Platform platforms, string outputDirectory, string buildEventHandlerTypeName)
        {
            Run(null, null, platforms, outputDirectory, buildEventHandlerTypeName);
        }

        public static void Run(int internalResourceVersion, Platform platforms, string outputDirectory, string buildEventHandlerTypeName)
        {
            Run((int?)internalResourceVersion, null, platforms, outputDirectory, buildEventHandlerTypeName);
        }


        public static void Run(int internalResourceVersion, Platform platforms, string outputDirectory, bool isForceRebuildAssetBundle)
        {
            Run((int?)internalResourceVersion, isForceRebuildAssetBundle, platforms, outputDirectory, null);
        }
        public static void Run(int internalResourceVersion, string outputDirectory, string buildEventHandlerTypeName, bool isForceRebuildAssetBundle)
        {
            Run((int?)internalResourceVersion, isForceRebuildAssetBundle, null, outputDirectory, buildEventHandlerTypeName);
        }

        public static void Run(Platform platforms, string outputDirectory, string buildEventHandlerTypeName, bool isForceRebuildAssetBundle)
        {
            Run(null, isForceRebuildAssetBundle, platforms, outputDirectory, buildEventHandlerTypeName);
        }

        public static void Run(int internalResourceVersion, Platform platforms, string outputDirectory, string buildEventHandlerTypeName, bool isForceRebuildAssetBundle)
        {
            Run((int?)internalResourceVersion, isForceRebuildAssetBundle, platforms, outputDirectory, buildEventHandlerTypeName);
        }


        private static void Run(int? internalResourceVersion, bool? isForceRebuildAssetBundle, Platform? platforms, string outputDirectory, string buildEventHandlerTypeName)
        {
            var performance = new PerformanceSample($"{nameof(BuildResources)}.{nameof(Run)}");

            Debug.Log("AnalysisOnBuild: " + EditorResourceSettings.AnalysisOnBuild);
            if (EditorResourceSettings.AnalysisOnBuild)
            {
                RunAnalysisByLatestConfig();
            }

            ResourceBuilderController controller = new ResourceBuilderController();
            if (!controller.Load())
            {
                throw new GameFrameworkException("Load configuration failure.");
            }
            else
            {
                Debug.Log("Load configuration success.");
            }

            //默认读取配置的平台
            if (platforms.HasValue)
            {
                controller.Platforms = platforms.Value;
            }

            if (internalResourceVersion.HasValue)
            {
                controller.InternalResourceVersion = internalResourceVersion.Value;
            }

            if (outputDirectory != null)
            {
                controller.OutputDirectory = outputDirectory;
            }

            if (buildEventHandlerTypeName != null)
            {
                controller.BuildEventHandlerTypeName = buildEventHandlerTypeName;
            }

            if (isForceRebuildAssetBundle.HasValue)
            {
                controller.ForceRebuildAssetBundleSelected = isForceRebuildAssetBundle.Value;
            }



            if (!controller.IsValidOutputDirectory)
            {
                throw new GameFrameworkException(Utility.Text.Format("Output directory '{0}' is invalid.", controller.OutputDirectory));
            }

            if (!controller.BuildResources())
            {
                throw new GameFrameworkException($"Build resources failure. ({performance.ElapsedSeconds:0.#}s)");
            }
            else
            {
                Debug.Log($"Build resources success. ({performance.ElapsedSeconds:0.#}s)");
                controller.Save();
            }

            performance.Dispose();
        }



        /// <summary>
        /// 清空Assets/StreamingAssets目录
        /// </summary>
        public static void ClearStreamingAssets()
        {
            ClearStreamingAssets(null);
        }

        public static void ClearStreamingAssets(string relativePath)
        {
            string streamingAssetsPath = Utility.Path.GetRegularPath(Path.GetFullPath(EditorResourceSettings.EditorStreamingAssetsPath));
            string targetPath;
            if (string.IsNullOrEmpty(relativePath))
            {
                targetPath = streamingAssetsPath;
            }
            else
            {
                targetPath = Path.Combine(streamingAssetsPath, relativePath);
            }
            Debug.Log("ClearStreamingAssets: " + targetPath.ToRelativePath("."));

            if (!Directory.Exists(streamingAssetsPath))
            {
                return;
            }

            if (Directory.Exists(targetPath))
            {
                foreach (string fileName in MetaFile.EnumerateFiles(targetPath, true))
                {
                    if (fileName.Contains(".gitkeep"))
                    {
                        continue;
                    }
                    File.Delete(fileName);
                }
            }

            if (!string.IsNullOrEmpty(relativePath))
            {
                foreach (var pattern in new string[] {
                ResourceBuilderController.LocalVersionListFileName ,
                ResourceBuilderController.RemoteVersionListFileName })
                {
                    string file;
                    file = Directory.GetFiles(streamingAssetsPath, pattern, SearchOption.TopDirectoryOnly).FirstOrDefault();
                    if (File.Exists(file))
                        File.Delete(file);
                }
            }
        }



        public static void CopyToStreamingAsset(string workingPath, string outputPackagePath, string outputFullPath, string outputPackedPath)
        {
            using (new PerformanceSample("CopyToStreamingAssets"))
            {
                EditorUtility.DisplayProgressBar("Copy To StreamingAssets", "", 0f);
                string streamingAssetsPath = Utility.Path.GetRegularPath(Path.GetFullPath(EditorResourceSettings.EditorStreamingAssetsPath));
                AssetDatabase.StartAssetEditing();
                ClearStreamingAssets();
                Debug.Log("CopyToStreamingAsset: " + outputPackagePath.ToRelativePath(".") + " > " + streamingAssetsPath.ToRelativePath("."));

                if (!Directory.Exists(outputPackedPath))
                    throw new System.Exception("Directory not exists. path: " + outputPackedPath);
                CopyToTargetPath(streamingAssetsPath, workingPath, outputPackagePath, outputFullPath, outputPackedPath);
                MetaFile.DeleteEmptyChildrenDirectory(streamingAssetsPath, true);
                CheckTargetDirectoryCase(streamingAssetsPath, outputPackagePath);
                AssetDatabase.StopAssetEditing();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();
            }
        }
        public static void CopyToStreamingAsset(string relativePath, string workingPath, string outputPackagePath, string outputFullPath, string outputPackedPath)
        {
            using (new PerformanceSample("CopyToStreamingAssets"))
            {
                EditorUtility.DisplayProgressBar("Copy To StreamingAssets", "", 0f);
                string streamingAssetsPath = Utility.Path.GetRegularPath(Path.GetFullPath(EditorResourceSettings.EditorStreamingAssetsPath));
                AssetDatabase.StartAssetEditing();
                ClearStreamingAssets(relativePath);

                Debug.Log("CopyToStreamingAsset: " + outputPackagePath.ToRelativePath(".") + " > " + Path.Combine(streamingAssetsPath, relativePath).ToRelativePath("."));

                if (!Directory.Exists(outputPackedPath))
                    throw new System.Exception("Directory not exists. path: " + outputPackedPath);
                CopyToTargetPath(streamingAssetsPath, relativePath, workingPath, outputPackagePath, outputFullPath, outputPackedPath);
                MetaFile.DeleteEmptyChildrenDirectory(streamingAssetsPath, true);
                CheckTargetDirectoryCase(streamingAssetsPath, outputPackagePath);
                AssetDatabase.StopAssetEditing();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();
            }
        }

        public static void CheckTargetDirectoryCase(string targetDir, string srcDir)
        {
            HashSet<string> srcRelDirs = new HashSet<string>();
            Dictionary<string, string> srcRelDirMap = new Dictionary<string, string>();

            foreach (var dir in Directory.GetDirectories(srcDir, "*", SearchOption.AllDirectories))
            {
                string relDir = dir.ToRelativePath(srcDir).Replace('\\', '/');
                srcRelDirs.Add(relDir);
                srcRelDirMap[relDir.ToLower()] = relDir;
            }

            foreach (var dir in Directory.GetDirectories(targetDir, "*", SearchOption.AllDirectories))
            {
                var relDir = dir.ToRelativePath(targetDir).Replace('\\', '/');

                if (!srcRelDirs.Contains(relDir))
                {
                    string dirPath = Path.Combine(targetDir, srcRelDirMap[relDir.ToLower()]);
                    string guid = Guid.NewGuid().ToString();
                    Directory.Move(dir, dir + guid);
                    Directory.Move(dir + guid, dirPath);
                }
            }
        }

        /// <summary>
        /// 拷贝资源到目标路径
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="workingPath"></param>
        /// <param name="outputPackagePath"></param>
        /// <param name="outputFullPath"></param>
        /// <param name="outputPackedPath"></param>
        public static void CopyToTargetPath(string targetPath, string workingPath, string outputPackagePath, string outputFullPath,
            string outputPackedPath)
        {
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            int totalFile = 0;
            //COPY PACK + VERSIONLIST 支持热更检查模式
            string[] fileNames = Directory.GetFiles(outputPackedPath, "*", SearchOption.AllDirectories);

            foreach (string fileName in fileNames)
            {
                string destFileName = Utility.Path.GetRegularPath(Path.Combine(targetPath,
                    fileName.Substring(outputPackedPath.Length)));
                System.IO.FileInfo destFileInfo = new System.IO.FileInfo(destFileName);
                if (!destFileInfo.Directory.Exists)
                {
                    destFileInfo.Directory.Create();
                }
                File.Copy(fileName, destFileName);
                totalFile++;
            }

            //COPY GameFrameworkVersion 支持单机模式打包运行
            fileNames = Directory.GetFiles(outputPackagePath, "*"+ResourceBuilderController.RemoteVersionListFileName, SearchOption.AllDirectories);

            foreach (string fileName in fileNames)
            {
                string destFileName = Utility.Path.GetRegularPath(Path.Combine(targetPath,
                    fileName.Substring(outputPackagePath.Length)));
                System.IO.FileInfo destFileInfo = new System.IO.FileInfo(destFileName);
                if (!destFileInfo.Directory.Exists)
                {
                    destFileInfo.Directory.Create();
                }
                File.Copy(fileName, destFileName);
                totalFile++;
            }

            Log.Info($"Copy total file : {totalFile}");
        }

        public static void CopyToTargetPath(string targetRootPath, string targetRelativePath, string workingPath, string outputPackagePath, string outputFullPath,
          string outputPackedPath)
        {
            string targetPath;
            if (string.IsNullOrEmpty(targetRelativePath))
                targetRelativePath = "";
            targetPath = Path.Combine(targetRootPath, targetRelativePath);

            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            int totalFile = 0;

            //COPY PACK + VERSIONLIST 支持热更检查模式
            totalFile += MetaFile.CopyDirectory(Path.Combine(outputPackedPath, targetRelativePath), targetPath);

            //COPY GameFrameworkVersion 支持单机模式打包运行
            string file = Path.Combine(outputPackagePath, ResourceBuilderController.RemoteVersionListFileName);
            if (File.Exists(file))
            {
                File.Copy(file, Path.Combine(targetRootPath, ResourceBuilderController.RemoteVersionListFileName));
                totalFile++;
            }

            if (!string.IsNullOrEmpty(targetRelativePath))
            {
                file = Path.Combine(outputPackedPath, ResourceBuilderController.LocalVersionListFileName);
                if (File.Exists(file))
                {
                    File.Copy(file, Path.Combine(targetRootPath, ResourceBuilderController.LocalVersionListFileName));
                    totalFile++;
                }
            }
            Log.Info($"Copy total file : {totalFile}");
        }

        /// <summary>
        /// 编译 lua
        /// </summary>
        public static string CompileLua(string outputPath = null)
        {

            //获取 lua 编译程序
            LuaPreprocessBuild luaPreprocessBuild = null;
            foreach (var group in EditorResourceSettings.Groups)
            {
                foreach (var g in group.PreprocessBuilds)
                {
                    if (g.PreprocessBuild is LuaPreprocessBuild)
                    {
                        luaPreprocessBuild = g.PreprocessBuild as LuaPreprocessBuild;
                        break;
                    }
                }
            }

            if (luaPreprocessBuild == null)
                throw new Exception($"[Resources Settings] [Preprocess Build] require <{nameof(LuaPreprocessBuild)}> provider");

            string originOutputPath = null;

            //设置输出路径
            originOutputPath = luaPreprocessBuild.outputPath;
            if (string.IsNullOrEmpty(outputPath))
                outputPath = luaPreprocessBuild.outputPath;

            if (outputPath != originOutputPath)
                luaPreprocessBuild.outputPath = outputPath;

            try
            {
                //编译 lua 
                luaPreprocessBuild.PreprocessBuildInitialize();
                luaPreprocessBuild.PreprocessBuild();
                luaPreprocessBuild.PreprocessBuildCleanup();
            }
            catch
            {
                throw;
            }

            finally
            {
                //恢复输出路径
                if (outputPath != originOutputPath)
                    luaPreprocessBuild.outputPath = originOutputPath;
            }

            AssetDatabase.Refresh();


            return outputPath;
        }

        [MenuItem(EditorUtilityx.MenuPrefix + "Build Lua", priority = -28)]
        public static void BuildLua()
        {
            var performance = new PerformanceSample($"{nameof(BuildResources)}.Build Lua");


            ResourceBuilderController controller = new ResourceBuilderController();
            if (!controller.Load(incrementalVersion: false))
            {
                throw new GameFrameworkException("Load configuration failure.");
            }
            else
            {
                Debug.Log("Load configuration success.");
            }

            var platform = controller.CurrentPlatform;

            //获取已有的最新资源版本号
            string lastestVersionFolderName = controller.FindLastestVersionFolderName();

            if (string.IsNullOrEmpty(lastestVersionFolderName))
                throw new Exception("Not found lastest resources version");

            Debug.Log("Lastest resources build version: " + lastestVersionFolderName);

            ResourceBuilderController.ParseResVersion(lastestVersionFolderName, out var resVer, out var appVer);

            controller.InternalResourceVersion = resVer;
            controller.PartBuildTriggerEvent = true;

            //生成 lua
            string rootAssetPath = CompileLua();

            //获取打包的lua文件
            List<string> luaFiles = new List<string>();
            foreach (var file in MetaFile.EnumerateFiles(rootAssetPath, true))
            {
                luaFiles.Add(file);
            }

            BuildLua(controller, rootAssetPath, luaFiles.ToArray());

            //复制到 StreamingAssets
            if (EditorResourceSettings.CopyToStreamingAssets && controller.IsCurrentPlatform(platform))
            {
                string workingPath, outputPackagePath, outputFullPath, outputPackedPath;
                controller.GetOutputPath(platform, out workingPath, out outputPackagePath, out outputFullPath, out outputPackedPath);
                CopyToStreamingAsset(rootAssetPath, workingPath, outputPackagePath, outputFullPath, outputPackedPath);
            }
            performance.Dispose();
        }

        public static void BuildLua(ResourceBuilderController controller, string rootAssetPath, string[] assetPaths, string[] filePaths = null)
        {
            rootAssetPath = rootAssetPath.Replace('\\', '/').ToLower();

            ResourceEditorController resourceEditorController = new ResourceEditorController();
            resourceEditorController.LoadConfig();
            var analysisHandler = resourceEditorController.GetAnalysisHandler();

            //生成需要打包的 Resource
            Dictionary<string, Resource> resources = new Dictionary<string, Resource>();
            Resource resource;
            for (int i = 0; i < assetPaths.Length; i++)
            {
                string assetPath = assetPaths[i];
                assetPath = assetPath.Replace('\\', '/');
                var dir = Path.GetDirectoryName(assetPath);
                dir = dir.Replace('\\', '/');
                string fileSystem = rootAssetPath + "/lua";
                if (dir.Length > rootAssetPath.Length)
                    fileSystem += "_" + dir.Substring(rootAssetPath.Length + 1).Replace('/', '_');

                var resourceName = fileSystem + "_" + Path.GetFileNameWithoutExtension(assetPath);
                if (!resources.TryGetValue(resourceName, out resource))
                {
                    resource = Resource.Create(resourceName, null, fileSystem, LoadType.LoadFromBinary, true, new string[0]);
                    resources[resourceName] = resource;
                    analysisHandler.OnPreprocessResource(resource);
                }
                Asset asset = Asset.CreateByAssetPath(assetPath, resource);
                asset.DependencyAssetPaths = new string[0];
                if (filePaths != null && filePaths.Length > i)
                    asset.FilePath = filePaths[i];
                resource.AssignAsset(asset, false);                
            }

            if (!controller.BuildPartResources(resources.Values.ToArray(), rootAssetPath))
            {
                throw new GameFrameworkException($"Build resources failure.");
            }
            else
            {
                Debug.Log($"Build resources success.");
                controller.Save();
            }

        }

    }
}
