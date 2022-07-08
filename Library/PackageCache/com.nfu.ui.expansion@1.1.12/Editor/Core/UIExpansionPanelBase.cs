using UnityEngine;


namespace ND.UI
{
    public abstract class UIExpansionPanelBase
    {
        protected UIExpansionWindow _window;

        protected string _panelName = "UnKnown";

        public string PanelName
        {
            get
            {
                return _panelName;
            }
        }

        public UIExpansionPanelBase(UIExpansionWindow window)
        {
            _window = window;
        }

        public abstract void OnGUI(Rect panelArea);

        public abstract void OnEnter();

        public abstract void OnExit();

        public abstract void OnCurDealUIChanged();

        public abstract void OnUpdate(float deltaTime);
    }
}