using UnityEngine;
using ND.Managers.ResourceMgr.Framework.Resource;

namespace ND.Managers.ResourceMgr.Runtime
{
    public class LoadAssetHandler : ICancelable
    {
        private LoadAssetCallbacks m_loadAssetCallbacks;

        private LoadAssetSuccessCallback m_LoadAssetSuccessCallback;
        private LoadAssetFailureCallback m_LoadAssetFailureCallback;
        private LoadAssetUpdateCallback m_LoadAssetUpdateCallback;
        private LoadAssetDependencyAssetCallback m_LoadAssetDependencyAssetCallback;

        /// <summary>
        /// 优化GC
        /// </summary>
        private LoadAssetSuccessCallback m_CachedLoadAssetSuccessCallback;
        private LoadAssetFailureCallback m_CachedLoadAssetFailureCallback;
        private LoadAssetUpdateCallback m_CachedLoadAssetUpdateCallback;
        private LoadAssetDependencyAssetCallback m_CachedLoadAssetDependencyAssetCallback;


        public override void Cancel()
        {
            m_LoadAssetSuccessCallback = null;
            m_LoadAssetFailureCallback = null;
            m_LoadAssetUpdateCallback = null;
            m_LoadAssetDependencyAssetCallback = null;
        }

        private LoadAssetHandler()
        {
            m_CachedLoadAssetSuccessCallback = this.loadAssetSuccessCallback;
            m_CachedLoadAssetFailureCallback = this.loadAssetFailureCallback;
            m_CachedLoadAssetUpdateCallback = this.loadAssetUpdateCallback;
            m_CachedLoadAssetDependencyAssetCallback = this.loadAssetDependencyAssetCallback;

        }

        /// <summary>
        /// 初始化加载资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadAssetSuccessCallback">加载资源成功回调函数。</param>
        public LoadAssetHandler(LoadAssetSuccessCallback loadAssetSuccessCallback)
            : this(loadAssetSuccessCallback, null, null, null)
        {
        }

        /// <summary>
        /// 初始化加载资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadAssetSuccessCallback">加载资源成功回调函数。</param>
        /// <param name="loadAssetFailureCallback">加载资源失败回调函数。</param>
        public LoadAssetHandler(LoadAssetSuccessCallback loadAssetSuccessCallback, LoadAssetFailureCallback loadAssetFailureCallback)
            : this(loadAssetSuccessCallback, loadAssetFailureCallback, null, null)
        {
        }

        /// <summary>
        /// 初始化加载资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadAssetSuccessCallback">加载资源成功回调函数。</param>
        /// <param name="loadAssetUpdateCallback">加载资源更新回调函数。</param>
        public LoadAssetHandler(LoadAssetSuccessCallback loadAssetSuccessCallback, LoadAssetUpdateCallback loadAssetUpdateCallback)
            : this(loadAssetSuccessCallback, null, loadAssetUpdateCallback, null)
        {
        }

        /// <summary>
        /// 初始化加载资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadAssetSuccessCallback">加载资源成功回调函数。</param>
        /// <param name="loadAssetDependencyAssetCallback">加载资源时加载依赖资源回调函数。</param>
        public LoadAssetHandler(LoadAssetSuccessCallback loadAssetSuccessCallback, LoadAssetDependencyAssetCallback loadAssetDependencyAssetCallback)
            : this(loadAssetSuccessCallback, null, null, loadAssetDependencyAssetCallback)
        {
        }

        /// <summary>
        /// 初始化加载资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadAssetSuccessCallback">加载资源成功回调函数。</param>
        /// <param name="loadAssetFailureCallback">加载资源失败回调函数。</param>
        /// <param name="loadAssetUpdateCallback">加载资源更新回调函数。</param>
        public LoadAssetHandler(LoadAssetSuccessCallback loadAssetSuccessCallback, LoadAssetFailureCallback loadAssetFailureCallback, LoadAssetUpdateCallback loadAssetUpdateCallback)
            : this(loadAssetSuccessCallback, loadAssetFailureCallback, loadAssetUpdateCallback, null)
        {
        }

