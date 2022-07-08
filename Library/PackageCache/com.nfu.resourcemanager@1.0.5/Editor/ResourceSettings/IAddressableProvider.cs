namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{
    /// <summary>
    /// 资源寻址提供程序
    /// </summary>
    public interface IAddressableProvider
    {

        /// <summary>
        /// 路径是否可用
        /// </summary>
        bool IsAssetPathMatch(string assetPath);

        /// <summary>
        /// AssetPath 获取资源的包名，变体名，资源名
        /// </summary>
        bool GetAddressableName(string assetPath, out string bundleName, out string variant, out string assetName);


        void OnPreprocessAnalysis(string assetPath, ResourceEditorImplate builder);



    }

}