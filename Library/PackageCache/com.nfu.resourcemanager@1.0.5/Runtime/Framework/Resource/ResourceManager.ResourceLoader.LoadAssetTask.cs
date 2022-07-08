//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;

namespace ND.Managers.ResourceMgr.Framework.Resource
{
    public sealed partial class ResourceManager : GameFrameworkModule, IResourceManager
    {
        public sealed partial class ResourceLoader
        {

            private sealed class LoadAssetTask : LoadResourceTaskBase
            {
                private LoadAssetCallbacks m_LoadAssetCallbacks;

                public LoadAssetTask()
                {
                    m_LoadAssetCallbacks = default;
                }

                public override bool IsScene
                {
                    get
                    {
                        return false;
                    }
                }


                public static LoadAssetTask Create(AssetObject assetObject, Type assetType, int priority, LoadAssetCallbacks loadAssetCallbacks, object userData, bool rapidly)
                {
                    LoadAssetTask loadAssetTask = ReferencePool.Acquire<LoadAssetTask>();
                    loadAssetTask.Initialize(assetObject, assetType, priority, userData, rapidly);
                    loadAssetTask.m_LoadAssetCallbacks = loadAssetCallbacks;

                    ///设置异步加载状态
                    if (!assetObject.IsLoaded && !assetObject.IsLoading)
                        assetObject.OnLoading(true);
                    Utility.OnCreateAssetTask(loadAssetTask);
                    return loadAssetTask;
                }

                public override void Clear()
                {
                    base.Clear();
                    m_LoadAssetCallbacks = default;
                }

                public override void OnLoadAssetSuccess(LoadResourceAgent agent, AssetObject asset, float duration)
                {
                    base.OnLoadAssetSuccess(agent, asset, duration);
                    if (m_LoadAssetCallbacks.LoadAssetSuccessCallback != null)
                    {

                        asset.GetRealAsset();
                        m_LoadAssetCallbacks.LoadAssetSuccessCallback(AssetName, asset.GetAsset(AssetType), duration, UserData);
                    }
                }

                public override void OnLoadAssetFailure(LoadResourceAgent agent, LoadResourceStatus status, string errorMessage)
                {
                    base.OnLoadAssetFailure(agent, status, errorMessage);
                    if (m_LoadAssetCallbacks.LoadAssetFailureCallback != null)
                    {
                        m_LoadAssetCallbacks.LoadAssetFailureCallback(AssetName, status, errorMessage, UserData);
                    }
                }

                public override void OnLoadAssetUpdate(LoadResourceAgent agent, LoadResourceProgress type, float progress)
                {
                    base.OnLoadAssetUpdate(agent, type, progress);
                    if (type == LoadResourceProgress.LoadAsset)
                    {
                        if (m_LoadAssetCallbacks.LoadAssetUpdateCallback != null)
                        {
                            m_LoadAssetCallbacks.LoadAssetUpdateCallback(AssetName, progress, UserData);
                        }
                    }
                }

                public override void OnLoadDependencyAsset(LoadResourceAgent agent, string dependencyAssetName, AssetObject dependencyAsset)
                {
                    base.OnLoadDependencyAsset(agent, dependencyAssetName, dependencyAsset);
                    if (m_LoadAssetCallbacks.LoadAssetDependencyAssetCallback != null)
                    {
                        m_LoadAssetCallbacks.LoadAssetDependencyAssetCallback(AssetName, dependencyAssetName, LoadedDependencyAssetCount, TotalDependencyAssetCount, UserData);
                    }
                }
            }
        }
    }
}
