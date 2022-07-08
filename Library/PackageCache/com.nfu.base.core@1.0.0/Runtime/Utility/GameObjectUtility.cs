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
//     File Name           :        GameObjectUtility.cs
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

namespace ND.Core.Utility
{
    public class GameObjectUtility
    {
        /// <summary>
        /// 查找gameObject
        /// </summary>
        /// <param name="root"></param>
        /// <param name="path"></param>
        /// <param name="build"></param>
        /// <param name="dontDestroy"></param>
        /// <returns></returns>
        public static GameObject FindGameObject(GameObject root, string path, bool build, bool dontDestroy)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            string[] subPath = path.Split('/');
            if (subPath.Length == 0)
            {
                return null;
            }

            return FindGameObject(root, subPath, 0, build, dontDestroy);
        }

        /// <summary>
        /// 查找gameObject
        /// </summary>
        /// <param name="root"></param>
        /// <param name="subPath"></param>
        /// <param name="index"></param>
        /// <param name="build"></param>
        /// <param name="dontDestroy"></param>
        /// <returns></returns>
        public static GameObject FindGameObject(GameObject root, string[] subPath, int index, bool build,
            bool dontDestroy)
        {
            GameObject client = null;

            if (root == null)
            {
                client = GameObject.Find(subPath[index]);
            }
            else
            {
                var child = root.transform.Find(subPath[index]);
                if (child != null)
                {
                    client = child.gameObject;
                }
            }

            if (client == null)
            {
                if (build)
                {
                    client = new GameObject(subPath[index]);
                    if (root != null)
                    {
                        client.transform.SetParent(root.transform);
                    }

                    if (dontDestroy && index == 0)
                    {
                        if (Application.isPlaying)
                            GameObject.DontDestroyOnLoad(client.transform.root.gameObject);
                    }
                }
            }

            if (client == null)
            {
                return null;
            }

            if (++index == subPath.Length)
            {
                return client;
            }

            return FindGameObject(client, subPath, index, build, dontDestroy);
        }
    }
}