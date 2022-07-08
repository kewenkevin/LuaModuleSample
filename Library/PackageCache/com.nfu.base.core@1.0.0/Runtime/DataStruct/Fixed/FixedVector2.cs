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
//     File Name           :        FixedVector2.cs
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
namespace ND.Core.DataStruct
{
    public struct FixedVector2
    {
        public Fixed64 x;
        public Fixed64 y;

        public FixedVector2(Fixed64 x, Fixed64 y)
        {
            this.x = x;
            this.y = y;
        }

        public FixedVector2(Fixed64 x, int y)
        {
            this.x = x;
            this.y = (Fixed64) y;
        }

        public FixedVector2(int x, int y)
        {
            this.x = (Fixed64) x;
            this.y = (Fixed64) y;
        }

        public FixedVector2(FixedVector2 v)
        {
            this.x = v.x;
            this.y = v.y;
        }

        public static FixedVector2 operator -(FixedVector2 a, int b)
        {
            Fixed64 x = a.x - b;
            Fixed64 y = a.y - b;
            return new FixedVector2(x, y);
        }

        public Fixed64 this[int index]
        {
            get { return index == 0 ? x : y; }
            set
            {
                if (index == 0)
                {
                    x = value;
                }
                else
                {
                    y = value;
                }
            }
        }

        public static FixedVector2 Zero
        {
            get { return new FixedVector2(Fixed64.Zero, Fixed64.Zero); }
        }

        public static FixedVector2 operator +(FixedVector2 a, FixedVector2 b)
        {
            Fixed64 x = a.x + b.x;
            Fixed64 y = a.y + b.y;
            return new FixedVector2(x, y);
        }

        public static FixedVector2 operator -(FixedVector2 a, FixedVector2 b)
        {
            Fixed64 x = a.x - b.x;
            Fixed64 y = a.y - b.y;
            return new FixedVector2(x, y);
        }

        public static FixedVector2 operator *(Fixed64 d, FixedVector2 a)
        {
            Fixed64 x = a.x * d;
            Fixed64 y = a.y * d;
            return new FixedVector2(x, y);
        }

        public static FixedVector2 operator *(FixedVector2 a, Fixed64 d)
        {
            Fixed64 x = a.x * d;
            Fixed64 y = a.y * d;
            return new FixedVector2(x, y);
        }

        public static FixedVector2 operator /(FixedVector2 a, Fixed64 d)
        {
            Fixed64 x = a.x / d;
            Fixed64 y = a.y / d;
            return new FixedVector2(x, y);
        }

        public static bool operator ==(FixedVector2 lhs, FixedVector2 rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public static bool operator !=(FixedVector2 lhs, FixedVector2 rhs)
        {
            return lhs.x != rhs.x || lhs.y != rhs.y;
        }

        public override bool Equals(object obj)
        {
            return obj is FixedVector2 && ((FixedVector2) obj) == this;
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() + this.y.GetHashCode();
        }


        public static Fixed64 SqrMagnitude(FixedVector2 a)
        {
            return a.x * a.x + a.y * a.y;
        }

        public static Fixed64 Distance(FixedVector2 a, FixedVector2 b)
        {
            return Magnitude(a - b);
        }

        public static Fixed64 Magnitude(FixedVector2 a)
        {
            return Fixed64.Sqrt(FixedVector2.SqrMagnitude(a));
        }

        public void Normalize()
        {
            Fixed64 n = x * x + y * y;
            if (n == Fixed64.Zero)
                return;

            n = Fixed64.Sqrt(n);

            if (n < (Fixed64) 0.0001)
            {
                return;
            }

            n = 1 / n;
            x *= n;
            y *= n;
        }

        public FixedVector2 GetNormalized()
        {
            FixedVector2 v = new FixedVector2(this);
            v.Normalize();
            return v;
        }

        public override string ToString()
        {
            return string.Format("x:{0} y:{1}", x, y);
        }
        
        public UnityEngine.Vector2 ToVector2()
        {
            return new UnityEngine.Vector2((float)x, (float)y);
        }
    }
}