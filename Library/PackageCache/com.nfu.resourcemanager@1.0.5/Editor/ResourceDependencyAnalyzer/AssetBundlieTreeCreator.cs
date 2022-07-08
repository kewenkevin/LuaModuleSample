using System.Collections.Generic;
using System.IO;
using System.Linq;
using ND.Managers.ResourceMgr.Editor.ResourceTools;
using UnityEditor;
using UnityEngine;


namespace ND.Managers.ResourceMgr.Editor.ResourceDependencyAnalyzer
{
    public class AssetBundlieTreeCreator
    {

        private IAssetBundleAnalysisHandler m_AssetBundleAnalysisHandler;

        private string assetVariantFolderName => EditorResourceSettings.VariantAssetPath;

        /// 没有依赖的文件扩展名
        /// </summary>
        private static HashSet<string> m_NoneDependFileExtension = new HashSet<string>()
        {
            ".bytes",
            ".mp4",
            ".lua",
            ".fbx",
            ".fbx",
            ".anim",
            ".shader",
            ".png",
            ".psd",
            ".exr",
            ".tga",
            ".tga",
            ".spriteatlas",
        };

        public AssetBundlieTreeCreator(IAssetBundleAnalysisHandler assetBundleAnalysisHandler)
        {
            m_AssetBundleAnalysisHandler = assetBundleAnalysisHandler;
        }

        AssetBundleArchive assetBundleArchive = new AssetBundleArchive();
        #region 资源路径与进度条提示相关

        public AssetLink Archive => assetBundleArchive.GetAssetArchive();

        string processTitle = string.Empty;
        int processIndex = 0;
        int processIndexMax = 0;
        string processTip = string.Empty;
        private float process
        {
            get
            {
                if (processIndexMax.Equals(0) ||
                    processIndexMax < 0 ||
                    processIndex < 0)
                {
                    return 0;
                }
                return processIndex / processIndexMax;
            }
        }
        #endregion

        #region 依赖收集接口
        public void GetDependencies()
        {
            var performance = new PerformanceSample($"{GetType().Name}.{nameof(GetDependencies)}");

            HashSet<string> allAsset = new HashSet<string>();
            string[] allVariantAsset;

            foreach (var dir in EditorResourceSettings.GetAllAssetRootPaths())
            {
                foreach (var file in Directory.GetFiles(dir, "*.*", System.IO.SearchOption.AllDirectories))
                {
                    if (file.EndsWith(".meta"))
                        continue;
                    allAsset.Add(file);
                }
            }


            #region 手动Label标记资源
            
            var assets = AssetDatabase.FindAssets(EditorResourceSettings.GetAllAssetIncludeLabelsFilter());
            EditorResourceSettings.ResetCachedIncludeAssetPaths();
            for (int i = 0; i < assets.Length; i++)
            {
                var key = AssetDatabase.GUIDToAssetPath(assets[i]);
                if (!Directory.Exists(key))
                {
                    if (!allAsset.Contains(key))
                    {
                        allAsset.Add(key);
                        EditorResourceSettings.AddCachedIncludeAssetPaths(key);
                    }
                }
            }
            #endregion


            #region 读配置表定义的打包资源

            AssetPackListCollection collection = new AssetPackListCollection();
            if (collection.Load())
            {
                var assetPackedConfigs = collection.GetAssets();

                for (int i = 0; i < assetPackedConfigs.Length; i++)
                {
                    allAsset.Add(assetPackedConfigs[i].assetPath);
                    EditorResourceSettings.AddCachedIncludeAssetPaths(assetPackedConfigs[i].assetPath);
                }

                collection.Clear();
            }

            #endregion
            
            

            if (Directory.Exists(assetVariantFolderName))
            {
                allVariantAsset = Directory.GetFiles(assetVariantFolderName, "*.*", System.IO.SearchOption.AllDirectories);
            }
            else
            {
                allVariantAsset = new string[0];
            }

            FindSelectedPathDependentAssetToSet(allAsset.ToArray(), allVariantAsset);
            performance.Dispose();
        }

        #endregion

        #region 生成或更新依赖树接口

        public void UpdateAndAddTree(AssetDependenciesTreeBinary dependenciesTreeRoot)
        {
            AnalysisDependenciesTree(dependenciesTreeRoot);
        }
        #endregion

