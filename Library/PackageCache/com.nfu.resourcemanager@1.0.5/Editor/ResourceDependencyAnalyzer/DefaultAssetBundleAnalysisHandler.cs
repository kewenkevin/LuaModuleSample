using ND.Managers.ResourceMgr.Editor.ResourceTools;

namespace ND.Managers.ResourceMgr.Editor.ResourceDependencyAnalyzer
{
    public class DefaultAssetBundleAnalysisHandler : IAssetBundleAnalysisHandler
    {
        public override void OnPreprocessResource(Resource resource)
        {
            resource.Packed =EditorResourceSettings.IsPackedResource(resource.FullName);
            //string path = resource.FullName;
            //if (path.StartsWith("Assets/"))
            //    resource.FileSystem = path.Split('/')[1];            

            //resource.AddResourceGroup("Default"); 
        }

        public override void OnPreprocessAnalysisBegin()
        {
            
        }

        public override void OnPreprocessAnalysis(AddResourceFunction addResourceFunction,
            AssignAssetFunction assignAssetFunction,GetResourceFunction getResourceFunction,GetAssetByPathFunction getAssetByPathFunction)
        {
           
        }

        public override bool TryGetDistinctiveABNameFromOtherAssetPath(string assetPath, out string abName)
        {
            abName = assetPath;
            abName= EditorResourceSettings.FormatAssetBundleName(abName);
            return false;
        }
        public override bool TryGetDistinctiveABNameFromRootAssetPath(string assetPath, out string abName)
        {
            abName = assetPath;
            abName = EditorResourceSettings.FormatAssetBundleName(abName);
            return true;
        }

    }
}