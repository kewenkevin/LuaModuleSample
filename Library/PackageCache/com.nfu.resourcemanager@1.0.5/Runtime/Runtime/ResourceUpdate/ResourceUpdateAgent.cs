using System;
using System.Collections.Generic;
using ND.Managers.ResourceMgr.Runtime;
using UnityEngine;

namespace ResourceMgr.Runtime.ResourceUpdate
{
    public class ResourceUpdateAgent : MonoBehaviour
    {
        private bool m_UpdateResourcesComplete = false;
        private int m_UpdateCount = 0;
        private long m_UpdateTotalZipLength = 0L;
        private int m_UpdateSuccessCount = 0;
        private List<UpdateLengthData> m_UpdateLengthData = new List<UpdateLengthData>();

        public event Action OnFinish;
        public event Action OnProgress;

        public long UpdateResourceTotalZipLength
        {
            get { return m_UpdateTotalZipLength; }
            set { m_UpdateTotalZipLength = value; }
        }

        public int UpdateResourceCount
        {
            get { return m_UpdateCount; }
            set { m_UpdateCount = value; }
        }

        public int UpdateSuccessCount
        {
            get { return m_UpdateSuccessCount; }
            set { m_UpdateSuccessCount = value; }
        }


        public float ProgressTotal
        {
            get;
            set;
        }
        

        public long CurrentTotalUpdateLength
        {
            get;
            set;
        }

        public float CurrentSpeed
        {
            get { return ResourceEntry.Download.CurrentSpeed; }
        }

        void Start()
        {

            m_UpdateResourcesComplete = false;


            m_UpdateSuccessCount = 0;
            m_UpdateLengthData.Clear();


            ResourceEntry.Resource.ResourceUpdateStart += OnResourceUpdateStart;
            ResourceEntry.Resource.ResourceUpdateChanged += OnResourceUpdateChanged;
            ResourceEntry.Resource.ResourceUpdateSuccess += OnResourceUpdateSuccess;
            ResourceEntry.Resource.ResourceUpdateFailure += OnResourceUpdateFailure;

            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                ResourceEntry.Shutdown(ShutdownType.Quit);
                return;
            }

            StartUpdateResources(null);
        }



        void OnDestroy()
        {

            ResourceEntry.Resource.ResourceUpdateStart -= OnResourceUpdateStart;
            ResourceEntry.Resource.ResourceUpdateChanged -= OnResourceUpdateChanged;
            ResourceEntry.Resource.ResourceUpdateSuccess -= OnResourceUpdateSuccess;
            ResourceEntry.Resource.ResourceUpdateFailure -= OnResourceUpdateFailure;

        }

        protected void Update()
        {
            if (m_UpdateResourcesComplete) OnFinish?.Invoke();
        }

        private void StartUpdateResources(object userData)
        {
            Log.Info("Start update resources...");
            ResourceEntry.Resource.UpdateResources(OnUpdateResourcesComplete);
        }

        private void RefreshProgress()
        {
            CurrentTotalUpdateLength = 0L;
            for (int i = 0; i < m_UpdateLengthData.Count; i++)
            {
                CurrentTotalUpdateLength += m_UpdateLengthData[i].Length;
            }

            ProgressTotal = (float) CurrentTotalUpdateLength / m_UpdateTotalZipLength;
            // string descriptionText = string.Format("{0},{1},{2},{3},{4},{5}",
            //     m_UpdateSuccessCount.ToString(), m_UpdateCount.ToString(), 
            //     GetByteLengthString(currentTotalUpdateLength),
            //     GetByteLengthString(m_UpdateTotalZipLength), progressTotal,
            //     GetByteLengthString((int) Entry.Download.CurrentSpeed));
            // m_UpdateResourceForm.SetProgress(progressTotal, descriptionText);
            OnProgress?.Invoke();
        }

