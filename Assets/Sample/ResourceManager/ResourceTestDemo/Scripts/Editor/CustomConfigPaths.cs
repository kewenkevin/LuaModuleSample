using System.IO;
using ND.Managers.ResourceMgr.Editor.ResourceDependencyAnalyzer;
using ND.Managers.ResourceMgr.Editor.ResourceTools;
using ND.Managers.ResourceMgr.Framework;
using UnityEngine;

namespace ND.Gameplay.Managers.ResourceManagerV2.Editor
{
    public static class CustomConfigPaths
    {
        [ResourceBuilderConfigPath]
        public static string ResourceBuilderConfigPath = Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "GameSettings/NFUSettings/ResourceManager/ResourceCustomConfigs/ResourceBuilder.xml"));
        
        [ResourceCollectionConfigPath]
        public static string ResourceCollectionConfigPath =  Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "GameSettings/NFUSettings/ResourceManager/ResourceCustomConfigs/ResourceCollection.xml"));
        
        [ResourceEditorConfigPath]
        public static string ResourceEditorConfigPath =  Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "GameSettings/NFUSettings/ResourceManager/ResourceCustomConfigs/ResourceEditor.xml"));
        
        [AssetBundleAnalysisRootPath]
        public static string AssetBundleAnalysisRootPath =  "Assets/Sample/ResourceManager/ResourcesAssets/";

        [AssetVariantPath]
        public static string AssetVariantPath =  "Assets/Sample/ResourceManager/ResourceVariant/";
        
        [RawAssetPath]
        public static string RawAssetsPath =  "Assets/Sample/ResourceManager/RawAssets/";
    }
}