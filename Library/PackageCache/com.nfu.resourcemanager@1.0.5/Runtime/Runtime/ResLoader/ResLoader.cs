//-----------------------------------------------------------------------
// Created By 甘道夫
// contact E-mail: wwei@ND.com
// Date: 2020-09-11
// 本文件中为资源加载器的实现，提供默认的一种资源管理方式，详情请参见文档或示例代码
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ND.Managers.ResourceMgr.Framework;
using ND.Managers.ResourceMgr.Framework.Resource;

namespace ND.Managers.ResourceMgr.Runtime
{
    /// <summary>
    /// 资源加载器，推荐的资源管理方式，资源用后可以释放该加载器或让加载器回池，加载器将不再引用资源，资源计数分别减一
    /// </summary>
    public class ResLoader : CustomYieldInstruction
    {
        private static int nextId = 1;
        private int id;
        public int Id => id;

#if UNITY_EDITOR
        /// <summary>
        /// 调用堆栈
        /// </summary>
        private string stacktrace;
#endif


        /// <summary>
        /// 已加载的资源情况
        /// </summary>
        private readonly Dictionary<(string, Type), Res> m_ResArray = new Dictionary<(string, Type), Res>();
        private readonly Dictionary<(string, Type), Res> m_WaitingResArray = new Dictionary<(string, Type), Res>();
        private string assetNamePrefix;

        /// <summary>
        /// 所有资源加载完成的回调
        /// </summary>
        private Action m_allFinishListener;
        private bool isDisposed;
        private Res mainAssetRes;

        /// <summary>
        /// 加载中的资源总数
        /// </summary>
        public int loadingCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 第一个添加资源
        /// </summary>
        public object MainAsset
        {
            get
            {
                if (mainAssetRes != null)
                {
                    return mainAssetRes.asset;
                }
                return null;
            }
        }

        /// <summary>
        /// 分配一个资源加载器
        /// </summary>
        /// <returns></returns>
        public static ResLoader Alloc()
        {
            return _Alloc(null);
        }

        /// <summary>
        /// 分配一个资源加载器
        /// </summary>
        /// <param name="assetNamePrefix">资源名前缀</param>        
        public static ResLoader Alloc(string assetNamePrefix)
        {
            return _Alloc(assetNamePrefix);
        }
        private static ResLoader _Alloc(string assetNamePrefix)
        {
            var loader = ResourceEntry.ResLoaderPool.Alloc();
            if (string.IsNullOrEmpty(assetNamePrefix))
                assetNamePrefix = null;
            loader.assetNamePrefix = assetNamePrefix;
#if UNITY_EDITOR
            if ((ResourceSettings.Options & ResourceSettings.ResourceOptions.AutoGCStackTrace) != 0)
            {
                //查找自动GC的ResLoader
                loader.stacktrace = new System.Diagnostics.StackTrace(2, true).ToString();
            }
#endif
            return loader;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        internal ResLoader()
        {
            id = nextId;
            nextId++;
        }

        // /// <summary>
        // /// 当GC回收析构加载器时，加载器取消对所有资源的引用
        // /// </summary>
        ~ResLoader()
        {
#if UNITY_EDITOR
            //记录自动释放的
            if (!isDisposed && stacktrace != null)
            {
                string msg = stacktrace;
                if (m_WaitingResArray != null)
                {
                    foreach (var item in m_WaitingResArray)
                        msg = item.Key + "\n" + msg;
                }
                if (m_ResArray != null)
                {
                    foreach (var item in m_ResArray)
                        msg = item.Key + "\n" + msg;
                }
                Utility.CollectAutoGCs.Add(msg);
            }
#endif
            DisposeOnGC();
        }


        // /// <summary>
        // /// loader添加一个等待加载的资源(急速模式)
        // /// </summary>
        // /// <param name="name">资源名</param>
        // public ResLoader Add2LoadRapid(string name, Res.ResLoadCompleteCallBack listener = null)
        // {
        //     return Add2LoadRapid(name, null, listener);
        // }

        /// <summary>
        /// loader添加一个等待加载的资源
        /// </summary>
        /// <param name="name">资源名</param>
        public ResLoader Add2Load(string name, Res.ResLoadCompleteCallBack listener = null)
        {
            return Add2Load(name, null, listener);
        }

        /// <summary>
        /// loader添加一个等待加载的资源
        /// </summary>
        /// <param name="name">资源名</param>
        public ResLoader Add2Load(string name, Type type, Res.ResLoadCompleteCallBack listener = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                Log.Error("Res Name Is Null.");
                return this;
            }
            if (assetNamePrefix != null)
                name = assetNamePrefix + name;
            var key = GetResKey(name, type);
            if (!m_WaitingResArray.ContainsKey(key))
            {
                m_WaitingResArray.Add(key, Res.Create(name, type, false));
            }
            m_WaitingResArray[key].RegisterLoadCompleteCallBack(listener);
            if (mainAssetRes == null)
                mainAssetRes = m_WaitingResArray[key];
            return this;
        }



