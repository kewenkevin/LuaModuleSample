using System;
using ND.UI.Core.StyleDataEditor;
using UnityEditor;
using UnityEngine;

namespace ND.UI.Core.TextEditor
{
    public static class TextEditorUtility
    {
        public static void DrawTextStyleField(GUIContent label, SerializedProperty property)
        {
            if (property == null)
            {
                return;
            }
            var rect = EditorGUILayout.GetControlRect(false, 20f, "button");
            
            void ApplyCallback(Core.TextStyleBase colorStyle, string refPath)
            {
                if (refPath != property.propertyPath) return;
                
                property.objectReferenceValue = colorStyle;
                property.serializedObject.ApplyModifiedProperties();
            }

            DrawTextStyleField( rect, label, (TextStyleBase) property.objectReferenceValue, property.propertyPath, ApplyCallback );
        }
        
        
        public static TextStyleBase DrawTextStyleField(Rect rect, GUIContent label, TextStyleBase currTextStyle, string refPath = null, Action<Core.TextStyleBase, string> applyCallback = null)
        {
            var content = EditorGUIUtility.ObjectContent(currTextStyle, typeof(Core.TextStyleBase));
            if (currTextStyle != null)
            {
                content.text = $"{TextStyleBaseEditor.GetAliasName(currTextStyle)}";
            }
            
            var objectButtonFieldStyle = new GUIStyle("ObjectFieldThumb");
            objectButtonFieldStyle.fixedHeight = 16;
            objectButtonFieldStyle.fixedWidth = rect.width - 20;
            objectButtonFieldStyle.fontSize = 12;
           
            
            if (label != GUIContent.none)
            {
                rect = EditorGUI.PrefixLabel(rect, label);
                objectButtonFieldStyle.fixedWidth -= EditorGUIUtility.labelWidth;
            }

            #region 修改样式

            #if UNITY_2019_1_OR_NEWER
                var btnStyle = new GUIStyle("ObjectFieldButton");
            #else
                var btnStyle = new GUIStyle("IN ObjectField");
            #endif
            
            btnStyle.margin = new RectOffset(2, 2, 2, 2);
            var changeBtnRect = rect;
            changeBtnRect.x = rect.x + rect.width - 20;
            changeBtnRect.height -= 4;
            changeBtnRect.width = 20;
            if (GUI.Button(changeBtnRect, "", btnStyle))
            {
                TextStyleSelector.Show(currTextStyle, refPath, applyCallback);
            }
            #endregion

            
            if (GUI.Button(rect, content, objectButtonFieldStyle))
            {
                if(currTextStyle != null) {
                    EditorGUIUtility.PingObject(currTextStyle);
                }
            }

            return currTextStyle;
        }

        
        
        

        public static void DrawTextColorStyleField(GUIContent label, SerializedProperty property)
        {
            if (property == null)
            {
                return;
            }
            
            var rect = EditorGUILayout.GetControlRect(false, 20f, "button");

            void ApplyCallback(ColorStyleBase colorStyle, string refPath)
            {
                if (refPath != property.propertyPath) return;
                
                property.objectReferenceValue = colorStyle;
                
                var rst = property.serializedObject.ApplyModifiedProperties();
                
            }

            DrawTextColorStyleField( rect, label, (ColorStyleBase) property.objectReferenceValue, property.propertyPath, ApplyCallback );
        }

        public static ColorStyleBase DrawTextColorStyleField(Rect rect, GUIContent label, ColorStyleBase currTextColorStyle, string refPath = null, Action<ColorStyleBase, string> applyCallback = null)
        {
            var content = EditorGUIUtility.ObjectContent(currTextColorStyle, typeof(ColorStyleBase));
            if (currTextColorStyle != null)
            {
                content.text = $"{ColorStyleBaseEditor.GetAliasName(currTextColorStyle)}";
            }
            
            var objectButtonFieldStyle = new GUIStyle("ObjectFieldThumb");
            objectButtonFieldStyle.fixedHeight = 16;
            objectButtonFieldStyle.fixedWidth = rect.width - 20;
            objectButtonFieldStyle.fontSize = 12;
            
            if (label != GUIContent.none)
            {
                rect = EditorGUI.PrefixLabel(rect, label);
                objectButtonFieldStyle.fixedWidth -= EditorGUIUtility.labelWidth;
            }

            #region 修改样式

            #if UNITY_2019_1_OR_NEWER
                var btnStyle = new GUIStyle("ObjectFieldButton");
            #else
                var btnStyle = new GUIStyle("IN ObjectField");
            #endif
            btnStyle.margin = new RectOffset(2, 2, 2, 2);
            var changeBtnRect = rect;
            changeBtnRect.x = rect.x + rect.width - 20;
            changeBtnRect.height -= 4;
            changeBtnRect.width = 20;
            if (GUI.Button(changeBtnRect, "", btnStyle))
            {
                TextColorStyleSelector.Show(currTextColorStyle, refPath, applyCallback);
            }
            #endregion

            
            if (GUI.Button(rect, content, objectButtonFieldStyle))
            {
                if(currTextColorStyle != null) {
                    EditorGUIUtility.PingObject(currTextColorStyle);
                }
            }

            return currTextColorStyle;
        }
        
        
    }
}