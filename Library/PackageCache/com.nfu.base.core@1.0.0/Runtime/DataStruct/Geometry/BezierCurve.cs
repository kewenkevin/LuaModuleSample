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
//     File Name           :        BezierCurve.cs
// 
//     Programmer          :        Wei Wei (Battle Mage Gandalf)
// 
//     Start Date          :        04/07/2021
// 
//     Last Update         :        04/07/2021 16:30 [Wei]
// 
//     Description         :        Quad Bezier Curve define by start point,control point 1,control point 2,destination point 
// 
// =============================================================================================
// Contributors:
// ---------------------------------------------------------------------------------------------
// Battle Mage Gandalf                 wwei@yoozoo.com             Product technology Center
// =============================================================================================

using System;
using UnityEngine;

namespace ND.Core.DataStruct.Geometry
{
    [Serializable]
    public class BezierCurve
    {
        private const int DefaultDifferentialTimes = 10000;
        private const int DefineNoneLength = -1;

        private float totalLength = -1;
        private int _DifferentiaTimes = DefaultDifferentialTimes;

        [SerializeField] private Vector3 p0;
        [SerializeField] private Vector3 p1;
        [SerializeField] private Vector3 p2;
        [SerializeField] private Vector3 p3;

        public Vector3 P0
        {
            get => p0;
            set
            {
                p0 = value;
                totalLength = DefineNoneLength;
            }
        }

        public Vector3 P1
        {
            get => p1;
            set
            {
                p1 = value;
                totalLength = DefineNoneLength;
            }
        }

        public Vector3 P2
        {
            get => p2;
            set
            {
                p2 = value;
                totalLength = DefineNoneLength;
            }
        }

        public Vector3 P3
        {
            get => p3;
            set
            {
                p3 = value;
                totalLength = DefineNoneLength;
            }
        }

        public int DifferentiaTimes
        {
            get => _DifferentiaTimes;
            set
            {
                _DifferentiaTimes = value;
                totalLength = -1;
            }
        }

        public float IntegralLength
        {
            get
            {
                if (totalLength < 0)
                    totalLength = GetLength(1);
                return totalLength;
            }
        }

        public BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            P0 = p0;
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }

        public Vector3 GetPosition(float t)
        {
            float it = 1 - t;
            return it * it * it * P0 + 3 * it * it * t * P1 + 3 * it * t * t * P2 + t * t * t * P3;
        }

        public Vector3 GetSpeedVector(float t)
        {
            float it = 1 - t;
            return -3 * P0 * it * it + 3 * P1 * it * it - 6 * P1 * it * t + 6 * P2 * it * t - 3 * P2 * t * t +
                   3 * P3 * t * t;
        }

        private float GetSpeed(float t)
        {
            var speed = GetSpeedVector(t);
            return speed.magnitude;
        }

        //-------------------------------------------------------------------------------------
        private float GetLength(float t)
        {
            int steps = (int) Mathf.Ceil(DifferentiaTimes * t);
            if (steps == 0)
                return 0;
            if (steps % 2 != 0)
                steps++;
            float halfSteps = steps * .5f;
            float nSum = 0;
            float n1Sum = 0;
            float disStep = t / steps;

            for (int i = 0; i < halfSteps; i++)
            {
                n1Sum += GetSpeed((2 * i + 1) * disStep);
                nSum += GetSpeed(2 * i * disStep);
            }

            return (GetSpeed(0) + GetSpeed(1) + 2 * n1Sum + 4 * nSum) * disStep / 3;
        }

        //-------------------------------------------------------------------------------------
        private float getEven(float t)
        {
            float len = t * GetLength(1);
            float uc = 0;
            do
            {
                float ulen = GetLength(t);
                float uspeed = GetSpeed(t);
                uc = t - (ulen - len) / uspeed;
                if (Mathf.Abs(uc - t) < 0.0001)
                    break;

                t = uc;
            } while (true);

            return uc;
        }
    }
}