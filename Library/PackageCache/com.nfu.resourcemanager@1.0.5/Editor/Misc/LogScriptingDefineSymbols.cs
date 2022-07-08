//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using UnityEditor;

namespace ND.Managers.ResourceMgr.Editor
{
    /// <summary>
    /// 日志脚本宏定义。
    /// </summary>
    public static class LogScriptingDefineSymbols
    {
        private const string EnableLogScriptingDefineSymbol = "ENABLE_LOG";
        private const string EnableDebugAndAboveLogScriptingDefineSymbol = "ENABLE_DEBUG_AND_ABOVE_LOG";
        private const string EnableInfoAndAboveLogScriptingDefineSymbol = "ENABLE_INFO_AND_ABOVE_LOG";
        private const string EnableWarningAndAboveLogScriptingDefineSymbol = "ENABLE_WARNING_AND_ABOVE_LOG";
        private const string EnableErrorAndAboveLogScriptingDefineSymbol = "ENABLE_ERROR_AND_ABOVE_LOG";
        private const string EnableFatalAndAboveLogScriptingDefineSymbol = "ENABLE_FATAL_AND_ABOVE_LOG";
        private const string EnableDebugLogScriptingDefineSymbol = "ENABLE_DEBUG_LOG";
        private const string EnableInfoLogScriptingDefineSymbol = "ENABLE_INFO_LOG";
        private const string EnableWarningLogScriptingDefineSymbol = "ENABLE_WARNING_LOG";
        private const string EnableErrorLogScriptingDefineSymbol = "ENABLE_ERROR_LOG";
        private const string EnableFatalLogScriptingDefineSymbol = "ENABLE_FATAL_LOG";

        private static readonly string[] AboveLogScriptingDefineSymbols = new string[]
        {
            EnableDebugAndAboveLogScriptingDefineSymbol,
            EnableInfoAndAboveLogScriptingDefineSymbol,
            EnableWarningAndAboveLogScriptingDefineSymbol,
            EnableErrorAndAboveLogScriptingDefineSymbol,
            EnableFatalAndAboveLogScriptingDefineSymbol
        };

        private static readonly string[] SpecifyLogScriptingDefineSymbols = new string[]
        {
            EnableDebugLogScriptingDefineSymbol,
            EnableInfoLogScriptingDefineSymbol,
            EnableWarningLogScriptingDefineSymbol,
            EnableErrorLogScriptingDefineSymbol,
            EnableFatalLogScriptingDefineSymbol
        };

        /// <summary>
        /// 禁用所有日志脚本宏定义。
        /// </summary>
        [MenuItem(EditorUtilityx.LogLevelMenuPrefix + "Disable All Logs", false, 30)]
        public static void DisableAllLogs()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol(EnableLogScriptingDefineSymbol);

            foreach (string specifyLogScriptingDefineSymbol in SpecifyLogScriptingDefineSymbols)
            {
                ScriptingDefineSymbols.RemoveScriptingDefineSymbol(specifyLogScriptingDefineSymbol);
            }

            foreach (string aboveLogScriptingDefineSymbol in AboveLogScriptingDefineSymbols)
            {
                ScriptingDefineSymbols.RemoveScriptingDefineSymbol(aboveLogScriptingDefineSymbol);
            }
        }

        /// <summary>
        /// 开启所有日志脚本宏定义。
        /// </summary>
        [MenuItem(EditorUtilityx.LogLevelMenuPrefix + "Enable All Logs", false, 31)]
        public static void EnableAllLogs()
        {
            DisableAllLogs();
            ScriptingDefineSymbols.AddScriptingDefineSymbol(EnableLogScriptingDefineSymbol);
        }
        [MenuItem(EditorUtilityx.LogLevelMenuPrefix + "Enable All Logs", validate = true)]
        private static bool EnableAllLogs_Validate()
        {
            Menu.SetChecked(EditorUtilityx.LogLevelMenuPrefix + "Enable All Logs", ScriptingDefineSymbols.HasScriptingDefineSymbol(EnableLogScriptingDefineSymbol));
            return true;
        }

        /// <summary>
        /// 开启调试及以上级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(EditorUtilityx.LogLevelMenuPrefix + "Enable Debug And Above Logs", false, 32)]
        public static void EnableDebugAndAboveLogs()
        {
            SetAboveLogScriptingDefineSymbol(EnableDebugAndAboveLogScriptingDefineSymbol);
        }
        [MenuItem(EditorUtilityx.LogLevelMenuPrefix + "Enable Debug And Above Logs", validate = true)]
        private static bool EnableDebugAndAboveLogs_Validate()
        {
            Menu.SetChecked(EditorUtilityx.LogLevelMenuPrefix + "Enable Debug And Above Logs", ScriptingDefineSymbols.HasScriptingDefineSymbol(EnableDebugAndAboveLogScriptingDefineSymbol));
            return true;
        }

