using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using UnityEditor.Callbacks;
using System;
using System.Xml;
using System.Text.RegularExpressions;

/// <summary>
/// 工具介绍：
/// 自定义Console双击日志跳转功能， 支持Lua / 封装Debugger / 原生Debug 跳转
//  /
//  / 使用说明：
///
/// 1. 设置Lua跳转IDE:   菜单栏 > Lua > Set Your IDE Path     
/// 2. 设置Lua项目配置文件（.iml）： 菜单栏 > Lua/Set Your Luaproject File
/// 3. 双击Console Log 执行跳转
/// 
/// 
/// 补充说明：
/// Lua项目配置文件采用xml的形式去配置Lua工程所包含的目录
/// 只能跳转带有堆栈信息的Log（Lua跳转，需要输出的日志也要带堆栈）
/// 
/// 
/// 已知问题：
/// 1. 当Unity报ConsoleWindoe为空引用时会失效，需要重启Unity编辑器, 不然执行不到双击回调
/// 2. Lua默认跳转逻辑是跳转到第一条不是忽略代码的文件行数，如果不准确，检查是否需要添加 LUA_IGNORE_LINE
/// 3. C# 默认跳转逻辑同上, 忽略规则添加到 CS_IGNORE_LINE
/// 
/// </summary>

namespace Yoozoo.Tools.LuaIDELog
{
    public class LocalLog2IDE : Editor
    {

        /// <summary>
        /// 设置IDE exe
        /// </summary>
        [MenuItem("Tools/LuaTools/LuaIDELog/Set Your IDE Path", false, 200)]
        static void SetExternalEditorPath()
        {
            string path = EditorUserSettings.GetConfigValue(EXTERNAL_EDITOR_PATH_KEY);
            path = EditorUtility.OpenFilePanel("Select Your IDE", path, "exe");

            if (path != "")
            {
                EditorUserSettings.SetConfigValue(EXTERNAL_EDITOR_PATH_KEY, path);
                Debug.Log("Set Your IDE Path: " + path);
            }
        }
        
        /// <summary>
        /// 定位堆栈信息的正则
        /// </summary>
        static readonly string regexRuleCS = @"at ([\w\W]*):(\d+)\)";

        static readonly string regexRuleLUA =  @"([a-zA-Z])?:?\\.+(.lua):(\d+)?"; // @"(.*):(\d+)";

        /// <summary>
        /// 避免跳转到封装的Debugger中
        /// </summary>
        static readonly string[] CS_IGNORE_LINE = {"UnityEngine.Debug:", "Debugger:Log"}; //Debugger是自己封装的，有则替换

        /// <summary>
        /// 自定义要忽略的堆栈信息
        /// </summary>
        static readonly string[] LUA_IGNORE_LINE =
        {
            "[C]:",
            "stack traceback:",
            "in metamethod \'__index\'",
            "in function \'__index\'",
            "in function \'__newindex\'",
            "in function \'__add\'",
            "in function \'log\'",
            "in function \'print\'",
            "in function \'error\'",
            " in function \'warn\'",
            "in function \'__lt\'",

            "in function \'__concat\'",
            "event:",
            "in function \'__len\'",
            " in function \'__sub\'",
            "in function \'__div\'",

        };

        #region 编辑器字段

        const string EXTERNAL_EDITOR_PATH_KEY = "";
        const string LUA_PROJECT_File_Key = "";

        #endregion

        #region 反射获取ConsoleWindow操作方法

        static Type m_ConsoleWindowType = null;
        static FieldInfo m_ActiveTextInfo;
        static FieldInfo m_ConsoleWindowFileInfo;
        static object consoleWindow;

        static void GetReflectionInfo()
        {
            m_ConsoleWindowType = Type.GetType("UnityEditor.ConsoleWindow,UnityEditor");
            m_ActiveTextInfo =
                m_ConsoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
            m_ConsoleWindowFileInfo =
                m_ConsoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            consoleWindow = m_ConsoleWindowFileInfo.GetValue(null);
        }

        #endregion


        /// <summary>
        /// 双击console的回调
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="line">Unity原生解析出来的行号</param>
        /// <returns></returns>
        [OnOpenAssetAttribute(1)]
        public static bool OnOpen(int instanceID, int line)
        {
            GetReflectionInfo();
            if ((object) EditorWindow.focusedWindow != consoleWindow)
            {
                return false;
            }

            return FindCode();
        }

        /// <summary>
        /// 解析堆栈信息，找到跳转文件和行号
        /// 默认跳转到第一条非忽略文本
        /// </summary>
        /// <returns></returns>
        static bool FindCode()
        {
            var windowInstance = m_ConsoleWindowFileInfo.GetValue(null);
            var activeText = m_ActiveTextInfo.GetValue(windowInstance);
            string[] contentStrings = activeText.ToString().Split('\n');

            if (contentStrings.Length < 2) return false;

            bool isLuaLog = IsLuaLog(contentStrings[1]);

            for (int index = 0; index < contentStrings.Length; index++)
            {
                if (isLuaLog)
                {
                    if (ContainsIgnoreLine(contentStrings[index], false))
                    {
                        continue;
                    }
                }
                else
                {
                    if (ContainsIgnoreLine(contentStrings[index], true))
                    {
                        continue;
                    }
                }

                return PingAndOpen(contentStrings[index]);
            }

            return false;
        }

