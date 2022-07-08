using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ND.UI
{
    public enum TransitionTimelineItemTypeState
    {
        Frame,
        Tween,
    }

    public class TransitionTimelineItemBase 
    {
        protected TransitionTimelineItemTypeState _itemType;

        protected TransitionLineTreeItemBase _parent;

        protected int _operableId = 0;

        protected Rect _showArea = default(Rect);

        protected int _useButtonIndex = 0;

        protected bool _hasDraged = false;

        protected Vector2 _mouseDownPos;

        public static int OperableItemHashNum = 0;

        public TransitionTimelineItemTypeState ItemType { get => _itemType; set => _itemType = value; }
        public TransitionLineTreeItemBase Parent { get => _parent; set => _parent = value; }

        public TransitionTimelineItemBase(TransitionLineTreeItemBase parent, TransitionTimelineItemTypeState itemType)
        {
            _parent = parent;
            _itemType = itemType;
            _operableId = (" Transition Timeline Item" + ++OperableItemHashNum).GetHashCode();
        }

        public void HandleInput()
        {
            int controlID = GUIUtility.GetControlID(_operableId, FocusType.Passive, GetOperateArea());
            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (!GetOperateArea().Contains(Event.current.mousePosition))
                    {
                        return;
                    }
                    GUIUtility.hotControl = controlID;
                    _useButtonIndex = Event.current.button;
                    _hasDraged = false;
                    _mouseDownPos = Event.current.mousePosition;
                    OnMouseDown();
                    UIExpansionManager.Instance.TransitionSettings.CurOperableTimelineItem = this;
                    // DynamicEffectPanelData.Instance.CurDealFrameIndex = GetFrameIndexByMousePos(Event.current.mousePosition.x);
                    // DynamicEffectPanelData.Instance.NeedRepaint = true;
                    return;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        UIExpansionManager.Instance.TransitionSettings.CurOperableTimelineItem = null;
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
                            OnItemDragEnd();
                        }
                    }
                    return;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        // DynamicEffectPanelData.Instance.CurDealFrameIndex = GetFrameIndexByMousePos(Event.current.mousePosition.x);
                        // DynamicEffectPanelData.Instance.NeedRepaint = true;
                        if (!_hasDraged)
                        {
                            _hasDraged = true;
                            OnItemDragStart();
                        }
                        else
                        {
                            OnItemDragUpdate();
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
        }

        public virtual void OnGUI(Rect rowArea)
        {

        }

        public virtual void OnItemDragStart()
        {

        }

        public virtual void OnItemDragUpdate()
        {

        }

        public virtual void OnItemDragEnd()
        {

        }

        protected virtual void OnMouseDown()
        {

        }

        protected virtual Rect GetOperateArea()
        {
            return default(Rect);
        }
    }
}