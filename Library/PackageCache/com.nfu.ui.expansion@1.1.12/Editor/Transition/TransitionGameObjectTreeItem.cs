using ND.UI.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace ND.UI
{
    public class TransitionGameObjectTreeItem : TransitionTreeItemBase
    {
        protected GameObject _target;

        public GameObject Target { get => _target; set => _target = value; }

        public TransitionGameObjectTreeItem(GameObject target)
        {
            _type = TransitionTreeItemType.GameObject;
            _target = target;
            _foldoutValue = true;
            _height = EditorGUIUtility.singleLineHeight + 2;
        }

        public override void OnHeaderGUI(int index, Rect itemArea)
        {
            base.OnHeaderGUI(index, itemArea);
            if (_childrenList.Count > 0)
            {
                Rect foldoutArea = new Rect(
                    itemArea.x - 20 + DepthValue * 20,
                    itemArea.y + 1,
                    EditorGUIUtility.singleLineHeight,
                    EditorGUIUtility.singleLineHeight);
                FoldoutValue = EditorGUI.Foldout(foldoutArea, _foldoutValue, string.Empty);
            }
            Rect GameObjectArea = new Rect(
                itemArea.x - 6 + DepthValue * 20,
                itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                140,
                EditorGUIUtility.singleLineHeight);
            GUI.enabled = false;
            EditorGUI.ObjectField(GameObjectArea, _target, typeof(GameObject), true);
            GUI.enabled = true;
        }

        public override void OnAddGearGUI(int index, Rect itemArea)
        {
            base.OnAddGearGUI(index, itemArea);
            if (_childrenList.Count > 0)
            {
                Rect foldoutArea = new Rect(
                    itemArea.x - 20 + DepthValue * 20,
                    itemArea.y + 1,
                    EditorGUIUtility.singleLineHeight,
                    EditorGUIUtility.singleLineHeight);
                AddLineFoldoutValue = EditorGUI.Foldout(foldoutArea, AddLineFoldoutValue, string.Empty);
            }
            Rect GameObjectArea = new Rect(
                itemArea.x - 6 + DepthValue * 20,
                itemArea.y + (AddLineHeight - EditorGUIUtility.singleLineHeight) / 2,
                140,
                EditorGUIUtility.singleLineHeight);
            GUI.enabled = false;
            EditorGUI.ObjectField(GameObjectArea, _target, typeof(GameObject), true);
            GUI.enabled = true;
        }

        public TransitionLineTreeItemBase GetLineTreeItem(LineTypeState lineType)
        {
            foreach (TransitionTreeItemBase child in _childrenList)
            {
                if (child.Type == TransitionTreeItemType.Line && (child as TransitionLineTreeItemBase).LineType == lineType)
                {
                    return child as TransitionLineTreeItemBase;
                }
            }
            return null;
        }

        public void MonitorLineChange()
        {
            foreach (TransitionTreeItemBase treeItem in _childrenList)
            {
                if (treeItem.Type ==  TransitionTreeItemType.Line)
                {
                    (treeItem as TransitionLineTreeItemBase).MonitorValueChange();
                }
            }
        }


        public static bool CheckCanAddLine(GameObject target, LineTypeState lineType)
        {
            switch (lineType)
            {
                // UIExpansion
                /*
                case LineTypeState.Controller:
                    UIExpansion uiExpansion = target.GetComponent<UIExpansion>();
                    return (uiExpansion != null && uiExpansion.ControllerConfigs != null && uiExpansion.ControllerConfigs.Length > 0);
                case LineTypeState.Transition:
                    UIExpansion uiExpansion1 = target.GetComponent<UIExpansion>();
                    return (uiExpansion1 != null && uiExpansion1.TransitionConfigs != null && uiExpansion1.TransitionConfigs.Length > 0);
                // GameObject
                */
                case LineTypeState.Active:
                    return true;
                // RectTransform
                case LineTypeState.Position:
                case LineTypeState.SizeDelta:
                case LineTypeState.Rotation:
                case LineTypeState.Scale:
                    RectTransform rectTransform = target.GetComponent<RectTransform>();
                    return rectTransform != null;
                // Text
                case LineTypeState.TextColor:
                    Text text = target.GetComponent<Text>();
                    return text != null;
                // Image
                case LineTypeState.ImageSprite:
                case LineTypeState.ImageColor:
                    Image image = target.GetComponent<Image>();
                    return image != null;
                default:
                    return false;
            }
        }
    }
}