
// 本文件下提供 资源构建后相关操作的辅助 比如将构建好的目录下特定版本的资源文件拷入包下的streamingAsset中
//------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ND.Managers.ResourceMgr.Editor.ResourceTools;
using ND.Managers.ResourceMgr.Framework;
using ND.Managers.ResourceMgr.Framework.FileSystem;
using FileInfo = ND.Managers.ResourceMgr.Framework.FileSystem.FileInfo;

namespace ND.Gameplay.Managers.ResourceManagerV2.Editor
{
    /// <summary>
    /// SteamingAssets下资源产生的辅助类
    /// </summary>
    public class StreamingAssetBuild
    {
        [MenuItem("Tools/Build/Copy Latest Build To StreamingAsset")]
        public static void GenerateStreamingAssetByLatest()
        {
            string outputPath = Application.dataPath + "/../Output";
            string latestVersion = getDefaultOutputRootLatestVersion(outputPath);
            GenerateStreamingAssetByVersion(outputPath,latestVersion);
        }
        
        /// <summary>
        /// 拷贝最新的版本至StreamingAsset下
        /// </summary>
        /// <param name="outputPath"></param>
        public static void GenerateStreamingAssetByLatest(string outputPath)
        {
            string latestVersion = getDefaultOutputRootLatestVersion(outputPath);
            GenerateStreamingAssetByVersion(outputPath,latestVersion);
        }
        
        /// <summary>
        /// 拷贝指定的版本至StreamingAsset下
        /// </summary>
        /// <param name="outputPath"></param>
        /// <param name="latestVersion"></param>
        public static void GenerateStreamingAssetByVersion(string outputPath, string latestVersion)
        {
            ClearStreamingAssets();
            GenerateStreamingAssetByVersion(outputPath,latestVersion,currentPlatform);
        }

        #region 路径计算

        /// <summary>
        /// 获取当前的平台情况
        /// </summary>
        private static Platform currentPlatform
        {
            get
            {
#if UNITY_ANDROID
                return Platform.Android;
#elif UNITY_IOS
                return Platform.IOS;
#elif UNITY_STANDALONE_WIN
                return Platform.Windows;
#elif UNITY_STANDALONE_OSX
                return Platform.MacOS;
#elif UNITY_STANDALONE_LINUX
                return Platform.Linux;
#endif
            }
        }

        /// <summary>
        /// 获取目前打包过目录下存在的最新版本资源的版本号
        /// </summary>
        /// <param name="realOutPutPath"></param>
        /// <returns></returns>
        /// <exception cref="Exception">如果版本文件无记录则抛出异常</exception>
        private static string getDefaultOutputRootLatestVersion(string realOutPutPath)
        {
            //从构建报告确认版本号
            var directories = Directory.GetDirectories(realOutPutPath+"/BuildReport", "*_*_*_*");

            var paths = directories.ToList();
            
            //根据文件夹名字确定的版本号进行排序，越新版本越靠后
            paths.Sort((s, s1) =>
            {
                var v1 =Path.GetFileName(s).Split('_');
                var v2 = Path.GetFileName(s1).Split('_');

                for (int i = 0; i < 4; i++)
                {
                    var p1= Int32.Parse(v1[i]);
                    var p2 = Int32.Parse(v2[i]);

                    if (p1 > p2) return 1;
                    else if (p1 < p2) return -1;
                }
                return 0;
            });

            //取出最新版本号返回
            if(paths.Count>0)
                return Path.GetFileName(paths[paths.Count-1]);
            else
                throw new Exception("There's No Avaliable Resource Build Record In "+ realOutPutPath);
        }


        /// <summary>
        /// 生成AB的工作路径
        /// </summary>
        /// <param name="outputDirectory"></param>
        /// <returns></returns>
        private static string getWorkingPath(string outputDirectory)
        {
            return Utility.Path.GetRegularPath(new DirectoryInfo(Utility.Text.Format("{0}/Working/", outputDirectory))
                .FullName);
        }

        /// <summary>
        /// 单机模式打包全量路径路径
        /// </summary>
        /// <param name="outputDirectory"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        private static string getOutputPackagePath(string outputDirectory, string version)
        {
            return Utility.Path.GetRegularPath(
                new DirectoryInfo(Utility.Text.Format("{0}/Package/{1}/", outputDirectory, version)).FullName);
        }


