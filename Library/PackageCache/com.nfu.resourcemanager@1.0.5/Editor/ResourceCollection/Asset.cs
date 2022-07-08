//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------
using System;
using UnityEditor;

namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{
    /// <summary>
    /// 资源。
    /// </summary>
    public sealed class Asset : IComparable<Asset>
    {
        private string assetPathLower;

        private Asset()
        {
        }

        private Asset(string guid, Resource resource)
        {
            Guid = guid;
            Resource = resource;
            AssetPath = AssetDatabase.GUIDToAssetPath(Guid);
        }

        public string Guid
        {
            get;
            private set;
        }

        public string Name
        {
            get
            {
                return AssetPath;
            }
        }
        public string AssetPath
        {
            get;
            set;
        }
        public string AssetPathLower
        {
            get
            {
                if (assetPathLower == null)
                    assetPathLower = AssetPath.ToLower();
                return assetPathLower;
            }
        }


        public Resource Resource
        {
            get;
            set;
        }


        public string[] DependencyAssetPaths
        {
            get; 
            set;
        }

        /// <summary>
        /// 支持 Assets 目录外的资源打包，如: Lua
        /// </summary>
        public string FilePath { get; set; }

        public string GetFilePath()
        {
            if (!string.IsNullOrEmpty(FilePath))
                return FilePath;
            return AssetPath;
        }

        public int CompareTo(Asset asset)
        {
            return string.Compare(Guid, asset.Guid, StringComparison.Ordinal);
        }

        public static Asset Create(string guid)
        {
            return new Asset(guid, null);
        }

        public static Asset Create(string guid, Resource resource)
        {
            return new Asset(guid, resource);
        }

        public static Asset CreateByAssetPath(string assetPath, Resource resource)
        {
            return new Asset()
            {
                AssetPath = assetPath,
                Resource = resource
            };
        }
    }
}
