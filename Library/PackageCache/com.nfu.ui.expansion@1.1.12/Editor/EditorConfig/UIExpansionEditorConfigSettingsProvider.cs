using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ND.UI
{
    public class UIExpansionEditorConfigSettingsProvider : SettingsProvider
    {
        private const string ASSET_NAME = "EditorConfigAsset";
        private const string PATH_POSTFIX = ".asset";
        private const string NONE_OPTION_NAME = "<None>";
        private const string SETTING_PATH = "NFU/UI/UIExpansion Setting";
        
        private const string EXPORT_PATH = "/Packages/com.nfu.ui.expansion/Export~";
        private const string TARGET_PATH = "/Assets/GameSettings/NFUSettings/UIExpansion/Editor";
        
        private UIExpansionEditorConfig m_config;
        private static bool s_uiExpansionIsPureMore;
        private static List<string> s_luaExporterTypeNames;
        private int m_luaExporterTypeNameIndex;
        
        UIExpansionEditorConfigSettingsProvider(string path, SettingsScope scope) : base(path, scope)
        {
        }

        [SettingsProvider]
        private static SettingsProvider CreateMyCustomSettingsProvider()
        {
            return new UIExpansionEditorConfigSettingsProvider(SETTING_PATH, SettingsScope.Project);
        }

        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <returns></returns>
        public static UIExpansionEditorConfig GetConfig()
        {
            return AssetDatabase.LoadAssetAtPath<UIExpansionEditorConfig>(GetConfigPath());
        }

        /// <summary>
        /// 是否是简洁模式
        /// </summary>
        /// <returns></returns>
        public static bool IsPureMode()
        {
            return s_uiExpansionIsPureMore;
        }

        /// <summary>
        /// 外部打开设置面板
        /// </summary>
        public static void OpenSettingsProvider()
        {
            foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = ass.GetType("UnityEditor.SettingsWindow");
                if (type != null)
                {
                    var mGetWindow = typeof(EditorWindow).GetMethod("GetWindow", new[] { typeof(Type) });
                    var mSelectProviderByName = type.GetMethod("SelectProviderByName", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (mGetWindow != null && mSelectProviderByName != null)
                    {
                        mSelectProviderByName.Invoke(mGetWindow.Invoke(null, new object[] { type }), new object[] { SETTING_PATH });
                    }

                    break;
                }
            }
        }

        public override void OnGUI(string searchContext)
        {
            PaintIsPureModeSettingsArea();
            PaintLuaExportSettingsArea();
            PaintLuaSettingsArea();
            PaintUIAnimationSettingsArea();
            //PaintSaveArea();
        }

        /// <summary>
        /// 初始化面板配置数据
        /// </summary>
        /// <param name="searchContext"></param>
        /// <param name="rootElement"></param>
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);

            
            //先创建GameSettings文件夹
            if (!AssetDatabase.IsValidFolder("Assets" + "/GameSettings"))
            {
                AssetDatabase.CreateFolder("Assets",  "GameSettings");
            }
            
            //先创建存放配置的目录
            if (!AssetDatabase.IsValidFolder("Assets" + "/GameSettings/NFUSettings"))
            {
                AssetDatabase.CreateFolder("Assets/GameSettings", "NFUSettings");
            }

            if (!AssetDatabase.IsValidFolder("Assets" + "/GameSettings/NFUSettings/UIExpansion"))
            {
                AssetDatabase.CreateFolder("Assets" + "/GameSettings/NFUSettings", "UIExpansion");
            }

            //创建配置
            m_config = AssetDatabase.LoadAssetAtPath<UIExpansionEditorConfig>(GetConfigPath());
            if (m_config == null)
            {
                m_config = ScriptableObject.CreateInstance<UIExpansionEditorConfig>();
                AssetDatabase.CreateAsset(m_config, GetConfigPath());
            }
            
            // //删除遗留导出类
            // string oldLuaExporter = Application.dataPath + "/GameSettings/NFUSettings/UIExpansion/Editor/ToLuaUIScriptExporter.cs";
            // if (File.Exists(oldLuaExporter))
            // {
            //     File.Delete(oldLuaExporter);
            // }
            
            //拷贝默认页面模板txt到Asset目录下
            string path = Application.dataPath + "/GameSettings/NFUSettings/UIExpansion/Editor/PageTemplate.txt";
            string path2 = Application.dataPath + "/GameSettings/NFUSettings/UIExpansion/Editor/ModuleTemplate.txt";
            string path3 = Application.dataPath + "/GameSettings/NFUSettings/UIExpansion/Editor/WidgetTemplate.txt";
            string path4 = Application.dataPath + "/GameSettings/NFUSettings/UIExpansion/Editor/CustomLuaExporter.cs";

            if (!File.Exists(path) || !File.Exists(path2) || !File.Exists(path3) || !File.Exists(path4))
            {
                string exportPath = GetAbsolutePath(EXPORT_PATH);
                string targetPath = GetAbsolutePath(TARGET_PATH);
                CopyAssetToOtherPath(exportPath, targetPath);
            }
            
            
            
            InitLuaExporterSettings();


            if (EditorPrefs.HasKey("UIExpansion.IsPureMode"))
            {
                s_uiExpansionIsPureMore = EditorPrefs.GetBool("UIExpansion.IsPureMode");
            }
            else
            {
                EditorPrefs.SetBool("UIExpansion.IsPureMode", s_uiExpansionIsPureMore);
            }

            AssetDatabase.Refresh();
        }
        
        /// <summary>
        /// 切换面板或者关闭面板自动保存配置
        /// </summary>
        public override void OnDeactivate()
        {
            base.OnDeactivate();
            if (m_config != null)
            {

                EditorPrefs.SetBool("UIExpansion.IsPureMode", s_uiExpansionIsPureMore);
                EditorUtility.SetDirty(m_config);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// UIExpansion配置文件路径
        /// </summary>
        /// <returns></returns>
        public static string GetConfigPath()
        {
            return $"Assets/GameSettings/NFUSettings/UIExpansion/{ASSET_NAME}{PATH_POSTFIX}";
        }

        /// <summary>
        /// 初始化lua导出类
        /// </summary>
        private void InitLuaExporterSettings()
        {
            if (m_config.luaFolderPathArray.Count == 0)
            {
                m_config.luaFolderPathArray.Add("");
            }

            if (s_luaExporterTypeNames == null)
            {
                s_luaExporterTypeNames = new List<string>()
                {
                    NONE_OPTION_NAME
                };
                s_luaExporterTypeNames.AddRange(TypeUtils.GetEditorTypeNames(typeof(ILuaExporter)));
            }

            for (int i = 0; i < s_luaExporterTypeNames.Count; i++)
            {
                if (s_luaExporterTypeNames[i] == m_config.luaExporterClass)
                {
                    m_luaExporterTypeNameIndex = i;
                    break;
                }
            }
        }

        /// <summary>
        /// lua导出类配置
        /// </summary>
        private void PaintLuaExportSettingsArea()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField($"Lua导出类", GUILayout.Width(150));
                if (s_luaExporterTypeNames == null)
                {
                    return;
                }

                string[] names = s_luaExporterTypeNames.ToArray();
                int selectedIndex = EditorGUILayout.Popup(m_luaExporterTypeNameIndex, names);
                if (selectedIndex != m_luaExporterTypeNameIndex)
                {
                    m_luaExporterTypeNameIndex = selectedIndex;
                    m_config.luaExporterClass = selectedIndex <= 0 ? string.Empty : names[selectedIndex];
                }
            }
            EditorGUILayout.EndHorizontal();
            
            m_config.goPrefix = EditorGUILayout.TextField("导出对象名称前缀", m_config.goPrefix);

            string[] nfuClass =  TypeUtils.GetEditorTypeNames(typeof(NFULuaExporter));
            //继承nfuLuaExporter显示配置，否则不显示
            if ( nfuClass.Contains(m_config.luaExporterClass))
            {
                if (m_config.pageTemplate == null || m_config.moduleTemplate == null || m_config.widgetTemplate == null)
                {
                    //配置引用模板txt
                    m_config.pageTemplate =  AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameSettings/NFUSettings/UIExpansion/Editor/PageTemplate.txt");
                    m_config.moduleTemplate =  AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameSettings/NFUSettings/UIExpansion/Editor/ModuleTemplate.txt");
                    m_config.widgetTemplate =  AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameSettings/NFUSettings/UIExpansion/Editor/WidgetTemplate.txt");
                }
                
                m_config.pageTemplate = (TextAsset)EditorGUILayout.ObjectField("NFU默认页面Page模板",m_config.pageTemplate, typeof(TextAsset), false);
                m_config.moduleTemplate = (TextAsset)EditorGUILayout.ObjectField("NFU默认子模块Module模板",m_config.moduleTemplate, typeof(TextAsset), false);
                m_config.widgetTemplate = (TextAsset)EditorGUILayout.ObjectField("NFU默认挂件Widget模板",m_config.widgetTemplate, typeof(TextAsset), false);
                
                m_config.nfuModule = EditorGUILayout.TextField("NFU默认module绑定", m_config.nfuModule);
                EditorGUILayout.LabelField("                                                   (AAA是module的GameObject名称占位符，BBB是module类名占位符)");
                EditorGUILayout.Space(4);
                m_config.nfuCallback = EditorGUILayout.TextField("NFU默认Lua回调", m_config.nfuCallback);
                EditorGUILayout.LabelField("                                                   (?????是按钮GameObject占位符)");
                EditorGUILayout.Space(4);
            }
        }

        /// <summary>
        /// UIExpansion配置
        /// </summary>
        private void PaintIsPureModeSettingsArea()
        {
            EditorGUILayout.BeginHorizontal();
            {
                //GUILayout.Space(10);
                GUILayout.Label("强制开启UIExpansion简洁模式(Pure Mode)");
                s_uiExpansionIsPureMore = EditorGUILayout.Toggle(s_uiExpansionIsPureMore);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// lua配置
        /// </summary>
        private void PaintLuaSettingsArea()
        {
            EditorGUILayout.LabelField($"Lua文件路径目录");
            for (int index = 0; index < m_config.luaFolderPathArray.Count; index++)
            {
                if (index >= m_config.luaFolderPathArray.Count)
                {
                    break;
                }

                string newFolderPath = SetFolderPath(m_config.luaFolderPathArray[index], "Lua文件路径", index);

                if (!m_config.luaFolderPathArray[index].Equals(newFolderPath))
                {
                    m_config.luaFolderPathArray[index] = newFolderPath;
                    //EditorUtility.DisplayDialog("修改根目录成功", "请手动把Lua文件按照原目录层级移至对应根目录下，防止文件定位功能失效", "确定");
                }

                GUILayout.Space(4);
            }
        }

        /// <summary>
        /// 设置lua路径
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string SetFolderPath(string folderPath, string type, int index)
        {
            string fullPath = string.IsNullOrEmpty(folderPath) ? "" : Path.GetFullPath(Path.Combine(Application.dataPath, folderPath));
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("", fullPath);
                if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(20f)))
                {
                    m_config.luaFolderPathArray.Add("");
                    Repaint();
                }

                if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(20f)))
                {
                    if (m_config.luaFolderPathArray.Count > 1)
                    {
                        m_config.luaFolderPathArray.RemoveAt(index);
                        Repaint();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("修改", EditorStyles.miniButton, GUILayout.Width(60f)))
                {
                    string path;
                    //TODO 以下打开路径可以自定义
                    if (string.IsNullOrEmpty(fullPath))
                    {
                        path = EditorUtility.OpenFolderPanel($"请选择{type}的目录", Application.dataPath, "");
                    }
                    else
                    {
                        path = EditorUtility.OpenFolderPanel($"请选择{type}的目录", fullPath, "");
                    }

                    if (!string.IsNullOrEmpty(path))
                    {
                        path = FolderBrowserHelper.GetRelativePath(path, Application.dataPath);

                        folderPath = path;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("当前指定的目录不存在", "请修改目录", "确定");
                    }
                }

                if (GUILayout.Button("定位", EditorStyles.miniButton, GUILayout.Width(60f)))
                {
                    if (!string.IsNullOrEmpty(fullPath))
                    {   
                        //MacOS使用此方法打开目录会报错
                        //FolderBrowserHelper.OpenFolder($@"{fullPath}/");
                        
                        EditorUtility.RevealInFinder(fullPath);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("当前指定的目录不存在", "请修改目录", "确定");
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            return folderPath;
        }

        /// <summary>
        /// 默认打开关闭动画配置
        /// </summary>
        private void PaintUIAnimationSettingsArea()
        {
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("界面默认打开动画", GUILayout.Width(150));
                    m_config.defaultOpenAni = EditorGUILayout.ObjectField(m_config.defaultOpenAni,
                        typeof(AnimationClip), false, GUILayout.Width(350)) as AnimationClip;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("界面默认关闭动画", GUILayout.Width(150));
                    m_config.defaultCloseAni =
                        EditorGUILayout.ObjectField(m_config.defaultCloseAni, typeof(AnimationClip), false, GUILayout.Width(350)) as AnimationClip;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        
        /// <summary>
        /// 复制文件到其他目录
        /// </summary>
        /// <param name="assetDir"></param>
        /// <param name="aimDir"></param>
        private static void CopyAssetToOtherPath(string assetDir, string aimDir)
        {
            if (Directory.Exists(assetDir))
            {
                string[] filesPath = Directory.GetFiles(assetDir);
                filesPath = filesPath.Where(x => !x.EndsWith(".meta")).ToArray();
                if (!Directory.Exists(aimDir))
                {
                    Directory.CreateDirectory(aimDir);
                }

                foreach (string path in filesPath)
                {
                    string name = Path.GetFileName(path);
                    string aim = Path.Combine(aimDir, name);
                    if (!File.Exists(aim))
                    {
                        File.Copy(path, aim);
                    }
                    
                }

                string[] dirArr = Directory.GetDirectories(assetDir);
                if (dirArr.Length > 0)
                {
                    foreach (string directory in dirArr)
                    {
                        string outPath = $"{aimDir}/{Path.GetFileName(directory)}";
                        CopyAssetToOtherPath(directory, outPath);
                    }
                }
            }
        }
        
        /// <summary>
        /// 获取绝对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetAbsolutePath(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            string[] arr = path.Split('/');
            string resultPath = Path.GetDirectoryName(Application.dataPath);
            foreach (string s in arr)
            {
                if (string.Equals(s, ".."))
                {
                    resultPath = Path.GetDirectoryName(resultPath);
                }
            }

            string relativePath = path.Replace("../", "");
            relativePath = Path.Combine(resultPath + relativePath);
            return relativePath;
        }
        
        
        
        // /// <summary>
        // /// 保存配置
        // /// </summary>
        // private void PaintSaveArea()
        // {
        //     GUILayout.Space(20);
        //     if (GUILayout.Button("保存"))
        //     {
        //         //本地化保存UIExpansion显示模式
        //         EditorPrefs.SetBool("UIExpansion.IsPureMode", s_uiExpansionIsPureMore);
        //         EditorUtility.SetDirty(m_config);
        //         AssetDatabase.SaveAssets();
        //         AssetDatabase.Refresh();
        //         Repaint();
        //
        //         EditorUtility.DisplayDialog("保存配置", "保存配置成功", "确定");
        //     }
        // }
        
        
    }
}