        private string GetByteLengthString(long byteLength)
        {
            if (byteLength < 1024L) // 2 ^ 10
            {
                return $"{byteLength} Bytes";
            }

            if (byteLength < 1048576L) // 2 ^ 20
            {
                return $"{(byteLength / 1024f):F2} KB";
            }

            if (byteLength < 1073741824L) // 2 ^ 30
            {
                return $"{(byteLength / 1048576f):F2} MB";
            }

            if (byteLength < 1099511627776L) // 2 ^ 40
            {
                return $"{(byteLength / 1073741824f):F2} GB";
            }

            if (byteLength < 1125899906842624L) // 2 ^ 50
            {
                return $"{(byteLength / 1099511627776f):F2} TB";
            }

            if (byteLength < 1152921504606846976L) // 2 ^ 60
            {
                return $"{(byteLength / 1125899906842624f):F2} PB";
            }

            return $"{(byteLength / 1152921504606846976f):F2} EB";
        }

        private void OnUpdateResourcesComplete(ND.Managers.ResourceMgr.Framework.Resource.IResourceGroup resourceGroup,
            bool result)
        {
            if (result)
            {
                m_UpdateResourcesComplete = true;
                Log.Info("Update resources complete with no errors.");
                OnFinish?.Invoke();
            }
            else
            {
                Log.Error("Update resources complete with errors.");
            }
        }

        private void OnResourceUpdateStart(object sender, ND.Managers.ResourceMgr.Runtime.ResourceUpdateStartEventArgs ne)
        {


            for (int i = 0; i < m_UpdateLengthData.Count; i++)
            {
                if (m_UpdateLengthData[i].Name == ne.Name)
                {
                    Debug.LogWarningFormat("Update resource '{0}' is invalid.", ne.Name);
                    m_UpdateLengthData[i].Length = 0;
                    RefreshProgress();
                    return;
                }
            }

            m_UpdateLengthData.Add(new UpdateLengthData(ne.Name));
        }

        private void OnResourceUpdateChanged(object sender,
            ND.Managers.ResourceMgr.Runtime.ResourceUpdateChangedEventArgs ne)
        {


            for (int i = 0; i < m_UpdateLengthData.Count; i++)
            {
                if (m_UpdateLengthData[i].Name == ne.Name)
                {
                    m_UpdateLengthData[i].Length = ne.CurrentLength;
                    RefreshProgress();
                    return;
                }
            }

            Debug.LogWarningFormat("Update resource '{0}' is invalid.", ne.Name);
        }

        private void OnResourceUpdateSuccess(object sender,
            ND.Managers.ResourceMgr.Runtime.ResourceUpdateSuccessEventArgs ne)
        {

            Debug.LogFormat("Update resource '{0}' success.", ne.Name);

            for (int i = 0; i < m_UpdateLengthData.Count; i++)
            {
                if (m_UpdateLengthData[i].Name == ne.Name)
                {
                    m_UpdateLengthData[i].Length = ne.ZipLength;
                    m_UpdateSuccessCount++;
                    RefreshProgress();
                    return;
                }
            }

            Debug.LogWarningFormat("Update resource '{0}' is invalid.", ne.Name);
        }

        private void OnResourceUpdateFailure(object sender,
            ND.Managers.ResourceMgr.Runtime.ResourceUpdateFailureEventArgs ne)
        {

            if (ne.RetryCount >= ne.TotalRetryCount)
            {
                Debug.LogErrorFormat(
                    "Update resource '{0}' failure from '{1}' with error message '{2}', retry count '{3}'.", ne.Name,
                    ne.DownloadUri, ne.ErrorMessage, ne.RetryCount.ToString());
                return;
            }
            else
            {
                Debug.LogFormat("Update resource '{0}' failure from '{1}' with error message '{2}', retry count '{3}'.",
                    ne.Name,
                    ne.DownloadUri, ne.ErrorMessage, ne.RetryCount.ToString());
            }

            for (int i = 0; i < m_UpdateLengthData.Count; i++)
            {
                if (m_UpdateLengthData[i].Name == ne.Name)
                {
                    m_UpdateLengthData.Remove(m_UpdateLengthData[i]);
                    RefreshProgress();
                    return;
                }
            }

            Debug.LogWarningFormat("Update resource '{0}' is invalid.", ne.Name);
        }

        private class UpdateLengthData
        {
            private readonly string m_Name;

            public UpdateLengthData(string name)
            {
                m_Name = name;
            }

            public string Name
            {
                get { return m_Name; }
            }

            public int Length { get; set; }
        }

    }
}
