using UnityEditor;
using UnityEngine;

namespace ND.UI.NDUI.Utils
{
    public static class UIPrefabUtils
    {
        public static bool IsPrefabInstance(GameObject go)
        {
        
            if (go == null)
            {
                return false;
            }
            
            var nearest = PrefabUtility.GetNearestPrefabInstanceRoot(go);
            if (nearest == null)
            {
                return false;
            }
            
            var correspondingObject = PrefabUtility.GetCorrespondingObjectFromSource(nearest);
            var instanceHandle = PrefabUtility.GetPrefabInstanceHandle(nearest);
            return correspondingObject != null && instanceHandle != null;
        }

        public static bool IsPrefabInstance(MonoBehaviour mono) => IsPrefabInstance(mono?.gameObject);

        public static void OpenPrefab(MonoBehaviour mono) => OpenPrefab(mono?.gameObject);

        public static void OpenPrefab(GameObject go)
        {
            if (go == null) 
                return;
            
            var nearestPrefab = PrefabUtility.GetNearestPrefabInstanceRoot(go);
            if (nearestPrefab == null)
            {
                return;
            }

            var assetPrefab= PrefabUtility.GetCorrespondingObjectFromSource(nearestPrefab);
            if (assetPrefab != null)
            {
                AssetDatabase.OpenAsset(assetPrefab);
            }
        }
    }
}