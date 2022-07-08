//-----------------------------------------------------------------------
// Created By 甘道夫
// contact E-mail: wwei@ND.com
// Date: 2020-09-11
// 本文件中为资源加载器的资源二次包装
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using ND.Managers.ResourceMgr.Framework;
using ND.Managers.ResourceMgr.Framework.Resource;

namespace ND.Managers.ResourceMgr.Runtime
{
    /// <summary>
    /// 资源管理器管理的资源抽象
    /// </summary>
    public class Res
    {
        /// <summary>
        /// 资源名
        /// </summary>
        public string ResName { get; private set; }

        /// <summary>
        /// 加载到的资源
        /// </summary>
        public object asset { get; private set; }


        /// <summary>
        /// 资源加载进度 0~1
        /// </summary>
        public float progress { get; private set; }


        /// <summary>
        /// 是否正在加载中
        /// </summary>
        private bool m_loading = false;

        private bool m_loaded;
        /// <summary>
        /// 是否已经被卸载
        /// </summary>
        private bool m_unloaded = false;


        private Type m_type = null;


        private ICancelable m_loadHandler;

        private bool m_rapidly = false;

        public bool IsRapidly
        {
            get
            {
                return m_rapidly;
            }
        }

        public ResState state
        {
            get
            {
                if (!m_loading)
                {
                    if (m_unloaded)
                    {
                        return ResState.unloaded;
                    }
                    else if (m_loaded)
                    {
                        return ResState.loaded;
                    }
                    else
                    {
                        return ResState.waitForLoad;
                    }
                }
                else
                {
                    return ResState.loading;
                }
            }
        }


        /// <summary>
        /// 资源加载回调
        /// </summary>
        /// <param name="success">加载是否成功</param>
        /// <param name="assetName">资源名</param>
        /// <param name="asset">资源原始asset</param>
        public delegate void ResLoadCompleteCallBack(bool success, string assetName, object asset);

        /// <summary>
        /// 当前关注的回调列表，触发后的回调会被清空
        /// </summary>

        private Stack<ResLoadCompleteCallBack> callBacks;

        private Stack<ResLoadCompleteCallBack> callBacksInternal;

        /// <summary>
        /// true: 使用中，false: 在对象池中
        /// </summary>
        private bool used;
        //优化 GC
        private LoadAssetSuccessCallback cachedLoadAssetSuccessCallback;
        private LoadAssetFailureCallback cachedLoadAssetFailureCallback;


        public Res()
        {
            cachedLoadAssetSuccessCallback = loadAssetSuccessCallback;
            cachedLoadAssetFailureCallback = loadAssetFailureCallback;

        }

        public Res(string name, Type type, bool rapidly)
            : this()
        {
            ResName = name;
            progress = 0;
            m_type = type;
            m_rapidly = rapidly;
        }


        public Type Type { get => m_type; }

        /// <summary>
        /// 注册资源加载结束回调
        /// </summary>
        /// <param name="callBack">回调方法</param>
        public void RegisterLoadCompleteCallBack(ResLoadCompleteCallBack callBack)
        {
            if (callBack == null)
                return;
            if (callBacks == null) callBacks = new Stack<ResLoadCompleteCallBack>();
            callBacks.Push(callBack);
        }

        /// <summary>
        /// 注册资源加载结束回调
        /// </summary>
        /// <param name="callBack">回调方法</param>
        public void RegisterLoadCompleteCallBackInternal(ResLoadCompleteCallBack callBack)
        {
            if (callBack == null)
                return;
            if (callBacksInternal == null) callBacksInternal = new Stack<ResLoadCompleteCallBack>();
            callBacksInternal.Push(callBack);

        }


        /// <summary>
        /// 开始加载
        /// </summary>
        public virtual void Load()
        {
            if (m_loading) return;
            if (asset == null)
            {
                m_loading = true;

                m_loadHandler = NFUResource.LoadAssetAsync(ResName, m_type, cachedLoadAssetSuccessCallback,
                    cachedLoadAssetFailureCallback);
            }
            else
            {
                loadAssetSuccessCallback(ResName, asset, 0, null);
            }
        }
        /// <summary>
        /// 同步加载
        /// </summary>
        public UnityEngine.Object LoadAssetImmediate(string assetName, Type assetType)
        {
            if (asset == null)
            {
                asset = ResourceEntry.Resource.LoadAssetImmediate(assetName, assetType);
                loadAssetSuccessCallback(assetName, asset, 0, null);
            }
            return (UnityEngine.Object)asset;
        }


