using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace ND.Managers.ResourceMgr.Runtime
{


    /// <summary>
    /// zip_file.aar 源代码 【SDK\extras\google\market_apk_expansion\zip_file】，是从【Google Play APK Expansion library】导出的库
    /// </summary>
    public class ResourcesAndroidUtility
    {

        private static AndroidJavaClass m_ResourcesAndroidUtilityClass;
        private static bool? m_IsObb;
        private static int? m_BundleVersionCode;

        /// <summary>
        /// ResourcesAndroidUtility.java 类
        /// </summary>
        public static AndroidJavaClass ResourcesAndroidUtilityClass
        {
            get
            {
                if (m_ResourcesAndroidUtilityClass == null)
                    m_ResourcesAndroidUtilityClass = new AndroidJavaClass("ResourcesAndroidUtility");
                return m_ResourcesAndroidUtilityClass;
            }
        }

        /// <summary>
        /// StreamingAssets 是否为 .obb
        /// </summary>
        public static bool IsObb
        {
            get
            {
                if (m_IsObb == null)
                {
                    string str = $".obb!/assets";
                    m_IsObb = Application.streamingAssetsPath.EndsWith(str, StringComparison.InvariantCultureIgnoreCase);
                }
                return m_IsObb.Value;
            }
        }


        public static int BundleVersionCode
        {
            get
            {
                if (m_BundleVersionCode != null)
                    return m_BundleVersionCode.Value;

                AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

                var ca = up.GetStatic<AndroidJavaObject>("currentActivity");

                AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");

                var pInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", Application.identifier, 0);

                m_BundleVersionCode = pInfo.Get<int>("versionCode");
                return m_BundleVersionCode.Value;
            }
        }

        public static byte[] ReadFileAllBytes(string url)
        {
            byte[] result = null;
            if (Application.platform == RuntimePlatform.Android)
            {
                if (url.StartsWith("jar:file://"))
                {
                    string relativePath = url.Substring(Application.streamingAssetsPath.Length + 1);

                    sbyte[] bytes;

                    if (IsObb)
                        bytes = ResourcesAndroidUtilityClass.CallStatic<sbyte[]>("ReadStreamingAssetsAllBytesWithObb", relativePath);
                    else
                        bytes = ResourcesAndroidUtilityClass.CallStatic<sbyte[]>("ReadStreamingAssetsAllBytes", relativePath);
                    if (bytes != null)
                    {
                        byte[] buff = new byte[bytes.Length];
                        Buffer.BlockCopy(bytes, 0, buff, 0, bytes.Length);
                        result = buff;
                    }
                }
            }

            if (result == null)
            {
                if (url.StartsWith("file://"))
                    result = File.ReadAllBytes(url.Substring(7));
                else
                    result = File.ReadAllBytes(url);
            }
            return result;
        }

        public static AndroidJavaObject GetStreamingAssetsInputStream(string path)
        {
            return ResourcesAndroidUtilityClass.CallStatic<AndroidJavaObject>("GetStreamingAssetsInputStreamWithObb", path);
        }



    }
}