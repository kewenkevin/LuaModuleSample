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
//     File Name           :        FixedVector3.cs
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
    public struct FixedVector3
    {
        public Fixed64 x;
        public Fixed64 y;
        public Fixed64 z;

        public FixedVector3(Fixed64 x, Fixed64 y, Fixed64 z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public FixedVector3(FixedVector3 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
        }

        public Fixed64 this[int index]
        {
            get
            {
                if (index == 0)
                    return x;
                else if (index == 1)
                    return y;
                else
                    return z;
            }
            set
            {
                if (index == 0)
                    x = value;
                else if (index == 1)
                    y = value;
                else
                    y = value;
            }
        }

        public static FixedVector3 Zero
        {
            get { return new FixedVector3(Fixed64.Zero, Fixed64.Zero, Fixed64.Zero); }
        }

        public static FixedVector3 operator +(FixedVector3 a, FixedVector3 b)
        {
            Fixed64 x = a.x + b.x;
            Fixed64 y = a.y + b.y;
            Fixed64 z = a.z + b.z;
            return new FixedVector3(x, y, z);
        }

        public static FixedVector3 operator -(FixedVector3 a, FixedVector3 b)
        {
            Fixed64 x = a.x - b.x;
            Fixed64 y = a.y - b.y;
            Fixed64 z = a.z - b.z;
            return new FixedVector3(x, y, z);
        }

        public static FixedVector3 operator *(Fixed64 d, FixedVector3 a)
        {
            Fixed64 x = a.x * d;
            Fixed64 y = a.y * d;
            Fixed64 z = a.z * d;
            return new FixedVector3(x, y, z);
        }

        public static FixedVector3 operator *(FixedVector3 a, Fixed64 d)
        {
            Fixed64 x = a.x * d;
            Fixed64 y = a.y * d;
            Fixed64 z = a.z * d;
            return new FixedVector3(x, y, z);
        }

        public static FixedVector3 operator /(FixedVector3 a, Fixed64 d)
        {
            Fixed64 x = a.x / d;
            Fixed64 y = a.y / d;
            Fixed64 z = a.z / d;
            return new FixedVector3(x, y, z);
        }

        public static bool operator ==(FixedVector3 lhs, FixedVector3 rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
        }

        public static bool operator !=(FixedVector3 lhs, FixedVector3 rhs)
        {
            return lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z;
        }

        public static Fixed64 SqrMagnitude(FixedVector3 a)
        {
            return a.x * a.x + a.y * a.y + a.z * a.z;
        }

        public static Fixed64 Distance(FixedVector3 a, FixedVector3 b)
        {
            return Magnitude(a - b);
        }

        public static Fixed64 Magnitude(FixedVector3 a)
        {
            return Fixed64.Sqrt(FixedVector3.SqrMagnitude(a));
        }

        public void Normalize()
        {
            Fixed64 n = x * x + y * y + z * z;
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
            z *= n;
        }

        public FixedVector3 GetNormalized()
        {
            FixedVector3 v = new FixedVector3(this);
            v.Normalize();
            return v;
        }

        public override string ToString()
        {
            return string.Format("x:{0} y:{1} z:{2}", x, y, z);
        }

        public override bool Equals(object obj)
        {
            return obj is FixedVector2 && ((FixedVector3) obj) == this;
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() + this.y.GetHashCode() + this.z.GetHashCode();
        }

        public static FixedVector3 Lerp(FixedVector3 from, FixedVector3 to, Fixed64 factor)
        {
            return from * (1 - factor) + to * factor;
        }
        public UnityEngine.Vector3 ToVector3()
        {
            return new UnityEngine.Vector3((float)x, (float)y, (float)z);
        }
    }
}