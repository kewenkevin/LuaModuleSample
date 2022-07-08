using System.Collections.Generic;

namespace ND.UI
{
    public abstract class EditorTreeItemBase
    {
        protected bool _foldoutValue;

        protected int _depthValue;

        protected float _height = UIExpansionEditorUtility.SINGLELINE_HEIGHT + 2;

        protected EditorTreeItemBase _parent;

        protected bool _isMatchedSearch = false;

        protected List<EditorTreeItemBase> _childrenList = new List<EditorTreeItemBase>();
        
        
        public virtual int showPriority => 0;

        public int DepthValue
        {
            get { return _depthValue; }
            set { _depthValue = value; }
        }

        public float Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public EditorTreeItemBase Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public List<EditorTreeItemBase> ChildrenList
        {
            get { return _childrenList; }
            set { _childrenList = value; }
        }

        public bool FoldoutValue
        {
            get { return _foldoutValue; }
            set
            {
                if (_foldoutValue == value)
                {
                    return;
                }
                _foldoutValue = value;
                OnFoldoutValueChange();
            }
        }
        public bool IsMatchedSearch
        {
            get => _isMatchedSearch;
            set => _isMatchedSearch = value;
        }

        protected virtual void OnFoldoutValueChange()
        {

        }

        public void AddChild(EditorTreeItemBase treeItem)
        {
            _childrenList.Add(treeItem);
            treeItem.Parent = this;
        }
    }
}