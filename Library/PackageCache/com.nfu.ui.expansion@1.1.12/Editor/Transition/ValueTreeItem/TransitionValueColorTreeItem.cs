using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ND.UI
{
    public class TransitionValueColorTreeItem : TransitionValueTreeItemBase
    {
        public TransitionValueColorTreeItem(string tag, int valueIndex) : base(TransitionValueTypeState.Color, tag, valueIndex)
        {
        }

        public override void OnHeaderGUI(int index, Rect itemArea)
        {
            base.OnHeaderGUI(index, itemArea);

            TimelineKeyFrameItem frameItem = Line.GetFrameItem(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
            if (frameItem != null)
            {
                Rect toggleArea = new Rect(
                    itemArea.x - 6 + (DepthValue - 1) * 20,
                    itemArea.y + 2,
                    EditorGUIUtility.singleLineHeight,
                    EditorGUIUtility.singleLineHeight);
                bool tempActive = EditorGUI.Toggle(toggleArea, frameItem.Actives[_valueIndex]);
                {
                    if (tempActive != frameItem.Actives[_valueIndex])
                    {
                        frameItem.Actives[_valueIndex] = tempActive;
                        OnValueChanged();
                    }
                }

                Rect tagArea = new Rect(
                    itemArea.x - 6 + DepthValue * 20,
                    itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                    140,
                    EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(tagArea, _tag);

                Rect valueArea = new Rect(
                    itemArea.width - 60,
                    itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                    50,
                    EditorGUIUtility.singleLineHeight);
                Color frameColor = new Color(frameItem.Values[0], frameItem.Values[1], frameItem.Values[2], frameItem.Values[3]);
                Color tempColor = EditorGUI.ColorField(valueArea, frameColor);
                if (tempColor != frameColor)
                {
                    frameItem.Values[0] = tempColor.r;
                    frameItem.Values[1] = tempColor.g;
                    frameItem.Values[2] = tempColor.b;
                    frameItem.Values[3] = tempColor.a;
                    OnValueChanged();
                }
            }
            else
            {
                Rect tagArea = new Rect(
                    itemArea.x - 6 + DepthValue * 20,
                    itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                    140,
                    EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(tagArea, _tag);
            }
        }
    }
}