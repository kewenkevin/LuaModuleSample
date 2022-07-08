//-----------------------------------------------------------------------
// Created By CHENSHENG
// 本文件资源系统主要接口封装，供外部调用
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using ND.Managers.ResourceMgr.Framework.Resource;
using Object = UnityEngine.Object;

namespace ND.Managers.ResourceMgr.Runtime
{
    /// <summary>
    /// NFU 资源系统接口封装类
    /// </summary>
    public static class NFUResource
    {

        public delegate void LoadAssetSuccessCallback<in T>(string assetName, T asset, float duration, object userData) where T : UnityEngine.Object;
        public enum LoadResMode
        {
            Priority,   //优先级模式
            Rapid       //急速模式
        }

        /// <summary>
        /// 资源模块
        /// </summary>
        public static ResourceComponent Resource => ResourceEntry.Resource;

        /// <summary>
        /// 网络请求模块
        /// </summary>
        public static WebRequestComponent WebRequest => ResourceEntry.WebRequest;

        public static bool Initializated => Resource && Resource.Initializated;


        /// <summary>
        /// 初始化资源系统
        /// </summary>
        public static void Initialize()
        {
            ResourceEntry.InitBuiltinComponents();
        }

        /// <summary>
        /// 开始版本检测，资源检测以及更新流程
        /// 请注意 务必晚于游戏第一帧运行，如果一定要在第一帧运行请使用 yield return new WaitForEndFrame();来等待帧的最后再执行热更新开始，否则会有异常错误
        /// </summary>
        public static void HotUpdateStart()
        {
            ResourceEntry.ResourceUpdate.Begin();
        }


        /// <summary>
        /// 关闭资源系统，重启游戏可以调用
        /// </summary>
        /// <param name="type">关闭方式</param>
        public static void Shutdown(ShutdownType type)
        {
            ResourceEntry.Shutdown(type);
        }


        /// <summary>
        /// 检查资源状态
        /// </summary>
        /// <param name="assetName">资源名</param>
        /// <returns>资源状态</returns>
        public static HasAssetResult HasAsset(string assetName)
        {
            return ResourceEntry.Resource.HasAsset(assetName);
        }

        #region Resource Info

        public static string GetResourceName(string assetName)
        {
            return ResourceEntry.Resource.ResourceManager.GetResourceName(assetName);
        }

        public static string GetResourceFullName(string assetName)
        {
            return ResourceEntry.Resource.ResourceManager.GetResourceFullName(assetName);
        }
        public static string GetResourceFileName(string assetName)
        {
            return ResourceEntry.Resource.ResourceManager.GetResourceFileName(assetName);
        }

        public static string GetResourcePath(string assetName)
        {
            return ResourceEntry.Resource.ResourceManager.GetResourcePath(assetName);
        }

        #endregion


