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
//     File Name           :        StringExtension.cs
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


namespace ND.Core.Extensions.CSharp
{
    public static class StringExtension
    {

        #region Basic
        
        public static bool IsEmpty(this string str)
        {
            return str == null || str.Length <= 0;
        }

        public static bool IsNotEmpty(this string str)
        {
            return str != null && str.Length > 0;
        }
        
        public static string Format(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static string Clone(this string value)
        {
            if (null == value || value.Length <= 0)
            {
                return string.Empty;
            }

            return string.Copy(value);
        }
        #endregion

       
        
    }
}