        // /// <summary>
        // /// loader添加一个等待加载的资源
        // /// </summary>
        // /// <param name="name">资源名</param>
        // public ResLoader Add2LoadRapid(string name, Type type, Res.ResLoadCompleteCallBack listener = null)
        // {
        //     if (string.IsNullOrEmpty(name))
        //     {
        //         Log.Error("Res Name Is Null.");
        //         return this;
        //     }
        //     if (assetNamePrefix != null)
        //         name = assetNamePrefix + name;
        //
        //     string key = ResourceManager.GetAssetFullName(name, type);
        //     if (!m_WaitingResArray.ContainsKey(key))
        //     {
        //         m_WaitingResArray.Add(key, Res.Create(name, type, true));
        //     }
        //     m_WaitingResArray[key].RegisterLoadCompleteCallBack(listener);
        //     if (mainAssetRes == null)
        //         mainAssetRes = m_WaitingResArray[key];
        //     return this;
        // }

        /// <summary>
        /// 一个资源加载完成后的回调
        /// </summary>
        /// <param name="success"></param>
        /// <param name="asset"></param>
        private void OnLoadOneComplete(bool success, string assetName, Type assetType, object asset)
        {
            var key = GetResKey(assetName, assetType);
            if (m_WaitingResArray.ContainsKey(key))
            {
                if (!m_ResArray.ContainsKey(key))
                    m_ResArray.Add(key, m_WaitingResArray[key]);
                m_WaitingResArray.Remove(key);
                loadingCount--;
            }
            if (loadingCount == 0)
            {
                if (m_allFinishListener != null)
                {
                    Utility.Profiler.BeginSample("ResLoader.OnLoadOneComplete");
                    m_allFinishListener.Invoke();
                    Utility.Profiler.EndSample();
                }
            }
        }

        /// <summary>
        /// 立即加载所有已注册的资源
        /// </summary>
        /// <param name="listener"></param>
        public void Load(Action listener = null)
        {
            m_allFinishListener = listener;
            DoLoadAsync();
        }

        private void DoLoadAsync()
        {
            // loadingCount += m_WaitingResArray.Count;

            var wait = ObjectPool<List<Res>>.Get();

            wait.AddRange(m_WaitingResArray.Values);

            foreach (var res in wait)
            {
                res.RegisterLoadCompleteCallBackInternal((success, assetName, asset) =>
                {
                    OnLoadOneComplete(success, assetName, res.Type, asset);
                });
            }
            loadingCount += wait.Count;
            for (int i = wait.Count - 1; i >= 0; i--)
            {
                var res = wait[i];
                res.Load();
            }

            ObjectPool<List<Res>>.Release(wait);
        }

        /// <summary>
        /// 同步加载
        /// </summary>
        public UnityEngine.Object LoadAssetImmediate(string assetName, Type assetType = null)
        {
            if (assetNamePrefix != null)
                assetName = assetNamePrefix + assetName;
            var key = GetResKey(assetName, assetType);
            if (m_ResArray.ContainsKey(key))
            {
                return m_ResArray[key].asset as UnityEngine.Object;
            }

            var res = Res.Create(assetName, assetType, false);
            var asset = res.LoadAssetImmediate(assetName, assetType);
            m_ResArray.Add(key, res);

            return asset;
        }


        /// <summary>
        /// 同步加载
        /// </summary>
        public T LoadAssetImmediate<T>(string assetName)
            where T : UnityEngine.Object
        {
            return LoadAssetImmediate(assetName, typeof(T)) as T;
        }