        /// <summary>
        /// 全量热更资源路径
        /// </summary>
        /// <param name="outputDirectory"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        private static string getOutputFullPath(string outputDirectory, string version)
        {

            return Utility.Path.GetRegularPath(
                new DirectoryInfo(Utility.Text.Format("{0}/Full/{1}/", outputDirectory, version)).FullName);
        }

        /// <summary>
        /// 热更包内资源路径
        /// </summary>
        /// <param name="outputDirectory"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        private static string getOutputPackedPath(string outputDirectory, string version)
        {
            return Utility.Path.GetRegularPath(
                new DirectoryInfo(Utility.Text.Format("{0}/Packed/{1}/", outputDirectory, version)).FullName);
        }

        #endregion
        
        /// <summary>
        /// 根据版本号拷贝资源到streamingAsset路径
        /// </summary>
        /// <param name="outPutRootPath">打包后资源所在的根目录</param>
        /// <param name="bundleVersion">Application的版本号 如：1.0.0</param>
        /// <param name="resVersion">资源版本号 internalResourceVersion</param>
        /// <param name="platform">要拷贝资源所属平台信息</param>
        public static void GenerateStreamingAssetByVersion(string outPutRootPath,string bundleVersion, string resVersion,Platform platform)
        {
            GenerateStreamingAssetByVersion(outPutRootPath,bundleVersion + "." + resVersion,platform);
        }

        /// <summary>
        /// 根据版本号拷贝资源到streamingAsset路径
        /// </summary>
        /// <param name="outPutRootPath">打包后资源所在的根目录</param>
        /// <param name="version">整体版本号 如1.0.0.113561</param>
        /// <param name="platform">要拷贝资源所属平台信息</param>
        public static void GenerateStreamingAssetByVersion(string outPutRootPath,string version,Platform platform)
        {
            version = version.Replace('.', '_');

            string platformName = platform.ToString();

            string workingPath = Utility.Text.Format("{0}{1}/", getWorkingPath(outPutRootPath), platformName);
            string outputPackagePath = Utility.Text.Format("{0}{1}/", getOutputPackagePath(outPutRootPath,version), platformName);
            string outputFullPath = Utility.Text.Format("{0}{1}/", getOutputFullPath(outPutRootPath,version), platformName);
            string outputPackedPath = Utility.Text.Format("{0}{1}/", getOutputPackedPath(outPutRootPath,version), platformName);
            
            CopyToStreamingAsset(workingPath,outputPackagePath,outputFullPath,outputPackedPath);
        }
        
        /// <summary>
        /// 清空Assets/StreamingAssets目录
        /// </summary>
        public static void ClearStreamingAssets()
        {
            string streamingAssetsPath = Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "StreamingAssets"));
            if (!Directory.Exists(streamingAssetsPath))
            {
                Directory.CreateDirectory(streamingAssetsPath);
            }
            
            string[] fileNames = Directory.GetFiles(streamingAssetsPath, "*", SearchOption.AllDirectories);
            foreach (string fileName in fileNames)
            {
                if (fileName.Contains(".gitkeep"))
                {
                    continue;
                }

                File.Delete(fileName);
            }

            Utility.Path.RemoveEmptyDirectory(streamingAssetsPath);
        }

        /// <summary>
        /// 拷贝资源到目标路径
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="workingPath"></param>
        /// <param name="outputPackagePath"></param>
        /// <param name="outputFullPath"></param>
        /// <param name="outputPackedPath"></param>
        public static void CopyToTargetPath(string targetPath,string workingPath,string outputPackagePath, string outputFullPath,
            string outputPackedPath)
        {
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            
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
            }
            
            //COPY GameFrameworkVersion 支持单机模式打包运行
            fileNames = Directory.GetFiles(outputPackagePath, "*GameFrameworkVersion.dat", SearchOption.AllDirectories);

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
            }
        }
        
        public static void CopyToStreamingAsset(string workingPath,string outputPackagePath, string outputFullPath,
            string outputPackedPath)
        {
            string streamingAssetsPath =
                Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "StreamingAssets"));
            CopyToTargetPath(streamingAssetsPath,workingPath,outputPackagePath, outputFullPath, outputPackedPath);
        }
    }
}