using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{

    /// <summary>
    /// 设置 Lua 资源包
    /// </summary>
    internal class LuaAddressableProvider : IAddressableProvider
    {

        [Tooltip("Lua 根目录")]
        public string rootPath = "Assets/Lua";


        public bool IsAssetPathMatch(string assetPath)
        {
            if (!assetPath.StartsWith(rootPath))
                return false;
            return true;
        }

        public bool GetAddressableName(string assetPath, out string bundleName, out string variant, out string assetName)
        {
            bundleName = null;
            variant = null;
            assetName = null;
            return false;
        }

        public void OnPreprocessAnalysis(string assetPath, ResourceEditorImplate builder)
        {

            var file = assetPath;

            var dir = Path.GetDirectoryName(file);
            dir = dir.Replace('\\', '/');
            string startPath = this.rootPath;
            startPath = startPath.Replace('\\', '/');
            if (startPath.EndsWith("/"))
                startPath = startPath.Substring(0, startPath.Length - 1);

            string fileSystemName = startPath + "/Lua";
            if (dir.Length > startPath.Length)
                fileSystemName += "_" + dir.Substring(startPath.Length + 1).Replace('/', '_');

            var resourceName = fileSystemName + "_" + Path.GetFileNameWithoutExtension(file);

            Resource resource = builder.Controller.GetResource(resourceName, null);
            if (resource == null)
            {
                builder.AddResource(resourceName, null, false);
                resource = builder.Controller.GetResource(resourceName, null);
            }

            if (resource != null)
            {
                resource.FileSystem = fileSystemName;
                resource.LoadType = LoadType.LoadFromBinary;
                SourceAsset asset = builder.Controller.SourceAssetRoot.GetAssetByPath(file);
                if (asset == null)
                    Debug.LogError("file asset null: " + file);
                builder.AssignAsset(asset, resource);
            }
        }
    }
}