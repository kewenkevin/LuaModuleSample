//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using ND.Managers.ResourceMgr.Framework.FileSystem;
using System;

namespace ND.Managers.ResourceMgr.Runtime
{
    /// <summary>
    /// 默认文件系统辅助器。
    /// </summary>
    public class DefaultFileSystemHelper : FileSystemHelperBase
    {
        private const string AndroidFileSystemPrefixString = "jar:";

        /// <summary>
        /// 创建文件系统流。
        /// </summary>
        /// <param name="fullPath">要加载的文件系统的完整路径。</param>
        /// <param name="access">要加载的文件系统的访问方式。</param>
        /// <param name="createNew">是否创建新的文件系统流。</param>
        /// <returns>创建的文件系统流。</returns>
        public override FileSystemStream CreateFileSystemStream(string fullPath, FileSystemAccess access, bool createNew)
        {
            if (fullPath.StartsWith(AndroidFileSystemPrefixString, StringComparison.Ordinal))
            {
                //obb 的 InputStream Seek 有问题，没有用 AndroidFileSystemStream 流读取 
                if (ResourcesAndroidUtility.IsObb)
                    return new AndroidInputStream(fullPath);
                return new AndroidFileSystemStream(fullPath, access, createNew);
            }
            else
            {
                return new CommonFileSystemStream(fullPath, access, createNew);
            }
        }
    }
}
