using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ND.Managers.ResourceMgr.Editor;
using ND.Managers.ResourceMgr.Editor.ResourceTools;
using UnityEditor;

namespace DefaultNamespace
{
    public class ResourceSceneTools
    {

        [MenuItem(EditorUtilityx.AdvancedMenuPrefix + "场景标记", false, 51)]
        public static void MarkScene()
        {
            var allSceneAsset = Directory.GetFiles(EditorResourceSettings.AssetRootPath, "*.unity", System.IO.SearchOption.AllDirectories);
            
            for (int i = 0; i < allSceneAsset.Length; i++)
            {
                var path = allSceneAsset[i].Replace("\\", "/");
                allSceneAsset[i] = path;
            }

            EditorBuildSettingsScene[] settings = EditorBuildSettings.scenes.Where(o => !string.IsNullOrEmpty(o.path)).ToArray();
            List<EditorBuildSettingsScene> scenesSettings = new List<EditorBuildSettingsScene>(settings);

            for (int i = 0; i < scenesSettings.Count; i++)
            {
                if (EditorResourceSettings.IsAssetRootPath(scenesSettings[i].path))
                {
                    if (!allSceneAsset.Where(o => o.Equals(scenesSettings[i].path, StringComparison.InvariantCultureIgnoreCase)).Any())
                    {
                        scenesSettings.RemoveAt(i);
                        i--;
                    }
                }
            }

            for (int i = 0; i < allSceneAsset.Length; i++)
            {
                var path = allSceneAsset[i];
                bool founded = false;
                for (int j = 0; j < settings.Length; j++)
                {
                    if (string.Equals(path, settings[j].path, System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        founded = true;
                        break;
                    }
                }

                if (!founded)
                {
                    scenesSettings.Add(new EditorBuildSettingsScene(allSceneAsset[i], true));
                }
            }
            var newList = scenesSettings.ToArray();
            if (IsSceneListChanged(EditorBuildSettings.scenes, newList))
            {
                EditorBuildSettings.scenes = newList;
            }
        }

        [MenuItem(EditorUtilityx.AdvancedMenuPrefix + "场景反标记", false, 52)]
        public static void UnMarkScene()
        {

            List<EditorBuildSettingsScene> scenesSettings = new List<EditorBuildSettingsScene>();
            EditorBuildSettingsScene[] settings = EditorBuildSettings.scenes.Where(o => !string.IsNullOrEmpty(o.path)).ToArray();
            for (int i = 0; i < settings.Length; i++)
            {
                if (!EditorResourceSettings.IsAssetRootPath(settings[i].path))
                {
                    scenesSettings.Add(settings[i]);
                }
            }
            var newList = scenesSettings.ToArray();
            if (IsSceneListChanged(EditorBuildSettings.scenes, newList))
            {
                EditorBuildSettings.scenes = newList;
            }
        }

        static bool IsSceneListChanged(EditorBuildSettingsScene[] list1, EditorBuildSettingsScene[] list2)
        {
            if (list1.Length != list2.Length)
                return true;
            for (int i = 0; i < list1.Length; i++)
            {
                var scene1 = list1[i];
                var scene2 = list2[i];
                if (scene1.CompareTo(scene2) != 0)
                {
                    return true;
                }
            }
            return false;
        }

    }
}