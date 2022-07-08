using ND.Managers.ResourceMgr.Framework.Resource;
using ND.Managers.ResourceMgr.Runtime;
using UnityEngine;
using UnityEngine.Serialization;

namespace ResourceMgr.Runtime.ResourceUpdate
{
    
        

    public class UpdateAppVersion:UpdateStepAgentHelperBase
    {
        [FormerlySerializedAs("NextStepPass")]
        [Tooltip("更新成功后续步骤")]
        [SerializeField] 
        private UpdateStepAgentHelperBase nextStepPass = null;
        
        protected override void OnBegin()
        {
            ResourceEntry.Resource.UpdateVersionList(
                ResourceEntry.ResourceUpdate.GetData<VarInt>("VersionListLength"),
                ResourceEntry.ResourceUpdate.GetData<VarInt>("VersionListHashCode"),
                ResourceEntry.ResourceUpdate.GetData<VarInt>("VersionListZipLength"),
                ResourceEntry.ResourceUpdate.GetData<VarInt>("VersionListZipHashCode"),
                new UpdateVersionListCallbacks(OnUpdateVersionListSuccess, OnUpdateVersionListFailure ));
        }
     

        private void OnUpdateVersionListSuccess(string downloadPath, string downloadUri)
        {
            Log.Info("Update version list from '{0}' success.", downloadUri);
            nextStepPass.Begin();
        }

        private void OnUpdateVersionListFailure(string downloadUri, string errorMessage)
        {
            Log.Warning("Update version list from '{0}' failure, error message is '{1}'.", downloadUri, errorMessage);
            ResourceEntry.ResourceUpdate.OnResourceFailedFinish();
        }
        
    }
}