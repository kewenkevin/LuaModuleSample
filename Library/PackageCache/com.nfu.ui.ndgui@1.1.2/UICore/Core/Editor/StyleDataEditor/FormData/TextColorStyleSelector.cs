using System.Collections.Generic;
using ND.UI.Core.StyleDataEditor;
using UnityEditor;
using UnityEngine;

namespace ND.UI.Core.TextEditor
{
    internal class TextColorStyleSelector : CustomStyleSelector<TextColorStyleSelector, Core.ColorStyleBase>
    {
        protected override string CustomWin_Title => "选择所需要的字体颜色样式";
        protected override Vector2 CustomWin_MinSize => new Vector2(380, 400);
        protected override Vector2 CustomWin_MaxSize => new Vector2(380, 4096);

        protected override int DefaultListItemHeight => 50;

        protected override int CalcItemHeight(Core.ColorStyleBase style)
        {
            if (style == null)
                return DefaultListItemHeight;
            else
                return DefaultListItemHeight + 10;
        }
        
        
        
        protected override void DrawStyleItem(Core.ColorStyleBase style, Rect itemRect, Vector2 offset)
        {
            
            var labelRect = itemRect;
            var htmlStrRect = itemRect;
            
            labelRect.height = htmlStrRect.height = 20;
            
            labelRect.width -= 104;
            
            htmlStrRect.x = htmlStrRect.width - 96;
            htmlStrRect.width = 100;
            
            var labelContent = EditorGUIUtility.ObjectContent(style, typeof(Core.ColorStyleBase));
            labelContent.text = $"{ColorStyleBaseEditor.GetAliasName(style)}";
            
            var labelStyle = new GUIStyle(GUI.skin.textField);
            labelStyle.fontSize = 13;
            labelStyle.imagePosition = ImagePosition.ImageLeft;
            
            EditorGUI.LabelField(labelRect, labelContent, labelStyle);
            EditorGUI.SelectableLabel(htmlStrRect, $"#{ColorUtility.ToHtmlStringRGB(style.Color)}", GUI.skin.textField);

            var colorBoxRect = itemRect;
            colorBoxRect.height = 18;
            colorBoxRect.y += 26;
            
            if(style.Type == Core.ColorStyleBase.ColorType.Solid)
            {
                var colorRect = colorBoxRect;
                colorRect.height -= 2;
                GUI.DrawTexture(colorRect, GetColorTex(style.Color));
    
                var alphaRect = colorBoxRect;
                alphaRect.y += colorBoxRect.height - 2;
                alphaRect.height = 2;
                GUI.DrawTexture(alphaRect, GetColorTex(Color.black));
                alphaRect.width *= style.Color.a;
                GUI.DrawTexture(alphaRect, GetColorTex(Color.white));
            }
            else
            {
                GUI.DrawTexture(colorBoxRect, GetColorTex(style.Gradients[0], style.Gradients[1]));
            }
            
        }
        
        
        private static readonly Dictionary<string, Texture2D> mColorTexDict = new Dictionary<string, Texture2D>();
        
        private Texture2D GetColorTex(Color top, Color bottom)
        {
            var key = ColorUtility.ToHtmlStringRGBA(top) + "_" + ColorUtility.ToHtmlStringRGBA(bottom);
            if (!mColorTexDict.TryGetValue(key, out var tex) || tex == null)
            {
                tex = new Texture2D(2, 1);
                tex.SetPixels(new Color[] {top, bottom});
                tex.wrapMode = TextureWrapMode.Clamp;
                tex.Apply();
                if (mColorTexDict.ContainsKey(key))
                {
                    mColorTexDict[key] = tex;
                }
                else
                {
                    mColorTexDict.Add(key, tex);
                }
            }
            return tex;
            
        }
        
        private Texture2D GetColorTex( Color col)
        {
            var key = ColorUtility.ToHtmlStringRGBA(col);
            
            if (!mColorTexDict.TryGetValue(key, out var tex) || tex == null)
            {
                tex = new Texture2D(1, 1);
                tex.SetPixels(new Color[] {col});
                tex.wrapMode = TextureWrapMode.Repeat;
                tex.Apply();
                if (mColorTexDict.ContainsKey(key))
                {
                    mColorTexDict[key] = tex;
                }
                else
                {
                    mColorTexDict.Add(key, tex);
                }
            }
            return tex;
        }

    }
}