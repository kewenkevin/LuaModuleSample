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
//     File Name           :        Fixed64.cs
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
using System;

namespace ND.Core.DataStruct
{
    public partial struct Fixed64 : IEquatable<Fixed64>, IComparable<Fixed64>
    {
        readonly long m_rawValue;

        public static readonly decimal Precision = (decimal) (new Fixed64(1L));
        public static readonly Fixed64 One = new Fixed64(ONE);
        public static readonly Fixed64 Zero = new Fixed64();
        public static readonly Fixed64 PI = new Fixed64(Pi);
        public static readonly Fixed64 PITimes2 = new Fixed64(PiTimes2);
        public static readonly Fixed64 PIOver180 = new Fixed64((long) 72);
        public static readonly Fixed64 Rad2Deg = Fixed64.Pi * (Fixed64) 2 / (Fixed64) 360;
        public static readonly Fixed64 Deg2Rad = (Fixed64) 360 / (Fixed64.Pi * (Fixed64) 2);

        const long Pi = 12868;
        const long PiTimes2 = 25736;

        public const int FRACTIONAL_PLACES = 12;
        const long ONE = 1L << FRACTIONAL_PLACES;

        public static int Sign(Fixed64 value)
        {
            return
                value.m_rawValue < 0 ? -1 :
                value.m_rawValue > 0 ? 1 :
                0;
        }

        public static Fixed64 Abs(Fixed64 value)
        {
            return new Fixed64(value.m_rawValue > 0 ? value.m_rawValue : -value.m_rawValue);
        }

        public static Fixed64 Floor(Fixed64 value)
        {
            return new Fixed64((long) ((ulong) value.m_rawValue & 0xFFFFFFFFFFFFF000));
        }

        public static Fixed64 Ceiling(Fixed64 value)
        {
            var hasFractionalPart = (value.m_rawValue & 0x0000000000000FFF) != 0;
            return hasFractionalPart ? Floor(value) + One : value;
        }

        public static Fixed64 operator +(Fixed64 x, Fixed64 y)
        {
            return new Fixed64(x.m_rawValue + y.m_rawValue);
        }

        public static Fixed64 operator +(Fixed64 x, int y)
        {
            return x + (Fixed64) y;
        }

        public static Fixed64 operator +(int x, Fixed64 y)
        {
            return (Fixed64) x + y;
        }

        public static Fixed64 operator +(Fixed64 x, float y)
        {
            return x + (Fixed64) y;
        }

        public static Fixed64 operator +(float x, Fixed64 y)
        {
            return (Fixed64) x + y;
        }

        public static Fixed64 operator +(Fixed64 x, double y)
        {
            return x + (Fixed64) y;
        }

        public static Fixed64 operator +(double x, Fixed64 y)
        {
            return (Fixed64) x + y;
        }

        public static Fixed64 operator -(Fixed64 x, Fixed64 y)
        {
            return new Fixed64(x.m_rawValue - y.m_rawValue);
        }

        public static Fixed64 operator -(Fixed64 x, int y)
        {
            return x - (Fixed64) y;
        }

        public static Fixed64 operator -(int x, Fixed64 y)
        {
            return (Fixed64) x - y;
        }

        public static Fixed64 operator -(Fixed64 x, float y)
        {
            return x - (Fixed64) y;
        }

        public static Fixed64 operator -(float x, Fixed64 y)
        {
            return (Fixed64) x + y;
        }

        public static Fixed64 operator -(Fixed64 x, double y)
        {
            return x - (Fixed64) y;
        }

        public static Fixed64 operator -(double x, Fixed64 y)
        {
            return (Fixed64) x - y;
        }


        public static Fixed64 operator *(Fixed64 x, Fixed64 y)
        {
            return new Fixed64((x.m_rawValue * y.m_rawValue) >> FRACTIONAL_PLACES);
        }

        public static Fixed64 operator *(Fixed64 x, int y)
        {
            return x * (Fixed64) y;
        }

        public static Fixed64 operator *(int x, Fixed64 y)
        {
            return (Fixed64) x * y;
        }

        public static Fixed64 operator *(Fixed64 x, float y)
        {
            return x * (Fixed64) y;
        }

        public static Fixed64 operator *(float x, Fixed64 y)
        {
            return (Fixed64) x * y;
        }

        public static Fixed64 operator *(Fixed64 x, double y)
        {
            return x * (Fixed64) y;
        }

        public static Fixed64 operator *(double x, Fixed64 y)
        {
            return (Fixed64) x * y;
        }

        public static Fixed64 operator /(Fixed64 x, Fixed64 y)
        {
            return new Fixed64((x.m_rawValue << FRACTIONAL_PLACES) / y.m_rawValue);
        }

        public static Fixed64 operator /(Fixed64 x, int y)
        {
            return x / (Fixed64) y;
        }

        public static Fixed64 operator /(int x, Fixed64 y)
        {
            return (Fixed64) x / y;
        }

        public static Fixed64 operator /(Fixed64 x, float y)
        {
            return x / (Fixed64) y;
        }

