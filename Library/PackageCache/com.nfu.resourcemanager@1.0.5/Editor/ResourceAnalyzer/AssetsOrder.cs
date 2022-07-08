﻿//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{
    public enum AssetsOrder : byte
    {
        AssetNameAsc,
        AssetNameDesc,
        DependencyResourceCountAsc,
        DependencyResourceCountDesc,
        DependencyAssetCountAsc,
        DependencyAssetCountDesc,
        ScatteredDependencyAssetCountAsc,
        ScatteredDependencyAssetCountDesc,
    }
}
