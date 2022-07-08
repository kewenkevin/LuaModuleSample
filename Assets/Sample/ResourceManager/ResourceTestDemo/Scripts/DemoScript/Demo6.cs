using System.IO;
using ND.Managers.ResourceMgr.Framework.Resource;
using ND.Managers.ResourceMgr.Runtime;
using UnityEngine;

public class Demo6 : MonoBehaviour
{
    public string prefabPath = "Assets/ResourcesAssets/Prefabs/TestModel.prefab";

    private void Awake()
    {
        if (ResourceUserSettings.ResourceMode != ResourceMode.UpdatableWhilePlaying)
        {
            Debug.LogWarning("自动将编辑器下资源模式设置为UpdatableWhilePlaying，以开启边下边玩。");
            ResourceUserSettings.ResourceMode = ResourceMode.UpdatableWhilePlaying;
        }
    }


    void Start()
    {
        NFUResource.Initialize();
    }

    private bool isHotUpdateStart = false;
    void OnGUI()
    {
        if (!isHotUpdateStart && !NFUResource.Initializated && GUILayout.Button("HotUpdateStart"))
        {
            isHotUpdateStart = true;
            var path = Path.Combine(Directory.GetCurrentDirectory(), "output/BuildReport/versionInfo.json");
            if(!File.Exists(path))
            {
                Debug.LogWarning($"本地热更新测试之前，请先Build并生成versionInfo.json文件:{path}");
            }
            else
            {
                ClearLocalResources();
                NFUResource.HotUpdateStart();
            }
        }

        if (NFUResource.Initializated && GUILayout.Button("Load Prefab（只下载所需资源）"))
        {
            NFUResource.LoadAssetAsync<GameObject>(prefabPath, (assetName, asset, duration, data) =>
            {
                var obj = asset;
                if (obj != null)
                {
                    Instantiate(obj);
                }
            }, (assetName, status, message, data) =>
            {
                Debug.Log("Loading asset " + assetName + "error:"+message);
            });
        }

        if (NFUResource.Initializated && GUILayout.Button("Update All Resource"))
        {
            NFUResource.Resource.UpdateResources((group, result) =>
            {
                if(result)
                    Debug.Log("Update All Resource Successfully!!");
                else
                    Debug.LogWarning("Update All Resource Default!!");
            });
        }
    }
    
    static void ClearLocalResources()
    {
        string localPath = Application.persistentDataPath;

        foreach (var fileOrDirectory in new string[] { 
            "assets",
            "GameFrameworkList.dat",
            "GameFrameworkVersion.dat"})
        {
            string path = Path.Combine(localPath, fileOrDirectory);
            
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            else if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        Debug.LogWarning("清空本地资源，开始热更新");
    }
}
