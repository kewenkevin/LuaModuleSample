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
//     File Name           :        DefaultAssetPackListDataProvider.cs
// 
//     Programmer          :        Wei Wei (Battle Mage Gandalf)
// 
//     Start Date          :        04/14/2021
// 
//     Last Update         :        04/14/2021 13:38 [Wei]
// 
//     Description         :        write here
// 
// =============================================================================================
// Contributors:
// ---------------------------------------------------------------------------------------------
// Battle Mage Gandalf                 wwei@yoozoo.com             Product technology Center
// =============================================================================================

using System.Collections.Generic;
using System.IO;
using System.Xml;
using ND.Managers.ResourceMgr.Framework;
using UnityEditor;
using UnityEngine;

namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{
    public class DefaultAssetPackListDataProvider : IAssetPackListDataProvider
    {
        public string Tag { get; } = "default";
        public event GameFrameworkAction<int, int> OnLoadingAsset = null;

        public event GameFrameworkAction OnLoadCompleted = null;

        private string configPath;

        public DefaultAssetPackListDataProvider()
        {
            configPath = Type.GetConfigurationPath<PackedAssetListPathAttribute>() ??
                         Utility.Path.GetRegularPath(Path.GetFullPath(
                             "Packages/com.nfu.resourcemanager/Configs/PackedAssetCollection.xml"));
        }

        public bool LoadConfig(ref SortedDictionary<string, AssetPackedConfig> data)
        {

            if (!File.Exists(configPath))
            {
                return false;
            }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(configPath);
                XmlNode xmlRoot = xmlDocument.SelectSingleNode("UnityGameFramework");
                XmlNode xmlCollection = xmlRoot.SelectSingleNode("PackedAssetList");
                XmlNode xmlAssets = xmlCollection.SelectSingleNode("Assets");

                XmlNodeList xmlNodeList = null;
                XmlNode xmlNode = null;
                int count = 0;

                xmlNodeList = xmlAssets.ChildNodes;
                count = xmlNodeList.Count;
                for (int i = 0; i < count; i++)
                {
                    if (OnLoadingAsset != null)
                    {
                        OnLoadingAsset(i, count);
                    }

                    xmlNode = xmlNodeList.Item(i);
                    if (xmlNode.Name != "Asset")
                    {
                        continue;
                    }

                    string assetPath = xmlNode.Attributes.GetNamedItem("Name").Value;
                    if (!AssignAsset(ref data,assetPath))
                    {
                        Debug.LogWarning(Utility.Text.Format("Can not assign asset '{0}'", assetPath));
                        continue;
                    }
                }

                if (OnLoadCompleted != null)
                {
                    OnLoadCompleted();
                }

                return true;
            }
            catch
            {
                File.Delete(configPath);
                if (OnLoadCompleted != null)
                {
                    OnLoadCompleted();
                }

                return false;
            }


        }

        public bool SaveConfig(ref SortedDictionary<string, AssetPackedConfig> data)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));

                XmlElement xmlRoot = xmlDocument.CreateElement("UnityGameFramework");
                xmlDocument.AppendChild(xmlRoot);

                XmlElement xmlCollection = xmlDocument.CreateElement("PackedAssetList");
                xmlRoot.AppendChild(xmlCollection);

                XmlElement xmlAssets = xmlDocument.CreateElement("Assets");
                xmlCollection.AppendChild(xmlAssets);

                XmlElement xmlElement = null;
                XmlAttribute xmlAttribute = null;


                foreach (AssetPackedConfig asset in data.Values)
                {
                    xmlElement = xmlDocument.CreateElement("Asset");
                    xmlAttribute = xmlDocument.CreateAttribute("Name");
                    xmlAttribute.Value = asset.assetPath;
                    xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    xmlAssets.AppendChild(xmlElement);
                }

                string configurationDirectoryName = Path.GetDirectoryName(configPath);
                if (!Directory.Exists(configurationDirectoryName))
                {
                    Directory.CreateDirectory(configurationDirectoryName);
                }

                xmlDocument.Save(configPath);
                AssetDatabase.Refresh();
                return true;
            }
            catch
            {
                if (File.Exists(configPath))
                {
                    File.Delete(configPath);
                }

                return false;
            }
        }
        
        
        public bool AssignAsset(ref SortedDictionary<string, AssetPackedConfig> data,string name)
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

            AssetPackedConfig asset = GetAsset(ref data,name);

            if (asset == null)
            {
                asset = AssetPackedConfig.Create(name);
                data.Add(asset.assetPath, asset);
            }

            return true;
        }
        
        public AssetPackedConfig GetAsset(ref SortedDictionary<string, AssetPackedConfig> data,string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                return null;
            }

            AssetPackedConfig asset = null;
            if (data.TryGetValue(assetName, out asset))
            {
                return asset;
            }

            return null;
        }
    }
}