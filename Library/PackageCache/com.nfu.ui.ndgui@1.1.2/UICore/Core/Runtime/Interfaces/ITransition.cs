namespace ND.UI.Core
{
    public interface ITransition
    {
        void Play(int times = 1, float delay = 0,
            UnityEngine.Events.UnityAction onComplete = null, bool reverse = false);

        void SetPaused(bool paused);

        void Stop(bool setToComplete = true, bool processCallback = false);
    }
}