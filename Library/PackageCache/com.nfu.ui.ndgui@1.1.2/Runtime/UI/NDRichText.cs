using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Vector2=UnityEngine.Vector2;
using Vector3=UnityEngine.Vector3;
using System;
using ND.UI.NDEvents;

namespace ND.UI.NDUI
{
    [AddComponentMenu("NDUI/NDRichText", 10)]
    [ExecuteAlways]
    public class NDRichText : NDText, IPointerClickHandler
    {
        [TextArea(3, 10)]
        [SerializeField]
        private string m_RichText;
        private StringBuilder m_TextBuilder = new StringBuilder();
        private UIVertex m_TempVertex = UIVertex.simpleVert;
        List<int> m_LineStartIndex = new List<int>();

        [Serializable]
        public class RichTextHrefEvent: UnityEvent2<string>{ }

        [SerializeField]
        private RichTextHrefEvent m_OnClick = new RichTextHrefEvent();

        public RichTextHrefEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }

        [SerializeField]
        private bool m_Dirty = false;

        public delegate GameObject GetPrefab(string path);
        public delegate void ReleasePrefab(GameObject go);

        public GetPrefab OnGetPrefab;
        public ReleasePrefab OnReleasePrefab;

        public enum LineAlignment
        {
            Down,
            Mid,
            Up
        }

        [SerializeField]
        private LineAlignment m_LineAlignment;

        public LineAlignment lineAlignment
        {
            get
            {
                return m_LineAlignment;
            }
            set
            {
                if (m_LineAlignment == value)
                    return;
                m_LineAlignment = value;

                SetVerticesDirty();
                SetLayoutDirty();
            }
        }

        protected override void OnEnable()
        {
            supportRichText = true;
            alignByGeometry = true;
            m_Text = Parse(m_RichText);
            if(!string.IsNullOrEmpty(m_LocalizationKey) && !string.Equals(m_Text, m_LocalizationKey))
            {
                m_LocalizationKey = null;
            }
            base.OnEnable();
        }

        public string richText
        {
            get
            {
                return m_RichText;
            }
            set
            {
                m_LocalizationKey = string.Empty;
                if (string.IsNullOrEmpty(value))
                {
                    if (string.IsNullOrEmpty(m_Text))
                    {
                        return;
                    }
                    m_Text = "";
                    SetVerticesDirty();
                }
                else if (m_RichText != value)
                {
                    m_Text = Parse(value);
                    SetVerticesDirty();
                    SetLayoutDirty();
                }
                //缓存一个
                m_RichText = value;
            }
        }

        protected override void Set(float value)
        {
            base.Set(value);
            richText = text;
        }

        class HrefTagInfo
        {
            public int startIndex;
            public int overrideStartIndex;
            public int endIndex;
            public int overrideEndIndex;
            public string param;
            public List<Rect> boxes;
        }

        class PrefabTagInfo
        {
            public int startIndex;
            public int overrideStartIndex;
            public string path;
            public float width;
            public float height;
            public float scale;
            public UnityEngine.Vector3 pos;
            public bool used = false;
            public float showHeight { get { return height * scale; } }
        }

        [SerializeField]
        private Stack<string> m_HrefParamStack = new Stack<string>();
        [SerializeField]
        private List<HrefTagInfo> m_HrefTagInfos = new List<HrefTagInfo>();
        [SerializeField]
        private List<PrefabTagInfo> m_PrefabTagInfos = new List<PrefabTagInfo>();
        [SerializeField]
        private List<GameObject> m_ShowPrefabs = new List<GameObject>();

