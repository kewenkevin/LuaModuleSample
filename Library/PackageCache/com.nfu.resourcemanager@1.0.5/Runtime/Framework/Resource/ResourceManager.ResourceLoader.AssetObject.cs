//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.U2D;
using ND.Managers.ResourceMgr.Framework.ObjectPool;
using ND.Managers.ResourceMgr.Runtime;
using Object = UnityEngine.Object;

namespace ND.Managers.ResourceMgr.Framework.Resource
{
    public sealed partial class ResourceManager : GameFrameworkModule, IResourceManager
    {
        public sealed partial class ResourceLoader
        {
            /// <summary>
            /// 资源对象。
            /// </summary>
            public sealed class AssetObject : ObjectBase
            {
                private List<AssetObject> m_DependencyAssets;
                private HashSet<AssetObject> m_BeDependencyAssets;
                private ResourceObject m_Resource;
                private IResourceHelper m_ResourceHelper;
                private ResourceLoader m_ResourceLoader;
                public string AssetName { get; private set; }
                public Type AssetType { get; private set; }

                public object[] subAssets { get; private set; }

                /// <summary>
                /// 是否异步加载
                /// </summary>
                public bool IsLoadAsync { get; private set; }

                private Dictionary<Type, (object asset, object[] subAssets)> assets = new Dictionary<Type, (object asset, object[] subAssets)>();

                private static HashSet<AssetObject> checkCircleDepend = new HashSet<AssetObject>();

                public AssetObject()
                {
                    m_DependencyAssets = new List<AssetObject>();
                    m_BeDependencyAssets = new HashSet<AssetObject>();
                    m_Resource = null;
                    m_ResourceHelper = null;
                    m_ResourceLoader = null;
                }

                public override bool CustomCanReleaseFlag
                {
                    get
                    {
                        //使用m_AssetDependencyCount 导致循环依赖不能释放
                        //应该使用 Spawn 次数为0 释放
                        //int targetReferenceCount = 0;
                        //m_ResourceLoader.m_AssetDependencyCount.TryGetValue(this, out targetReferenceCount);
                        //return base.CustomCanReleaseFlag && targetReferenceCount <= 0;
                        return base.CustomCanReleaseFlag && m_BeDependencyAssets.Count == 0;
                    }
                }
                public ResourceObject Resource
                {
                    get => m_Resource;
                }

                public List<AssetObject> DependencyAssets { get => m_DependencyAssets; }

                /// <summary>
                /// 被依赖
                /// </summary>
                public HashSet<AssetObject> BeDependencyAssets { get => m_BeDependencyAssets; }

                /// <summary>
                /// Asset 是否已加载
                /// </summary>
                public bool IsAssetLoaded
                {
                    get; private set;
                }

                /// <summary>
                /// 依赖的 Asset 是否已加载
                /// </summary>
                public bool IsDependencyAssetLoaded
                {
                    get
                    {
                        foreach (var depAsset in m_DependencyAssets)
                        {
                            if (!depAsset.IsAssetLoaded)
                                return false;
                        }
                        return true;
                    }
                }

                /// <summary>
                /// Asset 和依赖的 Asset 都已加载
                /// </summary>
                public override bool IsLoaded
                {
                    get
                    {
                        if (!base.IsLoaded)
                        {
                            if (IsAssetLoaded && IsDependencyAssetLoaded)
                            {
                                IsLoaded = true;
                            }
                        }
                        return base.IsLoaded;
                    }
                    protected set => base.IsLoaded = value;
                }


                public static AssetObject Create(string name, string assetName, Type assetType, List<AssetObject> dependencyAssets, ResourceObject resource, IResourceHelper resourceHelper, ResourceLoader resourceLoader)
                {
                    return Create(name, assetName, assetType, null, null, dependencyAssets, resource, resourceHelper, resourceLoader, false);
                }


