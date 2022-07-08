using System.Collections.Generic;
using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;
using UnityEditor;

namespace ND.UI
{
    public enum ControllerTreeItemType
    {
        Root,
        GameObject,
        Gear,
    }

    public enum ControllerTreeItemState
    {
        Hide,
        Show,
    }

    public class ControllerTreeItemBase : EditorTreeItemBase
    {
        protected static int OperateItemAtomNum = 0;

        protected static ControllerTreeItemBase CurOperableItem;

        protected ControllerTreeItemType _type;

        protected ControllerTreeItemState _state = ControllerTreeItemState.Hide;

        protected bool _addGearFoldoutValue = false;

        protected float _addGearHeight = UIExpansionEditorUtility.SINGLELINE_HEIGHT + 2;

        protected int _operableId = 0;

        protected int _useButtonIndex = 0;

        protected bool _hasDraged = false;

        protected Vector2 _mouseDownPos;

        public ControllerTreeItemState State
        {
            get { return _state; }

            set
            {
                if (_parent != null && value == ControllerTreeItemState.Show)
                {
                    (_parent as ControllerTreeItemBase).State = ControllerTreeItemState.Show;
                }

                if (value == _state)
                {
                    return;
                }

                _state = value;
                OnSelfStateChange(value);
            }
        }

        public ControllerTreeItemType Type
        {
            get { return _type; }
        }

        public virtual GearPriority Priority { get; set; } = GearPriority.Normal;


        public override int showPriority => (int)Priority;

        public bool AddGearFoldoutValue
        {
            get
            {
                return _addGearFoldoutValue;
            }

            set
            {
                if (_addGearFoldoutValue == value)
                {
                    return;
                }
                _addGearFoldoutValue = value;
                UIExpansionManager.Instance.CurControllerWrapper.RebuildAddGearTreeList();
            }
        }

        protected override void OnFoldoutValueChange()
        {
            UIExpansionManager.Instance.CurControllerWrapper.RebuildShowTreeList();
        }

        public float AddGearHeight
        {
            get
            {
                return _addGearHeight;
            }

            set
            {
                _addGearHeight = value;
            }
        }

        public ControllerTreeItemBase()
        {
            _operableId = ("UIEXPANSION CONTROLLER TREE ITEM ID: " + ++OperateItemAtomNum).GetHashCode();
        }

        public virtual void OnHeaderGUI(int index, Rect itemArea)
        {
            GUI.DrawTexture(itemArea, index % 2 == 1 ? UIExpansionEditorUtility.TreeItemBG1 : UIExpansionEditorUtility.TreeItemBG2);
            HandleHeaderInput(itemArea);
        }
        
        public virtual void OnHeaderPriorityGUI(int index, Rect itemArea)
        {
            
        }

