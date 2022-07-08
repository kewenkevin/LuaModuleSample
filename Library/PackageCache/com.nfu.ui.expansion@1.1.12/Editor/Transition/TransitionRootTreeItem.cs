using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ND.UI
{
    public class TransitionRootTreeItem : TransitionGameObjectTreeItem
    {
        public TransitionRootTreeItem(GameObject target) : base(target)
        {

        }

        public override float RebuildShowTreeList(List<TransitionTreeItemBase> showTreeList)
        {
            float treeHeight = 0;
            if (_childrenList.Count > 0)
            {
                for (int i = 0; i < _childrenList.Count; i++)
                {
                    TransitionTreeItemBase transitionTreeItem = _childrenList[i] as TransitionTreeItemBase;
                    if (transitionTreeItem.State == TransitionTreeItemState.Show)
                    {
                        treeHeight += transitionTreeItem.RebuildShowTreeList(showTreeList);
                    }
                }
            }
            return treeHeight;
        }

        public override float RebuildAddLineTreeList(List<TransitionTreeItemBase> addLineTreeList)
        {
            float treeHeight = 0;
            if (_childrenList.Count > 0)
            {
                for (int i = 0; i < _childrenList.Count; i++)
                {
                    TransitionTreeItemBase transitionTreeItem = _childrenList[i] as TransitionTreeItemBase;
                    if (transitionTreeItem.State ==  TransitionTreeItemState.Hide || transitionTreeItem.GetChildrenStateNum(TransitionTreeItemState.Hide) > 0)
                    {
                        treeHeight += transitionTreeItem.RebuildAddLineTreeList(addLineTreeList);
                    }
                }
            }
            return treeHeight;
        }

    
    }
}