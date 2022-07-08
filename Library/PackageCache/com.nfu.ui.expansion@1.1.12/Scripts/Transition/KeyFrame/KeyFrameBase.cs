using ND.UI.Core.Model;

namespace ND.UI
{
    public abstract class KeyFrameBase 
    {
        protected Transition _parent;

        protected bool _active;

        protected float _frameTime;

        public Transition Parent
        {
            get => _parent;
        }

        public bool Active
        {
            get => _active;
        }

        protected UIExpansion Owner
        {
            get
            {
                return _parent.Owner;
            }
        }

        public float FrameTime { get => _frameTime; set => _frameTime = value; }

        public KeyFrameBase(Transition parent, KeyFrameConfig config)
        {
            _parent = parent;
            Init(config);
        }

        public virtual void ApplyValue()
        {

        }

        public abstract void Init(KeyFrameConfig config);
    }
}