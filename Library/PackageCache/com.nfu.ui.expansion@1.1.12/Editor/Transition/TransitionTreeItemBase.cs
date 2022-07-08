using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using ND.UI.Core.Model;

namespace ND.UI
{
    public enum TransitionTreeItemType
    {
        Root,
        GameObject,
        Line,
        Value,
    }

    public enum TransitionValueTypeState
    {
        Bool,
        Float,
        Tween,
        Sprite,
        Color,
        TweenCurve,
    }

    public enum TransitionTreeItemState
    {
        Hide,
        Show,
    }

    public class TransitionTreeItemBase : EditorTreeItemBase
    {
        protected static TransitionTreeItemBase CurOperableItem;

        protected static int OperateItemAtomNum = 0;

        protected TransitionTreeItemType _type;

        protected TransitionTreeItemState _state = TransitionTreeItemState.Hide;

        protected bool _addLineFoldoutValue = false;

        protected float _addLineHeight = UIExpansionEditorUtility.SINGLELINE_HEIGHT + 2;

        protected int _operableId = 0;

        protected int _useButtonIndex = 0;

        protected bool _hasDraged = false;

        protected Vector2 _mouseDownPos;

        public TransitionTreeItemState State
        {
            get => _state;
            set
            {
                if (_parent != null && value == TransitionTreeItemState.Show)
                {
                    (_parent as TransitionTreeItemBase).State = TransitionTreeItemState.Show;
                }
                
                if (value == _state)
                {
                    return;
                }
                _state = value;
                OnSelfStateChange(value);
            }
        }
        public bool AddLineFoldoutValue
        {
            get => _addLineFoldoutValue;
            set
            {
                if (_addLineFoldoutValue == value)
                {
                    return;
                }
                _addLineFoldoutValue = value;
                UIExpansionManager.Instance.CurTransitionWrapper.RebuildAddGearTreeList();
            }
        }
        public float AddLineHeight { get => _addLineHeight; set => _addLineHeight = value; }
        public TransitionTreeItemType Type { get => _type; set => _type = value; }

        public TransitionTreeItemBase()
        {
            // _state = TransitionTreeItemState.Show;
            _foldoutValue = true;
            _operableId = ("UIEXPANSION CONTROLLER TREE ITEM ID: " + ++OperateItemAtomNum).GetHashCode();
        }

        public virtual void OnHeaderGUI(int index, Rect itemArea)
        {
            GUI.DrawTexture(itemArea, index % 2 == 1 ? UIExpansionEditorUtility.TreeItemBG1 : UIExpansionEditorUtility.TreeItemBG2);
            if (_type == TransitionTreeItemType.GameObject || _type == TransitionTreeItemType.Line)
            {
                HandleHeaderInput(itemArea);
            }
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
            this._state =  TransitionTreeItemState.Hide;
            for (int i = 0; i < _childrenList.Count; i++)
            {
                (_childrenList[i] as TransitionTreeItemBase).State =  TransitionTreeItemState.Hide;
            }
            UIExpansionManager.Instance.CurTransitionWrapper.RebuildShowTreeList();
        }

        public virtual void OnDetailGUI(int index, Rect itemArea)
        {
            GUI.DrawTexture(itemArea, index % 2 == 1 ? UIExpansionEditorUtility.TreeItemBG1 : UIExpansionEditorUtility.TreeItemBG2);
        }

        public virtual void OnAddGearGUI(int index, Rect itemArea)
        {
            GUI.DrawTexture(itemArea, index % 2 == 1 ? UIExpansionEditorUtility.TreeItemBG1 : UIExpansionEditorUtility.TreeItemBG2);
        }

        protected virtual void OnSelfStateChange(TransitionTreeItemState state)
        {
            if (UIExpansionManager.Instance.CurTransitionWrapper != null)
            {
                UIExpansionManager.Instance.CurTransitionWrapper.RebuildShowTreeList();
            }
        }

