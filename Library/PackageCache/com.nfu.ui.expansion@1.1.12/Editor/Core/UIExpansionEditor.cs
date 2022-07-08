using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;
using UnityEditor;

using Object = UnityEngine.Object;

namespace ND.UI
{
    [CustomEditor(typeof(UIExpansion))]
    public class UIExpansionEditor : UnityEditor.Editor
    {
        private UIExpansion _ui;

        private ILuaExporter _luaExporter;

        private GUIStyle lableStyleRed = new GUIStyle();

        private GUIStyle lableStyleBlue = new GUIStyle();

        private const string EnterPureModeStr = "Enter Pure Mode";
        private const string ExitPureModeStr = "Exit Pure Mode";
        private const string OpenUIExpansionWindowStr = "Open UIExpansion Window";
        private const string OpenControllerWindowModeStr = "Open Controller Window";
        
        private string pureModeStr = EnterPureModeStr;
        private string openWindowStr = OpenUIExpansionWindowStr;

        public ILuaExporter LuaExporter
        {
            get
            {
                if (_luaExporter == null)
                {
                    var config = UIExpansionEditorConfigSettingsProvider.GetConfig();
                    if (config == null)
                    {
                        Debug.LogError(
                            "[请先生成UIExpansion Editor Config配置] Edit -> Project Settings... ->UMT/UIExpansion/Editor Config中设置LuaExporter和lua路径");
                    }
                    else
                    {
                        RefreshLuaExporter(config);
                    }
                }

                return _luaExporter;
            }
        }

        SerializedObject m_Object;

        private void OnEnable()
        {
            lableStyleRed.fontSize = 12;
            lableStyleRed.normal.textColor = new Color(0.8f, 0.3f, 0.3f, 1);

            lableStyleBlue.fontSize = 12;
            lableStyleBlue.normal.textColor = new Color(46f / 256f, 163f / 256f, 256f / 256f, 256f / 256f);


            m_Object = new SerializedObject(target);

            RefreshLuaExporter(UIExpansionEditorConfigSettingsProvider.GetConfig());

            //_ui = target as UIExpansion;
            //GenExportComps();

        }

        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();
            _ui = target as UIExpansion;
            if (Application.isPlaying)
            {
                // OnPlayingModeGUI();
                OnEditorModeGUI();
            }
            else
            {
                OnEditorModeGUI();
            }
        }

        private void OnEditorModeGUI()
        {
            if (!_ui.IsPureMode)
            {
                pureModeStr = EnterPureModeStr;
                openWindowStr = OpenUIExpansionWindowStr;
            }
            else
            {
                pureModeStr = ExitPureModeStr;
                openWindowStr = OpenControllerWindowModeStr;
            }
            // if(UIExpansionManager.Instance.CurUIExpansion == _ui)
            // {
            //     EditorGUILayout.HelpBox("当前UI Expansion正在编辑中。", MessageType.Info);
            //     return;
            // }
            // EditorGUILayout.BeginHorizontal();
            if (UIExpansionEditorConfigSettingsProvider.IsPureMode())
            {
                EditorGUILayout.HelpBox("强制简洁模式[开启]中，请在Editor Config面板中设置", MessageType.Info);
            }
            else if (GUILayout.Button(pureModeStr,EditorStyles.miniButton))
            {
                _ui.IsPureMode = !_ui.IsPureMode;
            }
            // if (GUILayout.Button("Open UIExpansion Window",EditorStyles.miniButton))
            // {
            //     
            // }
            // EditorGUILayout.EndHorizontal();
            
            DisplayController();

            DisplayTransition();


            if (GUILayout.Button(openWindowStr))
            {
                UIExpansionWindow window = EditorWindow.GetWindow<UIExpansionWindow>();
                window.ChangeTarget(_ui);
            }

            DisplayLuaBinderAndExporter();

            DisplayYComponents();

        }

