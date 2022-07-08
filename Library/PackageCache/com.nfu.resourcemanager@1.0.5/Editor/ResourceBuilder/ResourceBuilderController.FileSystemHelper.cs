﻿//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using ND.Managers.ResourceMgr.Framework.FileSystem;

namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{
    public sealed partial class ResourceBuilderController
    {
        private sealed class FileSystemHelper : IFileSystemHelper
        {
            public FileSystemStream CreateFileSystemStream(string fullPath, FileSystemAccess access, bool createNew)
            {
                return new CommonFileSystemStream(fullPath, access, createNew);
            }
        }
    }
}
