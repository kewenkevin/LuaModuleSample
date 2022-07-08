//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using ND.Managers.ResourceMgr.Runtime;

namespace ND.Managers.ResourceMgr.Framework.Resource
{
    public sealed partial class ResourceManager : GameFrameworkModule, IResourceManager
    {
        public sealed partial class ResourceLoader
        {
#if UNITY_EDITOR

            //记录已加载的资源包
            public static HashSet<string> collectResources = new HashSet<string>();
            //记录已加载的资源
            public static HashSet<string> collectAssets = new HashSet<string>();

#endif
            private abstract class LoadResourceTaskBase : TaskBase
            {
                private static int s_Serial = 0;
                private object m_UserData;
                private DateTime m_StartTime;
                private AssetObject m_AssetObject;
                private Type m_AssetType;
                /// <summary>
                /// 判断AssetObject是否回池
                /// </summary>
                private int m_AssetObjectVersion;

                private bool m_rapidly = false;
                static HashSet<AssetObject> spawnAssetRefCount = new HashSet<AssetObject>();

                public LoadResourceTaskBase()
                {
                    m_UserData = null;
                    m_StartTime = default(DateTime);
                }

                public AssetObject AssetObject { get => m_AssetObject; }

                public string AssetName { get => m_AssetObject.AssetName; }

                public Type AssetType { get => m_AssetType; }

                public ResourceInfo ResourceInfo { get => m_AssetObject.Resource.ResourceInfo; }

                public ResourceObject ResourceObject { get => m_AssetObject.Resource; }

                public abstract bool IsScene
                {
                    get;
                }

                public object UserData
                {
                    get
                    {
                        return m_UserData;
                    }
                }

                public DateTime StartTime
                {
                    get
                    {
                        return m_StartTime;
                    }
                    set
                    {
                        m_StartTime = value;
                    }
                }

                public int LoadedDependencyAssetCount
                {
                    get
                    {
                        int loadedCount = 0;
                        for (int i = 0; i < AssetObject.DependencyAssets.Count; i++)
                        {
                            var depAsset = AssetObject.DependencyAssets[i];
                            if (depAsset.IsLoaded)
                                loadedCount++;
                        }
                        return loadedCount;
                    }
                }

                public int TotalDependencyAssetCount
                {
                    get
                    {
                        return AssetObject.DependencyAssets.Count;
                    }
                }

                public override string Description
                {
                    get
                    {
                        return AssetName;
                    }
                }


                public virtual bool IsDependency
                {
                    get => false;
                }

                public virtual bool IsRapidly
                {
                    get => m_rapidly;
                    set => m_rapidly = value;
                }

                public override bool IsCanceled
                {
                    get => m_AssetObject == null || m_AssetObjectVersion != m_AssetObject.Version;
                }

                public override void Clear()
                {
                    Debug.Assert(m_AssetObject != null);
                    base.Clear();
                    m_UserData = null;
                    m_StartTime = default(DateTime);
                    //if (!IsDependency)
                    //{
                    //    //Debug.Log("-------- Task Unspawn: " + m_AssetObject.Name);
                    //    ResourceV2Entry.Resource.ResourceManager.AssetPool.Unspawn(m_AssetObject.Name);
                    //}
                    m_AssetObject = null;
                    m_AssetObjectVersion = 0;
                    m_AssetType = null;
                }


                public List<AssetObject> GetDependencyAssets()
                {
                    return AssetObject.DependencyAssets;
                }

                public void LoadMain(LoadResourceAgent agent, ResourceObject resourceObject)
                {
                    agent.Helper.LoadAsset(resourceObject.Target, resourceObject.Name, AssetName, AssetType, IsScene, IsDependency, AssetObject, IsRapidly);
                }

                public virtual void OnLoadAssetSuccess(LoadResourceAgent agent, AssetObject asset, float duration)
                {
                }

                public virtual void OnLoadAssetFailure(LoadResourceAgent agent, LoadResourceStatus status, string errorMessage)
                {
                }

                public virtual void OnLoadAssetUpdate(LoadResourceAgent agent, LoadResourceProgress type, float progress)
                {
                }

                public virtual void OnLoadDependencyAsset(LoadResourceAgent agent, string dependencyAssetName, AssetObject dependencyAsset)
                {

                }

                protected void Initialize(AssetObject assetObject, Type assetType, int priority, object userData, bool rapidly)
                {
                    Debug.Assert(assetObject != null);
                    Initialize(++s_Serial, priority);
                    m_UserData = userData;
                    m_AssetObject = assetObject;
                    m_rapidly = rapidly;
                    m_AssetType = assetType;
                    m_AssetObjectVersion = assetObject.Version;
                    Log.Info(" Task: " + GetType().Name + " " + assetObject.Name);
                    //if (!IsDependency)
                    //{
                    //    //Debug.Log("-------- Spawn: " + assetObject.Name);
                    //    //任务增加 Asset 计数
                    //    ResourceV2Entry.Resource.ResourceManager.AssetPool.Spawn(assetObject.Name);
                    //    spawnAssetRefCount.Clear();
                    //    spawnAssetRefCount.Add(assetObject);
                    //    assetObject.SpawnDependencies(spawnAssetRefCount);
                    //}
#if UNITY_EDITOR
                    collectAssets.Add(assetObject.Name);
                    if (assetObject.Resource != null)
                        collectResources.Add(assetObject.Resource.Name);
#endif
                }

                ~LoadResourceTaskBase()
                {
                    if (m_AssetObject != null)
                    {
                        Clear();
                    }
                }

            }
        }
    }
}
