using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;

namespace ND.UI.NDUI
{
    [CustomEditor(typeof(NDList), true)]
    [CanEditMultipleObjects]
    /// <summary>
    /// Custom Editor for the Slider Component.
    /// Extend this class to write a custom editor for a component derived from Slider.
    /// </summary>
    public class NDListEditor : Editor
    {
        SerializedProperty m_Direction;
        SerializedProperty prefab;
        SerializedProperty totalCount;

        void OnEnable()
        {
            m_Direction = serializedObject.FindProperty("m_Direction");
            prefab = serializedObject.FindProperty("prefab");
            totalCount = serializedObject.FindProperty("m_totalCount");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_Direction);
            if (EditorGUI.EndChangeCheck())
            {
                NDList.Direction direction = (NDList.Direction)m_Direction.enumValueIndex;
                foreach (var obj in serializedObject.targetObjects)
                {
                    NDList slider = obj as NDList;
                    slider.SetDirection(direction);
                }
            }
            EditorGUILayout.PropertyField(prefab);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(totalCount);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in serializedObject.targetObjects)
                {
                    NDList slider = obj as NDList;
                    slider.UpdateCurrent(totalCount.intValue);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
