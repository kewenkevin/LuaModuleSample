using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ND.UI
{
    public class UIExpansionBinderTreeItem : BindingBinderTreeItemBase
    {
        public UIExpansionBinderTreeItem() : base(typeof(UIExpansionBinder))
        {

        }

        public override void RebuildTree()
        {
            base.RebuildTree();
            BindingLinkerTreeItem linkerTreeItem = new BindingLinkerTreeItem("Module");
            AddChild(linkerTreeItem);
        }
    }
}