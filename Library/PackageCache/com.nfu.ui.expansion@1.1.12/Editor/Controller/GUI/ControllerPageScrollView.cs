using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ND.UI
{
    public class ControllerPageScrollView : EditorScrollViewBase
    {
        private string _tempPageName;
        private GUIContent content = new GUIContent();
        public override void OnGUI(Rect viewArea)
        {
            base.OnGUI(viewArea);
            GUI.Box(_viewArea, string.Empty, EditorStyles.toolbar);
            GUI.BeginGroup(_viewArea, EditorStyles.toolbar);
            DrawPageNames();
            GUI.EndGroup();
        }

        private void DrawPageNames()
        {
            for (int i = 0; i < UIExpansionManager.Instance.CurControllerWrapper.PageNameList.Count; i++)
            {
                Color tempColor = GUI.color;
                if (i == UIExpansionManager.Instance.CurControllerWrapper.SelectedIndex)
                {
                    GUI.color = new Color(0.5f, 1, 1, 1f);
                }

                Rect backgroundArea = new Rect(
                    i*UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH + UIExpansionManager.Instance.ControllerSettings.PanelOffsetX,
                    0,
                    UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH,
                    EditorGUIUtility.singleLineHeight);
                /*
                if (i == UIExpansionManager.Instance.ControllerSettings.CurChangeNamePageIndex)
                {
                    Rect textArea = new Rect(backgroundArea);
                    textArea.width -= 40;
                    _tempPageName = GUI.TextField(textArea, _tempPageName);

                    Rect confirmBtnArea = new Rect(
                        (i + 1) * UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH + UIExpansionManager.Instance.ControllerSettings.PanelOffsetX - 40,
                        0,
                        20,
                        EditorGUIUtility.singleLineHeight);
                    if (GUI.Button(confirmBtnArea, "+", EditorStyles.toolbarButton))
                    {
                        if (!string.IsNullOrEmpty(_tempPageName))
                        {
                            UIExpansionManager.Instance.CurControllerWrapper.ChangePageName(i, _tempPageName);
                            UIExpansionManager.Instance.ControllerSettings.CurChangeNamePageIndex = -1;
                        }
                    }

                    Rect cancelBtnArea = new Rect(
                        (i + 1) * UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH + UIExpansionManager.Instance.ControllerSettings.PanelOffsetX - 20,
                        0,
                        20,
                        EditorGUIUtility.singleLineHeight);
                    if (GUI.Button(cancelBtnArea, "-", EditorStyles.toolbarButton))
                    {
                        UIExpansionManager.Instance.ControllerSettings.CurChangeNamePageIndex = -1;
                    }
                }
                else
                {
                    if (GUI.Button(backgroundArea, UIExpansionManager.Instance.CurControllerWrapper.PageNameList[i], EditorStyles.toolbarButton))
                    {
                        if (Event.current.button == 0)
                        {
                            Debug.Log("Left");
                            OnBtnLeftClick(i);

                        }
                        else if (Event.current.button == 1)
                        {
                            Debug.Log("Right");
                            OnBtnRightClick(i);
                        }
                    }
                }
                */

                //if (GUI.Button(backgroundArea, string.Format("[{0}]{1}({2})  ",i,UIExpansionManager.Instance.CurControllerWrapper.PageNameList[i],UIExpansionManager.Instance.CurControllerWrapper.PageTipsList[i]), EditorStyles.toolbarButton))
                
                content.text = string.Format("[{0}]{1}", i, UIExpansionManager.Instance.CurControllerWrapper.PageNameList[i]);
                content.tooltip = UIExpansionManager.Instance.CurControllerWrapper.PageTipsList[i];

                if (GUI.Button(backgroundArea, content, EditorStyles.toolbarButton))
                {
                    if (Event.current.button == 0)
                    {
                        // Debug.Log("Left");
                        OnBtnLeftClick(i);

                    }
                    else if (Event.current.button == 1)
                    {
                        // Debug.Log("Right");
                        // OnBtnRightClick(i);
                    }
                }
                if (i == UIExpansionManager.Instance.CurControllerWrapper.SelectedIndex)
                {
                    GUI.color = tempColor;
                }
            }
        }

        private void OnBtnLeftClick(int index)
        {
            UIExpansionManager.Instance.CurControllerWrapper.ChangeSelectedIndex(index);
        }

        private void OnBtnRightClick(int index)
        {
            return;
            // 封闭
            GenericMenu controllerListMenu = new GenericMenu();
            controllerListMenu.AddItem(new GUIContent("Change Name"), false, OnChangeNameBtnSelected, index);
            controllerListMenu.AddItem(new GUIContent("Delete This"), false, OnDeleteThisBtnSelected, index);
            controllerListMenu.ShowAsContext();
        }

        private void OnChangeNameBtnSelected(object indexObj)
        {
            int index = (int)indexObj;
            _tempPageName = UIExpansionManager.Instance.CurControllerWrapper.PageNameList[index];
            UIExpansionManager.Instance.ControllerSettings.CurChangeNamePageIndex = index;
        }

        private void OnDeleteThisBtnSelected(object indexObj)
        {
            int index = (int)indexObj;
            UIExpansionManager.Instance.DeleteControllerPage(index);
        }
    }
}