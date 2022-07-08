using ND.Managers.ResourceMgr.Runtime;

namespace ResourceMgr.Runtime.ResourceUpdate
{
    public class NotificationSuccess:UpdateStepAgentHelperBase
    {
        protected override void OnBegin()
        {
            ResourceEntry.ResourceUpdate.OnResourceSuccessFinish();
        }
    }
}