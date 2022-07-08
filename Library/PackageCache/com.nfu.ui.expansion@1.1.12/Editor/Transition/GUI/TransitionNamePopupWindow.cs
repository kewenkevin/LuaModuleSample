using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

namespace ND.UI
{
    public class TransitionNamePopupWindow : PopupWindowContent
    {
        private string _wrapperName = "";


        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("名称：", EditorStyles.boldLabel);
            _wrapperName = GUILayout.TextField(_wrapperName);
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("保存", GUILayout.Width(60)))
            {
                OnSaveBtnClick();
            }
            GUILayout.EndHorizontal();
        }

        public override void OnOpen()
        {
            if (UIExpansionManager.Instance.TransitionSettings.InCreateNewWrapperState)
            {
                _wrapperName = "";
            }
            if (UIExpansionManager.Instance.TransitionSettings.InChangeWrapperNameState)
            {
                _wrapperName = UIExpansionManager.Instance.CurTransitionWrapper.Name;
            }
        }

        public override void OnClose()
        {
            if (UIExpansionManager.Instance.TransitionSettings.InCreateNewWrapperState)
            {
                UIExpansionManager.Instance.TransitionSettings.InCreateNewWrapperState = false;
            }
            if (UIExpansionManager.Instance.TransitionSettings.InChangeWrapperNameState)
            {
                UIExpansionManager.Instance.TransitionSettings.InChangeWrapperNameState = false;
            }
   
        }

        private void OnSaveBtnClick()
        {
            if (UIExpansionManager.Instance.TransitionSettings.InCreateNewWrapperState)
            {
                if (string.IsNullOrEmpty(_wrapperName))
                {
                    EditorUtility.DisplayDialog("保存失败", "动效名称不能为空。", "确认");
                    return;
                }
                if (!IsNumAndEnCh(_wrapperName))
                {
                    EditorUtility.DisplayDialog("保存失败", "动效名称只能包含字母与数字。", "确认");
                    return;
                }
                foreach (UIExpansionTransitionWrapper transition in UIExpansionManager.Instance.CurUIExpansionWrapper.TransitionWrapperList)
                {
                    if (transition.Name == _wrapperName)
                    {
                        EditorUtility.DisplayDialog("保存失败", "已存在相同名称的动效。", "确认");
                        return;
                    }
                }
                UIExpansionManager.Instance.CreateNewTransitionWrapper(_wrapperName);
                this.editorWindow.Close();
            }

            if (UIExpansionManager.Instance.TransitionSettings.InChangeWrapperNameState)
            {
                if (string.IsNullOrEmpty(_wrapperName))
                {
                    EditorUtility.DisplayDialog("保存失败", "动效名称不能为空。", "确认");
                    return;
                }
                if (!IsNumAndEnCh(_wrapperName))
                {
                    EditorUtility.DisplayDialog("保存失败", "动效名称只能包含字母与数字。", "确认");
                    return;
                }
                foreach (UIExpansionTransitionWrapper transition in UIExpansionManager.Instance.CurUIExpansionWrapper.TransitionWrapperList)
                {
                    if ((transition != UIExpansionManager.Instance.CurTransitionWrapper) && transition.Name == _wrapperName)
                    {
                        EditorUtility.DisplayDialog("保存失败", "已存在相同名称的动效。", "确认");
                        return;
                    }
                }
                UIExpansionManager.Instance.CurTransitionWrapper.Name = _wrapperName;
                this.editorWindow.Close();
            }
        }

        public bool IsNumAndEnCh(string input)
        {
            string pattern = @"^[A-Za-z0-9]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }
    }
}