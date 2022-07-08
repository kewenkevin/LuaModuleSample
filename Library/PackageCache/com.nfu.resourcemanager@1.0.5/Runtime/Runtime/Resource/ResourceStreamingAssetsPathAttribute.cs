using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using ND.Managers.ResourceMgr.Framework;

namespace ND.Managers.ResourceMgr.Runtime
{

    /// <summary>
    /// 定制 StreamingAssets 路径，默认：空，兼容之前，比如: AssetBundles
    /// </summary>    
    [Obsolete("在设置面板配置 [StreamingAssets Path]")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ResourceStreamingAssetsPathAttribute : Attribute
    {
        private static string m_StreamingAssetsPath;
        private static bool m_HasAttribute;

        public ResourceStreamingAssetsPathAttribute()
        {
        }
        public ResourceStreamingAssetsPathAttribute(string streamingAssetsPath)
        {
            StreamingAssetsPath = streamingAssetsPath;
        }

        public string StreamingAssetsPath { get; set; }


        public static string GetStreamingAssetsPath()
        {
            bool hasAttr = false;
            string path = GetStreamingAssetsPath(out hasAttr);
            if (hasAttr)
                return path;
            return ResourceSettings.StreamingAssetsPath;
        }

        public static string GetStreamingAssetsPath(out bool hasAttr)
        {

            if (m_StreamingAssetsPath != null)
            {
                hasAttr = m_HasAttribute;
                return m_StreamingAssetsPath;
            }

            hasAttr = false;
            string streamingAssetsPath = null;

            foreach (var ass in AppDomain.CurrentDomain.GetAssemblies().Referenced(typeof(ResourceStreamingAssetsPathAttribute).Assembly))
            {
                foreach (var type in ass.GetTypes())
                {
                    foreach (var field in type.GetFields().Where(o => o.IsStatic && o.IsDefined(typeof(ResourceStreamingAssetsPathAttribute), true)))
                    {
                        streamingAssetsPath = field.GetValue(null) as string;
                        if (streamingAssetsPath == null)
                            streamingAssetsPath = string.Empty;
                        hasAttr = true;
                        break;
                    }
                    if (streamingAssetsPath != null)
                        break;
                }
                if (streamingAssetsPath != null)
                    break;
            }
            //默认根目录
            if (string.IsNullOrEmpty(streamingAssetsPath))
                streamingAssetsPath = "";
            m_StreamingAssetsPath = streamingAssetsPath;
            m_HasAttribute = hasAttr;
            return streamingAssetsPath;
        }

    }

}