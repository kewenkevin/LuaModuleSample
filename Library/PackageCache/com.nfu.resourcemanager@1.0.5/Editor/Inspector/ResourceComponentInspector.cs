//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using ND.Managers.ResourceMgr.Framework;
using ND.Managers.ResourceMgr.Framework.ObjectPool;
using ND.Managers.ResourceMgr.Framework.Resource;
using ND.Managers.ResourceMgr.Runtime;
using static ND.Managers.ResourceMgr.Framework.Resource.ResourceManager.ResourceLoader;

namespace ND.Managers.ResourceMgr.Editor
{
    [CustomEditor(typeof(ResourceComponent))]
    internal sealed class ResourceComponentInspector : GameFrameworkInspector
    {
        private static readonly string[] ResourceModeNames = new string[] { "Package", "Updatable", "Updatable While Playing" };
        private static readonly string[] ResourceCheckModeNames = new string[] { "Ignore Version", "Normal" };

        private SerializedProperty m_ResourceMode = null;
        private SerializedProperty m_ReadWritePathType = null;
        private SerializedProperty m_MinUnloadUnusedAssetsInterval = null;
        private SerializedProperty m_MaxUnloadUnusedAssetsInterval = null;
        private SerializedProperty m_AssetAutoReleaseInterval = null;
        private SerializedProperty m_AssetCapacity = null;
        private SerializedProperty m_AssetExpireTime = null;
        private SerializedProperty m_AssetPriority = null;
        private SerializedProperty m_ResourceAutoReleaseInterval = null;
        private SerializedProperty m_ResourceCapacity = null;
        private SerializedProperty m_ResourceExpireTime = null;
        private SerializedProperty m_ResourcePriority = null;
        private SerializedProperty m_UpdatePrefixUri = null;
        private SerializedProperty m_GenerateReadWriteVersionListLength = null;
        private SerializedProperty m_UpdateRetryCount = null;
        private SerializedProperty m_InstanceRoot = null;
        private SerializedProperty m_LoadResourceAgentHelperCount = null;
        private SerializedProperty m_GUISpriteAtlasPrefixedUrl = null;
        private SerializedProperty m_UpdateCheckMode = null;

        private FieldInfo m_EditorResourceModeFieldInfo = null;

        private int m_ResourceModeIndex = 0;
        private int m_UpdateCheckModeIndex = 0;

        private HelperInfo<ResourceHelperBase> m_ResourceHelperInfo = new HelperInfo<ResourceHelperBase>("Resource");
        private HelperInfo<LoadResourceAgentHelperBase> m_LoadResourceAgentHelperInfo = new HelperInfo<LoadResourceAgentHelperBase>("LoadResourceAgent");
        private string m_ResFilter;
        [SerializeField]
        private string m_AssetFilter;
        private static int displayMaxCount = 30;
        private static Color LoadedColor = Color.white;
        private static Color WaitingColor = new Color(0.5f, 0.5f, 0.5f, 1);
        private static Color LoadingColor = new Color(0.45f, 0.9f, 0.45f, 1);
        private static Color DelayCreateColor = new Color(0.3f, 0.7f, 0.9f, 1f);
        private static Color ExpireColor = new Color(0.9f, 0.3f, 0.3f, 1);

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            ResourceComponent t = (ResourceComponent)target;

            bool isEditorResourceMode = (bool)m_EditorResourceModeFieldInfo.GetValue(target);

            if (isEditorResourceMode)
            {
                EditorGUILayout.HelpBox("Editor resource mode is enabled. Some options are disabled.", MessageType.Warning);
            }

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
                {
                    EditorGUILayout.EnumPopup("Resource Mode", t.ResourceMode);
                }
                else
                {
                    int selectedIndex = EditorGUILayout.Popup("Resource Mode", m_ResourceModeIndex, ResourceModeNames);
                    if (selectedIndex != m_ResourceModeIndex)
                    {
                        m_ResourceModeIndex = selectedIndex;
                        m_ResourceMode.enumValueIndex = selectedIndex + 1;
                    }
                }

                m_ReadWritePathType.enumValueIndex = (int)(ReadWritePathType)EditorGUILayout.EnumPopup("Read Write Path Type", t.ReadWritePathType);
            }
            EditorGUI.EndDisabledGroup();


            var dragArea = GUILayoutUtility.GetRect(0f, 35f, GUILayout.ExpandWidth(true));
            string atlasPath = !string.IsNullOrEmpty(ResourceSettings.AtlasPath) ? ResourceSettings.AtlasPath : m_GUISpriteAtlasPrefixedUrl.stringValue;
            GUIContent title = new GUIContent("这是spriteatlas图集的根目录,拖拽以修改:\n" + atlasPath);
            if (string.IsNullOrEmpty(atlasPath))
            {
                title = new GUIContent("这是spriteatlas图集的根目录,拖拽以修改");
            }

