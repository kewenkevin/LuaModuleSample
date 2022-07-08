using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ND.UI
{
    public class BindingDetailScrollView : EditorScrollViewBase
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
            float height = UIExpansionManager.Instance.BindingSettings.PanelOffsetY;
            float itemWidth = Mathf.Max(_viewArea.width, 0); // todo...
            if (UIExpansionManager.Instance.CurBindingWrapper.ShowTreeList.Count == 0)
            {
                return;
            }
            for (int i = 0; i < UIExpansionManager.Instance.CurBindingWrapper.ShowTreeList.Count; i++)
            {
                float itemHeight = UIExpansionManager.Instance.CurBindingWrapper.ShowTreeList[i].Height;
                Rect itemArea = new Rect(
                    UIExpansionManager.Instance.ControllerSettings.PanelOffsetX,
                    height,
                    itemWidth,
                    itemHeight);
                
                if (UIExpansionManager.Instance.BindingSettings.IsSearching == BindingPanelIsSearchingState.True)
                {
                    if ( UIExpansionManager.Instance.CurBindingWrapper.ShowTreeList[i].IsMatchedSearch)
                    {
                        height += itemHeight;
                    }
                }
                else
                {
                    height += itemHeight;
                }
                UIExpansionManager.Instance.CurBindingWrapper.ShowTreeList[i].OnDetailGUI(i, itemArea);
            }
        }
    }
}