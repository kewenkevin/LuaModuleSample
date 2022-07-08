//
// Copyright 2020 Yoozoo Net Inc.
//
// UMT Framework and corresponding source code is free 
// software: you can redistribute it and/or modify it under the terms of 
// the GNU General Public License as published by the Free Software Foundation, 
// either version 3 of the License, or (at your option) any later version.

// UMT Framework and corresponding source code is distributed 
// in the hope that it will be useful, but with permitted additional restrictions 
// under Section 7 of the GPL. See the GNU General Public License in LICENSE.TXT 
// distributed with this program. You should have received a copy of the 
// GNU General Public License along with permitted additional restrictions 
// with this program. If not, see https://gitlab.uuzu.com/yoozooopensource/client/framework/core

/***********************************************************************************************
 ***                  C O N F I D E N T I A L  ---  U M T   T E A M                          ***
 ***********************************************************************************************
 *                                                                                             *
 *                 Project Name : UMT Framework Core Liberary                                  *
 *                                                                                             *
 *                    File Name : Vector2Utility.cs                                            *
 *                                                                                             *
 *                   Programmer : Wei Wei (Battle Mage Gandalf)                                *
 *                                                                                             *
 *                   Start Date : 04/07/2021                                                   *
 *                                                                                             *
 *                  Last Update : April 07, 2021 [Wei]                                         *
 *                                                                                             *
 *---------------------------------------------------------------------------------------------*
 * Contributors:                                                                               *
 * Battle Mage Gandalf                 wwei@yoozoo.com             Product technology Center   *
 * - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
using System;
using UnityEngine;

namespace ND.Core.Geometry
{
    public static class Vector2Utility
    {
       public static Vector2 Rotate(this Vector2 v, float degree)
        {
            float cos0 = Mathf.Cos(degree * Mathf.Deg2Rad);
            float sin0 = Mathf.Sin(degree * Mathf.Deg2Rad);
            v.x = v.x * cos0 - v.y * sin0;
            v.y = v.x * sin0 + v.y * cos0;
            return v;
        }

        
        /// <summary>
        /// 顺时针旋转向量90度
        /// </summary>
        /// <param name="v"></param>
        /// <param name="inverse">默认顺时针，true为逆时针</param>
        /// <returns></returns>
        public static Vector2 Rotate90(this Vector2 v, bool inverse = false)
        {
            int sin0 = inverse ? -1 : 1;
            return new Vector2(-v.y * sin0,v.x * sin0);
        }
        
        /// <summary>
        /// 旋转向量自身
        /// </summary>
        /// <param name="v"></param>
        /// <param name="inverse"></param>
        public static void SelfRotate90(this ref Vector2 v, bool inverse = false)
        {
            int sin0 = inverse ? -1 : 1;
            v.x = -v.y * sin0;
            v.y = v.x * sin0;
        }

        
        
        /// <summary>
        /// 顺时针旋转向量90度 n次
        /// </summary>
        /// <param name="vector2"></param>
        /// <param name="tm"></param>
        public static void RotateClockWise90(this ref Vector2 vector2, int tm = 1)
        {
            int o0 = tm & 0x01;
            int o1 = (tm & 0x02) >> 1;
            float x = o0 == 0x01 ? vector2.x : vector2.y;
            float y = o0 == 0x01 ? vector2.y : vector2.x;
            vector2.Set(x * (1 + o1 * -2), y * (1 + (o0 ^ o1) * -2));
        }


        public static Vector2 Clone(this Vector2 vec)
        {
            return new Vector2(vec.x, vec.y);
        }

        
        /// <summary>
        /// 返回矢量所在象限
        /// </summary>
        /// <param name="v"></param>
        /// <returns>象限 ，0为原点</returns>
        public static int CheckQuadrant(this Vector2 v)
        {
            if (v.x > 0)
            {
                if (v.y > 0) return 1;
                return v.y < 0 ? 4 : 0;
            }

            if (!(v.x < 0)) return 0;
            if (v.y > 0) return 2;
            return v.y < 0 ? 3 : 0;
        }

        /// <summary>
        /// 转换成整型向量
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector2Int ConvertToVector2IntFloor(Vector2 v2)
        {
            return new Vector2Int(Mathf.FloorToInt(v2.x), Mathf.FloorToInt(v2.y));
        }

        /// <summary>
        /// 得到一个分量值为正负1的二维矢量
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2Int GetSignOne(Vector2Int v)
        {
            return new Vector2Int(GetSignOne(v.x), GetSignOne(v.y));
        }

        public static Vector2 GetSignOne(Vector2 v)
        {
            return new Vector2(GetSignOne(v.x), GetSignOne(v.y));
        }

        /// <summary>
        /// 大于0返回1小于0返回-1
        /// </summary>
        /// <param name="i"></param>
        /// <param name="equalZero"></param>
        /// <returns></returns>
        public static int GetSignOne(int i, int equalZero = 0)
        {
            int r = equalZero;
            if (i > 0) r = 1;
            if (i < 0) r = -1;
            return r;
        }

        public static float GetSignOne(float i, float equalZero = 0)
        {
            float r = equalZero;
            if (i > 0) r = 1;
            if (i < 0) r = -1;
            return r;
        }

        

        /// <summary>
        /// 获得一个(0-1]正态分布的二维矢量
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetBoxMullerVect2()
        {
            float U1 = UnityEngine.Random.Range(Mathf.Epsilon, 1.0f);
            float U2 = UnityEngine.Random.Range(Mathf.Epsilon, 1.0f);
            float Z0 = Mathf.Sqrt(-2f * Mathf.Log(U1)) * Mathf.Cos(2f * Mathf.PI * U2);
            float Z1 = Mathf.Sqrt(-2f * Mathf.Log(U1)) * Mathf.Sin(2f * Mathf.PI * U2);
            return new Vector2(Z0, Z1);
        }
        
        
        public static Vector2 GetRandom()
        {
            return UnityEngine.Random.insideUnitCircle;
        }

    }
}