            GUI.Box(dragArea, title);
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    break;
                case EventType.DragPerform:
                    if (!dragArea.Contains(Event.current.mousePosition))
                    {
                        break;
                    }

                    if (DragAndDrop.paths != null)
                    {
                        if (DragAndDrop.paths.Length > 0)
                        {
                            var path = DragAndDrop.paths[0];
                            // Debug.Log(DragAndDrop.paths[i]);//输出拖入的文件或文件夹路径
                            if (!Directory.Exists(path))
                            {
                                path = Path.GetDirectoryName(DragAndDrop.paths[0]);
                            }

                            m_GUISpriteAtlasPrefixedUrl.stringValue = path.Replace("\\", "/");
                            ResourceSettings.AtlasPath = m_GUISpriteAtlasPrefixedUrl.stringValue;

                        }
                    }

                    Event.current.Use();
                    break;
                default:
                    break;
            }

            float minUnloadUnusedAssetsInterval = EditorGUILayout.Slider("Min Unload Unused Assets Interval", m_MinUnloadUnusedAssetsInterval.floatValue, 0f, 3600f);
            if (minUnloadUnusedAssetsInterval != m_MinUnloadUnusedAssetsInterval.floatValue)
            {
                if (EditorApplication.isPlaying)
                {
                    t.MinUnloadUnusedAssetsInterval = minUnloadUnusedAssetsInterval;
                }
                else
                {
                    m_MinUnloadUnusedAssetsInterval.floatValue = minUnloadUnusedAssetsInterval;
                }
            }

            float maxUnloadUnusedAssetsInterval = EditorGUILayout.Slider("Max Unload Unused Assets Interval", m_MaxUnloadUnusedAssetsInterval.floatValue, 0f, 3600f);
            if (maxUnloadUnusedAssetsInterval != m_MaxUnloadUnusedAssetsInterval.floatValue)
            {
                if (EditorApplication.isPlaying)
                {
                    t.MaxUnloadUnusedAssetsInterval = maxUnloadUnusedAssetsInterval;
                }
                else
                {
                    m_MaxUnloadUnusedAssetsInterval.floatValue = maxUnloadUnusedAssetsInterval;
                }
            }


            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying && isEditorResourceMode);
            {
                float assetAutoReleaseInterval = EditorGUILayout.DelayedFloatField("Asset Auto Release Interval", m_AssetAutoReleaseInterval.floatValue);
                if (assetAutoReleaseInterval != m_AssetAutoReleaseInterval.floatValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.AssetAutoReleaseInterval = assetAutoReleaseInterval;
                    }
                    else
                    {
                        m_AssetAutoReleaseInterval.floatValue = assetAutoReleaseInterval;
                    }
                }

                int assetCapacity = EditorGUILayout.DelayedIntField("Asset Capacity", m_AssetCapacity.intValue);
                if (assetCapacity != m_AssetCapacity.intValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.AssetCapacity = assetCapacity;
                    }
                    else
                    {
                        m_AssetCapacity.intValue = assetCapacity;
                    }
                }

                float assetExpireTime = EditorGUILayout.DelayedFloatField("Asset Expire Time", m_AssetExpireTime.floatValue);
                if (assetExpireTime != m_AssetExpireTime.floatValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.AssetExpireTime = assetExpireTime;
                    }
                    else
                    {
                        m_AssetExpireTime.floatValue = assetExpireTime;
                    }
                }

                int assetPriority = EditorGUILayout.DelayedIntField("Asset Priority", m_AssetPriority.intValue);
                if (assetPriority != m_AssetPriority.intValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.AssetPriority = assetPriority;
                    }
                    else
                    {
                        m_AssetPriority.intValue = assetPriority;
                    }
                }

                float resourceAutoReleaseInterval = EditorGUILayout.DelayedFloatField("Resource Auto Release Interval", m_ResourceAutoReleaseInterval.floatValue);
                if (resourceAutoReleaseInterval != m_ResourceAutoReleaseInterval.floatValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.ResourceAutoReleaseInterval = resourceAutoReleaseInterval;
                    }
                    else
                    {
                        m_ResourceAutoReleaseInterval.floatValue = resourceAutoReleaseInterval;
                    }
                }

                int resourceCapacity = EditorGUILayout.DelayedIntField("Resource Capacity", m_ResourceCapacity.intValue);
                if (resourceCapacity != m_ResourceCapacity.intValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.ResourceCapacity = resourceCapacity;
                    }
                    else
                    {
                        m_ResourceCapacity.intValue = resourceCapacity;
                    }
                }

                float resourceExpireTime = EditorGUILayout.DelayedFloatField("Resource Expire Time", m_ResourceExpireTime.floatValue);
                if (resourceExpireTime != m_ResourceExpireTime.floatValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.ResourceExpireTime = resourceExpireTime;
                    }
                    else
                    {
                        m_ResourceExpireTime.floatValue = resourceExpireTime;
                    }
                }

                int resourcePriority = EditorGUILayout.DelayedIntField("Resource Priority", m_ResourcePriority.intValue);
                if (resourcePriority != m_ResourcePriority.intValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.ResourcePriority = resourcePriority;
                    }
                    else
                    {
                        m_ResourcePriority.intValue = resourcePriority;
                    }
                }

                if (m_ResourceModeIndex > 0)
                {
                    string updatePrefixUri = EditorGUILayout.DelayedTextField("Update Prefix Uri", m_UpdatePrefixUri.stringValue);
                    if (updatePrefixUri != m_UpdatePrefixUri.stringValue)
                    {
                        if (EditorApplication.isPlaying)
                        {
                            t.UpdatePrefixUri = updatePrefixUri;
                        }
                        else
                        {
                            m_UpdatePrefixUri.stringValue = updatePrefixUri;
                        }
                    }

                    int generateReadWriteVersionListLength = EditorGUILayout.DelayedIntField("Generate Read Write Version List Length", m_GenerateReadWriteVersionListLength.intValue);
                    if (generateReadWriteVersionListLength != m_GenerateReadWriteVersionListLength.intValue)
                    {
                        if (EditorApplication.isPlaying)
                        {
                            t.GenerateReadWriteVersionListLength = generateReadWriteVersionListLength;
                        }
                        else
                        {
                            m_GenerateReadWriteVersionListLength.intValue = generateReadWriteVersionListLength;
                        }
                    }

                    int updateRetryCount = EditorGUILayout.DelayedIntField("Update Retry Count", m_UpdateRetryCount.intValue);
                    if (updateRetryCount != m_UpdateRetryCount.intValue)
                    {
                        if (EditorApplication.isPlaying)
                        {
                            t.UpdateRetryCount = updateRetryCount;
                        }
                        else
                        {
                            m_UpdateRetryCount.intValue = updateRetryCount;
                        }
                    }

                    if (!EditorApplication.isPlaying)
                    {
                        int selectedIndex = EditorGUILayout.Popup("Resource Update Check Mode", m_UpdateCheckModeIndex,
                            ResourceCheckModeNames);
                        if (selectedIndex != m_UpdateCheckModeIndex)
                        {
                            m_UpdateCheckModeIndex = selectedIndex;
                            m_UpdateCheckMode.enumValueIndex = selectedIndex;
                        }
                    }
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                EditorGUILayout.PropertyField(m_InstanceRoot);

                m_ResourceHelperInfo.Draw();
                m_LoadResourceAgentHelperInfo.Draw();
                m_LoadResourceAgentHelperCount.intValue = EditorGUILayout.IntSlider("Load Resource Agent Helper Count", m_LoadResourceAgentHelperCount.intValue, 1, 10000);
            }
            EditorGUI.EndDisabledGroup();




            if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("Unload Unused Assets", Utility.Text.Format("{0} / {1}", t.LastUnloadUnusedAssetsOperationElapseSeconds.ToString("F2"), t.MaxUnloadUnusedAssetsInterval.ToString("F2")));
                EditorGUILayout.LabelField("Read Only Path", t.ReadOnlyPath.ToString());
                EditorGUILayout.LabelField("Read Write Path", t.ReadWritePath.ToString());
                EditorGUILayout.LabelField("Current Variant", t.CurrentVariant ?? "<Unknwon>");
                EditorGUILayout.LabelField("Applicable Game Version", isEditorResourceMode ? "N/A" : t.ApplicableGameVersion ?? "<Unknwon>");
                EditorGUILayout.LabelField("Internal Resource Version", isEditorResourceMode ? "N/A" : t.InternalResourceVersion.ToString());
                EditorGUILayout.LabelField("Asset Count", isEditorResourceMode ? "N/A" : t.AssetCount.ToString());
                EditorGUILayout.LabelField("Resource Count", isEditorResourceMode ? "N/A" : t.ResourceCount.ToString());
                EditorGUILayout.LabelField("Resource Group Count", isEditorResourceMode ? "N/A" : t.ResourceGroupCount.ToString());
                if (m_ResourceModeIndex > 0)
                {
                    EditorGUILayout.LabelField("Applying Resource Pack Path", isEditorResourceMode ? "N/A" : t.ApplyingResourcePackPath ?? "<Unknwon>");
                    EditorGUILayout.LabelField("Apply Waiting Count", isEditorResourceMode ? "N/A" : t.ApplyWaitingCount.ToString());
                    EditorGUILayout.LabelField("Updating Resource Group", isEditorResourceMode ? "N/A" : t.UpdatingResourceGroup != null ? t.UpdatingResourceGroup.Name : "<Unknwon>");
                    EditorGUILayout.LabelField("Update Waiting Count", isEditorResourceMode ? "N/A" : t.UpdateWaitingCount.ToString());
                    EditorGUILayout.LabelField("Update Candidate Count", isEditorResourceMode ? "N/A" : t.UpdateCandidateCount.ToString());
                    EditorGUILayout.LabelField("Updating Count", isEditorResourceMode ? "N/A" : t.UpdatingCount.ToString());
                }


                GUILayout.Label($"Load Asset: {Utility.TotalAssetObject,-5} Task: {Utility.TotalAssetTask,-5} Main: {Utility.TotalAssetTask - Utility.TotalDependencyAssetTask,-5} Depend: {Utility.TotalDependencyAssetTask,-5} Frame: {Utility.TotalAssetTaskUsedFrame}");
                GUILayout.Label($"Load Resource: {Utility.TotalResourceObject,-5} ");


                EditorGUILayout.LabelField("Load Total Agent Count", isEditorResourceMode ? "N/A" : t.LoadTotalAgentCount.ToString());
                EditorGUILayout.LabelField("Load Free Agent Count", isEditorResourceMode ? "N/A" : t.LoadFreeAgentCount.ToString());
                EditorGUILayout.LabelField("Load Working Agent Count", isEditorResourceMode ? "N/A" : t.LoadWorkingAgentCount.ToString());
                EditorGUILayout.LabelField("Load Waiting Task Count", isEditorResourceMode ? "N/A" : t.LoadWaitingTaskCount.ToString());


                if (!isEditorResourceMode)
                {
                    EditorGUILayout.BeginFoldoutHeaderGroup(true, "Collect");
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUI.indentLevel++;
                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PrefixLabel($"Resources ({collectResources.Count}) Assets ({collectAssets.Count})");


                        if (GUILayout.Button("Clear", EditorStyles.toolbarButton))
                        {
                            collectResources.Clear();
                            collectAssets.Clear();
                        }
                        if (GUILayout.Button("Log", EditorStyles.toolbarButton))
                        {
                            Debug.Log(CollectToString());
                        }

                        if (GUILayout.Button("Save", EditorStyles.toolbarButton))
                        {
                            string file = EditorUtility.SaveFilePanel("Save Collect Resources", "", $"CollectResources-{DateTime.Now:yyyyMMddHHmmss}", ".txt");
                            if (!string.IsNullOrEmpty(file))
                            {
                                File.WriteAllText(file, CollectToString(), Encoding.UTF8);
                            }
                        }

                    }

                    using (new GUILayout.HorizontalScope())
                    {
                        var enabled = (ResourceSettings.Options & ResourceSettings.ResourceOptions.AutoGCStackTrace) != 0;

                        var oldGUIEnabled = GUI.enabled;
                        GUI.enabled = oldGUIEnabled & enabled;

                        EditorGUILayout.PrefixLabel(new GUIContent($"ResLoader GC ({Utility.CollectAutoGCs.Count})", "Enable settings 'Auto GC Stack Trace'"));


                        if (GUILayout.Button("Clear", EditorStyles.toolbarButton))
                        {
                            Utility.CollectAutoGCs.Clear();
                        }
                        if (GUILayout.Button("Log", EditorStyles.toolbarButton))
                        {
                            Debug.Log(AutoGCToString());
                        }

                        if (GUILayout.Button("Save", EditorStyles.toolbarButton))
                        {
                            string file = EditorUtility.SaveFilePanel("Save ResLoader GC", "", $"ResLoader-GC-{DateTime.Now:yyyyMMddHHmmss}", ".txt");
                            if (!string.IsNullOrEmpty(file))
                            {
                                File.WriteAllText(file, AutoGCToString(), Encoding.UTF8);
                            }
                        }
                        GUI.enabled = oldGUIEnabled;
                    }
                    EditorGUI.indentLevel--;


                    GUI.color = Color.white;
                    using (new GUILayout.HorizontalScope())
                    {
                        m_AssetFilter = EditorGUILayout.TextField(m_AssetFilter, EditorStyles.toolbarSearchField);
                        GUILayout.Label("Max", GUILayout.ExpandWidth(false));
                        displayMaxCount = EditorGUILayout.IntField(displayMaxCount, GUILayout.ExpandWidth(false), GUILayout.Width(30));
                    }

                    int count = 0;


                    Func<KeyValuePair<string, GameFrameworkLinkedListRange<ObjectPoolManager.Object<ResourceManager.ResourceLoader.ResourceObject>>>, ResourceManager.ResourceLoader.ResourceObject> getResource = (o) =>
                    {
                        if (o.Value.First != null)
                        {
                            var poolObject = o.Value.First.Value;
                            return poolObject.Target;
                        }
                        return null;
                    };

                    var allResources = (from o in t.ResourceManager.ResourcePool.GetAll.ToList()
                                        where Filter(o.Key)
                                        orderby o.Key
                                        select o).ToList();
                    int spawnZeroTotal = 0;
                    int loadingTotal = 0;
                    int waitingTotal = 0;
                    foreach (var VARIABLE in allResources)
                    {
                        if (VARIABLE.Value.First != null)
                        {
                            var poolObject = VARIABLE.Value.First.Value;

                            if (poolObject != null)
                            {
                                var resourceObject = poolObject.Target;
                                if (poolObject.SpawnCount <= 0)
                                    spawnZeroTotal++;
                                if (resourceObject.IsLoading)
                                {
                                    loadingTotal++;
                                }
                                if (!resourceObject.IsLoading && !resourceObject.IsResourceLoaded)
                                {
                                    waitingTotal++;
                                }
                            }
                        }
                    }

                    var foldoutStyle = new GUIStyle(EditorStyles.foldout);
                    foldoutStyle.richText = true;

                    m_foldRes = EditorGUILayout.Foldout(m_foldRes, $"Resources {ColorText(LoadedColor, "{0,-4}", allResources.Count)} {ColorText(WaitingColor, "waiting {0,-4}", waitingTotal)} {ColorText(LoadingColor, "loading {0,-4}", loadingTotal)} {ColorText(ExpireColor, "expire {0,-4}", spawnZeroTotal)}", true, foldoutStyle);

                    int spawnCount = 0;
                    int beDepCount = 0;


                    if (m_foldRes)
                    {
                        EditorGUILayout.BeginVertical("box");
                        EditorGUILayout.BeginHorizontal();

                        if (GUILayout.Button("Log", EditorStyles.toolbarButton))
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            ResourcesToString(allResources, stringBuilder);
                            Debug.Log(stringBuilder.ToString());
                        }

                        if (GUILayout.Button("Save", EditorStyles.toolbarButton))
                        {
                            string file = EditorUtility.SaveFilePanel("Save Resources", "", $"Resources-{DateTime.Now:yyyyMMddHHmmss}", ".txt");
                            if (!string.IsNullOrEmpty(file))
                            {
                                StringBuilder stringBuilder = new StringBuilder();
                                ResourcesToString(allResources, stringBuilder);
                                File.WriteAllText(file, stringBuilder.ToString(), Encoding.UTF8);
                            }
                        }

                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                        count = 0;

                        foreach (var VARIABLE in allResources)
                        {
                            if (count >= displayMaxCount)
                            {
                                GUILayout.Label("...");
                                break;
                            }
                            spawnCount = 0;
                            beDepCount = 0;
                            ResourceManager.ResourceLoader.ResourceObject resourceObject = null;
                            if (VARIABLE.Value.First != null)
                            {
                                var poolObject = VARIABLE.Value.First.Value;
                                spawnCount = poolObject.SpawnCount;
                                resourceObject = poolObject.Target;
                            }

                            if (resourceObject != null)
                            {
                                if (resourceObject.IsLoading)
                                    GUI.color = LoadingColor;
                                else if (resourceObject.IsDelayCreate)
                                    GUI.color = DelayCreateColor;
                                else if (resourceObject.IsResourceLoaded)
                                    GUI.color = LoadedColor;
                                else
                                    GUI.color = WaitingColor;

                                beDepCount = resourceObject.BeDependencyResources.Count;
                            }

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(VARIABLE.Key);
                            EditorGUILayout.LabelField(beDepCount.ToString(), GUILayout.Width(25));
                            EditorGUILayout.LabelField(spawnCount.ToString(), GUILayout.Width(25));
                            EditorGUILayout.EndHorizontal();

                            GUI.color = Color.white;

                            count++;
                        }

                        EditorGUILayout.EndVertical();
                    }


                    Func<KeyValuePair<string, GameFrameworkLinkedListRange<ObjectPoolManager.Object<ResourceManager.ResourceLoader.AssetObject>>>, ResourceManager.ResourceLoader.AssetObject> getAsset = (o) =>
                    {
                        if (o.Value.First != null)
                        {
                            var poolObject = o.Value.First.Value;
                            return poolObject.Target;
                        }
                        return null;
                    };
                    var allAssets = (from o in t.ResourceManager.AssetPool.GetAll.ToList()
                                     where Filter(o.Key)
                                     orderby getAsset(o)?.IsDelayCreate, o.Key
                                     select o).ToList();

                    int delayCount = 0;
                    spawnZeroTotal = 0;
                    loadingTotal = 0;
                    waitingTotal = 0;

                    foreach (var VARIABLE in allAssets)
                    {
                        if (VARIABLE.Value.First != null)
                        {
                            var poolObject = VARIABLE.Value.First.Value;

                            if (poolObject != null)
                            {
                                if (poolObject.SpawnCount <= 0)
                                    spawnZeroTotal++;
                                var asset = poolObject.Target;
                                if (asset.IsDelayCreate)
                                    delayCount++;
                                if (asset.IsLoading)
                                    loadingTotal++;
                                if (!asset.IsLoading && !asset.IsLoaded)
                                    waitingTotal++;
                            }
                        }
                    }


                    m_foldAsset = EditorGUILayout.Foldout(m_foldAsset, $"Assets {ColorText(LoadedColor, "{0,-4}", allAssets.Count)} {ColorText(WaitingColor, "waiting {0,-4}", waitingTotal)} {ColorText(LoadingColor, "loading {0,-4}", loadingTotal)} {ColorText(DelayCreateColor, "delay {0,-4}", delayCount)} {ColorText(ExpireColor, "expire {0,-4}", spawnZeroTotal)}", true, foldoutStyle);


                    if (m_foldAsset)
                    {
                        ResourceManager.ResourceLoader.AssetObject assetObject;

                        EditorGUILayout.BeginVertical("box");
                        spawnCount = 0;
                        EditorGUILayout.BeginHorizontal();


                        if (GUILayout.Button("Log", EditorStyles.toolbarButton))
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            AssetsToString(allAssets, stringBuilder);
                            Debug.Log(stringBuilder.ToString());
                        }

                        if (GUILayout.Button("Save", EditorStyles.toolbarButton))
                        {
                            string file = EditorUtility.SaveFilePanel("Save Assets", "", $"Assets-{DateTime.Now:yyyyMMddHHmmss}", ".txt");
                            if (!string.IsNullOrEmpty(file))
                            {
                                StringBuilder stringBuilder = new StringBuilder();
                                AssetsToString(allAssets, stringBuilder);
                                File.WriteAllText(file, stringBuilder.ToString(), Encoding.UTF8);
                            }
                        }

                        if (GUILayout.Button("Save All", EditorStyles.toolbarButton))
                        {
                            string file = EditorUtility.SaveFilePanel("Save All", "", $"Resources-All-{DateTime.Now:yyyyMMddHHmmss}", ".txt");
                            if (!string.IsNullOrEmpty(file))
                            {
                                StringBuilder stringBuilder = new StringBuilder();
                                ResourcesToString(allResources, stringBuilder);
                                AssetsToString(allAssets, stringBuilder);
                                File.WriteAllText(file, stringBuilder.ToString(), Encoding.UTF8);
                            }
                        }

                        GUILayout.FlexibleSpace();

                        EditorGUILayout.EndHorizontal();
                        count = 0;

                        foreach (var VARIABLE in allAssets)
                        {
                            assetObject = null;

                            if (count >= displayMaxCount)
                            {
                                GUILayout.Label("...");
                                break;
                            }
                            spawnCount = 0;
                            if (VARIABLE.Value.First != null)
                            {
                                var poolObject = VARIABLE.Value.First.Value;
                                spawnCount = poolObject.SpawnCount;
                                assetObject = poolObject.Target;
                            }
                            if (assetObject != null)
                            {
                                if (assetObject.IsLoading)
                                    GUI.color = LoadingColor;
                                else if (assetObject.IsDelayCreate)
                                    GUI.color = DelayCreateColor;
                                else if (assetObject.IsLoaded)
                                    GUI.color = LoadedColor;
                                else
                                    GUI.color = WaitingColor;
                            }
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(VARIABLE.Key);

                            EditorGUILayout.LabelField(spawnCount.ToString(), GUILayout.Width(25));
                            EditorGUILayout.EndHorizontal();
                            GUI.color = Color.white;
                            count++;
                        }

                        EditorGUILayout.EndVertical();
                    }


                    EditorGUILayout.LabelField($"Load Waiting Task ({(isEditorResourceMode ? 0 : t.LoadWaitingTaskCount)})");
                    EditorGUILayout.BeginVertical("box");
                    {
                        TaskInfo[] loadAssetInfos = t.GetAllLoadAssetInfos();
                        if (loadAssetInfos.Length > 0)
                        {
                            foreach (TaskInfo loadAssetInfo in loadAssetInfos)
                            {
                                DrawLoadAssetInfo(loadAssetInfo);
                            }

                            if (GUILayout.Button("Export CSV Data"))
                            {
                                string exportFileName = EditorUtility.SaveFilePanel("Export CSV Data", string.Empty, "Load Asset Task Data.csv", string.Empty);
                                if (!string.IsNullOrEmpty(exportFileName))
                                {
                                    try
                                    {
                                        int index = 0;
                                        string[] data = new string[loadAssetInfos.Length + 1];
                                        data[index++] = "Load Asset Name,Serial Id,Priority,Status";
                                        foreach (TaskInfo loadAssetInfo in loadAssetInfos)
                                        {
                                            data[index++] = Utility.Text.Format("{0},{1},{2},{3}", loadAssetInfo.Description, loadAssetInfo.SerialId.ToString(), loadAssetInfo.Priority.ToString(), loadAssetInfo.Status.ToString());
                                        }

                                        File.WriteAllLines(exportFileName, data, Encoding.UTF8);
                                        Debug.Log(Utility.Text.Format("Export load asset task CSV data to '{0}' success.", exportFileName));
                                    }
                                    catch (Exception exception)
                                    {
                                        Debug.LogError(Utility.Text.Format("Export load asset task CSV data to '{0}' failure, exception is '{1}'.", exportFileName, exception.ToString()));
                                    }
                                }
                            }
                        }

                    }
                    EditorGUILayout.EndVertical();

                }
            }

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }

        private bool m_foldRes = true;
        private bool m_foldAsset = true;

        protected override void OnCompileComplete()
        {
            base.OnCompileComplete();

            RefreshTypeNames();
        }

        private void OnEnable()
        {
            m_ResourceMode = serializedObject.FindProperty("m_ResourceMode");
            m_ReadWritePathType = serializedObject.FindProperty("m_ReadWritePathType");
            m_MinUnloadUnusedAssetsInterval = serializedObject.FindProperty("m_MinUnloadUnusedAssetsInterval");
            m_MaxUnloadUnusedAssetsInterval = serializedObject.FindProperty("m_MaxUnloadUnusedAssetsInterval");
            m_AssetAutoReleaseInterval = serializedObject.FindProperty("m_AssetAutoReleaseInterval");
            m_AssetCapacity = serializedObject.FindProperty("m_AssetCapacity");
            m_AssetExpireTime = serializedObject.FindProperty("m_AssetExpireTime");
            m_AssetPriority = serializedObject.FindProperty("m_AssetPriority");
            m_ResourceAutoReleaseInterval = serializedObject.FindProperty("m_ResourceAutoReleaseInterval");
            m_ResourceCapacity = serializedObject.FindProperty("m_ResourceCapacity");
            m_ResourceExpireTime = serializedObject.FindProperty("m_ResourceExpireTime");
            m_ResourcePriority = serializedObject.FindProperty("m_ResourcePriority");
            m_UpdatePrefixUri = serializedObject.FindProperty("m_UpdatePrefixUri");
            m_GenerateReadWriteVersionListLength = serializedObject.FindProperty("m_GenerateReadWriteVersionListLength");
            m_UpdateRetryCount = serializedObject.FindProperty("m_UpdateRetryCount");
            m_InstanceRoot = serializedObject.FindProperty("m_InstanceRoot");
            m_LoadResourceAgentHelperCount = serializedObject.FindProperty("m_LoadResourceAgentHelperCount");
            m_GUISpriteAtlasPrefixedUrl = serializedObject.FindProperty("m_GUISpriteAtlasPrefixedUrl");

            m_UpdateCheckMode = serializedObject.FindProperty("m_UpdateCheckMode");
            m_UpdateCheckModeIndex = m_UpdateCheckMode.enumValueIndex;

            m_EditorResourceModeFieldInfo = target.GetType().GetField("m_EditorResourceMode", BindingFlags.NonPublic | BindingFlags.Instance);

            m_ResourceHelperInfo.Init(serializedObject);
            m_LoadResourceAgentHelperInfo.Init(serializedObject);

            RefreshModes();
            RefreshTypeNames();
        }

        private void DrawLoadAssetInfo(TaskInfo loadAssetInfo)
        {
            EditorGUILayout.LabelField(loadAssetInfo.Description, Utility.Text.Format("[SerialId]{0} [Priority]{1} [Status]{2}", loadAssetInfo.SerialId.ToString(), loadAssetInfo.Priority.ToString(), loadAssetInfo.Status.ToString()));
        }

        private void RefreshModes()
        {
            m_ResourceModeIndex = m_ResourceMode.enumValueIndex > 0 ? m_ResourceMode.enumValueIndex - 1 : 0;
        }

        private void RefreshTypeNames()
        {
            m_ResourceHelperInfo.Refresh();
            m_LoadResourceAgentHelperInfo.Refresh();
            serializedObject.ApplyModifiedProperties();
        }
        bool Filter(string name)
        {
            if (!string.IsNullOrEmpty(m_AssetFilter) && name.IndexOf(m_AssetFilter, StringComparison.InvariantCultureIgnoreCase) == -1)
                return false;
            return true;
        }
        string ColorText(Color color, string text, params object[] args)
        {
            return string.Format("<color=#{0:X}{1:X}{2:X}{3:X}>{4}</color>",
                (int)(color.r * 255),
                (int)(color.g * 255),
                (int)(color.b * 255),
                (int)(color.a * 255),
                string.Format(text, args));
        }

        string GetResourceStateString(ResourceManager.ResourceLoader.ResourceObject resourceObject)
        {
            if (resourceObject == null)
                return "null";
            string state = "";
            if (resourceObject.IsLoading)
                state = "Loading";
            else if (resourceObject.IsDelayCreate)
                state = "Delay";
            else if (resourceObject.IsResourceLoaded)
                state = "Loaded";
            else
                state = "Waiting";
            return state;
        }

        void ResourcesToString(IEnumerable<KeyValuePair<string, GameFrameworkLinkedListRange<ObjectPoolManager.Object<ResourceManager.ResourceLoader.ResourceObject>>>> resources, StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine($"Resources ({resources.Count()})");


            foreach (var pair in resources)
            {
                string resourceName = pair.Key;
                int beDepCount = 0;
                int spawnCount = 0;

                ResourceManager.ResourceLoader.ResourceObject resourceObject = null;

                if (pair.Value.First != null)
                {
                    var poolObject = pair.Value.First.Value;
                    spawnCount = poolObject.SpawnCount;
                    resourceObject = poolObject.Target;
                }

                if (resourceObject != null)
                {
                    beDepCount = resourceObject.BeDependencyResources.Count;
                }

                stringBuilder.AppendLine($"{resourceName} ({GetResourceStateString(resourceObject)}, {beDepCount}, {spawnCount})");

                if (resourceObject != null)
                {
                    foreach (var dependency in resourceObject.DependencyResources)
                    {
                        stringBuilder.Append("    ").AppendLine($"{dependency.Name} ({GetResourceStateString(dependency)})");
                    }
                }

            }

        }
        void AssetsToString(IEnumerable<KeyValuePair<string, GameFrameworkLinkedListRange<ObjectPoolManager.Object<ResourceManager.ResourceLoader.AssetObject>>>> assets, StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine($"Assets ({assets.Count()})");

            foreach (var pair in assets)
            {
                int spawnCount = 0;
                ResourceManager.ResourceLoader.AssetObject assetObject = null;
                if (pair.Value.First != null)
                {
                    var poolObject = pair.Value.First.Value;
                    spawnCount = poolObject.SpawnCount;
                    assetObject = poolObject.Target;
                }
                string state = "";
                if (assetObject != null)
                {
                    if (assetObject.IsLoading)
                        state = "Loaiding";
                    else if (assetObject.IsDelayCreate)
                        state = "Delay";
                    else if (assetObject.IsLoaded)
                        state = "Loaded";
                    else
                        state = "Waiting";
                }
                stringBuilder.AppendLine($"{pair.Key} ({state}, {spawnCount})");
            }
        }


        string CollectToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"Collect Resources ({collectResources.Count})");
            foreach (var item in collectResources.OrderBy(o => o))
            {
                stringBuilder.AppendLine(item);
            }
            stringBuilder.AppendLine();

            stringBuilder.AppendLine($"Collect Assets ({collectAssets.Count})");
            foreach (var item in collectAssets.OrderBy(o => o))
            {
                stringBuilder.AppendLine(item);
            }
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }


        string AutoGCToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"Collect ResLoader GC ({Utility.CollectAutoGCs.Count})");
            string path = Path.GetFullPath(".") + Path.DirectorySeparatorChar;
            foreach (var item in Utility.CollectAutoGCs)
            {
                string str = item.Replace(path, "");
                stringBuilder.AppendLine(str)
                    .AppendLine();
            }
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

    }
}
