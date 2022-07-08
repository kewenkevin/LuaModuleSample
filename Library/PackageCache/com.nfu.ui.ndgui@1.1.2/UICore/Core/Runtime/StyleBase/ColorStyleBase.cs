using UnityEngine;

namespace ND.UI.Core
{
    /// <summary>
    /// 颜色风格序列化存储结构
    /// </summary>
    public class ColorStyleBase : ScriptableObject
    {
        public enum ColorType
        {
            Solid,
            Gradient
        }

        [SerializeField]
        protected ColorType m_ColorType = ColorType.Solid;
        
        [SerializeField]
        protected Color m_Color = Color.white;

        [SerializeField]
        protected Color[] m_Gradients;

        public Color[] Gradients => m_Gradients;

        public Color Color => m_Color; 
        
        public ColorType Type => m_ColorType;
        
        


    }
}