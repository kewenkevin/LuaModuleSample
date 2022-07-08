using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ND.Managers.ResourceMgr.Editor
{
    internal partial class EditorGUILayoutx
    {
        internal class FoldoutHeaderGroupScope : IDisposable
        {
            private FoldoutHeaderGroupState state;
            private IndentLevelVerticalScope indent;

            public FoldoutHeaderGroupScope(GUIContent label, bool initExpand = false, GUIStyle style = null, Action<Rect> menuAction = null, GUIStyle menuIcon = null, Action onShow = null, Action onHide = null, Action onGUI = null)
            {
                state = (FoldoutHeaderGroupState)GUIUtility.GetStateObject(typeof(FoldoutHeaderGroupState), GUIUtility.GetControlID(typeof(FoldoutHeaderGroupState).GetHashCode(), FocusType.Passive));
                if (!state.initialized)
                {
                    state.initialized = true;
                    state.value = initExpand;
                    if (state.value)
                    {
                        onShow?.Invoke();
                    }
                }
                var oldValue = state.value;
                using (new GUILayout.HorizontalScope())
                {
                    state.value = EditorGUILayout.BeginFoldoutHeaderGroup(state.value, label, style, menuAction, menuIcon);
                    Visiable = state.value;
                    if (oldValue != state.value)
                    {
                        if (state.value)
                            onShow?.Invoke();
                        else
                            onHide?.Invoke();
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();

                    onGUI?.Invoke();
                }
                indent = new IndentLevelVerticalScope();
            }

            public bool Visiable
            {
                get => state.value;
                set => state.value = value;
            }

            public void Dispose()
            {
                if (indent != null)
                {
                    indent.Dispose();
                    indent = null;
                }
            }

            [Serializable]
            class FoldoutHeaderGroupState
            {
                public bool initialized;
                public bool value;
                public DateTime dateTime;
                public float time;
            }
        }
    }

}