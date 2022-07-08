
using System.IO;
using UnityEngine;
using ND.Managers.ResourceMgr.Editor.ResourceDependencyAnalyzer;
using ND.Managers.ResourceMgr.Editor.ResourceTools;
using ND.Managers.ResourceMgr.Framework;

namespace ND.Gameplay.Managers.ResourceManagerV2.Editor
{
    public class CustomAssetBundleAnalysisHandler : IAssetBundleAnalysisHandler
    {
        private const string LuaPath = "Assets/Lua";
        
        private const string ASSET_ENTER_FOLDER_NAME = "Assets/Sample/ResourceManager/ResourcesAssets/";
        private const string ASSET_GUI_FOLDER_NAME = "UI";
        private static string ASSET_GUI_VARIANT_FOLDER_NAME = "Assets/ResourceVariant/En/";
        
        private string[] LuaSourcePath = new[]
        {
            Utility.Path.GetRegularPath(Path.GetFullPath(
                "Packages/com.nfu.resourcemanager/Configs/BuildSettings.xml")),
            "",
        };
        
        
        public override void OnPreprocessResource(Resource resource)
        {
            resource.Packed = true;
            string path = resource.FullName;
            if (path.StartsWith(LuaPath))
            {
                var directies = path.Split('/');
                if (directies.Length>2)
                    resource.FileSystem =directies[1]+"_"+directies[2];
                else if (directies.Length>1)
                    resource.FileSystem =directies[1];
            }
        }


        public override void OnPreprocessAnalysisBegin()
        {
            //AtlasBuild.GenerateBuild();
         //   LuaBuild.GenerateBuildLua();
        }

        public override void OnPreprocessAnalysis(AddResourceFunction addResourceFunction,
            AssignAssetFunction assignAssetFunction,GetResourceFunction getResourceFunction,GetAssetByPathFunction getAssetByPathFunction)
        {

            var files = Directory.GetFiles(LuaPath, "*.bytes", System.IO.SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                file = file.Replace('\\', '/');
                
                var rootPath = Path.GetDirectoryName(file);
                rootPath = rootPath.Replace('\\', '/');

                var fileSystemName = LuaPath + "/" + rootPath.Substring(7).Replace('/', '_');
                var resourceName = fileSystemName +"_"+Path.GetFileNameWithoutExtension(file);
                Resource resource = getResourceFunction(resourceName,null);
                if (resource == null)
                {
                    Debug.Log($"Add Lua Resource {resourceName}");
                    addResourceFunction(resourceName, null, false);
                    resource = getResourceFunction(resourceName, null);
                    resource.Packed = true;
                }

                if (resource != null)
                {
                    resource.FileSystem = fileSystemName;
                    resource.LoadType = LoadType.LoadFromBinary;
                    SourceAsset asset = getAssetByPathFunction(file);
                    assignAssetFunction(asset, resource);
                }
            }
            
        }
        
        public override bool TryGetDistinctiveABNameFromOtherAssetPath(string assetPath, out string abName)
        {
            if (assetPath.StartsWith(ASSET_GUI_VARIANT_FOLDER_NAME))
            {
                if (assetPath.Contains(ASSET_ENTER_FOLDER_NAME + ASSET_GUI_FOLDER_NAME+"/Common"))
                {
                    abName = ASSET_ENTER_FOLDER_NAME + ASSET_GUI_FOLDER_NAME +
                             "/Common";
                }
                else if(assetPath.Contains(ASSET_ENTER_FOLDER_NAME + ASSET_GUI_FOLDER_NAME+"/GUI"))
                {
                    var lasIndexOf = assetPath.LastIndexOf("/");
                    if (assetPath.Substring(lasIndexOf-3,3)!="GUI")
                    {
                        abName = Path.GetDirectoryName(assetPath).Replace('\\','/')+".spriteatlas";
                    }
                    else
                    {
                        abName = assetPath;
                    }
                }
                else
                {
                    abName = assetPath;
                }
            }
            else
            {
                abName = assetPath;
            }
            return false;
        }

        public override bool TryGetDistinctiveABNameFromRootAssetPath(string assetPath, out string abName)
        {
            if (assetPath.Contains(ASSET_ENTER_FOLDER_NAME + ASSET_GUI_FOLDER_NAME))
            {
                if (assetPath.Contains(ASSET_ENTER_FOLDER_NAME + ASSET_GUI_FOLDER_NAME + "/Common"))
                {
                    abName = ASSET_ENTER_FOLDER_NAME + ASSET_GUI_FOLDER_NAME +
                             "/Common";
                }
                else if (assetPath.Contains(ASSET_ENTER_FOLDER_NAME + ASSET_GUI_FOLDER_NAME + "/GUI"))
                {
                    if (assetPath.EndsWith(".prefab"))
                    {
                        abName = assetPath;
                        return true;
                    }
                    var GUIPath = ASSET_ENTER_FOLDER_NAME + ASSET_GUI_FOLDER_NAME + "/GUI";

                    var index = assetPath.IndexOf(GUIPath);

                    var path = assetPath.Substring(index + GUIPath.Length + 1);


                    var indexDir = path.IndexOf("/");

                    if (indexDir < 0)
                    {
                        abName = assetPath;
                    }
                    else if (path.StartsWith("icon/"))
                    {
                        path = path.Replace('\\', '/');
                        indexDir = path.LastIndexOf("/");
                        abName = (GUIPath + "/" + path.Substring(0, indexDir).Replace('/', '_')) + ".spriteatlas";
                    }
                    else
                    {
                        abName = (GUIPath + "/" + path.Substring(0, indexDir)).Replace('\\', '/') + ".spriteatlas";
                    }
                }
                else
                {
                    abName = assetPath;
                }
            }
            else
            {
                abName = assetPath;
            }
            return true;
        }
    }
}