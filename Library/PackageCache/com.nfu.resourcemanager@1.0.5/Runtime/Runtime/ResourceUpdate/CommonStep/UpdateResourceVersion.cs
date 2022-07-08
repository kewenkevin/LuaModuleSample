using ND.Managers.ResourceMgr.Runtime;
using UnityEngine;

namespace ResourceMgr.Runtime.ResourceUpdate
{
    public class UpdateResourceVersion:UpdateStepAgentHelperBase
    {
        protected ResourceUpdateAgent agent;
        [Tooltip("更新成功后续步骤")]
        [SerializeField] 
        private UpdateStepAgentHelperBase nextStepSuccess = null;
        
        /// <summary>
        /// 当前要更新的所有资源总大小
        /// </summary>
        public long UpdateResourceTotalZipLength => agent ? agent.UpdateResourceTotalZipLength : 0;
        
        /// <summary>
        /// 当前已更新的所有资源总大小
        /// </summary>
        public long CurrentTotalUpdateLength=> agent ? agent.CurrentTotalUpdateLength : 0;
        
        /// <summary>
        /// 需要更新的所有资源包总数
        /// </summary>
        public int UpdateResourceCount=> agent ? agent.UpdateResourceCount : 0;
        
        /// <summary>
        /// 已经成功更新的所有资源包总数
        /// </summary>
        public float UpdateSuccessCount=> agent ? agent.UpdateSuccessCount : 0;
        
        /// <summary>
        /// 当前的更新完成进度
        /// </summary>
        public float ProgressTotal=> agent ? agent.ProgressTotal : 0;
        
        /// <summary>
        /// 当前更新速度
        /// </summary>
        public float CurrentSpeed=> agent ? agent.CurrentSpeed : 0;
        
        
        

        
        protected override void OnBegin()
        {
            var go = new GameObject("ResourceUpdateAgent");
            go.transform.SetParent(transform);
            agent = go.AddComponent<ResourceUpdateAgent>();
            agent.UpdateResourceTotalZipLength = ResourceEntry.ResourceUpdate.GetData<VarLong>("UpdateResourceTotalZipLength");
            agent.UpdateResourceCount =  ResourceEntry.ResourceUpdate.GetData<VarInt>("UpdateResourceCount");
            agent.OnFinish += OnFinish; 
            agent.OnProgress += OnProgressChanged;
        }

        protected virtual void OnProgressChanged()
        {
            
        }
        
        private void OnFinish()
        {
            agent.OnFinish -= OnFinish;
            Destroy(agent.gameObject);
            if (nextStepSuccess != null) 
                nextStepSuccess.Begin();
        }
    };
}