                private static AssetObject Create(string name, string assetName, Type assetType, object target, object[] subAssets, List<AssetObject> dependencyAssets, ResourceObject resource, IResourceHelper resourceHelper, ResourceLoader resourceLoader, bool loaeded)
                {
                    //if (dependencyAssets == null)
                    //{
                    //    throw new GameFrameworkException("Dependency assets is invalid.");
                    //}

                    if (resource == null)
                    {
                        throw new GameFrameworkException("Resource is invalid.");
                    }

                    if (resourceHelper == null)
                    {
                        throw new GameFrameworkException("Resource helper is invalid.");
                    }

                    if (resourceLoader == null)
                    {
                        throw new GameFrameworkException("Resource loader is invalid.");
                    }

                    AssetObject assetObject = ReferencePool.Acquire<AssetObject>();
                    assetObject.Initialize(name, target);
                    assetObject.AssetName = assetName;
                    assetObject.AssetType = assetType;

                    assetObject.m_Resource = resource;
                    assetObject.m_ResourceHelper = resourceHelper;
                    assetObject.m_ResourceLoader = resourceLoader;
                    assetObject.subAssets = subAssets;
                    assetObject.IsAssetLoaded = loaeded;
                    assetObject.IsLoaded = false;
                    if (dependencyAssets != null)
                        assetObject.SetDependencyAssets(dependencyAssets);
                    Utility.OnCreateAssetObject(assetObject);
                    return assetObject;
                }

                public void SetDependencyAssets(IEnumerable<AssetObject> dependencyAssets)
                {
                    m_DependencyAssets.Clear();
                    m_DependencyAssets.AddRange(dependencyAssets);
                    foreach (var dependencyAsset in dependencyAssets)
                    {
                        dependencyAsset.m_BeDependencyAssets.Add(this);
                    }

                    //var resourceLoader = m_ResourceLoader;
                    //foreach (var dependencyAsset in dependencyAssets)
                    //{
                    //    int referenceCount = 0;
                    //    if (resourceLoader.m_AssetDependencyCount.TryGetValue(dependencyAsset, out referenceCount))
                    //    {
                    //        resourceLoader.m_AssetDependencyCount[dependencyAsset] = referenceCount + 1;
                    //    }
                    //    else
                    //    {
                    //        resourceLoader.m_AssetDependencyCount.Add(dependencyAsset, 1);
                    //    }
                    //}
                }

                /// <summary>
                /// 标记加载中防止循环加载
                /// </summary>
                /// <param name="isLoadAsync"></param>
                public void OnLoading(bool isLoadAsync)
                {
                    Log.Info($"Asset loading '{Name}'");
                    IsLoading = true;
                    IsLoaded = false;
                    this.IsLoadAsync = isLoadAsync;
                    ResourceEntry.Resource.ResourceManager.AssetPool.SetTarget(this, null);
                    Target = null;
                }

                public void OnLoaded(object target, object[] subAssets)
                {
                    if (!IsAssetLoaded)
                        Utility.OnLoadAssetCompleted();

                    Log.Info($"Asset loaded '{Name}', delay: {IsDelayCreate}");

                    ResourceEntry.Resource.ResourceManager.AssetPool.SetTarget(this, target);
                    Target = target;
                    this.subAssets = subAssets;
                    IsLoading = false;
                    IsAssetLoaded = true;


                    var atlas = target as SpriteAtlas;
                    if (atlas)
                    {
                        ResourceEntry.Resource.RegisterSpriteAtlas(atlas);
                    }

                }

                private (object asset, object[] subAssets) LoadAsset(Type assetType)
                {
                    (object asset, object[] subAssets) item;

                    if (assetType == null || assetType == typeof(object))
                        assetType = typeof(Object);

                    if (assets.TryGetValue(assetType, out item))
                    {
                        return item;
                    }

                    if (!Resource.IsLoaded)
                        throw new GameFrameworkException($"Can't load asset '{Name}', resource not loaded");

                    var bundle = Resource.Target as AssetBundle;
                    object asset = null;
                    object[] subAssets;

                    if (bundle.isStreamedSceneAssetBundle)
                    {
                        subAssets = new Object[] { new Object() };
                    }
                    else if (assetType == typeof(Object) || assetType == typeof(object))
                    {
                        asset = bundle.LoadAsset(AssetName);
                        subAssets = bundle.LoadAssetWithSubAssets(AssetName);
                    }
                    else
                    {
                        asset = bundle.LoadAsset(AssetName, assetType);
                        subAssets = bundle.LoadAssetWithSubAssets(AssetName, assetType);
                        if (asset == null)
                        {
                            Debug.LogError($"Load asset null, assetName: {AssetName}, assetType: {assetType?.Name}");
                        }
                    }

                    //移除主资源，和 LoadResourceAgentHelperLoadCompleteEventArgs Create 保持一致
                    subAssets = subAssets.Where(o => asset != o).ToArray();
                    item = new ValueTuple<object, object[]>(asset, subAssets);

                    assets[assetType] = item;

                    return item;
                }

