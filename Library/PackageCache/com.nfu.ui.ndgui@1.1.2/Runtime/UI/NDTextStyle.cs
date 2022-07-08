using UnityEngine;
using System;
using ND.UI.Core;

namespace ND.UI.NDUI
{

    [CreateAssetMenu(fileName = "NDTextStyle", menuName = "NDTextStyle", order = 1)]
    public class NDTextStyle : TextStyleBase
    {
        public static Action<string, Action<Font>> localizationProvider;

        public void Apply(NDText text) 
        {
            if (text == null)
            {
                return;
            }
            Set(text);
        }

        public void UpdateLocalizationFont()
        {
            if (localizationProvider!=null && m_Font!=null)
            {
                localizationProvider.Invoke(m_Font.name, (o) =>
                {
                    if (o!=null)
                    {
                        m_Font = o;
                    }
                });
            }
        }
        private void Set(NDText text) 
        {
            if (text == null)
            {
                return;
            }
            text.font = font;
            text.fontStyle = fontStyle;
            text.fontSize = fontSize;
        }
        
    }
}
