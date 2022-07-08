using ND.UI.Core;


namespace ND.UI
{
    public class ActiveLineTreeItem : TransitionLineTreeItemBase
    {
        private bool _monitorCheckValue;

        private bool _previewRecordValue;

        

        public ActiveLineTreeItem() : base(LineTypeState.Active)
        {
            _canUseTween = false;
            _valueSize = 1;
        }

        public override void AddFrame(int frameIndex)
        {
            TimelineKeyFrameItem frameItem = new TimelineKeyFrameItem(this, frameIndex, Target.activeSelf);
            _frameList.Add(frameItem);
            base.AddFrame(frameIndex);
        }

        public override void ApplyValue(TimelineKeyFrameItem keyFrameItem)
        {
            Target.SetActive(keyFrameItem.GetValue(Target.activeSelf));
        }

        public override void ApplyValue(TimelineTweenItem tweenItem)
        {
            
        }

        public override void Apply(int frameIndex)
        {
            for (int i = 0; i < _frameList.Count; i++)
            {
                if (_frameList[i].FrameIndex <= frameIndex && _frameList[i].LeftTween == null && _frameList[i].RightTween == null)
                {
                    bool frameValue = _frameList[i].GetValue(Target.activeSelf);
                    Target.SetActive(frameValue);
                }
            }
        }

        public override void OnPreviewStateChange(bool state)
        {
            if (state)
            {
                _previewRecordValue = Target.activeSelf;
            }
            else
            {
                Target.SetActive(_previewRecordValue);
            }
        }

        public override void RefreshMonitorValue()
        {
            TimelineKeyFrameItem frameItem = GetFrameItem(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
            if (frameItem != null)
            {
                _monitorCheckValue = frameItem.Values[0] == UIExpansionUtility.TrueValue;
            }
            else
            {
                _monitorCheckValue = Target.activeSelf;
            }
         
        }

        public override void MonitorValueChange()
        {
            if (Target.activeSelf == _monitorCheckValue)
            {
                return;
            }
            if (_state == TransitionTreeItemState.Hide)
            {
                _state = TransitionTreeItemState.Show;
                State = TransitionTreeItemState.Show;
                AddFrame(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
                OnValueChanged();
                _monitorCheckValue = Target.activeSelf;
            }
            else
            {
                TransitionTimelineItemBase timelineItem = GetOperableItem(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
                if (timelineItem != null)
                {
                    if (timelineItem.ItemType == TransitionTimelineItemTypeState.Frame)
                    {
                        (timelineItem as TimelineKeyFrameItem).Values[0] = Target.activeSelf ? UIExpansionUtility.TrueValue : UIExpansionUtility.FalseValue;
                        OnValueChanged();
                        _monitorCheckValue = Target.activeSelf;
                    }
                }
                else
                {
                    AddFrame(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
                    OnValueChanged();
                    _monitorCheckValue = Target.activeSelf;
                }
            }
        }


        protected override void AddValueTreeItems()
        {
            base.AddValueTreeItems();
            AddChild(new TransitionValueBoolTreeItem("Value:", 0));
        }
    }
}