        #region BundleName相关接口
        BundleNameAnalysis bundleNameAnalysis;
        public void AnalysisBundlName(IAssetBundleAnalysisHandler assetBundleAnalysisHandler)
        {
            bundleNameAnalysis = new BundleNameAnalysis(assetBundleArchive.GetAssetArchive(), assetBundleAnalysisHandler);
            bundleNameAnalysis.AnalysisAssetBundleName();
        }
        public AssetBundleBuild[] CreateAllBuildParameter()
        {
            return bundleNameAnalysis.CreateAllBuildParameter();
        }
        #endregion

        #region 收集选中资源的所有依赖并记录
        HashSet<string> hasColloectDependiesSet;

        public HashSet<string> variantOriginalHashSet;

        public List<string> UseableResource { get; private set; }

        private void FindSelectedPathDependentAssetToSet(string[] selectedPath, string[] variantPath)
        {
            UseableResource = new List<string>();
            processTitle = "依赖关系分析中:";
            processIndex = 0;
            processIndexMax = selectedPath.Length;
            processTip = string.Empty;
            hasColloectDependiesSet = new HashSet<string>();
            variantOriginalHashSet = new HashSet<string>();

            cachedDepAssetPaths = new Dictionary<string, string[]>();

            string curPath;
            bool hasSelectedObject = false;            

            for (int i = 0; i < selectedPath.Length; i++)
            {
                curPath = selectedPath[i];

                var assetPathFormat = curPath.Replace('\\', '/');
                if (!EditorResourceSettings.IsAssetRootPath(assetPathFormat))
                {
                    selectedPath[i] = null;
                    continue;
                }

                // assetPathFormat = assetPathFormat.ToLower();
                if (!IsValidFileType(assetPathFormat))
                {
                    selectedPath[i] = null;
                    continue;
                }
                hasSelectedObject = true;
                selectedPath[i] = assetPathFormat;
                hasColloectDependiesSet.Add(assetPathFormat);
            }

            for (int i = 0; i < variantPath.Length; i++)
            {
                curPath = variantPath[i];
                var assetPathFormat = curPath.Replace('\\', '/');
                if (!IsValidFileType(assetPathFormat))
                {
                    variantPath[i] = null;
                    continue;
                }
                variantPath[i] = assetPathFormat;

                var origninalAssetName = assetPathFormat.Substring(assetVariantFolderName.Length);
                origninalAssetName = origninalAssetName.Substring(origninalAssetName.IndexOf("/") + 1);
                variantOriginalHashSet.Add(origninalAssetName);


                hasColloectDependiesSet.Add(assetPathFormat);
            }
            

            if (!hasSelectedObject)
            {
                Debug.LogError("本次打包不包含合法资源");
                throw new System.Exception();
            }

            UseableResource = hasColloectDependiesSet.ToList();
            var handleDependAssetsPerformance = new PerformanceSample($"{GetType().Name}.FindSelectedPathDependentAssetToSet.HandleDependAssets");

            foreach (var assetPathFormat in selectedPath)
            {
                processIndex++;
                if (string.IsNullOrEmpty(assetPathFormat))
                {
                    continue;
                }
                processTip = string.Format("({0}/{1}) {2}", processIndex, selectedPath.Length, assetPathFormat);
                //m_assetBundleBuildHandle.DisplayProgressBar(processTitle, processTip, processIndex / (float)processIndexMax);
                assetBundleArchive.PushAssetRef(assetPathFormat, null, true);
                HandleDependAssets(assetPathFormat);
            }

            foreach (var assetPathFormat in variantPath)
            {
                processIndex++;
                if (string.IsNullOrEmpty(assetPathFormat))
                {
                    continue;
                }
                processTip = string.Format("({0}/{1}) {2}", processIndex, variantPath.Length, assetPathFormat);
                //m_assetBundleBuildHandle.DisplayProgressBar(processTitle, processTip, processIndex / (float)processIndexMax);
                assetBundleArchive.PushAssetRef(assetPathFormat, null, true);
                HandleDependAssets(assetPathFormat, true);
            }
            handleDependAssetsPerformance.Dispose();
            //m_assetBundleBuildHandle.ClearProgressBar();
            assetBundleArchive.CreateSourceList();
            assetBundleArchive.MergeAssetRef();
        }
        Dictionary<string, string[]> cachedDepAssetPaths;

