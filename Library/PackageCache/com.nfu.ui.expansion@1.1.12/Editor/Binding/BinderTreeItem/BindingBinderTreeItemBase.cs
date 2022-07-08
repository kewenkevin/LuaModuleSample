using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace ND.UI
{
    public class BindingBinderTreeItemBase : BindingTreeItemBase
    {
        protected string _binder;

        protected bool _hasDraged = false;
        protected bool _bModule = false;

        public string BinderType { get => _binder; set => _binder = value; }

        public BindingBinderTreeItemBase(Type binderType)
        {
            // Debug.Assert(binderType.IsSubclassOf(typeof(BinderBase)));
            _type = BindingTreeItemType.Binder;
            _binder = binderType.Name.Replace("Binder","");
            RebuildTree();
        }

        public override void OnDetailGUI(int index, Rect itemArea)
        {
            base.OnDetailGUI(index, itemArea);
            if (UIExpansionManager.Instance.BindingSettings.IsSearching == BindingPanelIsSearchingState.True)
            {
                if (IsMatchedSearch)
                {
                    if (_childrenList.Count > 0)
                    {
                        Rect foldoutArea = new Rect(
                            itemArea.x - 20 + DepthValue * 20,
                            itemArea.y + 1,
                            UIExpansionEditorUtility.SINGLELINE_HEIGHT,
                            UIExpansionEditorUtility.SINGLELINE_HEIGHT);
                        FoldoutValue = EditorGUI.Foldout(foldoutArea, _foldoutValue, string.Empty);
                    }
                    Rect tagArea = new Rect(
                        itemArea.x - 6 + DepthValue * 20,
                        itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                        140,
                        UIExpansionEditorUtility.SINGLELINE_HEIGHT);
                    EditorGUI.LabelField(tagArea, _binder.ToString());
                }
            }
            else
            {
                if (_childrenList.Count > 0)
                {
                    Rect foldoutArea = new Rect(
                        itemArea.x - 20 + DepthValue * 20,
                        itemArea.y + 1,
                        UIExpansionEditorUtility.SINGLELINE_HEIGHT,
                        UIExpansionEditorUtility.SINGLELINE_HEIGHT);
                    FoldoutValue = EditorGUI.Foldout(foldoutArea, _foldoutValue, string.Empty);
                }
                Rect tagArea = new Rect(
                    itemArea.x - 6 + DepthValue * 20,
                    itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                    140,
                    UIExpansionEditorUtility.SINGLELINE_HEIGHT);
                EditorGUI.LabelField(tagArea, _binder.ToString());
            }
        }

        public void OnColumnGUI(int index, Rect itemArea, bool bModule = false)
        {
            _bModule = bModule;
            if (UIExpansionManager.Instance.BindingSettings.BinderType == _binder)
            {
                GUI.DrawTexture(itemArea, UIExpansionEditorUtility.TreeItemSelectedBG);
            }
            else
            {
                GUI.DrawTexture(itemArea, index % 2 == 1 ? UIExpansionEditorUtility.TreeItemBG1 : UIExpansionEditorUtility.TreeItemBG2);
            }
            Rect tagArea = new Rect(
                itemArea.x + 14,
                itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                itemArea.width - 14,
                UIExpansionEditorUtility.SINGLELINE_HEIGHT);
            EditorGUI.LabelField(tagArea, _binder.ToString());
            HandleHeaderInput(itemArea);
        }

        public override void HandleHeaderInput(Rect itemArea)
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
            // Debug.Log("OnLeftMouseButtonClick");
            UIExpansionManager.Instance.BindingSettings.BinderType = _binder;
            GUI.FocusControl(null);
        }

        protected virtual void OnRightMouseButtonClick()
        {
            // Debug.Log("OnRightMouseButtonClick");
        }

        public virtual void RebuildTree()
        {
            _childrenList.Clear();
        }

        public void LoadConfig(LinkerConfig config)
        {
            for (int i = 0; i < _childrenList.Count; i++)
            {
                BindingLinkerTreeItem linkerTreeItem = _childrenList[i] as BindingLinkerTreeItem;
                if (linkerTreeItem.LinkerTypeStr == UIExpansionManager.Instance.CurUIExpansion.StoredStrings[config.LinkerTypeIndex])
                {
                    linkerTreeItem.ConfigSetLabel(UIExpansionManager.Instance.CurUIExpansion.StoredStrings[config.LabelIndex]);
                }
            }
        }
    }
}