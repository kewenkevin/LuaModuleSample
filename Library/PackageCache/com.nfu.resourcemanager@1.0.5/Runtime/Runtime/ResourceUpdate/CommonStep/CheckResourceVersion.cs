using ND.Managers.ResourceMgr.Runtime;
using UnityEngine;

namespace ResourceMgr.Runtime.ResourceUpdate
{
    public class CheckResourceVersion : UpdateStepAgentHelperBase
    {
        [Tooltip("通过检测下一步")]
        [SerializeField] 
        private UpdateStepAgentHelperBase nextStepPass = null;
        
        [Tooltip("未能通过检测下一步")]
        [SerializeField] 
        private UpdateStepAgentHelperBase nextStepNotPass = null;
        
        protected override void OnBegin()
        {
            Log.Info("Check Resource Version");
            ResourceEntry.Resource.CheckResources(OnCheckResourcesComplete);
        }
        
        public void OnCheckResourcesComplete(int movedCount, int removedCount, int updateCount, long updateTotalLength, long updateTotalZipLength)
        {
            Log.Info("Check resources complete, '{0}' resources need to update, zip length is '{1}', unzip length is '{2}'.", updateCount.ToString(), updateTotalZipLength.ToString(), updateTotalLength.ToString());
            if (updateCount > 0)
            {
                ResourceEntry.ResourceUpdate.SetData<VarLong>("UpdateResourceTotalZipLength",updateTotalZipLength);
                ResourceEntry.ResourceUpdate.SetData<VarInt>("UpdateResourceCount",updateCount);
                nextStepNotPass.Begin();
            }
            else
            {
                nextStepPass.Begin();
            }
        }
    }
}