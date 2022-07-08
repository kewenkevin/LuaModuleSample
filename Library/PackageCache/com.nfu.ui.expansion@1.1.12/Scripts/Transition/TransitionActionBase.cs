

namespace ND.UI
{
    public abstract class TransitionActionBase : ITweenListener
    {
        protected bool _active;

        protected Tweener _tweener;

        public bool Active { get => _active; set => _active = value; }

        public bool HasTweener { get => _tweener != null; }

        public abstract void Stop(bool setToComplete);

        public abstract bool Play();

        public abstract bool PlayReversed();

        public abstract void SetPaused(bool paused);
        public abstract void OnTweenStart();
        public abstract void OnTweenUpdate();
        public abstract void OnTweenComplete();
    }
}