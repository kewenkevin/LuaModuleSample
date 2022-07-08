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
//     File Name           :        EnumLabelDrawer.cs
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
using System;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace ND.Core.Attributes.Editor
{
    public class EnumLabel
    {
        public static object GetEnum(Type type, SerializedObject serializedObject, string path)
        {
            SerializedProperty property = GetPropety(serializedObject, path);
            return System.Enum.GetValues(type).GetValue(property.enumValueIndex);
        }

        public static object DrawEnum(Type type, SerializedObject serializedObject, string path)
        {
            return DrawEnum(type, serializedObject, GetPropety(serializedObject, path));
        }

        public static object DrawEnum(Type type, SerializedObject serializedObject, SerializedProperty property)
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(property);
            serializedObject.ApplyModifiedProperties();
            return System.Enum.GetValues(type).GetValue(property.enumValueIndex);
        }

        public static SerializedProperty GetPropety(SerializedObject serializedObject, string path)
        {
            string[] contents = path.Split('/');
            SerializedProperty property = serializedObject.FindProperty(contents[0]);
            for (int i = 1; i < contents.Length; i++)
            {
                property = property.FindPropertyRelative(contents[i]);
            }

            return property;
        }
    }

    [CustomPropertyDrawer(typeof(EnumLabelAttribute))]
    public class EnumLabelDrawer : PropertyDrawer
    {
        private Dictionary<string, string> customEnumNames = new Dictionary<string, string>();


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SetUpCustomEnumNames(property, property.enumNames);

            if (property.propertyType == SerializedPropertyType.Enum)
            {
                EditorGUI.BeginChangeCheck();
                string[] displayedOptions = property.enumNames
                    .Where(enumName => customEnumNames.ContainsKey(enumName))
                    .Select<string, string>(enumName => customEnumNames[enumName])
                    .ToArray();

                int[] indexArray = GetIndexArray(enumLabelAttribute.order);
                if (indexArray.Length != displayedOptions.Length)
                {
                    indexArray = new int[displayedOptions.Length];
                    for (int i = 0; i < indexArray.Length; i++)
                    {
                        indexArray[i] = i;
                    }
                }

                string[] items = new string[displayedOptions.Length];
                items[0] = displayedOptions[0];
                for (int i = 0; i < displayedOptions.Length; i++)
                {
                    items[i] = displayedOptions[indexArray[i]];
                }

                int index = -1;
                for (int i = 0; i < indexArray.Length; i++)
                {
                    if (indexArray[i] == property.enumValueIndex)
                    {
                        index = i;
                        break;
                    }
                }

                if ((index == -1) && (property.enumValueIndex != -1))
                {
                    SortingError(position, property, label);
                    return;
                }

                index = EditorGUI.Popup(position, enumLabelAttribute.label, index, items);
                if (EditorGUI.EndChangeCheck())
                {
                    if (index >= 0)
                        property.enumValueIndex = indexArray[index];
                }
            }
        }

        private EnumLabelAttribute enumLabelAttribute
        {
            get { return (EnumLabelAttribute) attribute; }
        }

        public void SetUpCustomEnumNames(SerializedProperty property, string[] enumNames)
        {


            object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(EnumLabelAttribute), false);
            foreach (EnumLabelAttribute customAttribute in customAttributes)
            {
                Type enumType = fieldInfo.FieldType;

                foreach (string enumName in enumNames)
                {
                    FieldInfo field = enumType.GetField(enumName);
                    if (field == null) continue;
                    EnumLabelAttribute[] attrs =
                        (EnumLabelAttribute[]) field.GetCustomAttributes(customAttribute.GetType(), false);

                    if (!customEnumNames.ContainsKey(enumName))
                    {
                        foreach (EnumLabelAttribute labelAttribute in attrs)
                        {
                            customEnumNames.Add(enumName, labelAttribute.label);
                        }
                    }
                }
            }
        }


        int[] GetIndexArray(int[] order)
        {
            int[] indexArray = new int[order.Length];
            for (int i = 0; i < order.Length; i++)
            {
                int index = 0;
                for (int j = 0; j < order.Length; j++)
                {
                    if (order[i] > order[j])
                    {
                        index++;
                    }
                }

                indexArray[i] = index;
            }

            return (indexArray);
        }

        void SortingError(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, new GUIContent(label.text + " (sorting error)"));
            EditorGUI.EndProperty();
        }
    }
}