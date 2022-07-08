//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using UnityEngine;
using ND.Managers.ResourceMgr.Framework.FileSystem;
using ND.Managers.ResourceMgr.Runtime;

namespace ND.Managers.ResourceMgr.Framework.Resource
{
    public sealed partial class ResourceManager : GameFrameworkModule, IResourceManager
    {
        public sealed partial class ResourceLoader
        {
            /// <summary>
            /// 加载资源代理。
            /// </summary>
            private sealed partial class LoadResourceAgent : ITaskAgent<LoadResourceTaskBase>
            {
                private static readonly Dictionary<string, string> s_CachedResourceNames = new Dictionary<string, string>(StringComparer.Ordinal);
                private static readonly HashSet<string> s_LoadingAssetNames = new HashSet<string>(StringComparer.Ordinal);
                private static readonly HashSet<string> s_LoadingResourceNames = new HashSet<string>(StringComparer.Ordinal);

                private readonly ILoadResourceAgentHelper m_Helper;
                private readonly IResourceHelper m_ResourceHelper;
                private readonly ResourceLoader m_ResourceLoader;
                private readonly string m_ReadOnlyPath;
                private readonly string m_ReadWritePath;
                private readonly DecryptResourceCallback m_DecryptResourceCallback;
                private LoadResourceTaskBase m_Task;
                private bool resourceObjectReady = false;


                public IResourceHelper ResourceHelper
                {
                    get => m_ResourceHelper;
                }

                /// <summary>
                /// 初始化加载资源代理的新实例。
                /// </summary>
                /// <param name="loadResourceAgentHelper">加载资源代理辅助器。</param>
                /// <param name="resourceHelper">资源辅助器。</param>
                /// <param name="resourceLoader">加载资源器。</param>
                /// <param name="readOnlyPath">资源只读区路径。</param>
                /// <param name="readWritePath">资源读写区路径。</param>
                /// <param name="decryptResourceCallback">解密资源回调函数。</param>
                public LoadResourceAgent(ILoadResourceAgentHelper loadResourceAgentHelper, IResourceHelper resourceHelper, ResourceLoader resourceLoader, string readOnlyPath, string readWritePath, DecryptResourceCallback decryptResourceCallback)
                {
                    if (loadResourceAgentHelper == null)
                    {
                        throw new GameFrameworkException("Load resource agent helper is invalid.");
                    }

                    if (resourceHelper == null)
                    {
                        throw new GameFrameworkException("Resource helper is invalid.");
                    }

                    if (resourceLoader == null)
                    {
                        throw new GameFrameworkException("Resource loader is invalid.");
                    }

                    if (decryptResourceCallback == null)
                    {
                        throw new GameFrameworkException("Decrypt resource callback is invalid.");
                    }

                    m_Helper = loadResourceAgentHelper;
                    m_ResourceHelper = resourceHelper;
                    m_ResourceLoader = resourceLoader;
                    m_ReadOnlyPath = readOnlyPath;
                    m_ReadWritePath = readWritePath;
                    m_DecryptResourceCallback = decryptResourceCallback;
                    m_Task = null;
                }

                public ILoadResourceAgentHelper Helper
                {
                    get
                    {
                        return m_Helper;
                    }
                }

                /// <summary>
                /// 获取加载资源任务。
                /// </summary>
                public LoadResourceTaskBase Task
                {
                    get
                    {
                        return m_Task;
                    }
                }

                /// <summary>
                /// 初始化加载资源代理。
                /// </summary>
                public void Initialize()
                {
                    m_Helper.LoadResourceAgentHelperUpdate += OnLoadResourceAgentHelperUpdate;
                    m_Helper.LoadResourceAgentHelperReadFileComplete += OnLoadResourceAgentHelperReadFileComplete;
                    m_Helper.LoadResourceAgentHelperReadBytesComplete += OnLoadResourceAgentHelperReadBytesComplete;
                    m_Helper.LoadResourceAgentHelperParseBytesComplete += OnLoadResourceAgentHelperParseBytesComplete;
                    m_Helper.LoadResourceAgentHelperLoadComplete += OnLoadResourceAgentHelperLoadComplete;
                    m_Helper.LoadResourceAgentHelperError += OnLoadResourceAgentHelperError;
                }

