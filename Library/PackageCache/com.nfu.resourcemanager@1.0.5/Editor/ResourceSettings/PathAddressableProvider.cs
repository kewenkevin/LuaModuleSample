using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{
    /// <summary>
    /// AssetBundle名称取文件路径，不包含文件扩展名
    /// </summary>
    public class PathAddressableProvider : IAddressableProvider
    {
        public bool GetAddressableName(string assetPath, out string bundleName, out string variant, out string assetName)
        {
            variant = null;
            assetName = assetPath;

            string dir = Path.GetDirectoryName(assetPath);
            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            bundleName = Path.Combine(dir, fileName).Replace('\\', '/');
             
            bundleName = EditorResourceSettings.FormatAssetBundleName(bundleName);
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