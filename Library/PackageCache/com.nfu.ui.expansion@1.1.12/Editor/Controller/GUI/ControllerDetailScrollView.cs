using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ND.UI
{
    public class ControllerDetailScrollView : EditorScrollViewBase
    {
        public override void OnGUI(Rect viewArea)
        {
            base.OnGUI(viewArea);
            GUI.BeginGroup(_viewArea, string.Empty);
            DrawDetailItems();
            GUI.EndGroup();
        }

        private void DrawDetailItems()
        {
            float height = UIExpansionManager.Instance.ControllerSettings.PanelOffsetY;
            float itemWidth = Mathf.Max(_viewArea.width, UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH * UIExpansionManager.Instance.CurControllerWrapper.PageNameList.Count);
            if (UIExpansionManager.Instance.CurControllerWrapper.ShowTreeList.Count == 0)
            {
                return;
            }
            for (int i = 0; i < UIExpansionManager.Instance.CurControllerWrapper.ShowTreeList.Count; i++)
            {
                float itemHeight = UIExpansionManager.Instance.CurControllerWrapper.ShowTreeList[i].Height;
                Rect itemArea = new Rect(
                    UIExpansionManager.Instance.ControllerSettings.PanelOffsetX,
                    height,
                    itemWidth,
                    itemHeight);
                height += itemHeight;
                UIExpansionManager.Instance.CurControllerWrapper.ShowTreeList[i].OnDetailGUI(i, itemArea);
            }
            for (int i = 0; i < UIExpansionManager.Instance.CurControllerWrapper.PageNameList.Count; i++)
            {
                Rect cutlineArea = new Rect(
                    UIExpansionManager.Instance.ControllerSettings.PanelOffsetX + UIExpansionEditorUtility.CONTROLLER_PAGE_WIDTH * (i + 1) - 0.5f,
                    0,
                    1,
                    _viewArea.height);
                GUI.DrawTexture(cutlineArea, UIExpansionEditorUtility.CutlineTex);
            }
        }
    }
}