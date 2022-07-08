using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

using UnityEngine.UI;

namespace ND.UI.NDUI
{
    [CustomEditor(typeof(NDRating), true)]
    public class NDRatingEditor : Editor
    {
        SerializedProperty m_Spacing;
        SerializedProperty m_BackgroundImage;
        SerializedProperty m_HighlightImage;
        
        //SerializedProperty m_Highlight;
        SerializedProperty m_Total;
        SerializedProperty m_Current;
        
        //SerializedProperty m_Direction;
        SerializedProperty m_Background;
        SerializedProperty m_Highlight;
        void OnEnable()
        {
            m_Spacing = serializedObject.FindProperty("m_Spacing");
            m_BackgroundImage = serializedObject.FindProperty("m_BackgroundImage");
            m_HighlightImage = serializedObject.FindProperty("m_HighlightImage");
            
            m_Background = serializedObject.FindProperty("m_Background");
            m_Highlight = serializedObject.FindProperty("m_Highlight");
            
            
            m_Total = serializedObject.FindProperty("m_Total");
            m_Current = serializedObject.FindProperty("m_Current");
            
        }

        enum RatingLayoutDirection
        {
            Vertical,
            Horizontal
        }

        private RatingLayoutDirection m_RatingLayoutDirection = RatingLayoutDirection.Vertical;
        private readonly Type m_HorizontalLayout = typeof(HorizontalLayoutGroup);
        private readonly Type m_VerticalLayout = typeof(VerticalLayoutGroup);
        
        private bool CheckIsSameLayout(out RatingLayoutDirection layoutLayoutDirection)
        {
            layoutLayoutDirection = RatingLayoutDirection.Horizontal;

            var layouts = new[]
            {
                m_Background.objectReferenceValue,
                m_Highlight.objectReferenceValue
            };
            
            var lastDirection = -1;
            foreach (var layout in layouts)
            {
                if (layout == null)
                {
                    return false;
                }
                
                var t = layout.GetType();
                if(t == m_HorizontalLayout)
                {
                    if (lastDirection > 0 && lastDirection != 2)
                    {
                        return false;
                    }

                    lastDirection = 2;
                    layoutLayoutDirection = RatingLayoutDirection.Horizontal;

                }
                else if(t == m_VerticalLayout)
                {
                    if (lastDirection > 0 && lastDirection != 1)
                    {
                        return false;
                    }
                    
                    lastDirection = 1;
                    layoutLayoutDirection = RatingLayoutDirection.Vertical;
                }
                else
                {
                    return false;
                }
            }
            
            return true;
        }

        private void ReplaceLayout(RatingLayoutDirection nextDirection)
        {
            Type nextLayoutGroupType;
            if (nextDirection == RatingLayoutDirection.Horizontal)
            {
                nextLayoutGroupType = m_HorizontalLayout;
            }
            else if (nextDirection == RatingLayoutDirection.Vertical)
            {
                nextLayoutGroupType = m_VerticalLayout;
            }
            else
            {
                return;
            }
            
            var layoutElements = new[]
            {
                m_Background,
                m_Highlight
            };
            
            
            for (var i = layoutElements.Length - 1; i >= 0; i--)
            {
                var oldLayout = (HorizontalOrVerticalLayoutGroup) layoutElements[i].objectReferenceValue;
                var parent = oldLayout.gameObject;
                DestroyImmediate(oldLayout);

                var nextLayout = (HorizontalOrVerticalLayoutGroup) parent.AddComponent(nextLayoutGroupType);
                layoutElements[i].objectReferenceValue = nextLayout;
            }
            
        }
        
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            serializedObject.Update();

            

            var rating = serializedObject.targetObject as NDRating;
            
            if (CheckIsSameLayout(out var layoutDirection))
            {
                EditorGUI.BeginChangeCheck();
                
                var nextLayout = (RatingLayoutDirection) EditorGUILayout.EnumPopup("Direction", layoutDirection);
                
                if (EditorGUI.EndChangeCheck() && nextLayout != layoutDirection)
                {
                    ReplaceLayout(nextLayout);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("当前组件已被魔改，请自行确保组件运行正常", MessageType.Warning);
            }
            
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_Spacing);
            if (EditorGUI.EndChangeCheck())
            {
                SyncSpacing();
            }
            
            
            
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.PropertyField(m_BackgroundImage);
            EditorGUILayout.PropertyField(m_HighlightImage);
            
            m_Total.intValue = Mathf.RoundToInt(EditorGUILayout.Slider( "Total", m_Total.intValue, 1, 100));
            
            m_Current.doubleValue = (double)EditorGUILayout.Slider( "Current", m_Current.floatValue, 0, m_Total.intValue);
            
            if (EditorGUI.EndChangeCheck())
            {
                var updateDisplayMethod = typeof(NDRating).GetMethod("UpdateDisplay", BindingFlags.Instance | BindingFlags.NonPublic);
                updateDisplayMethod?.Invoke(rating, new object[0]);

            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private void SyncSpacing()
        {
            var background = m_Background.objectReferenceValue as HorizontalOrVerticalLayoutGroup;
            var highLight = m_Highlight.objectReferenceValue as HorizontalOrVerticalLayoutGroup;
            
            if(background != null) background.spacing = m_Spacing.floatValue;
            if(highLight != null) highLight.spacing = m_Spacing.floatValue;
            
        }
    }
}
