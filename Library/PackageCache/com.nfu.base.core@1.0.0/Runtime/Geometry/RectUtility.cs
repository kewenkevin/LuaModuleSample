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
//     File Name           :        RectUtility.cs
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
    public static class RectUtility
    {
        /// <summary>
        /// 转换成整型矩形（向下取整）
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static RectInt ConvertToRectIntFloor(Rect rect)
        {
            Vector2Int pos = Vector2Utility.ConvertToVector2IntFloor(rect.position);
            Vector2Int size = Vector2Utility.ConvertToVector2IntFloor(rect.size);
            return new RectInt(pos, size);
        }

        /// <summary>
        /// 获取对角线方向
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Vector2 GetDiagonal(Rect rect)
        {
            return -rect.size;
        }

        /// <summary>
        /// 获取对角线方向
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Vector2Int GetDiagonal(RectInt rect)
        {
            return -rect.size;
        }

        /// <summary>
        /// 移动矩形
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static RectInt MoveRect(RectInt rect, Vector2Int dir)
        {
            return new RectInt(rect.position + dir, rect.size);
        }

       

        public static RectInt ExpandRectUp(RectInt rect, int up)
        {
            if (up < 0 && Mathf.Abs(rect.size.y) <= Mathf.Abs(up)) return rect;
            return rect.size.y > 0
                ? new RectInt(rect.position, new Vector2Int(rect.size.x, rect.size.y + up))
                : new RectInt(new Vector2Int(rect.position.x, rect.position.y + up),
                    new Vector2Int(rect.size.x, rect.size.y - up));
        }

        public static RectInt ExpandRectDown(RectInt rect, int down)
        {
            if (down < 0 && Mathf.Abs(rect.size.y) <= Mathf.Abs(down)) return rect;
            return rect.size.y < 0
                ? new RectInt(rect.position, new Vector2Int(rect.size.x, rect.size.y - down))
                : new RectInt(new Vector2Int(rect.position.x, rect.position.y - down),
                    new Vector2Int(rect.size.x, rect.size.y + down));
        }

        public static RectInt ExpandRectLeft(RectInt rect, int left)
        {
            if (left < 0 && Mathf.Abs(rect.size.x) <= Mathf.Abs(left)) return rect;
            return rect.size.x < 0
                ? new RectInt(rect.position, new Vector2Int(rect.size.x - left, rect.size.y))
                : new RectInt(new Vector2Int(rect.position.x - left, rect.position.y),
                    new Vector2Int(rect.size.x + left, rect.size.y));
        }

        public static RectInt ExpandRectRight(RectInt rect, int right)
        {
            if (right < 0 && Mathf.Abs(rect.size.x) <= Mathf.Abs(right)) return rect;
            return rect.size.x > 0
                ? new RectInt(rect.position, new Vector2Int(rect.size.x + right, rect.size.y))
                : new RectInt(new Vector2Int(rect.position.x + right, rect.position.y),
                    new Vector2Int(rect.size.x - right, rect.size.y));
        }


        /// <summary>
        /// 框选一个矩形
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static RectInt BoxRect(Vector2Int startIndex, Vector2Int endIndex)
        {
            Vector2Int dir = endIndex - startIndex;

            int x = startIndex.x + (dir.x < 0 ? 1 : 0); //+ VectUtil.GetSignOne(dir.x,0);
            int y = startIndex.y + (dir.y < 0 ? 1 : 0); //+ VectUtil.GetSignOne(dir.y,0);
            int w = dir.x + Vector2Utility.GetSignOne(dir.x, 1);
            int h = dir.y + Vector2Utility.GetSignOne(dir.y, 1);

            Vector2Int pos = new Vector2Int(x, y);
            Vector2Int size = new Vector2Int(w, h);
            return new RectInt(pos, size);
        }

        /// <summary>
        /// 把一个整型矩形旋转90度（右边补全）
        /// </summary>
        /// <param name="originRect"></param>
        /// <param name="isLeft"></param>
        /// <returns></returns>
        public static RectInt RotateRectInt(RectInt originRect, bool isLeft)
        {
            int w = Mathf.Abs(originRect.size.x);

            int h = Mathf.Abs(originRect.size.y);

            int max = Mathf.Max(w, h);

            RectInt rotatedRect = originRect;

            int quadrant = ((Vector2) GetDiagonal(originRect)).CheckQuadrant();

            //Debug.Log("当前第3象限 没有旋转");
            if (quadrant == 3)
            {
                if (isLeft)
                {
                    rotatedRect = new RectInt(originRect.position + Vector2Int.right * max,
                        Vector2Int.left * h + Vector2Int.up * w);
                }
                else
                {
                    rotatedRect = new RectInt(originRect.position + Vector2Int.up * max,
                        Vector2Int.right * h + Vector2Int.down * w);
                }
            }

            //Debug.Log("当前第4象限 左旋转过一次");
            if (quadrant == 4)
            {
                if (isLeft)
                {
                    rotatedRect = new RectInt(originRect.position + Vector2Int.up * max,
                        Vector2Int.left * h + Vector2Int.down * w);
                }
                else
                {
                    rotatedRect = new RectInt(originRect.position + Vector2Int.left * max,
                        Vector2Int.right * h + Vector2Int.up * w);
                }
            }

            //Debug.Log("当前第2象限 右旋转过一次");
            if (quadrant == 2)
            {
                if (isLeft)
                {
                    rotatedRect = new RectInt(originRect.position + Vector2Int.down * max,
                        Vector2Int.up * w + Vector2Int.right * h);
                }
                else
                {
                    rotatedRect = new RectInt(originRect.position + Vector2Int.right * max,
                        Vector2Int.left * h + Vector2Int.down * w);
                }

            }

            //Debug.Log("当前第1象限 反旋转(旋转过两次)");
            if (quadrant == 1)
            {
                if (isLeft)
                {
                    rotatedRect = new RectInt(originRect.position + Vector2Int.left * max,
                        Vector2Int.right * h + Vector2Int.down * w);
                }
                else
                {
                    rotatedRect = new RectInt(originRect.position + Vector2Int.down * max,
                        Vector2Int.left * h + Vector2Int.up * w);
                }
            }

            return rotatedRect;
        }

        public static RectInt RotateRectInRect(RectInt originRect, RectInt outRect, RectInt rotRect, bool isLeft)
        {
            float outmid_x = (float) (outRect.xMax + outRect.xMin) * 0.5f;
            float outmid_y = (float) (outRect.yMax + outRect.yMin) * 0.5f;

            float rotmid_x = (float) (rotRect.xMax + rotRect.xMin) * 0.5f;
            float rotmid_y = (float) (rotRect.yMax + rotRect.yMin) * 0.5f;

            var s_x = originRect.position.x;
            var s_y = originRect.position.y;

            var e_y = s_y + originRect.size.y;
            var e_x = s_x + originRect.size.x;

            float t_x = s_x - outmid_x;
            float t_y = s_y - outmid_y;

            float te_x = e_x - outmid_x;
            float te_y = e_y - outmid_y;

            int cos0 = 0;
            int sin0 = isLeft ? 1 : -1;

            float r_x = t_x * cos0 - t_y * sin0 + (rotmid_x);
            float r_y = t_x * sin0 + t_y * cos0 + (rotmid_y);

            float re_x = te_x * cos0 - te_y * sin0 + (rotmid_x);
            float re_y = te_x * sin0 + te_y * cos0 + (rotmid_y);

            return new RectInt(new Vector2Int((int) r_x, (int) r_y),
                new Vector2Int((int) (re_x - r_x), (int) (re_y - r_y)));
        }

        public static RectInt ReDiagonal(RectInt rect, Vector2 dia)
        {

            var originDia = GetDiagonal(rect);
            var center = rect.center;


            Vector2Int pos = rect.position;
            Vector2Int size = rect.size;

            if (originDia.x * dia.x < 0)
            {
                pos = new Vector2Int(Mathf.FloorToInt(center.x + size.x * 0.5f), pos.y);
                size = new Vector2Int(-size.x, size.y);
            }

            if (originDia.y * dia.y < 0)
            {
                pos = new Vector2Int(pos.x, (int) (center.y + size.y * 0.5f));
                size = new Vector2Int(size.x, -size.y);
            }

            return new RectInt(pos, size);
        }

        public static RectInt Copy(this RectInt rectInt)
        {
            Vector2Int pos = new Vector2Int(rectInt.position.x, rectInt.position.y);
            Vector2Int size = new Vector2Int(rectInt.size.x, rectInt.size.y);
            return new RectInt(pos, size);
        }
    }
}