        string[] GetDependencies(string assetPath)
        {
            string[] depAssetPaths;
            if (!cachedDepAssetPaths.TryGetValue(assetPath, out depAssetPaths))
            {
                depAssetPaths = AssetDatabase.GetDependencies(assetPath);
                cachedDepAssetPaths[assetPath] = depAssetPaths;
            }
            else
            {
                Debug.LogWarning("duplication GetDependencies " + assetPath);
            }
            return depAssetPaths;
        }

        private void HandleDependAssets(string assetPath, bool isVariant = false)
        {
            string[] depAssetPaths;
            if (m_NoneDependFileExtension.Contains(Path.GetExtension(assetPath).ToLower()))
            {
                depAssetPaths = new string[] { assetPath };
            }
            else
            {
                depAssetPaths = GetDependencies(assetPath);
            }

            var index = 0;
            foreach (var depAssetPath in depAssetPaths)
            {
                index++;
                var depAssetPathFormat = depAssetPath.Replace('\\', '/');
                // depAssetPathFormat = depAssetPathFormat.ToLower();
                if (assetPath.Equals(depAssetPathFormat))
                {
                    continue;
                }
                if (!IsValidFileType(depAssetPathFormat))
                {
                    continue;
                }
                processTip = string.Format("Deps:({0}/{1}) {2}", index, depAssetPaths.Length, depAssetPathFormat);
                //m_assetBundleBuildHandle.DisplayProgressBar(processTitle + assetPath, processTip, process);

                assetBundleArchive.PushAssetRef(assetPath, depAssetPathFormat);

                bool variantable = depAssetPathFormat.StartsWith(assetVariantFolderName) ||
                                   variantOriginalHashSet.Contains(depAssetPathFormat);

                bool depIsVariant = depAssetPathFormat.StartsWith(assetVariantFolderName);

                //如果父节点是变体，则父节点的所有依赖都相当于是被多个资源依赖
                if (isVariant)
                {
                    assetBundleArchive.PushAssetRef(depAssetPathFormat, null, true);
                }

                //如果自己包含体则，则自己必须被单独打包，
                if (variantable)
                {
                    assetBundleArchive.PushAssetRef(depAssetPathFormat, null, true);
                }

                if (hasColloectDependiesSet.Add(depAssetPathFormat))
                {
                    HandleDependAssets(depAssetPathFormat, variantable);
                }
            }
        }
        #endregion

        #region 判断当前资源可用性与依赖获取
        private bool IsValidFileType(string assetPath)
        {
            //配置中设置排除
            if (EditorResourceSettings.IsExcludeAssetPath(assetPath))
            {
                return false;
            }
            if (AssetDatabase.IsValidFolder(assetPath))
            {
                return false;
            }

            //工程内资源: Assets/，Package资源：Packages/<PackageName>/
            if (!assetPath.StartsWith("Assets/"))
            {
                return false;
            }

            return true;
        }

        #endregion