        /// <summary>
        /// loader取消对某个资源的引用
        /// </summary>
        /// <param name="name">资源名</param>
        public void ReleaseRes(string name, Type assetType = null)
        {
            if (assetNamePrefix != null)
                name = assetNamePrefix + name;
            var key = GetResKey(name, assetType);
            if (m_ResArray.ContainsKey(key))
            {
                m_ResArray[key].Unload(true);
                m_ResArray.Remove(key);
            }
            else if (m_WaitingResArray.ContainsKey(key))
            {
                m_WaitingResArray[key].Unload(true);
                m_WaitingResArray.Remove(key);
            }
            else
            {
                Log.Warning("You called ReleaseRes but There's no resource named {0} in ResLoader", name);
            }
        }

        /// <summary>
        /// loader取消对所有资源的引用
        /// </summary>
        public void ReleaseAllRes()
        {
            var tmp = ObjectPool<List<Res>>.Get();
            tmp.AddRange(m_ResArray.Values);
            tmp.AddRange(m_WaitingResArray.Values);

            foreach (var res in tmp)
            {
                res.Unload();
            }

            m_ResArray.Clear();
            m_WaitingResArray.Clear();

            ObjectPool<List<Res>>.Release(tmp);
        }


        /// <summary>
        /// 获取loader中的asset，如果已经加载。未加载则返回null
        /// </summary>
        /// <param name="assetName">资源的名称</param>
        /// <returns>资源</returns>
        public object GetAsset(string assetName, Type assetType = null)
        {
            if (assetNamePrefix != null)
                assetName = assetNamePrefix + assetName;
            var key = GetResKey(assetName, assetType);
            if (m_ResArray.ContainsKey(key))
            {
                return m_ResArray[key].asset;
            }
            else if (m_WaitingResArray.ContainsKey(key))
            {
                m_WaitingResArray[key].Unload(true);
                m_WaitingResArray.Remove(key);
            }
            return null;
        }

        /// <summary>
        /// 获取当前加载器中所有资源，状态通过资源本身获取
        /// </summary>
        public Res[] allRes
        {
            get
            {
                Res[] result;
                var list = ObjectPool<List<Res>>.Get();
                list.AddRange(m_ResArray.Values);
                list.AddRange(m_WaitingResArray.Values);
                result = list.ToArray();

                ObjectPool<List<Res>>.Release(list);
                return result;
            }
        }

        /// <summary>
        /// loader立即返回对象池，并取消其对asset的引用
        /// </summary>
        public void Recycle2Cache()
        {
            ResourceEntry.ResLoaderPool.Recycle(this);
        }

        /// <summary>
        /// 回池时的调用
        /// </summary>
        public void OnCacheReset()
        {
            loadingCount = 0;
            ReleaseAllRes();
            m_allFinishListener = null;
            assetNamePrefix = null;
            mainAssetRes = null;
#if UNITY_EDITOR
            stacktrace = null;
#endif
        }

        public void Dispose()
        {
            isDisposed = true;
            ReleaseAllRes();
            m_allFinishListener = null;
        }

        public void DisposeOnGC()
        {
            lock (m_ResArray)
            {
                var tmp = ObjectPool<List<Res>>.Get();
                tmp.AddRange(m_ResArray.Values);
                tmp.AddRange(m_WaitingResArray.Values);

                foreach (var res in tmp)
                {
                    ResourceEntry.Resource.ResGCReleaseAgent.Release(res);
                }

                m_ResArray.Clear();
                m_WaitingResArray.Clear();

                ObjectPool<List<Res>>.Release(tmp);
            }
            m_allFinishListener = null;
        }

        /// <summary>
        /// 当前加载进度
        /// </summary>
        private float progress
        {
            get
            {
                if (m_WaitingResArray.Count == 0)
                    return 1f;
                return 1 - ((float)loadingCount / m_WaitingResArray.Count);
            }
        }

        public override bool keepWaiting
        {
            get
            {
                if (m_WaitingResArray.Count > 0)
                {
                    return true;
                }
                return loadingCount > 0;
            }
        }

        private (string, Type) GetResKey(string assetName, Type assetType)
        {
            return new ValueTuple<string, Type>(assetName, assetType);
        }

    }
}