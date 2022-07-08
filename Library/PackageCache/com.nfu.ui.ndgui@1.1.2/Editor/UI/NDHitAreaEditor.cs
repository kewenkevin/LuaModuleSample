using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ND.UI.NDUI
{
    [CustomEditor(typeof(NDHitArea), true)]
    [CanEditMultipleObjects]
    /// <summary>
    /// Custom Editor for the YHitArea Component.
    /// </summary>
    public class NDHitAreaEditor : Editor
    {
        SerializedProperty m_RaycastTarget;

        protected void OnEnable()
        {
            m_RaycastTarget = serializedObject.FindProperty("m_RaycastTarget");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_RaycastTarget);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
