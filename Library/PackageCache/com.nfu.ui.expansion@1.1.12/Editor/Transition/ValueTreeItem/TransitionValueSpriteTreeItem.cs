using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ND.UI
{
    public class TransitionValueSpriteTreeItem : TransitionValueTreeItemBase
    {
        public TransitionValueSpriteTreeItem(string tag, int valueIndex) : base(TransitionValueTypeState.Sprite, tag, valueIndex)
        {
            _height = 60;
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
                // frameItem.Actives[_valueIndex] = EditorGUI.Toggle(toggleArea, frameItem.Actives[_valueIndex]);
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
                    itemArea.y + 5,
                    50,
                    50);
                // frameItem.SpriteValue = EditorGUI.ObjectField(valueArea, frameItem.SpriteValue, typeof(Sprite), false) as Sprite;
                Sprite tempValue = EditorGUI.ObjectField(valueArea, frameItem.SpriteValue, typeof(Sprite), false) as Sprite;
                {
                    if (tempValue != frameItem.SpriteValue)
                    {
                        frameItem.SpriteValue = tempValue;
                        OnValueChanged();
                    }
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