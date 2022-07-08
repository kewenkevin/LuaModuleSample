using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ND.UI
{
    [InitializeOnLoad]
    public class UIExpansionBindingInspectorHeaderGUI
    {
        private static UnityEditor.Editor m_editor = null;
        private static int m_currentSelect = 0;
        private static int m_previousGOInstanceId = 0;
        private static int m_previousHotControl = -1;

        private static bool needSave = false;
        private static bool dialogshowing = false;
        private static List<EditorTreeItemBase> m_previousBinderList;
        private static List<string> m_originBinderstring;

        private static bool isLegal = true;

        private enum UIEFROM
        {
            None,
            Self,
            Parent,
        }

        static UIExpansionBindingInspectorHeaderGUI()
        {
            ClearDisplay();
            UnityEditor.EditorApplication.update += ListenEditorWindowChange;
            UnityEditor.Editor.finishedDefaultHeaderGUI += DisplayUIExpansionBindingInspectorHeaderCallBack;
        }
        public static void ReDisplay()
        {
            UnityEditor.Editor.finishedDefaultHeaderGUI -= DisplayUIExpansionBindingInspectorHeaderCallBack;
            UnityEditor.Editor.finishedDefaultHeaderGUI += DisplayUIExpansionBindingInspectorHeaderCallBack;
        }

        public static void ClearDisplay()
        {
            UnityEditor.Editor.finishedDefaultHeaderGUI -= DisplayUIExpansionBindingInspectorHeaderCallBack;
            UnityEditor.EditorApplication.update -= ListenEditorWindowChange;
        }

        public static void ListenEditorWindowChange()
        {
            // Debug.Log("window: "+UnityEditor.EditorWindow.focusedWindow.titleContent.text);
            if (UnityEditor.EditorWindow.focusedWindow == null || UnityEditor.EditorWindow.focusedWindow.titleContent == null)
            {
                return;
            }
            if (UnityEditor.EditorWindow.focusedWindow.titleContent.text != "Inspector")
            {
                if (!CheckBindingInfo())
                {
                    ResetBackInfo();
                }
            }
        }

        static bool isMode = false;
        static bool needPaint = false;
        private static void DisplayUIExpansionBindingInspectorHeaderCallBack(UnityEditor.Editor editor)
        {
            m_editor = editor;
            // Debug.Log("window: "+UnityEditor.EditorWindow.focusedWindow.titleContent.text);
            // if (UnityEditor.EditorWindow.focusedWindow.titleContent.text != "Inspector")
            // {
            //     if (!CheckBindingInfo())
            //     {
            //         ResetBackInfo();
            //     }
            // }
            var selectedGameObject = editor.target as GameObject;
            if (selectedGameObject == null)
                return;

            if (m_previousGOInstanceId != selectedGameObject.GetInstanceID())
            {
                //CheckNeedSave();
                if (!CheckBindingInfo())
                {
                    ResetBackInfo();
                }
                UIExpansion uiExpansion;
                UIExpansion parentUIExpansion;

                switch (DoesItSelfOrParentHaveUIExpansion(selectedGameObject, out uiExpansion))
                {
                    case UIEFROM.Self:
                        parentUIExpansion = DoseItBelongToModel(selectedGameObject, uiExpansion);
                        isMode = parentUIExpansion != null;
                        if (!isMode)
                        {
                            UIExpansionManager.Instance.CurUIExpansion = uiExpansion;
                        }
                        else
                        {
                            UIExpansionManager.Instance.CurUIExpansion = parentUIExpansion;
                            if (UIExpansionManager.Instance.CurBindingWrapper.UIExpansionGoTreeItemDic.ContainsKey(
                                    selectedGameObject.GetInstanceID())
                                && PrefabUtility.GetPrefabAssetType(parentUIExpansion.gameObject) ==
                                PrefabAssetType.NotAPrefab
                                || UnityEditor.SceneManagement.EditorSceneManager.IsPreviewSceneObject(parentUIExpansion
                                    .gameObject))
                            {
                                isMode = false;
                            }
                            else if (PrefabUtility.GetPrefabAssetType(uiExpansion.gameObject) ==
                                     PrefabAssetType.NotAPrefab)
                            {
                                isMode = false;
                            }
                        }

                        needPaint = true;
                        break;
                    case UIEFROM.Parent:
                        parentUIExpansion = DoseItBelongToModel(selectedGameObject, uiExpansion);
                        isMode = parentUIExpansion != null;
                        if (!isMode)
                        {
                            if (UnityEditor.SceneManagement.EditorSceneManager.IsPreviewSceneObject(uiExpansion
                                .gameObject))
                            {
                                UIExpansionManager.Instance.CurUIExpansion = uiExpansion;
                            }
                            else
                            {
                                UIExpansionManager.Instance.CurUIExpansion =
                                    parentUIExpansion == null ? uiExpansion : parentUIExpansion;
                            }
                        }
                        else
                        {
                            UIExpansionManager.Instance.CurUIExpansion = uiExpansion;
                            if (UIExpansionManager.Instance.CurBindingWrapper.UIExpansionGoTreeItemDic.ContainsKey(
                                    selectedGameObject.gameObject.GetInstanceID())
                                && PrefabUtility.GetPrefabAssetType(uiExpansion.gameObject) ==
                                PrefabAssetType.NotAPrefab)
                            {
                                if (UnityEditor.SceneManagement.EditorSceneManager.IsPreviewSceneObject(uiExpansion
                                    .gameObject))
                                {
                                    isMode = false;
                                }
                            }
                            else if (PrefabUtility.GetPrefabAssetType(uiExpansion.gameObject) ==
                                     PrefabAssetType.NotAPrefab)
                            {
                                isMode = false;
                            }
                        }

                        needPaint = true;
                        break;
                    case UIEFROM.None:
                        needPaint = false;
                        break;
                }

                if (!needPaint || dialogshowing)
                {
                    return;
                }

                m_previousGOInstanceId = selectedGameObject.GetInstanceID();
            }
            
            //简洁模式
            if (UIExpansionManager.Instance.CurUIExpansion.IsPureMode || UIExpansionEditorConfigSettingsProvider.IsPureMode())
            {
                return;
            }

            if (UIExpansionManager.Instance.IsWindowOpen)
            {
                if (UIExpansionManager.Instance.WindowSettings.CurPanelIndex == 2)
                {
                    EditorGUILayout.HelpBox("和UIExpansionEditor编辑属性绑定冲突\n请先关闭UIExpansionEditor面板！", MessageType.Warning);
                    return;
                }
                else if (UIExpansionManager.Instance.IsWindowOpen && UIExpansionManager.Instance.WindowSettings.CurPanelIndex != 2)
                {
                    UIExpansionManager.Instance.BindingSettings.ColumnSelectedGOId = selectedGameObject.GetInstanceID();
                    PaintArea(isMode);
                    return;
                }
            }
            else
            {
                UIExpansionManager.Instance.BindingSettings.ColumnSelectedGOId = selectedGameObject.GetInstanceID();
                PaintArea(isMode);
                return;
            }
            
        }

        private static UIEFROM DoesItSelfOrParentHaveUIExpansion(GameObject selectedGameObject, out UIExpansion uiExpansion)
        {
#if(UNITY_2019_1_OR_NEWER)
            if (selectedGameObject.TryGetComponent<UIExpansion>(out uiExpansion))
#else
            uiExpansion = selectedGameObject.GetComponent<UIExpansion>();
            if(uiExpansion!=null)
#endif
            {
                return UIEFROM.Self;
            }
            else if (selectedGameObject.transform.parent !=null &&TryGetUIExpansionFromParent(selectedGameObject.transform.parent.gameObject,out uiExpansion))
            {
                return UIEFROM.Parent;
            }
            else
            {
                return UIEFROM.None;
            }
        }
        
        private static bool TryGetUIExpansionFromParent(GameObject gameObjectParent, out UIExpansion uiExpansion)
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

        private static UIExpansion DoseItBelongToModel(GameObject target, UIExpansion uiExpansion)
        {
            if (uiExpansion.transform.parent == null)
            {
                return null;
            }
            if (TryGetUIExpansionFromParent(uiExpansion.transform.parent.gameObject, out UIExpansion parentUIExpansion))
            {
                if (parentUIExpansion == uiExpansion)
                {
                    return null;
                }
                return parentUIExpansion;
            }
            return null;
        }

        private static void PaintArea(bool isMode, bool targetChanged = false)
        {
            UIExpansionManager.Instance.AutoSave = false;
            if (!UIExpansionManager.Instance.CurBindingWrapper.GoTreeItemDic.TryGetValue(UIExpansionManager.Instance.BindingSettings.ColumnSelectedGOId, out var targetGameObject))
            {
                UIExpansionManager.Instance.BindingSettings.ColumnSelectedGOId = 0;
                return;
            }
            List<BindingBinderTreeItemBase> binderList = targetGameObject.GetBinderList();
            if (binderList.Count == 0)
            {
                return;
            }

            if (m_currentSelect >= binderList.Count)
            {
                m_currentSelect = 0;
            }
            string[] binderNames = new string[binderList.Count];
            for (int i = 0; i < binderList.Count; i++)
            {
                binderNames[i] = binderList[i].BinderType.ToString();
            }
            EditorGUILayout.BeginVertical();
            int selectedIndex = GUILayout.Toolbar(m_currentSelect, binderNames);
            // if (selectedIndex != m_currentSelect)
            // {
            //     //CheckNeedSave();
            //     m_currentSelect = selectedIndex;
            //     if (!CheckBindingInfo())
            //     {
            //         ResetBackInfo();
            //     }
            //     else
            //     {
            //         return;
            //     }
            // }
            
            if (Application.isPlaying)
            {
                GUI.enabled = false;
            }
            else
            {
                GUI.enabled = true;
            }
            if(m_previousBinderList == null)
                RecordBindingInfo(binderList[m_currentSelect].ChildrenList);

            isLegal = true;
            for (int i = 0; i < binderList[m_currentSelect].ChildrenList.Count; i++)
            {
                BindingLinkerTreeItem linkerTreeItem = binderList[m_currentSelect].ChildrenList[i] as BindingLinkerTreeItem;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(linkerTreeItem.Tag);
                switch (linkerTreeItem.Tag)
                {
                    case "spriteName":
                    case "text":
                        if (isMode)
                        {
                            GUI.enabled = false;
                        }
                        if (GUILayout.Button("FastSet"))
                        {
                            linkerTreeItem.Label = linkerTreeItem.Target.name;
                            if (CheckLabelArea(linkerTreeItem))
                            {
                                if (m_previousHotControl != GUIUtility.hotControl)
                                {
                                    // Save();
                                    needSave = true;
                                    m_previousHotControl = GUIUtility.hotControl;
                                }
                            }
                        }
                        break;
                }

                if (isMode || Application.isPlaying)
                {
                    GUI.enabled = false;
                }
                var label = GUILayout.TextField(linkerTreeItem.Label);
                if (linkerTreeItem.Label != label)
                {
                    linkerTreeItem.Label = label;
                    needSave = true;
                }

                if (linkerTreeItem.Label != m_originBinderstring[i])
                {
                    Color tempColor = GUI.color;
                    GUI.color = new Color(0.4f, 0.9f, 0.5f, 1);
                    GUILayout.Label("*", EditorStyles.boldLabel);
                    GUI.color = tempColor;
                }
                
                GUI.enabled = true;
                if (CheckLabelArea(linkerTreeItem))
                {
                    if (m_previousHotControl != GUIUtility.hotControl)
                    {
                        // Save();
                        //needSave = true;
                        m_previousHotControl = GUIUtility.hotControl;
                    }
                }
                else
                {
                    isLegal = false;
                }
                m_previousHotControl = GUIUtility.hotControl;
                EditorGUILayout.EndHorizontal();
            }
            
            if (selectedIndex != m_currentSelect)
            {
                //CheckNeedSave();
                if (isLegal)
                {
                    m_currentSelect = selectedIndex;
                    if (!CheckBindingInfo())
                    {
                        ResetBackInfo();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("错误", "Label设置不正确，无法保存请修改", "确认");
                }
            }


            if (isLegal)
            {
                GUI.enabled = true;
            }
            else
            {
                GUI.enabled = false;
            }
            if (!Application.isPlaying && GUILayout.Button("Save"))
            {
                // Save();
                UIExpansionManager.Instance.CurUIExpansionWrapper.Save();
                m_editor.Repaint();
                needSave = false;
                dialogshowing = false;
                //重新记录label绑定信息
                RecordBindingInfo(binderList[m_currentSelect].ChildrenList);
            }
            EditorGUILayout.EndVertical();
            
        }

        private static bool CheckLabelArea(BindingLinkerTreeItem linkerTreeItem)
        {
            if (string.IsNullOrEmpty(linkerTreeItem.Label))
            {
                return true;
            }
            if (UIExpansionManager.Instance.CurBindingWrapper.IllegalLabelTreeItemList.Count > 0 && UIExpansionManager.Instance.CurBindingWrapper.IllegalLabelTreeItemList.Contains(linkerTreeItem))
            {
                Color tempColor = GUI.color;
                GUI.color = new Color(0.9f, 0.2f, 0.1f, 1);
                GUILayout.Label("含有非法字符", EditorStyles.boldLabel);
                GUI.color = tempColor;
                return false;
            }
            
            if (UIExpansionManager.Instance.CurBindingWrapper.IllegalMethodLabelTreeItemList.Count > 0 && UIExpansionManager.Instance.CurBindingWrapper.IllegalMethodLabelTreeItemList.Contains(linkerTreeItem))
            {
                Color tempColor = GUI.color;
                GUI.color = new Color(0.9f, 0.2f, 0.1f, 1);
                GUILayout.Label("不允许重复绑定同名方法", EditorStyles.boldLabel);
                GUI.color = tempColor;
                return false;
            }

            if (!UIExpansionManager.Instance.CurBindingWrapper.CheckRepeatLabelLegal(linkerTreeItem.Label))
            {
                Color tempColor = GUI.color;
                GUI.color = new Color(0.9f, 0.2f, 0.1f, 1);
                GUILayout.Label("重复的标签", EditorStyles.boldLabel);
                GUI.color = tempColor;
                return false;
            }

            return true;
        }
        
        private static bool CheckLabelAreaWithoutGUI(BindingLinkerTreeItem linkerTreeItem)
        {
            if (string.IsNullOrEmpty(linkerTreeItem.Label))
            {
                return true;
            }
            if (UIExpansionManager.Instance.CurBindingWrapper.IllegalLabelTreeItemList.Count > 0 && UIExpansionManager.Instance.CurBindingWrapper.IllegalLabelTreeItemList.Contains(linkerTreeItem))
            {
                return false;
            }
            
            if (UIExpansionManager.Instance.CurBindingWrapper.IllegalMethodLabelTreeItemList.Count > 0 && UIExpansionManager.Instance.CurBindingWrapper.IllegalMethodLabelTreeItemList.Contains(linkerTreeItem))
            {
                return false;
            }

            if (!UIExpansionManager.Instance.CurBindingWrapper.CheckRepeatLabelLegal(linkerTreeItem.Label))
            {
                return false;
            }

            return true;
        }

        private static void CheckNeedSave()
        {
            if (needSave && UIExpansionManager.Instance.CurUIExpansion != null)
            {
                // Debug.Log(string.Format("<color=#00ff00>{0}</color>", "[需要保存]"));
                Save();
            }

            needSave = false;
        }

        private static bool CheckBindingInfo()
        {
            if (needSave && UIExpansionManager.Instance.CurUIExpansion != null && !dialogshowing)
            {
                dialogshowing = true;
                BingingInfoChangedConfirm(() =>
                {
                    RestoreIllegalLabel();
                    UIExpansionManager.Instance.CurUIExpansionWrapper.Save();
                    m_editor.Repaint();
                    needSave = false;
                    dialogshowing = false;
                },
                () =>
                {
                    RestoreBindingInfo();
                    needSave = false;
                    dialogshowing = false;
                });
                
                return needSave;
            }
            return false;
        }

        private static void RestoreIllegalLabel()
        {
            int count = m_previousBinderList.Count;

            for (int i = 0; i < count; i++)
            {
                BindingLinkerTreeItem item = m_previousBinderList[i] as BindingLinkerTreeItem;
                if(item == null)
                    continue;
                if (!CheckLabelAreaWithoutGUI(item))
                {
                    item.Label = m_originBinderstring[i];
                }
            }
        }
        private static bool IsBindingInfoChanged()
        {
            int count = m_previousBinderList.Count;
            bool bChanged = false;
            for (int i = 0; i < count; i++)
            {
                BindingLinkerTreeItem item = m_previousBinderList[i] as BindingLinkerTreeItem;
                if(item == null)
                    break;
                string label = item.Label;
                if (!string.Equals(m_originBinderstring[i], label))
                {
                    bChanged = true;
                    break;
                }
            }

            return bChanged;
        }

        public static void RecordBindingInfo(List<EditorTreeItemBase> binderList)
        {
            m_previousBinderList = binderList;
            if(m_originBinderstring == null)
                m_originBinderstring = new List<string>();
            else
            {
                m_originBinderstring.Clear();
            }

            var count = m_previousBinderList.Count;
            for (var i = 0; i < count; i++)
            {
                BindingLinkerTreeItem item = m_previousBinderList[i] as BindingLinkerTreeItem;
                if(item == null)
                    break;
                m_originBinderstring.Add(item.Label);
            }
        }
        public static void RestoreBindingInfo()
        {
            int count = m_previousBinderList.Count;
            for (int i = 0; i < count; i++)
            {
                BindingLinkerTreeItem item = m_previousBinderList[i] as BindingLinkerTreeItem;
                if(item == null)
                    break;
                string label = item.Label;
                if (!string.Equals(m_originBinderstring[i], label))
                {
                    item.Label = m_originBinderstring[i];
                }
            }

            ResetBackInfo();
        }
        private static void ResetBackInfo()
        {
            m_previousBinderList = null;
            if (m_originBinderstring != null)
                m_originBinderstring.Clear();
        }
        private static void BingingInfoChangedConfirm(Action doOk, Action doCancel)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("界面绑定有部分Label被修改：");
            sb.AppendLine();
            if (isLegal)
            {
                sb.Append("是否继续保存？");
            }
            else
            {
                sb.Append("是否继续保存？注意: 不符合规范的标签将被自动还原");
            }
            ShowDialog("Label修改确认", sb.ToString(), "保存", "取消", doOk, doCancel);
        }

        private static void ShowDialog(string title,
            string message,
            string ok,
            string cancel,
            Action doOk = null, Action doCancel = null)
        {
            if (EditorUtility.DisplayDialog(title, message, ok, cancel))
            {
                doOk?.Invoke();
            }
            else
            {
                doCancel?.Invoke();
            }
        }

        private static void Save()
        {
            if (Application.isPlaying)
            {
                return;
            }
            
            string failedStr = "";
            if (!UIExpansionManager.Instance.CurUIExpansionWrapper.CheckCanSave(true, out failedStr))
            {
                return;
            }
            UIExpansionManager.Instance.CurUIExpansionWrapper.Save();
            // List<string> missLabelList = null;
            // UIExpansionManager.Instance.CurBindingWrapper.CheckAllLabel();
            // if (UIExpansionManager.Instance.CurBindingWrapper.CheckAllOldLabelExist(out missLabelList))
            // {
            //     UIExpansionManager.Instance.CurUIExpansionWrapper.Save();
            // }
            // else
            // {
            //     Debug.Log(string.Format("<color=#ff0000>{0}</color>", "[Inspector面板的检测]"));
            //     StringBuilder sb = new StringBuilder();
            //     sb.AppendLine("双向绑定界面有部分Label被删除：");
            //     for (int i = 0; i < missLabelList.Count; i++)
            //     {
            //         sb.Append(missLabelList[i]);
            //         sb.Append("  ");
            //     }
            //     sb.AppendLine();
            //     sb.Append("是否继续保存？");
            //     if (EditorUtility.DisplayDialog("Label丢失确认", sb.ToString(), "保存", "取消"))
            //     {
            //         UIExpansionManager.Instance.CurUIExpansionWrapper.Save();
            //         m_editor.Repaint();
            //         // UnityEditor.Editor.CreateEditor(Selection.objects[Selection.objects.Length-1]).Repaint(); 
            //         
            //     }
            //     else
            //     {
            //         // UnityEditor.Editor.CreateEditor(UnityEditor.EditorUtility.InstanceIDToObject(m_previousGOInstanceId)).Repaint(); 
            //         // Selection.activeObject = UnityEditor.EditorUtility.InstanceIDToObject(m_previousGOInstanceId);
            //         // m_editor.ReloadPreviewInstances();
            //     }
            // }
        }
    }
}

