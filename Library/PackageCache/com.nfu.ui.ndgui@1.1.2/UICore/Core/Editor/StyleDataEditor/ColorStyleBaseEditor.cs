using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace ND.UI.Core.StyleDataEditor
{
    [CustomEditor(typeof(ColorStyleBase))]
    public class ColorStyleBaseEditor : UnityEditor.Editor
    {
        SerializedProperty m_ColorType;

        SerializedProperty m_Color;

        SerializedProperty m_Gradients;

        AnimBool m_ColorAnim;
        AnimBool m_GradientAnim;


        private void OnEnable()
        {
            m_ColorType = serializedObject.FindProperty("m_ColorType");
            m_Color = serializedObject.FindProperty("m_Color");
            m_Gradients = serializedObject.FindProperty("m_Gradients");

            m_ColorAnim = new AnimBool();
            m_GradientAnim = new AnimBool();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var colorStyle = (ColorStyleBase) serializedObject.targetObject;

            //别名
            EditorGUI.BeginChangeCheck();
            var aliasName = EditorGUILayout.DelayedTextField("Alias Name", GetAliasName(colorStyle));
            if (EditorGUI.EndChangeCheck())
            {
                SetAliasName(colorStyle, aliasName);
            }

            EditorGUILayout.Space();


            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(m_ColorType, new GUIContent("Color Type"));
            EditorGUILayout.Space();

            m_ColorAnim.target = m_ColorType.intValue == (int) ColorStyleBase.ColorType.Solid;
            m_GradientAnim.target = m_ColorType.intValue == (int) ColorStyleBase.ColorType.Gradient;

            if (EditorGUILayout.BeginFadeGroup(m_ColorAnim.faded))
            {
                EditorGUILayout.PropertyField(m_Color, new GUIContent("Solid Color"));
            }

            EditorGUILayout.EndFadeGroup();

            if (EditorGUILayout.BeginFadeGroup(m_GradientAnim.faded))
            {
                EditorGUILayout.LabelField(new GUIContent("Gradient Color"));
                EditorGUILayout.BeginVertical(GUI.skin.box);
                ++EditorGUI.indentLevel;

                if (m_Gradients.isArray)
                {
                    var len = m_Gradients.arraySize;
                    if (len < 2)
                    {
                        len = InitColorArr(m_Gradients, 2, Color.white);
                    }

                    for (var i = 0; i < len; i++)
                    {
                        EditorGUILayout.PropertyField(m_Gradients.GetArrayElementAtIndex(i),
                            new GUIContent(string.Empty));
                    }
                }

                --EditorGUI.indentLevel;
                EditorGUILayout.EndVertical();
            }
            else
            {
                m_Gradients.ClearArray();
            }

            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.Space();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                ApplyToCurrentStage();
            }
        }

        private int InitColorArr(SerializedProperty prop, int size, Color color)
        {
            if (prop.isArray)
            {
                for (var i = prop.arraySize; i < size; i++)
                {
                    prop.InsertArrayElementAtIndex(i);
                    prop.GetArrayElementAtIndex(i).colorValue = color;
                }
            }

            return size;
        }

        protected virtual void ApplyToCurrentStage()
        {
        }

        public static string GetAliasName(ColorStyleBase style) => AliasNameData.Get(style)?.AliasName;

        public static void SetAliasName(ColorStyleBase style, string aliasName)
        {
            var aliasNameSet = AliasNameData.Get(style);
            if (aliasNameSet != null)
            {
                aliasNameSet.AliasName = aliasName;
            }
        }
    }

}
