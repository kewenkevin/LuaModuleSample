using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ND.Managers.ResourceMgr.Runtime;

namespace ND.Managers.ResourceMgr.Framework.Resource
{
    /// <summary>
    /// 用户个人调试使用的配置，不上传的配置
    /// </summary>
    [Serializable]
    public class ResourceUserSettings
    {
        [SerializeField]
        private bool editorMode = true;

        /// <summary>
        /// 0 表示用户未选择模式
        /// </summary>
        [SerializeField]
        private ResourceMode resourceMode = 0;


        [SerializeField]
        private string variant;
        
        [SerializeField]
        private string variantPrefixUrl;
        #region Provider


        private static SettingsProvider provider;

        private static SettingsProvider Provider
        {
            get
            {
                if (provider == null)
                {
                    provider = new SettingsProvider(typeof(ResourceUserSettings), ResourceSettings.PackageName, isRuntime: true, isProject: false);
                    provider.FileName = "Settings.json";
                }
                return provider;
            }
        }

        public static ResourceUserSettings Settings { get => (ResourceUserSettings)Provider.Settings; }

        /// <summary>
        /// 一些设置只能在编辑器使用
        /// </summary>  
        protected static void RequireEditor(string memberName)
        {
            if (!Application.isEditor)
                throw new MemberAccessException("Run only in the editor. member: " + memberName);
        }

        #endregion



        /// <summary>
        /// 编辑器模式，菜单切换
        /// </summary>
        public static bool EditorMode
        {
            get
            {
                RequireEditor(nameof(EditorMode));
                return Settings.editorMode;
            }
            set
            {
                RequireEditor(nameof(EditorMode));
                if (Provider.SetProperty(nameof(EditorMode), ref Settings.editorMode, value))
                {
                    if (EditorMode)
                    {
                        ResourceMode = ResourceMode.Unspecified;
                    }
                }
            }
        }

        /// <summary>
        /// 资源加载模式
        /// </summary>
        public static ResourceMode ResourceMode
        {
            get
            {
                RequireEditor(nameof(ResourceMode));
                return Settings.resourceMode;
            }
            set
            {
                RequireEditor(nameof(ResourceMode));
                if (Provider.SetProperty(nameof(ResourceMode), ref Settings.resourceMode, value))
                {
                    switch (ResourceMode)
                    {
                        case ResourceMode.Package:
                            EditorMode = false;
                            break;
                        case ResourceMode.Updatable:
                            EditorMode = false;
                            break;
                        case ResourceMode.Unspecified:
                            EditorMode = true;
                            break;
                    }
                }
            }
        }


        public static string Variant
        {
            get
            {
                RequireEditor(nameof(Variant));
                return Settings.variant;
            }
            set
            {
                RequireEditor(nameof(Variant));
                Provider.SetProperty(nameof(Variant), ref Settings.variant, value);
            }
        }
        
        public static string VariantPrefixUrl
        {
            get
            {
                RequireEditor(nameof(VariantPrefixUrl));
                return Settings.variantPrefixUrl;
            }
            set
            {
                RequireEditor(nameof(VariantPrefixUrl));
                Provider.SetProperty(nameof(VariantPrefixUrl), ref Settings.variantPrefixUrl, value);
            }
        }

    }

}