        private string Parse(string sourceString)
        {
            m_HrefParamStack.Clear();
            m_HrefTagInfos.Clear();
            m_PrefabTagInfos.Clear();
            m_TextBuilder.Clear();
            if (string.IsNullOrEmpty(sourceString))
            {
                return "";
            }
            bool hrefVaild = AllHrefTagVaild(sourceString);
            int subIndex = 0;
            for (int i = 0; i < sourceString.Length; i++)
            {
                char c = sourceString[i];
                int index = 0;
                if (c == '<')
                {
                    if (IsTagName(sourceString, "<href=", i, ref index) && hrefVaild)
                    {
                        int closeIndex = 0;
                        string param = "";
                        if (CloseOfHrefStartTag(sourceString, index, ref closeIndex, ref param))
                        {
                            string part = sourceString.Substring(subIndex, i - subIndex);
                            if (m_HrefParamStack.Count == 0)
                            {
                                m_TextBuilder.Append(part);
                                m_HrefParamStack.Push(param);
                            }
                            else
                            {
                                HrefTagInfo info = new HrefTagInfo();
                                info.startIndex = m_TextBuilder.Length * 4;
                                m_TextBuilder.Append(part);
                                info.endIndex = m_TextBuilder.Length * 4 - 1;
                                info.param = m_HrefParamStack.Peek();
                                m_HrefTagInfos.Add(info);
                                m_HrefParamStack.Push(param);
                            }
                            i = closeIndex;
                            subIndex = i + 1;
                        }
                    }
                    else if (IsTagName(sourceString, "</href>", i, ref index) && hrefVaild && m_HrefParamStack.Count > 0)
                    {
                        string part = sourceString.Substring(subIndex, i - subIndex);
                        string param = m_HrefParamStack.Pop();
                        HrefTagInfo info = new HrefTagInfo();
                        info.startIndex = m_TextBuilder.Length * 4;
                        m_TextBuilder.Append(part);
                        info.endIndex = m_TextBuilder.Length * 4 - 1;
                        info.param = param;
                        m_HrefTagInfos.Add(info);
                        i += 6;
                        subIndex = i + 1;
                    }
                    else if (IsTagName(sourceString, "<p=", i, ref index))
                    {
                        int closeIndex = 0;
                        if (PrefabTagClose(sourceString, index, ref closeIndex))
                        {
                            string path = "";
                            float w = 0;
                            float h = 0;
                            float scale = 0;
                            if (GetPrefabParams(sourceString, index - 3, closeIndex, ref path, ref w, ref h, ref scale))
                            {
                                string part = sourceString.Substring(subIndex, i - subIndex);
                                bool inHref = m_HrefParamStack.Count > 0;
                                if (inHref) //嵌套在href
                                {
                                    HrefTagInfo hrefInfo = new HrefTagInfo();
                                    hrefInfo.startIndex = m_TextBuilder.Length * 4;
                                    m_TextBuilder.Append(part);
                                    hrefInfo.endIndex = m_TextBuilder.Length * 4 - 1;
                                    hrefInfo.param = m_HrefParamStack.Peek();
                                    m_HrefTagInfos.Add(hrefInfo);
                                }
                                else
                                {
                                    m_TextBuilder.Append(part);
                                }
                                string replace = string.Format("<quad size={0} width={1}/>", (h * scale).ToString(), (w / h).ToString());
                                PrefabTagInfo pInfo = new PrefabTagInfo();
                                pInfo.startIndex = m_TextBuilder.Length * 4;
                                m_TextBuilder.Append(replace);
                                pInfo.path = path;
                                pInfo.width = w;
                                pInfo.height = h;
                                pInfo.scale = scale;
                                m_PrefabTagInfos.Add(pInfo);
                                if (inHref)
                                {
                                    HrefTagInfo hInfo = new HrefTagInfo();
                                    hInfo.startIndex = pInfo.startIndex;
                                    hInfo.endIndex = m_TextBuilder.Length * 4 - 1;
                                    hInfo.param = m_HrefParamStack.Peek();
                                    m_HrefTagInfos.Add(hInfo);
                                }
                                i = closeIndex;
                                subIndex = i + 1;
                            }
                        }
                    }
                }
            }
            if (subIndex < sourceString.Length)
            {
                m_TextBuilder.Append(sourceString.Substring(subIndex));
            }
            string finStr = m_TextBuilder.ToString();
            return finStr;
        }

