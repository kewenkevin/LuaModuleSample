using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using ND.Managers.ResourceMgr.Framework;
using ND.Managers.ResourceMgr.Runtime;
using UnityEditor;
using UnityEngine;

namespace ND.Managers.ResourceMgr.Editor
{
    public class EditorUtilityx
    {
        private static DateTime m_NextDisplayProgressBarTime;
        public static float IntervalDisplayTime = 0.03f;
        private static Dictionary<string, Regex> m_CachedRegex;
        private static Dictionary<System.Type, MemberInfo[]> m_SerializableMembers;

        private static string packageDir;
        internal const string MenuPrefix = "Tools/ResourceManager/";
        internal const string LogLevelMenuPrefix = EditorUtilityx.MenuPrefix + "Log Level/";
        internal const string OpenFolderMenuPrefix = EditorUtilityx.MenuPrefix + "Open Folder/";
        internal const string AdvancedMenuPrefix = EditorUtilityx.MenuPrefix + "Advanced/";
        private static string androidSDKHome;
        private static string androidADBPath;
        static Regex SafeCharRegex = new Regex(@"[^A-Za-z0-9_-]");
        static Regex SafeBundleNameCharRegex = new Regex(@"[^A-Za-z0-9_\./-]");

       public const string LogPrefix = "<color=#3ebff4>[Resource]</color> ";

        /// <summary>
        /// 无效的资源类型
        /// </summary>
        static HashSet<System.Type> invalidAssetTypes = new HashSet<System.Type>(new System.Type[] {
            typeof(PluginImporter) ,    //.dll
             typeof(MonoImporter),      //.cs
            });


        /// <summary>
        /// 资源包模块目录
        /// </summary>
        public static string PackageDir
        {
            get
            {
                if (string.IsNullOrEmpty(packageDir))
                {
                    packageDir = GetPackageDirectory(ResourceSettings.PackageName);
                }
                return packageDir;
            }
        }

        public static string AndroidSDKHome
        {
            get
            {

                if (androidSDKHome == null)
                {
                    androidSDKHome = EditorPrefs.GetString("AndroidSdkRoot");
                    if (string.IsNullOrEmpty(androidSDKHome))
                    {
                        androidSDKHome = Environment.GetEnvironmentVariable("ANDROID_HOME");
                    }
                    if (androidSDKHome == null)
                        androidSDKHome = string.Empty;
                }
                return androidSDKHome;
            }
            set { EditorPrefs.SetString("AndroidSdkRoot", value); }
        }

        public static string AndroidADBPath
        {
            get
            {
                if (androidADBPath == null)
                {
                    string sdkHome = AndroidSDKHome;
                    if (!string.IsNullOrEmpty(sdkHome))
                    {
                        androidADBPath = Path.Combine(AndroidSDKHome, "platform-tools/adb.exe");
                    }
                    if (androidADBPath == null)
                        androidADBPath = "adb.exe";
                }
                return androidADBPath;
            }
        }

        internal static string GetPackageDirectory(string packageName)
        {
            foreach (var dir in Directory.GetDirectories("Assets", packageName, SearchOption.AllDirectories))
            {
                if (File.Exists(Path.Combine(dir, "package.json")))
                {
                    return dir;
                }
            }

            string path = Path.Combine("Packages", packageName);
            if (File.Exists(Path.Combine(path, "package.json")))
            {
                return path;
            }

            return null;
        }

        #region 检查 prefab 丢失

