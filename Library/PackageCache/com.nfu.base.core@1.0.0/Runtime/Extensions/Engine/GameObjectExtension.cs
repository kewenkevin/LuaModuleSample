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
//     File Name           :        GameObjectExtension.cs
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

namespace ND.Core.Extensions.Engine
{
    public static class GameObjectExtension
    {
        public static void ActiveParent(this GameObject gameObject)
        {
            if (gameObject != null)
                gameObject.transform.parent.gameObject.SetActive(true);
        }

        public static void DeActiveParent(this GameObject gameObject)
        {
            if (gameObject != null)
                gameObject.transform.parent.gameObject.SetActive(false);
        }

        public static void Active(this GameObject gameObject, bool needDeactive = false)
        {
            if (gameObject != null)
            {
                if (needDeactive)
                {
                    gameObject.SetActive(false);
                }

                gameObject.SetActive(true);
            }
        }

        public static void DeActive(this GameObject gameObject)
        {
            if (gameObject != null)
                gameObject.SetActive(false);
        }

        public static void Destroy(this GameObject gameObject, float t = 0)
        {
            if (gameObject != null)
                GameObject.Destroy(gameObject, t);
        }
        
        
        
        
        
        public static T FindComponent<T>(this GameObject obj) where T : MonoBehaviour
        {
            T t = obj.GetComponent<T>();
            if (t == null)
            {
                t = obj.AddComponent<T>();
            }
            return t;
        }

        /// <summary>
        /// 设置物体的父物体
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="parent"></param>
        public static void SetParent(this GameObject obj, Transform parent, bool worldPositionStays = false)
        {
            obj.transform.SetParent(parent, worldPositionStays);
        }
	
	
        public static void Align(this Transform obj, Transform target , bool worldSpace = false)
        {
            if (worldSpace)
            {
                obj.position = target.position;
                obj.rotation = target.rotation;
                obj.localScale = target.localScale;
            }
            else
            {
                obj.localPosition = target.localPosition;
                obj.localRotation = target.localRotation;
                obj.localScale = target.localScale;
            }
        }
        
        
        
    }
}