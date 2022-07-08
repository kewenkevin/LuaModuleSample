using System.Collections.Generic;
using ND.UI.Core.StyleDataEditor;
using UnityEditor;
using UnityEngine;

namespace ND.UI.Core.TextEditor
{
    internal class TextStyleSelector : CustomStyleSelector<TextStyleSelector, Core.TextStyleBase>
    {
        protected override string CustomWin_Title => "选择所需要的文本字体样式";
        protected override Vector2 CustomWin_MinSize => new Vector2(380, 400);
        protected override Vector2 CustomWin_MaxSize => new Vector2(380, 4096);

        protected override int DefaultListItemHeight => 70;
        private readonly Dictionary<Core.TextStyleBase, int> m_HeightDict = new Dictionary<Core.TextStyleBase, int>();
        
        protected override int CalcItemHeight(Core.TextStyleBase style)
        {
            if (style != null && m_HeightDict.TryGetValue(style, out var height))
            {
                return height;
            }
            return DefaultListItemHeight;
        }

        
        protected override void DrawStyleItem(Core.TextStyleBase style, Rect itemRect, Vector2 offset)
        {
            var labelRect = itemRect;
            labelRect.height = 20;
            
            var labelContent = EditorGUIUtility.ObjectContent(style, typeof(Core.TextStyleBase));
            labelContent.text = $"{TextStyleBaseEditor.GetAliasName(style)}";
            
            var labelStyle = new GUIStyle(GUI.skin.textField);
            labelStyle.fontSize = 13;
            labelStyle.imagePosition = ImagePosition.ImageLeft;
            
            EditorGUI.LabelField(labelRect, labelContent, labelStyle);
            
            var previewRect = itemRect;
            previewRect.y += 24;
            previewRect.height -= 24;
            
            var previewStyle = new GUIStyle(GUI.skin.label);
            
            previewStyle.fontSize = style.fontSize;
            previewStyle.fontStyle = style.fontStyle;
            previewStyle.font = style.font;
            
            previewStyle.hover.textColor = previewStyle.normal.textColor = GetLabelColor(style);
            
            previewStyle.fixedWidth  = previewRect.width;
            previewStyle.fixedHeight = previewRect.height;
            
            EditorGUI.LabelField(previewRect, "字体样式FontStyle", previewStyle);

            m_HeightDict[style] = Mathf.CeilToInt(style.fontSize * 2f) + 24; //24 是顶部名字区域的高度
        }
    }
}