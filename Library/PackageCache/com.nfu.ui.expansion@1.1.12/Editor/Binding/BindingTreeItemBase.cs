using System.Collections.Generic;
using UnityEngine;

namespace ND.UI
{
    public enum BindingTreeItemType
    {
        Root,
        GameObject,
        Binder,
        Linker,
    }

    public enum BindingTreeItemState
    {
        UnUse,
        Use,
        Total,
    }

    public class BindingTreeItemBase : EditorTreeItemBase
    {
        protected static int OperateItemAtomNum = 0;

        protected static BindingTreeItemBase CurOperableItem;

        protected BindingTreeItemType _type;

        protected BindingTreeItemState _state = BindingTreeItemState.UnUse;

        protected int _operableId = 0;

        protected int _useButtonIndex = 0;

        protected Vector2 _mouseDownPos;

        public BindingTreeItemType Type
        {
            get
            {
                return _type;
            }

            set
            {
                _type = value;
            }
        }

        public BindingTreeItemState State
        {
            get
            {
                if (_type == BindingTreeItemType.Linker)
                {
                    if (string.IsNullOrEmpty((this as BindingLinkerTreeItem).Label))
                    {
                        return BindingTreeItemState.UnUse;
                    }
                    else
                    {
                        return BindingTreeItemState.Use;
                    }
                }
                else if (_type == BindingTreeItemType.Root && this is BindingLinkerTreeItem)
                {
                    if (string.IsNullOrEmpty((this as BindingLinkerTreeItem).Label))
                    {
                        return BindingTreeItemState.UnUse;
                    }
                    else
                    {
                        return BindingTreeItemState.Use;
                    }
                }
                else
                {
                    return BindingTreeItemState.Total;
                }
            }


        }

        public BindingTreeItemBase()
        {
            _operableId = ("UIEXPANSION BINDING TREE ITEM ID: " + ++OperateItemAtomNum).GetHashCode();
        }

        protected override void OnFoldoutValueChange()
        {
            UIExpansionManager.Instance.CurBindingWrapper.RebuildShowTreeList();
        }

        public virtual void OnHeaderGUI(int index,Rect itemArea)
        {
            GUI.DrawTexture(itemArea, index % 2 == 1 ? UIExpansionEditorUtility.TreeItemBG1 : UIExpansionEditorUtility.TreeItemBG2);
            HandleHeaderInput(itemArea);
        }

        public virtual void HandleHeaderInput(Rect itemArea)
        {

        }

        public virtual void OnDetailGUI(int index, Rect itemArea)
        {
            GUI.DrawTexture(itemArea, index % 2 == 1 ? UIExpansionEditorUtility.TreeItemBG1 : UIExpansionEditorUtility.TreeItemBG2);
        }

        public virtual void OnSingleLayerGUI(int index, Rect itemArea)
        {
            GUI.DrawTexture(itemArea, index % 2 == 1 ? UIExpansionEditorUtility.TreeItemBG1 : UIExpansionEditorUtility.TreeItemBG2);
        }

        public virtual float RebuildShowTreeList(List<BindingTreeItemBase> showTreeList, BindingTreeItemState state)
        {
            float treeHeight = this._height;
            showTreeList.Add(this);
            if (_parent != null)
            {
                _depthValue = _parent.DepthValue + 1;
            }
            if ((_childrenList.Count > 0 && _foldoutValue))
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

        public int GetChildrenStateNum(BindingTreeItemState state)
        {
            int stateNum = 0;
            for(int i = 0;i < _childrenList.Count; i++)
            {
                BindingTreeItemBase treeItem = _childrenList[i] as BindingTreeItemBase;
                stateNum += treeItem.State == state ? 1 : 0;
                stateNum += treeItem.GetChildrenStateNum(state);
            }
            return stateNum;
        }

        public virtual void RebuildLinkerConfigList(List<LinkerConfig> linkerConfigList, UIExpansionStoredDataBuilder dataBuilder)
        {
            if (ChildrenList.Count > 0)
            {
                for (int i = 0; i < _childrenList.Count; i++)
                {
                    (_childrenList[i] as BindingTreeItemBase).RebuildLinkerConfigList(linkerConfigList, dataBuilder);
                }
            }
        }

        public virtual void RebuildModuleConfigList(List<LinkerConfig> moduleConfigList, UIExpansionStoredDataBuilder dataBuilder)
        {
            if (ChildrenList.Count > 0)
            {
                for (int i = 0; i < _childrenList.Count; i++)
                {
                    (_childrenList[i] as BindingTreeItemBase).RebuildModuleConfigList(moduleConfigList, dataBuilder);
                }
            }
        }

        public virtual void ImplicitSetfoldoutValue(bool foldoutValue)
        {
            _foldoutValue = foldoutValue;
            if (_childrenList.Count > 0)
            {
                foreach(var child in _childrenList)
                {
                    (child as BindingTreeItemBase).ImplicitSetfoldoutValue(foldoutValue);
                }
            }
        }

        public virtual void CheckAllLabel()
        {
            if (_childrenList.Count > 0)
            {
                foreach (var child in _childrenList)
                {
                    (child as BindingTreeItemBase).CheckAllLabel();
                }
            }
        }

        public virtual float RebuildColumnGOTreeList(List<BindingGameObjectTreeItem> columnGOTreeList, BindingTreeItemState state)
        {
            float treeHeight = 0;
            bool foldoutValue = false;
            if(this.Type == BindingTreeItemType.GameObject)
            {
                treeHeight = this.Height;
                columnGOTreeList.Add(this as BindingGameObjectTreeItem);
                foldoutValue = (this as BindingGameObjectTreeItem).ColumnFoldoutValue;
            }
            if (_parent != null)
            {
                _depthValue = _parent.DepthValue + 1;
            }
            if ((_childrenList.Count > 0 && foldoutValue))
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
    }
}