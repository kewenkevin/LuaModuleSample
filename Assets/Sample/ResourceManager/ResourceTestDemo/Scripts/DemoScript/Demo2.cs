using ND.Managers.ResourceMgr.Framework.Resource;
using ND.Managers.ResourceMgr.Runtime;
using ND.ResourceMgr.RunTime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Demo2 : MonoBehaviour
{
    public string SceneName;
    public void LoadScene()
    {
        NFUResource.LoadSceneAsset(SceneName, new LoadSceneCallbacks((duration, data) =>
        {
            Debug.Log("LoadSceneSuccess");
            SceneLoadHelper.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
        }));
    }
    
    public void UnLoadScene()
    {
        NFUResource.UnloadSceneAsset(SceneName, new UnloadSceneCallbacks((assetName, data) =>
        {
            SceneLoadHelper.UnLoadSceneAsync(SceneName);
            Debug.Log("UnLoadSceneSuccess");
        }));
    }
}
