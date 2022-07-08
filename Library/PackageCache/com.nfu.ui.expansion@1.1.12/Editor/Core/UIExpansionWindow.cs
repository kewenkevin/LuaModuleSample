using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;


namespace ND.UI
{
    public class UIExpansionWindow : EditorWindow
    {
        private DateTime _timeStamp;
        private int _refreshRate;
        private void Awake()
        {
            // Debug.Log("On Window Awake");
            base.titleContent = new GUIContent("UIExpansion Editor");
            this.minSize = new Vector2(UIExpansionEditorUtility.WINDOW_MIN_WIDTH, UIExpansionEditorUtility.WINDOW_MIN_HEIGHT);
            _timeStamp = DateTime.Now;
            _refreshRate = 15;
            
        }

        private void OnEnable()
        {
            // Debug.Log("On Window Enable");
            UIExpansionEditorUtility.Init();
            UIExpansionManager.Instance.IsWindowOpen = true;
            UIExpansionManager.Instance.LoadLastUIExpansion();
            UIExpansionManager.Instance.NeedReInit = true;
            UIExpansionManager.Instance.NeedRepaint = true;
            // RefreshDisplayAndWrapperData();
            //UIExpansionManager.Instance.CurBindingWrapper.CheckAllLabel();
            EditorApplication.update += (EditorApplication.CallbackFunction)System.Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(UIExpansionManager.Instance.OnWindowUpdate));
            // EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.hierarchyWindowItemOnGUI += OnSelectObjectInHierarchy;
        }

        private void OnGUI()
        {
            // 按一定频率强制刷新编辑器界面显示，用来解决因Editor模式下界面刷新较慢而引起的显示异常bug
            double delta = (DateTime.Now - _timeStamp).TotalMilliseconds;
            _timeStamp = DateTime.Now;
            if (delta >= 1f / _refreshRate)
            {
                // Debug.Log(((GameObject)EditorApplication.hierarchyWindowItemOnGUI.Target).name); 
                
                Repaint();
            }
            
            Rect toolbarArea = new Rect(
                0,
                0,
                base.position.width,
                UIExpansionEditorUtility.SINGLELINE_HEIGHT);
            DrawToolBar(toolbarArea);

            Rect panelArea = new Rect(
                0,
                UIExpansionEditorUtility.SINGLELINE_SPACING,
                base.position.width,
                base.position.height - UIExpansionEditorUtility.SINGLELINE_SPACING);
            DrawPanel(panelArea);
            if (UIExpansionManager.Instance.NeedReInit)
            {
                UIExpansionManager.Instance.NeedReInit = false;
                UIExpansionManager.Instance.Init();
            }
            if (UIExpansionManager.Instance.NeedRepaint)
            {
                UIExpansionManager.Instance.NeedRepaint = false;
                base.Repaint();
            }
        }

        private void OnSelectObjectInHierarchy(int instanceId, Rect rect)
        {
            GameObject selectedGameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            if (Event.current != null && rect.Contains(Event.current.mousePosition)
                                      && Event.current.button == 0 && Event.current.type < EventType.MouseUp)
            {
                if (selectedGameObject)
                {
                    
                    Debug.Log(selectedGameObject.name);
                    UIExpansion uiExpansion;
#if(UNITY_2019_1_OR_NEWER)
                    if (selectedGameObject.TryGetComponent<UIExpansion>(out uiExpansion))
#else
                    uiExpansion = selectedGameObject.GetComponent<UIExpansion>();
                    if(uiExpansion!=null)
#endif
                    {
                        // Debug.Log("uiExpansion"+uiExpansion);
                        ChangeTarget(uiExpansion);
                        UIExpansionManager.Instance.WindowSettings.BindingSettings.WorkType = BindingPanelWorkTypeState.Display;
                        Repaint();
                    }
                    else
                    {
                        if (selectedGameObject.transform.parent !=null &&TryGetUIExpansionFromParent(selectedGameObject.transform.parent.gameObject,out uiExpansion))
                        {
                            // Debug.Log("uiExpansion"+uiExpansion);
                            ChangeTarget(uiExpansion);
                            UIExpansionManager.Instance.WindowSettings.BindingSettings.WorkType = BindingPanelWorkTypeState.Display;
                            Repaint();
                        }
                        else
                        {
                            // Debug.Log("No find any UIExpansion in go's relation chain");
                        }
                    }
                }
            }
        }

