using ND.UI.Core;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace ND.UI
{
    public class ControllerGameObjectTreeItem :ControllerTreeItemBase
    {
        protected GameObject _target;

        public GameObject Target
        {
            get
            {
                return _target;
            }

            set
            {
                _target = value;
            }
        }

        public ControllerGameObjectTreeItem(GameObject target)
        {
            _type = ControllerTreeItemType.GameObject;
            _foldoutValue = true;
            _target = target;
            _height = EditorGUIUtility.singleLineHeight + 2;
        }

        public override void OnHeaderGUI(int index, Rect itemArea)
        {
            base.OnHeaderGUI(index, itemArea);
            if(_childrenList.Count > 0)
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
                AddGearFoldoutValue = EditorGUI.Foldout(foldoutArea, AddGearFoldoutValue, string.Empty);
            }
            Rect GameObjectArea = new Rect(
                itemArea.x - 6 + DepthValue * 20,
                itemArea.y + (_addGearHeight - EditorGUIUtility.singleLineHeight) / 2,
                140,
                EditorGUIUtility.singleLineHeight);
            GUI.enabled = false;
            EditorGUI.ObjectField(GameObjectArea, _target, typeof(GameObject), true);
            GUI.enabled = true;
        }

        public ControllerGearTreeItemBase GetTargetGear(GearTypeState gearType, string name = null)
        {
            foreach (ControllerTreeItemBase treeItem in _childrenList)
            {
                if (treeItem.Type == ControllerTreeItemType.Gear && (treeItem as ControllerGearTreeItemBase).Gear == gearType)
                {
                    switch ((treeItem as ControllerGearTreeItemBase).Gear)
                    {
                        case GearTypeState.Controller:
                            ControllerGearTreeItem controllerGearTreeItem = treeItem as ControllerGearTreeItem;
                            if (controllerGearTreeItem.ControllerName == name)
                            {
                                return controllerGearTreeItem;
                            }
                            break;
                        default:
                            return treeItem as ControllerGearTreeItemBase;
                    }
                }
            }
            return null;
        }

        public void ChangeGearState(ControllerTreeItemState state, GearTypeState gearType, string name = null)
        {
            foreach (ControllerGearTreeItemBase gearTreeItem in _childrenList)
            {
                if (gearTreeItem.Gear == gearType)
                {
                    gearTreeItem.State = state;
                }
            }
        }

        public void MonitorGearChange()
        {
            foreach (ControllerTreeItemBase treeItem in _childrenList)
            {
                if (treeItem.Type == ControllerTreeItemType.Gear)
                {
                    (treeItem as ControllerGearTreeItemBase).MonitorValueChange();
                }
            }
        }

        public static bool CheckCanAddGear(GameObject target, GearTypeState gearType)
        {
            switch (gearType)
            {
                // UIExpansion
                case GearTypeState.Controller:
                    UIExpansion uiExpansion = target.GetComponent<UIExpansion>();
                    return (uiExpansion != null && uiExpansion.ControllerConfigs != null && uiExpansion.ControllerConfigs.Length > 0);
                case GearTypeState.Transition:
                    UIExpansion uiExpansion1 = target.GetComponent<UIExpansion>();
                    return (uiExpansion1 != null && uiExpansion1.TransitionConfigs != null && uiExpansion1.TransitionConfigs.Length > 0);
                
                // GameObject
                case GearTypeState.Active:
                case GearTypeState.OverallAlpha:
                    return true;
                
                // RectTransform
                case GearTypeState.Position:
                case GearTypeState.SizeDelta:
                case GearTypeState.Rotation:
                case GearTypeState.Scale:
                case GearTypeState.PercentPosition:
                    RectTransform rectTransform = target.GetComponent<RectTransform>();
                    return rectTransform != null;
                
                // Text
                case GearTypeState.LocalizationKey:
                case GearTypeState.TextStr:
                case GearTypeState.TextColor:
                    Text text = target.GetComponent<Text>();
                    return text != null;
                case GearTypeState.TextFontStyle:
                case GearTypeState.TextColorStyle:
                    ND.UI.Core.IColorStyleUseAble yText = target.GetComponent<ND.UI.Core.IColorStyleUseAble>();
                    return yText != null;
                
                // Image
                case GearTypeState.ImageSprite:
                case GearTypeState.ImageColor:
                    Image image = target.GetComponent<Image>();
                    return image != null;
                
                //RawImage
                case GearTypeState.RawImageColor:
                    RawImage rawImage = target.GetComponent<RawImage>();
                    return rawImage != null;
                case GearTypeState.ImageMaterial:
                    return target.GetComponent<Image>() != null || target.GetComponent<RawImage>() != null;
                
                // Rating
                case GearTypeState.RatingCurrent:
                    return target.GetComponent<ND.UI.Core.IRating>() != null;
                case GearTypeState.RatingTotal:
                    ND.UI.Core.IRating yRating = target.GetComponent<ND.UI.Core.IRating>();
                    return yRating != null && yRating.canModifyTotalByCode;
                
                default:
                    return false;
            }
        }
    }
}