        public static Fixed64 operator /(float x, Fixed64 y)
        {
            return (Fixed64) x / y;
        }

        public static Fixed64 operator /(double x, Fixed64 y)
        {
            return (Fixed64) x / y;
        }

        public static Fixed64 operator /(Fixed64 x, double y)
        {
            return x / (Fixed64) y;
        }

        public static Fixed64 operator %(Fixed64 x, Fixed64 y)
        {
            return new Fixed64(x.m_rawValue % y.m_rawValue);
        }

        public static Fixed64 operator -(Fixed64 x)
        {
            return new Fixed64(-x.m_rawValue);
        }

        public static bool operator ==(Fixed64 x, Fixed64 y)
        {
            return x.m_rawValue == y.m_rawValue;
        }

        public static bool operator !=(Fixed64 x, Fixed64 y)
        {
            return x.m_rawValue != y.m_rawValue;
        }

        public static bool operator >(Fixed64 x, Fixed64 y)
        {
            return x.m_rawValue > y.m_rawValue;
        }

        public static bool operator >(Fixed64 x, int y)
        {
            return x > (Fixed64) y;
        }

        public static bool operator <(Fixed64 x, int y)
        {
            return x < (Fixed64) y;
        }

        public static bool operator >(Fixed64 x, float y)
        {
            return x > (Fixed64) y;
        }

        public static bool operator <(Fixed64 x, float y)
        {
            return x < (Fixed64) y;
        }

        public static bool operator <(Fixed64 x, Fixed64 y)
        {
            return x.m_rawValue < y.m_rawValue;
        }

        public static bool operator >=(Fixed64 x, Fixed64 y)
        {
            return x.m_rawValue >= y.m_rawValue;
        }

        public static bool operator <=(Fixed64 x, Fixed64 y)
        {
            return x.m_rawValue <= y.m_rawValue;
        }

        public static Fixed64 operator >>(Fixed64 x, int amount)
        {
            return new Fixed64(x.RawValue >> amount);
        }

        public static Fixed64 operator <<(Fixed64 x, int amount)
        {
            return new Fixed64(x.RawValue << amount);
        }


        public static explicit operator Fixed64(long value)
        {
            return new Fixed64(value * ONE);
        }

        public static explicit operator long(Fixed64 value)
        {
            return value.m_rawValue >> FRACTIONAL_PLACES;
        }

        public static explicit operator Fixed64(float value)
        {
            return new Fixed64((long) (value * ONE));
        }

        public static explicit operator float(Fixed64 value)
        {
            return (float) value.m_rawValue / ONE;
        }

        public static explicit operator int(Fixed64 value)
        {
            return (int) ((float) value);
        }

        public static explicit operator Fixed64(double value)
        {
            return new Fixed64((long) (value * ONE));
        }

        public static explicit operator double(Fixed64 value)
        {
            return (double) value.m_rawValue / ONE;
        }

        public static explicit operator Fixed64(decimal value)
        {
            return new Fixed64((long) (value * ONE));
        }

        public static explicit operator decimal(Fixed64 value)
        {
            return (decimal) value.m_rawValue / ONE;
        }

        public override bool Equals(object obj)
        {
            return obj is Fixed64 && ((Fixed64) obj).m_rawValue == m_rawValue;
        }

        public override int GetHashCode()
        {
            return m_rawValue.GetHashCode();
        }

        public bool Equals(Fixed64 other)
        {
            return m_rawValue == other.m_rawValue;
        }

        public int CompareTo(Fixed64 other)
        {
            return m_rawValue.CompareTo(other.m_rawValue);
        }

        public override string ToString()
        {
            return ((decimal) this).ToString();
        }

        public string ToStringRound(int round = 2)
        {
            return (float) Math.Round((float) this, round) + "";
        }

        public static Fixed64 FromRaw(long rawValue)
        {
            return new Fixed64(rawValue);
        }

        public static Fixed64 Pow(Fixed64 x, int y)
        {
            if (y == 1) return x;
            Fixed64 result = Fixed64.Zero;
            Fixed64 tmp = Pow(x, y / 2);
            if ((y & 1) != 0) //奇数    
            {
                result = x * tmp * tmp;
            }
            else
            {
                result = tmp * tmp;
            }

            return result;
        }


        public long RawValue
        {
            get { return m_rawValue; }
        }


        Fixed64(long rawValue)
        {
            m_rawValue = rawValue;
        }

        public Fixed64(int value)
        {
            m_rawValue = value * ONE;
        }

        public static Fixed64 Sqrt(Fixed64 f, int numberIterations)
        {
            if (f.RawValue < 0)
            {
                throw new ArithmeticException("sqrt error");
            }

            if (f.RawValue == 0)
                return Fixed64.Zero;

            Fixed64 k = f + Fixed64.One >> 1;
            for (int i = 0; i < numberIterations; i++)
                k = (k + (f / k)) >> 1;

            if (k.RawValue < 0)
                throw new ArithmeticException("Overflow");
            else
                return k;
        }

