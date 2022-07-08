using System.Collections.Generic;
using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;

namespace ND.UI
{
    public class EventLineTreeItem : TransitionLineTreeItemBase
    {
        public EventLineTreeItem() : base(LineTypeState.Event)
        {
            _canUseTween = false;
            _valueSize = 1;
            _state = TransitionTreeItemState.Show;
        }

        public override void AddFrame(int frameIndex)
        {
            string eventName = UIExpansionManager.Instance.TransitionSettings.TempAddEventName;
            UIExpansionManager.Instance.TransitionSettings.TempAddEventName = "";
            TimelineKeyFrameItem frameItem = new TimelineKeyFrameItem(this, frameIndex, eventName);
            _frameList.Add(frameItem);
            base.AddFrame(frameIndex);
        }

        public override void ApplyValue(TimelineKeyFrameItem keyFrameItem)
        {
            Debug.Log(keyFrameItem.GetStrValue());
        }

        public override void MonitorValueChange()
        {
            
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
                    frame.StrValue,
                    frame.RightTween != null,
                    frame.RightTween != null ? frame.RightTween.EaseType : EaseType.None);
                KeyFrameConfig keyFrameConfig = new KeyFrameConfig(_lineType, dataList);
                keyFrameCfgList.Add(keyFrameConfig);
            }
            return keyFrameCfgList;
        }
    }
}