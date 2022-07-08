using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ND.Managers.ResourceMgr.Editor
{
    internal static partial class EditorGUILayoutx
    {
        public class Contents
        {
            public static GUIContent Positive = new GUIContent("✚");
            public static GUIContent LeftArrow = new GUIContent("←");
            public static GUIContent RightArrow = new GUIContent("→");
            public static GUIContent Close = new GUIContent("✕");
            public static GUIContent Refresh = new GUIContent("↻");

            static GUIContent toolbarPlus;
            public static GUIContent ToolbarPlus
            {
                get
                {
                    if (toolbarPlus == null)
                        toolbarPlus = EditorGUIUtility.IconContent("Toolbar Plus");
                    return toolbarPlus;
                }
            }

            static GUIContent rotateTool;
          public  static GUIContent RotateTool
            {
                get
                {
                    if (rotateTool == null)
                        rotateTool = EditorGUIUtility.IconContent("RotateTool");
                    return rotateTool;
                }
            }


        }


        public class Styles
        {

            static GUIStyle placeholder;
            public static GUIStyle Placeholder
            {
                get
                {
                    if (placeholder == null)
                    {
                        placeholder = new GUIStyle("label");
                        placeholder.normal.textColor = Color.grey;
                        placeholder.fontSize -= 1;
                        placeholder.padding.left++;
                        placeholder.padding.top++;
                    }
                    return placeholder;
                }
            }

            static GUIStyle whiteTexture;
            static GUIStyle BackgroundColor
            {
                get
                {
                    if (whiteTexture == null)
                    {
                        whiteTexture = new GUIStyle();
                        whiteTexture.normal.background = EditorGUIUtility.whiteTexture;
                    }
                    return whiteTexture;
                }
            }



        }

        static FieldInfo getLastControlIdField;
        public static int GetLastControlId()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                if (getLastControlIdField == null)
                    getLastControlIdField = typeof(UnityEditor.EditorGUIUtility).GetField("s_LastControlID", BindingFlags.Static | BindingFlags.NonPublic);
                if (getLastControlIdField != null)
                    return (int)getLastControlIdField.GetValue(null);
            }
#endif
            return 0;
        }


    }
}
