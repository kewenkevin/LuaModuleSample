using ND.UI.Core;
using ND.UI.Core.Model;

namespace ND.UI
{
    public class TransitionGearTreeItem : ControllerGearTreeItemBase
    {
        public TransitionGearTreeItem() : base(GearTypeState.Transition)
        {
        }

        public override void ApplyValue(int pageIndex)
        {
        
        }

        public override GearConfig BuildConfig(UIExpansionStoredDataBuilder dataBuilder)
        {
            return null;
        }

        public override void LoadConfig(GearConfig config)
        {
            _state = ControllerTreeItemState.Show;
            State = ControllerTreeItemState.Show;
        }

        public override void MonitorValueChange()
        {
        
        }

        public override void OnRemovePage(int pageIndex)
        {
        
        }

        public override void RecordValue(int pageIndex = NEW_PAGE_INDEX)
        {
           
        }
    }
}