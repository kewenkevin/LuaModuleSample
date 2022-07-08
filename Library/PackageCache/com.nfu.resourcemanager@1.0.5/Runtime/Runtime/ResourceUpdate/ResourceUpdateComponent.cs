using System.Collections.Generic;
using ND.Managers.ResourceMgr.Framework;
using ND.Managers.ResourceMgr.Runtime;
using UnityEngine;
using UnityEngine.Events;
using ND.Managers.ResourceMgr.Framework.Resource;

namespace ResourceMgr.Runtime.ResourceUpdate
{
    [DisallowMultipleComponent]
    [AddComponentMenu("ResourceMgr/ResourceUpdate")]
    public sealed class ResourceUpdateComponent : GameFrameworkComponent
    {
        [SerializeField]
        private UpdateStepAgentHelperBase firstStep = null;

        private Dictionary<string, Variable> datas = new Dictionary<string, Variable>();

        /// <summary>
        /// 当成功更新后的事件
        /// </summary>
        public UnityEvent OnUpdateSuccess;

        /// <summary>
        /// 更新过程出现问题的事件
        /// </summary>
        public UnityEvent OnUpdateFailed;


        /// <summary>
        /// 开始更新
        /// </summary>
        public void Begin()
        {
            if (firstStep != null)
                firstStep.Begin();
            else
                Log.Error("You must set first step before start it!");
        }



        public void OnResourceSuccessFinish()
        {
            Log.Info("Variant: " + ResourceEntry.Resource.CurrentVariant);
            ResourceEntry.Resource.ResourceManager.Initializated = true;
            Debug.Log($"Resource Initalize completed, resource version: {ResourceEntry.Resource.ResourceManager.InternalResourceVersion}");
            OnUpdateSuccess?.Invoke();
        }

        public void OnResourceFailedFinish()
        {
            OnUpdateFailed?.Invoke();
        }

        public bool HasData(string name)
        {
            return datas.ContainsKey(name);
        }

        public TData GetData<TData>(string name) where TData : Variable
        {
            if (HasData(name))
                return datas[name] as TData;
            else
                return default(TData);
        }


        public Variable GetData(string name)
        {
            if (HasData(name))
                return datas[name];
            else
                return default(Variable);
        }


        public void SetData<TData>(string name, TData data) where TData : Variable
        {
            SetData(name, data as Variable);
        }

        public void SetData(string name, Variable data)
        {
            if (HasData(name))
                datas[name] = data;
            else
                datas.Add(name, data);
        }
    }
}