        private bool TryGetUIExpansionFromParent(GameObject gameObjectParent, out UIExpansion uiExpansion)
        {
            if (gameObjectParent == null)
            {
                uiExpansion = null;
                return false;
            }
#if(UNITY_2019_1_OR_NEWER)
            if (gameObjectParent.TryGetComponent<UIExpansion>(out uiExpansion))
#else
            uiExpansion = gameObjectParent.GetComponent<UIExpansion>();
            if(uiExpansion!=null)
#endif
            {
                return true;
            }
            else if (gameObjectParent.transform.parent == null)
            {
                uiExpansion = null;
                return false;
            }
            {
                return TryGetUIExpansionFromParent(gameObjectParent.transform.parent.gameObject, out uiExpansion);
            }
        }

        private void DrawToolBar(Rect toolbarArea)
        {
            float leftX = toolbarArea.x;
            float rightX = toolbarArea.xMax;
            float itemWidth = 0;
            GUI.Box(toolbarArea, string.Empty, EditorStyles.toolbar);

            itemWidth = 200;
            UIExpansionManager.Instance.CurUIExpansion = EditorGUI.ObjectField(new Rect(leftX, toolbarArea.y, itemWidth, UIExpansionEditorUtility.SINGLELINE_HEIGHT), UIExpansionManager.Instance.CurUIExpansion, typeof(UIExpansion), true) as UIExpansion;
            leftX += itemWidth;
            if (UIExpansionManager.Instance.CurUIExpansion == null)
            {
                return;
            }
            itemWidth = 120;
            if (UIExpansionManager.Instance.CurUIExpansion.IsPureMode || UIExpansionEditorConfigSettingsProvider.IsPureMode())
            {
                GUI.Label(new Rect(leftX, toolbarArea.y, itemWidth, UIExpansionEditorUtility.SINGLELINE_HEIGHT), "Controller", EditorStyles.toolbarButton);
            }
            else
            {
                if (GUI.Button(new Rect(leftX, toolbarArea.y, itemWidth, UIExpansionEditorUtility.SINGLELINE_HEIGHT), UIExpansionManager.Instance.WindowSettings.Panels[UIExpansionManager.Instance.WindowSettings.CurPanelIndex].PanelName, EditorStyles.toolbarDropDown))
                {
                    GenericMenu genericMenu = new GenericMenu();
                    for (int i = 0; i < UIExpansionManager.Instance.WindowSettings.Panels.Length; i++)
                    {
                        genericMenu.AddItem(new GUIContent(UIExpansionManager.Instance.WindowSettings.Panels[i].PanelName), UIExpansionManager.Instance.WindowSettings.CurPanelIndex == i, OnToolbarGenericMenuSelect, i);
                    }
                    genericMenu.ShowAsContext();
                }
            }
            leftX += itemWidth;

            itemWidth = 80;
            
            //检测是否是runtime，禁止runtime时进行保存操作
            EditorGUI.BeginDisabledGroup(false);
            if (Application.isPlaying)
            {
                UIExpansionManager.Instance.WindowSettings.AutoSave = false;
                EditorGUI.BeginDisabledGroup(true);
            }
            UIExpansionManager.Instance.WindowSettings.AutoSave = GUI.Toggle(new Rect(rightX - itemWidth, toolbarArea.y, itemWidth, UIExpansionEditorUtility.SINGLELINE_HEIGHT), UIExpansionManager.Instance.WindowSettings.AutoSave, "Auto Save");
            EditorGUI.EndDisabledGroup();
            
            rightX -= itemWidth+10;
            if (!UIExpansionManager.Instance.AutoSave)
            {
                itemWidth = 60;
                if (GUI.Button(new Rect(rightX - itemWidth, toolbarArea.y, itemWidth, UIExpansionEditorUtility.SINGLELINE_HEIGHT), "Save", EditorStyles.toolbarButton))
                {
                    //检测是否是runtime，禁止runtime时进行保存操作
                    if(Application.isPlaying)
                    {
                        EditorUtility.DisplayDialog("警告", "Runtime模式不允许保存，请退出Runtime后再保存", "确认");
                        return;
                    }
                    string failedStr = null;
                    if (UIExpansionManager.Instance.CurUIExpansionWrapper.CheckCanSave(true,out failedStr))
                    {
                        List<string> missLabelList = null;
                        if (UIExpansionManager.Instance.CurBindingWrapper.CheckAllOldLabelExist(out missLabelList))
                        {
                            UIExpansionManager.Instance.CurUIExpansionWrapper.Save();
                        }
                        else
                        {
                            Debug.Log(string.Format("<color=#ff0000>{0}</color>", "[UIExpansionWindow的检测]"));
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("双向绑定界面有部分Label被删除：");
                            for (int i = 0; i < missLabelList.Count; i++)
                            {
                                sb.Append(missLabelList[i]);
                                sb.Append("  ");
                            }
                            sb.AppendLine();
                            sb.Append("是否继续保存？");
                            if (EditorUtility.DisplayDialog("Label丢失确认", sb.ToString(), "保存", "取消"))
                            {
                                UIExpansionManager.Instance.CurUIExpansionWrapper.Save();
                            }
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("保存失败", failedStr, "确认");
                    }
                }
                rightX -= itemWidth;
            }

            itemWidth = 80;
            if (GUI.Button(new Rect(rightX - itemWidth, toolbarArea.y, itemWidth, UIExpansionEditorUtility.SINGLELINE_HEIGHT), "Refresh", EditorStyles.toolbarButton))
            {
                //RefreshDisplayAndWrapperData();
                UIExpansionManager.Instance.NeedReInit = true;
                UIExpansionManager.Instance.NeedRepaint = true;
            }
            rightX -= itemWidth;
        }

        private void DrawPanel(Rect panelArea)
        {
            if (UIExpansionManager.Instance.CurUIExpansion == null)
            {
                return;
            }
            UIExpansionManager.Instance.WindowSettings.Panels[UIExpansionManager.Instance.WindowSettings.CurPanelIndex].OnGUI(panelArea);
        }

        private void OnToolbarGenericMenuSelect(object data)
        {
            int index = Mathf.Clamp((int)data, 0, UIExpansionManager.Instance.WindowSettings.Panels.Length - 1);
            if (index == UIExpansionManager.Instance.WindowSettings.CurPanelIndex)
            {
                return;
            }
            UIExpansionManager.Instance.WindowSettings.Panels[UIExpansionManager.Instance.WindowSettings.CurPanelIndex].OnExit();
            UIExpansionManager.Instance.WindowSettings.CurPanelIndex = index;
            UIExpansionManager.Instance.WindowSettings.Panels[UIExpansionManager.Instance.WindowSettings.CurPanelIndex].OnEnter();
        }

        private void RefreshDisplayAndWrapperData()
        {
            UIExpansionManager.Instance.WindowSettings.Refresh();
            UIExpansionManager.Instance.CurUIExpansionWrapper.Refresh();
        }

        public void ChangeTarget(UIExpansion tempUIExpansion)
        {
            UIExpansionManager.Instance.CurUIExpansion = tempUIExpansion;
            // UIExpansionManager.Instance.Init();
        }

        private void OnDisable()
        {
            // EditorApplication.hierarchyWindowItemOnGUI -= OnSelectObjectInHierarchy;
            // EditorApplication.update -= UIExpansionManager.Instance.OnWindowUpdate;
            // UIExpansionManager.Instance.OnDispose();
            UIExpansionManager.Instance.IsWindowOpen = false;
        }

        private void OnDestroy()
        {
            
            EditorApplication.hierarchyWindowItemOnGUI -= OnSelectObjectInHierarchy;
            EditorApplication.update -= UIExpansionManager.Instance.OnWindowUpdate;
            // UIExpansionManager.Instance.OnDispose();
            UIExpansionManager.Instance.IsWindowOpen = false;
        }

        [MenuItem("UICreator/打开UIExpansion编辑器",false,20001)]
        private static void ShowWindow()
        {
            EditorWindow.GetWindow<UIExpansionWindow>();
        }
    }
}