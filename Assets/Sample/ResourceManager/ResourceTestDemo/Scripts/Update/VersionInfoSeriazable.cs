
using System;
using ResourceMgr.Runtime.ResourceUpdate;

namespace ND.Gameplay.Managers.ResourceManagerV2.Editor
{
    [Serializable]
    public class VersionInfoSeriazable
    {
        // 是否需要强制更新游戏应用
            public bool ForceUpdateGame;

            // 最新的游戏版本号
            public string LatestGameVersion;

            // 最新的游戏内部版本号
            public int InternalGameVersion;

            // 最新的资源内部版本号
            public int InternalResourceVersion;

            // 资源更新下载地址
            public string UpdatePrefixUri;

            // 资源版本列表长度
            public int VersionListLength;

            // 资源版本列表哈希值
            public int VersionListHashCode;

            // 资源版本列表压缩后长度
            public int VersionListZipLength;

            // 资源版本列表压缩后哈希值
            public int VersionListZipHashCode;
            
            public static implicit operator VersionInfoSeriazable(VersionInfo versionInfo)
            {
                var tmp = new VersionInfoSeriazable();
                tmp.ForceUpdateGame = versionInfo.ForceUpdateGame;
                tmp.LatestGameVersion = versionInfo.LatestGameVersion;
                tmp.InternalResourceVersion = versionInfo.InternalResourceVersion;
                tmp.UpdatePrefixUri = versionInfo.UpdatePrefixUri;
                tmp.VersionListLength = versionInfo.VersionListLength;
                tmp.VersionListHashCode = versionInfo.VersionListHashCode;
                tmp.VersionListZipLength = versionInfo.VersionListZipLength;
                tmp.VersionListZipHashCode = versionInfo.VersionListZipHashCode;
                tmp.InternalGameVersion = versionInfo.InternalGameVersion;
                return tmp;
            }

            public static implicit operator VersionInfo(VersionInfoSeriazable seriazable)
            {
                var tmp = new VersionInfo();
                tmp.ForceUpdateGame = seriazable.ForceUpdateGame;
                tmp.LatestGameVersion = seriazable.LatestGameVersion;
                tmp.InternalResourceVersion = seriazable.InternalResourceVersion;
                tmp.UpdatePrefixUri = seriazable.UpdatePrefixUri;
                tmp.VersionListLength = seriazable.VersionListLength;
                tmp.VersionListHashCode = seriazable.VersionListHashCode;
                tmp.VersionListZipLength = seriazable.VersionListZipLength;
                tmp.VersionListZipHashCode = seriazable.VersionListZipHashCode;
                tmp.InternalGameVersion = seriazable.InternalGameVersion;
                return tmp;
            }

    }
}