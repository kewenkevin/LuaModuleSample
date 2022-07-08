using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace NFU.Tools.LuaCmd
{
    public class LuaCmdWindow:EditorWindow
    {
        private bool m_inited;
        private IEditorInternal m_internal;
        private VisualElement m_root;
        private ListView m_historyListView;

        private List<string> m_historyTitles=new List<String>();
        private List<string> m_historyContents=new List<String>();

        private TextField m_contentField;
        private TextField m_titleField;
        private int m_selectedIndex;
        private bool m_firstInit;

        [MenuItem("Tools/LuaTools/LuaCmdWindow")]
        static void Init()
        {
            var window = EditorWindow.GetWindow<LuaCmdWindow>("LuaCmdWindow");
            window.minSize=new Vector2(800,600);
            window.Refresh();
            window.Show();
        }


        void OnGUI()
        {
            if (!EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                EditorGUILayout.LabelField("Must playing Editor,please enter play mode....");
            }
            else
            {
                if (!m_inited)
                {
                    Refresh();
                }
            }
        }
        void Refresh()
        {
            if (!EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                return;
            }
            if (!m_inited)
            {
                var assemblys = AppDomain.CurrentDomain.GetAssemblies();
                for (int i = 0; i < assemblys.Length; i++)
                {
                    var assembly = assemblys[i];
                    if (assembly.GetName().Name == "Assembly-CSharp-Editor")
                    {
                        var types = assembly.GetTypes();
                        for (int j = 0; j < types.Length; j++)
                        {
                            var type = types[j];
                            var s = type.GetInterface("NFU.Tools.LuaCmd.IEditorInternal");
                            if (s != null)
                            {
                                m_internal = (IEditorInternal)Activator.CreateInstance(type);
                            }
                        }
                    }
                }
                if (m_internal == null)
                {
                    EditorUtility.DisplayDialog("Error", "Must have a class to inherit the IEditorInternal interface", "Ok");
                    UnityEngine.Debug.LogError("Must have a class to inherit the IEditorInternal interface");
                    m_inited = false;
                    Close();
                    return;
                }
                
                m_root = rootVisualElement;
                //2.读取UXML
                var visualTree =
                    AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.nfu.tools.luatools/luaCmd/Editor/LuaCmd.uxml");
                //3.读取USS
                var styleSheet =
                    AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.nfu.tools.luatools/luaCmd/Editor/LuaCmd.uss");
                //4.创建新的子节点
                VisualElement labelFromUXML = visualTree.CloneTree();
                //5.为UXML添加USS样式
                labelFromUXML.styleSheets.Add(styleSheet);
                m_root.Add(labelFromUXML);
                    
                var container=m_root.Q<VisualElement>("container");
                var contentContainer = container.Q<VisualElement>("contentContainer");
                m_contentField = contentContainer.Q<TextField>("cmdContent");
                m_contentField.RegisterValueChangedCallback(e =>
                {
                    var newStr = e.newValue;
                    if (newStr == m_historyContents[m_selectedIndex])
                    {
                        return;
                    }
                    m_historyContents[m_selectedIndex] = newStr;
                });
                m_titleField = contentContainer.Q<TextField>("title");
                m_titleField.RegisterValueChangedCallback((e) =>
                {
                    var newStr = e.newValue;
                    if (newStr == m_historyTitles[m_selectedIndex])
                    {
                        return;
                    }
                    if (m_historyTitles.Contains(newStr) || string.IsNullOrEmpty(newStr))
                    {
                        newStr += " " + UnityEngine.Random.Range(1, 10000);
                    }
                    m_historyTitles[m_selectedIndex] = newStr;
                    m_titleField.value = newStr;
                    m_historyListView.Refresh();
                });
                m_historyListView = container.Q<ListView>("history");
                m_historyListView.makeItem = () => new Label();
                m_historyListView.bindItem =  (e, i) => { (e as Label).text = m_historyTitles[i]; };;
                m_historyListView.itemsSource = m_historyTitles;
                m_historyListView.selectionType = SelectionType.Single;
                m_historyListView.onSelectionChanged += objects =>
                {
                    var selected = objects[0] as string;
                    var index=m_historyTitles.IndexOf(selected);
                    m_contentField.value = m_historyContents[index];
                    m_selectedIndex = index;
                    m_titleField.value = selected;
                };
                var toolbar = m_root.Q<VisualElement>("toolbar");
                toolbar.Q<Button>("btn-add").clicked += OnAdd;
                toolbar.Q<Button>("btn-execute").clicked += OnExecute;

                if (m_historyTitles.Count>0)
                {
                    m_historyListView.selectedIndex = 0;
                }
                m_inited = true;
            }

            if (m_internal!=null)
            {
                if (m_historyTitles.Count<=0)
                {
                    OnAdd();
                }
            }
        }

        void OnAdd()
        {
            m_historyTitles.Add("New Cmd "+m_historyTitles.Count);
            string contents = "";
            for (int i = 0; i < 30; i++)
            {
                contents += "\n";
            }
            m_historyContents.Add(contents);
            m_historyListView.Refresh();
            if (m_historyTitles.Count>0)
            {
                m_historyListView.selectedIndex = 0;
            }
        }

        void OnExecute()
        {
            if (m_internal!=null)
            {
                var content = m_historyContents[m_selectedIndex];
                string str ="local status,err=pcall(function()\n" +
                            content+"\n"+
                            "end\n"+
                            ")\n"+
                            "if not status then\n"+
                            "print(err)\n"+
                            "else\n"+
                            "print(\"execute success\")\n"+
                            "end\n"
                    ;
                m_internal.DoString(str);
            }
        }

        // void SaveConfig()
        // {
        //     var files = Directory.GetFiles(cacheFolderPath,"*.txt",SearchOption.AllDirectories);
        //     for (int i = 0; i < files.Length; i++)
        //     {
        //         
        //     }
        // }
        //
        // void ReadConfig()
        // {
        //     var files = Directory.GetFiles(cacheFolderPath,"*.txt",SearchOption.AllDirectories);
        //     for (int i = 0; i < files.Length; i++)
        //     {
        //         
        //     }
        // }
    }
}