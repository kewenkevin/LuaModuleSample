using ND.Managers.ResourceMgr.Editor.ResourceTools;

namespace ND.Managers.ResourceMgr.Editor.ResourceDependencyAnalyzer
{
    /// <summary>
    /// 资源分析器，计算资源对应的资源包名称
    /// </summary>
    public abstract class IAssetBundleAnalysisHandler
    {
        public delegate Resource GetResourceFunction(string name, string variant);
        public delegate void AddResourceFunction(string name, string variant, bool refresh);
        public delegate void AssignAssetFunction(SourceAsset sourceAsset, Resource resource);
        public delegate SourceAsset GetAssetByPathFunction(string path);
        
        /// <summary>
        /// 分析开始
        /// </summary>
        public abstract void OnPreprocessAnalysisBegin();
        public abstract void OnPreprocessResource(Resource resource);
        /// <summary>
        /// 处理分析
        /// </summary>
        public abstract void OnPreprocessAnalysis(AddResourceFunction addResourceFunction,AssignAssetFunction assignAssetFunction,GetResourceFunction getResourceFunction,GetAssetByPathFunction getAssetByPathFunction);

        /// <summary>
        /// 打包目标目录下资源ab名方式
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="abName"></param>
        /// <returns></returns>
        public abstract bool TryGetDistinctiveABNameFromRootAssetPath(string assetPath, out string abName);

        
        /// <summary>
        /// 非打包目录下资源ab名方式
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="abName"></param>
        /// <returns></returns>
        public abstract bool TryGetDistinctiveABNameFromOtherAssetPath(string assetPath, out string abName);
    }
}