                /// <summary>
                /// 加载资源代理轮询。
                /// </summary>
                /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
                /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
                public void Update(float elapseSeconds, float realElapseSeconds)
                {
                    if (m_Task.IsCanceled)
                    {
                        Log.Info($"Task canceled. Id: {m_Task.SerialId}");
                        m_Task.Done = true;
                        return;
                    }


                    if (m_Task is LoadAssetTask)
                    {
                        if (!m_Task.ResourceObject.IsLoaded || !m_Task.AssetObject.IsDependencyAssetLoaded)
                            return;
                    }
                    else
                    {
                        if (!m_Task.ResourceObject.IsResourceLoaded)
                            return;
                    }

                    if (!resourceObjectReady)
                        OnResourceObjectReady(m_Task.AssetObject.Resource);

                    if (m_Task.AssetObject.IsLoaded)
                    {
                        m_Task.Done = true;
                    }
                }

                /// <summary>
                /// 关闭并清理加载资源代理。
                /// </summary>
                public void Shutdown()
                {
                    Reset();
                    m_Helper.LoadResourceAgentHelperUpdate -= OnLoadResourceAgentHelperUpdate;
                    m_Helper.LoadResourceAgentHelperReadFileComplete -= OnLoadResourceAgentHelperReadFileComplete;
                    m_Helper.LoadResourceAgentHelperReadBytesComplete -= OnLoadResourceAgentHelperReadBytesComplete;
                    m_Helper.LoadResourceAgentHelperParseBytesComplete -= OnLoadResourceAgentHelperParseBytesComplete;
                    m_Helper.LoadResourceAgentHelperLoadComplete -= OnLoadResourceAgentHelperLoadComplete;
                    m_Helper.LoadResourceAgentHelperError -= OnLoadResourceAgentHelperError;
                }

                public static void Clear()
                {
                    s_CachedResourceNames.Clear();
                    s_LoadingAssetNames.Clear();
                    s_LoadingResourceNames.Clear();
                }

