using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ND.UI
{
    public class ControllerWrapperEditAction
    {
        public enum EditActionStateType
        {
            Remove,
            Add,
        }

        public ControllerWrapperEditAction(EditActionStateType state, int pageIndex)
        {
            m_editActionState = state;
            m_editPageIndex = pageIndex;
        }

        public EditActionStateType m_editActionState;

        public int m_editPageIndex;
    }

    public class ControllerWrapperDetailPopupWindow : EditorWindow
    {
        private string _wrapperName = "";

        private List<string> _pageNameList = new List<string>();
        private List<string> _pageTipsList = new List<string>();//标签备注

        private Vector2 _scrollViewVec2 = new Vector2();

        private List<ControllerWrapperEditAction> _editActionList;
        private GUIStyle _red = new GUIStyle(EditorStyles.textField);

        private static void ShowWindow()
        {
            EditorWindow.GetWindow<ControllerWrapperDetailPopupWindow>();
        }
        
        public Vector2 GetWindowSize()
        {
            return new Vector2(500, 190);
        }

        public void OnGUI()
        {
            bool bCanSave = true;
            _red.normal.textColor = Color.red;
            _red.onFocused.textColor = Color.red;
            _red.onHover.textColor = Color.red;
            _red.focused.textColor = Color.red;
            _red.onActive.textColor = Color.red;
            GUILayout.Label("名称：", EditorStyles.boldLabel);
            if (Utils.CheckLuaKeyWorlds(_wrapperName))
            {
                _wrapperName = GUILayout.TextField(_wrapperName);
            }
            else
            {
                _wrapperName = GUILayout.TextField(_wrapperName, _red);
                bCanSave = false;
            }
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("页面：", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.Label("备注：", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
           if( GUILayout.Button("+", GUILayout.Width(20)))
            {
                _pageNameList.Add("");
                //标签备注
                _pageTipsList.Add("");
                if (UIExpansionManager.Instance.ControllerSettings.InEditState)
                {
                    _editActionList.Add(new ControllerWrapperEditAction(ControllerWrapperEditAction.EditActionStateType.Add, 0));
                }
            }
            GUILayout.EndHorizontal();
            _scrollViewVec2 = GUILayout.BeginScrollView(_scrollViewVec2, GUILayout.Height(86));
            for (int i = 0; i < _pageNameList.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(i + ":", GUILayout.Width(20));
                if (Utils.CheckLuaKeyWorlds(_pageNameList[i]))
                {
                    _pageNameList[i] = EditorGUILayout.TextField(_pageNameList[i]);
                }
                else
                {
                    _pageNameList[i] = EditorGUILayout.TextField(_pageNameList[i], _red);
                    bCanSave = false;
                }
                //标签备注
                _pageTipsList[i] = EditorGUILayout.TextField(_pageTipsList[i]);
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    _pageNameList.RemoveAt(i);
                    _pageTipsList.RemoveAt(i);
                    if (UIExpansionManager.Instance.ControllerSettings.InEditState)
                    {
                        _editActionList.Add(new ControllerWrapperEditAction(ControllerWrapperEditAction.EditActionStateType.Remove, i));
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (bCanSave)
            {
                if (GUILayout.Button("保存", GUILayout.Width(60)))
                {
                    OnSaveBtnClick();
                }
            }
            else
            {
                GUILayout.Label("有非法字符不能保存", _red);
            }
            GUILayout.EndHorizontal();
        }

        private void OnSaveBtnClick()
        {
            if (UIExpansionManager.Instance.ControllerSettings.InCreateNewWrapperState)
            {
                if (string.IsNullOrEmpty(_wrapperName))
                {
                    EditorUtility.DisplayDialog("保存失败", "控制器名称不能为空。", "确认");
                    return;
                }
                if (!IsNumAndEnCh(_wrapperName))
                {
                    EditorUtility.DisplayDialog("保存失败", "控制器名称只能包含字母与数字。", "确认");
                    return;
                }
                foreach (UIExpansionControllerWrapper controller in UIExpansionManager.Instance.CurUIExpansionWrapper.ControllerWrapperList)
                {
                    if (controller.Name == _wrapperName)
                    {
                        EditorUtility.DisplayDialog("保存失败", "已存在相同名称的控制器。", "确认");
                        return;
                    }
                }
                if (_pageNameList.Count < 2)
                {
                    EditorUtility.DisplayDialog("保存失败", "控制器页面数量必须大于2。", "确认");
                    return;
                }
                Debug.Log(_pageTipsList.Count);
                UIExpansionManager.Instance.CreateNewControllerWrapper(_wrapperName, _pageNameList,_pageTipsList);
                // this.editorWindow.Close();
                this.Close();
            }
            else if (UIExpansionManager.Instance.ControllerSettings.InEditState)
            {
                if (string.IsNullOrEmpty(_wrapperName))
                {
                    EditorUtility.DisplayDialog("保存失败", "控制器名称不能为空。", "确认");
                    return;
                }
                if (!IsNumAndEnCh(_wrapperName))
                {
                    EditorUtility.DisplayDialog("保存失败", "控制器名称只能包含字母与数字。", "确认");
                    return;
                }
                foreach (UIExpansionControllerWrapper controller in UIExpansionManager.Instance.CurUIExpansionWrapper.ControllerWrapperList)
                {
                    if ((controller != UIExpansionManager.Instance.CurControllerWrapper) && controller.Name == _wrapperName)
                    {
                        EditorUtility.DisplayDialog("保存失败", "已存在相同名称的控制器。", "确认");
                        return;
                    }
                }
                if (_pageNameList.Count < 2)
                {
                    EditorUtility.DisplayDialog("保存失败", "控制器页面数量必须大于2。", "确认");
                    return;
                }
                UIExpansionManager.Instance.CurControllerWrapper.ApplyEditData(_editActionList, _wrapperName, _pageNameList, _pageTipsList);
                // this.editorWindow.Close();
                this.Close();
            }
        }

        public bool IsNumAndEnCh(string input)
        {
            string pattern = @"^[A-Za-z0-9]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        // public override void OnOpen()
        public void Awake()
        {
            if (UIExpansionManager.Instance.ControllerSettings.InCreateNewWrapperState)
            {
                _wrapperName = "";
                _pageNameList = new List<string>() { "", "" };
                _pageTipsList = new List<string>() { "", "" };
            }
            if (UIExpansionManager.Instance.ControllerSettings.InEditState)
            {
                _wrapperName = UIExpansionManager.Instance.CurControllerWrapper.Name;
                _pageNameList = new List<string>();
                _pageTipsList = new List<string>();
                _editActionList = new List<ControllerWrapperEditAction>();
                for (int i = 0; i < UIExpansionManager.Instance.CurControllerWrapper.PageNameList.Count; i++)
                {
                    _pageNameList.Add(UIExpansionManager.Instance.CurControllerWrapper.PageNameList[i]);
                    _pageTipsList.Add(UIExpansionManager.Instance.CurControllerWrapper.PageTipsList[i]);
                }
            }
        }

        public void OnDestroy()
        {
            if (UIExpansionManager.Instance.ControllerSettings.InCreateNewWrapperState)
            {
                UIExpansionManager.Instance.ControllerSettings.InCreateNewWrapperState = false;
            }
            if (UIExpansionManager.Instance.ControllerSettings.InEditState)
            {
                UIExpansionManager.Instance.ControllerSettings.InEditState = false;
            }

        }
    }
}