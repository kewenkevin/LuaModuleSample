using ND.UI.NDUI.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace ND.UI.NDUI
{
    
    public class NDLine : MaskableGraphic 
    {
        public bool drawAble = false;
        public int segments = 10;
        public float linewidth = 10;
        public Vector2[] points = new []
        {
            new Vector2(){x=0,y=0},
            new Vector2(){x=10,y=10},
        };
        
        /**
    * points 为需要穿过的点
    * segments 为曲线细分度
    * linewidth 为曲线粗细
    */
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            vh.DrawBeziers(points, segments, linewidth,color);
        }

       

    }
}