                /*
                /// <summary>
                /// 开始处理加载资源任务。
                /// </summary>
                /// <param name="task">要处理的加载资源任务。</param>
                /// <returns>开始处理任务的状态。</returns>
                public StartTaskStatus Start(LoadResourceTaskBase task)
                {
                    if (task == null)
                    {
                        throw new GameFrameworkException("Task is invalid.");
                    }

                    startFrameCount = Time.frameCount;
                    dirtyResourceObject = null;

                    m_Task = task;
                    m_Task.StartTime = DateTime.Now;
                    ResourceInfo resourceInfo = m_Task.ResourceInfo;

                    if (!resourceInfo.Ready)
                    {
                        m_Task.StartTime = default(DateTime);
                        return StartTaskStatus.HasToWait;
                    }


                    //资源是否加载锁定中
                    if (IsAssetLoading(m_Task.AssetName))
                    {
                        m_Task.StartTime = default(DateTime);
                        return StartTaskStatus.HasToWait;
                    }

                    if (!m_Task.IsScene)
                    {
                        if (m_Task.AssetObject.IsLoaded)
                        {
                            OnAssetObjectReady(m_Task.AssetObject);
                            return StartTaskStatus.Done;
                        }
                    }

                    foreach (var dependencyAsset in m_Task.AssetObject.DependencyAssets)
                    {
                        if (!dependencyAsset.IsLoaded)
                        {
                            m_Task.StartTime = default(DateTime);
                            return StartTaskStatus.HasToWait;
                        }
                    }

                    string resourceName = resourceInfo.ResourceName.Name;
                    //资源包是否加载锁定中
                    if (IsResourceLoading(resourceName))
                    {
                        m_Task.StartTime = default(DateTime);
                        return StartTaskStatus.HasToWait;
                    }

                    s_LoadingAssetNames.Add(m_Task.AssetName);


                    if (m_Task.ResourceObject.IsLoaded)
                    {
                        OnResourceObjectReady(m_Task.ResourceObject);
                        return StartTaskStatus.CanResume;
                    }

                    s_LoadingResourceNames.Add(resourceName);
                    m_Task.ResourceObject.OnLoading(true);


                    string fullPath = null;
                    if (!s_CachedResourceNames.TryGetValue(resourceName, out fullPath))
                    {
                        fullPath = Utility.Path.GetRegularPath(Path.Combine(resourceInfo.StorageInReadOnly ? m_ReadOnlyPath : m_ReadWritePath, resourceInfo.UseFileSystem ? resourceInfo.FileSystemName : resourceInfo.ResourceName.FullName));
                        s_CachedResourceNames.Add(resourceName, fullPath);
                    }

                    if (resourceInfo.LoadType == LoadType.LoadFromFile)
                    {
                        if (resourceInfo.UseFileSystem)
                        {
                            IFileSystem fileSystem = m_ResourceLoader.m_ResourceManager.GetFileSystem(resourceInfo.FileSystemName, resourceInfo.StorageInReadOnly);
                            m_Helper.ReadFile(fileSystem, resourceInfo.ResourceName.FullName,m_Task.IsRapidly);
                        }
                        else
                        {
                            m_Helper.ReadFile(fullPath,m_Task.IsRapidly);
                        }
                    }
                    else if (resourceInfo.LoadType == LoadType.LoadFromMemory || resourceInfo.LoadType == LoadType.LoadFromMemoryAndQuickDecrypt || resourceInfo.LoadType == LoadType.LoadFromMemoryAndDecrypt)
                    {
                        if (resourceInfo.UseFileSystem)
                        {
                            IFileSystem fileSystem = m_ResourceLoader.m_ResourceManager.GetFileSystem(resourceInfo.FileSystemName, resourceInfo.StorageInReadOnly);
                            m_Helper.ReadBytes(fileSystem, resourceInfo.ResourceName.FullName,m_Task.IsRapidly);
                        }
                        else
                        {
                            m_Helper.ReadBytes(fullPath,m_Task.IsRapidly);
                        }
                    }
                    else
                    {
                        throw new GameFrameworkException(Utility.Text.Format("Resource load type '{0}' is not supported.", resourceInfo.LoadType.ToString()));
                    }

                    return StartTaskStatus.CanResume;
                }
                */

                /// <summary>
                /// 开始处理加载资源任务。
                /// </summary>
                /// <param name="task">要处理的加载资源任务。</param>
                /// <returns>开始处理任务的状态。</returns>
                public StartTaskStatus Start(LoadResourceTaskBase task)
                {
                    if (task == null)
                    {
                        throw new GameFrameworkException("Task is invalid.");
                    }
                    m_Task = task;
                    m_Task.StartTime = DateTime.Now;

                    if (task.IsCanceled)
                    {
                        task.Done = true;
                        Log.Info($"Task canceled. Id: {m_Task.SerialId}");
                        return StartTaskStatus.Done;
                    }

                    if (StartLoadResource())
                    {
                        if (!m_Task.Done)
                        {
                            if (m_Task.ResourceObject.IsLoaded)
                            {
                                if (!m_Task.AssetObject.IsLoaded)
                                {
                                    if (!m_Task.IsScene)
                                    {
                                        OnAssetObjectReady(m_Task.AssetObject);
                                    }
                                    m_Task.Done = true;
                                    return StartTaskStatus.Done;
                                }
                            }
                        }

                        if (m_Task.Done || m_Task.AssetObject.IsLoaded)
                        {
                            return StartTaskStatus.Done;
                        }
                        return StartTaskStatus.CanResume;
                    }

                    if (m_Task.Done)
                        return StartTaskStatus.Done;

                    if (m_Task.AssetObject.IsLoaded)
                    {
                        if (!m_Task.IsScene)
                        {
                            OnAssetObjectReady(m_Task.AssetObject);
                        }
                        else
                        {
                            OnResourceObjectReady(m_Task.AssetObject.Resource);
                        }
                        m_Task.Done = true;
                        return StartTaskStatus.Done;
                    }

                    //资源是否加载锁定中
                    if (IsAssetLoading(m_Task.AssetName))
                    {
                        m_Task.StartTime = default(DateTime);
                        return StartTaskStatus.HasToWait;
                    }

                    if (m_Task.ResourceObject.IsLoaded)
                    {
                        //Asset 加载异步或同步
                        s_LoadingAssetNames.Add(m_Task.AssetName);
                        OnResourceObjectReady(m_Task.AssetObject.Resource);
                        return StartTaskStatus.CanResume;
                    }
                    return StartTaskStatus.HasToWait;
                }

