//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Profiling;
using static ND.Managers.ResourceMgr.Framework.Resource.ResourceManager.ResourceLoader;

namespace ND.Managers.ResourceMgr.Framework
{
    /// <summary>
    /// 实用函数集。
    /// </summary>
    public static partial class Utility
    {
        #region 统计性能用

        /// <summary>
        /// 总加载资源任务数
        /// </summary>
        public static int TotalAssetTask { get; private set; }
        /// <summary>
        /// 总加载依赖资源任务数
        /// </summary>
        public static int TotalDependencyAssetTask { get; private set; }
        /// <summary>
        /// 总加载资源数量
        /// </summary>
        public static int TotalAssetObject { get; private set; }
        /// <summary>
        /// 总加载资源数量，完成
        /// </summary>
        public static int TotalLoadAssetCompleted { get; private set; }
        /// <summary>
        /// 总加载资源包数量
        /// </summary>
        public static int TotalResourceObject { get; private set; }
        /// <summary>
        /// 总加载资源包数量，完成
        /// </summary>
        public static int TotalLoadResourceCompleted { get; private set; }
        /// <summary>
        /// 总任务使用的帧数
        /// </summary>
        public static long TotalAssetTaskUsedFrame { get; private set; }
        /// <summary>
        /// 任务最后的帧数
        /// </summary>
        private static int LastAssetTaskUsedFrame { get; set; }

        #endregion

        /// <summary>
        /// 调试查错, 查找自动GC的ResLoader
        /// </summary>
        public static List<string> CollectAutoGCs { get; private set; } = new List<string>();


        [Conditional("UNITY_EDITOR")]
        internal static void OnCreateAssetTask(TaskBase task, bool depend = false)
        {
            TotalAssetTask++;
            if (depend)
                TotalDependencyAssetTask++;
        }


        [Conditional("UNITY_EDITOR")]
        public static void OnCreateAssetObject(AssetObject assetObject)
        {
            TotalAssetObject++;
        }
        [Conditional("UNITY_EDITOR")]
        public static void OnLoadAssetCompleted()
        {
            TotalLoadAssetCompleted++;
        }

        [Conditional("UNITY_EDITOR")]
        public static void OnCreateResourceObject(ResourceObject resourceObject)
        {
            TotalResourceObject++;
        }
        [Conditional("UNITY_EDITOR")]
        public static void OnLoadResourceCompleted()
        {
            TotalLoadResourceCompleted++;
        }
        [Conditional("UNITY_EDITOR")]
        public static void OnAssetTaskUpdate()
        {
            if (LastAssetTaskUsedFrame != Time.frameCount)
            {
                LastAssetTaskUsedFrame = Time.frameCount;
                TotalAssetTaskUsedFrame++;
            }
        }

        public static class Profiler
        {

            [Conditional("UNITY_EDITOR")]
            [Conditional("ENABLE_LOG")]
            public static void BeginSample(string name)
            {
                UnityEngine.Profiling.Profiler.BeginSample(name);
            }

            [Conditional("UNITY_EDITOR")]
            [Conditional("ENABLE_LOG")]
            public static void EndSample()
            {
                UnityEngine.Profiling.Profiler.EndSample();
            }

        }
    }
}
