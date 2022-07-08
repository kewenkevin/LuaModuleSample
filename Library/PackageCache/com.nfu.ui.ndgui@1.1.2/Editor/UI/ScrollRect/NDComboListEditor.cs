using UnityEditor;

namespace ND.UI.NDUI
{
    [CustomEditor(typeof(NDComboList),true)]
    public class NDComboListEditor : NDScrollRectEditor
    {
        SerializedProperty m_CellSpacing;
        SerializedProperty m_ViewCells;
        SerializedProperty m_TotalCount;
        SerializedProperty m_needScroll;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_CellSpacing = serializedObject.FindProperty("m_CellSpacing");
            m_ViewCells = serializedObject.FindProperty("m_ViewCells");
            // m_needScroll = serializedObject.FindProperty("needScroll");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
#if UNITY_2019_1_OR_NEWER
            EditorGUILayout.PropertyField(m_ViewCells);
            EditorGUILayout.PropertyField(m_CellSpacing);
            // EditorGUILayout.PropertyField(m_needScroll);
#else
            EditorGUILayout.PropertyField(m_ViewCells,true);
            EditorGUILayout.PropertyField(m_CellSpacing,true);
#endif
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
            
            var scroll = target as NDComboList;
            scroll.CheckLayout();
        }
    }
}