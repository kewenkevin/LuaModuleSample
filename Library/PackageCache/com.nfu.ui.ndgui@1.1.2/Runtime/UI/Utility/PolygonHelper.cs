using System.Collections.Generic;
using ND.Core.DataStruct.Geometry;
using ND.Core.Geometry;
using UnityEngine;
using UnityEngine.UI;

namespace ND.UI.NDUI.Utility
{
    public static class PolygonHelper
    {
        public static void AddUIVertexTriangles(this VertexHelper vh, UIVertex[] verts)
        {
            for (int i = 0; i < verts.Length; i += 3)
            {
                int currentVertCount = vh.currentVertCount;

                for (int index = i; index < i + 3; index++)
                {
                    vh.AddVert(verts[index].position, verts[index].color, verts[index].uv0, verts[index].uv1,
                        verts[index].normal, verts[index].tangent);
                }

                vh.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
            }
        }


        public static void AddUIVertexTriangle(this VertexHelper vh, UIVertex[] verts)
        {
            int currentVertCount = vh.currentVertCount;
            for (int index = 0; index < 3; ++index)
                vh.AddVert(verts[index].position, verts[index].color, verts[index].uv0, verts[index].uv1,
                    verts[index].normal, verts[index].tangent);
            vh.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
        }


        private static void vertex(this List<UIVertex> vertices, float x, float y, Color color)
        {
            UIVertex v = UIVertex.simpleVert;
            v.color = color;
            v.position = new Vector2(x, y);
            v.uv0 = Vector2.zero;
        }



        //画圆形

        public static void circle(this VertexHelper vh, Vector2 pos, float radius, Color color, bool filled = false)
        {
            circle(vh, pos.x, pos.y, radius, Mathf.Max(1, (int) (6 * Mathf.Pow(radius, 1 / 3f))), color, filled);
        }

        public static void circle(this VertexHelper vh, float x, float y, float radius, Color color,
            bool filled = false)
        {
            circle(vh, x, y, radius, Mathf.Max(1, (int) (6 * Mathf.Pow(radius, 1 / 3f))), color, filled);
        }

        public static void circle(this VertexHelper vh, float x, float y, float radius, int segments, Color color,
            bool filled = false)
        {
            if (segments > 0)
            {
                float angle = 2 * Mathf.PI / segments;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);
                float cx = radius, cy = 0;

                List<UIVertex> vs = new List<UIVertex>();

                segments--;
                for (int i = 0; i < segments; i++)
                {
                    vs.vertex(x, y, color);
                    vs.vertex(x + cx, y + cy, color);
                    float temp = cx;
                    cx = cos * cx - sin * cy;
                    cy = sin * temp + cos * cy;
                    vs.vertex(x + cx, y + cy, color);
                }

                vs.vertex(x, y, color);

                vs.vertex(x + cx, y + cy, color);

                cx = radius;
                cy = 0;
                vs.vertex(x + cx, y + cy, color);

                vh.AddUIVertexTriangles(vs.ToArray());
            }
        }


        public static void DrawBeziers(this VertexHelper vh, Vector2[] points, float segment, float width,Color color)
        {

            List<BezierCurve> beziers = CreateBeziers(points);

            if (beziers != null)
            {
                for (int i = 0; i < beziers.Count; i++)
                {
                    BezierCurve bezier = beziers[i];
                    DrawBezier(vh, bezier, segment, width,color);
                }
            }
        }



        private static void DrawBezier(VertexHelper vh, BezierCurve bezier, float segment, float width,Color color)
        {
            List<Vector2> lpos = new List<Vector2>();
            List<Vector2> rpos = new List<Vector2>();
            for (int i = 0; i <= segment; i++)
            {
                Vector2 bezierPos = bezier.GetPosition((float) i / (float) segment);
                Vector2 bezierSpeed = bezier.GetSpeedVector((float) i / (float) segment);
                Vector2 offseta = bezierSpeed.normalized.Rotate90() * (0.5f * width);
                Vector2 offsetb = bezierSpeed.normalized.Rotate90(true) * (0.5f * width);

                lpos.Add(bezierPos + offseta);
                rpos.Add(bezierPos + offsetb);
            }

            for (int j = 0; j < segment; j++)
            {
                vh.AddUIVertexQuad(GetQuad(color,lpos[j], lpos[j + 1], rpos[j + 1], rpos[j]));
            }
        }



        private static UIVertex[] GetQuad(Color color,params Vector2[] vertPos)
        {
            UIVertex[] vs = new UIVertex[4];
            Vector2[] uv = new Vector2[4];
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(0, 1);
            uv[2] = new Vector2(1, 0);
            uv[3] = new Vector2(1, 1);
            for (int i = 0; i < 4; i++)
            {
                UIVertex v = UIVertex.simpleVert;
                v.color = color;
                v.position = vertPos[i];
                v.uv0 = uv[i];
                vs[i] = v;
            }

            return vs;
        }

        private static List<BezierCurve> CreateBeziers(Vector2[] points)
        {
            float scale = 0.6f;

            List<BezierCurve> beziers = new List<BezierCurve>();

            int originCount = points.Length - 1;


            List<Vector2> midpoints = new List<Vector2>();
            for (int i = 0; i < originCount; i++)
            {
                midpoints.Add(new Vector2(
                    Mathf.Lerp(points[i].x, points[i + 1].x, 0.5f),
                    Mathf.Lerp(points[i].y, points[i + 1].y, 0.5f))
                );
            }
            
            Vector2[] ctrlPoints = new Vector2[originCount*2];
            float offsetx;
            float offsety;
            
            ctrlPoints[0] = new Vector2(points[0].x, points[0].y);
            for (int i = 0; i < originCount - 1; i++)
            {
                Vector2 orginPoint = points[i + 1];

                offsetx = orginPoint.x - Mathf.Lerp(midpoints[i].x, midpoints[i + 1].x, 0.5f);
                offsety = orginPoint.y - Mathf.Lerp(midpoints[i].y, midpoints[i + 1].y, 0.5f);
                

                ctrlPoints[i * 2 + 1] = new Vector2(midpoints[i].x + offsetx, midpoints[i].y + offsety);
                ctrlPoints[i * 2 + 2] = new Vector2(midpoints[i+ 1].x + offsetx, midpoints[i+ 1].y + offsety);


                ctrlPoints[i * 2 + 1] = Vector2.Lerp(orginPoint, ctrlPoints[i * 2 + 1], scale);
                ctrlPoints[i * 2 + 2] = Vector2.Lerp(orginPoint, ctrlPoints[i * 2 + 2], scale);
            }

            ctrlPoints[originCount * 2 - 1] = new Vector2(points[points.Length - 1].x, points[points.Length - 1].y);
            

            for (int i = 0; i < originCount; i++)
            {
                BezierCurve bezier = getBezier(points[i], ctrlPoints[i * 2], ctrlPoints[i * 2 + 1], points[i + 1]);
                beziers.Add(bezier);
            }

            return beziers;
        }


        public static BezierCurve getBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return new BezierCurve(p0, p1, p2, p3);
        }

    }
}