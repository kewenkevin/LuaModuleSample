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
//     File Name           :        EncodeExtension.cs
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
using System.Security.Cryptography;
using System.Text;
using ND.Core.Common;

namespace ND.Core.Extensions.CSharp
{
    public static class EncodeExtension
    {
        public static string GetMD5(this string str)
        {
            string cl = str;
            StringBuilder pwd = new StringBuilder();
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
                pwd.Append(s[i].ToString("X2"));
            }
            return pwd.ToString();
        }
        
        public static Dictionary<string, string> GetUrlParamDict(this string parmStr)
        {
            List<string> parmList = parmStr.ToStringList("&");
            Dictionary<string, string> kVDict = new Dictionary<string, string>();
            for (int i = 0; i < parmList.Count; ++i)
            {
                List<string> parms = parmList[i].ToStringList("=");
                if (parms.Count == 2)
                {
                    kVDict.Add(parms[0], parms[1]);
                }
            }
            return kVDict;
        }
        
        public static string UrlEncode(this string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }
            return (sb.ToString());
        }


    }
}