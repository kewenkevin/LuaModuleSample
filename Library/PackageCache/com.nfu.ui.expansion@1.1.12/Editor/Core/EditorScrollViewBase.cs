using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ND.UI
{
    public class EditorScrollViewBase
    {
        protected Rect _viewArea;

        public virtual void OnGUI(Rect viewArea)
        {
            _viewArea = viewArea;
        }
    }
}