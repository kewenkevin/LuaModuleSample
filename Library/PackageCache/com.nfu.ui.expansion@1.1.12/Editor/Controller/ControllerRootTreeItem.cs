using System;
using System.Collections.Generic;
using ND.UI.Core;
using UnityEngine;

namespace ND.UI
{
    public class ControllerRootTreeItem :ControllerGameObjectTreeItem
    {
        public static HashSet<GearTypeState> IgnoreGears = new HashSet<GearTypeState>
        {
            // UIExpansion
            GearTypeState.Controller,
            GearTypeState.Transition,

            // GameObject
            // GearTypeState.Active,

            // RectTransform
            // GearTypeState.Position,
            // GearTypeState.SizeDelta,
            // GearTypeState.Rotation,
            // GearTypeState.Scale,
        };

        public ControllerRootTreeItem(GameObject target) : base(target)
        {
            _type = ControllerTreeItemType.Root;
            _height = 0;
            _foldoutValue = true;
        }

        public void AddGearItems()
        {
            foreach (GearTypeState gearType in Enum.GetValues(typeof(GearTypeState)))
            {
                if (ControllerGameObjectTreeItem.CheckCanAddGear(_target, gearType) && !IgnoreGears.Contains(gearType))
                {
                    string gearClassName = "ND.UI." + gearType.ToString() + "GearTreeItem";
                    Type gearClassType = System.Type.GetType(gearClassName);
                    ControllerGearTreeItemBase gearTreeItem = Activator.CreateInstance(gearClassType, true) as ControllerGearTreeItemBase;
                    AddChild(gearTreeItem);
                }
            }
        }

        public override float RebuildShowTreeList(List<ControllerTreeItemBase> showTreeList)
        {
            float treeHeight = 0;
            if (_childrenList.Count > 0)
            {
                for (int i = 0; i < _childrenList.Count; i++)
                {
                    ControllerTreeItemBase controllerTreeItem = _childrenList[i] as ControllerTreeItemBase;
                    if (controllerTreeItem.State == ControllerTreeItemState.Show)
                    {
                        treeHeight += controllerTreeItem.RebuildShowTreeList(showTreeList);
                    }
                }
            }
            return treeHeight;
        }

        public override float RebuildAddGearTreeList(List<ControllerTreeItemBase> addGearTreeList)
        {
            float treeHeight = 0;
            _depthValue = 1;
            treeHeight++; 
            addGearTreeList.Add(this);
            if (_childrenList.Count > 0)
            {
                for (int i = 0; i < _childrenList.Count; i++)
                {
                    ControllerTreeItemBase controllerTreeItem = _childrenList[i] as ControllerTreeItemBase;
                    if (controllerTreeItem.State == ControllerTreeItemState.Hide || controllerTreeItem.GetChildrenStateNum(ControllerTreeItemState.Hide) > 0)
                    {
                        treeHeight += controllerTreeItem.RebuildAddGearTreeList(addGearTreeList); 
                    }
                }
            }
            return treeHeight;
        }
    }
}