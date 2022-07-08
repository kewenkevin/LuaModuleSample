using System.Collections.Generic;
using UnityEditor;

namespace ND.Managers.ResourceMgr.Editor.ResourceDependencyAnalyzer
{
    public class AssetBundleArchive
    {
        #region 依赖记录 与外部获取
        private AssetLink allAsset = new AssetLink();
        public AssetLink GetAssetArchive()
        {
            return allAsset;
        }
        public void PushAssetRef(string originAssetPath, string refAssetPath, bool isSource = false)
        {
            if (allAsset == null)
            {
                allAsset = new AssetLink();
            }
            var selfNode = allAsset.GetNode(originAssetPath);
            if (isSource)
            {
                selfNode.IsSource = isSource;
            }
            if (!string.IsNullOrEmpty(refAssetPath) && !originAssetPath.Equals(refAssetPath))
            {
                var refNode = allAsset.GetNode(refAssetPath);

                selfNode.AddRef(refNode.Index);
                refNode.AddUsed(selfNode.Index);
            }
        }
        #endregion

        #region 对现有依赖进行初步分析后处理（包括：记录进行依赖合并（即断开父节点与依赖子节点之间的联系，创建原始引用列表））
        /// <summary>
        /// 创建原始引用列表
        /// </summary>
        public void CreateSourceList()
        {
            for (int i = 0; i < allAsset.Count; i++)
            {
                allAsset[i].CreateSourceList(allAsset);
            }
        }

        /// <summary>
        /// 合并子依赖项
        /// 
        /// 尚不清楚循环依赖会有何种表现（应该不存在这种事把）
        /// </summary>
        public void MergeAssetRef()
        {
            AssetNode node;
            HashSet<int> excludeIndexSet = new HashSet<int>();
            List<int> excludeIndexList;
            string processTip;
            for (int i = 0; i < allAsset.Count; i++)
            {
                node = allAsset[i];
                excludeIndexSet.Clear();
                //遍历引用列表分析需要排除的项
                foreach (var refIndex in node.RefIndexs)
                {
                    if (refIndex == i)
                    {
                        continue;
                    }
                    processTip = string.Format("Deps:({0}/{1}) {2}", i, allAsset.Count, node.AssetPath);
                    EditorUtilityx.DisplayProgressBar("合并依赖项" + node.AssetPath, processTip, i / (float)allAsset.Count);
                    excludeIndexList = RemoveSurplusUsedRef(refIndex, i);
                    //防止提前排除导致部分引用没被分析
                    //利用set排除重复项
                    foreach (var excludeIndex in excludeIndexList)
                    {
                        excludeIndexSet.Add(excludeIndex);
                    }
                }
                foreach (var excludeIndex in excludeIndexSet)
                {
                    node.RefIndexs.Remove(excludeIndex);
                }
            }
            EditorUtility.ClearProgressBar();

        }
        /// <summary>
        /// 将当前节点的依赖子节点排除出父节点
        /// 以断开父节点与依赖子节点之间的联系
        /// </summary>
        /// <param name="curIndex">当前项索引</param>
        /// <param name="excludeIndex">不进行断开联系的父节点索引 防止父节点循环崩坏 </param>
        /// <returns></returns>
        private List<int> RemoveSurplusUsedRef(int curIndex, int excludeIndex = -1)
        {
            AssetNode curNode = allAsset[curIndex];
            AssetNode sonRefNode;
            AssetNode parentNode;
            List<int> curNodeRefList = curNode.RefIndexs;
            List<int> curNodeUsedList = curNode.UsedIndexs;
            int curNodeUsedIndex;
            foreach (var sonRefIndex in curNodeRefList)
            {
                if (sonRefIndex == curIndex)
                {
                    continue;
                }
                sonRefNode = allAsset[sonRefIndex];
                for (int i = curNodeUsedList.Count - 1; i >= 0; i--)
                {
                    curNodeUsedIndex = curNodeUsedList[i];
                    parentNode = allAsset[curNodeUsedIndex];
                    if (parentNode.IsSource && curNode.IsSource && sonRefNode.SourceIndexs.Count > 1)
                    {
                        sonRefNode.SourceIndexs.Remove(parentNode.Index);
                    }
                    if (curNodeUsedIndex == excludeIndex)
                    {
                        sonRefNode.UsedIndexs.Remove(parentNode.Index);
                        continue;
                    }
                    //断开父节点与依赖子节点间的联系
                    if (parentNode.RefIndexs.Remove(sonRefIndex))
                    {
                        sonRefNode.UsedIndexs.Remove(parentNode.Index);
                    }
                }
            }
            return curNodeRefList;
        }
        #endregion

    }

