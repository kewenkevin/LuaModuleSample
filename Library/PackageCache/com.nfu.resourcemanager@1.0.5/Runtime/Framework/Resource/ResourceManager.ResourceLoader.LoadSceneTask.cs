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
            private sealed class LoadSceneTask : LoadResourceTaskBase
            {
                private LoadSceneCallbacks m_LoadSceneCallbacks;

                public LoadSceneTask()
                {
                    m_LoadSceneCallbacks = null;
                }

                public override bool IsScene
                {
                    get
                    {
                        return true;
                    }
                }

                public static LoadSceneTask Create(AssetObject sceneAssetObject, int priority, LoadSceneCallbacks loadSceneCallbacks, object userData , bool rapidly)
                {
                    LoadSceneTask loadSceneTask = ReferencePool.Acquire<LoadSceneTask>();
                    loadSceneTask.Initialize(sceneAssetObject, null,  priority,   userData , rapidly);
                    loadSceneTask.m_LoadSceneCallbacks = loadSceneCallbacks;
                    return loadSceneTask;
                }

                public override void Clear()
                {
                    base.Clear();
                    m_LoadSceneCallbacks = null;
                }

                public override void OnLoadAssetSuccess(LoadResourceAgent agent, AssetObject asset, float duration)
                {
                    base.OnLoadAssetSuccess(agent, asset, duration);
                    if (m_LoadSceneCallbacks.LoadSceneSuccessCallback != null)
                    {
                        m_LoadSceneCallbacks.LoadSceneSuccessCallback( duration, UserData);
                    }
                }

                public override void OnLoadAssetFailure(LoadResourceAgent agent, LoadResourceStatus status, string errorMessage)
                {
                    base.OnLoadAssetFailure(agent, status, errorMessage);
                    if (m_LoadSceneCallbacks.LoadSceneFailureCallback != null)
                    {
                        m_LoadSceneCallbacks.LoadSceneFailureCallback(AssetName, status, errorMessage, UserData);
                    }
                }

                public override void OnLoadAssetUpdate(LoadResourceAgent agent, LoadResourceProgress type, float progress)
                {
                    base.OnLoadAssetUpdate(agent, type, progress);
                    if (type == LoadResourceProgress.LoadScene)
                    {
                        if (m_LoadSceneCallbacks.LoadSceneUpdateCallback != null)
                        {
                            m_LoadSceneCallbacks.LoadSceneUpdateCallback(AssetName, progress, UserData);
                        }
                    }
                }

                public override void OnLoadDependencyAsset(LoadResourceAgent agent, string dependencyAssetName, AssetObject dependencyAsset)
                {
                    base.OnLoadDependencyAsset(agent, dependencyAssetName, dependencyAsset);
                    if (m_LoadSceneCallbacks.LoadSceneDependencyAssetCallback != null)
                    {
                        m_LoadSceneCallbacks.LoadSceneDependencyAssetCallback(AssetName, dependencyAssetName, LoadedDependencyAssetCount, TotalDependencyAssetCount, UserData);
                    }
                }
            }
        }
    }
}
