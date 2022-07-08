// Copyright 2020 Yoozoo Net Inc.
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
// with this program. If not, see https://gitlab.uuzu.com/yoozooopensource/client/framework/core
// 
// ***********************************************************************************************
// ***                  C O N F I D E N T I A L  ---  U M T   T E A M                          ***
// ***********************************************************************************************
// 
//     Project Name        :        UMT Framework Core Library
// 
//     File Name           :        RectExtension.cs
// 
//     Programmer          :        Wei Wei (Battle Mage Gandalf)
// 
//     Start Date          :        04/12/2021
// 
//     Last Update         :        04/12/2021 15:55 [Wei]
// 
//     Description         :        write here
// 
// =============================================================================================
// Contributors:
// ---------------------------------------------------------------------------------------------
// Battle Mage Gandalf                 wwei@yoozoo.com             Product technology Center
// =============================================================================================

using ND.Core.Geometry;
using UnityEngine;

namespace ND.Core.Extensions.Engine
{
    public static class RectExtension
    {
        /// <summary>
        /// 向四周扩展一个矩形
        /// </summary>
        /// <param name="rect">目标举行</param>
        /// <param name="outLine">向外扩展距离</param>
        /// <returns></returns>
        public static RectInt Expand(this RectInt rect, int outLine)
        {
            Vector2Int dir = Vector2Utility.GetSignOne(rect.size);
            Vector2Int newSize = rect.size + dir * outLine * 2;
            if (newSize.x == 0 || newSize.y == 0) return rect;
            Vector2Int newPos = rect.position - dir * outLine;
            return new RectInt(newPos, newSize);
        }
    }
}