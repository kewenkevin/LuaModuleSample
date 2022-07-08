//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using ND.Managers.ResourceMgr.Runtime;
using UnityEditor;

namespace ND.Managers.ResourceMgr.Editor
{
    [CustomEditor(typeof(EditorResourceComponent))]
    internal sealed class EditorResourceComponentInspector : GameFrameworkInspector
    {
        private SerializedProperty m_EnableCachedAssets = null;
        private SerializedProperty m_LoadAssetCountPerFrame = null;
        private SerializedProperty m_MinLoadAssetRandomDelaySeconds = null;
        private SerializedProperty m_MaxLoadAssetRandomDelaySeconds = null;
        
        private SerializedProperty m_MinLoadSceneRandomDelaySeconds = null;
        private SerializedProperty m_MaxLoadSceneRandomDelaySeconds = null;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorResourceComponent t = (EditorResourceComponent)target;

            if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("Load Waiting Asset Count", t.LoadWaitingAssetCount.ToString());
            }

            EditorGUILayout.PropertyField(m_EnableCachedAssets);
            EditorGUILayout.PropertyField(m_LoadAssetCountPerFrame);
            EditorGUILayout.PropertyField(m_MinLoadAssetRandomDelaySeconds);
            EditorGUILayout.PropertyField(m_MaxLoadAssetRandomDelaySeconds);
            
            EditorGUILayout.PropertyField(m_MinLoadSceneRandomDelaySeconds);
            EditorGUILayout.PropertyField(m_MaxLoadSceneRandomDelaySeconds);

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }

        private void OnEnable()
        {
            m_EnableCachedAssets = serializedObject.FindProperty("m_EnableCachedAssets");
            m_LoadAssetCountPerFrame = serializedObject.FindProperty("m_LoadAssetCountPerFrame");
            m_MinLoadAssetRandomDelaySeconds = serializedObject.FindProperty("m_MinLoadAssetRandomDelaySeconds");
            m_MaxLoadAssetRandomDelaySeconds = serializedObject.FindProperty("m_MaxLoadAssetRandomDelaySeconds");
            
            m_MinLoadSceneRandomDelaySeconds = serializedObject.FindProperty("m_MinLoadSceneRandomDelaySeconds");
            m_MaxLoadSceneRandomDelaySeconds = serializedObject.FindProperty("m_MaxLoadSceneRandomDelaySeconds");
        }
    }
}