        /// <summary>
        /// 是否是需要忽略的行
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isDefalut"></param>
        /// <returns></returns>
        static bool ContainsIgnoreLine(string text, bool isCSlog)
        {
            string[] ignoreLines = isCSlog ? CS_IGNORE_LINE : LUA_IGNORE_LINE;
            for (int i = 0; i < ignoreLines.Length; i++)
            {
                if (text.Trim().Contains(ignoreLines[i]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检查是否是Lua日志
        /// </summary>
        /// <param name="contentStrings"></param>
        /// <returns></returns>
        static bool IsLuaLog(string logStr)
        {
            return logStr.Contains("stack traceback:");
        }

        /// <summary>
        /// 匹配文件名和行号
        /// </summary>
        /// <param name="fileContext"></param>
        /// <returns></returns>
        static bool PingAndOpen(string fileContext)
        {
            //fileContext = fileContext.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
            fileContext = fileContext.Replace("\n", "").Replace("/", "\\").Replace("\t", "").Replace("\r", "");
            string regex;
            bool isCSLog = false;
            if (!fileContext.Contains(".cs"))
            {
                regex = regexRuleLUA;
            }
            else
            {
                /*regex = regexRuleCS;
                isCSLog = true;*/
                return false;
            }

            Match match = Regex.Match(@fileContext, regex);
            if (match.Groups.Count > 1)
            {
                int matchLength = match.Groups[0].Value.Length;
                string matchString = match.Groups[0].Value;
                int splitInex = match.Groups[0].Value.LastIndexOf(':');
                if (splitInex < 0 && splitInex + 1 < matchLength)
                {
                    return false;
                }

                string path =  matchString.Substring(0, splitInex);
                string line =  matchString.Substring(splitInex + 1, matchLength - splitInex - 1);

                //string path = match.Groups[1].Value;
                //string line = match.Groups[2].Value;
                OnOpenAsset(path, int.Parse(line), isCSLog);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 跳转前的检查Lua目录路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool OnOpenAsset(string path, int line, bool csLog)
        {
            //bool isAssetsCS = true;
            /*if (csLog)
            {
                UnityEngine.Object codeObject = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));

                if (codeObject == null)
                {
                    if (!path.StartsWith("Assets"))
                    {
                        string pathRoot = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets"));
                        path = Path.Combine(pathRoot, path);
                        if (File.Exists(path))
                        {
                            isAssetsCS = false;
                        }
                    }
                    else
                    {
                        Debug.LogError(string.Format("【路径不存在】：{0}", path));
                        return false;
                    }
                }
            }*/

            if (!File.Exists(path))
            {
                Debug.LogError(string.Format("【路径不存在】：{0}", path));
                return false;
            }

            var filePath = path; //path.Trim();
            /*if (csLog && isAssetsCS)
            {
                OpenFileByDefault(filePath, line);
                return true;
            }*/

            return OpenFileAtLineExternal(filePath, line);
        }


        /// <summary>
        /// 跳转前检查IDE
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        static bool OpenFileAtLineExternal(string fileName, int line)
        {
            string editorPath = EditorUserSettings.GetConfigValue(EXTERNAL_EDITOR_PATH_KEY);
            if (string.IsNullOrEmpty(editorPath) || !File.Exists(editorPath))
            {
                // 没有path就弹出面板设置
                SetExternalEditorPath();
            }

            OpenFileWith(fileName, line);
            return true;
        }

        /// <summary>
        /// Lua跳转
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="line"></param>
        static void OpenFileWith(string fileName, int line)
        {
            string fileNameString = "\"" + fileName + "\"";
            string editorPath = EditorUserSettings.GetConfigValue(EXTERNAL_EDITOR_PATH_KEY);
            string projectRootPath = EditorUserSettings.GetConfigValue(LUA_PROJECT_File_Key);
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = editorPath;
            string procArgument = "";

            if (editorPath.IndexOf("idea") != -1 || editorPath.IndexOf("rider") != -1)
            {
                procArgument = string.Format("{0} --line {1} {2}", projectRootPath, line, fileNameString);
            }
            else if (editorPath.IndexOf("Code") != -1)
            {
                procArgument = string.Format("-g {0}:{1}:0", fileNameString, line);
            }
            else
            {
                procArgument = string.Format("{0}:{1}:0", fileNameString, line);
            }

            proc.StartInfo.Arguments = procArgument;
            proc.Start();
        }

        /// <summary>
        /// Cs调用Unity原生打开方式
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="line"></param>
        public static void OpenFileByDefault(string filePath, int line)
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(filePath, typeof(MonoScript));
            AssetDatabase.OpenAsset(obj, line);
        }

    }
}