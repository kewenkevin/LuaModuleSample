using ND.UI.NDUI.Utils;
using UnityEditor;
using UnityEngine;

namespace ND.UI.NDUI
{
    [CustomEditor(typeof(NDDropDown), true)]
    [CanEditMultipleObjects]
    public class NDDropDownEditor : NDSelectableEditor
    {
        private SerializedProperty m_Template;
        private SerializedProperty m_CaptionText;
        private SerializedProperty m_CaptionImage;
        private SerializedProperty m_ItemText;
        private SerializedProperty m_ItemImage;
        private SerializedProperty m_OnSelectionChanged;
        private SerializedProperty m_Value;
        private SerializedProperty m_Options;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_Template = this.serializedObject.FindProperty("m_Template");
            this.m_CaptionText = this.serializedObject.FindProperty("m_CaptionText");
            this.m_CaptionImage = this.serializedObject.FindProperty("m_CaptionImage");
            this.m_ItemText = this.serializedObject.FindProperty("m_ItemText");
            this.m_ItemImage = this.serializedObject.FindProperty("m_ItemImage");
            this.m_OnSelectionChanged = this.serializedObject.FindProperty("m_OnValueChanged");
            this.m_Value = this.serializedObject.FindProperty("m_Value");
            this.m_Options = this.serializedObject.FindProperty("m_Options");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.m_Template);
            EditorGUILayout.PropertyField(this.m_CaptionText);

            var isPrefabInstance = UIPrefabUtils.IsPrefabInstance(serializedObject.targetObject as MonoBehaviour);

            EditorGUI.BeginDisabledGroup(isPrefabInstance);
            
            EditorGUILayout.PropertyField(this.m_CaptionImage);
            
            EditorGUILayout.PropertyField(this.m_ItemImage);
            
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.PropertyField(this.m_ItemText);

            
            EditorGUILayout.PropertyField(this.m_Value);
            EditorGUILayout.PropertyField(this.m_Options);
            EditorGUILayout.PropertyField(this.m_OnSelectionChanged);
            this.serializedObject.ApplyModifiedProperties();
        }
    }
}