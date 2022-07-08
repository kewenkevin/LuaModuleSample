using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace ND.Managers.ResourceMgr.Editor
{
    internal partial class EditorGUILayoutx
    {
        public class AlternateScope : GUI.Scope
        {
            private Color color1;
            private Color color2;
            private int index;

            public AlternateScope()
                : this(Color.clear, new Color(1, 1, 1, 0.03f))
            {

            }
            public AlternateScope(Color color1, Color color2)
            {
                this.color1 = color1;
                this.color2 = color2;
                this.index = 0;
            }

            public IDisposable Horizontal(params GUILayoutOption[] options)
            {
                Color color;
                if (index % 2 == 0)
                    color = color1;
                else
                    color = color2;
                index++;
                return new AlternateHorizontalScope(color, options);
            }
            public IDisposable Horizontal(Color color, params GUILayoutOption[] options)
            {
                index++;
                return new AlternateHorizontalScope(color, options);
            }
            public IDisposable Vertical(params GUILayoutOption[] options)
            {
                Color color;
                if (index % 2 == 0)
                    color = color1;
                else
                    color = color2;
                index++;
                return new AlternateVerticalScope(color, options);
            }
            public IDisposable Vertical(Color color, params GUILayoutOption[] options)
            {
                index++;
                return new AlternateVerticalScope(color, options);
            }

            protected override void CloseScope()
            {
            }

            class AlternateHorizontalScope : GUI.Scope
            {
                private Color originColor;
                static GUIStyle backgroundStyle;

                public AlternateHorizontalScope(Color color, params GUILayoutOption[] options)
                {
                    if (backgroundStyle == null)
                    {
                        backgroundStyle = new GUIStyle();
                        backgroundStyle.normal.background = EditorGUIUtility.whiteTexture;
                        backgroundStyle.padding = new RectOffset(0, 0, 0, 0);
                    }
                    originColor = GUI.backgroundColor;
                    GUI.backgroundColor *= color;
                    GUILayout.BeginHorizontal(backgroundStyle, options);
                }

                protected override void CloseScope()
                {
                    GUILayout.EndHorizontal();
                    GUI.backgroundColor = originColor;
                }
            }

            class AlternateVerticalScope : GUI.Scope
            {
                private Color originColor;
                static GUIStyle backgroundStyle;

                public AlternateVerticalScope(Color color, params GUILayoutOption[] options)
                {
                    if (backgroundStyle == null)
                    {
                        backgroundStyle = new GUIStyle();
                        backgroundStyle.normal.background = EditorGUIUtility.whiteTexture;
                        backgroundStyle.padding = new RectOffset(0, 0, 0, 0);
                    }
                    originColor = GUI.backgroundColor;
                    GUI.backgroundColor *= color;
                    GUILayout.BeginVertical(backgroundStyle, options);
                }

                protected override void CloseScope()
                {
                    GUILayout.EndVertical();
                    GUI.backgroundColor = originColor;
                }
            }

        }
    }


}