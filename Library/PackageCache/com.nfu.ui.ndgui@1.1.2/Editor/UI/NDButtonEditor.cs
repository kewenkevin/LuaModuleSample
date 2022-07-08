using ND.UI.Core.TextEditor;
using ND.UI.NDUI.Utils;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace ND.UI.NDUI
{
    [CustomEditor(typeof(NDButton), true)]
    [CanEditMultipleObjects]
    public class NDButtonEditor : NDSelectableEditor
    {
        SerializedProperty m_OnClickProperty;
        SerializedProperty m_OnDoubleClickProperty;
        SerializedProperty m_OnLongPressProperty;
        SerializedProperty m_OnLongClickProperty;
        SerializedProperty m_ScaleTweenInfo;
        SerializedProperty m_DoubleTime;
        SerializedProperty m_LongClickTime;
        SerializedProperty m_LongPressTime;
        SerializedProperty m_LongIntervalTime;

        SerializedProperty m_EnableButtonStateTextColor;
        SerializedProperty m_ButtonStateTextColorStyles;
        SerializedProperty m_Text;
        
        NDButton btn;

        private AnimBool m_EnableButtonStateTextColorAnim;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            m_OnClickProperty = serializedObject.FindProperty("m_OnClick");
            m_OnDoubleClickProperty = serializedObject.FindProperty("m_OnDoubleClick");
            m_OnLongClickProperty = serializedObject.FindProperty("m_OnLongClick");
            m_OnLongPressProperty = serializedObject.FindProperty("m_OnLongPress");
            m_ScaleTweenInfo = serializedObject.FindProperty("m_ScaleTweenInfo");
            m_DoubleTime = serializedObject.FindProperty("m_DoubleTime");
            m_LongClickTime = serializedObject.FindProperty("m_LongClickTime");
            m_LongPressTime = serializedObject.FindProperty("m_LongPressTime");
            m_LongIntervalTime = serializedObject.FindProperty("m_LongIntervalTime");
            
            m_EnableButtonStateTextColor = serializedObject.FindProperty("m_EnableButtonStateTextColor");
            m_ButtonStateTextColorStyles = serializedObject.FindProperty("m_ButtonStateTextColorStyles");
            m_Text = serializedObject.FindProperty("m_Text");

            m_EnableButtonStateTextColorAnim = new AnimBool();
            
            btn = target as NDButton;
        }

        private AnimationCurve m_DownScaleCurve;
        private AnimationCurve m_UpScaleCurve;
        
        private bool m_ShowCallbacks;
        private readonly string[] BUTTON_STATE_NAMES = {"Normal", "Highlighted", "Pressed", "Disabled"};
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
                
            serializedObject.Update();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical("box");
            m_ScaleTweenInfo.FindPropertyRelative("shouldTween").boolValue = EditorGUILayout.Toggle("ScaleTween", m_ScaleTweenInfo.FindPropertyRelative("shouldTween").boolValue);
            if (m_ScaleTweenInfo.FindPropertyRelative("shouldTween").boolValue)
            {
                m_ScaleTweenInfo.FindPropertyRelative("downScaleCurve").animationCurveValue = EditorGUILayout.CurveField("DownScaleCurve", m_ScaleTweenInfo.FindPropertyRelative("downScaleCurve").animationCurveValue);
                m_ScaleTweenInfo.FindPropertyRelative("UpScaleCurve").animationCurveValue = EditorGUILayout.CurveField("UpScaleCurve", m_ScaleTweenInfo.FindPropertyRelative("UpScaleCurve").animationCurveValue);
            }
            else
            {
                if (m_DownScaleCurve == null)
                {
                    m_DownScaleCurve = new AnimationCurve();
                    m_DownScaleCurve.AddKey(0f, 1);
                    m_DownScaleCurve.AddKey(0.2f, 0.8f);
                }
                m_ScaleTweenInfo.FindPropertyRelative("downScaleCurve").animationCurveValue = m_DownScaleCurve;
                if (m_UpScaleCurve == null)
                { 
                    m_UpScaleCurve = new AnimationCurve();
                    m_UpScaleCurve.AddKey(0f, 0.8f);
                    m_UpScaleCurve.AddKey(0.2f, 1);
                }
                m_ScaleTweenInfo.FindPropertyRelative("UpScaleCurve").animationCurveValue = m_UpScaleCurve;
            }
            
            EditorGUILayout.EndVertical();

            
            var isPrefabInstance = UIPrefabUtils.IsPrefabInstance(serializedObject.targetObject as MonoBehaviour);

            EditorGUI.BeginDisabledGroup(isPrefabInstance);
            
            EditorGUI.BeginChangeCheck();

            var isEnabled = m_EnableButtonStateTextColor.boolValue = EditorGUILayout.ToggleLeft("开启文字样式设置", m_EnableButtonStateTextColor.boolValue);

            m_EnableButtonStateTextColorAnim.target = isEnabled;
            
            if (EditorGUI.EndChangeCheck() )
            {
                if(isEnabled)
                {
                    var childText = btn.GetComponentInChildren<NDText>();;
                    m_Text.objectReferenceValue = childText;
                    InitColorArr(m_ButtonStateTextColorStyles, 4);
                }
                else
                {
                    m_Text.objectReferenceValue = null;
                    m_ButtonStateTextColorStyles.ClearArray();
                }
                serializedObject.ApplyModifiedProperties();
            }
            
            if (EditorGUILayout.BeginFadeGroup(m_EnableButtonStateTextColorAnim.faded))
            {
                ++EditorGUI.indentLevel;
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Button Text", GUILayout.Width(100));
                    EditorGUILayout.ObjectField(m_Text, typeof(NDText), new GUIContent(string.Empty));
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.LabelField("Each Button State Text Color");
                    
                    ++EditorGUI.indentLevel;
                    {
                        var len = m_ButtonStateTextColorStyles.arraySize;
                        for (var i = 0; i < len; i++)
                        {
                            var styleProp = m_ButtonStateTextColorStyles.GetArrayElementAtIndex(i);

                            var stateName = BUTTON_STATE_NAMES[i];
                
                            EditorGUILayout.BeginHorizontal(GUI.skin.box);
                
                            //EditorGUILayout.LabelField(stateName, GUILayout.Width(100));
                
                            TextEditorUtility.DrawTextColorStyleField(new GUIContent(stateName), styleProp);
                
                            EditorGUILayout.EndHorizontal();
                
                        }
                        
                    }
                    --EditorGUI.indentLevel;
                }
                --EditorGUI.indentLevel;
            }
            EditorGUILayout.EndFadeGroup();
            
            EditorGUI.EndDisabledGroup();
            
            
            EditorGUILayout.Space();
            
            
            var foldoutStyle = new GUIStyle("Foldout");
            foldoutStyle.fontStyle = FontStyle.Bold;
            m_ShowCallbacks = EditorGUILayout.Foldout(m_ShowCallbacks, "Callbacks", foldoutStyle);
            ++EditorGUI.indentLevel;
            if(m_ShowCallbacks)
            {
                EditorGUILayout.Space();
                //EditorGUILayout.LabelField("CallBack", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(m_OnClickProperty);
                EditorGUILayout.Space();
                m_DoubleTime.floatValue = EditorGUILayout.FloatField("Double Click Time(s)", Mathf.Clamp(m_DoubleTime.floatValue, 0.1f, 10f));
                bool shouldDouble = (btn.onDoubleClick.GetPersistentEventCount() > 0);
                EditorGUILayout.PropertyField(m_OnDoubleClickProperty);
                EditorGUILayout.Space();
                m_LongClickTime.floatValue = EditorGUILayout.Slider("Long Click Time(s)", m_LongClickTime.floatValue, 0.2f, 10f);
                bool shouldLongClick = (btn.onLongClick.GetPersistentEventCount() > 0);
                if (shouldDouble && shouldLongClick)
                {
                    if (m_LongClickTime.floatValue < m_DoubleTime.floatValue)
                    {
                        EditorGUILayout.HelpBox("长点击的触发时间应大于双击间隔", MessageType.Warning);
                    }
                }
                EditorGUILayout.PropertyField(m_OnLongClickProperty);
                EditorGUILayout.Space();
                m_LongPressTime.floatValue = EditorGUILayout.Slider("Long Press Time(s)", m_LongPressTime.floatValue, 0.2f, 10f);
                m_LongIntervalTime.floatValue = EditorGUILayout.Slider("Interval Time(s)", m_LongIntervalTime.floatValue, 0.1f, 10);
                bool shouldPress = (btn.onLongPress.GetPersistentEventCount() > 0);
                if (shouldDouble && shouldPress)
                {
                    if (m_LongPressTime.floatValue < m_DoubleTime.floatValue)
                    {
                        EditorGUILayout.HelpBox("长按的触发时间应大于双击间隔", MessageType.Warning);
                    }
                }
                EditorGUILayout.PropertyField(m_OnLongPressProperty);
                EditorGUILayout.EndVertical();
            }
            --EditorGUI.indentLevel;
            
            
            serializedObject.ApplyModifiedProperties();
        }

        private int InitColorArr(SerializedProperty prop, int size)
        {
            if (prop.isArray)
            {
                prop.ClearArray();
                for (var i = 0; i < size; i++)
                {
                    prop.InsertArrayElementAtIndex(i);
                }
            }

            return size;
        }
    }
}
