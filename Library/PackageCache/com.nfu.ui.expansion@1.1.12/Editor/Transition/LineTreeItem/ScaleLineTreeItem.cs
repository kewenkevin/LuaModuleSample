using ND.UI.Core;
using UnityEngine;

namespace ND.UI
{
    public class ScaleLineTreeItem : TransitionLineTreeItemBase
    {
        private Vector3 _monitorCheckValue;

        private Vector3 _previewRecordValue;

        private RectTransform LineTarget
        {
            get { return Target.GetComponent<RectTransform>(); }
        }

        public ScaleLineTreeItem( ) : base(LineTypeState.Scale)
        {
            _canUseTween = true;
            _valueSize = 3;
        }

        public override void AddFrame(int frameIndex)
        {
            TimelineKeyFrameItem frameItem = new TimelineKeyFrameItem(this, frameIndex, LineTarget.localScale);
            _frameList.Add(frameItem);
            base.AddFrame(frameIndex);
        }

        public override void ApplyValue(TimelineKeyFrameItem keyFrameItem)
        {
            Vector3 scale = new Vector3(
                keyFrameItem.Actives[0] ? keyFrameItem.Values[0] : LineTarget.localScale.x,
                keyFrameItem.Actives[1] ? keyFrameItem.Values[1] : LineTarget.localScale.y,
                keyFrameItem.Actives[2] ? keyFrameItem.Values[2] : LineTarget.localScale.z);
            LineTarget.localScale = scale;
        }

        public override void ApplyValue(TimelineTweenItem tweenItem)
        {
            Vector3 scale = new Vector3(
                tweenItem.Actives[0] ? tweenItem.Value[0] : LineTarget.localScale.x,
                tweenItem.Actives[1] ? tweenItem.Value[1] : LineTarget.localScale.y,
                tweenItem.Actives[2] ? tweenItem.Value[2] : LineTarget.localScale.z);
            LineTarget.localScale = scale;
        }

        public override void Apply(int frameIndex)
        {
            for (int i = 0; i < _frameList.Count; i++)
            {
                if (_frameList[i].FrameIndex <= frameIndex && _frameList[i].LeftTween == null && _frameList[i].RightTween == null)
                {
                    Vector3 frameValue = _frameList[i].GetValue(LineTarget.localScale);
                    LineTarget.localScale = frameValue;
                }
            }
            for (int i = 0; i < _tweenList.Count; i++)
            {
                if (_tweenList[i].LeftFrame.FrameIndex <= frameIndex)
                {
                    Vector3 tweenValue = _tweenList[i].GetValue(frameIndex, LineTarget.localScale);
                    LineTarget.localScale = tweenValue;
                }
            }
        }

        public override void OnPreviewStateChange(bool state)
        {
            if (state)
            {
                _previewRecordValue = LineTarget.localScale;
            }
            else
            {
                LineTarget.localScale = _previewRecordValue;
            }
        }

        protected override void AddValueTreeItems()
        {
            base.AddValueTreeItems();
            AddChild(new TransitionValueTweenTreeItem("Tween:", 0));
            AddChild(new TransitionValueFloatTreeItem("X:", 0));
            AddChild(new TransitionValueFloatTreeItem("Y:", 1));
            AddChild(new TransitionValueFloatTreeItem("Z:", 2));
            AddChild(new TransitionValueTweenCurveTreeItem("Curve:", 0));
        }

        public override void RefreshMonitorValue()
        {
            TimelineKeyFrameItem frameItem = GetFrameItem(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
            if (frameItem != null)
            {
                _monitorCheckValue = frameItem.Vector3Value;
            }
            else
            {
                _monitorCheckValue = LineTarget.localScale;
            }
        }

        public override void MonitorValueChange()
        {
            if (LineTarget.localScale == _monitorCheckValue)
            {
                return;
            }
            if (_state == TransitionTreeItemState.Hide)
            {
                _state = TransitionTreeItemState.Show;
                State = TransitionTreeItemState.Show;
                AddFrame(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
                OnValueChanged();
                _monitorCheckValue = LineTarget.localScale;
            }
            else
            {
                TransitionTimelineItemBase timelineItem = GetOperableItem(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
                if (timelineItem != null)
                {
                    if (timelineItem.ItemType == TransitionTimelineItemTypeState.Frame)
                    {
                        (timelineItem as TimelineKeyFrameItem).Vector3Value = LineTarget.localScale;
                        OnValueChanged();
                        _monitorCheckValue = LineTarget.localScale;
                    }
                }
                else
                {
                    AddFrame(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
                    _monitorCheckValue = LineTarget.localScale;
                    OnValueChanged();
                }
            }
        }
    }
}