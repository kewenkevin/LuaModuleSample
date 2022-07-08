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
//     File Name           :        Hex.cs
// 
//     Programmer          :        Wei Wei (Battle Mage Gandalf)
// 
//     Start Date          :        04/07/2021
// 
//     Last Update         :        04/07/2021 16:30 [Wei]
// 
//     Description         :        Hex position in x & y & z coordinate system(x,y,z on 2D coordinate system angle 120°)
// 
// =============================================================================================
// Contributors:
// ---------------------------------------------------------------------------------------------
// Battle Mage Gandalf                 wwei@yoozoo.com             Product technology Center
// =============================================================================================

using System;
using UnityEngine;

namespace ND.Core.DataStruct.Geometry
{
    [Serializable]
    public struct Hex
    {
        [SerializeField] public int x;
        [SerializeField] public int y;
        [SerializeField] public int z;

        public Hex(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }


        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    default:
                        throw new IndexOutOfRangeException("Invalid Hex index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid HexTransform index!");
                }
            }
        }

        //========== 快速构造 ==========

        /// <summary>
        /// 原点
        /// </summary>
        public static Hex zero
        {
            get { return new Hex(0, 0, 0); }
        }


        /// <summary>
        /// 左上
        /// </summary>
        public static Hex LTop
        {
            get { return new Hex(0, 1, -1); }
        }

        /// <summary>
        /// 右上
        /// </summary>
        public static Hex RTop
        {
            get { return new Hex(1, 0, -1); }
        }

        public static Hex Left
        {
            get { return new Hex(-1, 1, 0); }
        }

        public static Hex Right
        {
            get { return new Hex(1, -1, 0); }
        }

        public static Hex LBottom
        {
            get { return new Hex(-1, 0, 1); }
        }

        public static Hex RBottom
        {
            get { return new Hex(0, -1, 1); }
        }


        public static Hex[] Directions
        {
            get
            {
                return new Hex[6]
                {
                    Hex.LTop,
                    Hex.RTop,
                    Hex.Right,
                    Hex.RBottom,
                    Hex.LBottom,
                    Hex.Left
                };
            }
        }

        //========== 数组操作 =========
        public static Hex[] operator +(Hex[] a, Hex b)
        {
            Hex[] c = new Hex[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                c[i] = a[i] + b;
            }

            return c;
        }


        public static Hex[] operator -(Hex[] a, Hex b)
        {
            Hex[] c = new Hex[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                c[i] = a[i] - b;
            }

            return c;
        }

        //========== 运算符 =========

        public static Hex operator +(Hex a, Hex b)
        {
            return new Hex(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Hex operator -(Hex a, Hex b)
        {
            return new Hex(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Hex operator -(Hex a)
        {
            return new Hex(-a.x, -a.y, -a.z);
        }

        public static Hex operator *(Hex a, int d)
        {
            return new Hex(a.x * d, a.y * d, a.z * d);
        }

        public static Hex operator *(int d, Hex a)
        {
            return new Hex(a.x * d, a.y * d, a.z * d);
        }

        public static Hex operator /(Hex a, int d)
        {
            return new Hex(a.x / d, a.y / d, a.z / d);
        }

        public static bool operator ==(Hex lhs, Hex rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
        }

        public static bool operator !=(Hex lhs, Hex rhs)
        {
            return lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z;
        }

        //========== 工具 =========
        public static int Distance(Hex from, Hex to)
        {
            return (int) Mathf.Max(Mathf.Abs(from.x - to.x), Mathf.Abs(from.y - to.y), Mathf.Abs(from.z - to.z));
        }


        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }

        public string ToString(string format)
        {
            return $"({x.ToString(format)}, {y.ToString(format)}, {z.ToString(format)})";
        }

        public void Fix()
        {
            y = -x - z;
        }
    }
}