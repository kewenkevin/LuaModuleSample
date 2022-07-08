﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


namespace ND.UI.NDUI
{
    [CustomEditor(typeof(NDScrollbar), true)]
    [CanEditMultipleObjects]
    public class NDScrollBarEditor : NDSelectableEditor
    {
        private SerializedProperty m_HandleRect;
        private SerializedProperty m_Direction;
        private SerializedProperty m_Value;
        private SerializedProperty m_Size;
        private SerializedProperty m_NumberOfSteps;
        private SerializedProperty m_OnValueChanged;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_HandleRect        = serializedObject.FindProperty("m_HandleRect");
            m_Direction         = serializedObject.FindProperty("m_Direction");
            m_Value             = serializedObject.FindProperty("m_Value");
            m_Size              = serializedObject.FindProperty("m_Size");
            m_NumberOfSteps     = serializedObject.FindProperty("m_NumberOfSteps");
            m_OnValueChanged    = serializedObject.FindProperty("m_OnValueChanged");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();

            // EditorGUILayout.PropertyField(m_HandleRect);
            EditorGUI.BeginChangeCheck();
            RectTransform newRectTransform = EditorGUILayout.ObjectField("Handle Rect", m_HandleRect.objectReferenceValue, typeof(RectTransform), true) as RectTransform;
            if (EditorGUI.EndChangeCheck())
            {
                // Handle Rect will modify its GameObject RectTransform drivenBy, so we need to Record the old and new RectTransform.
                List<UnityEngine.Object> modifiedObjects = new List<UnityEngine.Object>();
                modifiedObjects.Add(newRectTransform);
                foreach (var target in m_HandleRect.serializedObject.targetObjects)
                {
                    MonoBehaviour mb = target as MonoBehaviour;
                    if (mb == null)
                        continue;

                    modifiedObjects.Add(mb);
                    modifiedObjects.Add(mb.GetComponent<RectTransform>());
                }
                Undo.RecordObjects(modifiedObjects.ToArray(), "Change Handle Rect");
                m_HandleRect.objectReferenceValue = newRectTransform;
            }

            if (m_HandleRect.objectReferenceValue != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_Direction);
                if (EditorGUI.EndChangeCheck())
                {
                    Scrollbar.Direction direction = (Scrollbar.Direction)m_Direction.enumValueIndex;
                    foreach (var obj in serializedObject.targetObjects)
                    {
                        Scrollbar scrollbar = obj as Scrollbar;
                        scrollbar.SetDirection(direction, true);
                    }
                }

                EditorGUILayout.PropertyField(m_Value);
                EditorGUILayout.PropertyField(m_Size);
                EditorGUILayout.PropertyField(m_NumberOfSteps);

                bool warning = false;
                foreach (var obj in serializedObject.targetObjects)
                {
                    Scrollbar scrollbar = obj as Scrollbar;
                    Scrollbar.Direction dir = scrollbar.direction;
                    if (dir == Scrollbar.Direction.LeftToRight || dir == Scrollbar.Direction.RightToLeft)
                        warning = (scrollbar.navigation.mode != Navigation.Mode.Automatic && (scrollbar.FindSelectableOnLeft() != null || scrollbar.FindSelectableOnRight() != null));
                    else
                        warning = (scrollbar.navigation.mode != Navigation.Mode.Automatic && (scrollbar.FindSelectableOnDown() != null || scrollbar.FindSelectableOnUp() != null));
                }

                if (warning)
                    EditorGUILayout.HelpBox("The selected scrollbar direction conflicts with navigation. Not all navigation options may work.", MessageType.Warning);

                EditorGUILayout.Space();
                // Draw the event notification options
                EditorGUILayout.PropertyField(m_OnValueChanged);
            }
            else
            {
                EditorGUILayout.HelpBox("Specify a RectTransform for the scrollbar handle. It must have a parent RectTransform that the handle can slide within.", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}