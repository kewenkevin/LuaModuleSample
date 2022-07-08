using System.Collections;
using ND.Managers.ResourceMgr.Runtime;
using UnityEngine;

public class GameLauncher : MonoBehaviour
{

    public string Variant = "none";
    public GameObject Mask;
    
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        NFUResource.Initialize();
        //TODO: 用于设置变体
        NFUResource.currentVariant = Variant;
        ResourceEntry.ResourceUpdate.OnUpdateSuccess.AddListener(UpdateFinish);
        NFUResource.HotUpdateStart();
    }

    // Update is called once per frame
    void UpdateFinish()
    {
        if (Mask != null)
        {
            Mask.SetActive(false);
        }
    }
}
