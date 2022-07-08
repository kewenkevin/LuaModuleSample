using UnityEngine;

namespace ND.UI.Core
{
    
    /// <summary>
    /// 文本风格序列化存储结构
    /// </summary>
    public class TextStyleBase: ScriptableObject
    {
        [SerializeField]
        protected Font m_Font;
        [SerializeField]
        protected FontStyle m_FontStyle;
        [Range(0, 300)]
        [SerializeField]
        protected int m_FontSize;
        // [SerializeField] 
        // private float m_LetterSpacing;

        public Font font { get { return m_Font; } }
        public FontStyle fontStyle { get { return m_FontStyle; } }
        public int fontSize { get { return m_FontSize; } }
    }
}