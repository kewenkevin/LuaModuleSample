using ND.UI.NDUI.Utils;
using ND.UI.NDUI;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace ND.UI.NDUI
{
    [CustomEditor(typeof(NDRawImage), true)]
    [CanEditMultipleObjects]
    /// <summary>
    /// Custom Editor for the YRawImage Component.
    /// Extend this class to write a custom editor for a component derived from YRawImage.
    /// </summary>
    public class NDRawImageEditor : RawImageEditor
    {
        SerializedProperty m_CullNoneSprite;

        SerializedProperty m_FlipVertical;
        SerializedProperty m_FlipHorizontal;
        SerializedProperty m_Texture;
        SerializedProperty m_UVRect;
        GUIContent m_UVRectContent;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            m_CullNoneSprite = serializedObject.FindProperty("m_CullNoneSprite");
            
            m_FlipHorizontal = serializedObject.FindProperty("m_flipVertical");
            m_FlipVertical = serializedObject.FindProperty("m_flipHorizontal");
            
            m_UVRectContent     = EditorGUIUtility.TrTextContent("UV Rect");

            m_Texture           = serializedObject.FindProperty("m_Texture");
            m_UVRect            = serializedObject.FindProperty("m_UVRect");
        }

        public override void OnInspectorGUI()
        {
            var isPrefabInstance = UIPrefabUtils.IsPrefabInstance(serializedObject.targetObject as MonoBehaviour);
            if(isPrefabInstance){
                
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
            
            
            EditorGUI.BeginDisabledGroup(isPrefabInstance);
                
            EditorGUILayout.PropertyField(m_Texture);
            AppearanceControlsGUI();

            EditorGUI.EndDisabledGroup();
                
            

            RaycastControlsGUI();
#if UNITY_2019_1_OR_NEWER
            MaskableControlsGUI();
#endif
            EditorGUILayout.PropertyField(m_UVRect, m_UVRectContent);
            SetShowNativeSize(false);
            NativeSizeButtonGUI();

            serializedObject.ApplyModifiedProperties();
        }
        
        void SetShowNativeSize(bool instant)
        {
            base.SetShowNativeSize(m_Texture.objectReferenceValue != null, instant);
        }
    }
}