        /// <summary>
        /// 初始化加载资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadAssetSuccessCallback">加载资源成功回调函数。</param>
        /// <param name="loadAssetFailureCallback">加载资源失败回调函数。</param>
        /// <param name="loadAssetDependencyAssetCallback">加载资源时加载依赖资源回调函数。</param>
        public LoadAssetHandler(LoadAssetSuccessCallback loadAssetSuccessCallback, LoadAssetFailureCallback loadAssetFailureCallback, LoadAssetDependencyAssetCallback loadAssetDependencyAssetCallback)
            : this(loadAssetSuccessCallback, loadAssetFailureCallback, null, loadAssetDependencyAssetCallback)
        {
        }

        /// <summary>
        /// 初始化加载资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadAssetSuccessCallback">加载资源成功回调函数。</param>
        /// <param name="loadAssetFailureCallback">加载资源失败回调函数。</param>
        /// <param name="loadAssetUpdateCallback">加载资源更新回调函数。</param>
        /// <param name="loadAssetDependencyAssetCallback">加载资源时加载依赖资源回调函数。</param>
        public LoadAssetHandler(LoadAssetSuccessCallback loadAssetSuccessCallback, LoadAssetFailureCallback loadAssetFailureCallback, LoadAssetUpdateCallback loadAssetUpdateCallback, LoadAssetDependencyAssetCallback loadAssetDependencyAssetCallback)
            : this()
        {
            m_LoadAssetSuccessCallback = loadAssetSuccessCallback;
            m_LoadAssetFailureCallback = loadAssetFailureCallback;
            m_LoadAssetUpdateCallback = loadAssetUpdateCallback;
            m_LoadAssetDependencyAssetCallback = loadAssetDependencyAssetCallback;

            m_loadAssetCallbacks = new LoadAssetCallbacks(m_CachedLoadAssetSuccessCallback, m_CachedLoadAssetFailureCallback, m_CachedLoadAssetUpdateCallback, m_CachedLoadAssetDependencyAssetCallback);
        }

        public LoadAssetCallbacks loadAssetCallbacks
        {
            get { return m_loadAssetCallbacks; }
        }

        private void loadAssetSuccessCallback(string assetName, object asset, float duration, object userData)
        {
            if (m_LoadAssetSuccessCallback == null)
            {
                NFUResource.Release(asset);
            }
            else
            {
                m_LoadAssetSuccessCallback?.Invoke(assetName, asset, duration, userData);
            }
        }

        private void loadAssetFailureCallback(string assetName, LoadResourceStatus status, string errorMessage,
            object userData)
        {
            m_LoadAssetFailureCallback?.Invoke(assetName, status, errorMessage, userData);
        }

        private void loadAssetUpdateCallback(string assetName, float progress, object userData)
        {
            m_LoadAssetUpdateCallback?.Invoke(assetName, progress, userData);
        }
        private void loadAssetDependencyAssetCallback(string assetName, string dependencyAssetName, int loadedCount, int totalCount, object userData)
        {
            m_LoadAssetDependencyAssetCallback?.Invoke(assetName, dependencyAssetName, loadedCount, totalCount, userData);
        }


        public static LoadAssetHandler Create(LoadAssetSuccessCallback loadAssetSuccessCallback, LoadAssetFailureCallback loadAssetFailureCallback, LoadAssetUpdateCallback loadAssetUpdateCallback = null, LoadAssetDependencyAssetCallback loadAssetDependencyAssetCallback = null)
        {
            var handler = new LoadAssetHandler();
            handler.m_LoadAssetSuccessCallback = loadAssetSuccessCallback;
            handler.m_LoadAssetFailureCallback = loadAssetFailureCallback;
            handler.m_LoadAssetUpdateCallback = loadAssetUpdateCallback;
            handler.m_LoadAssetDependencyAssetCallback = loadAssetDependencyAssetCallback;
            handler.m_loadAssetCallbacks = new LoadAssetCallbacks(handler.m_CachedLoadAssetSuccessCallback, handler.m_CachedLoadAssetFailureCallback, handler.m_CachedLoadAssetUpdateCallback, handler.m_CachedLoadAssetDependencyAssetCallback);
            return handler;
        }

    }
}