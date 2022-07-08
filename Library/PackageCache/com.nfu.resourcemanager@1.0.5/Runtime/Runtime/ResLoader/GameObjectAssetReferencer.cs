using System;
using UnityEngine;

namespace ND.Managers.ResourceMgr.Runtime
{
    public class GameObjectAssetReferencer : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        private UnityEngine.Object refrenceAsset = null;

        [HideInInspector]
        [SerializeField]
        private string refrenceAssetName = null;
        public void RefAsset(string assetName,UnityEngine.Object asset)
        {
            refrenceAsset = asset;
            refrenceAssetName = assetName;
        }

        private void Awake()
        {
            //如果实例化已经有了，则再加载一次让计数自增1
            if (!string.IsNullOrEmpty(refrenceAssetName) && refrenceAsset!=null)
            {
                NFUResource.LoadAssetAsync<GameObject>(refrenceAssetName, LoadAssetSuccessCallback);
            }
        }

        private void LoadAssetSuccessCallback(string assetname, GameObject asset, float duration, object userdata)
        {
            if (refrenceAsset == null)
                refrenceAsset = asset as GameObject;
        }


        private void OnDestroy()
        {
            if (refrenceAsset!=null)
            {
                NFUResource.Release(refrenceAsset);
                refrenceAsset = null;
            }
        }
    }
}