using UnityEditor;
using UnityEngine;

namespace ND.UI.Core.StyleDataEditor
{
    [CustomEditor(typeof(TextStyleBase))]
    public class TextStyleBaseEditor : UnityEditor.Editor
    {
        SerializedProperty m_Font;
        SerializedProperty m_FontStyle;
        SerializedProperty m_FontSize;

        private void OnEnable()
        {
            m_Font = serializedObject.FindProperty("m_Font");
            m_FontStyle = serializedObject.FindProperty("m_FontStyle");
            m_FontSize = serializedObject.FindProperty("m_FontSize");
            
        }
        
        public override void OnInspectorGUI()
        {
            var textStyle = (TextStyleBase)serializedObject.targetObject;
            
            serializedObject.Update();
            
            //别名
            EditorGUI.BeginChangeCheck();
            var aliasName = EditorGUILayout.DelayedTextField("Alias Name", GetAliasName(textStyle));
            if(EditorGUI.EndChangeCheck()) {
                SetAliasName(textStyle, aliasName);
            }
            
            EditorGUILayout.Space();
            
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.PropertyField(m_Font);
            EditorGUILayout.PropertyField(m_FontStyle);
            EditorGUILayout.PropertyField(m_FontSize);

            if (EditorGUI.EndChangeCheck())
            {
                Debug.Assert(m_Font.objectReferenceValue != null, "请注意当前选择的字体为空");
                serializedObject.ApplyModifiedProperties();
                ApplyToCurrentStage();
            }
            
            EditorGUILayout.Space();
            
            EditorGUILayout.Separator();
            
            var style = new GUIStyle("label");
            style.richText = true;
            style.fontSize = textStyle.fontSize;
            style.fontStyle = textStyle.fontStyle;
            style.font = textStyle.font;
            GUILayout.Label($"字体样式FontStyle", style);
        }
        
        public static string GetAliasName(TextStyleBase style) => AliasNameData.Get(style)?.AliasName;

        public static void SetAliasName(TextStyleBase style, string aliasName)
        {
            var aliasNameSet = AliasNameData.Get(style);
            if (aliasNameSet != null)
            {
                aliasNameSet.AliasName = aliasName;
            }
        }

        protected virtual void ApplyToCurrentStage()
        {
        }
    }
}