                /// <summary>
                /// 加载资源包
                /// </summary>
                bool StartLoadResource()
                {
                    if (m_Task.ResourceObject.IsResourceLoaded)
                        return false;

                    ResourceInfo resourceInfo = m_Task.ResourceInfo;
                    string resourceName = resourceInfo.ResourceName.Name;

                    if (!resourceInfo.Ready)
                    {
                        m_Task.StartTime = default(DateTime);
                        return false;
                    }

                    //资源包是否加载锁定中
                    if (IsResourceLoading(resourceName))
                    {
                        m_Task.StartTime = default(DateTime);
                        return false;
                    }
                    //如果Resource是同步加载，Asset需要提前加锁
                    s_LoadingAssetNames.Add(m_Task.AssetName);
                    s_LoadingResourceNames.Add(resourceName);

                    //加载资源包
                    m_Task.ResourceObject.OnLoading(true);


                    string fullPath = null;
                    if (!s_CachedResourceNames.TryGetValue(resourceName, out fullPath))
                    {
                        fullPath = Utility.Path.GetRegularPath(Path.Combine(resourceInfo.StorageInReadOnly ? m_ReadOnlyPath : m_ReadWritePath, resourceInfo.UseFileSystem ? resourceInfo.FileSystemName : resourceInfo.ResourceName.FullName));
                        s_CachedResourceNames.Add(resourceName, fullPath);
                    }

                    if (resourceInfo.LoadType == LoadType.LoadFromFile)
                    {
                        if (resourceInfo.UseFileSystem)
                        {
                            IFileSystem fileSystem = m_ResourceLoader.m_ResourceManager.GetFileSystem(resourceInfo.FileSystemName, resourceInfo.StorageInReadOnly);
                            m_Helper.ReadFile(fileSystem, resourceInfo.ResourceName.FullName, m_Task.IsRapidly);
                        }
                        else
                        {
                            m_Helper.ReadFile(fullPath, m_Task.IsRapidly);
                        }
                    }
                    else if (resourceInfo.LoadType == LoadType.LoadFromMemory || resourceInfo.LoadType == LoadType.LoadFromMemoryAndQuickDecrypt || resourceInfo.LoadType == LoadType.LoadFromMemoryAndDecrypt)
                    {
                        if (resourceInfo.UseFileSystem)
                        {
                            IFileSystem fileSystem = m_ResourceLoader.m_ResourceManager.GetFileSystem(resourceInfo.FileSystemName, resourceInfo.StorageInReadOnly);
                            m_Helper.ReadBytes(fileSystem, resourceInfo.ResourceName.FullName, m_Task.IsRapidly);
                        }
                        else
                        {
                            m_Helper.ReadBytes(fullPath, m_Task.IsRapidly);
                        }
                    }
                    else
                    {
                        throw new GameFrameworkException(Utility.Text.Format("Resource load type '{0}' is not supported.", resourceInfo.LoadType.ToString()));
                    }

                    return true;
                }

                /// <summary>
                /// 重置加载资源代理。
                /// </summary>
                public void Reset()
                {
                    m_Helper.Reset();
                    m_Task = null;
                    resourceObjectReady = false;
                }

                private static bool IsAssetLoading(string assetName)
                {
                    return s_LoadingAssetNames.Contains(assetName);
                }

