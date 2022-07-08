using System;
using System.ComponentModel;
using ND.Managers.ResourceMgr.Framework;
using ND.Managers.ResourceMgr.Framework.Resource;
using ND.Managers.ResourceMgr.Runtime;
using UnityEngine;
using Version = ND.Managers.ResourceMgr.Framework.Version;
namespace ResourceMgr.Runtime.ResourceUpdate
{
    public class CheckAppVersion : UpdateStepAgentHelperBase
    {
        [HideInInspector]public string CheckVersionUrl = "http://www.baidu.com";
        
        [Tooltip("通过检测下一步")]
        [SerializeField] 
        public UpdateStepAgentHelperBase nextStepPass = null;

        [Tooltip("未能通过检测下一步")] 
        [SerializeField]
        public UpdateStepAgentHelperBase nextStepNotPass = null;

        public Func<string, VersionInfo> GetAppVersionInfo;

        protected override void OnBegin()
        {
            ResourceEntry.WebRequest.WebRequestSuccess += OnCheckVersionWebRequestSuccess;
            ResourceEntry.WebRequest.WebRequestFailure += OnCheckVersionWebRequestFailure;
            ResourceEntry.WebRequest.AddWebRequest(Utility.Text.Format(CheckVersionUrl, GetPlatformPath()), this);
        }

        private void OnCheckVersionWebRequestSuccess(object sender, WebRequestSuccessEventArgs ne)
        {
            if (!ReferenceEquals(ne.UserData, this))
            {
                return;
            }

            ResourceEntry.WebRequest.WebRequestSuccess -= OnCheckVersionWebRequestSuccess;
            ResourceEntry.WebRequest.WebRequestFailure -= OnCheckVersionWebRequestFailure;

            // 解析版本信息
            byte[] versionInfoBytes = ne.GetWebResponseBytes();
            var httpResponseStr = System.Text.Encoding.Default.GetString(versionInfoBytes);


            CheckByResponseInfo(httpResponseStr);
        }


        protected virtual VersionInfo ParseAppVersionInfo(string responseContent)
        {
            VersionInfo versionInfo = null;
            if (GetAppVersionInfo == null)
            {
                versionInfo = new VersionInfo();
                //todo 默认需要一个解析方式
                // versionInfo.ForceUpdateGame = false;
                // versionInfo.LatestGameVersion = "0.1";
                // versionInfo.InternalGameVersion = 0;
                // versionInfo.InternalResourceVersion = 24;
                // versionInfo.UpdatePrefixUri = "http://192.168.90.222:8765/GameFrameworkV1";
                // versionInfo.VersionListHashCode = Convert.ToInt32("b28b333c", 16);
                // versionInfo.VersionListZipHashCode = -980091430;
                // versionInfo.VersionListZipLength = 624;
                // versionInfo.VersionListLength = 1533;
            }
            else
            {
                versionInfo = GetAppVersionInfo.Invoke(responseContent);
            }
            
            return versionInfo;
        }


        protected void CheckByResponseInfo(string responseContent)
        {
            VersionInfo m_VersionInfo =  ParseAppVersionInfo(responseContent);

            if (m_VersionInfo == null)
            {
                Log.Error("Parse VersionInfo failure.");
                ResourceEntry.ResourceUpdate.OnResourceFailedFinish();
                return;
            }

            Log.Info("Latest game version is '{0} ({1})', local game version is '{2} ({3})'.",
                m_VersionInfo.LatestGameVersion, m_VersionInfo.InternalGameVersion.ToString(), Version.GameVersion,
                Version.InternalGameVersion.ToString());

            if (m_VersionInfo.ForceUpdateGame)
            {
                // 需要强制更新游戏应用
                ResourceEntry.ResourceUpdate.OnResourceFailedFinish();
#if UNITY_EDITOR
                if (UnityEditor.EditorUtility.DisplayDialog("需要热更新", "", ""))
                {
                    ResourceEntry.Shutdown(ShutdownType.Quit);
                }
#else
                ResourceEntry.Shutdown(ShutdownType.Quit);
#endif
            }

            // 设置资源更新下载地址
            ResourceEntry.Resource.UpdatePrefixUri = Utility.Path.GetRegularPath(m_VersionInfo.UpdatePrefixUri);

            if (ResourceEntry.Resource.CheckVersionList(m_VersionInfo.InternalResourceVersion, m_VersionInfo.LatestGameVersion) ==
                CheckVersionListResult.NeedUpdate)
            {
                ResourceEntry.ResourceUpdate.SetData<VarInt>("VersionListLength", m_VersionInfo.VersionListLength);
                ResourceEntry.ResourceUpdate.SetData<VarInt>("VersionListHashCode",  m_VersionInfo.VersionListHashCode);
                ResourceEntry.ResourceUpdate.SetData<VarInt>("VersionListZipLength",  m_VersionInfo.VersionListZipLength);
                ResourceEntry.ResourceUpdate.SetData<VarInt>("VersionListZipHashCode",  m_VersionInfo.VersionListZipHashCode);
                nextStepNotPass.Begin();
            }
            else
            {
                nextStepPass.Begin();
            }
        }

        private void OnCheckVersionWebRequestFailure(object sender, WebRequestFailureEventArgs ne)
        {
            if (!ReferenceEquals(ne.UserData, this))
            {
                return;
            }
            ResourceEntry.WebRequest.WebRequestSuccess -= OnCheckVersionWebRequestSuccess;
            ResourceEntry.WebRequest.WebRequestFailure -= OnCheckVersionWebRequestFailure;
            OnWebRequestFailure(sender, ne);
        }

        protected virtual void OnWebRequestFailure(object sender, WebRequestFailureEventArgs ne)
        {
            Log.Warning("Check version failure, error message is '{0}'.", ne.ErrorMessage);
            ResourceEntry.ResourceUpdate.OnResourceFailedFinish();
        }


        private string GetPlatformPath()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.Android:
                    return ResourceSettings.GetPlatformName(Application.platform);
                default:
                    throw new System.NotSupportedException(Utility.Text.Format("Platform '{0}' is not supported.",
                        Application.platform.ToString()));
            }
        }
    }
}