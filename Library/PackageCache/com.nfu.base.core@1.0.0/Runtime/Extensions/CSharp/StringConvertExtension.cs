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
//     File Name           :        StringConvertExtension.cs
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


using System;
using System.Collections.Generic;
using UnityEngine;

namespace ND.Core.Common
{
    public static class StringConvertExtension
    {
        /// <summary>
        /// 字符串转int
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToInt(this string str)
        {
            int temp = 0;
            int.TryParse(str, out temp);
            return temp;
        }

        /// <summary>
        /// 字符串转Float
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static float ToFloat(this string str)
        {
            float temp = 0;
            float.TryParse(str, out temp);
            return temp;
        }


        public static bool ToBool(this string value)
        {
            bool ret = false;
            if (value.ToInt() > 0)
            {
                return true;
            }
            else
            {
                bool.TryParse(value, out ret);
            }

            return ret;
        }

        public static long ToInt64(this string value)
        {
            Int64 ret = 0;
            Int64.TryParse(value, out ret);

            return ret;
        }

        public static Vector2 ToVector2(this string pos, char split = ',')
        {
            if (string.IsNullOrEmpty(pos))
            {
                return Vector2.zero;
            }

            char[] splits = new char[1] {split};
            string[] str = pos.Split(splits);

            float x = str.Length > 0 ? str[0].ToFloat() : 0f;
            float y = str.Length > 1 ? str[1].ToFloat() : 0f;

            Vector2 ret = new Vector2(x, y);

            return ret;
        }

        /// <summary>
        /// 字符串转Vector3
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Vector3 ToVector3(string pos, char split = ',')
        {
            if (string.IsNullOrEmpty(pos))
            {
                return Vector3.zero;
            }

            char[] splits = new char[1] {split};
            string[] str = pos.Split(splits);

            float x = str.Length > 0 ? str[0].ToFloat() : 0f;
            float y = str.Length > 1 ? str[1].ToFloat() : 0f;
            float z = str.Length > 2 ? str[2].ToFloat() : 0f;

            Vector3 ret = new Vector3(x, y, z);

            return ret;
        }

        public static Color ToColor(string value, char split)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Color.white;
            }

            char[] splits = new char[1] {split};
            string[] str = value.Split(splits);

            float r = str.Length > 0 ? str[0].ToInt() / 255.0f : 0f;
            float g = str.Length > 1 ? str[1].ToInt() / 255.0f : 0f;
            float b = str.Length > 2 ? str[2].ToInt() / 255.0f : 0f;
            float a = str.Length > 3 ? str[3].ToInt() / 255.0f : 1f;

            Color ret = new Color(r, g, b, a);

            return ret;
        }

        public static Color ToColorHex(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Color.white;
            }

            int length = value.Length;
            int strCount = length / 2;
            if (length % 2 != 0)
            {
                ++strCount;
            }

            string[] str = new string[strCount];
            for (int i = 0; i < strCount; ++i)
            {
                int leftLength = Mathf.Min(2, length - i * 2);
                str[i] = value.Substring(i * 2, leftLength);
            }

            try
            {
                float r = str.Length > 0 ? Convert.ToInt16(str[0], 16) / 255.0f : 0f;
                float g = str.Length > 1 ? Convert.ToInt16(str[1], 16) / 255.0f : 0f;
                float b = str.Length > 2 ? Convert.ToInt16(str[2], 16) / 255.0f : 0f;
                float a = str.Length > 3 ? Convert.ToInt16(str[3], 16) / 255.0f : 1f;

                Color ret = new Color(r, g, b, a);

                return ret;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return Color.white;
        }


        public static int[] ToIntArray(this string data, string split = "-")
        {
            string[] strArray = data.Split(split[0]);

            int[] ret = new int[strArray.Length];

            for (int i = 0; i < strArray.Length; ++i)
            {
                ret[i] = strArray[i].ToInt();
            }

            return ret;
        }

        public static object[] ToObjectArray(this string value, string splitter = ";")
        {
            if (string.IsNullOrEmpty(value) || splitter == null)
            {
                return new string[0];
            }

            string[] strArray = value.Split(splitter[0]);
            return strArray;
        }


        public static List<int> ToIntList(this string value, string splitter = ";")
        {
            List<int> list = new List<int>();

            if (string.IsNullOrEmpty(value) || splitter == null)
            {
                return list;
            }

            string[] strArray = value.Split(splitter[0]);

            for (int i = 0; i < strArray.Length; i++)
            {
                list.Add(strArray[i].ToInt());
            }

            return list;
        }

        public static List<float> ToFloatList(this string value, string splitter = ";")
        {
            List<float> list = new List<float>();

            if (string.IsNullOrEmpty(value) || splitter == null)
            {
                return list;
            }

            string[] strArray = value.Split(splitter[0]);

            for (int i = 0; i < strArray.Length; i++)
            {
                list.Add(strArray[i].ToFloat());
            }

            return list;
        }

        public static List<bool> ToBoolList(this string value, string splitter = ";")
        {
            List<bool> list = new List<bool>();
            if (string.IsNullOrEmpty(value) || splitter == null)
            {
                return list;
            }

            string[] strArray = value.Split(splitter[0]);

            for (int i = 0; i < strArray.Length; i++)
            {
                list.Add(strArray[i].ToBool());
            }

            return list;
        }

        public static List<string> ToStringList(this string value, string splitter = ";")
        {
            List<string> list = new List<string>();

            if (string.IsNullOrEmpty(value) || splitter == null)
            {
                return list;
            }

            string[] strArray = value.Split(splitter[0]);

            for (int i = 0; i < strArray.Length; i++)
            {
                list.Add(strArray[i]);
            }

            return list;
        }
    }
}