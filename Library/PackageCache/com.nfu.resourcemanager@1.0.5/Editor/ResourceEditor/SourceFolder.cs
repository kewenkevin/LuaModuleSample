//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Collections.Generic;
using ND.Managers.ResourceMgr.Framework;
using UnityEditor;
using UnityEngine;

namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{
    public sealed class SourceFolder
    {
        private static Texture s_CachedIcon = null;

        private readonly List<SourceFolder> m_Folders;
        private readonly List<SourceAsset> m_Assets;

        public SourceFolder(string name, SourceFolder folder)
        {
            m_Folders = new List<SourceFolder>();
            m_Assets = new List<SourceAsset>();

            Name = name;
            Folder = folder;
        }

        public string Name
        {
            get;
            private set;
        }

        public SourceFolder Folder
        {
            get;
            private set;
        }

        public string FromRootPath
        {
            get
            {
                return Folder == null ? string.Empty : (Folder.Folder == null ? Name : Utility.Text.Format("{0}/{1}", Folder.FromRootPath, Name));
            }
        }

        public int Depth
        {
            get
            {
                return Folder != null ? Folder.Depth + 1 : 0;
            }
        }

        public static Texture Icon
        {
            get
            {
                if (s_CachedIcon == null)
                {
                    s_CachedIcon = AssetDatabase.GetCachedIcon("Assets");
                }

                return s_CachedIcon;
            }
        }

        public void Clear()
        {
            m_Folders.Clear();
            m_Assets.Clear();
        }

        public SourceFolder[] GetFolders()
        {
            return m_Folders.ToArray();
        }

        public SourceFolder GetFolder(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new GameFrameworkException("Source folder name is invalid.");
            }

            foreach (SourceFolder folder in m_Folders)
            {
                if (folder.Name == name)
                {
                    return folder;
                }
            }

            return null;
        }

        public SourceFolder AddFolder(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new GameFrameworkException("Source folder name is invalid.");
            }

            SourceFolder folder = GetFolder(name);
            if (folder != null)
            {
                throw new GameFrameworkException("Source folder is already exist.");
            }

            folder = new SourceFolder(name, this);
            m_Folders.Add(folder);

            return folder;
        }

        public SourceAsset[] GetAssets()
        {
            return m_Assets.ToArray();
        }

        public SourceAsset GetAsset(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new GameFrameworkException("Source asset name is invalid.");
            }

            foreach (SourceAsset asset in m_Assets)
            {
                if (asset.Name == name)
                {
                    return asset;
                }
            }

            return null;
        }

        public SourceAsset AddAsset(string guid, string path, string name)
        {
            if (string.IsNullOrEmpty(guid))
            {
                throw new GameFrameworkException("Source asset guid is invalid.");
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new GameFrameworkException("Source asset path is invalid.");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new GameFrameworkException("Source asset name is invalid.");
            }

            SourceAsset asset = GetAsset(name);
            if (asset != null)
            {
                throw new GameFrameworkException(Utility.Text.Format("Source asset '{0}' is already exist.", name));
            }

            asset = new SourceAsset(guid, path, name, this);
            m_Assets.Add(asset);

            return asset;
        }


        public SourceAsset GetAssetByPath(string path)
        {
            string origin = path;
            if (path.StartsWith(Name))
            {
                path = path.Remove(0, Name.Length + 1);
                var nextTag = path.IndexOf("/");
                if (nextTag >= 0)
                {
                    var nextFolder = GetFolder(path.Substring(0, nextTag));
                    if (nextFolder == null)
                    {
                        System.Type assetType = null;
                        try
                        {
                            var obj = AssetDatabase.LoadAssetAtPath<Object>(origin);
                            if (obj)
                                assetType = obj.GetType();
                        }
                        catch { }
                        throw new System.Exception($"missing folder [{Name}/{path.Substring(0, nextTag)}], asset: [{origin}] asset type: [{assetType}], [SourceAssetUnionTypeFilter] try add type '{(assetType == null ? "" : "t:" + assetType.Name)}'");
                    }
                    return nextFolder.GetAssetByPath(path);
                }
                else
                {
                    return GetAsset(path);
                }
            }
            else
            {
                throw new GameFrameworkException("Source asset name is invalid.");
            }
        }
    }
}
