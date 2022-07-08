using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{
    /// <summary>
    /// AssetBundle名称取文件夹名称
    /// </summary>
    public class FolderAddressableProvider : IAddressableProvider
    {
        public bool GetAddressableName(string assetPath, out string bundleName, out string variant, out string assetName)
        {
            variant = null;
            string dir = Path.GetDirectoryName(assetPath).Replace('\\','/');
            bundleName = dir;
            bundleName = EditorResourceSettings.FormatAssetBundleName(bundleName);
            assetName = assetPath;
            return true;
        }

        public bool IsAssetPathMatch(string assetPath)
        {
            return true;
        }

        public void OnPreprocessAnalysis(string assetPath, ResourceEditorImplate builder)
        {

        }
    }

}