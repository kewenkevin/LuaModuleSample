using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEditor;

namespace ND.UI.NDUI
{
    [CustomEditor(typeof(NDToggle), true)]
    [CanEditMultipleObjects]
    /// <summary>
    /// Custom Editor for the Toggle Component.
    /// Extend this class to write a custom editor for a component derived from Toggle.
    /// </summary>
    public class NDToggleEditor : NDSelectableEditor
    {
        SerializedProperty m_OnValueChangedProperty;
        SerializedProperty m_onProperty;
        SerializedProperty m_offProperty;
        SerializedProperty m_GraphicProperty;
        SerializedProperty m_InversGraphicProperty;
        SerializedProperty m_GroupProperty;
        SerializedProperty m_IsOnProperty;


        protected override void OnEnable()
        {
            base.OnEnable();
            m_GraphicProperty = serializedObject.FindProperty("graphic");
            m_InversGraphicProperty = serializedObject.FindProperty("inversGraphic");
            m_GroupProperty = serializedObject.FindProperty("m_Group");
            m_IsOnProperty = serializedObject.FindProperty("m_IsOn");
            m_OnValueChangedProperty = serializedObject.FindProperty("onValueChanged");
            m_onProperty = serializedObject.FindProperty("on");
            m_offProperty = serializedObject.FindProperty("off");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();
            var toggle = serializedObject.targetObject as NDToggle;
            EditorGUI.BeginChangeCheck();
             EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUILayout.PropertyField(m_IsOnProperty);
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                EditorSceneManager.MarkSceneDirty(toggle.gameObject.scene);
                var group = m_GroupProperty.objectReferenceValue as NDToggleGroup;

                toggle.isOn = m_IsOnProperty.boolValue;

                if (group != null && toggle.IsActive())
                {
                    if (toggle.isOn || (!group.AnyTogglesOn() && !group.allowSwitchOff))
                    {
                        toggle.isOn = true;
                        group.NotifyToggleOn(toggle);
                    }
                }
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_GraphicProperty);
            EditorGUILayout.PropertyField(m_InversGraphicProperty);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
                toggle.UpdateStatus();
            }
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_GroupProperty);
            if (EditorGUI.EndChangeCheck())
            {
                EditorSceneManager.MarkSceneDirty(toggle.gameObject.scene);
                var group = m_GroupProperty.objectReferenceValue as NDToggleGroup;
                toggle.group = group;
            }

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("状态改变:", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(m_onProperty);
            EditorGUILayout.PropertyField(m_offProperty);
            EditorGUILayout.EndVertical();

            {
                SerializedProperty persistentCalls = m_onProperty.FindPropertyRelative("m_PersistentCalls.m_Calls");
                for (int i = 0; i < persistentCalls.arraySize; ++i)
                {
                    persistentCalls.GetArrayElementAtIndex(i).FindPropertyRelative("m_CallState").intValue = (int)UnityEngine.Events.UnityEventCallState.EditorAndRuntime;
                }
            }

            {
                SerializedProperty persistentCalls = m_offProperty.FindPropertyRelative("m_PersistentCalls.m_Calls");
                for (int i = 0; i < persistentCalls.arraySize; ++i)
                {
                    persistentCalls.GetArrayElementAtIndex(i).FindPropertyRelative("m_CallState").intValue = (int)UnityEngine.Events.UnityEventCallState.EditorAndRuntime;
                }
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();

            if (EditorGUI.EndChangeCheck())
            {
                toggle.UpdateStatus();
            }

            EditorGUILayout.Space();

            // Draw the event notification options
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("事件回调:", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(m_OnValueChangedProperty);
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
