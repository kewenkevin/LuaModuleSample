using ND.UI.Core.Model;

namespace ND.UI
{
    public class EventKeyFrame : KeyFrameBase
    {
        private string _eventStr;

        public EventKeyFrame(Transition parent, KeyFrameConfig config) : base(parent, config)
        {
        }

        public override void Init(KeyFrameConfig config)
        {
            _frameTime = (float)Owner.StoredInts[config.dataList[1]] / 30f;
            _eventStr = Owner.StoredStrings[config.dataList[2]];
            _active = true;
        }

        public override void ApplyValue()
        {
            if (_parent.OnStringEvent != null)
            {
                _parent.OnStringEvent(_eventStr);
            }
        }
    }
}