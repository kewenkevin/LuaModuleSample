using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public class XluaImporter 
{
    const string XLuaVersion =  "2.1.15";
    const string NFUPackageVersion =  "1.0.1";
    const string XLuaSourceAssetPath = "Packages/com.nfu.xlua/Samples~/Xlua";
    static string XLuaTargetAssetPath = "";

    private static void Initialize()
    {
        if (string.IsNullOrEmpty(XLuaTargetAssetPath))
        {
            XLuaTargetAssetPath = Application.dataPath + "/XLua";
        }
    }

    // [UnityEditor.Callbacks.DidReloadScripts(0)]
    [MenuItem("Tools/XLua运行时代码展开")]
    private static void OnScriptReload()
    {
        try
        {
            Initialize();
            
            var luaEnv = TypeAssembly.GetType("XLua.LuaEnv");
            
            if (luaEnv == null || !Directory.Exists(XLuaTargetAssetPath))
            {
                if (UnityEditor.EditorUtility.DisplayDialog("注意", "项目使用了Xlua包，但是并未展开到工程目录，是否展开", "展开", "取消"))
                {
                    CopyXluaToProject();
                }
            }
            else
            {
                try
                {
                    var XLuaVersionType = TypeAssembly.GetType("XLua.XLuaVersionDefine");
                    if (XLuaVersionType == null)
                    {
                        OnVersionDiff("[Unknown]","[Unknown]");
                        return;
                    }
                    var versionField = XLuaVersionType.GetField("XLuaVersion", BindingFlags.Static | BindingFlags.Public);
                    if (versionField == null)
                    {
                        OnVersionDiff("[Unknown]","[Unknown]");
                        return;
                    } 
                    
                    string s = versionField.GetValue(null) as string;

                    if (string.IsNullOrEmpty(s) || !s.Equals(XLuaVersion))
                    {
                        OnVersionDiff(s,"[Unknown]");
                        return;
                    }
                    
                    
                    var nfuVersionField = XLuaVersionType.GetField("NFUPackageVersion", BindingFlags.Static | BindingFlags.Public);
                    if (nfuVersionField == null)
                    {
                        OnVersionDiff(s,"[Unknown]");
                        return;
                    }

                   
                    
                    string nfuVersion = nfuVersionField.GetValue(null) as string;

                    if (string.IsNullOrEmpty(nfuVersion) || !nfuVersion.Equals(NFUPackageVersion))
                    {
                        OnVersionDiff(s,nfuVersion);
                        return;
                    }
                    
                    UnityEditor.EditorUtility.DisplayDialog("注意", $"无需展开!!!" +
                                                                  $"已有{s}/{nfuVersion}，package内是{XLuaVersion}/{NFUPackageVersion}","OK");
                }
                catch (Exception e)
                {
                    OnVersionDiff("[Unknown]","[Unknown]");
                }
            }
        }
        catch (Exception e)
        {
            if (UnityEditor.EditorUtility.DisplayDialog("注意", "项目使用了Xlua包，但是并未展开到工程目录，是否展开", "展开", "取消"))
            {
                CopyXluaToProject();
            }
        }
        
    }

    private static void OnVersionDiff(string newVersion,string newVersionNFU)
    {
        if (UnityEditor.EditorUtility.DisplayDialog("注意", $"项目使用了Xlua包，但是版本不同，" +
                                                          $"已有{newVersion}/{newVersionNFU}，package内是{XLuaVersion}/{NFUPackageVersion}。是否重新展开", "展开", "取消"))
        {
            CopyXluaToProject();
        }
    }

    private static void CopyXluaToProject()
    {
        var directory = Path.GetFullPath(XLuaSourceAssetPath);
        
        if (Directory.Exists(directory))
        {
            if (Directory.Exists(XLuaTargetAssetPath))
            {
                if (UnityEditor.EditorUtility.DisplayDialog("警报", "目标目录已有，请先删除XLua目录", "删除继续", "取消展开"))
                {
                    DirectoryInfo di = new DirectoryInfo(XLuaTargetAssetPath); 
                    di.Delete(true);
                    CopyDirectory(directory,XLuaTargetAssetPath);
                }
            }
            else
            {
                CopyDirectory(directory,XLuaTargetAssetPath);
            }

            SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "XLua");
            SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "XLua");
            SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "XLua");
            
            AssetDatabase.Refresh();
            UnityEditor.EditorUtility.DisplayDialog("信息", "展开完成", "OK");
        }
        else
        {
            UnityEditor.EditorUtility.DisplayDialog("警告", "未找到XLua展开资源：" + directory, "OK");
        }

    }

    private static void SetScriptingDefineSymbolsForGroup(BuildTargetGroup targetGroup, string define)
    {
        if (!IsExistInScriptingDefineSymbolsForGroup(targetGroup, define))
        {
            UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, define);
        }
    }
    
    private static bool IsExistInScriptingDefineSymbolsForGroup(BuildTargetGroup targetGroup, string define)
    {
        string[] targetGroupDefines;
        UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out targetGroupDefines);
        if (targetGroupDefines != null & targetGroupDefines.Length > 0)
        {
            foreach (var defineItem in targetGroupDefines)
            {
                if (defineItem.Equals(define))
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    private static void CopyDirectory(string sourceDirPath,string SaveDirPath)
    {
        try
        {
            //如果指定的存储路径不存在，则创建该存储路径
            if (!Directory.Exists(SaveDirPath))
            {
                //创建
                Directory.CreateDirectory(SaveDirPath);
            }
            //获取源路径文件的名称
            string[] files = Directory.GetFiles(sourceDirPath);
            //遍历子文件夹的所有文件
            foreach(string file in files)
            {
                string pFilePath = SaveDirPath + "\\" + Path.GetFileName(file);
                if (File.Exists(pFilePath))
                    continue;
                File.Copy(file, pFilePath, true);
            }
            string[] dirs = Directory.GetDirectories(sourceDirPath);
            //递归，遍历文件夹
            foreach (string dir in dirs)
            {
                CopyDirectory(dir, SaveDirPath + "\\" + Path.GetFileName(dir));
            }
            

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
