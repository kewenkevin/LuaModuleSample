using ND.UI.Core;
using UnityEngine;

namespace ND.UI
{
    public class SizeDeltaLineTreeItem : TransitionLineTreeItemBase
    {
        private Vector2 _monitorCheckValue;

        private Vector2 _previewRecordValue;

        private RectTransform LineTarget
        {
            get { return Target.GetComponent<RectTransform>(); }
        }

        public SizeDeltaLineTreeItem( ) : base(LineTypeState.SizeDelta)
        {
            _canUseTween = true;
            _valueSize = 2;
        }

        public override void AddFrame(int frameIndex)
        {
            TimelineKeyFrameItem frameItem = new TimelineKeyFrameItem(this, frameIndex, LineTarget.sizeDelta);
            _frameList.Add(frameItem);
            base.AddFrame(frameIndex);
        }

        public override void ApplyValue(TimelineKeyFrameItem keyFrameItem)
        {
            Vector2 sizeDelta = new Vector2(
                keyFrameItem.Actives[0] ? keyFrameItem.Values[0] : LineTarget.sizeDelta.x,
                keyFrameItem.Actives[1] ? keyFrameItem.Values[1] : LineTarget.sizeDelta.y);
            LineTarget.sizeDelta = sizeDelta;
        }

        public override void ApplyValue(TimelineTweenItem tweenItem)
        {
            Vector2 sizeDelta = new Vector2(
                tweenItem.Actives[0] ? tweenItem.Value[0] : LineTarget.sizeDelta.x,
                tweenItem.Actives[1] ? tweenItem.Value[1] : LineTarget.sizeDelta.y);
            LineTarget.sizeDelta = sizeDelta;
        }

        public override void Apply(int frameIndex)
        {
            for (int i = 0; i < _frameList.Count; i++)
            {
                if (_frameList[i].FrameIndex <= frameIndex && _frameList[i].LeftTween == null && _frameList[i].RightTween == null)
                {
                    Vector2 frameValue = _frameList[i].GetValue(LineTarget.sizeDelta);
                    LineTarget.sizeDelta = frameValue;
                }
            }
            for (int i = 0; i < _tweenList.Count; i++)
            {
                if (_tweenList[i].LeftFrame.FrameIndex <= frameIndex)
                {
                    Vector2 tweenValue = _tweenList[i].GetValue(frameIndex, LineTarget.sizeDelta);
                    LineTarget.sizeDelta = tweenValue;
                }
            }
        }

        public override void OnPreviewStateChange(bool state)
        {
            if (state)
            {
                _previewRecordValue = LineTarget.sizeDelta;
            }
            else
            {
                LineTarget.sizeDelta = _previewRecordValue;
            }
        }

        protected override void AddValueTreeItems()
        {
            base.AddValueTreeItems();
            AddChild(new TransitionValueTweenTreeItem("Tween:", 0));
            AddChild(new TransitionValueFloatTreeItem("X:", 0));
            AddChild(new TransitionValueFloatTreeItem("Y:", 1));
            AddChild(new TransitionValueTweenCurveTreeItem("Curve:", 0));
        }

        public override void RefreshMonitorValue()
        {
            TimelineKeyFrameItem frameItem = GetFrameItem(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
            if (frameItem != null)
            {
                _monitorCheckValue = frameItem.Vector2Value;
            }
            else
            {
                _monitorCheckValue = LineTarget.sizeDelta;
            }
        }

        public override void MonitorValueChange()
        {
            if(LineTarget.sizeDelta == _monitorCheckValue)
            {
                return;
            }
            if (_state == TransitionTreeItemState.Hide)
            {
                _state = TransitionTreeItemState.Show;
                State = TransitionTreeItemState.Show;
                AddFrame(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
                _monitorCheckValue = LineTarget.sizeDelta;
                OnValueChanged();
            }
            else
            {
                TransitionTimelineItemBase timelineItem = GetOperableItem(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
                if (timelineItem != null)
                {
                    if (timelineItem.ItemType == TransitionTimelineItemTypeState.Frame)
                    {
                        (timelineItem as TimelineKeyFrameItem).Vector2Value = LineTarget.sizeDelta;
                        OnValueChanged();
                        _monitorCheckValue = LineTarget.sizeDelta;
                    }
                }
                else
                {
                    AddFrame(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
                    _monitorCheckValue = LineTarget.sizeDelta;
                    OnValueChanged();
                }
            }
        }
    }
}