        /// <summary>
        /// 开启信息及以上级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(EditorUtilityx.LogLevelMenuPrefix + "Enable Info And Above Logs", false, 33)]
        public static void EnableInfoAndAboveLogs()
        {
            SetAboveLogScriptingDefineSymbol(EnableInfoAndAboveLogScriptingDefineSymbol);
        }
        [MenuItem(EditorUtilityx.LogLevelMenuPrefix + "Enable Info And Above Logs", validate = true)]
        private static bool EnableInfoAndAboveLogs_Validate()
        {
            Menu.SetChecked(EditorUtilityx.LogLevelMenuPrefix + "Enable Info And Above Logs", ScriptingDefineSymbols.HasScriptingDefineSymbol(EnableInfoAndAboveLogScriptingDefineSymbol));
            return true;
        }
        /// <summary>
        /// 开启警告及以上级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(EditorUtilityx.LogLevelMenuPrefix + "Enable Warning And Above Logs", false, 34)]
        public static void EnableWarningAndAboveLogs()
        {
            SetAboveLogScriptingDefineSymbol(EnableWarningAndAboveLogScriptingDefineSymbol);
        }
        [MenuItem(EditorUtilityx.LogLevelMenuPrefix + "Enable Warning And Above Logs", validate = true)]
        private static bool EnableWarningAndAboveLogs_Validate()
        {
            Menu.SetChecked(EditorUtilityx.LogLevelMenuPrefix + "Enable Warning And Above Logs", ScriptingDefineSymbols.HasScriptingDefineSymbol(EnableWarningAndAboveLogScriptingDefineSymbol));
            return true;
        }

        /// <summary>
        /// 开启错误及以上级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(EditorUtilityx.LogLevelMenuPrefix + "Enable Error And Above Logs", false, 35)]
        public static void EnableErrorAndAboveLogs()
        {
            SetAboveLogScriptingDefineSymbol(EnableErrorAndAboveLogScriptingDefineSymbol);
        }
        [MenuItem(EditorUtilityx.LogLevelMenuPrefix + "Enable Error And Above Logs", validate = true)]
        private static bool EnableErrorAndAboveLogs_Validate()
        {
            Menu.SetChecked(EditorUtilityx.LogLevelMenuPrefix + "Enable Error And Above Logs", ScriptingDefineSymbols.HasScriptingDefineSymbol(EnableErrorAndAboveLogScriptingDefineSymbol));
            return true;
        }

        /// <summary>
        /// 开启严重错误及以上级别的日志脚本宏定义。
        /// </summary>
        [MenuItem(EditorUtilityx.LogLevelMenuPrefix + "Enable Fatal And Above Logs", false, 36)]
        public static void EnableFatalAndAboveLogs()
        {
            SetAboveLogScriptingDefineSymbol(EnableFatalAndAboveLogScriptingDefineSymbol);
        }
        [MenuItem(EditorUtilityx.LogLevelMenuPrefix + "Enable Fatal And Above Logs", validate = true)]
        private static bool EnableFatalAndAboveLogs_Validate()
        {
            Menu.SetChecked(EditorUtilityx.LogLevelMenuPrefix + "Enable Fatal And Above Logs", ScriptingDefineSymbols.HasScriptingDefineSymbol(EnableFatalAndAboveLogScriptingDefineSymbol));
            return true;
        }

        /// <summary>
        /// 设置日志脚本宏定义。
        /// </summary>
        /// <param name="aboveLogScriptingDefineSymbol">要设置的日志脚本宏定义。</param>
        public static void SetAboveLogScriptingDefineSymbol(string aboveLogScriptingDefineSymbol)
        {
            if (string.IsNullOrEmpty(aboveLogScriptingDefineSymbol))
            {
                return;
            }

            foreach (string i in AboveLogScriptingDefineSymbols)
            {
                if (i == aboveLogScriptingDefineSymbol)
                {
                    DisableAllLogs();
                    ScriptingDefineSymbols.AddScriptingDefineSymbol(aboveLogScriptingDefineSymbol);
                    return;
                }
            }
        }

        /// <summary>
        /// 设置日志脚本宏定义。
        /// </summary>
        /// <param name="specifyLogScriptingDefineSymbols">要设置的日志脚本宏定义。</param>
        public static void SetSpecifyLogScriptingDefineSymbols(string[] specifyLogScriptingDefineSymbols)
        {
            if (specifyLogScriptingDefineSymbols == null || specifyLogScriptingDefineSymbols.Length <= 0)
            {
                return;
            }

            bool removed = false;
            foreach (string specifyLogScriptingDefineSymbol in specifyLogScriptingDefineSymbols)
            {
                if (string.IsNullOrEmpty(specifyLogScriptingDefineSymbol))
                {
                    continue;
                }

                foreach (string i in SpecifyLogScriptingDefineSymbols)
                {
                    if (i == specifyLogScriptingDefineSymbol)
                    {
                        if (!removed)
                        {
                            removed = true;
                            DisableAllLogs();
                        }

                        ScriptingDefineSymbols.AddScriptingDefineSymbol(specifyLogScriptingDefineSymbol);
                        break;
                    }
                }
            }
        }
    }
}
