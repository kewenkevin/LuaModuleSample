using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ND.Managers.ResourceMgr.Editor.GUIExtensions
{
    internal static partial class GUIExtensions
    {


        public static string DelayedTextField(this Rect rect, string value, out string current, GUIStyle style = null, Func<string, string> textField = null, Action<bool> onEnd = null)
        {
            var evt = Event.current;
            int ctrlId = GUIUtility.GetControlID(FocusType.Keyboard, rect);
            var state = (DelayedTextFieldState)GUIUtility.GetStateObject(typeof(DelayedTextFieldState), ctrlId);

            if (style == null)
                style = "textfield";

            Action<bool> submit = (b) =>
            {
                state.isEditing = false;
                if (GUIUtility.keyboardControl == state.inputControlId)
                    GUIUtility.keyboardControl = 0;
                if (b)
                {
                    if (!string.Equals(value, state.value))
                    {
                        value = state.value;
                        GUI.changed = true;
                    }
                }
                onEnd?.Invoke(b);
            };
            if (state.isEditing)
            {

                if (evt.type == EventType.KeyDown)
                {

                    if (evt.keyCode == KeyCode.Escape)
                    {
                        evt.Use();
                        submit(false);
                    }
                    else if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                    {
                        evt.Use();
                        submit(true);
                    }
                }
                else if (evt.type == EventType.MouseDown)
                {
                    if (!rect.Contains(evt.mousePosition))
                    {
                        evt.Use();
                        submit(true);
                    }
                }

                if (state.isEditing && GUIUtility.keyboardControl != state.inputControlId)
                {
                    submit(true);
                }
            }


            if (!state.isEditing)
            {
                state.value = value;
            }
            bool changed = GUI.changed;
            if (textField != null)
                state.value = textField(state.value);
            else
                state.value = GUI.TextField(rect, state.value, style);
            //GUILayout.Label(ctrlId + ", " + GetLastControlId() + "");
            if (!state.isEditing && GUIUtility.keyboardControl > 0)
            {
                int textCtrlId = EditorGUILayoutx.GetLastControlId();
                if ((textCtrlId != 0 && GUIUtility.keyboardControl == textCtrlId) || (GUIUtility.keyboardControl == ctrlId + 1 || GUIUtility.keyboardControl == ctrlId + 2))
                {
                    state.isEditing = true;
                    state.value = value;
                    state.inputControlId = GUIUtility.keyboardControl;
                }
            }

            GUI.changed = changed;
            current = state.value;
            return value;
        }



        class DelayedTextFieldState
        {
            public int inputControlId;
            public string value;
            public bool isEditing;
        }
        public static string DelayedPlaceholderField(this Rect rect, string text, GUIContent placeholder, GUIStyle textStyle = null, GUIStyle placeholderStyle = null, Func<string, string> textField = null)
        {
            string current;
            return DelayedPlaceholderField(rect, text, out current, placeholder, textStyle: textStyle, placeholderStyle: placeholderStyle, textField: textField);
        }

        public static string DelayedPlaceholderField(this Rect rect, string text, out string current, GUIContent placeholder, GUIStyle textStyle = null, GUIStyle placeholderStyle = null, Func<string, string> textField = null)
        {
            if (textStyle == null)
                textStyle = "textfield";

            bool isEmpty = false;

            text = DelayedTextField(rect, text, out current, textStyle, textField: textField);
            isEmpty = string.IsNullOrEmpty(current);

            if (isEmpty)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    if (placeholderStyle == null)
                        placeholderStyle = EditorGUILayoutx.Styles.Placeholder;
                    placeholderStyle.Draw(rect, placeholder, false, false, false, false);
                }
            }
            return text;
        }

        public static string ClipLeftText(this GUIStyle style, string text, string padding, int maxWidth)
        {
            if (text.Length < 2)
                return text;

            GUIContent content = new GUIContent(text);
            var size = style.CalcSize(content);

            if (size.x <= maxWidth)
                return text;

            int length = text.Length;
            int half;
            float newWidth = 0;
            //找到小于最大宽度
            half = (int)(length * 0.5f);
            while (half > 0)
            {
                length -= half;
                content.text = text.Substring(text.Length - length, length);
                newWidth = style.CalcSize(content).x;
                if (newWidth < maxWidth)
                {
                    break;
                }
                half = (int)(half * 0.5f);
            }

            //找到大于最大宽度
            half = (int)(half * 0.5f);
            while (half > 0)
            {
                int len = length + half;
                content.text = text.Substring(text.Length - len, len);
                newWidth = style.CalcSize(content).x;
                if (newWidth > maxWidth)
                {
                    break;
                }
                length += half;
                half = (int)(half * 0.5f);
            }

            //按字符补齐
            while (length < text.Length - 2)
            {
                int len = length + 3;
                content.text = text.Substring(text.Length - len, len);
                newWidth = style.CalcSize(content).x;
                if (newWidth > maxWidth)
                {
                    break;
                }
                length = len;
            }


            return padding + text.Substring(text.Length - length, length);
        }
    }
}