        public static IEnumerable<string> FindMissingPrefabs(string assetPath)
        {
            foreach (var guid in GetDependencyPrefabs(assetPath))

                if (string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(guid)))
                {
                    yield return guid;
                }
        }
        public static IEnumerable<string> GetDependencyPrefabs(string assetPath)
        {

            using (var fs = File.Open(assetPath, FileMode.Open, FileAccess.Read))
            using (var stream = new StreamReader(fs))
            {
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    string guid = FindFileGuid(line, "m_SourcePrefab");
                    if (guid != null)
                    {
                        yield return guid;
                    }
                }
            }
        }

        public static string FindFileGuid(string line, string type)
        {
            if (string.IsNullOrEmpty(line))
                return null;
            int len = line.Length;
            if (len == 0)
                return null;
            int index = 0;

            //跳过空白
            while (index < len && line[index] == ' ')
                index++;
            string str;
            str = type;
            if (!StartsWith(line, str, index))
                return null;
            index += str.Length;
            str = "guid: ";
            index = line.IndexOf(str, index);
            if (index == -1)
                return null;
            index += str.Length;
            int endIndex = line.IndexOf(',', index);
            if (endIndex == -1)
                return null;
            return line.Substring(index, endIndex - index);
        }
        static bool StartsWith(string str, string value, int startIndex)
        {
            int valueLen = value.Length;
            int len = str.Length;
            if (startIndex + valueLen > len)
                return false;
            for (int i = 0; i < valueLen; i++)
            {
                if (str[startIndex + i] != value[i])
                    return false;
            }
            return true;
        }

        #endregion

        /// <summary>
        /// 用于高频率调用的进度条信息，降低显示频率，提高性能
        /// </summary>
        public static void DisplayProgressBar(string title, string info, float progress)
        {
            if (DateTime.Now > m_NextDisplayProgressBarTime)
            {
                m_NextDisplayProgressBarTime = DateTime.Now.AddSeconds(IntervalDisplayTime);
                EditorUtility.DisplayProgressBar(title, info, progress);
            }
        }

        public static void ClearProgressBar()
        {
            EditorUtility.ClearProgressBar();
            m_NextDisplayProgressBarTime = DateTime.MinValue;
        }

        public static bool IncludeExclude(string input, string includePattern, string excludePattern)
        {
            if (!string.IsNullOrEmpty(includePattern) && !GetOrCacheRegex(includePattern).IsMatch(input))
                return false;

            if (!string.IsNullOrEmpty(excludePattern) && GetOrCacheRegex(excludePattern).IsMatch(input))
                return false;
            return true;
        }

        /// <summary>
        /// 缓存正则表达式
        /// </summary>
        public static Regex GetOrCacheRegex(string pattern)
        {
            if (m_CachedRegex == null)
                m_CachedRegex = new Dictionary<string, Regex>();
            Regex regex;
            if (!m_CachedRegex.TryGetValue(pattern, out regex))
            {
                regex = new Regex(pattern, RegexOptions.IgnoreCase);
                m_CachedRegex[pattern] = regex;
            }
            return regex;
        }

        #region edit serializable object

        internal static void GUISerializableObject(object target)
        {
            if (target == null)
                return;

            foreach (var mInfo in GetSerializableMembers(target.GetType()))
            {
                if (mInfo.IsDefined(typeof(HideInInspector), true))
                    continue;

                object value = null, newValue = null;
                System.Type valueType = null;
                if (mInfo.MemberType == MemberTypes.Field)
                {
                    FieldInfo fInfo = (FieldInfo)mInfo;
                    value = fInfo.GetValue(target);
                    valueType = fInfo.FieldType;
                }

                if (valueType == null)
                    continue;

                GUIContent label = new GUIContent();

                label.text = mInfo.Name;

                var labelAttr = mInfo.GetCustomAttribute<LabelAttribute>(true);
                if (labelAttr != null && !string.IsNullOrEmpty(labelAttr.Label))
                    label.text = labelAttr.Label;
                label.text = GetDisplayLabel(label.text);

                var toolipAttr = mInfo.GetCustomAttribute<TooltipAttribute>(true);
                if (toolipAttr != null)
                    label.tooltip = toolipAttr.tooltip;

                newValue = value;
                var oldChanged = GUI.changed;
                GUI.changed = false;
                if (valueType.IsArray)
                {
                    var elemType = valueType.GetElementType();

                    if (elemType == typeof(string))
                    {
                        newValue = new GUIContent(label).ArrayField(value as IList<string>, (v, index) =>
                        {
                            return (string)EditField(GUIContent.none, v, elemType);
                        }, initExpand: true);
                    }
                    else if (elemType == typeof(int))
                    {
                        newValue = new GUIContent(label).ArrayField(value as IList<int>, (v, index) =>
                        {
                            return (int)EditField(GUIContent.none, v, elemType);
                        }, initExpand: true);
                    }
                    else if (elemType == typeof(float))
                    {
                        newValue = new GUIContent(label).ArrayField(value as IList<float>, (v, index) =>
                        {
                            return (float)EditField(GUIContent.none, v, elemType);
                        }, initExpand: true);
                    }
                    else if (elemType == typeof(bool))
                    {
                        newValue = new GUIContent(label).ArrayField(value as IList<bool>, (v, index) =>
                        {
                            return (bool)EditField(GUIContent.none, v, elemType);
                        }, initExpand: true);
                    }
                    if (newValue != value)
                    {
                        GUI.changed = true;
                    }
                    else
                    {
                        if (!(value as IEnumerable).ItemsEquals(newValue as IEnumerable))
                            GUI.changed = true;
                    }
                }
                else
                {
                    newValue = EditField(label, value, valueType);

                }
                bool changed = GUI.changed;
                GUI.changed = oldChanged | changed;
                if (changed || !object.Equals(value, newValue))
                {
                    if (mInfo.MemberType == MemberTypes.Field)
                    {
                        FieldInfo fInfo = (FieldInfo)mInfo;
                        fInfo.SetValue(target, newValue);
                    }
                }

            }

        }

        /// <summary>
        /// 首字母大写，大写字母前加空格
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        internal static string GetDisplayLabel(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            string newText = text;
            int spaceCount = 0;
            for (int i = 1; i < text.Length; i++)
            {
                if ('A' <= text[i] && text[i] <= 'Z' && text[i - 1] != ' ')
                    spaceCount++;
            }
            if (spaceCount > 0)
            {
                char[] chs;
                chs = new char[text.Length + spaceCount];
                if ('a' <= text[0] && text[0] <= 'z')
                    chs[0] = (char)(text[0] + ('A' - 'a'));
                else
                    chs[0] = text[0];
                for (int i = 1, j = 1; i < text.Length; i++)
                {
                    if ('A' <= text[i] && text[i] <= 'Z' && text[i - 1] != ' ')
                        chs[j++] = ' ';
                    chs[j++] = text[i];
                }
                newText = new string(chs);
            }
            else
            {
                if ('a' <= text[0] && text[0] <= 'z')
                {
                    newText = (text[0]).ToString().ToUpper() + text.Substring(1);
                }
            }
            return newText;
        }

        internal static object EditField(GUIContent label, object value, System.Type valueType)
        {
            object newValue = value;
            if (valueType == typeof(string))
            {
                newValue = EditorGUILayout.DelayedTextField(label, value as string);
            }
            else if (valueType == typeof(float))
            {
                newValue = EditorGUILayout.DelayedFloatField(label, (float)value);
            }
            else if (valueType == typeof(bool))
            {
                newValue = EditorGUILayout.Toggle(label, (bool)value);
            }
            else if (valueType == typeof(int))
            {
                newValue = EditorGUILayout.DelayedIntField(label, (int)value);
            }
            else if (typeof(UnityEngine.Object).IsAssignableFrom(valueType))
            {
                newValue = EditorGUILayout.ObjectField(new GUIContent(label), (UnityEngine.Object)value, valueType, false);
            }
            return newValue;
        }

        static MemberInfo[] GetSerializableMembers(System.Type type)
        {
            MemberInfo[] members;
            if (m_SerializableMembers == null)
                m_SerializableMembers = new Dictionary<System.Type, MemberInfo[]>();
            if (!m_SerializableMembers.TryGetValue(type, out members))
            {
                List<MemberInfo> list = new List<MemberInfo>();
                foreach (var mInfo in type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField))
                {
                    if (mInfo.IsDefined(typeof(NonSerializedAttribute), true))
                        continue;

                    if (mInfo.MemberType == MemberTypes.Field)
                    {
                        FieldInfo fInfo = (FieldInfo)mInfo;
                        if (fInfo.IsInitOnly)
                            continue;
                        if (!fInfo.IsPublic && !fInfo.IsDefined(typeof(SerializeField), true))
                            continue;

                    }
                    else
                    {
                        continue;
                    }
                    list.Add(mInfo);
                }
                members = list.ToArray();

                m_SerializableMembers[type] = members;
            }
            return members;
        }

        #endregion

        public static void OpenFolder(string dir)
        {
            EditorUtility.RevealInFinder(dir);
        }

        /// <summary>
        /// 编辑器使用<see cref="Log"/>
        /// </summary>
        public static void InitLogHelper()
        {
            if (GameFrameworkLog.LogHelper == null)
            {
                GameFrameworkLog.ILogHelper logHelper = new DefaultLogHelper();
                GameFrameworkLog.SetLogHelper(logHelper);
            }
        }

        /// <summary>
        /// 安全字符: A-Z a-z 0-9 _ -
        /// </summary>
        public static string GetSafeChar(string input, string replacement = "_")
        {
            if (string.IsNullOrEmpty(input))
                return input;

            string safeStr;
            safeStr = SafeCharRegex.Replace(input, replacement);
            return safeStr;
        }

        public static string GetSafeBundleName(string bundleName, string replacement = "_", bool toLower = true)
        {
            if (string.IsNullOrEmpty(bundleName))
                return bundleName;

            string safeName;
            safeName = bundleName.Replace('\\', '/');
            safeName = SafeBundleNameCharRegex.Replace(safeName, replacement);
            if (toLower)
                safeName = safeName.ToLower();
            return safeName;
        }

        /// <summary>
        /// 判断是否有效的资源
        /// </summary>
        public static bool IsValidAsset(string assetPath)
        {
            //文件夹
            if (AssetDatabase.IsValidFolder(assetPath))
                return false;

            var importer = AssetImporter.GetAtPath(assetPath);

            //不能识别的文件，.meta
            if (!importer)
                return false;

            var importerType = importer.GetType();
            if (invalidAssetTypes.Contains(importerType))
                return false;

            return true;
        }
    }

    public class LabelAttribute : Attribute
    {
        public LabelAttribute(string label)
        {
            this.Label = label;
        }

        public string Label { get; set; }
    }
}