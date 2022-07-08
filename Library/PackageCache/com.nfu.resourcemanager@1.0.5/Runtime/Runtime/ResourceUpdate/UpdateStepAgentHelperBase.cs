using UnityEngine;
using UnityEngine.Events;

namespace ResourceMgr.Runtime.ResourceUpdate
{
    public abstract class UpdateStepAgentHelperBase : MonoBehaviour
    {
        protected UnityEvent<string,string> OnSuccessFinish;
        protected UnityEvent<string, string> OnFailedFinish;


        public void Begin()
        {
            OnBegin();
        }

        protected abstract void OnBegin();


    }
}