                private static bool IsResourceLoading(string resourceName)
                {
                    return s_LoadingResourceNames.Contains(resourceName);
                }

                private void OnAssetObjectReady(AssetObject assetObject)
                {
                    object asset = assetObject.Target;
                    if (m_Task.IsScene)
                    {
                        if(!m_ResourceLoader.m_SceneToAssetMap.ContainsKey(m_Task.AssetName))
                            m_ResourceLoader.m_SceneToAssetMap.Add(m_Task.AssetName, assetObject);
                    }

                    try
                    {
                        m_Task.OnLoadAssetSuccess(this, assetObject, (float)(DateTime.Now - m_Task.StartTime).TotalSeconds);
                        m_Task.Done = true;
                    }
                    catch (Exception e)
                    {
#if !UNITY_EDITOR
                        m_Task.Done = true;
#endif
                        Log.Error("asset: " + assetObject.Name);
                        throw e;
                    }
                }

                private void OnResourceObjectReady(ResourceObject resourceObject)
                {
                    if (!resourceObjectReady)
                    {
                        resourceObjectReady = true;
                        m_Task.LoadMain(this, resourceObject);
                    }
                }

                private void OnError(LoadResourceStatus status, string errorMessage)
                {
                    m_Helper.Reset();

                    try
                    {
                        m_Task.OnLoadAssetFailure(this, status, errorMessage);
                        s_LoadingAssetNames.Remove(m_Task.AssetName);
                        s_LoadingResourceNames.Remove(m_Task.ResourceInfo.ResourceName.Name);
                        m_Task.Done = true;
                    }
                    catch (Exception e)
                    {
#if !UNITY_EDITOR
                        s_LoadingAssetNames.Remove(m_Task.AssetName);
                        s_LoadingResourceNames.Remove(m_Task.ResourceInfo.ResourceName.Name);
                        m_Task.Done = true;
#endif
                        throw e;
                    }

                }

                private void OnLoadResourceAgentHelperUpdate(object sender, LoadResourceAgentHelperUpdateEventArgs e)
                {
                    m_Task.OnLoadAssetUpdate(this, e.Type, e.Progress);
                }

                private void OnLoadResourceAgentHelperReadFileComplete(object sender, LoadResourceAgentHelperReadFileCompleteEventArgs e)
                {
                    ResourceObject resourceObject = m_Task.ResourceObject;
                    resourceObject.OnLoaded(e.Resource);
                    s_LoadingResourceNames.Remove(m_Task.ResourceInfo.ResourceName.Name);
                    if (resourceObject.IsLoaded)
                    {
                        OnResourceObjectReady(resourceObject);
                    }
                }

                private void OnLoadResourceAgentHelperReadBytesComplete(object sender, LoadResourceAgentHelperReadBytesCompleteEventArgs e)
                {
                    byte[] bytes = e.GetBytes();
                    ResourceInfo resourceInfo = m_Task.ResourceInfo;
                    if (resourceInfo.LoadType == LoadType.LoadFromMemoryAndQuickDecrypt || resourceInfo.LoadType == LoadType.LoadFromMemoryAndDecrypt)
                    {
                        m_DecryptResourceCallback(bytes, 0, bytes.Length, resourceInfo.ResourceName.Name, resourceInfo.ResourceName.Variant, resourceInfo.ResourceName.Extension, resourceInfo.StorageInReadOnly, resourceInfo.FileSystemName, (byte)resourceInfo.LoadType, resourceInfo.Length, resourceInfo.HashCode);
                    }

                    m_Helper.ParseBytes(bytes, e.IsRapidly);
                }

                private void OnLoadResourceAgentHelperParseBytesComplete(object sender, LoadResourceAgentHelperParseBytesCompleteEventArgs e)
                {
                    ResourceObject resourceObject = m_Task.ResourceObject;
                    resourceObject.OnLoaded(e.Resource);
                    s_LoadingResourceNames.Remove(m_Task.ResourceInfo.ResourceName.Name);

                    if (resourceObject.IsLoaded)
                    {
                        OnResourceObjectReady(resourceObject);
                    }
                }

