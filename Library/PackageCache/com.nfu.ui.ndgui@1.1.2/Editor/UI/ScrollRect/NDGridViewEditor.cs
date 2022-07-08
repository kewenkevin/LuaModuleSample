
using UnityEditor;
using UnityEngine.UI;
using UnityEngine;

namespace ND.UI.NDUI
{
    [CustomEditor(typeof(NDGridView), true)]
    public class NDGridViewEditor : NDScrollRectEditor
    {
        SerializedProperty m_ConstraintCount;
        SerializedProperty m_GridSpacing;
        SerializedProperty m_Prefab;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_ConstraintCount = serializedObject.FindProperty("m_ConstraintCount");
            m_GridSpacing = serializedObject.FindProperty("m_GridSpacing");
            m_Prefab = serializedObject.FindProperty("m_Prefab");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_Prefab);
            EditorGUILayout.PropertyField(m_ConstraintCount);
            EditorGUILayout.PropertyField(m_GridSpacing);
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();


            var scroll = target as NDGridView;
            scroll.CheckLayout();
        }
    }
}