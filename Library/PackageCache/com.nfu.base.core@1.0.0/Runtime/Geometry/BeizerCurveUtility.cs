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
//     File Name           :        BeizerCurveUtility.cs
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

namespace ND.Core.Geometry
{
    public class BeizerCurveUtility
    {
        /// <summary>
        /// 三维 二次贝塞尔曲线上一个点
        /// </summary>
        /// <param name="t"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Vector3 CalculateQuadBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            Vector3 p = uu * p0;
            p += 2 * u * t * p1;
            p += tt * p2;

            return p;
        }

        /// <summary>
        /// 二维  二次贝塞尔曲线上一个点
        /// </summary>
        /// <param name="t"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Vector2 CalculateQuadBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            Vector2 p = uu * p0;
            p += 2 * u * t * p1;
            p += tt * p2;

            return p;
        }

        public static Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1 - t;
            float uu = u * u;
            float uuu = u * u * u;
            float tt = t * t;
            float ttt = t * t * t;


            Vector3 p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;

            return p;
        }

        public static Vector2 CalculateCubicBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float u = 1 - t;
            float uu = u * u;
            float uuu = u * u * u;
            float tt = t * t;
            float ttt = t * t * t;


            Vector2 p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;

            return p;
        }

        public static Vector3[] GetQuadBeizerList(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint,
            int segmentNum)
        {
            Vector3[] path = new Vector3[segmentNum];
            for (int i = 1; i <= segmentNum; i++)
            {
                float t = i / (float) segmentNum;
                Vector3 pixel = CalculateQuadBezierPoint(t, startPoint,
                    controlPoint, endPoint);
                path[i - 1] = pixel;
                Debug.Log(path[i - 1]);
            }

            return path;
        }

        public static Vector2[] GetQuadBeizerList(Vector2 startPoint, Vector2 controlPoint, Vector2 endPoint,
            int segmentNum)
        {
            Vector2[] path = new Vector2[segmentNum];
            for (int i = 1; i <= segmentNum; i++)
            {
                float t = i / (float) segmentNum;
                Vector2 pixel = CalculateQuadBezierPoint(t, startPoint,
                    controlPoint, endPoint);
                path[i - 1] = pixel;
                Debug.Log(path[i - 1]);
            }

            return path;
        }

        public static Vector3[] GetCubicBeizerList(Vector3 startPoint, Vector3 controlPoint1, Vector3 controlPoint2,
            Vector3 endPoint, int segmentNum)
        {
            Vector3[] path = new Vector3[segmentNum];
            for (int i = 1; i <= segmentNum; i++)
            {
                float t = i / (float) segmentNum;
                Vector3 pixel = CalculateCubicBezierPoint(t, startPoint,
                    controlPoint1, controlPoint2, endPoint);
                path[i - 1] = pixel;
                Debug.Log(path[i - 1]);
            }

            return path;
        }

        public static Vector2[] GetCubicBeizerList(Vector2 startPoint, Vector2 controlPoint1, Vector2 controlPoint2,
            Vector2 endPoint, int segmentNum)
        {
            Vector2[] path = new Vector2[segmentNum];
            for (int i = 1; i <= segmentNum; i++)
            {
                float t = i / (float) segmentNum;
                Vector2 pixel = CalculateCubicBezierPoint(t, startPoint,
                    controlPoint1, controlPoint2, endPoint);
                path[i - 1] = pixel;
                Debug.Log(path[i - 1]);
            }

            return path;
        }
    }
}