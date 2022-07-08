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
//     File Name           :        Vector3Extension.cs
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
using UnityEngine;

namespace ND.Core.Extensions.Engine
{
    public static class Vector3Extension
    {
        public static Vector2 xy(this Vector3 point)
        {
            return new Vector2(point.x, point.y);
        }

        public static Vector2 xz(this Vector3 point)
        {
            return new Vector2(point.x, point.z);
        }

        public static Vector2 yx(this Vector3 point)
        {
            return new Vector2(point.y, point.x);
        }

        public static Vector2 zx(this Vector3 point)
        {
            return new Vector2(point.z, point.x);
        }

        public static Vector2 yz(this Vector3 point)
        {
            return new Vector2(point.y, point.z);
        }

        public static Vector2 zy(this Vector3 point)
        {
            return new Vector2(point.z, point.y);
        }
    }
}