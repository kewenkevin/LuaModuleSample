
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;

namespace ND.UI.NDUI
{
    [CustomEditor(typeof(NDTableView), true)]
    public class NDTableViewEditor : NDScrollRectEditor
    {
        SerializedProperty m_CellSpacing;
        SerializedProperty m_TableViewCells;
        SerializedProperty m_TotalCount;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            m_CellSpacing = serializedObject.FindProperty("m_CellSpacing");
            m_TableViewCells = serializedObject.FindProperty("m_TableViewCells");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
#if UNITY_2019_1_OR_NEWER
            EditorGUILayout.PropertyField(m_TableViewCells);
            EditorGUILayout.PropertyField(m_CellSpacing);
#else
            EditorGUILayout.PropertyField(m_TableViewCells,true);
            EditorGUILayout.PropertyField(m_CellSpacing,true);
#endif
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();


            var scroll = target as NDTableView;
            scroll.CheckLayout();
        }
    }
}