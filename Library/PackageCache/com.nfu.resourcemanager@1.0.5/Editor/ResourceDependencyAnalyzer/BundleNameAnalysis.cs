using System.Collections.Generic;
using ND.Managers.ResourceMgr.Editor.ResourceTools;
using UnityEditor;

namespace ND.Managers.ResourceMgr.Editor.ResourceDependencyAnalyzer
{
    public class BundleNameAnalysis
    {
        private AssetLink allArchive;
        private Dictionary<string, List<AssetNode>> allSameABNameAssets = new Dictionary<string, List<AssetNode>>();
        private IAssetBundleAnalysisHandler m_AssetBundleAnalysisHandler;

        public BundleNameAnalysis(AssetLink allAsset, IAssetBundleAnalysisHandler assetBundleAnalysisHandler)
        {
            this.m_AssetBundleAnalysisHandler = assetBundleAnalysisHandler;
            allArchive = allAsset;
        }
        #region 路径 文件类型相关
        
        #endregion
        #region 资源全量式分析入口 会重新计算bundleName划分
        /// <summary>
        /// 对资源景行bundleName分析规划
        /// TODO:多依赖同时多依赖来自相同包
        /// </summary>
        public void AnalysisAssetBundleName()
        {
            AssetNode node;
            AssetNode parentNode;
            HashSet<int> excludeIndexSet = new HashSet<int>();
            string processTip;
            string bundleName;
            string variant;
            string assetName;
            for (int i = 0; i < allArchive.Count; i++)
            {
                node = allArchive[i];
                processTip = string.Format("progess:({0}/{1}) {2}", i, allArchive.Count, node.AssetPath);
                EditorUtilityx.DisplayProgressBar("分析BundleName" + node.AssetPath, processTip, i / (float)allArchive.Count);

                //优先使用配置获取
                if (EditorResourceSettings.GetAddressable(node.AssetPath, out bundleName, out variant, out assetName))
                {
                    node.BundleName = bundleName;
                }
                else if (TryGetDistinctiveABNameFromAssetPath(node.AssetPath, out bundleName))
                {
                    node.BundleName = bundleName;
                }
                ///基于当前不发生嵌套
                else if ((node.UsedIndexs.Count == 1) && !node.IsSource)
                {
                    parentNode = allArchive[node.UsedIndexs[0]];

                    if (string.IsNullOrEmpty(parentNode.BundleName))
                    {
                        excludeIndexSet.Add(i);
                    }
                    else
                    {
                        node.BundleName = parentNode.BundleName;
                    }
                }
                else
                {
                    node.BundleName = bundleName;
                }
            }
            int index = 0;
            foreach (var item in excludeIndexSet)
            {
                index++;
                node = allArchive[item];
                node.BundleName = GetSingleAssetBundleName(item);
                processTip = string.Format("progess:({0}/{1}) {2}", index, excludeIndexSet.Count, node.AssetPath);
                EditorUtilityx.DisplayProgressBar("合并单一资源BundleName" + node.AssetPath, processTip, index / (float)excludeIndexSet.Count);
            }
            EditorUtility.ClearProgressBar();
            PushAssetBundleArchive();
        }
        #endregion
        #region bundleName划分记录公共接口
        /// <summary>
        /// 向上追溯当前资源的BundleName
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetSingleAssetBundleName(int index)
        {
            AssetNode curNode = allArchive[index];
            AssetNode parentNode = allArchive[curNode.UsedIndexs[0]];
            if (parentNode == null)
            {
                throw new System.Exception("当前资源不存在父资源，资源：" + curNode.AssetPath + "BundleName:" + curNode.BundleName);
            }
            else if (string.IsNullOrEmpty(parentNode.BundleName))
            {
                return GetSingleAssetBundleName(parentNode.Index);
            }
            else if (parentNode.IsRecord)
            {
                string bundleName;
                TryGetDistinctiveABNameFromAssetPath(curNode.AssetPath, out bundleName);
                return bundleName;
            }
            return parentNode.BundleName;
        }
        /// <summary>
        /// 记录划分好的路径bundleName
        /// </summary>
        private void PushAssetBundleArchive()
        {
            allSameABNameAssets.Clear();
            AssetNode curNode;
            List<AssetNode> sameABNameAssets;
            for (int i = 0; i < allArchive.Count; i++)
            {
                curNode = allArchive[i];
                if (string.IsNullOrEmpty(curNode.BundleName))
                {
                    continue;
                }

                if (allSameABNameAssets.TryGetValue(curNode.BundleName, out sameABNameAssets))
                {
                    sameABNameAssets.Add(curNode);
                }
                else
                {
                    sameABNameAssets = new List<AssetNode>();
                    sameABNameAssets.Add(curNode);
                    allSameABNameAssets.Add(curNode.BundleName, sameABNameAssets);
                }
            }
        }
        #endregion
        #region bundleName划分逻辑代码
        public bool TryGetDistinctiveABNameFromAssetPath(string assetPath, out string bundleName)
        {
            if (EditorResourceSettings.IsAssetRootPath(assetPath))
            {
                return m_AssetBundleAnalysisHandler.TryGetDistinctiveABNameFromRootAssetPath(assetPath, out bundleName);
            }
            else
            {
                return m_AssetBundleAnalysisHandler.TryGetDistinctiveABNameFromOtherAssetPath(assetPath, out bundleName);
            }
        }
        #endregion
        #region 全量资源BundleName处理
        /// <summary>
        /// 将分析后的bundleName存入Unity和Archive中
        /// </summary>
        public AssetBundleBuild[] CreateAllBuildParameter()
        {
            return CreateAllBuildParameterArray();
        }

        #endregion
        #region  资源bundleName 处理公共接口

        /// <summary>
        /// 合成构建参数
        /// </summary>
        /// <param name="isClear">是否清楚原有bundleName</param>
        private AssetBundleBuild[] CreateAllBuildParameterArray()
        {
            AssetBundleBuild[] allAssetBundleBuild;
            allAssetBundleBuild = new AssetBundleBuild[allSameABNameAssets.Count];

            List<AssetNode> sameABNameAssets;
            int index = 0;
            var processTip = string.Empty;
            ///TODO:收集老资源共同进行打包
            foreach (var curSameNameKV in allSameABNameAssets)
            {
                var curProcess = (float)(index) / allSameABNameAssets.Count;
                processTip = string.Format("【{0}/{1}】", index, allSameABNameAssets.Count);
                EditorUtilityx.DisplayProgressBar("生成AssetBundleBuild", processTip + "正在写入BundleName：" + curSameNameKV.Key, curProcess);
                allAssetBundleBuild[index].assetBundleName = curSameNameKV.Key;
                sameABNameAssets = curSameNameKV.Value;
                
                allAssetBundleBuild[index].assetNames = new string[sameABNameAssets.Count];
                for (int i = 0; i < sameABNameAssets.Count; i++)
                {
                    allAssetBundleBuild[index].assetNames[i] = sameABNameAssets[i].AssetPath;
                }
                index++;
            }
            EditorUtility.ClearProgressBar();
            //SaveAllLog();
            return allAssetBundleBuild;
        }
        #endregion
    }
}