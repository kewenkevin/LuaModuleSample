using ND.UI.NDUI.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;


namespace ND.UI.NDUI
{
    [CustomEditor(typeof(NDImage), true)]
    [CanEditMultipleObjects]
    /// <summary>
    /// Custom Editor for the YImage Component.
    /// Extend this class to write a custom editor for a component derived from YImage.
    /// </summary>
    public class NDImageEditor : ImageEditor
    {
        private SerializedProperty m_CullNoneSprite;
        private SerializedProperty m_FlipVertical;
        private SerializedProperty m_FlipHorizontal;
        
        private SerializedProperty m_Sprite;
        private SerializedProperty m_Type;
        private SerializedProperty m_PreserveAspect;
        private SerializedProperty m_UseSpriteMesh;
        
        private AnimBool m_ShowType2;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            m_CullNoneSprite = serializedObject.FindProperty("m_CullNoneSprite");
            m_FlipHorizontal = serializedObject.FindProperty("m_flipVertical");
            m_FlipVertical = serializedObject.FindProperty("m_flipHorizontal");
            
            m_Sprite = serializedObject.FindProperty("m_Sprite");
            
            m_Type = serializedObject.FindProperty("m_Type");
            
            m_PreserveAspect = serializedObject.FindProperty("m_PreserveAspect");
            m_UseSpriteMesh = serializedObject.FindProperty("m_UseSpriteMesh");
            
            m_ShowType2 = new AnimBool(m_Sprite.objectReferenceValue != null);
        }
        
        
        public override void OnInspectorGUI()
        {
            var isPrefabInstance = UIPrefabUtils.IsPrefabInstance(serializedObject.targetObject as MonoBehaviour);
            if(isPrefabInstance) {
                
                EditorGUILayout.HelpBox("由于UI编辑规范限制，请打开Prefab后再编辑贴图与材质的相关属性", MessageType.Info, true);
                if (GUILayout.Button("打开Prefab"))
                {
                    UIPrefabUtils.OpenPrefab(serializedObject.targetObject as MonoBehaviour);
                }
                
            }
            
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_CullNoneSprite);
            EditorGUILayout.PropertyField(m_FlipHorizontal);
            EditorGUILayout.PropertyField(m_FlipVertical);
            serializedObject.ApplyModifiedProperties();
            
            serializedObject.Update();

            
            EditorGUI.BeginDisabledGroup(isPrefabInstance);
                
            SpriteGUI();
            AppearanceControlsGUI();
                
            EditorGUI.EndDisabledGroup();
            
            
            RaycastControlsGUI();
            
            #if UNITY_2019_1_OR_NEWER
            MaskableControlsGUI();
            #endif
            
            m_ShowType2.target = m_Sprite.objectReferenceValue != null;
            if (EditorGUILayout.BeginFadeGroup(m_ShowType2.faded))
            {
                TypeGUI();
            }
            EditorGUILayout.EndFadeGroup();

            SetShowNativeSize(false);
            if (EditorGUILayout.BeginFadeGroup(m_ShowNativeSize.faded))
            {
                EditorGUI.indentLevel++;

                if ((Image.Type) m_Type.enumValueIndex == Image.Type.Simple)
                {
                    EditorGUILayout.PropertyField(m_UseSpriteMesh);
                }

                EditorGUILayout.PropertyField(m_PreserveAspect);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();
            NativeSizeButtonGUI();

            serializedObject.ApplyModifiedProperties();
        }
        
        void SetShowNativeSize(bool instant)
        {
            Image.Type type = (Image.Type)m_Type.enumValueIndex;
            bool showNativeSize = (type == Image.Type.Simple || type == Image.Type.Filled) && m_Sprite.objectReferenceValue != null;
            base.SetShowNativeSize(showNativeSize, instant);
        }
    }
}