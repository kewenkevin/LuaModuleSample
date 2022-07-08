//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ND.Managers.ResourceMgr.Framework.ObjectPool;
using ND.Managers.ResourceMgr.Runtime;

namespace ND.Managers.ResourceMgr.Framework.Resource
{
    public sealed partial class ResourceManager : GameFrameworkModule, IResourceManager
    {
        public sealed partial class ResourceLoader
        {
            /// <summary>
            /// 资源对象。
            /// </summary>
            public sealed class ResourceObject : ObjectBase
            {
                private List<ResourceObject> m_DependencyResources;
                private IResourceHelper m_ResourceHelper;
                private ResourceLoader m_ResourceLoader;
                private ResourceInfo m_ResourceInfo;
                private HashSet<ResourceObject> m_BeDependencyResources;

                public ResourceObject()
                {
                    m_DependencyResources = new List<ResourceObject>();
                    m_BeDependencyResources = new HashSet<ResourceObject>();
                    m_ResourceHelper = null;
                    m_ResourceLoader = null;
                }

                public override bool CustomCanReleaseFlag
                {
                    get
                    {
                        //int targetReferenceCount = 0;
                        //m_ResourceLoader.m_ResourceDependencyCount.TryGetValue(this, out targetReferenceCount);
                        //return base.CustomCanReleaseFlag && targetReferenceCount <= 0;
                        return base.CustomCanReleaseFlag && m_BeDependencyResources.Count == 0;
                    }
                }

                internal ResourceInfo ResourceInfo { get => m_ResourceInfo; set => m_ResourceInfo = value; }

                public bool IsLoadAsync { get; private set; }

                /// <summary>
                /// Resource 是否已加载，但不表示依赖 Resource 已加载
                /// </summary>
                public bool IsResourceLoaded
                {
                    get; private set;
                }

                /// <summary>
                /// 依赖的 Resource 是否已加载
                /// </summary>
                public bool IsDependencyResourceLoaded
                {
                    get
                    {
                        foreach (var depResource in m_DependencyResources)
                        {
                            if (!depResource.IsResourceLoaded)
                                return false;
                        }
                        return true;
                    }
                }

                /// <summary>
                /// Resource 和依赖的 Resource 都已加载
                /// </summary>
                public override bool IsLoaded
                {
                    get
                    {
                        if (!base.IsLoaded)
                        {
                            if (IsResourceLoaded && IsDependencyResourceLoaded)
                            {
                                IsLoaded = true;
                            }
                        }
                        return base.IsLoaded;
                    }
                    protected set => base.IsLoaded = value;
                }

                /// <summary>
                /// 依赖的资源
                /// </summary>
                public IEnumerable<ResourceObject> DependencyResources
                {
                    get => m_DependencyResources;
                }

                /// <summary>
                /// 被依赖
                /// </summary>
                public HashSet<ResourceObject> BeDependencyResources { get => m_BeDependencyResources; }

                public static ResourceObject Create(string name, object target, IResourceHelper resourceHelper, ResourceLoader resourceLoader)
                {
                    return Create(name, target, resourceHelper, resourceLoader, true);
                }

                public static ResourceObject Create(string name, IResourceHelper resourceHelper, ResourceLoader resourceLoader)
                {
                    return Create(name, null, resourceHelper, resourceLoader, false);
                }

                private static ResourceObject Create(string name, object target, IResourceHelper resourceHelper, ResourceLoader resourceLoader, bool loaded)
                {
                    if (resourceHelper == null)
                    {
                        throw new GameFrameworkException("Resource helper is invalid.");
                    }

                    if (resourceLoader == null)
                    {
                        throw new GameFrameworkException("Resource loader is invalid.");
                    }

                    ResourceObject resourceObject = ReferencePool.Acquire<ResourceObject>();
                    resourceObject.Initialize(name, target);
                    resourceObject.m_ResourceHelper = resourceHelper;
                    resourceObject.m_ResourceLoader = resourceLoader;
                    resourceObject.IsResourceLoaded = loaded;
                    resourceObject.IsLoaded = false;
                    // if (name.EndsWith("_spriteatlas"))
                    // {
                    //     var abTarget = target as AssetBundle;
                    //     if (abTarget != null&&ResourceV2Entry.Resource!=null)
                    //     {
                    //         var atlas = abTarget.LoadAllAssets<SpriteAtlas>();
                    //         foreach (var sa in atlas)
                    //         {
                    //             ResourceV2Entry.Resource.RegisterSpriteAtlas(sa);
                    //         }
                    //     }
                    // }

                    //Log.Info($"Resource create '{name}'");
                    Utility.OnCreateResourceObject(resourceObject);
                    return resourceObject;
                }

