using ND.UI.Core;
using UnityEngine;

namespace ND.UI
{
    public class RotationLineTreeItem : TransitionLineTreeItemBase
    {
        private Vector3 _monitorCheckValue;

        private Vector3 _previewRecordValue;

        private RectTransform LineTarget
        {
            get { return Target.GetComponent<RectTransform>(); }
        }

        public RotationLineTreeItem( ) : base(LineTypeState.Rotation)
        {
            _canUseTween = true;
            _valueSize = 3;
        }

        public override void AddFrame(int frameIndex)
        {
            TimelineKeyFrameItem frameItem = new TimelineKeyFrameItem(this, frameIndex, LineTarget.localEulerAngles);
            _frameList.Add(frameItem);
            base.AddFrame(frameIndex);
        }

        public override void ApplyValue(TimelineKeyFrameItem keyFrameItem)
        {
            Vector3 eulerAngles = new Vector3(
                keyFrameItem.Actives[0] ? keyFrameItem.Values[0] : LineTarget.localEulerAngles.x,
                keyFrameItem.Actives[1] ? keyFrameItem.Values[1] : LineTarget.localEulerAngles.y,
                keyFrameItem.Actives[2] ? keyFrameItem.Values[2] : LineTarget.localEulerAngles.z);
            LineTarget.localEulerAngles = eulerAngles;
        }

        public override void ApplyValue(TimelineTweenItem tweenItem)
        {
            Vector3 eulerAngles = new Vector3(
                tweenItem.Actives[0] ? tweenItem.Value[0] : LineTarget.localEulerAngles.x,
                tweenItem.Actives[1] ? tweenItem.Value[1] : LineTarget.localEulerAngles.y,
                tweenItem.Actives[2] ? tweenItem.Value[2] : LineTarget.localEulerAngles.z);
            LineTarget.localEulerAngles = eulerAngles;
        }

        public override void Apply(int frameIndex)
        {
            for (int i = 0; i < _frameList.Count; i++)
            {
                if (_frameList[i].FrameIndex <= frameIndex && _frameList[i].LeftTween == null && _frameList[i].RightTween == null)
                {
                    Vector3 frameValue = _frameList[i].GetValue(LineTarget.localEulerAngles);
                    LineTarget.localEulerAngles = frameValue;
                }
            }
            for (int i = 0; i < _tweenList.Count; i++)
            {
                if (_tweenList[i].LeftFrame.FrameIndex <= frameIndex)
                {
                    Vector3 tweenValue = _tweenList[i].GetValue(frameIndex, LineTarget.localEulerAngles);
                    LineTarget.localEulerAngles = tweenValue;
                }
            }
        }

        public override void OnPreviewStateChange(bool state)
        {
            if (state)
            {
                _previewRecordValue = LineTarget.localEulerAngles;
            }
            else
            {
                LineTarget.localEulerAngles = _previewRecordValue;
            }
        }

        protected override void AddValueTreeItems()
        {
            base.AddValueTreeItems();
            AddChild(new TransitionValueTweenTreeItem("Tween:", 0));
            AddChild(new TransitionValueFloatTreeItem("X:", 0));
            AddChild(new TransitionValueFloatTreeItem("Y:", 1));
            AddChild(new TransitionValueFloatTreeItem("Z:", 2));
            AddChild(new TransitionValueTweenCurveTreeItem("Curve", 0));
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
                _monitorCheckValue = LineTarget.localEulerAngles;
            } 
        }

        public override void MonitorValueChange()
        {
            if (LineTarget.localEulerAngles == _monitorCheckValue)
            {
                return;
            }
            if (_state == TransitionTreeItemState.Hide)
            {
                _state = TransitionTreeItemState.Show;
                State = TransitionTreeItemState.Show;
                AddFrame(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
                OnValueChanged();
                _monitorCheckValue = LineTarget.localEulerAngles;
            }
            else
            {
                TransitionTimelineItemBase timelineItem = GetOperableItem(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
                if (timelineItem != null)
                {
                    if (timelineItem.ItemType == TransitionTimelineItemTypeState.Frame)
                    {
                        (timelineItem as TimelineKeyFrameItem).Vector3Value = LineTarget.localEulerAngles;
                        _monitorCheckValue = LineTarget.localEulerAngles;
                        OnValueChanged();
                    }
                }
                else
                {
                    AddFrame(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
                    OnValueChanged();
                    _monitorCheckValue = LineTarget.localEulerAngles;
                }
            }
        }
    }
}