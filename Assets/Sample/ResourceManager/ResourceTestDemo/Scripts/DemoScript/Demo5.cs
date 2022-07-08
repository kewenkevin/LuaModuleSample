using System.Collections;
using System.Collections.Generic;
using ND.Managers.ResourceMgr.Runtime;
using UnityEngine;

public class Demo5 : MonoBehaviour
{
   public string AssetNamne = "";
   public void LoadAsset()
   {
      NFUResource.LoadAssetAsync<GameObject>(AssetNamne, (assetName, asset, duration, data) =>
      {
         Debug.Log("sucess");
         var obj = asset ;
         Instantiate(obj);
         
      } ,((assetName, status, message, data) =>
      {
         Debug.Log("fault");
      }));
      
   }
}