        private bool AllHrefTagVaild(string sourceString)
        {
            int hrefStartTagCount = 0;
            int hrefEndTagCount = 0;
            for (int i = 0; i < sourceString.Length; i++)
            {
                char c = sourceString[i];
                int index = 0;
                if (c == '<')
                {
                    if (IsTagName(sourceString, "<href=", i, ref index))
                    {
                        int closeIndex = 0;
                        if (HrefStartTagVaild(sourceString, index, ref closeIndex))
                        {
                            hrefStartTagCount++;
                            i = closeIndex;
                        }
                    }
                    else if (IsTagName(sourceString, "</href>", i, ref index))
                    {
                        hrefEndTagCount++;
                    }
                }
            }
            return hrefStartTagCount == hrefEndTagCount;
        }

        private bool IsTagName(string text, string tag, int index, ref int startIndex)
        {
            if (text.Length < index + tag.Length)
            {
                return false;
            }
            for (int i = 0; i < tag.Length; i++)
            {
                if (char.ToLower(text[index + i]) != tag[i])
                {
                    return false;
                }
            }
            startIndex = index + tag.Length;
            return true;
        }


        private bool PrefabTagClose(string text, int index, ref int closeIndex)
        {
            closeIndex = 0;
            for (int i = index; i < text.Length - 1; i++)
            {
                // Break at '/>'
                if (text[i] == '/' && text[i + 1] == '>')
                {
                    closeIndex = i + 1;
                    return true;
                }
            }
            return false;
        }

        private bool CloseOfHrefStartTag(string text, int index, ref int closeIndex, ref string param)
        {
            for (int i = index; i < text.Length; i++)
            {
                // Break at '>'
                if (text[i] == '>')
                {
                    closeIndex = i;
                    param = text.Substring(index, closeIndex - index);
                    return true;
                }
            }
            return false;
        }

        private bool HrefStartTagVaild(string text, int index, ref int closeIndex)
        {
            for (int i = index; i < text.Length; i++)
            {
                // Break at '>'
                if (text[i] == '>')
                {
                    closeIndex = i;
                    return true;
                }
            }
            return false;
        }

        private bool GetPrefabParams(string text, int startIndex, int endIndex, ref string name, ref float width, ref float height, ref float scale)
        {
            name = "";
            width = fontSize;
            height = fontSize;
            scale = 1;
            string tag = text.Substring(startIndex, endIndex - startIndex - 1);
            bool vaild = true;
            for (int i = 0; i < tag.Length; i++)
            {
                char cc = tag[i];
                char c = char.ToLower(cc);
                if (c == 'p')
                {
                    int pCloseIndex = 0;
                    if (!CheckPrefabTag(tag, "p=", i, ref pCloseIndex, ref name))
                    {
                        vaild = false;
                        break;
                    }
                    i = pCloseIndex;
                }
                else if (c == 'w')
                {
                    int wCloseIndex = 0;
                    if (!CheckPrefabSubTag(tag, "w=", i, ref wCloseIndex, ref width))
                    {
                        vaild = false;
                        break;
                    }
                    i = wCloseIndex;
                }
                else if (c == 'h')
                {
                    int hCloseIndex = 0;
                    if (!CheckPrefabSubTag(tag, "h=", i, ref hCloseIndex, ref height))
                    {
                        vaild = false;
                        break;
                    }
                    i = hCloseIndex;
                }
                else if (c == 's')
                {
                    int sCloseIndex = 0;
                    if (!CheckPrefabSubTag(tag, "s=", i, ref sCloseIndex, ref scale))
                    {
                        vaild = false;
                        break;
                    }
                    i = sCloseIndex;
                }
            }
            return vaild;
        }

        private bool CheckPrefabTag(string tag, string check, int index, ref int closeIndex, ref string value)
        {
            int startIndex = 0;
            closeIndex = 0;
            bool vaild = true;
            if (IsTagName(tag, check, index, ref startIndex))
            {
                string s = PrefabSubTagClose(tag, startIndex, ref closeIndex);
                if (string.IsNullOrEmpty(s))
                {
                    vaild = false;
                }
                else
                {
                    value = s;
                }
            }
            else
            {
                vaild = false;
            }
            return vaild;
        }

        private bool CheckPrefabSubTag(string tag, string check, int index, ref int closeIndex, ref float value)
        {
            int startIndex = 0;
            closeIndex = 0;
            bool vaild = true;
            if (IsTagName(tag, check, index, ref startIndex))
            {
                string s = PrefabSubTagClose(tag, startIndex, ref closeIndex);
                if (string.IsNullOrEmpty(s))
                {
                    vaild = false;
                }
                else
                {
                    if (!float.TryParse(s, out value))
                    {
                        vaild = false;
                    }
                }
            }
            else
            {
                vaild = false;
            }
            return vaild;
        }