        /// <summary>
        /// 卸载该资源
        /// </summary>
        public void Unload(bool withFailCallback = false)
        {
            m_unloaded = true;

            if (m_loadHandler != null)
            {
                //还没加载完，则直接取消加载，并返回失败                
                m_loadHandler.Cancel();
                m_loadHandler = null;
                if (withFailCallback)
                    loadAssetFailureCallback(ResName, LoadResourceStatus.NotReady, "Unload On Loading", null);
            }


            if (m_loaded)
            {
                //已加载完，则卸载
                ResourceEntry.Resource.UnloadAsset(ResName, m_type);
            }


            ResName = null;
            asset = null;
            progress = 0;
            m_loading = false;
            m_type = null;
            m_rapidly = false;
            m_loaded = false;

            callBacks?.Clear();
            callBacksInternal?.Clear();

            if (used)
            {
                ObjectPool<Res>.Release(this);
            }
        }


        private void loadAssetSuccessCallback(string assetName, object asset, float duration, object userData)
        {
            m_loading = false;
            m_loaded = true;
            if (m_loadHandler != null)
            {
                m_loadHandler.Cancel();
                m_loadHandler = null;
            }
            this.asset = asset;

            //先触发用户事件
            if (callBacks != null)
            {

                Utility.Profiler.BeginSample("Res.loadAssetSuccessCallback");
                while (callBacks.Count > 0)
                {
                    callBacks.Pop()?.Invoke(true, ResName, asset);
                }
                Utility.Profiler.EndSample();
            }

            //再触发内部事件
            if (callBacksInternal != null)
            {
                Utility.Profiler.BeginSample("Res.loadAssetSuccessCallback.Internal");
                while (callBacksInternal.Count > 0)
                {
                    callBacksInternal.Pop()?.Invoke(true, ResName, asset);
                }
                Utility.Profiler.EndSample();
            }

        }

        private void loadAssetFailureCallback(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            m_loading = false;
            m_loaded = true;
            if (m_loadHandler != null)
            {
                m_loadHandler.Cancel();
                m_loadHandler = null;
            }
            Log.Warning(assetName + " Load Failed :" + errorMessage);
            asset = null;
            if (callBacks != null)
            {
                Utility.Profiler.BeginSample("Res.loadAssetFailureCallback");
                while (callBacks.Count > 0)
                {
                    callBacks.Pop()?.Invoke(false, ResName, asset);
                }
                Utility.Profiler.EndSample();
            }

            if (callBacksInternal != null)
            {
                Utility.Profiler.BeginSample("Res.loadAssetFailureCallback.Internal");
                while (callBacksInternal.Count > 0)
                {
                    callBacksInternal.Pop()?.Invoke(false, ResName, asset);
                }
                Utility.Profiler.EndSample();
            }


        }

        // private void loadAssetUpdateCallback(string assetName, float progress, object userData)
        // {
        //     this.progress = progress;
        // }

        public static Res Create(string name, Type type, bool rapidly)
        {
            Res res = ObjectPool<Res>.Get();
            res.ResName = name;
            res.m_type = type;
            res.m_rapidly = rapidly;
            return res;
        }
        [UnityEngine.Scripting.Preserve]
        class ResPoolProvider : ObjectPool<Res>.IProvider
        {
            public int Priority => 0;

            public Res Create()
            {
                return new Res();
            }

            public void Release(Res obj)
            {
                obj.used = false;
            }

            public void Use(Res obj, bool newCreate)
            {
                Debug.Assert(obj.used == false);
                obj.used = true;
            }
        }
        [UnityEngine.Scripting.Preserve]
        class ListRes : ObjectPool<List<Res>>.IProvider
        {
            public int Priority => 0;

            /// <summary>
            /// 比默认的 Activator.CreateInstance 性能高
            /// </summary>
            /// <returns></returns>
            public List<Res> Create()
            {
                return new List<Res>();
            }

            /// <summary>
            /// 回收清空列表
            /// </summary>
            public void Release(List<Res> obj)
            {
                obj.Clear();
            }

            public void Use(List<Res> obj, bool newCreate)
            {
            }
        }

    }
}