using System;
using System.Collections.Generic;
using UnityEngine;

namespace ND.UI.NDUI.Utility
{
    public static class ImageFlipUVUtility
    {
        private static readonly int[] FLIP_X_Y_ORDERS = {2, 3, 0, 1};
        private static readonly int[] FLIP_X_ORDERS = {3, 2, 1, 0};
        private static readonly int[] FLIP_Y_ORDERS = {1, 0, 3, 2};
        
        private static readonly int[] FLIPED_XY_BLOCK_INDICES = {8, 7, 6, 5, 4, 3, 2, 1, 0};
        private static readonly int[] FLIPED_X_BLOCK_INDICES = {6, 7, 8, 3, 4, 5, 0, 1, 2};
        private static readonly int[] FLIPED_Y_BLOCK_INDICES = {2, 1, 0, 5, 4, 3, 8, 7, 6};
        
        private static readonly int[] NORMAL_BLOCK_INDICES = {0, 1, 2, 3, 4, 5, 6, 7, 8};
        
        
        private static readonly List<Vector2> TempVec2List = new List<Vector2>();
        
        public static void CorrectMeshFlippedUV(Mesh mesh, bool flipVertical, bool flipHorizontal) 
        {
            if (flipHorizontal && flipVertical)
            {
                mesh.uv = SortV2By(mesh.uv, FLIP_X_Y_ORDERS, FLIPED_XY_BLOCK_INDICES);
            }
            else if (flipVertical)
            {
                mesh.uv = SortV2By(mesh.uv, FLIP_Y_ORDERS, FLIPED_Y_BLOCK_INDICES);
            }
            else if(flipHorizontal)
            {
                mesh.uv =  SortV2By(mesh.uv, FLIP_X_ORDERS, FLIPED_X_BLOCK_INDICES);
            }
            
        }

        
        private static Vector2[] SortV2By(Vector2[] source, IReadOnlyList<int> orders, IReadOnlyList<int> flipBlockIndices)
        {
            TempVec2List.Clear();
            var len = Math.Min(36, source.Length) / 4;
            
            var blockIndices = (len > 1 ) ? flipBlockIndices : NORMAL_BLOCK_INDICES;
            
            for (var i = 0; i < len; i++)
            {
                int bIndex;
                if (len < 9 && i>3) //对九宫格中间不填充的特殊索引兼容处理
                {
                    bIndex = blockIndices[i+1] * 4;                    
                }
                else
                {
                    bIndex = blockIndices[i] * 4;
                }
                
                for (var j = 0; j < 4; j++)
                {
                    var sIndex = bIndex + orders[j];
                    
                    if (len <9 && sIndex>16) //对九宫格中间不填充的特殊索引兼容处理 
                    {
                        sIndex -= 4;
                    }
                    
                    TempVec2List.Add(source[sIndex]);
                }
            }
            
            return TempVec2List.ToArray();
        }
    }
}