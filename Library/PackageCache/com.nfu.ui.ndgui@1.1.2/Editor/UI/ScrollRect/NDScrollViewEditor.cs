using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace ND.UI.NDUI
{
    [CustomEditor(typeof(NDScrollView), true)]
    public class NDScrollViewEditor : ScrollRectEditor
    {
        SerializedProperty m_LayoutMode;
        SerializedProperty m_Content;
        SerializedProperty m_Horizontal;
        SerializedProperty m_Vertical;
        SerializedProperty m_MovementType;
        SerializedProperty m_Elasticity;
        SerializedProperty m_Inertia;
        SerializedProperty m_DecelerationRate;
        SerializedProperty m_ScrollSensitivity;
        SerializedProperty m_Viewport;
        SerializedProperty m_HorizontalScrollbar;
        SerializedProperty m_VerticalScrollbar;
        SerializedProperty m_HorizontalScrollbarVisibility;
        SerializedProperty m_VerticalScrollbarVisibility;
        SerializedProperty m_HorizontalScrollbarSpacing;
        SerializedProperty m_VerticalScrollbarSpacing;
        SerializedProperty m_OnValueChanged;
        
        AnimBool m_ShowElasticity;
        AnimBool m_ShowDecelerationRate;
        
        bool m_ViewportIsNotChild, m_HScrollbarIsNotChild, m_VScrollbarIsNotChild;
        
        static string s_HError = "For this visibility mode, the Viewport property and the Horizontal Scrollbar property both needs to be set to a Rect Transform that is a child to the Scroll Rect.";
        static string s_VError = "For this visibility mode, the Viewport property and the Vertical Scrollbar property both needs to be set to a Rect Transform that is a child to the Scroll Rect.";

        private static string s_DefaultModeNotice =
            "<color=#00ffff><size=12>Default Mode Only Support Fixed Size Contents.</size> \n <size=14>默认模式下仅支持固定尺寸大小的内容！</size></color>";
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            m_Content               = serializedObject.FindProperty("m_Content");
            m_Horizontal            = serializedObject.FindProperty("m_Horizontal");
            m_Vertical              = serializedObject.FindProperty("m_Vertical");
            m_MovementType          = serializedObject.FindProperty("m_MovementType");
            m_Elasticity            = serializedObject.FindProperty("m_Elasticity");
            m_Inertia               = serializedObject.FindProperty("m_Inertia");
            m_DecelerationRate      = serializedObject.FindProperty("m_DecelerationRate");
            m_ScrollSensitivity     = serializedObject.FindProperty("m_ScrollSensitivity");
            m_Viewport              = serializedObject.FindProperty("m_Viewport");
            m_HorizontalScrollbar   = serializedObject.FindProperty("m_HorizontalScrollbar");
            m_VerticalScrollbar     = serializedObject.FindProperty("m_VerticalScrollbar");
            m_HorizontalScrollbarVisibility = serializedObject.FindProperty("m_HorizontalScrollbarVisibility");
            m_VerticalScrollbarVisibility   = serializedObject.FindProperty("m_VerticalScrollbarVisibility");
            m_HorizontalScrollbarSpacing    = serializedObject.FindProperty("m_HorizontalScrollbarSpacing");
            m_VerticalScrollbarSpacing      = serializedObject.FindProperty("m_VerticalScrollbarSpacing");
            m_OnValueChanged        = serializedObject.FindProperty("m_OnValueChanged");

            m_ShowElasticity = new AnimBool(Repaint);
            m_ShowDecelerationRate = new AnimBool(Repaint);
        }
        
        public enum ViewLayoutType
        {
            DEFAULT = 0,
            VERTICAL = 1,
            HORIZONTAL = 2
        }
        
        
        
        private ViewLayoutType CalcViewLayout()
        {
            var view = serializedObject?.targetObject as NDScrollView;
            if (view == null) return ViewLayoutType.DEFAULT;

            var layoutGroup = view.content.GetComponent<HorizontalOrVerticalLayoutGroup>();
            if (layoutGroup != null)
            {
                if (layoutGroup.GetType() == typeof(HorizontalLayoutGroup))
                {
                    return ViewLayoutType.HORIZONTAL;
                }
                else
                {
                    return ViewLayoutType.VERTICAL;
                }
            }


            return ViewLayoutType.DEFAULT;
        }
        
        public override void OnInspectorGUI()
        {
            SetAnimBools(false);
            
            serializedObject.Update();

            var currLayoutType = CalcViewLayout();
            var newLayoutType = (ViewLayoutType)EditorGUILayout.EnumPopup(new GUIContent("Content Layout Mode"), currLayoutType);
            if (currLayoutType != newLayoutType)
            {
                UpdateViewLayout(newLayoutType);
            }
            
            if ( newLayoutType == ViewLayoutType.DEFAULT )
            {
                GUIStyle myStyle = GUI.skin.GetStyle("HelpBox");
                myStyle.richText = true;
                EditorGUILayout.TextArea(s_DefaultModeNotice, myStyle);
                
                EditorGUILayout.Separator();
                EditorGUILayout.PropertyField(m_Horizontal);
                EditorGUILayout.PropertyField(m_Vertical);            }
            
            
            
            EditorGUILayout.PropertyField(m_Content);

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

            EditorGUILayout.PropertyField(m_ScrollSensitivity);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_Viewport);

            EditorGUILayout.PropertyField(m_HorizontalScrollbar);
            if (m_HorizontalScrollbar.objectReferenceValue && !m_HorizontalScrollbar.hasMultipleDifferentValues)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_HorizontalScrollbarVisibility, EditorGUIUtility.TrTextContent("Visibility"));

                if ((ScrollRect.ScrollbarVisibility)m_HorizontalScrollbarVisibility.enumValueIndex == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport
                    && !m_HorizontalScrollbarVisibility.hasMultipleDifferentValues)
                {
                    if (m_ViewportIsNotChild || m_HScrollbarIsNotChild)
                        EditorGUILayout.HelpBox(s_HError, MessageType.Error);
                    EditorGUILayout.PropertyField(m_HorizontalScrollbarSpacing, EditorGUIUtility.TrTextContent("Spacing"));
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(m_VerticalScrollbar);
            if (m_VerticalScrollbar.objectReferenceValue && !m_VerticalScrollbar.hasMultipleDifferentValues)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_VerticalScrollbarVisibility, EditorGUIUtility.TrTextContent("Visibility"));

                if ((ScrollRect.ScrollbarVisibility)m_VerticalScrollbarVisibility.enumValueIndex == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport
                    && !m_VerticalScrollbarVisibility.hasMultipleDifferentValues)
                {
                    if (m_ViewportIsNotChild || m_VScrollbarIsNotChild)
                        EditorGUILayout.HelpBox(s_VError, MessageType.Error);
                    EditorGUILayout.PropertyField(m_VerticalScrollbarSpacing, EditorGUIUtility.TrTextContent("Spacing"));
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_OnValueChanged);
            
            serializedObject.ApplyModifiedProperties();
        }
        
        void SetAnimBools(bool instant)
        {
            SetAnimBool(m_ShowElasticity, !m_MovementType.hasMultipleDifferentValues && m_MovementType.enumValueIndex == (int)ScrollRect.MovementType.Elastic, instant);
            SetAnimBool(m_ShowDecelerationRate, !m_Inertia.hasMultipleDifferentValues && m_Inertia.boolValue == true, instant);
        }

        void SetAnimBool(AnimBool a, bool value, bool instant)
        {
            if (instant)
                a.value = value;
            else
                a.target = value;
        }
        
        void UpdateViewLayout(ViewLayoutType layoutType)
        {
            var view = serializedObject?.targetObject as NDScrollView;
            if (view == null || view.content == null)
            {
                return;
            }
            
            var layoutGroup = view.content.GetComponent<HorizontalOrVerticalLayoutGroup>();
            var contentSizeFitter = view.content.GetComponent<ContentSizeFitter>();
            
            
            //根据布局模式检查是否需要修改layoutgroup组件，并刷新contentsizefitter的模式
            switch (layoutType)
            {
                case ViewLayoutType.DEFAULT:
                {
                    if (layoutGroup != null)
                    {
                        ReleaseComponent(layoutGroup);
                        ReleaseComponent(contentSizeFitter);
                        view.vertical = true;
                        view.horizontal = true;
                    }
        
                    break;
                }
                case ViewLayoutType.VERTICAL:
                {
                    if (!(layoutGroup is VerticalLayoutGroup))
                    {
                        ReleaseComponent(layoutGroup);
                        view.content.gameObject.AddComponent<VerticalLayoutGroup>();
                        if (contentSizeFitter == null)
                        {
                            contentSizeFitter = view.content.gameObject.AddComponent<ContentSizeFitter>();
                        }
                        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                        view.vertical = true;
                        view.horizontal = false;
                    }
        
                    break;
                }
                case ViewLayoutType.HORIZONTAL:
                {
                    if (!(layoutGroup is HorizontalLayoutGroup))
                    {
                        ReleaseComponent(layoutGroup);
                        view.content.gameObject.AddComponent<HorizontalLayoutGroup>();
                        if (contentSizeFitter == null)
                        {
                            contentSizeFitter = view.content.gameObject.AddComponent<ContentSizeFitter>();
                        }
                        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                        view.vertical = false;
                        view.horizontal = true;
                    }
        
                    break;
                }
            }
        }
        void ReleaseComponent(Object target)
        {
            if (target == null)
            {
                return;
            }
            DestroyImmediate(target);
        }
    }
}