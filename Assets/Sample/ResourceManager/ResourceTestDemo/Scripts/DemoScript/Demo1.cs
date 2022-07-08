using System.Collections;
using System.Collections.Generic;
using ND.Gameplay.Managers.ResourceManagerV2.Editor;
using ND.Managers.ResourceMgr.Framework.Resource;
using ND.Managers.ResourceMgr.Runtime;
using ResourceMgr.Runtime.ResourceUpdate;
using UnityEngine;

public class Demo1 : MonoBehaviour
{
    public string prefabPath;

    public GameObject LoadObject;
    
    public GameObject LoadObject2;

    public CheckAppVersion cav;

    public ResLoader Load;

    void Awake()
    {
        cav.GetAppVersionInfo = ParseAppVersionInfo;
    }
    
    public void LoadAsset()
    {
        NFUResource.LoadAssetAsync<GameObject>(prefabPath, (assetName, asset, duration, data) =>
        {
            var obj = asset;
            if (obj != null)
            {
                LoadObject = Instantiate(obj);
            }
        }, (assetName, status, message, data) =>
        {
            Debug.Log("Loading asset " + assetName + "error");
        });
    }
    
    public void UnLoadAsset()
    {
        NFUResource.Release(prefabPath, typeof(GameObject));
    }
    
    
    public void LoadGameObject()
    {
        NFUResource.InstantiateAsync(prefabPath, (assetName, asset, duration, data) =>
        {
            LoadObject2 = asset as GameObject;
            
        }, (assetName, status, message, data) =>
        {
            Debug.Log("Loading asset " + assetName + "error");
        });
    }
    
    public void DestroyGameObject()
    {
        Destroy(LoadObject2);
    }


    public void LoadAssetsByResLoad()
    {
        if(Load == null)
            Load = ResLoader.Alloc();
        Load.Add2Load(prefabPath, (success, assetName, asset) =>
        {
            var obj = asset as GameObject;
            if (obj != null)
            {
                LoadObject = Instantiate(obj);
            }
        });
        Load.Load();
    }
    
    public void ReleaseResLoad()
    {
        Load.Recycle2Cache();
    }
    
    protected VersionInfo ParseAppVersionInfo(string responseContent)
    {
        VersionInfoSeriazable vInfoSer = JsonUtility.FromJson<VersionInfoSeriazable>(responseContent);
        VersionInfo info = vInfoSer;
        return info;
    }
}
