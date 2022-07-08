using System.IO;
using ResourceMgr.Runtime.ResourceUpdate;
using UnityEngine;
using ND.Gameplay.Managers.ResourceManagerV2.Editor;
using ND.Managers.ResourceMgr.Framework.Resource;
using ND.Managers.ResourceMgr.Runtime;
using ND.Managers.ResourceMgr.Framework;

public class CustomCheckAppVersion : UpdateStepAgentHelperBase
{
    [SerializeField] private string CheckVersionUrl = "versionInfo.json";

    [Tooltip("通过检测下一步")] [SerializeField] private UpdateStepAgentHelperBase nextStepPass = null;

    [Tooltip("未能通过检测下一步")] [SerializeField]
    private UpdateStepAgentHelperBase nextStepNotPass = null;

    protected override void OnBegin()
    {
        CheckVersionUrl = string.Format("file:///{0}/{1}",Path.Combine(Directory.GetCurrentDirectory(), "output/BuildReport"), CheckVersionUrl);
        Debug.Log("CheckVersionUrl:"+ CheckVersionUrl);

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


    protected VersionInfo ParseAppVersionInfo(string responseContent)
    {
        VersionInfoSeriazable versionInfo = JsonUtility.FromJson<VersionInfoSeriazable>(responseContent);
        return versionInfo;
    }
    


    protected void CheckByResponseInfo(string responseContent)
    {
        VersionInfo m_VersionInfo = ParseAppVersionInfo(responseContent);

        if (m_VersionInfo == null)
        {
            Log.Error("Parse VersionInfo failure.");
            ResourceEntry.ResourceUpdate.OnResourceFailedFinish();
            return;
        }

        // Log.Info("Latest game version is '{0} ({1})', local game version is '{2} ({3})'.",
        //     m_VersionInfo.LatestGameVersion, m_VersionInfo.InternalGameVersion.ToString(),
        //     ND.Managers.ResourceEntry.Framework.Version.GameVersion,
        //     ND.Managers.ResourceEntry.Framework.Version.InternalGameVersion.ToString());

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

        if (ResourceEntry.Resource.CheckVersionList(m_VersionInfo.InternalResourceVersion) ==
            CheckVersionListResult.NeedUpdate)
        {
            ResourceEntry.ResourceUpdate.SetData<VarInt>("VersionListLength", m_VersionInfo.VersionListLength);
            ResourceEntry.ResourceUpdate.SetData<VarInt>("VersionListHashCode", m_VersionInfo.VersionListHashCode);
            ResourceEntry.ResourceUpdate.SetData<VarInt>("VersionListZipLength", m_VersionInfo.VersionListZipLength);
            ResourceEntry.ResourceUpdate.SetData<VarInt>("VersionListZipHashCode",
                m_VersionInfo.VersionListZipHashCode);
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
                return "Windows";

            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                return "MacOS";

            case RuntimePlatform.IPhonePlayer:
                return "IOS";

            case RuntimePlatform.Android:
                return "Android";

            default:
                throw new System.NotSupportedException(Utility.Text.Format("Platform '{0}' is not supported.",
                    Application.platform.ToString()));
        }
    }
}


