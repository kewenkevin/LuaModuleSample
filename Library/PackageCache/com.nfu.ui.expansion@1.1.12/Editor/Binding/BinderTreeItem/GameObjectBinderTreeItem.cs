using System;
using System.Collections.Generic;

namespace ND.UI
{
    public class GameObjectBinderTreeItem : BindingBinderTreeItemBase
    {
        public static Dictionary<GameObjectBinder.AttributeType, LinkerValueState> AttributeValueDic = new Dictionary<GameObjectBinder.AttributeType, LinkerValueState> {
            {GameObjectBinder.AttributeType.Active,   LinkerValueState.Boolean },
        };

        public GameObjectBinderTreeItem() : base(typeof(GameObjectBinder))
        {
            
        }

        public override void RebuildTree()
        {
            base.RebuildTree();
            foreach (GameObjectBinder.AttributeType binderType in Enum.GetValues(typeof(GameObjectBinder.AttributeType)))
            {
                BindingLinkerTreeItem linkerTreeItem = new BindingLinkerTreeItem(binderType.ToString(), binderType.ToString(), (int)AttributeValueDic[binderType]);
                AddChild(linkerTreeItem);
            }
        }
    }
}