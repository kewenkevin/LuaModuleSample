using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;

namespace ND.UI.NDUI
{
    [CustomEditor(typeof(NDSlider), true)]
    [CanEditMultipleObjects]
    /// <summary>
    /// Custom Editor for the Slider Component.
    /// Extend this class to write a custom editor for a component derived from Slider.
    /// </summary>
    public class NDSliderEditor : SelectableEditor
    {
        SerializedProperty m_Direction;
        SerializedProperty m_TextType;
        SerializedProperty m_FillRect;
        SerializedProperty m_HandleRect;
        SerializedProperty m_Text;
        SerializedProperty m_MinValue;
        SerializedProperty m_MaxValue;
        SerializedProperty m_WholeNumbers;
        SerializedProperty m_Value;
        SerializedProperty m_OnValueChanged;
        SerializedProperty m_Tween;
        SerializedProperty m_TweenDuration;
        SerializedProperty m_OnTweenComplete;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_FillRect = serializedObject.FindProperty("m_FillRect");
            m_HandleRect = serializedObject.FindProperty("m_HandleRect");
            m_Text = serializedObject.FindProperty("m_Text");
            m_TextType = serializedObject.FindProperty("m_TextType");
            m_Direction = serializedObject.FindProperty("m_Direction");
            m_MinValue = serializedObject.FindProperty("m_MinValue");
            m_MaxValue = serializedObject.FindProperty("m_MaxValue");
            m_WholeNumbers = serializedObject.FindProperty("m_WholeNumbers");
            m_Value = serializedObject.FindProperty("m_Value");
            m_OnValueChanged = serializedObject.FindProperty("m_OnValueChanged");
            m_Tween = serializedObject.FindProperty("m_Tween");
            m_TweenDuration = serializedObject.FindProperty("m_TweenDuration");
            m_OnTweenComplete = serializedObject.FindProperty("m_OnTweenComplete");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();

            EditorGUILayout.PropertyField(m_Tween);
            if (EditorGUILayout.BeginFadeGroup(m_Tween.boolValue?1:0))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_TweenDuration);
                EditorGUILayout.PropertyField(m_OnTweenComplete);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(m_FillRect);
            EditorGUILayout.PropertyField(m_HandleRect);
            EditorGUILayout.PropertyField(m_Text);
            EditorGUILayout.PropertyField(m_TextType);

            if (m_FillRect.objectReferenceValue != null || m_HandleRect.objectReferenceValue != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_Direction);
                if (EditorGUI.EndChangeCheck())
                {
                    NDSlider.Direction direction = (NDSlider.Direction)m_Direction.enumValueIndex;
                    foreach (var obj in serializedObject.targetObjects)
                    {
                        NDSlider slider = obj as NDSlider;
                        slider.SetDirection(direction, true);
                    }
                }


                EditorGUI.BeginChangeCheck();
                float newMin = EditorGUILayout.FloatField("Min Value", m_MinValue.floatValue);
                if (EditorGUI.EndChangeCheck() && newMin <= m_MaxValue.floatValue)
                {
                    m_MinValue.floatValue = newMin;
                }

                EditorGUI.BeginChangeCheck();
                float newMax = EditorGUILayout.FloatField("Max Value", m_MaxValue.floatValue);
                if (EditorGUI.EndChangeCheck() && newMax >= m_MinValue.floatValue)
                {
                    m_MaxValue.floatValue = newMax;
                }

                EditorGUILayout.PropertyField(m_WholeNumbers);
                EditorGUILayout.Slider(m_Value, m_MinValue.floatValue, m_MaxValue.floatValue);

                bool warning = false;
                foreach (var obj in serializedObject.targetObjects)
                {
                    NDSlider slider = obj as NDSlider;
                    NDSlider.Direction dir = slider.direction;
                    if (dir == NDSlider.Direction.LeftToRight || dir == NDSlider.Direction.RightToLeft)
                        warning = (slider.navigation.mode != Navigation.Mode.Automatic && (slider.FindSelectableOnLeft() != null || slider.FindSelectableOnRight() != null));
                    else
                        warning = (slider.navigation.mode != Navigation.Mode.Automatic && (slider.FindSelectableOnDown() != null || slider.FindSelectableOnUp() != null));
                }

                if (warning)
                    EditorGUILayout.HelpBox("The selected slider direction conflicts with navigation. Not all navigation options may work.", MessageType.Warning);

                // Draw the event notification options
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_OnValueChanged);
            }
            else
            {
                EditorGUILayout.HelpBox("Specify a RectTransform for the slider fill or the slider handle or both. Each must have a parent RectTransform that it can slide within.", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
