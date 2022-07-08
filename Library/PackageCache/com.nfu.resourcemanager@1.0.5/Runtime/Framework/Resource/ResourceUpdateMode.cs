// 
// Copyright 2020 ND Net Inc.
// 
// UMT Framework and corresponding source code is free
// software: you can redistribute it and/or modify it under the terms of
// the GNU General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// UMT Framework and corresponding source code is distributed
// in the hope that it will be useful, but with permitted additional restrictions
// under Section 7 of the GPL. See the GNU General Public License in LICENSE.TXT
// distributed with this program. You should have received a copy of the
// GNU General Public License along with permitted additional restrictions
// with this program. If not, see https://gitlab.uuzu.com/NDopensource/client/framework/resoruce
// 
// ***********************************************************************************************
// ***                  C O N F I D E N T I A L  ---  U M T   T E A M                          ***
// ***********************************************************************************************
// 
//     Project Name        :        UMT Framework Resource Library
// 
//     File Name           :        UpdateMode.cs
// 
//     Programmer          :        Wei Wei (Battle Mage Gandalf)
// 
//     Start Date          :        04/08/2021
// 
//     Last Update         :        04/08/2021 10:03 [Wei]
// 
//     Description         :        write here
// 
// =============================================================================================
// Contributors:
// ---------------------------------------------------------------------------------------------
// Battle Mage Gandalf                 wwei@ND.com             Product technology Center
// =============================================================================================

namespace ND.Managers.ResourceMgr.Framework.Resource
{
    public enum ResourceUpdateMode : byte
    {
        /// <summary>
        /// 忽略版本号
        /// </summary>
        IgnoreVersion = 0,
        
        /// <summary>
        /// 向更高版本号更新，不降级
        /// </summary>
        Normal = 1,
    }
}