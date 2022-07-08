//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using ND.Managers.ResourceMgr.Framework;
using ND.Managers.ResourceMgr.Framework.Resource;
using System;
using UnityEngine;

namespace ND.Managers.ResourceMgr.Runtime
{
    /// <summary>
    /// 基础组件。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("ResourceMgr/Base")]
    public sealed class BaseComponent : GameFrameworkComponent
    {

        [SerializeField]
        private string m_VersionHelperTypeName = "ND.Managers.ResourceMgr.Runtime.DefaultVersionHelper";

        [SerializeField]
        private string m_LogHelperTypeName = "ND.Managers.ResourceMgr.Runtime.DefaultLogHelper";

        [SerializeField]
        private string m_ZipHelperTypeName = "ND.Managers.ResourceMgr.Runtime.DefaultZipHelper";

        /// <summary>
        /// 获取或设置是否使用编辑器资源模式（仅编辑器内有效）。
        /// </summary>
        public bool EditorResourceMode
        {
            get
            {
                if (!Application.isPlaying)
                    return true;
                if (Application.isEditor)
                    return ResourceUserSettings.EditorMode;
                return false;
            }
        }


        /// <summary>
        /// 获取或设置编辑器资源辅助器。
        /// </summary>
        public IResourceManager EditorResourceHelper
        {
            get;
            set;
        }

        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            InitVersionHelper();
            InitLogHelper();
            Log.Info("Game Framework Version: {0}", ND.Managers.ResourceMgr.Framework.Version.GameFrameworkVersion);
            Log.Info("Game Version: {0} ({1})", ND.Managers.ResourceMgr.Framework.Version.GameVersion, ND.Managers.ResourceMgr.Framework.Version.InternalGameVersion.ToString());
            Log.Info("Unity Version: {0}", Application.unityVersion);

#if UNITY_5_3_OR_NEWER || UNITY_5_3
            InitZipHelper();

            Utility.Converter.ScreenDpi = Screen.dpi;

            if (EditorResourceMode)
            {
                Log.Info("During this run, Game Framework will use editor resource files, which you should validate first.");
            }

            
#else
            Log.Error("Game Framework only applies with Unity 5.3 and above, but current Unity version is {0}.", Application.unityVersion);
            GameEntry.Shutdown(ShutdownType.Quit);
#endif
#if UNITY_5_6_OR_NEWER
            Application.lowMemory += OnLowMemory;
#endif
        }

        private void Start()
        {
        }

        private void Update()
        {
            GameFrameworkEntry.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void OnApplicationQuit()
        {
#if UNITY_5_6_OR_NEWER
            Application.lowMemory -= OnLowMemory;
#endif
            StopAllCoroutines();
        }

        private void OnDestroy()
        {
            GameFrameworkEntry.Shutdown();
        }

        internal void Shutdown()
        {
            Destroy(gameObject);
        }

        private void InitVersionHelper()
        {
            if (string.IsNullOrEmpty(m_VersionHelperTypeName))
            {
                return;
            }

            Type versionHelperType = Utility.Assembly.GetType(m_VersionHelperTypeName);
            if (versionHelperType == null)
            {
                throw new GameFrameworkException(Utility.Text.Format("Can not find version helper type '{0}'.", m_VersionHelperTypeName));
            }

            ND.Managers.ResourceMgr.Framework.Version.IVersionHelper versionHelper = (ND.Managers.ResourceMgr.Framework.Version.IVersionHelper)Activator.CreateInstance(versionHelperType);
            if (versionHelper == null)
            {
                throw new GameFrameworkException(Utility.Text.Format("Can not create version helper instance '{0}'.", m_VersionHelperTypeName));
            }

            ND.Managers.ResourceMgr.Framework.Version.SetVersionHelper(versionHelper);
        }

        private void InitLogHelper()
        {
            if (string.IsNullOrEmpty(m_LogHelperTypeName))
            {
                return;
            }

            Type logHelperType = Utility.Assembly.GetType(m_LogHelperTypeName);
            if (logHelperType == null)
            {
                throw new GameFrameworkException(Utility.Text.Format("Can not find log helper type '{0}'.", m_LogHelperTypeName));
            }

            GameFrameworkLog.ILogHelper logHelper = (GameFrameworkLog.ILogHelper)Activator.CreateInstance(logHelperType);
            if (logHelper == null)
            {
                throw new GameFrameworkException(Utility.Text.Format("Can not create log helper instance '{0}'.", m_LogHelperTypeName));
            }

            GameFrameworkLog.SetLogHelper(logHelper);
        }

        private void InitZipHelper()
        {
            if (string.IsNullOrEmpty(m_ZipHelperTypeName))
            {
                return;
            }

            Type zipHelperType = Utility.Assembly.GetType(m_ZipHelperTypeName);
            if (zipHelperType == null)
            {
                Log.Error("Can not find Zip helper type '{0}'.", m_ZipHelperTypeName);
                return;
            }

            Utility.ZipUtil.IZipHelper zipHelper = (Utility.ZipUtil.IZipHelper)Activator.CreateInstance(zipHelperType);
            if (zipHelper == null)
            {
                Log.Error("Can not create Zip helper instance '{0}'.", m_ZipHelperTypeName);
                return;
            }

            Utility.ZipUtil.SetZipHelper(zipHelper);
        }

        private void OnLowMemory()
        {
            Log.Info("Low memory reported...");

            ResourceComponent resourceCompoent = ResourceEntry.GetComponent<ResourceComponent>();
            if (resourceCompoent != null)
            {
                resourceCompoent.ForceUnloadUnusedAssets(true);
            }
        }
    }
}