        private void DisplayController()
        {
            #region Controller

            if (_ui.ControllerConfigs != null && _ui.ControllerConfigs.Length > 0)
            {
                EditorGUILayout.LabelField("Controllers:", EditorStyles.boldLabel);
                for (int i = 0; i < _ui.ControllerConfigs.Length; i++)
                {
                    ControllerConfig config = _ui.ControllerConfigs[i];
                    int[] optionValues = new int[config.pageNames.Length];

                    for (int j = 0; j < config.pageNames.Length; j++)
                    {
                        optionValues[j] = j;
                    }

                    /*string[] controllerOptionStrs = new string[config.pageNames.Length];
                    for (int j = 0; j < config.pageNames.Length; j++)
                    {
<<<<<<< Updated upstream
                        controllerOptionStrs[j] = string.Format("[{0}] {1} ({2})", j, config.pageNames[j], config.pageTips[j]);
=======
                        controllerOptionStrs[j] = string.Format("[{0}] {1}", j, config.pageNames[j]);
                    }*/
                    
                    /*int tempIndex = EditorGUILayout.IntPopup("    " + config.name, config.selectedIndex,
                        controllerOptionStrs, optionValues);*/
                    
                    GUIContent[] controllerOptionStrs = new GUIContent[config.pageNames.Length];
                    for (int j = 0; j < config.pageNames.Length; j++)
                    {
                        controllerOptionStrs[j] = new GUIContent();
                        controllerOptionStrs[j].text = string.Format("[{0}] {1}", j, config.pageNames[j]);
                        controllerOptionStrs[j].tooltip = config.pageTips[j];
                    }
                    GUIContent configName = new GUIContent("    " + config.name);

                    int tempIndex = EditorGUILayout.IntPopup(configName, config.selectedIndex,
                        controllerOptionStrs, optionValues);
                    if (tempIndex != config.selectedIndex)
                    {
                        config.selectedIndex = tempIndex;
                        _ui.EditorChangeControllerSelectedIndex(config.name, tempIndex);
                        EditorUtility.SetDirty(_ui);
                    }
                }
#if UNITY_2018
                for (int i = 0; i < 8; i++)
                {
                    EditorGUILayout.Space();
                }
#else
                EditorGUILayout.Space(8f);
#endif
            }

            #endregion
        }

        private void DisplayTransition()
        {
            if (_ui.IsPureMode || UIExpansionEditorConfigSettingsProvider.IsPureMode())
            {
                return;
            }
            #region Transition

            if (_ui.TransitionConfigs != null && _ui.TransitionConfigs.Length > 0)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Transition:", EditorStyles.boldLabel);
                if (Application.isPlaying)
                {
                    if (GUILayout.Button("Replay"))
                    {
                        _ui.PlayAllTransitions();
                    }
                }

                EditorGUILayout.EndHorizontal();
                for (int i = 0; i < _ui.TransitionConfigs.Length; i++)
                {
                    TransitionConfig config = _ui.TransitionConfigs[i];
                    GUILayout.BeginHorizontal();
                    if (config.autoPlay)
                    {
                        EditorGUILayout.LabelField("    " + config.name, "Auto Play On");
                    }
                    else
                    {
                        EditorGUILayout.LabelField("    " + config.name);
                    }

                    GUILayout.EndHorizontal();
                }
#if UNITY_2018
                for (int i = 0; i < 8; i++)
                {
                    EditorGUILayout.Space();
                }
#else
                EditorGUILayout.Space(8f);
#endif
            }

