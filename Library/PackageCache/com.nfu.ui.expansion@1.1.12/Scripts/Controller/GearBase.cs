using ND.UI.Core;
using ND.UI.Core.Model;

namespace ND.UI
{
    public abstract class GearBase
    {
        protected Controller _parent;

        protected bool _active;

        protected UIExpansion Owner
        {
            get { return _parent.Owner; }
        }

        public GearBase(Controller parent, GearConfig config)
        {
            _parent = parent;
            Init(config);
        }

        public virtual void Apply()
        {
            if (!_active)
            {
                return;
            }
        }

        public abstract void Init(GearConfig config);
    }
}