using UnityEditor;
using UnityEngine;

namespace ND.UI
{
    public class TransitionValueTweenCurveTreeItem : TransitionValueTreeItemBase
    {
        public TransitionValueTweenCurveTreeItem(string tag, int valueIndex) : base(TransitionValueTypeState.TweenCurve, tag, valueIndex)
        {
        }

        AnimationCurve _curveX = AnimationCurve.Linear(0,0,1,1);
        public override void OnHeaderGUI(int index, Rect itemArea)
        {
            base.OnHeaderGUI(index, itemArea);
            
            //获取当前处理的时间轴帧对象
            TimelineKeyFrameItem frameItem =
                Line.GetFrameItem(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
            // EditorGUI.BeginDisabledGroup(frameItem.RightTween.EaseType != EaseType.Custom);
            if (frameItem == null)
            {
                EditorGUI.BeginDisabledGroup(true);
            }
            else if (frameItem.RightTween == null)
            {
                EditorGUI.BeginDisabledGroup(true);
            }
            else
            {
                EditorGUI.BeginDisabledGroup(frameItem.RightTween.EaseType != EaseType.Custom);
            }
            // EditorGUI.BeginDisabledGroup(true);
            if (frameItem!=null)
            {
                // if (frameItem.RightTween != null)
                // {
                //     if (frameItem.RightTween.RightFrame != null)
                //     {
                //         if (frameItem.RightTween.RightFrame.CustomerCurve != null)
                //         {
                //             _curveX = frameItem.RightTween.RightFrame.CustomerCurve;
                //         }
                //     }
                // }
                if (frameItem.CustomerCurve != null)
                {
                    _curveX = frameItem.CustomerCurve;
                }
            }
            
            Rect labelArea = new Rect(
                itemArea.x - 6 + (DepthValue - 1) * 20,
                itemArea.y + 2,
                50,
                EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelArea, _tag);
            if (frameItem != null)
            {
                Rect curveArea = new Rect(
                    itemArea.width - 300,
                    itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                    290,
                    EditorGUIUtility.singleLineHeight);
                
                EditorGUI.CurveField(curveArea, " ", _curveX);

                if (frameItem.RightTween != null)
                {
                    if (frameItem.RightTween.RightFrame != null)
                    {
                        frameItem.RightTween.RightFrame.CustomerCurve = _curveX;
                    }
                }

            }
            EditorGUI.EndDisabledGroup();
            
        }
    }
}