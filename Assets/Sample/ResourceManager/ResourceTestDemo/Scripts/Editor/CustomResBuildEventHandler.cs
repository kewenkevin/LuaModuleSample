using System.IO;
using ResourceMgr.Runtime.ResourceUpdate;
using ND.Managers.ResourceMgr.Editor.ResourceTools;
using ND.Managers.ResourceMgr.Framework;
using UnityEditor;
using UnityEngine;

namespace ND.Gameplay.Managers.ResourceManagerV2.Editor
{
    public class CustomResBuildEventHandler: IResBuildEventHandler
    {
        public bool ContinueOnFailure
        {
            get
            {
                return false;
            }
        }
        private string outputDirectory;
        private string applicableGameVersion;
        private int internalResourceVersion;
        private string unityVersion;

        public void OnPreprocessAllPlatforms(string productName, string companyName, string gameIdentifier,
            string gameFrameworkVersion, string unityVersion, string applicableGameVersion, int internalResourceVersion, BuildAssetBundleOptions buildOptions, bool zip,
            string outputDirectory, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath, string buildReportPath)
        {
            this.outputDirectory = outputDirectory ;
            this.applicableGameVersion = applicableGameVersion;
            this.internalResourceVersion = internalResourceVersion;
            this.unityVersion = unityVersion;

            StreamingAssetBuild.ClearStreamingAssets();
        }

        public void OnPostprocessAllPlatforms(string productName, string companyName, string gameIdentifier,
            string gameFrameworkVersion, string unityVersion, string applicableGameVersion, int internalResourceVersion, BuildAssetBundleOptions buildOptions, bool zip,
            string outputDirectory, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath, string buildReportPath)
        {
            Debug.Log("OnPostprocessAllPlatforms");
        }

        public void OnPreprocessPlatform(Platform platform, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath)
        {
            Debug.Log("OnPreprocessPlatform");
        }

        public void OnBuildAssetBundlesComplete(Platform platform, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath, AssetBundleManifest assetBundleManifest)
        {
            Debug.Log("OnBuildAssetBundlesComplete");
        }



        public void OnOutputUpdatableVersionListData(Platform platform, string versionListPath, int versionListLength,
            int versionListHashCode, int versionListZipLength, int versionListZipHashCode)
        {
            var reportPath = outputDirectory + "/BuildReport";
            var fullPath = outputDirectory + "/Full";
            if (!Directory.Exists(reportPath))
                Directory.CreateDirectory(reportPath);

            string version = string.Format("{0}_{1}", applicableGameVersion.Replace('.', '_'),
                internalResourceVersion);
            string versionPath = HostServerData.Current.ServerFilePath + $"versionInfos/{EditorUserBuildSettings.activeBuildTarget}";
            if (!Directory.Exists(versionPath))
                Directory.CreateDirectory(versionPath);
            

            VersionInfo versionInfo = new VersionInfo();
            versionInfo.ForceUpdateGame = false;
            versionInfo.LatestGameVersion = applicableGameVersion;
            versionInfo.InternalResourceVersion = internalResourceVersion;
            versionInfo.UpdatePrefixUri = $"http://{HostServerData.Current.IpAddress}:{HostServerData.Current.ServerPort}/Full/{version}/{platform.ToString()}";
            versionInfo.VersionListLength = versionListLength;
            versionInfo.VersionListHashCode = versionListHashCode;
            versionInfo.VersionListZipLength = versionListZipLength;
            versionInfo.VersionListZipHashCode = versionListZipHashCode;
            File.WriteAllText(versionPath + "/versionInfo.json",JsonUtility.ToJson((VersionInfoSeriazable)versionInfo));
        }

        public void OnPostprocessPlatform(Platform platform, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath, bool isSuccess)
        {
            Debug.Log("CustomResBuildEventHandler.OnPostprocessPlatform");
            if (!outputPackageSelected)
            {
                return;
            }
            string ServerPath = HostServerData.Current.ServerFilePath + "Demo/" + ((platform == Platform.Windows || platform == Platform.Windows64) ? Platform.Windows : platform);;
            CopyFullToServer(outputFullPath, ServerPath);
        }
        private void CopyFullToServer(string outFullPath, string serverPath)
        {
            if (!Directory.Exists(serverPath))
            {
                Directory.CreateDirectory(serverPath);
            }
            string[] fileNames = Directory.GetFiles(outFullPath, "*", SearchOption.AllDirectories);
            
            foreach (string fileName in fileNames)
            {
                string destFileName = Utility.Path.GetRegularPath(Path.Combine(serverPath,
                    fileName.Substring(outFullPath.Length)));
                System.IO.FileInfo destFileInfo = new System.IO.FileInfo(destFileName);
                if (!destFileInfo.Directory.Exists)
                {
                    destFileInfo.Directory.Create();
                }
                File.Copy(fileName, destFileName, true);
            }
        }
        
        public static void ClearServerAssets(string serverPath)
        {
           
            if (!Directory.Exists(serverPath))
            {
                Directory.CreateDirectory(serverPath);
            }
            
            string[] fileNames = Directory.GetFiles(serverPath, "*", SearchOption.AllDirectories);
            foreach (string fileName in fileNames)
            {
                if (fileName.Contains(".gitkeep"))
                {
                    continue;
                }

                File.Delete(fileName);
            }

            Utility.Path.RemoveEmptyDirectory(serverPath);
        }
    }
}