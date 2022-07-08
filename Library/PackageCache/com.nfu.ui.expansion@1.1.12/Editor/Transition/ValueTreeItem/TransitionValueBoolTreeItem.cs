using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ND.UI
{
    public class TransitionValueBoolTreeItem : TransitionValueTreeItemBase
    {
        public TransitionValueBoolTreeItem(string tag, int valueIndex) : base(TransitionValueTypeState.Bool, tag, valueIndex)
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
                    itemArea.y + (_height - EditorGUIUtility.singleLineHeight) / 2,
                    EditorGUIUtility.singleLineHeight,
                    EditorGUIUtility.singleLineHeight);
                // frameItem.Values[_valueIndex] = EditorGUI.Toggle(valueArea, frameItem.Values[_valueIndex] > 0) ? UIExpansionUtility.TrueValue : UIExpansionUtility.FalseValue;
                bool tempValue = EditorGUI.Toggle(valueArea, frameItem.Values[_valueIndex] > 0);
                if (tempValue != frameItem.Values[_valueIndex] > 0)
                {
                    frameItem.Values[_valueIndex] = tempValue ? UIExpansionUtility.TrueValue : UIExpansionUtility.FalseValue;
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