        #region 根据依赖记录中的数据写入依赖树中
        AssetLink allArchive;
        List<StringInt> bundleIndex;
        List<Dependencies> dependenciesList;
        /// <summary>
        /// TODO：将不是直接取出资源排除出记录列表（减小dictionary和二进制文件大小，优化取值序列化和反序列化速度）
        /// </summary>
        /// <param name="dependenciesTree"></param>
        /// <returns></returns>
        private AssetDependenciesTreeBinary AnalysisDependenciesTree(AssetDependenciesTreeBinary dependenciesTree)
        {
            allArchive = assetBundleArchive.GetAssetArchive();
            ///记录所有资源Bundle位置
            List<StringInt> dependenciesIndex = dependenciesTree.loaclRecord.allTreeNodeIndex;
            ///记录Bundle中的资源
            var bundleToAssetPaths = dependenciesTree.loaclRecord.bundleToAssetPaths;
            ///根据BundleName获取bunlde位置
            List<StringInt> formatDependenciesIndex = dependenciesTree.treeNodeIndex;
            ///根据BundleName获取bunlde位置
            ///和上边功能重复TODO：合并
            bundleIndex = dependenciesTree.loaclRecord.bundleIndex;
            ///bundle实际数据列表
            dependenciesList = dependenciesTree.dependentTree;
            ///需生成资源ID的 资源toID Map
            List<StringInt> sourceAssetsIndex = dependenciesTree.loaclResourceMap;
            ///assetName to Bundle位置
            var resourceDic = dependenciesTree.resourceIndex;
            Dependencies dependencies;
            AssetNode archive;
            int index;
            List<string> assetPaths;
            for (int i = 0; i < allArchive.Count; i++)
            {
                archive = allArchive[i];

                if (StringInt.TryGetValue(dependenciesIndex, archive.AssetPath, out index))
                {
                    UpdateDependencies(dependenciesList[index], archive);
                }
                else
                {
                    if (StringInt.TryGetValue(bundleIndex, archive.BundleName, out index))
                    {
                        dependenciesIndex.Add(StringInt.Create(archive.AssetPath, index));
                        UpdateDependencies(dependenciesList[index], archive);
                    }
                    else
                    {
                        dependenciesIndex.Add(StringInt.Create(archive.AssetPath, dependenciesList.Count));
                        bundleIndex.Add(StringInt.Create(archive.BundleName, dependenciesList.Count));
                        dependencies = new Dependencies();
                        dependencies.bundleName = archive.BundleName;
                        index = dependenciesList.Count;
                        dependencies.dependenciesHashArray = new List<int>();
                        dependenciesList.Add(dependencies);
                        UpdateDependencies(dependencies, archive);
                    }
                }
                if (archive.IsSource)
                {
                    int buIndex;
                    if (StringInt.TryGetValue(formatDependenciesIndex, archive.BundleName, out buIndex))
                    {
                        if (buIndex != index)
                        {
                            Debug.LogError("BundleName:" + archive.BundleName + "与 其他BundleName 发生冲突，old buIndex: " + buIndex + " current Index : " + index);
                        }
                    }
                    else
                    {
                        formatDependenciesIndex.Add(StringInt.Create(archive.BundleName, index));
                    }
                    resourceDic.Add(new StringInt() { key = archive.AssetPath, value = index });
                }
                if (StringListString.TryGetValue(bundleToAssetPaths, archive.BundleName, out assetPaths))
                {
                    if (!assetPaths.Contains(archive.AssetPath))
                    {
                        assetPaths.Add(archive.AssetPath);
                    }
                }
                else
                {
                    assetPaths = new List<string>();
                    assetPaths.Add(archive.AssetPath);
                    bundleToAssetPaths.Add(StringListString.Create(archive.BundleName, assetPaths));
                }
            }
            return dependenciesTree;
        }


        private void UpdateDependencies(Dependencies dependencies, AssetNode archive)
        {
            AssetNode archiveTemp;
            var refIndexs = archive.RefIndexs;
            int index;
            for (int i = 0; i < refIndexs.Count; i++)
            {
                archiveTemp = allArchive[refIndexs[i]];
                if (archiveTemp.BundleName == archive.BundleName)
                {
                    continue;
                }
                if (StringInt.TryGetValue(bundleIndex, archiveTemp.BundleName, out index))
                {
                    if (!dependencies.dependenciesHashArray.Contains(index))
                    {
                        dependencies.dependenciesHashArray.Add(index);
                    }
                }
                else
                {
                    //在 assetData中被引用数据将在之后的AnalysisDependenciesTree（）函数中刷新

                    bundleIndex.Add(StringInt.Create(archiveTemp.BundleName, dependenciesList.Count));
                    var sonDependencies = new Dependencies();
                    sonDependencies.bundleName = archiveTemp.BundleName;
                    sonDependencies.dependenciesHashArray = new List<int>();
                    if (!dependencies.dependenciesHashArray.Contains(dependenciesList.Count))
                    {
                        dependencies.dependenciesHashArray.Add(dependenciesList.Count);
                    }
                    dependenciesList.Add(sonDependencies);
                }
            }
        }
        #endregion


        public IEnumerable<Resource> GetDependencyResources(ResourceCollection collection, string assetPath)
        {
            var archive = Archive;
            var node = archive.GetNode(assetPath);

            HashSet<string> set = new HashSet<string>();
            foreach (var usedIndex in node.UsedIndexs)
            {
                foreach (var res in collection.GetResources())
                {
                    var usedNode = archive[usedIndex];
                    if (res.GetAssetByAssetPath(usedNode.AssetPath) != null)
                    {
                        if (!set.Contains(res.FullName))
                        {
                            set.Add(res.FullName);
                            yield return res;
                        }
                        break;
                    }
                }
            }
        }



    }
}