                public void OnLoading(bool isLoadAsync)
                {
                    Log.Info($"Resource loading '{Name}'");
                    IsLoading = true;
                    IsResourceLoaded = false;
                    IsLoaded = false;
                    IsLoadAsync = isLoadAsync;
                    ResourceEntry.Resource.ResourceManager.ResourcePool.SetTarget(this, null);
                    Target = null;
                }

                public void OnLoaded(object target)
                {
                    if (IsResourceLoaded)
                        Log.Warning($"Resource already loaded {Name}");
                    else
                        Log.Info($"Resource loaded '{Name}'");
                    ResourceEntry.Resource.ResourceManager.ResourcePool.SetTarget(this, target);
                    Target = target;
                    IsLoading = false;
                    IsResourceLoaded = true;
#if UNITY_EDITOR                   
                    //修复编辑器模式加载AssetBundle的材质球                    
                    if (ResourceSettings.EditorModeReloadShader)
                    {
                        var assetBundle = target as AssetBundle;
                        if (assetBundle && !assetBundle.isStreamedSceneAssetBundle)
                        {
                            foreach (var mat in assetBundle.LoadAllAssets<Material>())
                            {
                                if (mat.shader)
                                {
                                    var shader = Shader.Find(mat.shader.name);
                                    if (shader)
                                    {
                                        mat.shader = shader;
                                    }
                                }
                            }
                        }
                    }
#endif


                    Utility.OnLoadResourceCompleted();
                }

                public T[] LoadAllAssets<T>() where T : Object
                {
                    var abTarget = Target as AssetBundle;
                    if (abTarget != null)
                    {
                        return abTarget.LoadAllAssets<T>();
                    }
                    return new T[0];
                }

                public override IEnumerable<ObjectBase> GetBeDependencyResources()
                {
                    return BeDependencyResources;
                }

                public override void Clear()
                {
                    base.Clear();
                    m_DependencyResources.Clear();
                    m_BeDependencyResources.Clear();
                    m_ResourceHelper = null;
                    m_ResourceLoader = null;
                    IsLoadAsync = false;
                    IsResourceLoaded = false;
                }


                public void AddDependencyResource(ResourceObject dependencyResource)
                {

                    if (this == dependencyResource)
                    {
                        return;
                    }

                    if (m_DependencyResources.Contains(dependencyResource))
                    {
                        return;
                    }

                    dependencyResource.m_BeDependencyResources.Add(this);

                    m_DependencyResources.Add(dependencyResource);

                    //int referenceCount = 0;
                    //if (m_ResourceLoader.m_ResourceDependencyCount.TryGetValue(dependencyResource, out referenceCount))
                    //{
                    //    m_ResourceLoader.m_ResourceDependencyCount[dependencyResource] = referenceCount + 1;
                    //}
                    //else
                    //{
                    //    m_ResourceLoader.m_ResourceDependencyCount.Add(dependencyResource, 1);
                    //}
                }

                protected internal override void Release(bool isShutdown)
                {
                    Log.Info($"Resource unloaded '{Name}'");
                    foreach (var dependencyResource in m_DependencyResources)
                    {
                        dependencyResource.m_BeDependencyResources.Remove(this);
                    }

                    //if (!isShutdown)
                    //{
                    //int targetReferenceCount = 0;
                    //if (m_ResourceLoader.m_ResourceDependencyCount.TryGetValue(this, out targetReferenceCount) && targetReferenceCount > 0)
                    //{
                    //    throw new GameFrameworkException(Utility.Text.Format("Resource target '{0}' reference count is '{1}' larger than 0.", Name, targetReferenceCount.ToString()));
                    //}

                    //foreach (var dependencyResource in m_DependencyResources)
                    //{
                    //    int referenceCount = 0;
                    //if (m_ResourceLoader.m_ResourceDependencyCount.TryGetValue(dependencyResource, out referenceCount))
                    //{
                    //    m_ResourceLoader.m_ResourceDependencyCount[dependencyResource] = referenceCount - 1;
                    //}
                    //else
                    //{
                    //    throw new GameFrameworkException(Utility.Text.Format("Resource target '{0}' dependency asset reference count is invalid.", Name));
                    //}
                    //}
                    //}

                    //m_ResourceLoader.m_ResourceDependencyCount.Remove(this);
                    m_ResourceHelper.Release(Target);
                }
            }
        }
    }
}
