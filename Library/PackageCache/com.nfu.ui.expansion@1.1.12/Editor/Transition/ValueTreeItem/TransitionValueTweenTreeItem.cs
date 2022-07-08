using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ND.UI
{

    public class TransitionValueTweenTreeItem : TransitionValueTreeItemBase
    {
        public TransitionValueTweenTreeItem(string tag, int valueIndex) : base(TransitionValueTypeState.Tween, tag, valueIndex)
        {
        }

        public override void OnHeaderGUI(int index, Rect itemArea)
        {
            base.OnHeaderGUI(index, itemArea);
            var operableItem = Line.GetOperableItem(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
            if (operableItem == null)
            {
                float startX = itemArea.x - 6 + DepthValue * 20;
                Rect tagArea = new Rect(
                    startX,
                    itemArea.y + 2,
                    60,
                    20);
                EditorGUI.LabelField(tagArea, "Tween:");
            }
            else
            {
                TimelineKeyFrameItem keyFrameItem = null;
                if (operableItem.ItemType ==  TransitionTimelineItemTypeState.Frame)
                {
                    keyFrameItem = operableItem as TimelineKeyFrameItem;
                }
                else
                {
                    TimelineTweenItem tweenItem = (operableItem as TimelineTweenItem);
                    keyFrameItem = tweenItem.LeftFrame;
                }
                bool toggleCanTouch = keyFrameItem.RightTween != null || keyFrameItem.Parent.CanCreateTween(keyFrameItem);
                Rect toggleArea = new Rect(
                    itemArea.x - 6 + (DepthValue - 1) * 20,
                    itemArea.y + 2,
                    20,
                    20);
                if (toggleCanTouch)
                {
                    bool tempRightTween = EditorGUI.Toggle(toggleArea, keyFrameItem.RightTween != null);
                    if (tempRightTween)
                    {
                        CreateRightTween(keyFrameItem);
                    }
                    else
                    {
                        RemoveRightTween(keyFrameItem);
                    }
                }
                else
                {
                    EditorGUI.Toggle(toggleArea, keyFrameItem.RightTween != null);
                }
                float startX = itemArea.x - 6 + DepthValue * 20;
                Rect tagArea = new Rect(
                    startX,
                    itemArea.y + 2,
                    60,
                    20);
                EditorGUI.LabelField(tagArea, "Tween:");

                if (keyFrameItem.RightTween != null)
                {
                    Rect tweenArea = new Rect(
                        itemArea.xMax - 101,
                        itemArea.y + 1,
                        100,
                        20);
                    if (GUI.Button(tweenArea, keyFrameItem.RightTween.EaseType.ToString(), "DropDown"))
                    {
                        GenericMenu genericMenu = new GenericMenu();

                        foreach (EaseType tween in Enum.GetValues(typeof(EaseType)))
                        {
                            genericMenu.AddItem(new GUIContent(tween.ToString()), keyFrameItem.RightTween.EaseType == tween, OnTweenSelect, (int)tween);
                        }
                        genericMenu.ShowAsContext();
                    }
                    
                    // Rect CurveArea = new Rect(
                    //     itemArea.x - 6 + DepthValue * 20,
                    //     itemArea.y - 20,
                    //     60,
                    //     20);
                    // EditorGUI.LabelField(CurveArea, "Curve:");
                }
            }
        }

        private void CreateRightTween(TimelineKeyFrameItem leftFrame)
        {
            if (leftFrame.Parent.CanCreateTween(leftFrame))
            {
                TimelineKeyFrameItem rightFrame = leftFrame.Parent.GetRightFirstFrameItem(leftFrame.FrameIndex);
                leftFrame.Parent.CreateTween(leftFrame, rightFrame);
                OnValueChanged();
            }
        }

        private void RemoveRightTween(TimelineKeyFrameItem leftFrame)
        {
            if (leftFrame.RightTween != null)
            {
                leftFrame.Parent.RemoveTween(leftFrame.RightTween);
                OnValueChanged();
            }
        }

        private void OnTweenSelect(object indexObj)
        {
            EaseType tweenType = (EaseType)(int)indexObj;
            var operableItem = Line.GetOperableItem(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
            TimelineKeyFrameItem targetFrameItem = null;
            if (operableItem.ItemType ==  TransitionTimelineItemTypeState.Frame)
            {
                targetFrameItem = operableItem as TimelineKeyFrameItem;
            }
            else
            {
                TimelineTweenItem tweenItem = (operableItem as TimelineTweenItem);
                targetFrameItem = tweenItem.LeftFrame;
            }
            if (tweenType == targetFrameItem.RightTween.EaseType)
            {
                return;
            }
            targetFrameItem.RightTween.EaseType = tweenType;
            OnValueChanged();
        }
    }
}