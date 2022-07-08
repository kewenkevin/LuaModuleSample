using ND.UI.Core;
using ND.UI.NDUI.Effects;
using UnityEngine;

namespace ND.UI.NDUI
{
    [CreateAssetMenu(fileName = "NDTextColorStyle", menuName = "NDTextColorStyle")]
    public class NDTextColorStyle : ColorStyleBase
    {
        public void Apply(NDText text) 
        {
            if (text == null)
            {
                return;
            }
            Set(text);
        }

        public void Set(NDText text)
        {
            if (text == null)
            {
                return;
            }

            switch (m_ColorType)
            {
                case ColorType.Solid:
                    SetSolidColor(text);
                    break;
                case ColorType.Gradient:
                    SetGradientColor(text);
                    break;
            }
            
        }

        private void SetSolidColor(NDText text)
        {
            text.color = m_Color;
            if (m_Gradients!= null && m_Gradients.Length > 0)
            {
                text.color = m_Gradients[0];
            }

            var textGradient = text.GetComponent<TextGradient>();
            if (textGradient != null)
            {
                textGradient.enabled = false;
            }
        }

        private void SetGradientColor(NDText text)
        {
            if (m_Gradients== null || m_Gradients.Length < 2)
            {
                SetSolidColor(text);
                return;
            }
            
            // text.colorStyle = null;
            
            var textGradient = text.GetComponent<TextGradient>();
            if (textGradient != null)
            {
                textGradient.enabled = true;
            }
            else
            {
                textGradient = text.gameObject.AddComponent<TextGradient>();
            }

            if (m_Gradients != null)
            {
                textGradient.TopColor = m_Gradients[0];
                textGradient.BottomColor = m_Gradients[1];
            }

        }

    }
}