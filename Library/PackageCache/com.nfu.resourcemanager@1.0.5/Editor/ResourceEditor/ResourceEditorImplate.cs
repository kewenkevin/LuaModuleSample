
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ND.Managers.ResourceMgr.Editor.ResourceDependencyAnalyzer;
using ND.Managers.ResourceMgr.Framework;
using ND.Managers.ResourceMgr.Runtime;

using UnityEditor;
using UnityEngine;

namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{

    /// <summary>
    /// 分析操作类，包含添加资源和资源包方法
    /// </summary>
    public class ResourceEditorImplate
    {
        private ResourceEditorController m_Controller = null;
        private IAssetBundleAnalysisHandler builder;
        public ResourceEditorController Controller { get => m_Controller; }

        /// <summary>
        /// 当分析结果发生改变时通知事件
        /// </summary>
        public static event Action Changed;

        public ResourceEditorImplate()
        {
            m_Controller = new ResourceEditorController();

            m_Controller.OnLoadingResource += OnLoadingResource;
            m_Controller.OnLoadingAsset += OnLoadingAsset;
            m_Controller.OnLoadCompleted += OnLoadCompleted;

        }

        /// <summary>
        /// 加载配置
        /// </summary>
        public void Load()
        {
            if (m_Controller.Load())
            {
                Debug.Log("Load configuration success.");
            }
            else
            {
                Debug.LogWarning("Load configuration failure.");
            }

            InitShowTypes();

        }

        /// <summary>
        /// 开始分析资源
        /// </summary>
        public void Run()
        {
            EditorUtility.DisplayProgressBar("Clear", "Processing...", 0f);
            ClearAll();
            EditorUtility.ClearProgressBar();

            EditorUtility.DisplayProgressBar("Save First", "Processing...", 0f);
            SaveConfiguration();
            EditorUtility.ClearProgressBar();

            AutoAnalysis();

            EditorUtility.DisplayProgressBar("Save Final", "Processing...", 0f);
            SaveConfiguration();
            EditorUtility.ClearProgressBar();

            if (EditorResourceSettings.SetAssetBundleName && Controller.ResourceCount > 0)
            {
                EditorUtility.DisplayProgressBar("Update All AssetBundleName", "", 0f);
                m_Controller.ResourceCollection.UpdateAllAssetBundleNameAndVariant();
                EditorUtility.ClearProgressBar();
            }
        }

        /// <summary>
        /// 保存分析结果
        /// </summary>
        private void SaveConfiguration()
        {
            if (m_Controller.Save())
            {
                Debug.Log("Save configuration success.");
            }
            else
            {
                Debug.LogWarning("Save configuration failure.");
            }
        }
        
        public void AddResource(string name, string variant, bool refresh)
        {
            AddResource(name, variant, refresh, true);
        }
        
        /// <summary>
        /// 添加资源包
        /// </summary>
        public void AddResource(string name, string variant, bool refresh, bool toLower)
        {
            if (variant == string.Empty)
            {
                variant = null;
            }

            string fullName = GetResourceFullName(name, variant);
            if (m_Controller.AddResource(name, variant, null, LoadType.LoadFromFile, false, toLower: toLower))
            {
                //if (refresh)
                //{
                //    RefreshResourceTree();
                //}
                Resource resource = m_Controller.GetResource(name, variant);
                builder.OnPreprocessResource(resource);
                Debug.Log(Utility.Text.Format("Add resource '{0}' success.", fullName));
            }
            else
            {
                Debug.LogWarning(Utility.Text.Format("Add resource '{0}' failure.", fullName));
            }
        }



        /// <summary>
        /// 添加资源到资源包
        /// </summary>
        public void AssignAsset(SourceAsset sourceAsset, Resource resource)
        {
            if (null == sourceAsset)
            {
                return;
            }

            if (!m_Controller.AssignAsset(sourceAsset.Guid, resource.Name, resource.Variant))
            {
                Debug.LogWarning(Utility.Text.Format("Assign asset '{0}' to resource '{1}' failure.", sourceAsset.Name,
                    resource.FullName));
            }
        }


        private void CleanResource()
        {
            int unknownAssetCount = m_Controller.RemoveUnknownAssets();
            int unusedResourceCount = m_Controller.RemoveUnusedResources();

            Debug.Log(Utility.Text.Format(
                "Clean complete, {0} unknown assets and {1} unused resources has been removed.",
                unknownAssetCount.ToString(), unusedResourceCount.ToString()));
        }

        /// <summary>
        /// 清除分析结果
        /// </summary>
        public void ClearAll()
        {
            CleanResource();
            m_Controller.ClearRecord();
        }

        /// <summary>
        /// 运行分析
        /// </summary>
        public void AutoAnalysis()
        {
            var performance = new PerformanceSample($"{GetType().Name}.{nameof(AutoAnalysis)}");
            CleanResource();

            AssetDependenciesTreeBinary dependenciesTree = new AssetDependenciesTreeBinary();
            var builder = GetAnalysisStrategy();
            this.builder = builder;

            EditorUtility.DisplayProgressBar("OnPreprocessAnalysisBegin", "", 0f);
            using (new PerformanceSample($"{builder.GetType().Name}.OnPreprocessAnalysisBegin"))
            {
                builder.OnPreprocessAnalysisBegin();
            }
            EditorUtility.ClearProgressBar();


            Dictionary<System.Type, IResourcePreprocessBuild> preprocessBuilds = new Dictionary<System.Type, IResourcePreprocessBuild>();
            //PreprocessBuildInitialize
            using (new PerformanceSample("PreprocessBuildInitialize"))
            {
                foreach (var group in EditorResourceSettings.Groups)
                {
                    foreach (var preprocessBuild in group.PreprocessBuilds)
                    {
                        if (preprocessBuild.PreprocessBuild != null)
                        {
                            if (!preprocessBuilds.ContainsKey(preprocessBuild.PreprocessBuild.GetType()))
                            {
                                EditorUtility.DisplayProgressBar("Preprocess Build Initialize", preprocessBuild.PreprocessBuild.GetType().Name, 0f);
                                using (new PerformanceSample($"{preprocessBuild.PreprocessBuild.GetType().Name}.PreprocessBuildInitialize"))
                                {
                                    preprocessBuild.PreprocessBuild.PreprocessBuildInitialize();
                                }
                                preprocessBuilds.Add(preprocessBuild.PreprocessBuild.GetType(), preprocessBuild.PreprocessBuild);
                            }
                        }
                    }
                }
            }

            //PreprocessBuild
            using (new PerformanceSample("PreprocessBuild"))
            {
                foreach (var group in EditorResourceSettings.Groups)
                {
                    foreach (var preprocessBuild in group.PreprocessBuilds)
                    {
                        if (preprocessBuild.PreprocessBuild != null)
                        {
                            EditorUtility.DisplayProgressBar("PreprocessBuild", preprocessBuild.PreprocessBuild.GetType().Name, 0f);
                            using (new PerformanceSample($"{preprocessBuild.PreprocessBuild.GetType().Name}.PreprocessBuild"))
                            {
                                preprocessBuild.PreprocessBuild.PreprocessBuild();
                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh();
                            }
                        }
                    }
                }
            }

            //PreprocessBuildCleanup
            using (new PerformanceSample("PreprocessBuildCleanup"))
            {
                foreach (var preprocessBuild in preprocessBuilds.Values)
                {
                    EditorUtility.DisplayProgressBar("Preprocess Build Cleanup", preprocessBuild.GetType().Name, 0f);
                    using (new PerformanceSample($"{preprocessBuild.GetType().Name}.PreprocessBuildCleanup"))
                    {
                        preprocessBuild.PreprocessBuildCleanup();
                    }
                }
            }

            EditorUtility.ClearProgressBar();

            var treeCreator = new AssetBundlieTreeCreator(builder);
            treeCreator.GetDependencies();
            treeCreator.AnalysisBundlName(builder);
            //ProcessAddressableAssets();
            treeCreator.UpdateAndAddTree(dependenciesTree);
            AssetBundleBuild[] allAssetBuildParam = treeCreator.CreateAllBuildParameter();


            m_Controller.ClearRecord();

            AddResourceByAssetBundleBuildInfo(treeCreator.UseableResource, allAssetBuildParam, builder, treeCreator.variantOriginalHashSet);


            if (EditorResourceSettings.StripAutoDependency)
            {
                StripAutoDependency(treeCreator);
            }

            if (EditorResourceSettings.CheckMissingPrefab)
            {
                CheckMissingPrefab();
            }

            //确保清除进度条
            EditorUtility.ClearProgressBar();
            Changed?.Invoke();
            performance.Dispose();
        }

        internal List<System.Type> analysisStrategyTypes = new List<System.Type>();
        internal string[] analysisStrategyNames = new string[0];
        internal int m_analysisStrategySelectIndex = 0;

        /// <summary>
        /// 获取分析策略
        /// </summary>
        /// <returns></returns>
        private IAssetBundleAnalysisHandler GetAnalysisStrategy()
        {
            if (m_analysisStrategySelectIndex >= 0 && m_analysisStrategySelectIndex < analysisStrategyTypes.Count)
            {
                return Activator.CreateInstance(analysisStrategyTypes[m_analysisStrategySelectIndex]) as IAssetBundleAnalysisHandler;
            }
            else
            {
                return new DefaultAssetBundleAnalysisHandler();
            }
        }


        /// <summary>
        /// 获取所有策略实现
        /// </summary>
        private void InitShowTypes()
        {
            List<string> names = new List<string>();
            analysisStrategyTypes.Clear();
            var cType = typeof(IAssetBundleAnalysisHandler);
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
            .Referenced(cType.Assembly)
            .SelectMany(o => o.GetTypes()))
            {
                if (type.IsSubclassOf(cType))
                {
                    analysisStrategyTypes.Add(type);
                    if (type.Name == m_Controller.AnalysisHandlerName)
                    {
                        m_analysisStrategySelectIndex = names.Count;
                    }

                    names.Add(type.Name);
                }
            }

            analysisStrategyNames = names.ToArray();
        }

        /// <summary>
        /// 处理原始资源，目录为<see cref="EditorResourceSettings.RawAssetPath"/>，该目录资源资源生成资源包和资源信息，资源模式为 <see cref="LoadType.LoadFromBinary"/> , 该目录内资源特殊处理不进 `AssetBundle`直接复制到输出目录
        /// </summary>
        private void ProcessRawAssets(IAssetBundleAnalysisHandler builder)
        {
            var rawAssetPath = ResourceSettings.RawAssetsPath;
            if (!Directory.Exists(rawAssetPath))
            {
                return;
            }
            string rawAssetPathLower = rawAssetPath.ToLower();
            var files = Directory.GetFiles(rawAssetPath, "*.*", System.IO.SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                if (file.EndsWith(".meta")) continue;

                file = file.Replace('\\', '/');

                var resourceName = Path.Combine(rawAssetPathLower, file.ToRelativePath(rawAssetPath));
                resourceName = resourceName.Replace('\\', '/');

                Resource resource = m_Controller.GetResource(resourceName, null);
                if (resource == null)
                {
                    AddResource(resourceName, null, false, toLower: false);
                    resource = m_Controller.GetResource(resourceName, null);
                }

                if (resource == null)
                {
                    continue;
                }

                if (resource != null)
                {
                    resource.LoadType = LoadType.LoadFromBinary;
                    SourceAsset asset = m_Controller.SourceAssetRoot.GetAssetByPath(file);
                    if (asset != null)
                        AssignAsset(asset, resource);
                }
            }
        }


        public HashSet<string> ProcessAddressableAssets()
        {
            string bundleName;
            string assetName;
            string variant;
            HashSet<string> assets = new HashSet<string>();
            foreach (var assetPath in AssetDatabase.GetAllAssetPaths())
            {
                if (AssetDatabase.IsValidFolder(assetPath))
                    continue;
                if (EditorResourceSettings.GetAddressable(assetPath, out bundleName, out variant, out assetName))
                {
                    AddResource(bundleName, variant, false);
                    Resource resource = m_Controller.GetResource(bundleName, variant);
                    if (resource == null)
                        throw new Exception($"get resource null, bundleName: {bundleName}");
                    var asset = m_Controller.SourceAssetRoot.GetAssetByPath(assetPath);
                    AssignAsset(asset, resource);
                }
            }
            return assets;
        }

        /// <summary>
        /// 添加自定义资源包
        /// </summary>
        private void ProcessAddressable()
        {
            EditorUtility.DisplayProgressBar("Addressable", "", 0f);


            string bundleName;
            string assetName;
            string variant;

            foreach (var assetPath in AssetDatabase.GetAllAssetPaths())
            {
                if (AssetDatabase.IsValidFolder(assetPath))
                    continue;

                for (int i = EditorResourceSettings.Groups.Length - 1; i >= 0; i--)
                {
                    var group = EditorResourceSettings.Groups[i];
                    var rule = group.FindRule(assetPath);
                    if (rule != null)
                    {
                        if (rule.AddressableProvider != null)
                        {
                            rule.AddressableProvider.OnPreprocessAnalysis(assetPath, this);
                        }
                        break;
                    }
                }
            }
            EditorUtility.ClearProgressBar();

        }

        /// <summary>
        /// 分析 <see cref="AssetBundleBuild"/>计算所有资源
        /// </summary>
        /// <param name="needUseAssets">所有使用到的资源</param>
        /// <param name="allAssetBuildParam">所有资源包对应的资源</param>
        /// <param name="builder">分析器</param>
        /// <param name="variantOriginalHashSet">变体资源</param>
        public void AddResourceByAssetBundleBuildInfo(List<string> needUseAssets, AssetBundleBuild[] allAssetBuildParam, IAssetBundleAnalysisHandler builder, HashSet<string> variantOriginalHashSet)
        {
            m_Controller.Load();

            ProcessRawAssets(builder);

            ProcessAddressable();


            builder.OnPreprocessAnalysis(AddResource, AssignAsset, m_Controller.GetResource, m_Controller.SourceAssetRoot.GetAssetByPath);

            //处理变体资源
            var assetVariantFolderName = EditorResourceSettings.VariantAssetPath;


            HashSet<string> variantBundleNames = new HashSet<string>();
            foreach (var assetPath in variantOriginalHashSet)
            {
                AssetBundleBuild assetBuildBuild = allAssetBuildParam.Where(o => o.assetNames.Contains(assetPath)).FirstOrDefault();
                if (assetBuildBuild.assetBundleName != null)
                {
                    string bundleName = assetBuildBuild.assetBundleName;
                    if (bundleName.StartsWith(assetVariantFolderName))
                    {
                        bundleName = bundleName.Substring(assetVariantFolderName.Length);

                        bundleName = bundleName.Substring(bundleName.IndexOf("/") + 1);
                    }
                    variantBundleNames.Add(bundleName);
                }
            }


            for (int i = 0; i < allAssetBuildParam.Length; i++)
            {
                var assetBundle = allAssetBuildParam[i].assetBundleName;
                string variant = null;
                string variantName = "none";
                var originalResourceName = assetBundle;
                if (originalResourceName.StartsWith(assetVariantFolderName))
                {
                    originalResourceName = allAssetBuildParam[i].assetBundleName.Substring(assetVariantFolderName.Length);
                    variantName = originalResourceName.Substring(0, originalResourceName.IndexOf("/"));
                    originalResourceName = originalResourceName.Substring(originalResourceName.IndexOf("/") + 1);
                }


                if (variantBundleNames.Contains(originalResourceName))
                {
                    assetBundle = originalResourceName;
                    variant = variantName?.ToLower();
                }


                assetBundle = assetBundle.Replace(' ', '_');
                assetBundle = assetBundle.Replace('.', '_');

                AddResource(assetBundle, variant, false);

                Resource resource = m_Controller.GetResource(assetBundle, variant);
                if (resource == null)
                {
                    continue;
                }


                var assetNames = allAssetBuildParam[i].assetNames;

                List<string> waitSearchAssetNames = assetNames.ToList();
                List<string> waitAddAssetNames = assetNames.ToList();

                // while (waitSearchAssetNames.Count>0)
                // {
                //     string assetName = waitSearchAssetNames[0];
                //     waitSearchAssetNames.RemoveAt(0);
                //     
                //     var dependencies = AssetDatabase.GetDependencies(assetName);
                //     for (int k = 0; k < dependencies.Length; k++)
                //     {
                //         var name = dependencies[k];
                //         if (name.Equals(assetName)) continue;
                //         if (needUseAssets.Contains(name)) continue;
                //         waitAddAssetNames.Remove(name);
                //     }
                //    
                // }

                for (int j = 0; j < waitAddAssetNames.Count; j++)
                {
                    string assetName = waitAddAssetNames[j];
                    var asset = m_Controller.SourceAssetRoot.GetAssetByPath(assetName);

                    if (asset != null)
                    {
                        AssignAsset(asset, resource);
                    }
                    else
                    {
                        m_Controller.RemoveResource(assetBundle, variant);
                        Debug.LogErrorFormat("资源 {0} 存在异常，无法获取，请检查配置类型过滤列表，或资源本身导入状态！，本次添加将被取消", assetName);
                    }
                }
            }
        }





        public string GetResourceFullName(string name, string variant)
        {
            return variant != null ? Utility.Text.Format("{0}.{1}", name, variant) : name;
        }

        private void OnLoadingResource(int index, int count)
        {
            EditorUtilityx.DisplayProgressBar("Loading Resources",
                Utility.Text.Format("Loading resources, {0}/{1} loaded.", index.ToString(), count.ToString()),
                (float)index / count);
        }

        private void OnLoadingAsset(int index, int count)
        {
            EditorUtilityx.DisplayProgressBar("Loading Assets",
                Utility.Text.Format("Loading assets, {0}/{1} loaded.", index.ToString(), count.ToString()),
                (float)index / count);
        }

        private void OnLoadCompleted()
        {
            EditorUtilityx.ClearProgressBar();
        }

        /// <summary>
        /// 剔除自动依赖，合并依赖1次的资源
        /// </summary>
        public void StripAutoDependency(AssetBundlieTreeCreator treeCreator)
        {

            using (new PerformanceSample($"{nameof(ResourceEditorImplate)}.{nameof(StripAutoDependency)}"))
            {

                string variantPath = EditorResourceSettings.VariantAssetPath;

                int depCount = 0;

                List<string> excludePaths = new List<string>();

                foreach (var path in new string[] { EditorResourceSettings.LuaAssetPath, 
                             ResourceSettings.RawAssetsPath })
                {
                    if (string.IsNullOrEmpty(path))
                        continue;
                    if (path.EndsWith("/"))
                        excludePaths.Add(path.ToLower());
                    else
                        excludePaths.Add(path.ToLower() + "/");
                }
                var assets = m_Controller.ResourceCollection.GetAssets();

                Asset asset;
                List<string> unassignedAssetGuids = new List<string>();
                for (int i = 0; i < assets.Length; i++)
                {
                    asset = assets[i];
                    EditorUtilityx.DisplayProgressBar("Strip Auto Dependency", $"{i}/{assets.Length}", (i / (float)assets.Length));

                    if (asset.Resource.Count == 0)
                        continue;

                    if (excludePaths.FirstOrDefault(o => asset.Name.ToLower().StartsWith(o)) != null)
                        continue;


                    if (!(EditorResourceSettings.IsAssetRootPath(asset.Name) || (!string.IsNullOrEmpty(variantPath) && asset.Name.StartsWith(variantPath))))
                    {
                        depCount = treeCreator.GetDependencyResources(m_Controller.ResourceCollection, asset.Name).Count();

                        if (depCount <= 1)
                        {
                            Debug.Log($"使用隐式打包: " + asset.Name + " 资源包名: " + asset.Resource.Name);
                            unassignedAssetGuids.Add(asset.Guid);
                        }
                    }
                }

                for (int i = 0; i < unassignedAssetGuids.Count; i++)
                {
                    m_Controller.UnassignAsset(unassignedAssetGuids[i]);
                }
                
                int stripCount = m_Controller.RemoveUnusedResources();
                Debug.Log($"移除空资源包数量:  {stripCount}");
                EditorUtilityx.ClearProgressBar();
            }
        }


        #region 检查 prefab 丢失

        /// <summary>
        /// Prefab 引用丢失检查，发现丢失输出错误日志提示
        /// </summary>
        public void CheckMissingPrefab()
        {
            using (new PerformanceSample(nameof(CheckMissingPrefab)))
            {
                var assets = m_Controller.ResourceCollection.GetAssets();
                List<string> assetPaths = new List<string>(assets.Length);
                List<string> guids = new List<string>();

                foreach (var asset in assets)
                {
                    if (asset.AssetPath.EndsWith(".prefab", StringComparison.InvariantCultureIgnoreCase))
                    {
                        assetPaths.Add(asset.AssetPath);
                    }
                }

                string assetPath;
                for (int i = 0; i < assetPaths.Count; i++)
                {
                    assetPath = assetPaths[i];
                    EditorUtilityx.DisplayProgressBar("Check Missing Prefab", $"{i}/{assetPaths.Count}", (i / (float)assetPaths.Count));

                    foreach (var guid in EditorUtilityx.FindMissingPrefabs(assetPath))
                    {
                        Debug.LogError($"{assetPath} missing : {guid}");
                        guids.Add(guid);
                    }
                }

                EditorUtilityx.ClearProgressBar();
            }
        }

        #endregion
    }
}