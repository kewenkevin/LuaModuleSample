//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Linq;
using UnityEngine;

namespace ND.Managers.ResourceMgr.Framework.Resource
{
    /// <summary>
    /// 加载资源代理辅助器异步加载资源完成事件。
    /// </summary>
    public sealed class LoadResourceAgentHelperLoadCompleteEventArgs : GameFrameworkEventArgs
    {
        /// <summary>
        /// 初始化加载资源代理辅助器异步加载资源完成事件的新实例。
        /// </summary>
        public LoadResourceAgentHelperLoadCompleteEventArgs()
        {
            Asset = null;
        }

        /// <summary>
        /// 获取加载的资源。
        /// </summary>
        public object Asset
        {
            get;
            private set;
        }

        public object[] SubAsset
        {
            get;
            private set;
        }

        public bool LoadResourceAll
        {
            get;
            private set;
        }


        /// <summary>
        /// 创建加载资源代理辅助器异步加载资源完成事件。
        /// </summary>
        /// <param name="asset">加载的资源。</param>
        /// <param name="rawSubAssets">资源是否已经过移除 MainAsset 处理，true表示未处理</param>
        /// <returns>创建的加载资源代理辅助器异步加载资源完成事件。</returns>
        public static LoadResourceAgentHelperLoadCompleteEventArgs Create(object asset, object[] subAsset, bool loadAll = false, bool rawSubAssets = true)
        {
            LoadResourceAgentHelperLoadCompleteEventArgs loadResourceAgentHelperLoadCompleteEventArgs = ReferencePool.Acquire<LoadResourceAgentHelperLoadCompleteEventArgs>();
            loadResourceAgentHelperLoadCompleteEventArgs.Asset = asset;
            if (subAsset == null)
            {
                subAsset = new object[0];
            }

            if (rawSubAssets && subAsset.Length > 0)
            {
                loadResourceAgentHelperLoadCompleteEventArgs.SubAsset = subAsset.Where(o=>asset!=o).ToArray();
            }
            else
                loadResourceAgentHelperLoadCompleteEventArgs.SubAsset = subAsset;


            loadResourceAgentHelperLoadCompleteEventArgs.LoadResourceAll = loadAll;
            return loadResourceAgentHelperLoadCompleteEventArgs;
        }

        /// <summary>
        /// 清理加载资源代理辅助器异步加载资源完成事件。
        /// </summary>
        public override void Clear()
        {
            Asset = null;
        }
    }
}