        public static Fixed64 Sqrt(Fixed64 f)
        {
            byte numberOfIterations = 8;
            if (f.RawValue > 0x64000)
                numberOfIterations = 12;
            if (f.RawValue > 0x3e8000)
                numberOfIterations = 16;
            return Sqrt(f, numberOfIterations);
        }


        #region Sin

        public static Fixed64 Sin(Fixed64 i)
        {
            Fixed64 j = (Fixed64) 0;
            for (; i < Fixed64.Zero; i += Fixed64.FromRaw(25736)) ;
            if (i > Fixed64.FromRaw(25736))
                i %= Fixed64.FromRaw(25736);
            Fixed64 k = (i * Fixed64.FromRaw(10)) / Fixed64.FromRaw(714);
            if (i != Fixed64.Zero && i != Fixed64.FromRaw(6434) && i != Fixed64.FromRaw(12868) &&
                i != Fixed64.FromRaw(19302) && i != Fixed64.FromRaw(25736))
                j = (i * Fixed64.FromRaw(100)) / Fixed64.FromRaw(714) - k * Fixed64.FromRaw(10);
            if (k <= Fixed64.FromRaw(90))
                return sin_lookup(k, j);
            if (k <= Fixed64.FromRaw(180))
                return sin_lookup(Fixed64.FromRaw(180) - k, j);
            if (k <= Fixed64.FromRaw(270))
                return -sin_lookup(k - Fixed64.FromRaw(180), j);
            else
                return -sin_lookup(Fixed64.FromRaw(360) - k, j);
        }

        private static Fixed64 sin_lookup(Fixed64 i, Fixed64 j)
        {
            if (j > 0 && j < Fixed64.FromRaw(10) && i < Fixed64.FromRaw(90))
                return Fixed64.FromRaw(SIN_TABLE[i.RawValue]) +
                       ((Fixed64.FromRaw(SIN_TABLE[i.RawValue + 1]) - Fixed64.FromRaw(SIN_TABLE[i.RawValue])) /
                        Fixed64.FromRaw(10)) * j;
            else
                return Fixed64.FromRaw(SIN_TABLE[i.RawValue]);
        }

        private static int[] SIN_TABLE =
        {
            0, 71, 142, 214, 285, 357, 428, 499, 570, 641,
            711, 781, 851, 921, 990, 1060, 1128, 1197, 1265, 1333,
            1400, 1468, 1534, 1600, 1665, 1730, 1795, 1859, 1922, 1985,
            2048, 2109, 2170, 2230, 2290, 2349, 2407, 2464, 2521, 2577,
            2632, 2686, 2740, 2793, 2845, 2896, 2946, 2995, 3043, 3091,
            3137, 3183, 3227, 3271, 3313, 3355, 3395, 3434, 3473, 3510,
            3547, 3582, 3616, 3649, 3681, 3712, 3741, 3770, 3797, 3823,
            3849, 3872, 3895, 3917, 3937, 3956, 3974, 3991, 4006, 4020,
            4033, 4045, 4056, 4065, 4073, 4080, 4086, 4090, 4093, 4095,
            4096
        };

        #endregion


        #region Cos, Tan, Asin

        public static Fixed64 Cos(Fixed64 i)
        {
            return Sin(i + Fixed64.FromRaw(6435));
        }

        public static Fixed64 Tan(Fixed64 i)
        {
            return Sin(i) / Cos(i);
        }

        public static Fixed64 Asin(Fixed64 F)
        {
            bool isNegative = F < 0;
            F = Abs(F);

            if (F > Fixed64.One)
                throw new ArithmeticException("Bad Asin Input:" + (double) F);

            Fixed64 f1 = ((((Fixed64.FromRaw(145103 >> FRACTIONAL_PLACES) * F) -
                          Fixed64.FromRaw(599880 >> FRACTIONAL_PLACES) * F) +
                         Fixed64.FromRaw(1420468 >> FRACTIONAL_PLACES) * F) -
                        Fixed64.FromRaw(3592413 >> FRACTIONAL_PLACES) * F) +
                       Fixed64.FromRaw(26353447 >> FRACTIONAL_PLACES);
            Fixed64 f2 = PI / (Fixed64) 2 - (Sqrt(Fixed64.One - F) * f1);

            return isNegative ? -f2 : f2;
        }

        #endregion

        #region ATan, ATan2

        public static Fixed64 Atan(Fixed64 F)
        {
            return Asin(F / Sqrt(Fixed64.One + (F * F)));
        }

        public static Fixed64 Atan2(Fixed64 F1, Fixed64 F2)
        {
            if (F2.RawValue == 0 && F1.RawValue == 0)
                return (Fixed64) 0;

            Fixed64 result = (Fixed64) 0;
            if (F2 > 0)
                result = Atan(F1 / F2);
            else if (F2 < 0)
            {
                if (F1 >= (Fixed64) 0)
                    result = (PI - Atan(Abs(F1 / F2)));
                else
                    result = -(PI - Atan(Abs(F1 / F2)));
            }
            else
                result = (F1 >= (Fixed64) 0 ? PI : -PI) / (Fixed64) 2;

            return result;
        }

        #endregion
    }

}