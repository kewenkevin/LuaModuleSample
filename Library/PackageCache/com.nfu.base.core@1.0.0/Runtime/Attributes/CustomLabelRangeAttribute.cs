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
//     File Name           :        CustomLabelRangeAttribute.cs
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
using UnityEngine;

namespace ND.Core.Attributes
{
    public class CustomLabelRangeAttribute : PropertyAttribute
    {
        public readonly string name;
    
        public readonly float min;
        public readonly float max;

        /// <summary>
        ///   <para>Attribute used to make a float or int variable in a script be restricted to a specific range.</para>
        /// </summary>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        public CustomLabelRangeAttribute(string name,float min, float max)
        {
            this.min = min;
            this.max = max;
            this.name = name;
        }
    
        public CustomLabelRangeAttribute(string name,int min, int max)
        {
            this.min = min;
            this.max = max;
            this.name = name;
        }
    }
}