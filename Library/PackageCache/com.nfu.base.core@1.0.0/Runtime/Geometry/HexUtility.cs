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
//     File Name           :        HexUtility.cs
// 
//     Programmer          :        Wei Wei (Battle Mage Gandalf)
// 
//     Start Date          :        04/07/2021
// 
//     Last Update         :        04/07/2021 16:30 [Wei]
// 
//     Description         :        Hexgean support tools
// 
// =============================================================================================
// Contributors:
// ---------------------------------------------------------------------------------------------
// Battle Mage Gandalf                 wwei@yoozoo.com             Product technology Center
// =============================================================================================

using System;
using System.Collections.Generic;
using ND.Core.DataStruct.Geometry;
using ND.Core.Enums;
using UnityEngine;

namespace ND.Core.Geometry
{
    public static class HexUtility
    {
        public static Hex ToHex(this AxialHex ax)
        {
            return new Hex(ax.q, -ax.q - ax.r, ax.r);
        }

        public static AxialHex ToAxial(this Hex hex)
        {
            return new AxialHex(hex.x, hex.z);
        }


        public static Hex ToHex(this NetHex grid, HexType type)
        {
            Hex hex = new Hex(0, 0, 0);
            int col = grid.col;
            int row = grid.row;
            switch (type)
            {
                case HexType.Odd_q:
                {
                    hex.x = col;
                    hex.z = row - (col - (col & 1)) / 2;
                    hex.y = -hex.x - hex.z;
                }
                    break;
                case HexType.Even_q:
                {
                    hex.x = col;
                    hex.z = row - (col + (col & 1)) / 2;
                    hex.y = -hex.x - hex.z;
                }
                    break;
                case HexType.Odd_r:
                {
                    hex.x = col - (row - (row & 1)) / 2;
                    hex.z = row;
                    hex.y = -hex.x - hex.z;
                }
                    break;
                case HexType.Even_r:
                {
                    hex.x = col - (row + (row & 1)) / 2;
                    hex.z = row;
                    hex.y = -hex.x - hex.z;
                }
                    break;
                default:
                    break;
            }

            return hex;
        }


        public static NetHex ToNet(this Hex hex, HexType type)
        {
            int col;
            int row;
            switch (type)
            {
                case HexType.Odd_q:
                {
                    col = hex.x;
                    row = hex.z + (hex.x - (hex.x & 1)) / 2;
                }
                    break;
                case HexType.Even_q:
                {
                    col = hex.x;
                    row = hex.z + (hex.x + (hex.x & 1)) / 2;
                }
                    break;
                case HexType.Odd_r:
                {
                    col = hex.x + (hex.z - (hex.z & 1)) / 2;
                    row = hex.z;
                }
                    break;
                case HexType.Even_r:
                {
                    col = hex.x + (hex.z + (hex.z & 1)) / 2;
                    row = hex.z;
                }
                    break;
                default:
                {
                    col = 0;
                    row = 0;
                }
                    break;
            }

            return new NetHex(col, row);
        }

        //类型是否尖角朝上
        public static bool IsPointTop(this HexType type)
        {
            return type == HexType.Even_r || type == HexType.Odd_r;
        }

        //网格中心相对0点便宜位置，y轴为0。
        public static Vector3 ToPixel(this Hex hex, float size, HexType type)
        {

            AxialHex ax = hex.ToAxial();
            if (IsPointTop(type))
                return new Vector3(
                    size * 1.73205080756887f * (ax.q + ax.r * .5f),
                    0,
                    size * 1.5f * ax.r);
            else
                return new Vector3(size * 1.5f * ax.q,
                    0,
                    size * 1.73205080756887f * (ax.r + ax.q * .5f));
        }


        public static Hex ToHex(this Vector3 pixel, float size, HexType type)
        {
            float q = 0;
            float r = 0;
            if (IsPointTop(type))
            {
                q = ((pixel.x * 1.73205080756887f - pixel.z) / (size * 3));
                r = (pixel.z * 0.666666666666f / size);
            }
            else
            {
                q = (pixel.x * 0.666666666666f / size);
                r = ((pixel.z * 1.73205080756887f - pixel.x) / (size * 3));
            }

            AxialHex ax = AxialHex.RoundAxial(q, r);
            return ax.ToHex();
        }



        public static Hex RoundHex(float x, float y, float z)
        {
            float rx = Mathf.Round(x);
            float ry = Mathf.Round(y);
            float rz = Mathf.Round(z);

            float x_diff = Mathf.Abs(rx - x);
            float y_diff = Mathf.Abs(ry - y);
            float z_diff = Mathf.Abs(rz - z);

            if (x_diff > y_diff && x_diff > z_diff)
                rx = -ry - rz;
            else if (y_diff > z_diff)
                ry = -rx - rz;
            else
                rz = -rx - ry;

            return new Hex((int) rx, (int) ry, (int) rz);
        }


        /// <summary>
        /// 获取相邻坐标组，左上顺时针旋转
        /// </summary>
        /// <param name="hex">目标坐标</param>
        /// <returns>坐标组</returns>
        public static Hex[] GetNeighbor(this Hex hex)
        {
            return Hex.Directions + hex;
        }


        /// <summary>
        /// 获取经过两点之间的坐标组
        /// </summary>
        /// <param name="from">开始坐标</param>
        /// <param name="target">结束坐标</param>
        /// <returns>坐标组</returns>
        public static Hex[] LineTo(this Hex from, Hex target)
        {
            int N = Hex.Distance(from, target);
            Hex[] results = new Hex[N];

            for (int i = 0; i <= N; i++)
            {
                float t = (float) i / (float) N;
                results[i] = HexUtility.RoundHex(Mathf.Lerp(from.x, target.x, t), Mathf.Lerp(from.y, target.y, t),
                    Mathf.Lerp(from.z, target.z, t));
            }

            return results;
        }



        /// <summary>
        /// 获取中心点范围内的所有坐标
        /// </summary>
        /// <param name="center">中心点</param>
        /// <param name="radius">范围</param>
        /// <returns>坐标组</returns>
        public static Hex[] Range(this Hex center, int radius)
        {
            List<Hex> results = new List<Hex>();
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = Math.Max(-radius, -dx - radius); dy <= Math.Min(radius, -dx + radius); dy++)
                {
                    int dz = -dx - dy;
                    results.Add(center + new Hex(dx, dy, dz));
                }
            }

            return results.ToArray();
        }

        /// <summary>
        /// 获取以center为圆心的环形区域，半径为Radius
        /// </summary>
        /// <param name="center">中心位置</param>
        /// <param name="radius">环半径（不为0）</param>
        /// <returns>环形包含的格子</returns>
        public static Hex[] Ring(this Hex center, int radius)
        {
            List<Hex> results = new List<Hex>();


            Hex tmp = center + Hex.Directions[4] * radius;
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < radius; j++)
                {
                    results.Add(tmp);
                    tmp = tmp.GetNeighbor()[i];
                }
            }

            return results.ToArray();
        }

    }
}