using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ND.UI
{
    public class BindingRootTreeItem : BindingGameObjectTreeItem
    {
        public static HashSet<string> IgnoreBinderType = new HashSet<string>
        {
            "UIExpansion",
        };

        public BindingRootTreeItem(GameObject target) : base(target)
        {
            _type = BindingTreeItemType.Root;
            // _height = 0;
            _columnFoldoutValue = true;
            _foldoutValue = true;
        }

        public override void OnDetailGUI(int index, Rect itemArea)
        {
            GUI.DrawTexture(itemArea, index % 2 == 1 ? UIExpansionEditorUtility.TreeItemBG1 : UIExpansionEditorUtility.TreeItemBG2);
            Rect GameObjectArea = new Rect(
                itemArea.x + (DepthValue) * 20,
                itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                140,
                UIExpansionEditorUtility.SINGLELINE_HEIGHT);
            GUI.enabled = false;
            EditorGUI.ObjectField(GameObjectArea, _target, typeof(GameObject), true);
            GUI.enabled = true;
        }

        public override void OnColumnGUI(int index, Rect itemArea)
        {
            if (UIExpansionManager.Instance.BindingSettings.ColumnSelectedGOId == Target.GetInstanceID())
            {
                GUI.DrawTexture(itemArea, UIExpansionEditorUtility.TreeItemSelectedBG);
            }
            else
            {
                GUI.DrawTexture(itemArea, index % 2 == 1 ? UIExpansionEditorUtility.TreeItemBG1 : UIExpansionEditorUtility.TreeItemBG2);
            }
            Rect GameObjectArea = new Rect(
                    itemArea.x + DepthValue * 20,
                    itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                    itemArea.width - DepthValue * 20,
                    UIExpansionEditorUtility.SINGLELINE_HEIGHT);

            EditorGUI.LabelField(GameObjectArea, _target.name);
            HandleHeaderInput(itemArea);
        }

        public override float RebuildShowTreeList(List<BindingTreeItemBase> showTreeList, BindingTreeItemState state)
        {
            /*
            float treeHeight = this._height;
            showTreeList.Add(this);
            */
            float treeHeight = 0;
            if (state == BindingTreeItemState.Use)
            {
                if (State == BindingTreeItemState.Use || GetChildrenStateNum(BindingTreeItemState.Use) > 0)
                {
                    treeHeight += this._height;
                    showTreeList.Add(this);
                }
            }
            else if (state == BindingTreeItemState.Total)
            {
                treeHeight += this._height;
                showTreeList.Add(this);
            }

            if (_childrenList.Count > 0)
            {
                for (int i = 0; i < _childrenList.Count; i++)
                {
                    BindingTreeItemBase bindingTreeItem = _childrenList[i] as BindingTreeItemBase;
                    switch (state)
                    {
                        case BindingTreeItemState.Use:
                            if (bindingTreeItem.State == BindingTreeItemState.Use || bindingTreeItem.GetChildrenStateNum(BindingTreeItemState.Use) > 0)
                            {
                                treeHeight += bindingTreeItem.RebuildShowTreeList(showTreeList, state);
                            }
                            break;
                        case BindingTreeItemState.UnUse:
                            if (bindingTreeItem.State == BindingTreeItemState.UnUse || bindingTreeItem.GetChildrenStateNum(BindingTreeItemState.UnUse) > 0)
                            {
                                treeHeight += bindingTreeItem.RebuildShowTreeList(showTreeList, state);
                            }
                            break;
                        default:
                            treeHeight += bindingTreeItem.RebuildShowTreeList(showTreeList, state);
                            break;
                    }
                }
            }
            return treeHeight;
        }

        public override float RebuildColumnGOTreeList(List<BindingGameObjectTreeItem> columnGOTreeList, BindingTreeItemState state)
        {
            float treeHeight = this.Height;
            columnGOTreeList.Add(this as BindingGameObjectTreeItem);
            if (_childrenList.Count > 0 )
            {
                for (int i = 0; i < _childrenList.Count; i++)
                {
                    BindingTreeItemBase bindingTreeItem = _childrenList[i] as BindingTreeItemBase;
                    switch (state)
                    {
                        case BindingTreeItemState.Use:
                            if (bindingTreeItem.State == BindingTreeItemState.Use || bindingTreeItem.GetChildrenStateNum(BindingTreeItemState.Use) > 0)
                            {
                                treeHeight += bindingTreeItem.RebuildColumnGOTreeList(columnGOTreeList, state);
                            }
                            break;
                        case BindingTreeItemState.UnUse:
                            if (bindingTreeItem.State == BindingTreeItemState.UnUse || bindingTreeItem.GetChildrenStateNum(BindingTreeItemState.UnUse) > 0)
                            {
                                treeHeight += bindingTreeItem.RebuildColumnGOTreeList(columnGOTreeList, state);
                            }
                            break;
                        default:
                            treeHeight += bindingTreeItem.RebuildColumnGOTreeList(columnGOTreeList, state);
                            break;
                    }
                }
            }
            return treeHeight;
        }

        public override void ImplicitSetfoldoutValue(bool foldoutValue)
        {
            if (_childrenList.Count > 0)
            {
                foreach (var child in _childrenList)
                {
                    (child as BindingTreeItemBase).ImplicitSetfoldoutValue(foldoutValue);
                }
            }
        }


    }
}