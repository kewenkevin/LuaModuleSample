using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ND.UI.NDUI
{
    // [AddComponentMenu("UI/Effects/TextSpacing")]
    public class TextSpacing : BaseMeshEffect
    {
        
        #region Struct

        public enum HorizontalAligmentType
        {
            Left,
            Center,
            Right
        }

        public class Line
        {
            // 起点索引
            public int StartVertexIndex
            {
                get { return _startVertexIndex; }
            }

            private int _startVertexIndex = 0;

            // 终点索引
            public int EndVertexIndex
            {
                get { return _endVertexIndex; }
            }

            private int _endVertexIndex = 0;

            // 该行占的点数目
            public int VertexCount
            {
                get { return _vertexCount; }
            }

            private int _vertexCount = 0;

            public Line(int startVertexIndex, int length)
            {
                _startVertexIndex = startVertexIndex;
                _endVertexIndex = length * 6 - 1 + startVertexIndex;
                _vertexCount = length * 6;
            }
        }

        #endregion

        private float m_spacing;
        private const string m_RichTextRegexPatterns = @"<b>|</b>|<i>|</i>|<size=.*?>|</size>|<Size=.*?>|</Size>|<color=.*?>|</color>|<Color=.*?>|</Color>|<material=.*?>|</material>";
        public float Spacing
        {
            get { return m_spacing; }
            set { m_spacing = value; }
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive() || vh.currentVertCount == 0)
            {
                return;
            }

            Text text = GetComponent<Text>();

            if (text == null)
            {
                Debug.LogError("Missing Text component");
                return;
            }

            // 水平对齐方式
            HorizontalAligmentType alignment;
            if (text.alignment == TextAnchor.LowerLeft || text.alignment == TextAnchor.MiddleLeft ||
                text.alignment == TextAnchor.UpperLeft)
            {
                alignment = HorizontalAligmentType.Left;
            }
            else if (text.alignment == TextAnchor.LowerCenter || text.alignment == TextAnchor.MiddleCenter ||
                     text.alignment == TextAnchor.UpperCenter)
            {
                alignment = HorizontalAligmentType.Center;
            }
            else
            {
                alignment = HorizontalAligmentType.Right;
            }

            var vertexs = new List<UIVertex>();
            vh.GetUIVertexStream(vertexs);
            string[] lineTexts;
            int returnCharOffsetFlag = 0;
            // var indexCount = vh.currentIndexCount;
            if (isSpaceOrReturnMeshExist(vertexs))
            {
                lineTexts = text.text.Split('\n');
                returnCharOffsetFlag = 1;
            }
            else
            {
                lineTexts = text.text.Replace(" ", "").Split('\n');
                returnCharOffsetFlag = 0;
            }
            
            var lines = new Line[lineTexts.Length];
            // 根据lines数组中各个元素的长度计算每一行中第一个点的索引，每个字、字母、空母均占6个点
            for (var i = 0; i < lines.Length; i++)
            {
                if (i == 0)
                {
                    lines[i] = new Line(0, lineTexts[i].Length + returnCharOffsetFlag);
                }
                else if (i > 0 && i < lines.Length - 1)
                {
                    lines[i] = new Line(lines[i - 1].EndVertexIndex + 1, lineTexts[i].Length + returnCharOffsetFlag);
                }
                else
                {
                    lines[i] = new Line(lines[i - 1].EndVertexIndex + 1, lineTexts[i].Length);
                }
            }
            // Debug.Log("[currentIndexCount]"+vh.currentIndexCount);
            // Debug.Log("[currentVertCount]"+vh.currentVertCount);
            UIVertex vt;
            // Debug.Log("[returnCharOffsetFlag]"+returnCharOffsetFlag);
            for (var i = 0; i < lines.Length; i++)
            {
                float wrapOffset = 0;
                for (var j = lines[i].StartVertexIndex; j <= lines[i].EndVertexIndex; j++)
                {
                    if (j < 0 || j >= vertexs.Count)
                    {
                        continue;
                    }

                    vt = vertexs[j];

                    var charCount = lines[i].EndVertexIndex - lines[i].StartVertexIndex;
                    //如果需要计算换行符
                    if (returnCharOffsetFlag == 1)
                    {
                        if (i == lines.Length - 1)
                        {
                            charCount += 6;
                        }
                    }
                    

                    if (j>5 && j%6==0)
                    {
                        // if ((Mathf.Abs(vertexs[j].position.y) - Mathf.Abs(vertexs[j-5].position.y))>5)
                        if (Mathf.Abs(vertexs[j-5].position.y - vertexs[j].position.y) > text.fontSize )
                        {
                            // wrapOffset = Spacing;
                            // wrapOffset = Mathf.Abs(vertexs[j].position.x - vertexs[lines[i].StartVertexIndex].position.x);
                            
                            if (alignment == HorizontalAligmentType.Left)
                            {
                                wrapOffset = Spacing * ((j - lines[i].StartVertexIndex) / 6);
                            }
                            else if (alignment == HorizontalAligmentType.Center)
                            {
                                // var offset = (charCount / 6) % 2 == 0 ? 0.5f : 0f;
                                wrapOffset = Spacing * ((j - lines[i].StartVertexIndex) / 6 - charCount / 12 + 1.5f);
                                // wrapOffset = (j - lines[i].StartVertexIndex) / 6;
                            }
                        }
                        
                    }

                    if (alignment == HorizontalAligmentType.Left)
                    {
                        vt.position += new Vector3(Spacing * ((j - lines[i].StartVertexIndex) / 6) - wrapOffset, 0, 0);
                    }
                    else if (alignment == HorizontalAligmentType.Right)
                    {
                        vt.position += new Vector3(Spacing * (-(charCount - j + lines[i].StartVertexIndex) / 6 - 0.5f), 0,
                            0);
                    }
                    else if (alignment == HorizontalAligmentType.Center)
                    {
                        var offset = (charCount / 6) % 2 == 0 ? 0.5f : 0f;
                        vt.position +=
                            new Vector3(Spacing * ((j - lines[i].StartVertexIndex) / 6 - charCount / 12 + offset) - wrapOffset, 0,
                                0);
                    }

                    vertexs[j] = vt;
                    // 以下注意点与索引的对应关系
                    if (j % 6 <= 2)
                    {
                        vh.SetUIVertex(vt, (j / 6) * 4 + j % 6);
                    }

                    if (j % 6 == 4)
                    {
                        vh.SetUIVertex(vt, (j / 6) * 4 + j % 6 - 1);
                    }
                }
            }
        }
        private bool isSpaceOrReturnMeshExist(List<UIVertex> vertices)
        {
            for (var j = 0; j <= vertices.Count; j+=6)
            {
                if (j < 0 || j >= vertices.Count)
                {
                    continue;
                }
                //遍历四边形面片，6个顶点为一组(2个三角形组成)
                if (j%6==0)
                {
                    // var ranges= vertexs.GetRange(j, 6);
                    //如果是空格或者换行 UGUI的面片均为0体积即6个顶点共点
                    if (vertices[j].position == vertices[j+1].position)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}