                private void OnLoadResourceAgentHelperLoadComplete(object sender, LoadResourceAgentHelperLoadCompleteEventArgs e)
                {
                    AssetObject assetObject = m_Task.AssetObject;
                    //if (m_Task.IsScene)
                    //{
                    //    assetObject = m_ResourceLoader.m_AssetPool.Spawn(m_Task.AssetFullName);
                    //}

                    //if (assetObject == null)
                    {

                        var asset = e.Asset;
                        if (e.LoadResourceAll && m_Task.AssetName.EndsWith(".spriteatlas"))
                        {
                            var assetNames = ResourceEntry.Resource.ResourceManager.GetResourceAllAssets(m_Task.ResourceObject.Name);
                            //
                            for (int i = 0; i < assetNames.Length; i++)
                            {
                                if (assetNames[i].Equals(m_Task.AssetName))
                                {
                                    var abTarget = m_Task.ResourceObject.Target as AssetBundle;
                                    asset = abTarget.LoadAsset(assetNames[i]);
                                    break;
                                }
                            }
                        }

                        //由于依赖问题，暂时注释，这里可以加速加载精灵时加载图集的速度
                        // if (e.LoadResourceAll)
                        // {
                        //     var abTarget = m_Task.ResourceObject.Target  as AssetBundle;
                        //
                        //     var assetNames = ResourceV2Entry.Resource.ResourceManager.GetResourceAllAssets(m_Task.ResourceObject.Name);
                        //
                        //     for (int i = 0; i < assetNames.Length; i++)
                        //     {
                        //         if (assetNames[i].Equals(m_Task.AssetName))
                        //         {
                        //             var assetTmp = abTarget.LoadAsset(assetNames[i]);
                        //             assetObject = AssetObject.Create(m_Task.AssetName, assetTmp, new object[0], dependencyAssets,
                        //                 m_Task.ResourceObject.Target, m_ResourceHelper, m_ResourceLoader);
                        //             m_ResourceLoader.m_AssetPool.Register(assetObject, true);
                        //             m_ResourceLoader.m_AssetToResourceMap.Add(e.Asset, m_Task.ResourceObject.Target);
                        //         }
                        //         else
                        //         {
                        //
                        //             var assetTmp = abTarget.LoadAsset(assetNames[i]);
                        //             if (assetTmp != null)
                        //             {
                        //                 AssetObject assetObjectTmp;
                        //                 if (!m_ResourceLoader.m_AssetPool.CanSpawn(assetNames[i]))
                        //                 {
                        //                     assetObjectTmp = AssetObject.Create(assetNames[i], assetTmp,
                        //                         new object[0],
                        //                         new List<object>(0), m_Task.ResourceObject.Target, m_ResourceHelper,
                        //                         m_ResourceLoader);
                        //                 }
                        //                 else
                        //                 {
                        //                     assetObjectTmp = m_ResourceLoader.m_AssetPool.Spawn(assetNames[i]);
                        //                 }
                        //
                        //                 m_ResourceLoader.m_AssetPool.Register(assetObjectTmp, true);
                        //                 m_ResourceLoader.m_AssetToResourceMap.Add(assetTmp,
                        //                     m_Task.ResourceObject.Target);
                        //                 
                        //                
                        //             }
                        //
                        //             
                        //         }
                        //     }
                        // }
                        // else
                        // {


                        if (!assetObject.IsAssetLoaded)
                        {
                            assetObject.OnLoaded(asset, e.SubAsset);
                        }
                    }

                    s_LoadingAssetNames.Remove(m_Task.AssetName);
                    OnAssetObjectReady(assetObject);
                }

                private void OnLoadResourceAgentHelperError(object sender, LoadResourceAgentHelperErrorEventArgs e)
                {
                    OnError(e.Status, e.ErrorMessage);
                }


            }

        }
        public static string GetAssetFullName(string assetName, Type type)
        {
            //优化GC
            if (type == null || type == typeof(UnityEngine.Object))
                return assetName;
            return assetName;
        }
    }
}
