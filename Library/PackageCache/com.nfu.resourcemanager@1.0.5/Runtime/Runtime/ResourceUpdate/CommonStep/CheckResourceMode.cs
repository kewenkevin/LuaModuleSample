using ND.Managers.ResourceMgr.Framework.Resource;
using ND.Managers.ResourceMgr.Runtime;
using UnityEngine;

namespace ResourceMgr.Runtime.ResourceUpdate
{
    public class CheckResourceMode:UpdateStepAgentHelperBase
    {
        [Tooltip("如果热更新则执行")]
        [SerializeField] 
        private UpdateStepAgentHelperBase hotUpdateModeNextStep = null;
        
        protected override void OnBegin()
        {
            if (ResourceEntry.Base.EditorResourceMode)
            {
                // 编辑器模式
                Log.Info("Editor resource mode detected.");
                ResourceEntry.ResourceUpdate.OnResourceSuccessFinish();
            }
            else if (ResourceEntry.Resource.ResourceMode == ResourceMode.Package)
            {
                // 单机模式
                Log.Info("Package resource mode detected.");
                ResourceEntry.Resource.InitResources(ResourceEntry.ResourceUpdate.OnResourceSuccessFinish);
            }
            else
            {
                // 可更新模式
                Log.Info("Updatable resource mode detected.");
                hotUpdateModeNextStep?.Begin();
            }
        }
    }
}
