using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ND.UI.Core.TextEditor
{
    internal abstract class CustomStyleSelector<TWin, T> : EditorWindow where TWin : CustomStyleSelector<TWin, T> where T : Object
    {
        private static CustomStyleSelector<TWin, T> m_SelectorWin;

        public static void Hide()
        {
            if (m_SelectorWin == null)
            {
                return;
            }
            m_SelectorWin.Close();
            m_SelectorWin = null;
        }
        
        public static void Show(T currentSelect, string refPath = null, Action<T, string> applyCallback = null)
        {
            if (m_SelectorWin == null)
            {
                m_SelectorWin = CreateInstance<TWin>();
                m_SelectorWin.titleContent = new GUIContent(m_SelectorWin.CustomWin_Title);
            }

            m_SelectorWin.minSize = m_SelectorWin.CustomWin_MinSize;
            m_SelectorWin.maxSize = m_SelectorWin.CustomWin_MaxSize;
            m_SelectorWin.m_CurrentSelectStyle = currentSelect;
            m_SelectorWin.m_RefPath = refPath;
            m_SelectorWin.m_ApplyCallback = applyCallback;
            m_SelectorWin.ShowUtility();
            m_SelectorWin.Focus();
        }

        public static bool IsShowing(string refPath) => m_SelectorWin != null && m_SelectorWin.m_RefPath == refPath;

        public static T Current => m_SelectorWin?.m_CurrentSelectStyle;
        private string m_RefPath;
        private Action<T, string> m_ApplyCallback;
        protected T m_CurrentSelectStyle;
        private List<T> m_StyleList;
        private Vector2 m_ScrollPos;


        protected abstract string CustomWin_Title { get; }
        protected abstract Vector2 CustomWin_MinSize { get; }
        protected abstract Vector2 CustomWin_MaxSize { get; }

        protected abstract int DefaultListItemHeight { get; }
        protected virtual int DefaultListItemSpacing { get; } = 6;

        private void OnEnable()
        {
            m_StyleList = new List<T>() {null};

            var projectAssets = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            foreach (var assetGuid in projectAssets)
            {
                var style = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(assetGuid));
                if (m_StyleList.Contains(style))
                {
                    continue;
                }

                m_StyleList.Add(style);
            }

            m_ScrollPos = Vector2.zero;
        }

        private void OnGUI()
        {
            var viewStyle = new GUIStyle(GUI.skin.scrollView)
            {
                margin = new RectOffset(5, 5, 5, 5)
            };

            var itemRect = new Rect(0, 0, position.width - 28, DefaultListItemHeight);
            
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, viewStyle);
            
            var currMousePos = Event.current.mousePosition;
            
            foreach (var style in m_StyleList)
            {
                itemRect.height = CalcItemHeight(style);
                itemRect = GUILayoutUtility.GetRect(itemRect.width, itemRect.height);
                
                // if (itemRect.width == 1 || itemRect.height == 1)
                // {
                //     continue;
                // }
                
                DrawSelectedUnder(itemRect, style);
                
                if (style == null)
                {
                    DrawClearStyleItem(itemRect);
                }
                else
                {
                    var itemArea = itemRect;
                    itemArea.position += new Vector2(5, 5);
                    itemArea.size -= new Vector2(10, 10);
                    DrawStyleItem(style, itemArea, m_ScrollPos);
                }

                if (DrawTransparentBtn(itemRect))
                {
                    ApplyStyle(style);
                }
                else
                {
                    if(itemRect.Contains(currMousePos)){
                        if (Event.current.button == 1)
                        {
                            ShowMenu(style);
                        }
                    
                        if (Event.current.button == 0 && Event.current.clickCount > 1)
                        {
                            PingAsset(style);
                        }
                    }
                }
                
                
                GUILayout.Space(DefaultListItemSpacing);
            }

            
            EditorGUILayout.EndScrollView();
        }

        private void ShowMenu(T style)
        {
            if (style == null)
            {
                return;
            }

            OnBeforeShowContextMenu(UIHelperContextMenu.AddItem);
            
            UIHelperContextMenu.AddItem("在Project窗口中显示", false, PingAsset, style);
            UIHelperContextMenu.Show();
        }

        protected virtual void OnBeforeShowContextMenu(Action<string, bool, GenericMenu.MenuFunction2, object> addMenuCall)
        {
            
        }

        private void PingAsset(object target)
        {
            // var target = arg0 as GameObject;
            var pingObj = target as Object;
            if ( pingObj != null ) 
            {
                Debug.Log("Now Editor Ping: " + AssetDatabase.GetAssetPath(pingObj));
                EditorGUIUtility.PingObject(pingObj);
            }
            
        }

        
        private void DrawSelectedUnder(Rect rect, T style)
        {
            var selectedStyle = new GUIStyle("box");
            if (m_CurrentSelectStyle == style)
            {
                selectedStyle.normal.background = getTexture2D(EditorGUIUtility.isProSkin ? DARK_SELECT_COLOR : LIGHT_SELECT_COLOR);
            }
            else
            {
                selectedStyle.normal.background = getTexture2D(EditorGUIUtility.isProSkin ? DARK_BG_COLOR : LIGHT_BG_COLOR);
            }

            GUI.Box(rect, GUIContent.none, selectedStyle);
        }

        
        private bool DrawTransparentBtn(Rect rect)
        {
            return GUI.Button(rect, GUIContent.none, resetStyleBackground(new GUIStyle(GUI.skin.button), getTexture2D(TRANSPARENT_COLOR_UP),  getTexture2D(TRANSPARENT_COLOR_DOWN)));
        }

        private GUIStyle resetStyleBackground(GUIStyle style, Texture2D background, Texture2D pressBackground = null)
        {
            if (pressBackground == null)
            {
                pressBackground = background;
            }
            style.normal.background = style.onNormal.background = style.focused.background = style.onFocused.background = style.hover.background = style.onHover.background = background;
            style.active.background = style.onActive.background = pressBackground;
            return style;
        }
        
        private void DrawClearStyleItem(Rect itemRect)
        {
            var isNoneSelection = m_CurrentSelectStyle == null;
            
            var boxArea = itemRect;
            boxArea.position += new Vector2(10, 10);
            boxArea.size -= new Vector2(20, 20);

            GUI.Box(boxArea, GUIContent.none, resetStyleBackground(new GUIStyle(), isNoneSelection ? getTexture2D(CLEARED_BOX_COLOR) : getTexture2D(TO_CLEAR_BOX_COLOR)));
            
            var labelStyle = new GUIStyle();
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.fontSize = 12;
            labelStyle.normal.textColor = isNoneSelection ? GUI.skin.label.normal.textColor : Color.black;
            GUI.Label(itemRect, isNoneSelection?"当前无样式":"清除样式", labelStyle);
        }

        protected Color GetLabelColor(T style)
        {
            if(m_CurrentSelectStyle == style)
            {
                return Color.black;
            }
            else
            {
                return GUI.skin.label.normal.textColor;
            }
        }

        protected virtual int CalcItemHeight(T style)
        {
            return DefaultListItemHeight;
        }
        
        protected abstract void DrawStyleItem(T style, Rect itemRect, Vector2 offset);
        
        protected void LocalInProject(T style)
        {
            Selection.activeObject = style;
            EditorGUIUtility.PingObject(style);
            
            Close();
        }

        protected void ApplyStyle(T style)
        {
            m_CurrentSelectStyle = style;
            m_ApplyCallback?.Invoke(style, m_RefPath);
        }

        private void OnDestroy()
        {
            if (this == m_SelectorWin)
            {
                m_SelectorWin = null;
            }
            m_ApplyCallback = null;
            m_RefPath = null;
        }
        

        private void OnLostFocus()
        {
            Close();
        }


        #region Editor用的一些颜色和颜色贴图缓存
        private static Color DARK_BG_COLOR = new Color(0.3f, 0.3f, 0.3f, 0.6f); 
        private static Color LIGHT_BG_COLOR = new Color(0.1f, 0.1f, 0.1f, 0.6f);
        
        private static Color DARK_SELECT_COLOR = new Color(0.1f, 0.5f, 0.5f, 0.5f); 
        private static Color LIGHT_SELECT_COLOR = new Color(0f, 0.27f, 0.6f, 1f); 
        
        private static Color TRANSPARENT_COLOR_UP = new Color(0f, 0f, 0f, 0.01f); 
        private static Color TRANSPARENT_COLOR_DOWN = new Color(0f, 0f, 0f, 0.1f); 
        
        private static Color TO_CLEAR_BOX_COLOR = new Color(0.75f, 0.75f, 0.75f, 1f); 
        private static Color CLEARED_BOX_COLOR = new Color(0.3f, 0.3f, 0.3f, 1f); 
        
        private static readonly Dictionary<Color, Texture2D> _colorDict = new Dictionary<Color, Texture2D>();
        private Texture2D getTexture2D(Color color)
        {
            if (_colorDict.TryGetValue(color, out var tex2D)) return tex2D;
            
            tex2D = new Texture2D(1, 1, TextureFormat.RGBA32,false);
            tex2D.alphaIsTransparency = true;
            tex2D.SetPixel(0,0,color);
            tex2D.Apply();
            
            _colorDict.Add(color, tex2D);
            
            return tex2D;
        }
        #endregion
    }

    static class UIHelperContextMenu
    {
        private static List<string> mEntries = new List<string>();
        private static GenericMenu mMenu;

        public static void AddSeparator(string path)
        {
            if (mMenu == null)
            {
                mMenu = new GenericMenu();
            }
            mMenu.AddSeparator(path);
        }


        public static void AddItem(string item, bool isChecked, GenericMenu.MenuFunction2 callback, object args)
        {
            
            if (callback != null)
            {
                if (mMenu == null)
                {
                    mMenu = new GenericMenu();
                }
                var count = 0;

                foreach (var str in mEntries)
                {
                    if (str == item)
                    {
                        ++count;
                    }
                }
	            
                mEntries.Add(item);

                if (count > 0)
                {
                    item += " [" + count + "]";
                }
	            
                mMenu.AddItem(new GUIContent(item), isChecked, callback, args);
                
            }
            else
            {
                AddDisabledItem(item);
            }
        }

        public static void Show()
        {
            if (mMenu == null) return;
	        
            mMenu.ShowAsContext();
            mMenu = null;
            mEntries.Clear();
        }

        private static void AddDisabledItem(string item)
        {
            if (mMenu == null)
            {
                mMenu = new GenericMenu();
            }
	        
            mMenu.AddDisabledItem(new GUIContent(item));
        }
    }
}