using UnityEditor;
using UnityEngine;

namespace ND.UI.NDUI.NDListView
{
    [CustomEditor(typeof(ListView), true)]
    public class NDListViewEditor : Editor
    {
        SerializedProperty m_cellItems;
        SerializedProperty m_totalCount;
        SerializedProperty m_selectionType;
        SerializedProperty m_itemSpace;
        SerializedProperty m_layout;
        SerializedProperty m_align;
        SerializedProperty m_verticalAlign;
        SerializedProperty m_isVirtual;

        protected virtual void OnEnable()
        {
            m_cellItems = serializedObject.FindProperty("m_ViewCells");
            m_totalCount = serializedObject.FindProperty("totalCount");
            m_selectionType = serializedObject.FindProperty("m_selectType");
            m_itemSpace = serializedObject.FindProperty("m_itemSpace");
            m_layout = serializedObject.FindProperty("_layout");
            m_align = serializedObject.FindProperty("_align");
            m_verticalAlign = serializedObject.FindProperty("_verticalAlign");
            m_isVirtual = serializedObject.FindProperty("m_isVirtual");
        }
        public override void OnInspectorGUI()
        {
            var list = target as ListView;
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(m_cellItems);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_totalCount);
            var totalCountChanged = EditorGUI.EndChangeCheck();
            EditorGUILayout.PropertyField(m_selectionType);
            EditorGUILayout.PropertyField(m_itemSpace);
            EditorGUILayout.PropertyField(m_layout);
            EditorGUILayout.PropertyField(m_align);
            EditorGUILayout.PropertyField(m_verticalAlign);
            EditorGUILayout.PropertyField(m_isVirtual);
            
            serializedObject.ApplyModifiedProperties();
            RestBtn();
            if (totalCountChanged)
            {
                list.EditorUpdate();
            }
        }

        void RestBtn()
        {
            var list = target as ListView;
            if (GUILayout.Button("Clear Editor Cells"))
            {
                list.EditorClean();
            }
        }
    }
}