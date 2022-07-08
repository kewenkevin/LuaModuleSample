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
//     File Name           :        Intersection2DUtils.cs
// 
//     Programmer          :        Wei Wei (Battle Mage Gandalf)
// 
//     Start Date          :        04/07/2021
// 
//     Last Update         :        04/07/2021 16:30 [Wei]
// 
//     Description         :        Intersection2DUtils
// 
// =============================================================================================
// Contributors:
// ---------------------------------------------------------------------------------------------
// Battle Mage Gandalf                 wwei@yoozoo.com             Product technology Center
// =============================================================================================

using ND.Core.DataStruct.Geometry;
using UnityEngine;

namespace ND.Core.Geometry
{
    public class Intersection2DUtility
    {
        /// <summary>
        /// 测试线段与线段是否相交
        /// </summary>
        /// <param name="a1">The start point of the first line</param>
        /// <param name="a2">The end point of the first line</param>
        /// <param name="b1">The start point of the second line</param>
        /// <param name="b2">The end point of the second line</param>
        /// <returns></returns>
        public static bool LineLine(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            var ua_t = (b2.x - b1.x) * (a1.y - b1.y) - (b2.y - b1.y) * (a1.x - b1.x);
            var ub_t = (a2.x - a1.x) * (a1.y - b1.y) - (a2.y - a1.y) * (a1.x - b1.x);
            var u_b = (b2.y - b1.y) * (a2.x - a1.x) - (b2.x - b1.x) * (a2.y - a1.y);

            if (Mathf.Abs(u_b) > float.Epsilon)
            {
                var ua = ua_t / u_b;
                var ub = ub_t / u_b;

                if (0 <= ua && ua <= 1 && 0 <= ub && ub <= 1)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 测试线段与矩形是否相交
        /// </summary>
        /// <param name="a1">The start point of the line</param>
        /// <param name="a2">The end point of the line</param>
        /// <param name="b">The rect</param>
        /// <returns></returns>
        public static bool LineRect(Vector2 a1, Vector2 a2, Rect b)
        {
            var r0 = new Vector2(b.x, b.y);
            var r1 = new Vector2(b.x, b.yMax);
            var r2 = new Vector2(b.xMax, b.yMax);
            var r3 = new Vector2(b.xMax, b.y);

            if (LineLine(a1, a2, r0, r1))
                return true;

            if (LineLine(a1, a2, r1, r2))
                return true;

            if (LineLine(a1, a2, r2, r3))
                return true;

            if (LineLine(a1, a2, r3, r0))
                return true;

            return false;
        }

        /// <summary>
        /// 测试线段与多边形是否相交
        /// </summary>
        /// <param name="a1">The start point of the line</param>
        /// <param name="a2">The end point of the line</param>
        /// <param name="b">The polygon, a set of points</param>
        /// <returns></returns>
        public static bool LinePolygon(Vector2 a1, Vector2 a2, Vector2[] b)
        {
            var length = b.Length;

            for (var i = 0; i < length; ++i)
            {
                var b1 = b[i];
                var b2 = b[(i + 1) % length];

                if (LineLine(a1, a2, b1, b2))
                    return true;
            }

            return false;
        }


        /// <summary>
        /// 测试矩形与矩形是否相交
        /// </summary>
        /// <param name="a">The first rect</param>
        /// <param name="b">The second rect</param>
        /// <returns></returns>
        public static bool RectRect(Rect a, Rect b)
        {
            // jshint camelcase:false

            var a_min_x = a.x;
            var a_min_y = a.y;
            var a_max_x = a.x + a.width;
            var a_max_y = a.y + a.height;

            var b_min_x = b.x;
            var b_min_y = b.y;
            var b_max_x = b.x + b.width;
            var b_max_y = b.y + b.height;

            return a_min_x <= b_max_x &&
                   a_max_x >= b_min_x &&
                   a_min_y <= b_max_y &&
                   a_max_y >= b_min_y;
        }

        /// <summary>
        /// 测试矩形与多边形是否相交
        /// </summary>
        /// <param name="a">The rect</param>
        /// <param name="b">The polygon, a set of points</param>
        /// <returns></returns>
        public static bool RectPolygon(Rect a, Vector2[] b)
        {
            int i, l;
            var r0 = new Vector2(a.x, a.y);
            var r1 = new Vector2(a.x, a.yMax);
            var r2 = new Vector2(a.xMax, a.yMax);
            var r3 = new Vector2(a.xMax, a.y);

            // intersection check
            if (LinePolygon(r0, r1, b))
                return true;

            if (LinePolygon(r1, r2, b))
                return true;

            if (LinePolygon(r2, r3, b))
                return true;

            if (LinePolygon(r3, r0, b))
                return true;

            // check if a contains b
            for (i = 0, l = b.Length; i < l; ++i)
            {
                if (PointInPolygon(b[i], new[] {r0, r1, r2, r3}))
                    return true;
            }

            return PointInPolygon(r0, b) && PointInPolygon(r1, b) && PointInPolygon(r2, b) && PointInPolygon(r3, b);
        }

        /// <summary>
        /// 测试多边形与多边形是否相交
        /// </summary>
        /// <param name="a">The first polygon, a set of points</param>
        /// <param name="b">The second polygon, a set of points</param>
        /// <returns></returns>
        public static bool PolygonPolygon(Vector2[] a, Vector2[] b)
        {
            int i, l;

            // check if a intersects b
            for (i = 0, l = a.Length; i < l; ++i)
            {
                var a1 = a[i];
                var a2 = a[(i + 1) % l];

                if (LinePolygon(a1, a2, b))
                    return true;
            }

            // check if a contains b
            for (i = 0, l = b.Length; i < l; ++i)
            {
                if (PointInPolygon(b[i], a))
                    return true;
            }

            // check if b contains a
            for (i = 0, l = a.Length; i < l; ++i)
            {
                if (PointInPolygon(a[i], b))
                    return true;
            }

            return false;
        }


        /// <summary>
        /// 测试圆形与圆形是否相交
        /// </summary>
        /// <param name="a">CircleA</param>
        /// <param name="b">CircleB</param>
        /// <returns></returns>
        public static bool CircleCircle(Circle a, Circle b)
        {
            return (a.position - b.position).magnitude < (a.radius + b.radius);
        }


        /// <summary>
        /// 测试矩形与圆形是否相交
        /// </summary>
        /// <param name="polygon">The Polygon, a set of points</param>
        /// <param name="circle">The Circle</param>
        /// <returns></returns>
        public static bool PolygonCircle(Vector2[] polygon, Circle circle)
        {
            var position = circle.position;
            if (PointInPolygon(position, polygon))
            {
                return true;
            }

            for (int i = 0, l = polygon.Length; i < l; i++)
            {
                var start = i == 0 ? polygon[polygon.Length - 1] : polygon[i - 1];
                var end = polygon[i];

                if (PointLineDistance(position, start, end, true) < circle.radius)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 测试一个点是否在一个多边形中
        /// </summary>
        /// <param name="point">The point</param>
        /// <param name="polygon">The polygon, a set of points</param>
        /// <returns></returns>
        public static bool PointInPolygon(Vector2 point, Vector2[] polygon)
        {
            if (polygon.Length == 4)
                return PointInQuad(point, polygon);


            var inside = false;
            var x = point.x;
            var y = point.y;

            // use some raycasting to test hits
            // https://github.com/substack/point-in-polygon/blob/master/index.js
            var length = polygon.Length;
            float xi, yi, xj, yj;
            for (int i = 0, j = length - 1; i < length; j = i++)
            {
                xi = polygon[i].x;
                yi = polygon[i].y;
                xj = polygon[j].x;
                yj = polygon[j].y;
                bool intersect = ((yi > y) != (yj > y)) && (x < (xj - xi) * (y - yi) / (yj - yi) + xi);
                if (intersect) inside = !inside;
            }

            return inside;
        }


        /// <summary>
        /// 计算点到直线的距离。如果这是一条线段并且垂足不在线段内，则会计算点到线段端点的距离。
        /// </summary>
        /// <param name="point">The point</param>
        /// <param name="start">The start point of line</param>
        /// <param name="end">The end point of line</param>
        /// <param name="isSegment">whether this line is a segment</param>
        /// <returns></returns>
        public static float PointLineDistance(Vector2 point, Vector2 start, Vector2 end, bool isSegment)
        {
            var dx = end.x - start.x;
            var dy = end.y - start.y;
            var d = dx * dx + dy * dy;
            var t = ((point.x - start.x) * dx + (point.y - start.y) * dy) / d;
            Vector2 p;

            if (!isSegment)
            {
                p = new Vector2(start.x + t * dx, start.y + t * dy);
            }
            else
            {
                if (d > float.Epsilon)
                {
                    if (t < 0) p = start;
                    else if (t > 1) p = end;
                    else p = new Vector2(start.x + t * dx, start.y + t * dy);
                }
                else
                {
                    p = start;
                }
            }

            dx = point.x - p.x;
            dy = point.y - p.y;
            return Mathf.Sqrt(dx * dx + dy * dy);
        }


        private static bool PointInQuad(Vector2 point, Vector2[] points)
        {
            if (points.Length != 4)
            {
                return PointInPolygon(point, points);
            }

            return !SameSide(point, points[0], points[1], points[3], points[2])
                   && !SameSide(point, points[1], points[2], points[0], points[3]);
        }

        private static bool SameSide(Vector2 p, Vector2 p0, Vector2 p1, Vector2 v0, Vector2 v1)
        {
            return IsLeft(p, p0, p1) * IsLeft(p, v0, v1) >= 0;
        }

        private static float IsLeft(Vector2 p, Vector2 v0, Vector2 v1)
        {
            return (v0.y - v1.y) * p.x + (v1.x - v0.x) * p.y + v0.x * v1.y - v1.x * v0.y;
        }
    }
}