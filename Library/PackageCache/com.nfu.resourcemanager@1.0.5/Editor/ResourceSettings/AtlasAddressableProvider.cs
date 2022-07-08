using System;
using System.IO;
using ND.Managers.ResourceMgr.Runtime;

namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{

    /// <summary>
    /// 设置图集资源包
    /// </summary>
    internal class AtlasAddressableProvider : IAddressableProvider
    {

        private const string AtlasExtension = ".spriteatlas";

        string GetAtlasPath()
        {
            string spriteAtlasPath = ResourceSettings.AtlasPath;

            if (string.IsNullOrEmpty(spriteAtlasPath))
                throw new System.Exception("[Atlas Path] null");
            spriteAtlasPath = spriteAtlasPath.Replace('\\', '/');
            return spriteAtlasPath;
        }

        public bool IsAssetPathMatch(string assetPath)
        {
            string spriteAtlasPath = GetAtlasPath();
            if (!(assetPath.StartsWith(spriteAtlasPath, StringComparison.InvariantCultureIgnoreCase) || assetPath.IndexOf("/" + spriteAtlasPath, StringComparison.InvariantCultureIgnoreCase) >= 0))
                return false;
            return true;
        }


        public bool GetAddressableName(string assetPath, out string bundleName, out string variant, out string assetName)
        {
            variant = null;

            string spriteAtlasPath = GetAtlasPath();
            if (!spriteAtlasPath.EndsWith("/"))
                spriteAtlasPath = spriteAtlasPath + "/";


            if (Path.GetExtension(assetPath).EndsWith(AtlasExtension, System.StringComparison.InvariantCultureIgnoreCase))
            {
                //图集
                //Root/a.spriteatlas => Root/a_spriteatlas
                bundleName = assetPath;
                bundleName = EditorResourceSettings.FormatAssetBundleName(bundleName);
            }
            else
            {
                //单个图
                //Root/a/b.png => Root/a_spriteatlas
                //Root/a/b/c.png => Root/a_b_spriteatlas
                //支持变体
                int index;
                index = assetPath.IndexOf(spriteAtlasPath, StringComparison.InvariantCultureIgnoreCase);
                string dir = Path.GetDirectoryName(assetPath);
                string baseDir = dir.Substring(0, index + spriteAtlasPath.Length);
                string subDir = dir.Substring(index + spriteAtlasPath.Length);
                //拼接图集子目录
                bundleName = Path.Combine(baseDir, subDir.Replace('\\', '/').Replace("/", EditorResourceSettings.AtlasDirectorySeparator) + ".spriteatlas");

                bundleName = bundleName.Replace('\\', '/');
                bundleName = EditorResourceSettings.FormatAssetBundleName(bundleName);
            }
            assetName = assetPath;
            return true;
        }

        public void OnPreprocessAnalysis(string assetPath, ResourceEditorImplate builder)
        {

        }
    }
}