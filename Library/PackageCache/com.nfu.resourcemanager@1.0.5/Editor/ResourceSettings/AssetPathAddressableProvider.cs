using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{
    /// <summary>
    /// 手动分配资源包名规则，支持<see cref="System.StringFormats.StringFormat"/>
    /// </summary>
    internal class AssetPathAddressableProvider : IAddressableProvider
    {

        [Tooltip("资源包，\n支持 StringFormat，格式：'{$AssetPath}'")]
        [SerializeField]
        public string bundleName;
        [HideInInspector]
        [Tooltip("资源名，\n支持 StringFormat，格式：'{$AssetPath}'")]
        [SerializeField]
        public string assetName;
        [HideInInspector]
        [Tooltip("变体名，\n支持 StringFormat，格式：'{$AssetPath}'")]
        [SerializeField]
        public string variant;

        //[Tooltip("变体名")]
        //[SerializeField]
        //public bool assetNameToLower;

        public bool IsAssetPathMatch(string assetPath)
        {
            return true;
        }

        public bool GetAddressableName(string assetPath, out string bundleName, out string variant, out string assetName)
        {
            variant = null;
            assetName = null;

            if (string.IsNullOrEmpty(this.bundleName))
                bundleName = assetPath;
            else
                bundleName = EditorResourceSettings.FormatString(this.bundleName, assetPath);

            if (string.IsNullOrEmpty(bundleName))
                return false;

            if (string.IsNullOrEmpty(this.assetName))
                assetName = assetPath;
            else
                assetName = EditorResourceSettings.FormatString(this.assetName, assetPath);

            if (string.IsNullOrEmpty(assetName))
                return false;

            if (!string.IsNullOrEmpty(this.variant))
                variant = EditorResourceSettings.FormatString(this.variant, assetPath);

            return true;
        }


        public void OnPreprocessAnalysis(string assetPath, ResourceEditorImplate builder)
        {
        }
    }


}