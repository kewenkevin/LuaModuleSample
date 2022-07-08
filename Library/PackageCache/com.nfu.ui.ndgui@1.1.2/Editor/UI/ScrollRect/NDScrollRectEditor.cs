using System;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;

namespace ND.UI.NDUI
{
    [CustomEditor(typeof(NDScrollRect), true)]
    public class NDScrollRectEditor : Editor
    {
        int index = 0;
        float speed = 1000;
        SerializedProperty m_TotalCount; 
        SerializedProperty m_Content;
        SerializedProperty m_Direction;
        SerializedProperty m_ReverseDirection;
        SerializedProperty m_MovementType;
        SerializedProperty m_Elasticity;
        SerializedProperty m_CanScroll;
        SerializedProperty m_Inertia;
        SerializedProperty m_Snap;
        SerializedProperty m_SnapLerp;
        SerializedProperty m_DecelerationRate;
        SerializedProperty m_ScrollSensitivity;
        SerializedProperty m_Viewport;
        SerializedProperty m_Scrollbar;
        SerializedProperty m_ScrollbarVisibility;
        SerializedProperty m_OnValueChanged;
        AnimBool m_ShowElasticity;
        AnimBool m_ShowDecelerationRate;
        AnimBool m_ShowSnapLerp;
        bool m_ViewportIsNotChild, m_ScrollbarIsNotChild;
        static string s_Error = "For this visibility mode, the Viewport property and the Scrollbar property both needs to be set to a Rect Transform that is a child to the Scroll Rect.";
       
        protected virtual void OnEnable()
        {
            m_Content = serializedObject.FindProperty("m_Content");
            m_Direction = serializedObject.FindProperty("direction");
            m_ReverseDirection = serializedObject.FindProperty("reverseDirection");
            m_TotalCount = serializedObject.FindProperty("totalCount");
            m_MovementType = serializedObject.FindProperty("m_MovementType");
            m_Elasticity = serializedObject.FindProperty("m_Elasticity");
            m_CanScroll = serializedObject.FindProperty("m_CanScroll");
            m_Inertia = serializedObject.FindProperty("m_Inertia");
            m_Snap = serializedObject.FindProperty("m_Snap");
            m_SnapLerp = serializedObject.FindProperty("m_SnapLerp");
            m_DecelerationRate = serializedObject.FindProperty("m_DecelerationRate");
            m_ScrollSensitivity = serializedObject.FindProperty("m_ScrollSensitivity");
            m_Viewport = serializedObject.FindProperty("m_Viewport");
            m_Scrollbar = serializedObject.FindProperty("m_Scrollbar");
            m_ScrollbarVisibility = serializedObject.FindProperty("m_ScrollbarVisibility");
            m_OnValueChanged = serializedObject.FindProperty("m_OnValueChanged");

            m_ShowElasticity = new AnimBool(Repaint);
            m_ShowDecelerationRate = new AnimBool(Repaint);
            m_ShowSnapLerp = new AnimBool(Repaint);
            SetAnimBools(true);
        }

        protected virtual void OnDisable()
        {
            m_ShowElasticity.valueChanged.RemoveListener(Repaint);
            m_ShowDecelerationRate.valueChanged.RemoveListener(Repaint);
            m_ShowSnapLerp.valueChanged.RemoveListener(Repaint);
        }

        void SetAnimBools(bool instant)
        {
            SetAnimBool(m_ShowElasticity, !m_MovementType.hasMultipleDifferentValues && m_MovementType.enumValueIndex == (int)ScrollRect.MovementType.Elastic, instant);
            SetAnimBool(m_ShowDecelerationRate, !m_Inertia.hasMultipleDifferentValues && m_Inertia.boolValue == true, instant);
            SetAnimBool(m_ShowSnapLerp, !m_Snap.hasMultipleDifferentValues && m_Snap.boolValue == true, instant);
        }

