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
//     File Name           :        RomanNumberExtension.cs
// 
//     Programmer          :        Wei Wei (Battle Mage Gandalf)
// 
//     Start Date          :        04/07/2021
// 
//     Last Update         :        04/07/2021 16:30 [Wei]
// 
//     Description         :        write here
// 
// =============================================================================================
// Contributors:
// ---------------------------------------------------------------------------------------------
// Battle Mage Gandalf                 wwei@yoozoo.com             Product technology Center
// =============================================================================================


using System.Collections.Generic;

namespace ND.Core.Extensions.CSharp
{
    /// <summary>
    /// Roman Number Support
    /// </summary>
    public static class RomanNumberExtension
    {
        /// <summary>
        /// int number convert to Roman String (positive only)
        /// </summary>
        /// <param name="num">number</param>
        /// <returns>roman string</returns>
        public static string ToRoman(this int num)
        {
            string res = string.Empty;
            List<int> val = new List<int> {1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1};
            List<string> str = new List<string> {"M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I"};
            for (int i = 0; i < val.Count; ++i)
            {
                while (num >= val[i])
                {
                    num -= val[i];
                    res += str[i];
                }
            }

            return res;
        }
        
        /// <summary>
        /// Roman String convert to int
        /// </summary>
        /// <param name="romanString"></param>
        /// <returns></returns>
        public static int RomanToInt(this string romanString)
        {
            Dictionary<char, int> dict = new Dictionary<char, int>
            {
                {'I', 1},
                {'V', 5},
                {'X', 10},
                {'L', 50},
                {'C', 100},
                {'D', 500},
                {'M', 1000}
            };
            int sum = 0;
            for (int i = 0; i < romanString.Length; i++)
            {
                int currentValue = dict[romanString[i]];
                if (i == romanString.Length - 1 || dict[romanString[i + 1]] <= currentValue)
                    sum += currentValue;
                else
                    sum -= currentValue;
            }
            return sum;
        }
    }
}