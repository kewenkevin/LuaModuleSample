// 
// Copyright 2020 Yoozoo Net Inc.
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
// with this program. If not, see https://gitlab.uuzu.com/yoozooopensource/client/framework/core
// 
// ***********************************************************************************************
// ***                  C O N F I D E N T I A L  ---  U M T   T E A M                          ***
// ***********************************************************************************************
// 
//     Project Name        :        UMT Framework Core Liberary
// 
//     File Name           :        AngleUtility.cs
// 
//     Programmer          :        Wei Wei (Battle Mage Gandalf)
// 
//     Start Date          :        04/07/2021
// 
//     Last Update         :        04/07/2021 16:30 [Wei]
// 
//     Description         :        Angle to Radian support & Fit Large Angle
// 
// =============================================================================================
// Contributors:
// ---------------------------------------------------------------------------------------------
// Battle Mage Gandalf                 wwei@yoozoo.com             Product technology Center
// =============================================================================================

using UnityEngine;

namespace ND.Core.Geometry
{
    public class AngleUtility
    {
        // 让角度在[-180, 180)
        public static float AngleFit(float angle)
        {
            angle %= 360f;

            if (angle < -180f)
            {
                angle += 360;
            }

            if (angle >= 180f)
            {
                angle -= 360f;
            }

            return angle;
        }

        // 让角度在[0, 360)
        public static float AngleFit2(float angle)
        {
            angle %= 360f;

            if (angle < 0)
            {
                angle += 360;
            }

            return angle;
        }

        public static float Angle2Radian(float angle)
        {
            return angle * Mathf.PI / 180f;
        }

        public static float Radian2Angle(float radian)
        {
            return radian * 180f / Mathf.PI;
        }
    }
}