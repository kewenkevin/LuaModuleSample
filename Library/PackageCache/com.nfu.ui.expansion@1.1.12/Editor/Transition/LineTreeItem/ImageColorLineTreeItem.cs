using ND.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ND.UI
{
    public class ImageColorLineTreeItem : TransitionLineTreeItemBase
    {
        private Color _monitorCheckValue;

        private Color _previewRecordValue;

        private Image LineTarget
        {
            get { return Target.GetComponent<Image>(); }
        }

        public ImageColorLineTreeItem( ) : base(LineTypeState.ImageColor)
        {
            _canUseTween = true;
            _valueSize = 4;
        }

        public override void AddFrame(int frameIndex)
        {
            TimelineKeyFrameItem frameItem = new TimelineKeyFrameItem(this, frameIndex, LineTarget.color);
            _frameList.Add(frameItem);
            base.AddFrame(frameIndex);
        }

        public override void ApplyValue(TimelineKeyFrameItem keyFrameItem)
        {
            LineTarget.color = keyFrameItem.GetValue(LineTarget.color);
        }

        public override void ApplyValue(TimelineTweenItem tweenItem)
        {
            Color col = new Color(
               tweenItem.Actives[0] ? tweenItem.Value[0] : LineTarget.color.r,
               tweenItem.Actives[1] ? tweenItem.Value[1] : LineTarget.color.g,
               tweenItem.Actives[2] ? tweenItem.Value[2] : LineTarget.color.b,
               tweenItem.Actives[3] ? tweenItem.Value[3] : LineTarget.color.a);
            LineTarget.color = col;
        }

        public override void Apply(int frameIndex)
        {
            for (int i = 0; i < _frameList.Count; i++)
            {
                if (_frameList[i].FrameIndex <= frameIndex && _frameList[i].LeftTween == null && _frameList[i].RightTween == null)
                {
                    Color frameValue = _frameList[i].GetValue(LineTarget.color);
                    LineTarget.color = frameValue;
                }
            }
            for (int i = 0; i < _tweenList.Count; i++)
            {
                if (_tweenList[i].LeftFrame.FrameIndex <= frameIndex)
                {
                    Color tweenValue = _tweenList[i].GetValue(frameIndex, LineTarget.color);
                    LineTarget.color = tweenValue;
                }
            }
        }

        public override void OnPreviewStateChange(bool state)
        {
            if (state)
            {
                _previewRecordValue = LineTarget.color;
            }
            else
            {
                LineTarget.color = _previewRecordValue;
            }
        }

        protected override void AddValueTreeItems()
        {
            base.AddValueTreeItems();
            AddChild(new TransitionValueTweenTreeItem("Tween:", 0));
            AddChild(new TransitionValueColorTreeItem("Value:", 0));
            AddChild(new TransitionValueTweenCurveTreeItem("Curve:", 0));
        }

        public override void RefreshMonitorValue()
        {
            TimelineKeyFrameItem frameItem = GetFrameItem(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
            if (frameItem != null)
            {
                _monitorCheckValue = frameItem.ColorValue;
            }
            else
            {
                _monitorCheckValue = LineTarget.color;
            }
        }

        public override void MonitorValueChange()
        {
            if (LineTarget.color == _monitorCheckValue)
            {
                return;
            }
            if (_state == TransitionTreeItemState.Hide)
            {
                _state = TransitionTreeItemState.Show;
                State = TransitionTreeItemState.Show;
                AddFrame(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
                OnValueChanged();
                _monitorCheckValue = LineTarget.color;
            }
            else
            {
                TransitionTimelineItemBase timelineItem = GetOperableItem(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
                if (timelineItem != null)
                {
                    if (timelineItem.ItemType == TransitionTimelineItemTypeState.Frame)
                    {
                        (timelineItem as TimelineKeyFrameItem).ColorValue = LineTarget.color;
                        OnValueChanged();
                        _monitorCheckValue = LineTarget.color;
                    }
                }
                else
                {
                    AddFrame(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
                    OnValueChanged();
                    _monitorCheckValue = LineTarget.color;
                }
            }
        }
    }
}