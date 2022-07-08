using System.Collections.Generic;

namespace ND.UI
{
    public class UIExpansionGameObjectTreeItem
    {
        private UIExpansion m_selfUIExpansion;
        private UIExpansionGameObjectTreeItem m_parent;
        private List<UIExpansionGameObjectTreeItem> m_childrenList;
        
        public UIExpansionGameObjectTreeItem(UIExpansion expansion, UIExpansionGameObjectTreeItem parent)
        {
            m_selfUIExpansion = expansion;
            m_parent = parent;
            m_childrenList = new List<UIExpansionGameObjectTreeItem>();
        }

        public UIExpansion UIExpansion
        {
            get => m_selfUIExpansion;
            set => m_selfUIExpansion = value;
        }
        
        public UIExpansionGameObjectTreeItem Parent
        {
            get => m_parent;
            set => m_parent = value;
        }

        public List<UIExpansionGameObjectTreeItem> ChildrenList
        {
            get => m_childrenList;
        }

        public UIExpansionGameObjectTreeItem AddChildToTree(UIExpansionGameObjectTreeItem child)
        {
            if (child == null)
            {
                return null;
            }

            // if (m_childrenList == null)
            // {
            //     m_childrenList = new List<UIExpansionGameObjectTreeItem>();
            // }

            // UIExpansionGameObjectTreeItem child = new UIExpansionGameObjectTreeItem(childUIExpansion, this);
            m_childrenList.Add(child);
            return child;
        }
    }
}