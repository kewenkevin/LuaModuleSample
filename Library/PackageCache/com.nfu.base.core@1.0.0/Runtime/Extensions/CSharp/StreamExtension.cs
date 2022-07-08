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
//     File Name           :        StreamExtension.cs
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


using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text;

namespace ND.Core.Extensions.CSharp
{


//数据转换
    public static class StreamExtension
    {
        #region Short

        /// <summary>
        /// 从流中读取一个short数据
        /// </summary>
        /// <returns></returns>
        public static short ReadShort(this Stream stream)
        {
            byte[] arr = new byte[2];
            stream.Read(arr, 0, 2);
            return BitConverter.ToInt16(arr, 0);
        }

        /// <summary>
        /// 把一个short数据写入流
        /// </summary>
        /// <param name="vaule"></param>
        public static void WriteShort(this Stream stream, short vaule)
        {
            byte[] arr = BitConverter.GetBytes(vaule);
            stream.Write(arr, 0, arr.Length);
        }

        #endregion

        #region UShort

        /// <summary>
        /// 从流中读取一个short数据
        /// </summary>
        /// <returns></returns>
        public static ushort ReadUShort(this Stream stream)
        {
            byte[] arr = new byte[2];
            stream.Read(arr, 0, 2);
            return BitConverter.ToUInt16(arr, 0);
        }

        /// <summary>
        /// 把一个short数据写入流
        /// </summary>
        /// <param name="vaule"></param>
        public static void WriteUShort(this Stream stream, ushort vaule)
        {
            byte[] arr = BitConverter.GetBytes(vaule);
            stream.Write(arr, 0, arr.Length);
        }

        #endregion

        #region Int

        /// <summary>
        /// 从流中读取一个short数据
        /// </summary>
        /// <returns></returns>
        public static int ReadInt(this Stream stream)
        {
            byte[] arr = new byte[4];
            stream.Read(arr, 0, 4);
            return BitConverter.ToInt32(arr, 0);
        }

        /// <summary>
        /// 把一个short数据写入流
        /// </summary>
        /// <param name="vaule"></param>
        public static void WriteInt(this Stream stream, int vaule)
        {
            byte[] arr = BitConverter.GetBytes(vaule);
            stream.Write(arr, 0, arr.Length);
        }

        #endregion

        #region UInt

        /// <summary>
        /// 从流中读取一个short数据
        /// </summary>
        /// <returns></returns>
        public static uint ReadUInt(this Stream stream)
        {
            byte[] arr = new byte[4];
            stream.Read(arr, 0, 4);
            return BitConverter.ToUInt32(arr, 0);
        }

        /// <summary>
        /// 把一个short数据写入流
        /// </summary>
        /// <param name="vaule"></param>
        public static void WriteUInt(this Stream stream, uint vaule)
        {
            byte[] arr = BitConverter.GetBytes(vaule);
            stream.Write(arr, 0, arr.Length);
        }

        #endregion

        #region Long

        /// <summary>
        /// 从流中读取一个short数据
        /// </summary>
        /// <returns></returns>
        public static long ReadLong(this Stream stream)
        {
            byte[] arr = new byte[8];
            stream.Read(arr, 0, 8);
            return BitConverter.ToInt64(arr, 0);
        }

        /// <summary>
        /// 把一个short数据写入流
        /// </summary>
        /// <param name="vaule"></param>
        public static void WriteLong(this Stream stream, long vaule)
        {
            byte[] arr = BitConverter.GetBytes(vaule);
            stream.Write(arr, 0, arr.Length);
        }

        #endregion

        #region ULong

        /// <summary>
        /// 从流中读取一个short数据
        /// </summary>
        /// <returns></returns>
        public static ulong ReadULong(this Stream stream)
        {
            byte[] arr = new byte[4];
            stream.Read(arr, 0, 4);
            return BitConverter.ToUInt64(arr, 0);
        }

        /// <summary>
        /// 把一个short数据写入流
        /// </summary>
        /// <param name="vaule"></param>
        public static void WriteULong(this Stream stream, ulong vaule)
        {
            byte[] arr = BitConverter.GetBytes(vaule);
            stream.Write(arr, 0, arr.Length);
        }

        #endregion

        #region Float

        /// <summary>
        /// 从流中读取一个short数据
        /// </summary>
        /// <returns></returns>
        public static float ReadFloat(this Stream stream)
        {
            byte[] arr = new byte[4];
            stream.Read(arr, 0, 4);
            return BitConverter.ToSingle(arr, 0);
        }

        /// <summary>
        /// 把一个short数据写入流
        /// </summary>
        /// <param name="vaule"></param>
        public static void WriteFloat(this Stream stream, float vaule)
        {
            byte[] arr = BitConverter.GetBytes(vaule);
            stream.Write(arr, 0, arr.Length);
        }

        #endregion

        #region Double

        /// <summary>
        /// 从流中读取一个short数据
        /// </summary>
        /// <returns></returns>
        public static double ReadDouble(this Stream stream)
        {
            byte[] arr = new byte[8];
            stream.Read(arr, 0, 8);
            return BitConverter.ToDouble(arr, 0);
        }

        /// <summary>
        /// 把一个short数据写入流
        /// </summary>
        /// <param name="vaule"></param>
        public static void WriteDouble(this Stream stream, double vaule)
        {
            byte[] arr = BitConverter.GetBytes(vaule);
            stream.Write(arr, 0, arr.Length);
        }

        #endregion

        #region Bool

        /// <summary>
        /// 从流中读取一个short数据
        /// </summary>
        /// <returns></returns>
        public static bool ReadBool(this Stream stream)
        {
            return stream.ReadByte() == 1;
        }

        /// <summary>
        /// 把一个short数据写入流
        /// </summary>
        /// <param name="vaule"></param>
        public static void WriteBool(this Stream stream, bool vaule)
        {
            stream.WriteByte((byte) (vaule == true ? 1 : 0));
        }

        #endregion

        #region String

        /// <summary>
        /// 从流中读取一个short数据
        /// </summary>
        /// <returns></returns>
        public static string ReadUTF8String(this Stream stream)
        {
            ushort len = stream.ReadUShort();
            byte[] arr = new byte[len];
            stream.Read(arr, 0, len);
            return Encoding.UTF8.GetString(arr);
        }

        /// <summary>
        /// 把一个short数据写入流
        /// </summary>
        /// <param name="vaule"></param>
        public static void WriteString(this Stream stream, string vaule)
        {
            byte[] arr = Encoding.UTF8.GetBytes(vaule);
            if (arr.Length > 65535)
            {
                throw new InvalidCastException("字符串超出范围");
            }

            stream.WriteUShort((ushort) arr.Length);
            stream.Write(arr, 0, arr.Length);
        }

        #endregion
    }
}