using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ND.Managers.ResourceMgr.Runtime
{
    /// <summary>
    /// 运行时资源配置
    /// </summary>
    [Serializable]
    public class ResourceSettings
    {
        [SerializeField]
        private string streamingAssetsPath = string.Empty;

        [SerializeField]
        private string atlasPath;

        [SerializeField]
        private bool downloadHeaderGzip;

        [SerializeField]
        private PlatformNameStyles platformNameStyle;

        [SerializeField]
        private bool editorModeReloadShader;

        [SerializeField]
        private ResourceOptions options;
        
        [SerializeField]
        private string rawAssetsPath = "Assets/ResourceManager/ResourceBuild/RawAssets";
        
        #region Provider

        public const string PackageName = "com.nfu.resourcemanager";

        private static SettingsProvider provider;

        private static SettingsProvider Provider
        {
            get
            {
                if (provider == null)
                {
                    provider = new SettingsProvider(typeof(ResourceSettings), PackageName, true, true);
                    provider.FileName = "Settings.json";
                }
                return provider;
            }
        }

        public static ResourceSettings Settings { get => (ResourceSettings)Provider.Settings; }

        #endregion



        public static string StreamingAssetsPath
        {
            get
            {
                if (!string.IsNullOrEmpty(Settings.streamingAssetsPath))
                    return Settings.streamingAssetsPath;
                bool hasAttr;
                //优先使用程序定义
                string path = ResourceStreamingAssetsPathAttribute.GetStreamingAssetsPath(out hasAttr);
                if (hasAttr)
                    return path;
                return string.Empty;
            }
            set => Provider.SetProperty(nameof(StreamingAssetsPath), ref Settings.streamingAssetsPath, value);
        }

        public static string AtlasPath
        {
            get => Settings.atlasPath;
            set => Provider.SetProperty(nameof(AtlasPath), ref Settings.atlasPath, value);
        }
        /// <summary>
        /// 下载请求Header Gizp
        /// </summary>
        public static bool DownloadRequestHeaderGzip
        {
            get => Settings.downloadHeaderGzip;
            set => Provider.SetProperty(nameof(DownloadRequestHeaderGzip), ref Settings.downloadHeaderGzip, value);
        }

        /// <summary>
        /// 编辑器 AssetBundle 模式重新加载 Shader，修复显示不正确
        /// </summary>
        public static bool EditorModeReloadShader
        {
            get => Settings.editorModeReloadShader;
            set => Provider.SetProperty(nameof(EditorModeReloadShader), ref Settings.editorModeReloadShader, value);
        }



        public static PlatformNameStyles PlatformNameStyle
        {
            get => Settings.platformNameStyle;
            set => Provider.SetProperty(nameof(PlatformNameStyle), ref Settings.platformNameStyle, value);
        }


        public static string RawAssetsPath
        {
            get => Settings.rawAssetsPath;
            set => Provider.SetProperty(nameof(RawAssetsPath), ref Settings.rawAssetsPath, value);
        }
        
        public static RuntimePlatform GetPlatform()
        {
            RuntimePlatform platform=Application.platform;
#if UNITY_ANDROID
            platform = RuntimePlatform.Android;
#elif UNITY_IOS
            platform = RuntimePlatform.IPhonePlayer;
#endif
            return platform;
        }

        public static string GetPlatformName()
        {
            RuntimePlatform platform = GetPlatform();
            return GetPlatformName(platform);
        }


        public static string GetPlatformName(RuntimePlatform platform)
        {
            string platformName;
            switch (platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    platformName = "Windows";
                    break;
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    platformName = "MacOS";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    if (PlatformNameStyle == PlatformNameStyles.Invariant)
                    {
                        platformName = "iOS";
                    }
                    else
                    {
                        //兼容 Platform.IOS
                        platformName = "IOS";
                    }
                    break;
                case RuntimePlatform.Android:
                    platformName = "Android";
                    break;
                default:
                    platformName = platform.ToString();
                    break;
            }
            if (PlatformNameStyle == PlatformNameStyles.Lower)
            {
                platformName = platformName.ToLower();
            }
            return platformName;
        }


        public static ResourceOptions Options
        {
            get => Settings.options;
            set => Provider.SetProperty(nameof(Options), ref Settings.options, value);
        }

        /// <summary>
        /// 平台名称样式，与<see cref="BuildTarget"/>保持一致，Default: <see cref="Platform"/>.IOS, Invariant: <see cref="BuildTarget"/>.iOS ,Lower: ios
        /// </summary>
        public enum PlatformNameStyles
        {
            Default,
            /// <summary>
            /// 区分大小写
            /// </summary>
            Invariant,
            /// <summary>
            /// 小写
            /// </summary>
            Lower
        }

        /// <summary>
        /// 资源选项
        /// </summary>
        public enum ResourceOptions
        {
            None,
            /// <summary>
            /// 开启堆栈日志
            /// </summary>
            AutoGCStackTrace = 0x1
        }
    }

}