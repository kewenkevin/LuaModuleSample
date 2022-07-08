// 
// Copyright 2020 Yoozoo Net Inc.
// 
// UMT Framework and corresponding source code is free
// software: you can redistribute it and/or modify it under the terms of
// the GNU General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// UMT Framework and corresponding source code is distributed
// in the hope that it will be useful, but with permitted additional restrictions
// under Section 7 of the GPL. See the GNU General Public License in LICENSE.TXT
// distributed with this program. You should have received a copy of the
// GNU General Public License along with permitted additional restrictions
// with this program. If not, see https://gitlab.uuzu.com/yoozooopensource/client/framework/core
// 
// ***********************************************************************************************
// ***                  C O N F I D E N T I A L  ---  U M T   T E A M                          ***
// ***********************************************************************************************
// 
//     Project Name        :        UMT Framework Core Library
// 
//     File Name           :        AssetPackListProvider.cs
// 
//     Programmer          :        Wei Wei (Battle Mage Gandalf)
// 
//     Start Date          :        04/14/2021
// 
//     Last Update         :        04/14/2021 12:24 [Wei]
// 
//     Description         :        write here
// 
// =============================================================================================
// Contributors:
// ---------------------------------------------------------------------------------------------
// Battle Mage Gandalf                 wwei@yoozoo.com             Product technology Center
// =============================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using ND.Managers.ResourceMgr.Framework;
using UnityEditor;
using UnityEngine;

namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{
    public class AssetPackListCollection
    {
        public event GameFrameworkAction<int, int> OnLoadingAsset = null;
        public event GameFrameworkAction OnLoadCompleted = null;



        private readonly List<IAssetPackListDataProvider> m_dataProvider;
        // private readonly string m_ConfigurationPath;

        private SortedDictionary<string,SortedDictionary<string, AssetPackedConfig>>
            m_Assets = new SortedDictionary<string,SortedDictionary<string, AssetPackedConfig>>();

        private static readonly string[] AssemblyNames = {"Assembly-CSharp","Assembly-CSharp-Editor"};
        
        public AssetPackListCollection()
        {

            var types = Type.GetTypes(typeof(IAssetPackListDataProvider), AssemblyNames);

            m_dataProvider = new List<IAssetPackListDataProvider>();
            
            if (types.Length > 0)
            {
                for (int i = 0; i < types.Length; i++)
                {
                    m_dataProvider.Add(Activator.CreateInstance(types[i]) as IAssetPackListDataProvider);
                }
            }
            m_dataProvider.Add(new DefaultAssetPackListDataProvider());
        }

        public void Clear()
        {
            m_Assets.Clear();
        }

        public bool Load()
        {
            Clear();
            if (m_dataProvider == null)
                return false;
            for (int i = 0; i < m_dataProvider.Count; i++)
            {
                var providerTag = m_dataProvider[i].Tag;
                if (!m_Assets.ContainsKey(providerTag))
                {
                    m_Assets.Add(providerTag, new SortedDictionary<string, AssetPackedConfig>());
                }
                var dic = m_Assets[providerTag];
                m_dataProvider[i].LoadConfig(ref dic);
            }

            return true;
        }

        /// <summary>
        /// 保存分析结果
        /// </summary>
        /// <returns></returns>
        public bool Save(string tag = null)
        {
            if (m_dataProvider == null)
                return false;
            for (int i = 0; i < m_dataProvider.Count; i++)
            {
                var providerTag = m_dataProvider[i].Tag;

                if (tag == null || tag == providerTag)
                {
                    if (!m_Assets.ContainsKey(providerTag))
                    {
                        m_Assets.Add(providerTag, new SortedDictionary<string, AssetPackedConfig>());
                    }

                    var dic = m_Assets[providerTag];
                    if (!m_dataProvider[i].SaveConfig(ref dic))
                    {
                        return false;
                    }

                    m_Assets[providerTag] = dic;
                }
            }

            return true;
        }

        public AssetPackedConfig GetAsset(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                return null;
            }

            AssetPackedConfig asset = null;

            var arr = m_Assets.Values.ToList();

            for (int j = 0; j < arr.Count; j++)
            {
                if (arr[j].TryGetValue(assetName, out asset))
                {
                    return asset;
                }
            }

            return null;
        }


        /// <summary>
        /// 添加资源
        /// </summary>
        public bool AssignAsset(string name,string providerTag = "default")
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogWarning("AssignAsset false, assetPath null, " + name);
                return false;
            }


            string guid = AssetDatabase.AssetPathToGUID(name);
            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogWarning("AssignAsset false, guid null, " + guid);
                return false;
            }

            AssetPackedConfig asset = GetAsset(name);

            if (asset == null)
            {
                asset = AssetPackedConfig.Create(name);
                
                if (!m_Assets.ContainsKey(providerTag))
                {
                    m_Assets.Add(providerTag, new SortedDictionary<string, AssetPackedConfig>());
                }
                var dic = m_Assets[providerTag];
                dic.Add(asset.assetPath, asset);
            }

            return true;
        }
        
        
        /// <summary>
        /// 获取所有资源
        /// </summary>
        public AssetPackedConfig[] GetAssets()
        {
            var list = new List<AssetPackedConfig>();
            var arr = m_Assets.Values.ToList();

            for (int j = 0; j < arr.Count; j++)
            {
                list.AddRange(arr[j].Values);
            }
            return list.ToArray();
        }

        public bool HasAsset(string assetPath,string providerTag = "default")
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return false;
            }
            if (m_Assets.ContainsKey(providerTag))
                return m_Assets[providerTag].ContainsKey(assetPath);
            else
            {
                return false;
            }
        }


        [MenuItem(EditorUtilityx.MenuPrefix+"/Generate Default AssetPackList File")]
        private static void Generate()
        {
            AssetPackListCollection collection = new AssetPackListCollection();
            collection.Save("default");
            AssetDatabase.Refresh();
        }
    }
}