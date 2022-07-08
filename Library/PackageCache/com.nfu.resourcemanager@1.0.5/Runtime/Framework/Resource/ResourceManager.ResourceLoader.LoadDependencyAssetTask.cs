//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

namespace ND.Managers.ResourceMgr.Framework.Resource
{
    public sealed partial class ResourceManager : GameFrameworkModule, IResourceManager
    {
        public sealed partial class ResourceLoader
        {
            private sealed class LoadDependencyAssetTask : LoadResourceTaskBase
            {
                private LoadResourceTaskBase m_MainTask;

                public LoadDependencyAssetTask()
                {
                    m_MainTask = null;
                }

                public override bool IsScene
                {
                    get
                    {
                        return false;
                    }
                }

                public override bool IsDependency => true;

                public static LoadDependencyAssetTask Create(AssetObject assetObject, int priority, LoadResourceTaskBase mainTask, object userData, bool rapidly)
                {
                    LoadDependencyAssetTask loadDependencyAssetTask = ReferencePool.Acquire<LoadDependencyAssetTask>();
                    loadDependencyAssetTask.Initialize(assetObject, null, priority, userData, rapidly);
                    loadDependencyAssetTask.m_MainTask = mainTask;

                    ///设置异步加载状态
                    if (!assetObject.IsLoaded && !assetObject.IsLoading)
                        assetObject.OnLoading(true);
                    Utility.OnCreateAssetTask(loadDependencyAssetTask, true);

                    return loadDependencyAssetTask;
                }

                public override void Clear()
                {
                    base.Clear();
                    m_MainTask = null;
                }

                public override void OnLoadAssetSuccess(LoadResourceAgent agent, AssetObject asset, float duration)
                {
                    base.OnLoadAssetSuccess(agent, asset, duration);
                    //MainTask 可能已回收
                    if (m_MainTask.AssetObject != null)
                        m_MainTask.OnLoadDependencyAsset(agent, AssetName, asset);
                }

                public override void OnLoadAssetFailure(LoadResourceAgent agent, LoadResourceStatus status, string errorMessage)
                {
                    base.OnLoadAssetFailure(agent, status, errorMessage);

                    if (m_MainTask.AssetObject != null)
                        m_MainTask.OnLoadAssetFailure(agent, LoadResourceStatus.DependencyError, Utility.Text.Format("Can not load dependency asset '{0}', internal status '{1}', internal error message '{2}'.", AssetName, status.ToString(), errorMessage));
                }
            }
        }
    }
}