        private string PrefabSubTagClose(string sourceString, int index, ref int closeIndex)
        {
            string subContent = null;
            for (int i = index; i < sourceString.Length; i++)
            {
                char c = sourceString[i];
                if (c == ' ')
                {
                    subContent = sourceString.Substring(index, i - index);
                    closeIndex = i;
                    return subContent;
                }
            }
            closeIndex = sourceString.Length - 1;
            subContent = sourceString.Substring(index);
            return subContent;
        }

        private UIVertex[] m_TempVerts = new UIVertex[4];
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (font == null)
                return;

            m_DisableFontTextureRebuiltCallback = true;

            UnityEngine.Vector2 extents = rectTransform.rect.size;
            var settings = GetGenerationSettings(extents);
            cachedTextGenerator.PopulateWithErrors(text, settings, gameObject);

            List<UIVertex> verts = new List<UIVertex>();
            cachedTextGenerator.GetVertices(verts);
            float unitsPerPixel = 1 / pixelsPerUnit;
            int vertCount = verts.Count;

            // We have no verts to process just return (case 1037923)
            if (vertCount <= 0)
            {
                toFill.Clear();
                return;
            }

            if (vertCount > 0)
            {
                SetLineStartIndex();  //设置行换位信息
                //设置预设标签信息
                SetPrefabTagValid(vertCount); 
                bool hideRich = HideRichVertex();
                if (hideRich)
                {
                    DealTagOverrideIndex();
                }

                bool needRichAlign = m_LineAlignment != LineAlignment.Down && !AllPrefabInvaild();  //需要使用富文本居中 居上排布
                //开始输入顶点信息
                UnityEngine.Vector2 roundingOffset = new UnityEngine.Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
                roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
                toFill.Clear();
                if (needRichAlign)
                {
                    AdjustVertexHeight(verts, toFill, unitsPerPixel, roundingOffset);
                }
                else
                {
                    if (roundingOffset != UnityEngine.Vector2.zero)
                    {
                        for (int i = 0; i < vertCount; ++i)
                        {
                            int tempVertsIndex = i & 3;
                            m_TempVerts[tempVertsIndex] = verts[i];
                            m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                            m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                            m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                            if (tempVertsIndex == 3)
                                toFill.AddUIVertexQuad(m_TempVerts);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < vertCount; ++i)
                        {
                            int tempVertsIndex = i & 3;
                            m_TempVerts[tempVertsIndex] = verts[i];
                            m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                            if (tempVertsIndex == 3)
                                toFill.AddUIVertexQuad(m_TempVerts);
                        }
                    }

                }
            }
            DealPrefabTagInfo(toFill);
            DealHrefTagInfo(verts, toFill);
            m_Dirty = true;
            m_DisableFontTextureRebuiltCallback = false;
        }

        private void DealTagOverrideIndex()
        {
            List<UICharInfo> charInfos = new List<UICharInfo>();
            cachedTextGenerator.GetCharacters(charInfos);
            int lastIndex = 0;
            int lastOverrideIndex = 0;
            for (int i = 0; i < m_PrefabTagInfos.Count; i++)
            {
                PrefabTagInfo pInfo = m_PrefabTagInfos[i];
                int index = pInfo.startIndex;
                pInfo.overrideStartIndex = GetOverrideIndex(charInfos, lastIndex, index, lastOverrideIndex);
                lastIndex = index;
                lastOverrideIndex = pInfo.overrideStartIndex;
            }
            int sLastIndex = 0;
            int sLastOverrideIndex = 0;
            int eLastIndex = 0;
            int eLastOverrideIndex = 0;
            for (int i = 0; i < m_HrefTagInfos.Count; i++)
            {
                HrefTagInfo hInfo = m_HrefTagInfos[i];
                int sIndex = hInfo.startIndex;
                hInfo.overrideStartIndex = GetOverrideIndex(charInfos, sLastIndex, sIndex, sLastOverrideIndex);
                sLastIndex = sIndex;
                sLastOverrideIndex = hInfo.overrideStartIndex;
                int eIndex = hInfo.endIndex + 1;
                hInfo.overrideEndIndex = GetOverrideIndex(charInfos, eLastIndex, eIndex, eLastOverrideIndex) - 1;
                eLastIndex = eIndex;
                eLastOverrideIndex = hInfo.overrideEndIndex + 1;
            }
        }

        private int GetOverrideIndex(List<UICharInfo> charInfos, int startIndex, int index, int overrideIndex)
        {
            int charIndex = startIndex / 4;
            for (int i = startIndex; i < index; i++)
            {
                int tempIndex = i & 3;
                if (tempIndex == 0)
                {
                    UICharInfo cInfo = charInfos[charIndex];
                    float charW = cInfo.charWidth;
                    if (charW != 0 && text[charIndex] != ' ')
                    {
                        overrideIndex += 4;
                    }
                    charIndex++;
                }
            }
            return overrideIndex;
        }

        private void AdjustVertexHeight(IList<UIVertex> verts, VertexHelper toFill, float unitsPerPixel, UnityEngine.Vector2 roundingOffset)
        {
            PrefabTagInfo info = null;
            List<PrefabTagInfo> infoList = new List<PrefabTagInfo>();
            float heightUp = float.MinValue;
            float heightBottom = float.MaxValue;
            int curLine = -1;
            bool newLine = false;
            bool isQuad = false;
            int lastStartIndex = 0;
            for (int i = 0; i < verts.Count; i += 2)
            {
                m_TempVertex = verts[i];
                int tempIndex = i & 3;
                if (tempIndex == 0)
                {
                    int line = GetLine(i);
                    if (curLine != line)
                    {
                        newLine = true;
                        curLine = line;
                    }
                }
                if (newLine)
                {
                    //新的一行  先计算上一行的内容  
                    DealVertex(verts, toFill, heightUp, heightBottom, infoList, lastStartIndex, i, unitsPerPixel, roundingOffset);
                    newLine = false;
                    info = null;
                    infoList.Clear();
                    heightUp = float.MinValue;
                    heightBottom = float.MaxValue;
                    lastStartIndex = i;
                    i = i - 2;
                }
                else
                {
                    if (tempIndex == 0)
                    {
                        isQuad = IsQuadIndex(i, ref info);
                        if (isQuad)
                        {
                            infoList.Add(info);
                        }
                        else
                        {
                            heightUp = m_TempVertex.position.y > heightUp ? m_TempVertex.position.y : heightUp;
                        }
                    }
                    else if (tempIndex == 2)
                    {
                        if (!isQuad)
                        {
                            heightBottom = m_TempVertex.position.y < heightBottom ? m_TempVertex.position.y : heightBottom;
                        } 
                    }
                }
            }
            DealVertex(verts, toFill, heightUp, heightBottom, infoList, lastStartIndex, verts.Count, unitsPerPixel, roundingOffset);
        }
    
        private void DealVertex(IList<UIVertex> verts, VertexHelper toFill, float heightUp, float heightBottom, List<PrefabTagInfo> infoList, int lastStartIndex, int curIndex, float unitsPerPixel, UnityEngine.Vector2 roundingOffset)
        {
            if (curIndex <= 0)
            {
                return;
            }
            if (infoList.Count > 0)
            {
                infoList.Sort((a, b) =>
                {
                    if (a.showHeight > b.showHeight)
                    {
                        return -1;
                    }
                    else if (a.showHeight == b.showHeight)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                });
            }
           
            float max = infoList.Count > 0 ? infoList[0].showHeight : 0;
            float lineHeight = 0;
            if (heightUp != float.MinValue && heightBottom != float.MaxValue)
            {
                lineHeight = heightUp - heightBottom;
            }
            if (max > lineHeight) //图比字大
            {
                float fontOffset = m_LineAlignment == LineAlignment.Mid ? (max - lineHeight) / 2 : max - lineHeight;
                for (int j = lastStartIndex; j < curIndex; j++)
                {
                    int tempIndex = j & 3;
                    m_TempVerts[tempIndex] = verts[j];
                    if (roundingOffset != UnityEngine.Vector2.zero)
                    {
                        m_TempVerts[tempIndex].position *= unitsPerPixel;
                        m_TempVerts[tempIndex].position.x += roundingOffset.x;
                        m_TempVerts[tempIndex].position.y += roundingOffset.y;
                    }
                    else
                    {
                        m_TempVerts[tempIndex].position *= unitsPerPixel;
                    }
                    m_TempVerts[tempIndex].position.y += fontOffset;
                    if (tempIndex == 3)
                    {
                        toFill.AddUIVertexQuad(m_TempVerts);
                    }
                }
                bool autoLF = HideRichVertex();
                for (int k = 0; k < infoList.Count; k++)
                {
                    PrefabTagInfo tempInfo = infoList[k];
                    float prefabOffset = m_LineAlignment == LineAlignment.Mid ? (max - tempInfo.showHeight) / 2 : max - tempInfo.showHeight;
                    int prefabVerIndex = HideRichVertex() ? tempInfo.overrideStartIndex : tempInfo.startIndex;
                    for (int l = 0; l < 4; l++)
                    {
                        toFill.PopulateUIVertex(ref m_TempVertex, prefabVerIndex + l); 
                        m_TempVertex.position.y = m_TempVertex.position.y - fontOffset + prefabOffset;
                        toFill.SetUIVertex(m_TempVertex, prefabVerIndex + l);
                    }
                }
            }
            else
            {
                bool autoLF = HideRichVertex();
                for (int j = 0; j < infoList.Count; j++)
                {
                    PrefabTagInfo tempInfo = infoList[j];
                    float prefabOffset = m_LineAlignment == LineAlignment.Mid ? (lineHeight - tempInfo.showHeight) / 2 : lineHeight - tempInfo.showHeight;
                    if (prefabOffset > 0)
                    {
                        int prefabVerIndex = HideRichVertex() ? tempInfo.overrideStartIndex : tempInfo.startIndex;
                        float heightOffset = 0;
                        m_TempVertex = verts[prefabVerIndex + 2];
                        heightOffset = m_TempVertex.position.y - heightBottom;
                        for (int l = 0; l < 4; l++)
                        {
                            m_TempVertex = verts[prefabVerIndex + l];
                            m_TempVertex.position.y = m_TempVertex.position.y + prefabOffset - heightOffset;
                            verts[prefabVerIndex + l] = m_TempVertex;
                        }
                    }
                }
                for (int k = lastStartIndex; k < curIndex; k++)
                {
                    int tempIndex = k & 3;
                    m_TempVerts[tempIndex] = verts[k];
                    if (roundingOffset != UnityEngine.Vector2.zero)
                    {
                        m_TempVerts[tempIndex].position *= unitsPerPixel;
                        m_TempVerts[tempIndex].position.x += roundingOffset.x;
                        m_TempVerts[tempIndex].position.y += roundingOffset.y;
                    }
                    else
                    {
                        m_TempVerts[tempIndex].position *= unitsPerPixel;
                    }
                    if (tempIndex == 3)
                    {
                        toFill.AddUIVertexQuad(m_TempVerts);
                    }
                }
            }
        }

        private void DealPrefabTagInfo(VertexHelper toFill)
        {
            int index = -1;
            for (int i = 0; i < m_PrefabTagInfos.Count; i++)
            {
                PrefabTagInfo info = m_PrefabTagInfos[i];
                if (info.used)
                {
                    index = HideRichVertex() ? info.overrideStartIndex : info.startIndex;
                    info.used = true;
                    float X = 0;
                    float Y = 0;
                    for (int j = index; j < index + 4; j++)
                    {
                        toFill.PopulateUIVertex(ref m_TempVertex, j);
                        //清理多余的乱码uv
                        m_TempVertex.uv0 = Vector2.zero;
                        X += m_TempVertex.position.x;
                        Y += m_TempVertex.position.y;
                        toFill.SetUIVertex(m_TempVertex, j);
                    }
                    info.pos = new Vector3(X / 4, Y / 4, 0);
                }
            }
        }

        private void SetPrefabTagValid(int verCount)
        {
            for (int i = 0; i < m_PrefabTagInfos.Count; i++)
            {
                PrefabTagInfo info = m_PrefabTagInfos[i];
                int index = HideRichVertex() ? info.overrideStartIndex : info.startIndex;
                if ((index + 4) <= verCount)
                {
                    info.used = true;
                }
                else
                {
                    info.used = false;
                }
            }
        }

        private bool AllPrefabInvaild()
        {
            for (int i = 0; i < m_PrefabTagInfos.Count; i++)
            {
                PrefabTagInfo info = m_PrefabTagInfos[i];
                if (info.used)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsQuadIndex(int index, ref PrefabTagInfo prefabTag)
        {
            bool isQuadIndex = false;
            for (int i = 0; i < m_PrefabTagInfos.Count; i++)
            {
                PrefabTagInfo info = m_PrefabTagInfos[i];
                int quadIndex = HideRichVertex() ? info.overrideStartIndex : info.startIndex;
                if (index == quadIndex)
                {
                    isQuadIndex = true;
                    prefabTag = info;
                    break;
                }
            }
            return isQuadIndex;
        }

        private void DealHrefTagInfo(IList<UIVertex> verts, VertexHelper toFill)
        {
            int startIndex = -1;
            int endIndex = -1;
            for (int i = 0; i < m_HrefTagInfos.Count; i++)
            {
                HrefTagInfo info = m_HrefTagInfos[i];
                info.boxes = new List<Rect>();
                startIndex = HideRichVertex() ? info.overrideStartIndex : info.startIndex;
                endIndex = HideRichVertex() ? info.overrideEndIndex : info.endIndex;
                if (endIndex < startIndex)
                {
                    continue;
                }
                if (startIndex >= verts.Count)
                {
                    continue;
                }
                toFill.PopulateUIVertex(ref m_TempVertex, startIndex);
                toFill.SetUIVertex(m_TempVertex, startIndex);
                //将超链接里面的文本顶点索引坐标加入到包围框  
                var pos = m_TempVertex.position;
                var bounds = new Bounds(pos, Vector3.zero);
                int curLine = GetLine(startIndex);
                bool newLine = false;
                for (int j = startIndex + 1; j <= endIndex; j++)
                {
                    if (j >= toFill.currentVertCount)
                    {
                        break;
                    }
                    if ((j & 3) == 0)
                    {
                        int line = GetLine(j);
                        if (curLine != line)
                        {
                            newLine = true;
                            curLine = line;
                        }
                    }
                    toFill.PopulateUIVertex(ref m_TempVertex, j);
                    pos = m_TempVertex.position;
                    if (newLine)
                    {
                        newLine = false;
                        //换行重新添加包围框  
                        info.boxes.Add(new Rect(bounds.min, bounds.size));
                        bounds = new Bounds(pos, Vector3.zero);
                    }
                    else
                    {
                        //扩展包围框  
                        bounds.Encapsulate(pos);
                    }
                    toFill.SetUIVertex(m_TempVertex, j);
                }
                //添加包围盒
                info.boxes.Add(new Rect(bounds.min, bounds.size));
            }
        }

        private int GetLine(int vertexIndex) 
        {
            if (m_LineStartIndex.Count <= 1)
            {
                return 0;
            }

            for (int i = 0; i < m_LineStartIndex.Count - 1; i++)
            {
                if (vertexIndex>= m_LineStartIndex[i] && vertexIndex < m_LineStartIndex[i + 1])
                {
                    return i;
                }
            }

            return m_LineStartIndex.Count - 1;
        }

        private void SetLineStartIndex() 
        {
            bool hideRich = HideRichVertex();
            List<UILineInfo> lineInfos = new List<UILineInfo>();
            cachedTextGenerator.GetLines(lineInfos);
            if (lineInfos.Count <= 0)
            {
                return;
            }
            m_LineStartIndex.Clear();
            if (!hideRich)
            {
                foreach (var item in lineInfos)
                {
                    m_LineStartIndex.Add(item.startCharIdx * 4);
                }
            }
            else
            {
                List<UICharInfo> charInfos = new List<UICharInfo>();
                cachedTextGenerator.GetCharacters(charInfos);
                int startIndex = 0;
                int overrideIndex = 0;
                for (int i = 0; i < lineInfos.Count; i++)
                {
                    int index = lineInfos[i].startCharIdx * 4;
                    int newIndex = GetOverrideIndex(charInfos, startIndex, index, overrideIndex);
                    m_LineStartIndex.Add(newIndex);
                    startIndex = index;
                    overrideIndex = newIndex;
                }
            }
        }

        private bool HideRichVertex()
        {
            if (cachedTextGenerator.vertexCount <= 0)
            {
                return false;
            }
            bool hide = !(cachedTextGenerator.characterCount * 4 == cachedTextGenerator.vertexCount);
            return hide;
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                return;
            }
#endif
            DrawTag();
        }

        public void DrawTag()
        {
            if (m_Dirty)
            {
                CreateTag();
                m_Dirty = false;
            }
        }

        private void CreateTag()
        {
            for (int i = m_ShowPrefabs.Count - 1; i >= 0; i--)
            {
                GameObject rawGo = m_ShowPrefabs[i];
                if (OnReleasePrefab == null)
                {
                    GameObject.DestroyImmediate(rawGo);
                }
                else
                {
                    OnReleasePrefab(rawGo);
                }                
            }
            m_ShowPrefabs.Clear();
            for (int i = 0; i < m_PrefabTagInfos.Count; i++)
            {
                PrefabTagInfo info = m_PrefabTagInfos[i];
                if (!info.used)
                {
                    continue;
                }
                GameObject go = null;
                if (OnGetPrefab == null)
                {
#if UNITY_EDITOR
                    GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/" + info.path + ".prefab");
                    if (prefab != null)
                    {
                        go = GameObject.Instantiate(prefab);
                    }
#endif
                }
                else
                {
                    go = OnGetPrefab(info.path);
                } 
                if (go == null)
                {
                    continue;
                }
                RectTransform rectTrans = go.GetComponent<RectTransform>();
                rectTrans.SetParent(this.transform);
                rectTrans.localScale = Vector3.one * info.scale;
                rectTrans.localRotation = UnityEngine.Quaternion.identity;
                rectTrans.pivot = new Vector2(0.5f, 0.5f);
                rectTrans.anchorMin = Vector2.zero;
                rectTrans.anchorMax = Vector2.zero;
                rectTrans.localPosition = info.pos;
                go.hideFlags = HideFlags.NotEditable;
                m_ShowPrefabs.Add(go);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Vector2 lp;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out lp);

            foreach (var info in m_HrefTagInfos)
            {
                var boxes = info.boxes;
                for (var i = 0; i < boxes.Count; ++i)
                {
                    if (boxes[i].Contains(lp))
                    {
                        OnClickHrefRect(info.param);
                        return;
                    }
                }
            }
        }

        private void OnClickHrefRect(string param)
        {
            m_OnClick.Invoke(param);
            Debug.Log(param);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            text = Parse(m_RichText);
        }

        private void OnDrawGizmos()
        {
            if (m_HrefTagInfos == null)
            {
                return;
            }
            Gizmos.color = Color.green;
            for (int i = 0; i < m_HrefTagInfos.Count; i++)
            {
                HrefTagInfo info = m_HrefTagInfos[i];
                if (info.boxes == null)
                {
                    continue;
                }
                for (int j = 0; j < info.boxes.Count; j++)
                {
                    Rect rect = info.boxes[j];
                    Vector3 point = transform.TransformPoint(rect.x, rect.y, 0);
                    Gizmos.DrawLine(new Vector3(point.x, point.y, 0), new Vector3(point.x + rect.width, point.y, 0));
                    Gizmos.DrawLine(new Vector3(point.x, point.y, 0), new Vector3(point.x, point.y + rect.height, 0));
                    Gizmos.DrawLine(new Vector3(point.x, point.y + rect.height, 0), new Vector3(point.x + rect.width, point.y + rect.height, 0));
                    Gizmos.DrawLine(new Vector3(point.x + rect.width, point.y + rect.height, 0), new Vector3(point.x + rect.width, point.y, 0));
                }
            }
        }
#endif
    }
}