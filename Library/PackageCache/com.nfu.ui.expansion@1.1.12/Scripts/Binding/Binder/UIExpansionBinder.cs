namespace ND.UI
{
    public class UIExpansionBinder : BinderBase
    {
        public enum AttributeType : int
        {
            Module,
            Controller,
            Transition,
        }

        public UIExpansionBinder(UIExpansion owner, LinkerConfig config) : base(owner, config)
        {
        }

        public override void Init(LinkerConfig config)
        {
          
        }

 
    }
}