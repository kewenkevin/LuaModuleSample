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
//     File Name           :        ArrayExtension.cs
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
using System.Text;

namespace ND.Core.Extensions.CSharp
{
    /// <summary>
    /// Extension Some Useful Methods for CSharp Arrays
    /// </summary>
    public static class ArrayExtension
    {
        /// <summary>
        /// Is array have any elements
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsEmpty<T>(this T[] list)
        {
            return list == null || list.Length == 0;
        }


        /// <summary>
        /// Is array not have any elements
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsNotEmpty<T>(this T[] list)
        {
            return list != null && list.Length > 0;
        }

        /// <summary>
        /// Peek an element from the array
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetAt<T>(this T[] list, int index)
        {
            if (list.IsEmpty() || index < 0 || index >= list.Length)
                return default(T);
            return list[index];
        }
        
        
        /// <summary>
        /// convert a array to an string like "1;2;3;4;5"
        /// </summary>
        /// <param name="list"></param>
        /// <param name="split">split symbol</param>
        /// <returns></returns>
        public static string ToString<T>(this List<T> list, char split = ';')
        {
            if (null == list || list.Count <= 0)
            {
                return "";
            }

            StringBuilder data = new StringBuilder();

            for (int i = 0; i < list.Count; ++i)
            {
                data.Append(list[i]);

                if (i != list.Count - 1)
                {
                    data.Append(split);
                }
            }

            return data.ToString();
        }

        /// <summary>
        /// copy an array
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] Clone<T>(this T[] array)
        {
            T[] copy = new T[array.Length];
            array.CopyTo(copy, 0);
            return copy;
        }


        /// <summary>
        /// concat two array to a new array
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] Contact<T>(this T[] array1, T[] array2)
        {
            T[] bind = new T[array1.Length + array2.Length];
            array1.CopyTo(bind, 0);
            array2.CopyTo(bind, array1.Length);
            return bind;
        }

        /// <summary>
        /// get an sub array
        /// </summary>
        /// <param name="args"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static T[] SubArray<T>(this T[] args, int start, int end = -1)
        {
            if (null == args || start >= args.Length || start < 0)
            {
                return new T[0];
            }

            int count = end > 0 && end < args.Length ? end : args.Length - start;

            T[] ret = new T[count];
            Array.Copy(args, start, ret, 0, count);

            return ret;
        }
    }
}