            #endregion
        }

        private void DisplayLuaBinderAndExporter()
        {
            if (_ui.IsPureMode || UIExpansionEditorConfigSettingsProvider.IsPureMode())
            {
                return;
            }
            #region Lua Bind and Export

            var editorConfigconfig = UIExpansionEditorConfigSettingsProvider.GetConfig();
            if (editorConfigconfig == null)
            {
                var labelStyle = new GUIStyle();
                labelStyle.normal.textColor = Color.red;
                GUILayout.Label("Can not found any config, Pls fill config first", labelStyle);
                GUILayout.Space(4);
                if (GUILayout.Button("Open UIExpansion Config Setting Window"))
                {
                    UIExpansionEditorConfigSettingsProvider.OpenSettingsProvider();
                }
            }

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("绑定的Lua");
                if (GUILayout.Button("查看Lua根路径", EditorStyles.miniButton, GUILayout.Width(122f)))
                {
                    UIExpansionEditorConfigSettingsProvider.OpenSettingsProvider();
                }
            }

            EditorGUILayout.EndHorizontal();

            bool bRightPath = true;
            
            var path = _ui.LuaBindPath;
            EditorGUILayout.BeginHorizontal();
            {
                if (!string.IsNullOrEmpty(path))
                {
                    
                    if (!IsRightLuaPath())
                    {
                        GUILayout.Label($"Lua根路径下不存Lua脚本,请重新绑定", lableStyleRed);
                        bRightPath = false;
                    }
                    
                    
                    GUILayout.Label(path, lableStyleBlue);
                    
                    
                    
                }
                else
                {
                    GUILayout.Label("尚未绑定，可点击修改选择绑定", lableStyleRed);
                }
            }
            EditorGUILayout.EndHorizontal();

            bool ifExitGui = false;
            EditorGUILayout.BeginHorizontal();
            {
                if (bRightPath && !string.IsNullOrEmpty(path))
                {
                    if (GUILayout.Button("定位", EditorStyles.miniButton, GUILayout.Width(60f)))
                    {
                        var result = EditorUtility.DisplayDialog("选择", "打开文件还是选择目录?", "打开文件", "选择目录");

                        string realPath = "";
                        for (int i = 0; i < editorConfigconfig.luaFolderPathArray.Count; i++)
                        {
                            string tmp = Path.Combine(Application.dataPath,
                                editorConfigconfig.luaFolderPathArray[i],
                                path.Replace(".", "/") + ".lua");
                            if (File.Exists(tmp))
                            {
                                realPath = tmp;
                            }
                        }

                        if (!string.IsNullOrEmpty(realPath))
                        {
                            if (result)
                                EditorUtility.OpenWithDefaultApp(realPath);
                            else
                                EditorUtility.RevealInFinder(realPath);
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("错误", "当前指定的目录不存在!!!", "知道了");
                            path = "";
                        }

                        ifExitGui = true;
                    }

                    if (GUILayout.Button(new GUIContent("解绑"), EditorStyles.miniButton, GUILayout.Width(60f)))
                    {
                        var result = EditorUtility.DisplayDialog("注意", "是否同时删除绑定文件", "删除lua文件", "只解绑");

                        if (result)
                        {
                            string realPath = "";
                            for (int i = 0; i < editorConfigconfig.luaFolderPathArray.Count; i++)
                            {
                                string tmp = Path.Combine(Application.dataPath,
                                    editorConfigconfig.luaFolderPathArray[i],
                                    path.Replace(".", "/") + ".lua");
                                if (File.Exists(tmp))
                                {
                                    realPath = tmp;

                                    break;
                                }
                            }

                            if (!string.IsNullOrEmpty(realPath))
                                File.Delete(realPath);
                        }

                        path = string.Empty;
                        EditorUtility.SetDirty(_ui);

                        ifExitGui = true;
                    }

                    if (GUILayout.Button(new GUIContent("导出"), EditorStyles.miniButton, GUILayout.Width(60f)))
                    {
                        string realPath = "";
                        for (int i = 0; i < editorConfigconfig.luaFolderPathArray.Count; i++)
                        {
                            string tmp = Path.Combine(Application.dataPath,
                                editorConfigconfig.luaFolderPathArray[i],
                                path.Replace(".", "/") + ".lua");
                            if (File.Exists(tmp))
                            {
                                realPath = tmp;

                                break;
                            }
                        }

                        if (!string.IsNullOrEmpty(realPath))
                        {
                            if (!LuaExporter.GenerateFile(_ui, realPath))
                            {
                                EditorUtility.DisplayDialog("错误", "导出失败!!!", "知道了");
                            }
                            else
                            {
                                EditorUtility.DisplayDialog("提示", "导出成功!!!", "知道了");
                            }
                        }

                        ifExitGui = true;
                    }

                    // if (GUILayout.Button(new GUIContent(m_showBtn), EditorStyles.miniButton, GUILayout.Width(60f)))
                    // {
                    //     m_bExpand = !m_bExpand;
                    //     m_showBtn = m_bExpand ? "隐藏" : "展开";
                    //
                    //     GenExportComps();
                    // }
                }
                else
                {
                    if (EditorGUILayout.DropdownButton(new GUIContent("选择"), FocusType.Passive, EditorStyles.miniButton,
                        GUILayout.Width(60f)))
                    {
                        GenericMenu menu = new GenericMenu();
                        if (editorConfigconfig == null)
                        {
                            return;
                        }

                        for (int i = 0; i < editorConfigconfig.luaFolderPathArray.Count; i++)
                        {
                            string folderPath = Path.GetFullPath(Path.Combine(Application.dataPath,
                                editorConfigconfig.luaFolderPathArray[i]));

                            //MacOS地址是'/'，在AddItem中'/'是会被识别成子目录,在Window地址是'\'，故不受影响
                            string displayPath = String.Copy(folderPath);
                            if (displayPath.Contains("/"))
                            {
                                displayPath = displayPath.Replace('/', '\\');
                            }
                            menu.AddItem(new GUIContent(displayPath), false, OnLuaPathSelected, folderPath);
                        }

                        menu.ShowAsContext();

                        ifExitGui = true;
                    }

                    if (EditorGUILayout.DropdownButton(new GUIContent("创建"), FocusType.Passive, EditorStyles.miniButton,
                        GUILayout.Width(60f)))
                    {
                        GenericMenu menu = new GenericMenu();
                        if (editorConfigconfig == null)
                        {
                            return;
                        }

                        for (int i = 0; i < editorConfigconfig.luaFolderPathArray.Count; i++)
                        {
                            string folderPath = Path.GetFullPath(Path.Combine(Application.dataPath,
                                editorConfigconfig.luaFolderPathArray[i]));

                            //MacOS地址是'/'，在AddItem中'/'是会被识别成子目录,在Window地址是'\'，故不受影响
                            string displayPath = String.Copy(folderPath);
                            if (displayPath.Contains("/"))
                            {
                                displayPath = displayPath.Replace('/', '\\');
                            }
                            menu.AddItem(new GUIContent(displayPath), false, OnLuaExportPathSelected, folderPath);
                        }

                        menu.ShowAsContext();

                        ifExitGui = true;
                    }
                }
                _ui.LuaBindPath = path;
            }
            if(ifExitGui)
            {
                GUIUtility.ExitGUI();
            }
            EditorGUILayout.EndHorizontal();

            #endregion
        }

        private void GenExportComps()
        {
            var p = m_Object.FindProperty("_bindedObjects");
            if (p != null)
            {
                for (int i = 0; i < p.arraySize; i++)
                {
                    var element = p.GetArrayElementAtIndex(i);
                    Object o = element.objectReferenceValue;


                    if (o is null)
                    {
                        Debug.Log("存在不存在的绑定组件，请重新刷新控件列表");
                    }
                    else
                    {
                        string toggleName;

                        if (o is GameObject)
                        {
                            toggleName = o.name;
                        }
                        else
                        {
                            var type = o.GetType();
                            var go = (o as Component).gameObject;
                            toggleName = $"{go.name}.{type.Name}";
                        }

                        if (!_ui.BindObjectToggleGroup.ContainsKey(toggleName))
                        {
                            if (toggleName.Contains("NDButton"))
                            {
                                _ui.BindObjectToggleGroup.Add(toggleName, true);
                            }
                            else
                            {
                                _ui.BindObjectToggleGroup.Add(toggleName, false);
                            }
                        }
                    }
                }

                m_showExportComps.Clear();
                foreach (var bindObject in _ui.BindObjectToggleGroup)
                {
                    if (m_bExpand)
                    {
                        m_showExportComps.Add(bindObject.Key, bindObject.Value);
                    }
                    else
                    {
                        if (bindObject.Value)
                        {
                            m_showExportComps.Add(bindObject.Key, bindObject.Value);
                        }
                    }
                }
            }
        }

        private string m_showBtn = "展开";
        private Dictionary<string, bool> m_showExportComps = new Dictionary<string, bool>();
        private bool m_bExpand = false;
        
        private void DisplayYComponents()
        {
            if (_ui.IsPureMode || UIExpansionEditorConfigSettingsProvider.IsPureMode())
            {
                return;
            }
            #region YComponents

            if (GUILayout.Button("刷新控件列表"))
            {
                List<Object> objs = new List<Object>();
                Dictionary<string, int> goNames = new Dictionary<string, int>();
                
                RefreshExportAbleList();

                ColllectTransformComponents(_ui.gameObject, objs, goNames, true);

                bool haveError = false;
                StringBuilder ErrorInfo = new StringBuilder();
                foreach (var kv in goNames)
                {
                    if (kv.Value > 1)
                    {
                        ErrorInfo.AppendLine($"名称【{kv.Key}】重复【{kv.Value}】次");
                        haveError = true;
                    }
                }
                
                
                if (!haveError)
                {
                    _ui.BindedObjects = objs.ToArray();
                    m_Object.ApplyModifiedProperties();
                    m_Object.Update();

                    UIExpansionManager.Instance.CurUIExpansionWrapper.Save();
                }
                else
                {
                    EditorUtility.DisplayDialog("命名冲突!", ErrorInfo.ToString(), "绑定失败");
                }

                //GenExportComps();
            }

            if (!HasOpenCloseAni())
            {
                if (GUILayout.Button("添加默认窗口打开关闭动画"))
                {
                    RefreshOpenCloseAnimation(UIExpansionEditorConfigSettingsProvider.GetConfig());
                }
            }
            else
            {
                if (GUILayout.Button("删除默认窗口打开关闭动画"))
                {
                    RemoveOpenCloseAnimation();
                }
            }

            

            var p = m_Object.FindProperty("_bindedObjects");
            if (p != null)
            {
                
                for (int i = 0; i < p.arraySize; i++)
                {

                    var element = p.GetArrayElementAtIndex(i);
                    Object o = element.objectReferenceValue;

                    if (o is null)
                    {
                        Debug.Log("存在不存在的绑定组件，请重新刷新控件列表");
                    }
                    else
                    {
                        string toggleName;
                        
                        if (o is GameObject)
                        {
                            toggleName = o.name;
                            
                        }
                        else
                        {
                            var type = o.GetType();
                            var go = (o as Component).gameObject;
                            toggleName = $"{go.name}.{type.Name}";
                           
                        }
                        
                        // if (!m_showExportComps.ContainsKey(toggleName))
                        // {
                        //     continue;
                        // }

                        //EditorGUI.indentLevel = o is GameObject ? 0 : 2;

                        EditorGUILayout.BeginHorizontal();
                        //_ui.BindObjectToggleGroup[toggleName] = EditorGUILayout.Toggle("",_ui.BindObjectToggleGroup[toggleName],GUILayout.Width(20));
                        EditorGUILayout.PropertyField(element, new GUIContent(toggleName));
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }

            #endregion
        }

        private Dictionary<System.Type, bool> exportAbleList;

        private void RefreshExportAbleList()
        {
            exportAbleList = new Dictionary<System.Type, bool>();

            var canIgnoreList = LuaExporter.GetCanExportTypes();
            for (int i = 0; i < canIgnoreList.Count; i++)
            {
                var type = canIgnoreList[i];
                if (!exportAbleList.ContainsKey(type))
                    exportAbleList.Add(type, true);
                else
                    exportAbleList[type] = true;
            }

            var forceIgnoreList = LuaExporter.GetForceIgnoreTypes();

            forceIgnoreList.Add(typeof(RectTransform));
            forceIgnoreList.Add(typeof(Transform));

            for (int i = 0; i < forceIgnoreList.Count; i++)
            {
                var type = forceIgnoreList[i];

                if (!exportAbleList.ContainsKey(type))
                    exportAbleList.Add(type, false);
                else
                    exportAbleList[type] = false;
            }
        }
        
        private void ColllectTransformComponents(GameObject gameObject, List<UnityEngine.Object> objs,
            Dictionary<string, int> goNames, bool isFirst = false)
        {
            
            //开放导出对象名称前缀配置
            string goPrefix = UIExpansionEditorConfigSettingsProvider.GetConfig().goPrefix;
            if (gameObject.name.StartsWith(goPrefix))
            {
                objs.Add(gameObject);
                objs.Add(gameObject.transform);

                if (!goNames.ContainsKey(gameObject.name))
                    goNames.Add(gameObject.name, 0);
                goNames[gameObject.name] = goNames[gameObject.name] + 1;
                var components = gameObject.GetComponents<UnityEngine.Component>();
                for (int i = 0; i < components.Length; i++)
                {
                    Component comp = components[i];
                    var type = comp.GetType();
                    if (exportAbleList.ContainsKey(type) && exportAbleList[type])
                    {
                        objs.Add(comp);
                    }
                }
            }


            if (isFirst || gameObject.GetComponent<UIExpansion>() == null)
            {
                var childCount = gameObject.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    var t = gameObject.transform.GetChild(i);
                    ColllectTransformComponents(t.gameObject, objs, goNames);
                }
            }
        }

        private void OnLuaPathSelected(object folderPath)
        {
            var fullFilePath = EditorUtility.OpenFilePanel($"请选择Lua Module文件", (string) folderPath, "lua");

            if (string.IsNullOrEmpty(fullFilePath)) return;

            var relativePath = FolderBrowserHelper.GetRelativePath(fullFilePath, (string) folderPath);

            var luaPath = relativePath.Replace("/", ".");
            luaPath = luaPath.Replace("\\", ".");
            if (luaPath.EndsWith(".lua"))
            {
                _ui.LuaBindPath = luaPath.Substring(0, luaPath.Length - 4);
                EditorUtility.SetDirty(_ui);
            }
        }
        
        private void OnLuaExportPathSelected(object folderPath)
        {
            var fullFilePath =
                EditorUtility.SaveFilePanel($"请选择导出的文件夹", (string) folderPath, _ui.gameObject.name, "lua");


            if (string.IsNullOrEmpty(fullFilePath)) return;


            var relativePath = FolderBrowserHelper.GetRelativePath(fullFilePath, (string) folderPath);


            if (relativePath.StartsWith(".."))
            {
                EditorUtility.DisplayDialog("错误", "当前指定路径并不在任何一个lua根目录下!!!", "知道了");

                return;
            }

            if (!LuaExporter.GenerateFile(_ui, fullFilePath))
            {
                EditorUtility.DisplayDialog("错误", "导出失败!!!", "知道了");

                return;
            }

            var luaPath = relativePath.Replace("/", ".");
            luaPath = luaPath.Replace("\\", ".");
            if (luaPath.EndsWith(".lua"))
            {
                _ui.LuaBindPath = luaPath.Substring(0, luaPath.Length - 4);
                EditorUtility.SetDirty(_ui);
            }
        }

        public bool RefreshLuaExporter(UIExpansionEditorConfig editorConfig)
        {
            bool retVal = false;
            var m_LuaExporterTypeNames = new List<string>();
            m_LuaExporterTypeNames.AddRange(TypeUtils.GetEditorTypeNames(typeof(ILuaExporter)));

            if (editorConfig != null && !string.IsNullOrEmpty(editorConfig.luaExporterClass) &&
                m_LuaExporterTypeNames.Contains(editorConfig.luaExporterClass))
            {
                System.Type luaExporterType = TypeAssembly.GetType(editorConfig.luaExporterClass);
                if (luaExporterType != null)
                {
                    ILuaExporter luaExporter = (ILuaExporter) Activator.CreateInstance(luaExporterType);
                    if (luaExporter != null)
                    {
                        _luaExporter = luaExporter;

                        return true;
                    }
                }
            }
            else
            {
                Debug.LogError(
                    "[请先生成UIExpansion Editor Config配置] ProjectSetting -> NFU -> UI -> UI Expansion Setting将自动默认配置");
                retVal = true;
            }

            return retVal;
        }

        private bool HasOpenCloseAni()
        {
            if (_ui == null)
                return false;
            var go = _ui.gameObject;
            var animation = go.GetComponent<Animation>();
            if (animation == null)
                return false;
            if (animation.GetClip("close") == null && animation.GetClip("open") == null)
                return false;
            return true;
        }

        private void RemoveOpenCloseAnimation()
        {
            if (_ui == null)
                return;
            GameObject targetGo = _ui.gameObject;
            if (targetGo != null)
            {
                Animation animation = targetGo.GetComponent<Animation>();
                if(animation.GetClip("close") != null)
                    animation.RemoveClip("close");
                if(animation.GetClip("open") != null)
                    animation.RemoveClip("open");
                if (animation.GetClipCount() <= 0)
                {
                    UnityEngine.Object.DestroyImmediate(animation, true);
                }
            }
        }
        private void RefreshOpenCloseAnimation(UIExpansionEditorConfig editorConfig)
        {
            if (editorConfig.defaultCloseAni == null && editorConfig.defaultOpenAni == null)
            {
                Debug.LogWarning("添加失败，未配置动画文件，请检查 " + UIExpansionEditorConfigSettingsProvider.GetConfigPath());
                return;
            }
            if (_ui == null)
                return;
            GameObject targetGo = _ui.gameObject;
            if (targetGo != null)
            {
                Animation animation = targetGo.GetComponent<Animation>();
                if (animation == null)
                {
                    animation = targetGo.AddComponent<Animation>();
                    if(editorConfig.defaultCloseAni != null)
                        animation.AddClip(editorConfig.defaultCloseAni, "close");
                    if(editorConfig.defaultOpenAni != null)
                        animation.AddClip(editorConfig.defaultOpenAni, "open");
                }
                else
                {
                    if (editorConfig.defaultCloseAni != null)
                    {
                        if (animation.GetClip("close") != null)
                        {
                            animation.RemoveClip("close");
                        }
                        animation.AddClip(editorConfig.defaultCloseAni, "close");
                    }

                    if (editorConfig.defaultOpenAni != null)
                    {
                        if (animation.GetClip("open"))
                        {
                            animation.RemoveClip("open");
                        }
                        animation.AddClip(editorConfig.defaultOpenAni, "open");
                    }

                }
            }
        }

        private void OnPlayingModeGUI()
        {
            if (_ui.ControllerConfigs != null && _ui.ControllerConfigs.Length > 0)
            {
                EditorGUILayout.LabelField("Controllers:", EditorStyles.boldLabel);
                for (int i = 0; i < _ui.ControllerConfigs.Length; i++)
                {
                    ControllerConfig config = _ui.ControllerConfigs[i];
                    IController controller = _ui.GetController(i);
                    if (controller == null)
                    {
                        continue;
                    }

                    int[] optionValues = new int[config.pageNames.Length];
                    for (int j = 0; j < config.pageNames.Length; j++)
                    {
                        optionValues[j] = j;
                    }

                    string[] controllerOptionStrs = new string[config.pageNames.Length];
                    for (int j = 0; j < config.pageNames.Length; j++)
                    {
                        controllerOptionStrs[j] = string.Format("[{0}] {1}", j, config.pageNames[j]);
                    }

                    int tempIndex = EditorGUILayout.IntPopup(config.name, controller.SelectedIndex,
                        controllerOptionStrs, optionValues);
                    controller.SelectedIndex = tempIndex;
                }
#if UNITY_2018
                for (int i = 0; i < 8; i++)
                {
                    EditorGUILayout.Space();
                }
#else
                EditorGUILayout.Space(8f);
#endif
            }
        }

        private bool IsRightLuaPath()
        {
            var editorConfig = UIExpansionEditorConfigSettingsProvider.GetConfig();
            var path = _ui.LuaBindPath;
            foreach (var t in editorConfig.luaFolderPathArray)
            {
                string tmp = Path.Combine(Application.dataPath, t, path.Replace(".", "/") + ".lua");
                if (File.Exists(tmp))
                {
                    return true;
                }
            }

            return false;
        }

    }
}