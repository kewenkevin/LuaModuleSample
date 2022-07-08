using UnityEngine;
using UnityEditor;

namespace ND.UI.NDUI
{
    [CustomEditor(typeof(NDRichText), true)]
    [CanEditMultipleObjects]
    public class NDRichTextEditor : NDTextEditor
    {
        NDRichText m_ndRichText;
        SerializedProperty m_InputText;
        SerializedProperty m_Dirty;
        SerializedProperty m_LineAlign;
        SerializedProperty m_OnClickProperty;
        FontData2Editor drawer;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_ndRichText = target as NDRichText;
            m_InputText = serializedObject.FindProperty("m_RichText");
            m_Dirty = serializedObject.FindProperty("m_Dirty");
            m_LineAlign = serializedObject.FindProperty("m_LineAlignment");
            m_OnClickProperty = serializedObject.FindProperty("m_OnClick");
            drawer = new FontData2Editor(m_LineAlign);
            m_ndRichText.alignByGeometry = true;
        }

        public override void DrawTextArea()
        {
            EditorGUILayout.PropertyField(m_InputText, new GUIContent("InputText"));
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(m_Text, new GUIContent("OutText"));
            EditorGUI.EndDisabledGroup();
        }

        public override void DrawFont()
        {
            float h = drawer.GetPropertyHeight(m_FontData, new GUIContent(""));
            Rect rect = EditorGUILayout.GetControlRect(false, h);
            drawer.OnGUI(rect, m_FontData, new GUIContent(""), m_Style.objectReferenceValue == null);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (m_Dirty.boolValue)
            {
                m_ndRichText.DrawTag();
            }
            if (!m_ndRichText.alignByGeometry)
            {
                m_ndRichText.alignByGeometry = true;
            }
            if (!m_ndRichText.supportRichText)
            {
                m_ndRichText.supportRichText = true;
            }
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_OnClickProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }


    public class FontData2Editor: FontDataEditor
    {
        public SerializedProperty lineAlignment;
        private float m_LineAlignmentHeight = 0f;
        public FontData2Editor(SerializedProperty lineAlignment)
        {
            this.lineAlignment = lineAlignment;
            m_LineAlignmentHeight = EditorGUI.GetPropertyHeight(lineAlignment);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = base.GetPropertyHeight(property, label);
            return height + m_LineAlignmentHeight - m_ResizeTextForBestFitHeight - m_EncodingHeight;
        }

        public override void OnDrawAlignByGeometry(Rect rect)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(rect, m_AlignByGeometry);
            EditorGUI.EndDisabledGroup();
        }

        static int s_LineAlignmentHash = "DoLineAligmentControl".GetHashCode();

        public override void OnDrawOtherAligmentControl(ref Rect rect)
        {
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            rect.height = m_LineAlignmentHeight;

            GUIContent alingmentContent = EditorGUIUtility.TrTextContent("Line Alignment");
            int id = EditorGUIUtility.GetControlID(s_LineAlignmentHash, FocusType.Keyboard, rect);

            EditorGUIUtility.SetIconSize(new Vector2(15, 15));
            EditorGUI.BeginProperty(rect, alingmentContent, lineAlignment);
            {
                Rect controlArea = EditorGUI.PrefixLabel(rect, id, alingmentContent);

                float width = kAlignmentButtonWidth * 3;
                float spacing = Mathf.Clamp(controlArea.width - width * 2, 2, 10);

                Rect horizontalAligment = new Rect(controlArea.x, controlArea.y, width, controlArea.height);

                DoLineAligmentControl(horizontalAligment, lineAlignment);
            }
            EditorGUI.EndProperty();
            EditorGUIUtility.SetIconSize(Vector2.zero);
        }

        public override void OnDrawBestFit(ref Rect rect) 
        {
            
        }

        public override void OnDrawRichText(ref Rect rect)
        {
            
        }

        private void DoLineAligmentControl(Rect position, SerializedProperty alignment) 
        {
            TextAnchor ta = (TextAnchor)alignment.intValue;
            NDRichText.LineAlignment lineAlign = (NDRichText.LineAlignment)ta;

            bool topAlign = (lineAlign == NDRichText.LineAlignment.Up);
            bool middleAlign = (lineAlign == NDRichText.LineAlignment.Mid);
            bool bottomAlign = (lineAlign == NDRichText.LineAlignment.Down);


            position.width = kAlignmentButtonWidth;

            EditorGUI.BeginChangeCheck();
            EditorToggle(position, topAlign, topAlign ? Styles.m_TopAlignTextActive : Styles.m_TopAlignText, Styles.alignmentButtonLeft);
            if (EditorGUI.EndChangeCheck())
            {
                SetLineAlignment(alignment, NDRichText.LineAlignment.Up);
            }

            position.x += position.width;
            EditorGUI.BeginChangeCheck();
            EditorToggle(position, middleAlign, middleAlign ? Styles.m_MiddleAlignTextActive : Styles.m_MiddleAlignText, Styles.alignmentButtonMid);
            if (EditorGUI.EndChangeCheck())
            {
                SetLineAlignment(alignment, NDRichText.LineAlignment.Mid);
            }

            position.x += position.width;
            EditorGUI.BeginChangeCheck();
            EditorToggle(position, bottomAlign, bottomAlign ? Styles.m_BottomAlignTextActive : Styles.m_BottomAlignText, Styles.alignmentButtonRight);
            if (EditorGUI.EndChangeCheck())
            {
                SetLineAlignment(alignment, NDRichText.LineAlignment.Down);
            }
        }

        private void SetLineAlignment(SerializedProperty alignment, NDRichText.LineAlignment lineAlignment) 
        {
            foreach (var obj in alignment.serializedObject.targetObjects)
            {
                NDRichText text = obj as NDRichText;
                NDRichText.LineAlignment currentAligment = text.lineAlignment;
                Undo.RecordObject(text, "Line Alignment");
                text.lineAlignment = lineAlignment;
                EditorUtility.SetDirty(obj);
            }
        }
    }
}