    #region 记录单一资源的所有依赖关系
    public class AssetNode
    {
        /// <summary>
        /// 是否已存于记录中
        /// </summary>
        public bool IsRecord = false;
        public bool IsSource = false;
        /// <summary>
        /// 自身索引
        /// </summary>
        private int index;
        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
            }
        }
        /// <summary>
        /// 使用这个节点的索引
        /// </summary>
        private List<int> usedIndexs = new List<int>();
        public List<int> UsedIndexs
        {
            get
            {
                return usedIndexs;
            }
        }
        /// <summary>
        /// 这个节点引用的其他节点索引
        /// </summary>
        private List<int> refIndexs = new List<int>();
        public List<int> RefIndexs
        {
            get
            {
                return refIndexs;
            }
        }
        private List<int> sourceIndexs = new List<int>();
        public List<int> SourceIndexs
        {
            get
            {
                return sourceIndexs;
            }
        }

        /// <summary>
        /// 资产路径
        /// </summary>
        private string assetPath;
        public string AssetPath
        {
            get
            {
                return assetPath;
            }
        }

        /// <summary>
        /// 资产路径
        /// </summary>
        private string bundleName;
        public string BundleName
        {
            get
            {
                return bundleName;
            }
            set
            {
                bundleName = value;
            }
        }
        public bool IsSelf(string path)
        {
            return assetPath == path;
        }

        public void SetAsset(string path)
        {
            assetPath = path;
        }

        public void AddUsed(int idx)
        {
            if (usedIndexs.Contains(idx))
            {
                return;
            }
            usedIndexs.Add(idx);
        }

        public void AddRef(int idx)
        {
            if (refIndexs.Contains(idx))
            {
                return;
            }
            refIndexs.Add(idx);
        }
        /// <summary>
        /// 在还未断开爷孙依赖关系之前执行，收集该资源引用的源头资源方便合并bundleName
        /// </summary>
        /// <param name="allAsset"></param>
        public void CreateSourceList(AssetLink allAsset)
        {
            sourceIndexs.Clear();
            foreach (var item in usedIndexs)
            {
                var assetNode = allAsset[item];
                if (assetNode.IsSource)
                {
                    sourceIndexs.Add(item);
                }
            }
        }
    }
    #endregion
    #region 记录所有依赖
    public class AssetLink
    {
        private List<AssetNode> link = new List<AssetNode>();
        //assetPath 字典优化性能
        private Dictionary<string, AssetNode> nodes = new Dictionary<string, AssetNode>();

        public int Count
        {
            get
            {
                return link.Count;
            }
        }

        public AssetNode this[int index]
        {
            get
            {
                return link[index];
            }
        }

        public AssetNode GetNode(string assetPath)
        {
            AssetNode targetNode = null;
       
            nodes.TryGetValue(assetPath, out targetNode);            

            if (targetNode == null)
            {
                targetNode = new AssetNode();
                targetNode.Index = link.Count;
                targetNode.SetAsset(assetPath);
                link.Add(targetNode);
                nodes[assetPath] = targetNode;
            }
            return targetNode;
        }

        public void Reset()
        {
            link.Clear();
        }
    }
    #endregion
}