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
//     File Name           :        TransformExtension.cs
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
using System.Collections.Generic;
using UnityEngine;

namespace ND.Core.Extensions.Engine
{
    public static class TransformExtension
    {
        #region Position

        public static void SetLocalPosition(Transform transform, float x, float y, float z)
        {
            if (transform != null)
                transform.SetLocalPosition(new Vector3(x, y, z));
        }


        public static void SetLocalPosition(this Transform transform, Vector3 pos)
        {
            if (transform != null)
                transform.localPosition = pos;
        }

        public static void SetLocalPositionX(this Transform transform, float x)
        {
            if (transform != null)
            {
                var pos = transform.localPosition;
                transform.localPosition = new Vector3(x, pos.y, pos.z);
            }
        }

        public static void SetLocalPositionY(this Transform transform, float y)
        {
            if (transform != null)
            {
                var pos = transform.localPosition;
                transform.localPosition = new Vector3(pos.x, y, pos.z);
            }
        }

        public static void SetLocalPositionZ(this Transform transform, float z)
        {
            if (transform != null)
            {
                var pos = transform.localPosition;
                transform.localPosition = new Vector3(pos.x, pos.y, z);
            }
        }

        public static void LocalReset(this Transform objTrans)
        {
            objTrans.localPosition = Vector3.zero;
            objTrans.localScale = Vector3.one;
            objTrans.localRotation = Quaternion.identity;
        }

        #endregion


        #region Layer

        public static void AlignLayer(GameObject template, GameObject go)
        {
            if (null == template || null == go)
            {
                return;
            }

            ChangeLayersRecursively(go.transform, template.layer);
        }

        public static void ChangeLayersRecursively(this Transform trans, string name)
        {
            trans.gameObject.layer = LayerMask.NameToLayer(name);
            foreach (Transform child in trans)
            {
                child.ChangeLayersRecursively(name);
            }
        }

        public static void ChangeLayersRecursively(this Transform trans, int layer)
        {
            trans.gameObject.layer = layer;
            foreach (Transform child in trans)
            {
                child.ChangeLayersRecursively(layer);
            }
        }

        public static void ChangeLayersRecursively(this GameObject gameObject, string name)
        {
            gameObject.layer = LayerMask.NameToLayer(name);
            foreach (Transform child in gameObject.transform)
            {
                child.ChangeLayersRecursively(name);
            }
        }

        public static void ChangeLayersRecursively(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            foreach (Transform child in gameObject.transform)
            {
                child.ChangeLayersRecursively(layer);
            }
        }


        // 将指定的层级替换成新的层级
        public static void SwitchLayerRecursively(Transform parent, int newLayer, int originalLayer)
        {
            // 无需替换相同层级
            if (newLayer == originalLayer)
            {
                return;
            }

            if (null == parent || parent.gameObject.layer != originalLayer)
            {
                return;
            }

            parent.gameObject.layer = newLayer;

            Transform[] childs = parent.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < childs.Length; ++i)
            {
                if (childs[i].gameObject.layer == originalLayer)
                {
                    childs[i].gameObject.layer = newLayer;
                }
            }
        }

        #endregion


        #region Material

        public static void SetShaderRecursively(this Transform parent, Shader s)
        {
            if (null == parent)
            {
                return;
            }

            Renderer renderer = parent.GetComponent<Renderer>();
            ParticleSystem ps = parent.GetComponent<ParticleSystem>();

            if (renderer != null && renderer.material != null && null == ps)
            {
                renderer.material.shader = s;
            }

            Transform[] childs = parent.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < childs.Length; ++i)
            {
                Renderer cr = childs[i].GetComponent<Renderer>();
                ParticleSystem cps = childs[i].GetComponent<ParticleSystem>();


                if (cr != null && cr.material != null && null == cps)
                {
                    cr.material.shader = s;
                }
            }
        }

        #endregion


        #region Child & Parent

        public static void SetParent(GameObject go, GameObject parent)
        {
            if (null == go || null == parent) return;
            go.transform.SetParent(parent.transform);
        }

        public static void SetParent(GameObject go, Transform parent)
        {
            if (null == go)
                return;
            go.transform.SetParent(parent);
        }

        public static bool IsMyChild(Transform parent, Transform child)
        {
            if (parent == null || child == null)
            {
                return false;
            }

            Transform lastParent = child.parent;
            while (lastParent != null)
            {
                if (lastParent.GetInstanceID() == parent.GetInstanceID())
                {
                    return true;
                }

                lastParent = lastParent.parent;
            }

            return false;
        }

        //查找物体组件
        public static T FindChildComponent<T>(this Transform parent, string childName) where T : Component
        {
            if (null == parent || string.IsNullOrEmpty(childName))
            {
                return null;
            }

            Transform child = parent.Find(childName);

            if (null == child)
            {
                return null;
            }

            return child.GetComponent<T>();
        }

        public static Transform FindChildRecursively(Transform parent, string name)
        {
            if (null == parent || null == name)
            {
                return null;
            }

            for (int i = 0; i < parent.childCount; ++i)
            {
                Transform child = parent.GetChild(i);

                if (child.name.Equals(name))
                {
                    return child;
                }

                Transform childChild = FindChildRecursively(child, name);

                if (childChild != null)
                {
                    return childChild;
                }
            }

            return null;
        }

        public static List<Transform> FindChildrenRecursively(Transform parent, string name)
        {
            List<Transform> ret = new List<Transform>();

            if (null == parent || null == name)
            {
                return ret;
            }

            Transform[] childs = parent.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < childs.Length; ++i)
            {
                if (childs[i].name == name)
                {
                    ret.Add(childs[i]);
                }
            }

            return ret;
        }

        public static List<T> FindChildrenRecursively<T>(Transform parent, string name) where T : Component
        {
            List<T> ret = new List<T>();

            if (null == parent || null == name)
            {
                return ret;
            }

            Transform[] childs = parent.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < childs.Length; ++i)
            {
                if (childs[i].name == name)
                {
                    T c = childs[i].GetComponent<T>();

                    if (c != null)
                    {
                        ret.Add(c);
                    }
                }
            }

            return ret;
        }

        #endregion
    }
}