        public void HandleHeaderInput(Rect itemArea)
        {
            int controlID = GUIUtility.GetControlID(_operableId, FocusType.Passive, itemArea);
            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (!itemArea.Contains(Event.current.mousePosition))
                    {
                        return;
                    }
                    GUIUtility.hotControl = controlID;
                    _useButtonIndex = Event.current.button;
                    _hasDraged = false;
                    _mouseDownPos = Event.current.mousePosition;
                    // OnMouseDown();
                    CurOperableItem = this;
                    // DynamicEffectPanelData.Instance.CurDealFrameIndex = GetFrameIndexByMousePos(Event.current.mousePosition.x);
                    // DynamicEffectPanelData.Instance.NeedRepaint = true;
                    return;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        CurOperableItem = null;
                        if (!_hasDraged)
                        {
                            switch (_useButtonIndex)
                            {
                                case 0:
                                    OnLeftMouseButtonClick();
                                    break;
                                case 1:
                                    OnRightMouseButtonClick();
                                    break;
                            }
                        }
                        else
                        {
                           // OnItemDragEnd();
                        }
                    }
                    return;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        // DynamicEffectPanelData.Instance.CurDealFrameIndex = GetFrameIndexByMousePos(Event.current.mousePosition.x);
                        // DynamicEffectPanelData.Instance.NeedRepaint = true;
                        if (Vector2.Distance(_mouseDownPos, Event.current.mousePosition) > 4)
                        {
                            _hasDraged = true;
                        }
                        Event.current.Use();
                    }
                    return;
            }
        }

        protected virtual void OnLeftMouseButtonClick()
        {
            Debug.Log("OnLeftMouseButtonClick");
        }

        protected virtual void OnRightMouseButtonClick()
        {
            Debug.Log("OnRightMouseButtonClick");
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Delete This"), false, HideThis);
            genericMenu.ShowAsContext();
        }

        private void HideThis()
        {
            this._state = ControllerTreeItemState.Hide;
            for(int i = 0;i < _childrenList.Count; i++)
            {
                (_childrenList[i] as ControllerTreeItemBase).State = ControllerTreeItemState.Hide;
            }
            UIExpansionManager.Instance.CurControllerWrapper.RebuildShowTreeList();
        }

        public virtual void OnDetailGUI(int index, Rect itemArea)
        {
            GUI.DrawTexture(itemArea, index % 2 == 1 ? UIExpansionEditorUtility.TreeItemBG1 : UIExpansionEditorUtility.TreeItemBG2);
        }

        public virtual void OnAddGearGUI(int index, Rect itemArea)
        {
            GUI.DrawTexture(itemArea, index % 2 == 1 ? UIExpansionEditorUtility.TreeItemBG1 : UIExpansionEditorUtility.TreeItemBG2);
        }

        public virtual float RebuildShowTreeList(List<ControllerTreeItemBase> showTreeList)
        {
            float treeHeight = this._height;
           

            //使用插入排序
            bool inserted = false;
            if (ChildrenList.Count <= 0)
            {
                for (int i = 0; i < showTreeList.Count; i++)
                {
                    if (showTreeList[i].Parent == this.Parent
                        && showTreeList[i].ChildrenList.Count <= 0
                        && showTreeList[i].showPriority < this.showPriority)
                    {
                        showTreeList.Insert(i, this);
                        inserted = true;
                        break;
                    }
                }
            }
            

            if(!inserted)
            showTreeList.Add(this);
            
            
            if (_parent != null)
            {
                _depthValue = _parent.DepthValue + 1;
            }
            if ((_childrenList.Count > 0 && _foldoutValue))
            {
                for (int i = 0; i < _childrenList.Count; i++)
                {
                    ControllerTreeItemBase controllerTreeitem = _childrenList[i] as ControllerTreeItemBase;
                    if (controllerTreeitem.State == ControllerTreeItemState.Show)
                    {
                        treeHeight += controllerTreeitem.RebuildShowTreeList(showTreeList);
                    }
                }
            }
            return treeHeight;
        }

        public virtual float RebuildAddGearTreeList(List<ControllerTreeItemBase> addGearTreeList)
        {
            float treeHeight = this._addGearHeight;
            addGearTreeList.Add(this);
            if (_parent != null)
            {
                _depthValue = _parent.DepthValue + 1;
            }
            if ( _childrenList.Count > 0 && _addGearFoldoutValue)
            {
                for (int i = 0; i < _childrenList.Count; i++)
                {
                    ControllerTreeItemBase controllerTreeItem = _childrenList[i] as ControllerTreeItemBase;
                    if(controllerTreeItem.State == ControllerTreeItemState.Hide || controllerTreeItem.GetChildrenStateNum(ControllerTreeItemState.Hide) > 0)
                    {
                        treeHeight += controllerTreeItem.RebuildAddGearTreeList(addGearTreeList);
                    }
                }
            }
            return treeHeight;
        }

        public virtual void RebuildGearConfigList(List<GearConfig> gearConfigList, UIExpansionStoredDataBuilder dataBuilder)
        {
            if (ChildrenList.Count > 0)
            {
                for (int i = 0; i < _childrenList.Count; i++)
                {
                    (_childrenList[i] as ControllerTreeItemBase).RebuildGearConfigList(gearConfigList, dataBuilder);
                }
            }
        }

        public int GetChildrenStateNum(ControllerTreeItemState state)
        {
            int stateNum = 0;
            for (int i = 0; i < _childrenList.Count; i++)
            {
                ControllerTreeItemBase treeItem = _childrenList[i] as ControllerTreeItemBase;
                stateNum += treeItem.State == state ? 1 : 0;
                stateNum += treeItem.GetChildrenStateNum(state);
            }
            return stateNum;
        }

        protected virtual void OnSelfStateChange(ControllerTreeItemState state) { }

        public virtual void OnAddNewPage()
        {
            if (_childrenList.Count > 0)
            {
                foreach (var child in _childrenList)
                {
                    (child as ControllerTreeItemBase).OnAddNewPage();
                }
            }
        }

        public virtual void OnRemovePage(int pageIndex)
        {
            if (_childrenList.Count > 0)
            {
                foreach (var child in _childrenList)
                {
                    (child as ControllerTreeItemBase).OnRemovePage(pageIndex);
                }
            }
        }

        public virtual void RefreshMonitorValue()
        {
            if (_childrenList.Count > 0)
            {
                foreach (var child in _childrenList)
                {
                    (child as ControllerTreeItemBase).RefreshMonitorValue();
                }
            }
        }

        public virtual void Apply()
        {
            if (_childrenList.Count > 0)
            {
                foreach (var child in _childrenList)
                {
                    (child as ControllerTreeItemBase).Apply();
                }
            }
        }
    }
}