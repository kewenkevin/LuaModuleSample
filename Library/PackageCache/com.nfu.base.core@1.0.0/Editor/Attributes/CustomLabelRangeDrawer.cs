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
//     File Name           :        CustomLabelRangeDrawer.cs
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
using UnityEditor;
using UnityEngine;

namespace ND.Core.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(CustomLabelRangeAttribute))]
    public class CustomLabelRangeDrawer: PropertyDrawer
    {
        private GUIContent _label = null;
    
        private float min = 0;
        private float max = 0;
        private bool isInt = false;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_label == null)
            {
                string name = (attribute as CustomLabelRangeAttribute).name;
                _label = new GUIContent(name);
                min = (attribute as CustomLabelRangeAttribute).min;
                max = (attribute as CustomLabelRangeAttribute).max;
            }

            if (property.propertyType == SerializedPropertyType.Integer)
            {
                label = EditorGUI.BeginProperty(position, _label, property);
                EditorGUI.BeginChangeCheck();
                float num = EditorGUI.IntSlider(position, _label, property.intValue, Mathf.CeilToInt(min), Mathf.CeilToInt(max));
                if (EditorGUI.EndChangeCheck())
                    property.floatValue = num;
                EditorGUI.EndProperty();
            }
            else if (property.propertyType == SerializedPropertyType.Float)
                EditorGUI.Slider(position, property, min, max,_label);
            else 
                EditorGUI.PropertyField(position, property, _label);
        }
    }
}