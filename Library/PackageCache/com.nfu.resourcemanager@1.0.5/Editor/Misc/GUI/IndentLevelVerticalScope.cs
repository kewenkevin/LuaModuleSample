using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace ND.Managers.ResourceMgr.Editor
{
    internal partial class EditorGUILayoutx
    {
        internal class IndentLevelVerticalScope : IDisposable
        {
            private int originIndentLevel;
            private float originLabelWidth;
            private bool disposed;

            public IndentLevelVerticalScope(GUIStyle style = null, params GUILayoutOption[] options)
                : this(EditorGUI.indentLevel + 1, style, options)
            {

            }

            public IndentLevelVerticalScope(int indentLevel, GUIStyle style = null, GUILayoutOption[] options = null)
            {
                originIndentLevel = EditorGUI.indentLevel;
                originLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUI.indentLevel = indentLevel;


                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel * 16);
                if (style == null)
                    EditorGUILayout.BeginVertical(options);
                else
                    EditorGUILayout.BeginVertical(style, options);

                EditorGUIUtility.labelWidth -= EditorGUI.indentLevel * 16;
                EditorGUI.indentLevel = 0;
            }

            public void Dispose()
            {
                CloseScope();
            }

            protected void CloseScope()
            {
                if (disposed)
                {
                    Debug.LogError("CloseScope");
                    return;
                }
                disposed = true;
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel = originIndentLevel;
                EditorGUIUtility.labelWidth = originLabelWidth;
            }

        }
    }
}