        /// <summary>
        /// 异步加载并创建一个GameObject，他会和资源绑定，销毁GameObject自动卸载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        public static ICancelable InstantiateAsync(string assetName, LoadAssetSuccessCallback loadAssetSuccessCallback = null,
            LoadAssetFailureCallback loadAssetFailureCallback = null)
        {
            bool rapid = false;
            var handler = LoadAssetHandler.Create((name, asset, duration, data) =>
            {
                if (asset is GameObject)
                {
                    GameObject go;
                    try
                    {
                        go = GameObject.Instantiate(asset as GameObject);
                        var assetReferencer = go.AddComponent<GameObjectAssetReferencer>();
                        assetReferencer.RefAsset(name, asset as GameObject);
                    }
                    catch (Exception e)
                    {
                        ResourceEntry.Resource.UnloadAsset(assetName);
                        throw e;
                    }
                    if (loadAssetSuccessCallback != null)
                        loadAssetSuccessCallback(assetName, go, duration, data);
                }
                else
                {
                    ResourceEntry.Resource.UnloadAsset(assetName);
                }
            }, (name, status, errorMessage, userData) =>
            {
                if (loadAssetFailureCallback != null)
                    loadAssetFailureCallback(assetName, status, errorMessage, userData);
            });
            if (rapid)
            {
                ResourceEntry.Resource.LoadAssetRapid(assetName, handler.loadAssetCallbacks);
            }
            else
            {
                ResourceEntry.Resource.LoadAsset(assetName, handler.loadAssetCallbacks);
            }
            return handler;
        }

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">资源类型</param>
        /// <param name="loadAssetSuccessCallback">资源成功回调</param>
        /// <param name="loadAssetFailureCallback">资源失败回调</param>
        /// <param name="mode"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public static ICancelable LoadAssetAsync(string assetName, Type assetType,
            LoadAssetSuccessCallback loadAssetSuccessCallback = null,
            LoadAssetFailureCallback loadAssetFailureCallback = null, 
            int priority = 0) 
        {
            var handler = GenerateHandler(assetName, loadAssetSuccessCallback, loadAssetFailureCallback);
            ResourceEntry.Resource.LoadAsset(assetName, assetType, priority, handler.loadAssetCallbacks);
            return handler;
        }

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        public static ICancelable LoadAssetAsync<T>(string assetName, LoadAssetSuccessCallback<T> loadAssetSuccessCallback = null,
            LoadAssetFailureCallback loadAssetFailureCallback = null, int priority = 0) where T : Object
        {
            return LoadAssetAsync(assetName, typeof(T), (name, asset, duration, data) =>
            {
                if (loadAssetSuccessCallback != null)
                {
                    loadAssetSuccessCallback.Invoke(name, (T)asset, duration, data);
                }
            }, loadAssetFailureCallback, priority);
        }
        

        public static UnityEngine.Object LoadAssetImmediate(string assetName, Type assetType)
        {
            return ResourceEntry.Resource.LoadAssetImmediate(assetName, assetType);
        }


        /// <summary>
        /// 卸载资源。
        /// </summary>
        /// <param name="asset">要卸载的资源。</param>
        public static void Release(object asset)
        {
            ResourceEntry.Resource.UnloadAsset(asset);
        }
        public static void Release(string assetName, Type assetType = null)
        {
            ResourceEntry.Resource.UnloadAsset(assetName, assetType);
        }

        /// <summary>
        /// 从资源系统同步加载一个二进制流文件，一般来说只用于lua的加载
        /// </summary>
        /// <param name="binaryAssetName">二进制文件名称</param>
        /// <returns>二进制数据</returns>
        public static byte[] LoadBinaryFromFileSystem(string binaryAssetName)
        {
            return ResourceEntry.Resource.LoadBinaryFromFileSystem(binaryAssetName);
        }

        /// <summary>
        /// 是否采用模拟模式
        /// </summary>
        /// <returns></returns>
        public static bool isEditorMode
        {
            get { return ResourceEntry.Base.EditorResourceMode; }
        }


        /// <summary>
        /// 设置或获取当前使用的资源变体，请务必在初始化后热更新开始前调用，否则可能出现异常
        /// </summary>
        public static string currentVariant
        {
            get
            {
                return ResourceEntry.Resource.CurrentVariant;
            }
            set { ResourceEntry.Resource.SetCurrentVariant(value); }
        }



        /// <summary>
        /// 异步加载场景资源。
        /// </summary>
        /// <param name="sceneAssetName">要加载场景资源的名称。</param>
        /// <param name="priority">加载场景资源的优先级。</param>
        /// <param name="loadSceneCallbacks">加载场景回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public static void LoadSceneAsset(string sceneAssetName, LoadSceneCallbacks loadSceneCallbacks = null, int priority = 0, Object userData = null )
        {
            ResourceEntry.Resource.ResourceManager.LoadScene(sceneAssetName, priority, loadSceneCallbacks, userData);
        }

        /// <summary>
        /// 异步卸载场景资源。
        /// </summary>
        /// <param name="sceneAssetName">要卸载场景资源的名称。</param>
        /// <param name="unloadSceneCallbacks">卸载场景回调函数集。</param>
        public static void UnloadSceneAsset(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks)
        {
            ResourceEntry.Resource.ResourceManager.UnloadScene(sceneAssetName, unloadSceneCallbacks);
        }

        /// <summary>
        /// 异步卸载场景。
        /// </summary>
        /// <param name="sceneAssetName">要卸载场景资源的名称。</param>
        /// <param name="unloadSceneCallbacks">卸载场景回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public static void UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks, object userData)
        {
            ResourceEntry.Resource.ResourceManager.UnloadScene(sceneAssetName, unloadSceneCallbacks, userData);
        }


        /// <summary>
        /// 获取二进制资源的实际路径。
        /// </summary>
        /// <param name="binaryAssetName">要获取实际路径的二进制资源的名称。</param>
        /// <returns>二进制资源的实际路径。</returns>
        /// <remarks>此方法仅适用于二进制资源存储在磁盘（而非文件系统）中的情况。若二进制资源存储在文件系统中时，返回值将始终为空。</remarks>
        public static string GetBinaryPath(string binaryAssetName)
        {
            return ResourceEntry.Resource.ResourceManager.GetBinaryPath(binaryAssetName);
        }

        /// <summary>
        /// 获取二进制资源的实际路径。
        /// </summary>
        /// <param name="binaryAssetName">要获取实际路径的二进制资源的名称。</param>
        /// <param name="storageInReadOnly">二进制资源是否存储在只读区中。</param>
        /// <param name="storageInFileSystem">二进制资源是否存储在文件系统中。</param>
        /// <param name="relativePath">二进制资源或存储二进制资源的文件系统，相对于只读区或者读写区的相对路径。</param>
        /// <param name="fileName">若二进制资源存储在文件系统中，则指示二进制资源在文件系统中的名称，否则此参数返回空。</param>
        /// <returns>是否获取二进制资源的实际路径成功。</returns>
        public static bool GetBinaryPath(string binaryAssetName, out bool storageInReadOnly, out bool storageInFileSystem, out string relativePath, out string fileName)
        {
            return ResourceEntry.Resource.ResourceManager.GetBinaryPath(binaryAssetName, out storageInReadOnly, out storageInFileSystem, out relativePath, out fileName);
        }

        /// <summary>
        /// 获取二进制资源的长度。
        /// </summary>
        /// <param name="binaryAssetName">要获取长度的二进制资源的名称。</param>
        /// <returns>二进制资源的长度。</returns>
        public static int GetBinaryLength(string binaryAssetName)
        {
            return ResourceEntry.Resource.ResourceManager.GetBinaryLength(binaryAssetName);
        }

        /// <summary>
        /// 异步加载二进制资源。
        /// </summary>
        /// <param name="binaryAssetName">要加载二进制资源的名称。</param>
        /// <param name="loadBinaryCallbacks">加载二进制资源回调函数集。</param>
        public static void LoadBinary(string binaryAssetName, LoadBinaryCallbacks loadBinaryCallbacks)
        {
            ResourceEntry.Resource.ResourceManager.LoadBinary(binaryAssetName, loadBinaryCallbacks);
        }

        /// <summary>
        /// 异步加载二进制资源。
        /// </summary>
        /// <param name="binaryAssetName">要加载二进制资源的名称。</param>
        /// <param name="loadBinaryCallbacks">加载二进制资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public static void LoadBinary(string binaryAssetName, LoadBinaryCallbacks loadBinaryCallbacks, object userData)
        {
            ResourceEntry.Resource.ResourceManager.LoadBinary(binaryAssetName, loadBinaryCallbacks, userData);
        }

        /// <summary>
        /// 获取某个资源名的所有assetName （不安全 不推荐使用）
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static string[] GetResourceAllAssets(string resourceName)
        {
            return ResourceEntry.Resource.ResourceManager.GetResourceAllAssets(resourceName);
        }


        /// <summary>
        /// 获取asset同包的所有精灵
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static object[] GetAllSubAssets(object rootAsset)
        {
            return ResourceEntry.Resource.ResourceManager.GetPackageSubAsset(rootAsset);
        }


        public static List<object> GetSubAssets(object rootAsset, Type type)
        {
            var assets = GetAllSubAssets(rootAsset);
            var result = new List<object>();
            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i].GetType() == type)
                {
                    result.Add(assets[i]);
                }
            }
            return result;
        }

        private static LoadAssetHandler GenerateHandler(string assetName, LoadAssetSuccessCallback loadAssetSuccessCallback, LoadAssetFailureCallback loadAssetFailureCallback)
        {
            return LoadAssetHandler.Create((string name, object asset, float duration, object userData) =>
                {
                    loadAssetSuccessCallback?.Invoke(assetName, asset, duration, userData);
                }
                , (string name, LoadResourceStatus status, string errorMessage, object userData) =>
                {
                    loadAssetFailureCallback?.Invoke(assetName, status, errorMessage, userData);
                });
        }

        public static List<T> GetSubAssets<T>(object rootAsset) where T : UnityEngine.Object
        {
            var assets = GetAllSubAssets(rootAsset);
            var result = new List<T>();
            for (int i = 0; i < assets.Length; i++)
            {
                var asset = assets[i] as T;
                if (asset != null)
                {
                    result.Add(asset);
                }
            }
            return result;
        }

    }
}