//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.IO;
using UnityEditor;
using UnityEngine;

namespace ND.Managers.ResourceMgr.Editor
{
    /// <summary>
    /// 帮助相关的实用函数。
    /// </summary>
    public static class Help
    {
        //[MenuItem("Tools/ResourceMgr/文档", false, 90)]
        //public static void ShowDocumentation()
        //{
        //    ShowHelp("https://gameframework.cn/document/");
        //}

        //[MenuItem("Tools/ResourceMgr/API文档", false, 91)]
        //public static void ShowApiReference()
        //{
        //    ShowHelp("https://gameframework.cn/api/");
        //}

        private static void ShowHelp(string uri)
        {
            Application.OpenURL(uri);
        }

        [MenuItem(EditorUtilityx.MenuPrefix + "Document", false, 90)]
        public static void OpenREADME()
        {
            string path;
            path = Path.Combine(EditorUtilityx.PackageDir, "README.md");
            path = Path.GetFullPath(path);
            if (File.Exists(path))
            {
                ShowHelp(path);
            }
            else
            {
                Debug.LogError("file not exists path: " + path);
            }
        }
    }
}
