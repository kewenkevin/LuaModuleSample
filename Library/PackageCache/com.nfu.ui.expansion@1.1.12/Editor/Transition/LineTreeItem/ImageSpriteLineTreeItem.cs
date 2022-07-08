using System.Collections.Generic;
using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;
using UnityEngine.UI;


namespace ND.UI
{
    public class ImageSpriteLineTreeItem : TransitionLineTreeItemBase
    {
        private Sprite _monitorCheckValue;

        private Sprite _previewRecordValue;

        private Image LineTarget
        {
            get { return Target.GetComponent<Image>(); }
        }

        public ImageSpriteLineTreeItem() : base(LineTypeState.ImageSprite)
        {
            _canUseTween = false;
        }

        public override void AddFrame(int frameIndex)
        {
            TimelineKeyFrameItem frameItem = new TimelineKeyFrameItem(this, frameIndex, LineTarget.sprite);
            _frameList.Add(frameItem);
            base.AddFrame(frameIndex);
        }

        public override void ApplyValue(TimelineKeyFrameItem keyFrameItem)
        {
            LineTarget.sprite = keyFrameItem.GetSpriteValue();
        }

        public override void Apply(int frameIndex)
        {
            for (int i = 0; i < _frameList.Count; i++)
            {
                if (_frameList[i].FrameIndex <= frameIndex && _frameList[i].LeftTween == null && _frameList[i].RightTween == null)
                {
                    LineTarget.sprite = _frameList[i].GetSpriteValue();
                }
            }
        }

        public override void OnPreviewStateChange(bool state)
        {
            if (state)
            {
                _previewRecordValue = LineTarget.sprite;
            }
            else
            {
                LineTarget.sprite = _previewRecordValue;
            }
        }

        protected override void AddValueTreeItems()
        {
            base.AddValueTreeItems();
            AddChild(new TransitionValueSpriteTreeItem("Value:", 0));
        }

        protected override List<KeyFrameConfig> BuildKeyFrameCfgs(UIExpansionStoredDataBuilder dataBuilder)
        {
            List<KeyFrameConfig> keyFrameCfgList = new List<KeyFrameConfig>();
            for (int i = 0; i < _frameList.Count; i++)
            {
                var frame = _frameList[i];
                List<ushort> dataList = dataBuilder.BuildDataList(
                    Target,
                    frame.FrameIndex,
                    frame.SpriteValue,
                    frame.RightTween != null,
                    frame.RightTween != null ? frame.RightTween.EaseType : EaseType.None);
                KeyFrameConfig keyFrameConfig = new KeyFrameConfig(_lineType, dataList);
                keyFrameCfgList.Add(keyFrameConfig);
            }
            return keyFrameCfgList;
        }

        public override void RefreshMonitorValue()
        {
            TimelineKeyFrameItem frameItem = GetFrameItem(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
            if (frameItem != null)
            {
                _monitorCheckValue = frameItem.SpriteValue;
            }
            else
            {
                _monitorCheckValue = LineTarget.sprite;
            }
        }

        public override void MonitorValueChange()
        {
            if (LineTarget.sprite == _monitorCheckValue)
            {
                return;
            }
            if (_state == TransitionTreeItemState.Hide)
            {
                _state = TransitionTreeItemState.Show;
                State = TransitionTreeItemState.Show;
                AddFrame(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
                _monitorCheckValue = LineTarget.sprite;
                OnValueChanged();
            }
            else
            {
                TransitionTimelineItemBase timelineItem = GetOperableItem(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
                if (timelineItem != null)
                {
                    if (timelineItem.ItemType == TransitionTimelineItemTypeState.Frame)
                    {
                        (timelineItem as TimelineKeyFrameItem).SpriteValue = LineTarget.sprite;
                        OnValueChanged();
                        _monitorCheckValue = LineTarget.sprite;
                    }
                }
                else
                {
                    AddFrame(UIExpansionManager.Instance.TransitionSettings.CurDealFrameIndex);
                    OnValueChanged();
                    _monitorCheckValue = LineTarget.sprite;
                }
            }
        }
    }
}