        public virtual float RebuildShowTreeList(List<TransitionTreeItemBase> showTreeList)
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
                    TransitionTreeItemBase transitionTreeItem = _childrenList[i] as TransitionTreeItemBase;
                    if (transitionTreeItem.State == TransitionTreeItemState.Show)
                    {
                        treeHeight += transitionTreeItem.RebuildShowTreeList(showTreeList);
                    }
                }
            }
            return treeHeight;
        }

        public virtual float RebuildAddLineTreeList(List<TransitionTreeItemBase> addLineTreeList)
        {
            float treeHeight = this._height;
            addLineTreeList.Add(this);
            if (_parent != null)
            {
                _depthValue = _parent.DepthValue + 1;
            }
            if (_childrenList.Count > 0 && _addLineFoldoutValue)
            {
                for (int i = 0; i < _childrenList.Count; i++)
                {
                    TransitionTreeItemBase transitionTreeItem = _childrenList[i] as TransitionTreeItemBase;
                    if (transitionTreeItem.State == TransitionTreeItemState.Hide || transitionTreeItem.GetChildrenStateNum(TransitionTreeItemState.Hide) > 0)
                    {
                        treeHeight += transitionTreeItem.RebuildAddLineTreeList(addLineTreeList);
                    }
                }
            }
            return treeHeight;
        }

        public int GetChildrenStateNum(TransitionTreeItemState state)
        {
            int stateNum = 0;
            for (int i = 0; i < _childrenList.Count; i++)
            {
                TransitionTreeItemBase treeItem = _childrenList[i] as TransitionTreeItemBase;
                stateNum += treeItem.State == state ? 1 : 0;
                stateNum += treeItem.GetChildrenStateNum(state);
            }
            return stateNum;
        }

        protected override void OnFoldoutValueChange()
        {
            UIExpansionManager.Instance.CurTransitionWrapper.RebuildShowTreeList();
        }

        public virtual void GetTypeLineTreeItems(TransitionTreeItemType itemType, List<TransitionTreeItemBase> lineList)
        {
            if (Type == itemType && State == TransitionTreeItemState.Show)
            {
                lineList.Add(this);
            }
            if (_childrenList.Count > 0)
            {
                foreach (var child in _childrenList)
                {
                    (child as TransitionTreeItemBase).GetTypeLineTreeItems(itemType, lineList);
                }

            }
        }

        public virtual void RefreshMonitorValue()
        {
            if (_childrenList.Count > 0)
            {
                foreach (var child in _childrenList)
                {
                    (child as TransitionTreeItemBase).RefreshMonitorValue();
                }
            }
        }

        public virtual void RebuildKeyFrameConfigList(List<KeyFrameConfig> keyFrameConfigList, UIExpansionStoredDataBuilder dataBuilder)
        {
            if (ChildrenList.Count > 0)
            {
                for (int i = 0; i < _childrenList.Count; i++)
                {
                    (_childrenList[i] as TransitionTreeItemBase).RebuildKeyFrameConfigList(keyFrameConfigList, dataBuilder);
                }
            }
        }

        public virtual int Play()
        {
            int num = 0;
            if (ChildrenList.Count > 0)
            {
                for (int i = 0; i < _childrenList.Count; i++)
                {
                    num += (_childrenList[i] as TransitionTreeItemBase).Play();
                }
            }
            return num;
        }

        public virtual void Stop()
        {

            if (ChildrenList.Count > 0)
            {
                for (int i = 0; i < _childrenList.Count; i++)
                {
                    (_childrenList[i] as TransitionTreeItemBase).Stop();
                }
            }
        }

        public virtual void Apply(int frameIndex)
        {
            if (ChildrenList.Count > 0)
            {
                for (int i = 0; i < _childrenList.Count; i++)
                {
                    (_childrenList[i] as TransitionTreeItemBase).Apply(frameIndex);
                }
            }
        }

        public virtual void OnPreviewStateChange(bool state)
        {
            if (ChildrenList.Count > 0)
            {
                for (int i = 0; i < _childrenList.Count; i++)
                {
                    (_childrenList[i] as TransitionTreeItemBase).OnPreviewStateChange(state);
                }
            }
        }

        protected virtual void OnValueChanged()
        {
            if (UIExpansionManager.Instance.WindowSettings.AutoSave)
            {
                string failedStr = null;
                if (UIExpansionManager.Instance.CurUIExpansionWrapper.CheckCanSave(true, out failedStr))
                {
                    List<string> missLabelList = null;
                    if (UIExpansionManager.Instance.CurBindingWrapper.CheckAllOldLabelExist(out missLabelList))
                    {
                        UIExpansionManager.Instance.CurUIExpansionWrapper.Save();
                    }
                    else
                    {
                        Debug.Log(string.Format("<color=#ff0000>{0}</color>", "[TransitionTreeItemBase的检测]"));
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("双向绑定界面有部分Label被删除：");
                        for (int i = 0; i < missLabelList.Count; i++)
                        {
                            sb.Append(missLabelList[i]);
                            sb.Append("  ");
                        }
                        sb.AppendLine();
                        sb.Append("是否继续保存？");
                        if (EditorUtility.DisplayDialog("Label丢失确认", sb.ToString(), "保存", "取消"))
                        {
                            UIExpansionManager.Instance.CurUIExpansionWrapper.Save();
                        }
                    }
                }
                else
                {
                    Debug.Log(string.Format("{0}原因: {1}", "保存失败,已退出自动保存模式", failedStr));
                }
            }
        }
    }
}