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
//     File Name           :        ComponentExtension.cs
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
    public static class ComponentExtension
    {
        public static void SetLocalPosition(this Component com, Vector3 pos)
        {
            if (com != null)
                com.transform.localPosition = pos;
        }

        public static void SetLocalPositionX(this Component com, float x)
        {
            if (com != null)
                com.transform.SetLocalPositionX(x);
        }

        public static void SetLocalPositionY(this Component com, float y)
        {
            if (com != null)
                com.transform.SetLocalPositionY(y);
        }

        public static void SetLocalPositionZ(this Component com, float z)
        {
            if (com != null)
                com.transform.SetLocalPositionZ(z);
        }



        public static void Destroy(this Component com, float t = 0)
        {
            if (com != null)
                GameObject.Destroy(com, t);
        }

        public static void DestroyGameObject(this Component com, float t = 0)
        {
            if (com != null)
                GameObject.Destroy(com.gameObject, t);
        }

        public static void SetActive(this Component com, bool isActive)
        {
            if (com != null)
                com.gameObject.SetActive(isActive);
        }

        /// <summary>
        /// 目标Component的gameObject激活
        /// </summary>
        /// <param name="com">目标Component</param>
        /// <param name="needDeActive">激活前是否尝试强制反激活一次</param>
        public static void Active(this Component com, bool needDeActive = false)
        {
            if (com != null)
            {
                if (needDeActive)
                {
                    com.DeActive();
                }

                com.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 目标Component的gameObject反激活
        /// </summary>
        /// <param name="com">目标Component</param>
        public static void DeActive(this Component com)
        {
            if (com != null)
                com.gameObject.SetActive(false);
        }



        public static void ActiveParent(this Component com, bool needDeActive = false)
        {
            if (com != null)
                com.transform.parent.Active(needDeActive);
        }

        public static void DeActiveParent(this Component com)
        {
            if (com != null)
                com.transform.parent.DeActive();
        }

        public static void ActiveChild(this Component com, int index, bool needDeactive = false)
        {
            if (com != null)
            {
                var child = com.transform.GetChild(index);
                if (child != null)
                {
                    if (needDeactive)
                    {
                        child.gameObject.SetActive(false);
                    }

                    child.gameObject.SetActive(true);
                }
            }
        }

        
        /// <summary>
        /// Deactive 所有目标index子对象，如果不是目标index，则不改变状态
        /// </summary>
        /// <param name="com">父组件对象</param>
        /// <param name="index">目标索引位置</param>
        public static void DeActiveChild(this Component com, int index)
        {
            if (com != null)
            {
                var child = com.transform.GetChild(index);
                if (child != null)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        
        /// <summary>
        /// Deactive 所有目标index子对象，如果不是目标index，则激活
        /// </summary>
        /// <param name="com">父组件对象</param>
        /// <param name="index">目标索引位置</param>
        public static void DeActiveChildBut(this Component com, int index)
        {
            if (com != null)
            {
                var length = com.transform.childCount;
                for (int i = 0; i < length; i++)
                {
                    com.transform.GetChild(i).gameObject.SetActive(i == index);
                }
            }
        }

        public static void DeActiveChildAll(this Component com)
        {
            if (com != null)
            {
                foreach (Transform item in com.transform)
                {
                    item.gameObject.SetActive(false);
                }
            }
        }

        static public T Clone<T>(this T com) where T : Component
        {
            if (com != null)
            {
                var newCom = GameObject.Instantiate<T>(com);
                Transform t = newCom.transform;
                t.parent = com.transform.parent;
                t.localPosition = com.transform.localPosition;
                t.localRotation = com.transform.localRotation;
                t.localScale = com.transform.localScale;
                newCom.gameObject.layer = com.gameObject.layer;
                return newCom;
            }

            return null;
        }

        public static T GetChild<T>(this Component com, int index) where T : Component
        {
            if (com != null)
            {
                var child = com.transform.GetChild(index);
                if (child != null)
                {
                    return child.GetComponent<T>();
                }
            }

            return null;
        }
    }
}