                public void LoadAsset()
                {
                    var item = LoadAsset(null);

                    IsDelayCreate = false;
                    OnLoaded(item.asset, item.subAssets);
                }

                public void SetDelayLoad()
                {
                    if (!IsAssetLoaded)
                    {
                        IsDelayCreate = true;
                        OnLoaded(new object(), null);
                    }
                }

                /// <summary>
                /// 如果为延迟对象则立即加载
                /// </summary>
                public void GetRealAsset()
                {
                    if (!IsAssetLoaded)
                        throw new Exception("not ready");

                    if (IsDelayCreate)
                    {
                        LoadAsset();
                    }
                }


                public object GetAsset(Type assetType = null)
                {
                    return LoadAsset(assetType).asset;
                }

                public object[] GetSubAssets(Type assetType = null)
                {
                    return LoadAsset(assetType).subAssets;
                }


                public override void Clear()
                {
                    base.Clear();
                    m_DependencyAssets.Clear();
                    m_BeDependencyAssets.Clear();
                    m_Resource = null;
                    m_ResourceHelper = null;
                    m_ResourceLoader = null;
                    AssetName = null;
                    AssetType = null;
                    IsLoadAsync = false;
                    IsLoading = false;
                    IsLoaded = false;
                    IsDelayCreate = true;
                    IsAssetLoaded = false;
                    if (assets != null)
                        assets.Clear();
                }

                protected internal override void OnSpawn()
                {
                    base.OnSpawn();
                }

                protected internal override void OnUnspawn()
                {
                    foreach (var dependencyAsset in m_DependencyAssets)
                    {
                        //被自动释放
                        if (dependencyAsset.AssetName == null)
                        {
                            continue;
                        }
                        m_ResourceLoader.m_AssetPool.Unspawn(dependencyAsset);
                    }
                    base.OnUnspawn();
                }

                protected internal override void Release(bool isShutdown)
                {
                    if (!isShutdown)
                    {

                        //int targetReferenceCount = 0;
                        //if (m_ResourceLoader.m_AssetDependencyCount.TryGetValue(this, out targetReferenceCount) && targetReferenceCount > 0)
                        //{
                        //    throw new GameFrameworkException(Utility.Text.Format("Asset target '{0}' reference count is '{1}' larger than 0.", Name, targetReferenceCount.ToString()));
                        //}

                        //foreach (var dependencyAsset in m_DependencyAssets)
                        //{
                        //    int referenceCount = 0;
                        //if (m_ResourceLoader.m_AssetDependencyCount.TryGetValue(dependencyAsset, out referenceCount))
                        //{
                        //    m_ResourceLoader.m_AssetDependencyCount[dependencyAsset] = referenceCount - 1;
                        //}
                        //else
                        //{
                        //    throw new GameFrameworkException(Utility.Text.Format("Asset target '{0}' dependency asset reference count is invalid.", Name));
                        //}
                        //}
                        Log.Info($"Asset unloaded '{Name}'");
                        m_ResourceLoader.m_ResourcePool.Unspawn(m_Resource);
                    }

                    foreach (var dependencyAsset in m_DependencyAssets)
                    {
                        dependencyAsset.m_BeDependencyAssets.Remove(this);
                    }

                    //m_ResourceLoader.m_AssetDependencyCount.Remove(this);
                    //m_ResourceHelper.Release(Target);
                }

                /// <summary>
                /// 不增加自己的引用计数
                /// </summary>
                public void SpawnDependencies(HashSet<AssetObject> checkCircleDepend)
                {

                    foreach (var dependencyAsset in m_DependencyAssets)
                    {
                        if (checkCircleDepend.Contains(dependencyAsset))
                            continue;
                        checkCircleDepend.Add(dependencyAsset);
                        m_ResourceLoader.m_AssetPool.Spawn(dependencyAsset.Name);
                        dependencyAsset.SpawnDependencies(checkCircleDepend);
                    }
                }

                public override IEnumerable<ObjectBase> GetBeDependencyResources()
                {
                    return BeDependencyAssets;
                }

            }
        }
    }
}
