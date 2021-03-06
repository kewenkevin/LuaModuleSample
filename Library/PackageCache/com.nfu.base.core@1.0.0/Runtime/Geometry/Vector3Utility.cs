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
//     File Name           :        Vector3Utility.cs
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
    public static class Vector3Utility
    {
        
        public static Vector3 GetSmoothEulerDestination(this Vector3 begin, Vector3 dest)
        {
            return begin + EulerFit(dest - begin);
        }

        public static Vector3 EulerFit(this Vector3 euler)
        {
            float x = AngleUtility.AngleFit(euler.x);
            float y = AngleUtility.AngleFit(euler.y);
            float z = AngleUtility.AngleFit(euler.z);

            return new Vector3(x, y, z);
        }
        
        public static Vector3 IgnoreHeight(this Vector3 point)
        {
            return new Vector3(point.x, 0f, point.z);
        }
        
        public static bool IsFarThan(this Vector3 pos1, Vector3 pos2, float distance)
        {
            Vector3 offset = pos1 - pos2;
            float sqrLen = offset.sqrMagnitude;
            return (sqrLen > distance * distance);
        }
        
        /*???????????????
        * 1.????????????????????????
        * 2.????????????????????????
        * 3.???????????????????????????  
        */

        /// <summary>
        /// ???????????????????????????
        /// </summary>
        /// <param name="point"></param>
        /// <param name="direct"></param>
        /// <param name="planeNormal"></param>
        /// <param name="planePoint"></param>
        /// <param name="maxDistance"></param>
        /// <returns></returns>
        public static Vector3 GetIntersectionWithLineAndPlane(Vector3 point, Vector3 direct, Vector3 planeNormal, Vector3 planePoint, float maxDistance = 100)
        {
            float p = Vector3.Dot(direct.normalized, planeNormal);
            if (Mathf.Abs(p) <= float.Epsilon)//==0
                return point + direct.normalized * maxDistance;
            float d = Vector3.Dot(planePoint - point, planeNormal) / p;
            return d * direct.normalized + point;
        }

        /// <summary>
        /// ???????????????????????????
        /// </summary>
        /// <param name="point"></param>
        /// <param name="planeNormal"></param>
        /// <param name="planePoint"></param>
        /// <returns></returns>
        public static float GetDistanceFromPointToPlane(Vector3 point, Vector3 planeNormal, Vector3 planePoint)
        {
            Plane plane = new Plane(planePoint, planeNormal);
            return plane.GetDistanceToPoint(point);
        }

        /// <summary>
        /// ???????????????????????????
        /// </summary>
        /// <param name="point"></param>
        /// <param name="lineDir"></param>
        /// <param name="linePoint"></param>
        /// <returns></returns>
        public static float GetDistanceFromPointToLine(Vector3 point, Vector3 linePoint, Vector3 lineDir)
        {
            Vector3 pl = point - linePoint;
            //float cos = Vector3.Dot(pl, lineDir.normalized);
            //return pl.magnitude * Mathf.Sqrt(1 - cos * cos);
            // 
            //        P - ????????????Q1, Q2??????????????????
            //  ??????????????????????????????????????????:  Q1)) / norm(Q2 - Q1);


            return Vector3.Cross(lineDir, pl).magnitude / lineDir.magnitude;
        }

        /// <summary>
        /// ????????????????????????
        /// </summary>
        /// <param name="point"></param>
        /// <param name="planeNormal"></param>
        /// <param name="planePoint"></param>
        /// <returns></returns>
        public static Vector3 GetProjectionPointFromPlane(Vector3 point, Vector3 planeNormal, Vector3 planePoint)
        {
            return Vector3.ProjectOnPlane(point - planePoint, planeNormal) + planePoint;
        }

        /// <summary>
        /// ???????????????????????????
        /// </summary>
        /// <param name="point"></param>
        /// <param name="planeNormal"></param>
        /// <param name="planePoint"></param>
        /// <returns></returns>
        public static Vector3 GetProjectionVectorFromPlane(Vector3 vect, Vector3 planeNormal)
        {
            return Vector3.ProjectOnPlane(vect, planeNormal);
        }

        /// <summary>
        /// ???????????????????????????
        /// </summary>
        /// <param name="point"></param>
        /// <param name="lineDir"></param>
        /// <param name="linePoint"></param>
        /// <returns></returns>
        public static Vector3 GetProjectionPointFromLine(Vector3 point, Vector3 lineDir, Vector3 linePoint)
        {
            Vector3 dir = Vector3.Dot(point - linePoint, lineDir) * lineDir / lineDir.sqrMagnitude;
            return linePoint + dir;
        }

        /// <summary>
        /// ??????A?????????B??????????????????
        /// </summary>
        /// <param name="vectA"></param>
        /// <param name="vectB"></param>
        /// <returns></returns>
        public static Vector3 GetProjectionOnVector(Vector3 vectA, Vector3 vectB)
        {
            return Vector3.Dot(vectA, vectB) * vectB / vectB.sqrMagnitude;
        }

        /// <summary>
        /// ?????????????????????????????????
        /// </summary>
        /// <param name="vectA"></param>
        /// <param name="vectB"></param>
        /// <returns></returns>
        public static float GetAngleBetweenVector(Vector3 vectA, Vector3 vectB)
        {
            return Vector3.AngleBetween(vectA, vectB) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// ????????????(0-1]???????????????????????????
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetBoxMullerVect2()
        {
            float U1 = UnityEngine.Random.Range(Mathf.Epsilon, 1.0f);
            float U2 = UnityEngine.Random.Range(Mathf.Epsilon, 1.0f);
            float Z0 = Mathf.Sqrt(-2f * Mathf.Log(U1)) * Mathf.Cos(2f * Mathf.PI * U2);
            float Z1 = Mathf.Sqrt(-2f * Mathf.Log(U1)) * Mathf.Sin(2f * Mathf.PI * U2);
            return new Vector2(Z0, Z1);
        }

        /// <summary>
        /// ????????????????????????
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetRandom()
        {
            return UnityEngine.Random.insideUnitSphere;
        }
        
        
     

    }
}