        void SetAnimBool(AnimBool a, bool value, bool instant)
        {
            if (instant)
                a.value = value;
            else
                a.target = value;
        }

        void CalculateCachedValues()
        {
            m_ViewportIsNotChild = false;
            m_ScrollbarIsNotChild = false;
            if (targets.Length == 1)
            {
                Transform transform = ((NDScrollRect)target).transform;
                if (m_Viewport.objectReferenceValue == null || ((RectTransform)m_Viewport.objectReferenceValue).transform.parent != transform)
                    m_ViewportIsNotChild = true;
                if (m_Scrollbar.objectReferenceValue == null || ((Scrollbar)m_Scrollbar.objectReferenceValue).transform.parent != transform)
                    m_ScrollbarIsNotChild = true;
            }
        }

        


        public override void OnInspectorGUI()
        {
            var scroll = target as NDScrollRect;
            
            SetAnimBools(false);

            serializedObject.Update();
            // Once we have a reliable way to know if the object changed, only re-cache in that case.
            CalculateCachedValues();
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_CanScroll);
            
            EditorGUILayout.PropertyField(m_TotalCount);
            var totalCountChanged = EditorGUI.EndChangeCheck();

            EditorGUILayout.PropertyField(m_Content);

            EditorGUILayout.PropertyField(m_Direction);
            //EditorGUILayout.PropertyField(m_ReverseDirection);

            EditorGUILayout.PropertyField(m_MovementType);
            if (EditorGUILayout.BeginFadeGroup(m_ShowElasticity.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_Elasticity);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(m_Inertia);
            if (EditorGUILayout.BeginFadeGroup(m_ShowDecelerationRate.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_DecelerationRate);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(m_Snap);
            if (EditorGUILayout.BeginFadeGroup(m_ShowSnapLerp.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_SnapLerp);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(m_ScrollSensitivity);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_Viewport);

            EditorGUILayout.PropertyField(m_Scrollbar);
            if (m_Scrollbar.objectReferenceValue && !m_Scrollbar.hasMultipleDifferentValues)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_ScrollbarVisibility, EditorGUIUtility.TrTextContent("Visibility"));

                if ((ScrollRect.ScrollbarVisibility)m_ScrollbarVisibility.enumValueIndex == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport
                    && !m_ScrollbarVisibility.hasMultipleDifferentValues)
                {
                    if (m_ViewportIsNotChild || m_ScrollbarIsNotChild)
                        EditorGUILayout.HelpBox(s_Error, MessageType.Error);
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_OnValueChanged);

            serializedObject.ApplyModifiedProperties();

            CustomInspectorGUI();

            //if (totalCountChanged && Application.isPlaying)
            if (totalCountChanged)
            {
                scroll.RefreshCells();
            }
            scroll.EditorUpdate();
        }

        protected void CustomInspectorGUI()
        {
            EditorGUILayout.Space();
            var scroll = target as NDScrollRect;
            GUI.enabled = Application.isPlaying;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear"))
            {
                scroll.ClearCells();
            }
            if (GUILayout.Button("Refresh"))
            {
                scroll.RefreshCells();
            }
            if (GUILayout.Button("ReloadData"))
            {
                scroll.ReloadData();
            }
            if (GUILayout.Button("Test"))
            {
                scroll.normalizedPosition = Vector2.one;
            }
            //if (GUILayout.Button("RefillFromEnd"))
            //{
            //    scroll.RefillCellsFromEnd();
            //}
            EditorGUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = 45;
            float w = (EditorGUIUtility.currentViewWidth - 100) / 2;
            EditorGUILayout.BeginHorizontal();
            index = EditorGUILayout.IntField("Index", index, GUILayout.Width(w));
            speed = EditorGUILayout.FloatField("Speed", speed, GUILayout.Width(w));
            if (GUILayout.Button("Scroll", GUILayout.Width(45)))
            {
                scroll.ScrollToCell(index, speed);
            }
            EditorGUILayout.EndHorizontal();

        }
    }
}