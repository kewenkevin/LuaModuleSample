using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ND.UI
{
    public class BindingGameObjectTreeItem : BindingTreeItemBase
    {
        protected GameObject _target;

        protected bool _columnFoldoutValue;

        protected int _useButtonIndex = 0;

        protected bool _hasDraged = false;

        public GameObject Target { get => _target; set => _target = value; }
        public bool ColumnFoldoutValue
        {
            get => _columnFoldoutValue;
            set
            {
                _columnFoldoutValue = value;
                UIExpansionManager.Instance.CurBindingWrapper.RebuildColumnGOTreeList();
            }
        }

        public BindingGameObjectTreeItem(GameObject target)
        {
            _type = BindingTreeItemType.GameObject;
            _target = target;
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
                            itemArea.x - 20 + (DepthValue) * 20,
                            itemArea.y + 1,
                            UIExpansionEditorUtility.SINGLELINE_HEIGHT,
                            UIExpansionEditorUtility.SINGLELINE_HEIGHT);
                        FoldoutValue = EditorGUI.Foldout(foldoutArea, _foldoutValue, string.Empty);
                    }
                    Rect GameObjectArea = new Rect(
                        itemArea.x - 6 + (DepthValue) * 20,
                        itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                        140,
                        UIExpansionEditorUtility.SINGLELINE_HEIGHT);
                    GUI.enabled = false;
                    EditorGUI.ObjectField(GameObjectArea, _target, typeof(GameObject), true);
                    GUI.enabled = true;
                }
            }
            else
            {
                if (_childrenList.Count > 0)
                {
                    Rect foldoutArea = new Rect(
                        itemArea.x - 20 + (DepthValue) * 20,
                        itemArea.y + 1,
                        UIExpansionEditorUtility.SINGLELINE_HEIGHT,
                        UIExpansionEditorUtility.SINGLELINE_HEIGHT);
                    FoldoutValue = EditorGUI.Foldout(foldoutArea, _foldoutValue, string.Empty);
                }
                Rect GameObjectArea = new Rect(
                    itemArea.x - 6 + (DepthValue) * 20,
                    itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                    140,
                    UIExpansionEditorUtility.SINGLELINE_HEIGHT);
                GUI.enabled = false;
                EditorGUI.ObjectField(GameObjectArea, _target, typeof(GameObject), true);
                GUI.enabled = true;
            }

            
            
        }

        public virtual void OnColumnGUI(int index, Rect itemArea)
        {
            if (Target == null)
            {
                UIExpansionManager.Instance.NeedReInit = true;
                UIExpansionManager.Instance.NeedRepaint = true;
                return;
            }
            if (UIExpansionManager.Instance.BindingSettings.ColumnSelectedGOId == Target.GetInstanceID())
            {
                GUI.DrawTexture(itemArea, UIExpansionEditorUtility.TreeItemSelectedBG);
            }
            else
            {
                GUI.DrawTexture(itemArea, index % 2 == 1 ? UIExpansionEditorUtility.TreeItemBG1 : UIExpansionEditorUtility.TreeItemBG2);
            }

            if (GOChildNum() > 0)
            {
                Rect foldoutArea = new Rect(
                    itemArea.x - 20 + DepthValue * 20,
                    itemArea.y + 1,
                    UIExpansionEditorUtility.SINGLELINE_HEIGHT,
                    UIExpansionEditorUtility.SINGLELINE_HEIGHT);
                ColumnFoldoutValue = EditorGUI.Foldout(foldoutArea, ColumnFoldoutValue, string.Empty);
            }
            Rect GameObjectArea = new Rect(
                    itemArea.x - 6 + (DepthValue) * 20,
                    itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                    itemArea.width - DepthValue * 20 + 6,
                    UIExpansionEditorUtility.SINGLELINE_HEIGHT);

            EditorGUI.LabelField(GameObjectArea, _target.name);
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
            UIExpansionManager.Instance.BindingSettings.ColumnSelectedGOId = Target.GetInstanceID();
            GUI.FocusControl(null);
        }

        protected virtual void OnRightMouseButtonClick()
        {
            // Debug.Log("OnRightMouseButtonClick");
        }

        public int GOChildNum()
        {
            int num = 0;
            foreach (var treeItem in _childrenList)
            {
                if ((treeItem as BindingTreeItemBase).Type == BindingTreeItemType.GameObject)
                {
                    num++;
                }
            }
            return num;
        }

        public BindingBinderTreeItemBase GetTargetBinder(string binderType, string name = null)
        {
            foreach (var treeItem in _childrenList)
            {
                if ((treeItem as BindingTreeItemBase).Type == BindingTreeItemType.Binder && (treeItem as BindingBinderTreeItemBase).BinderType == binderType)
                {
                    switch ((treeItem as BindingBinderTreeItemBase).BinderType)
                    {

                        default:
                            return treeItem as BindingBinderTreeItemBase;
                    }
                }
            }
            return null;
        }

        public List<BindingBinderTreeItemBase> GetBinderList()
        {
            List<BindingBinderTreeItemBase> binderList = new List<BindingBinderTreeItemBase>();
            foreach (var treeItem in _childrenList)
            {
                BindingTreeItemBase bindingTreeItem = treeItem as BindingTreeItemBase;
                if (bindingTreeItem.Type == BindingTreeItemType.Binder)
                {
                    binderList.Add(treeItem as BindingBinderTreeItemBase);
                }
            }
            return binderList;
        }

        public static bool CheckCanAddBindingComponent(GameObject target, string bindingType)
        {
            switch (bindingType)
            {
                case "GameObject":
                    return true;
                case "UIExpansion":
                    // todo...先复制后整理
                    // string assetPath = string.Empty;
                    if (PrefabUtility.GetPrefabInstanceStatus(target) == PrefabInstanceStatus.Connected)
                    {
                        //  UnityEngine.Object parentObject = PrefabUtility.GetCorrespondingObjectFromSource(target);
                        //  assetPath = AssetDatabase.GetAssetPath(parentObject);
                    }
                    else if (PrefabUtility.GetPrefabAssetType(target) == PrefabAssetType.Regular ||
                          PrefabUtility.GetPrefabAssetType(target) == PrefabAssetType.Variant)
                    {
                        //  assetPath = AssetDatabase.GetAssetPath(target);
                    }
                    else
                    {
                        return target.GetComponent<UIExpansion>() != null; 
                    }


                    return target.GetComponent<UIExpansion>() != null;
                default:
                    return target.GetComponent(bindingType) != null;
            }
        }

        public static System.Type GetBindingType(GameObject target, string bindingType)
        {
            switch (bindingType)
            {
                case "GameObject":
                    return typeof(GameObject);
                case "UIExpansion":
                    // todo...先复制后整理
                    // string assetPath = string.Empty;
                    if (PrefabUtility.GetPrefabInstanceStatus(target) == PrefabInstanceStatus.Connected)
                    {
                        //  UnityEngine.Object parentObject = PrefabUtility.GetCorrespondingObjectFromSource(target);
                        //  assetPath = AssetDatabase.GetAssetPath(parentObject);
                    }
                    else if (PrefabUtility.GetPrefabAssetType(target) == PrefabAssetType.Regular ||
                          PrefabUtility.GetPrefabAssetType(target) == PrefabAssetType.Variant)
                    {
                        //  assetPath = AssetDatabase.GetAssetPath(target);
                    }
                    else
                    {
                        return typeof(UIExpansion); 
                    }
                    return typeof(UIExpansion);
                default:
                    return target.GetComponent(bindingType).GetType();
            }
        }

    }
}