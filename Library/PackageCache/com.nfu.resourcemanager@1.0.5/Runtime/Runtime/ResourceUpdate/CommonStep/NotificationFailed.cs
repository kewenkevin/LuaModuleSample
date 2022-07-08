using ND.Managers.ResourceMgr.Runtime;

namespace ResourceMgr.Runtime.ResourceUpdate
{
    public class NotificationFailed:UpdateStepAgentHelperBase
    {
        protected override void OnBegin()
        {
            ResourceEntry.ResourceUpdate.OnResourceFailedFinish();
        }
    }
}