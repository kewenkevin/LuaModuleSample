

namespace ND.UI
{
    public enum BindingPanelShowTypeState
    {
        Detail,
        Column,
    }

    public enum BindingPanelWorkTypeState
    {
        Edit,
        Display,
    }
    
    public enum BindingPanelIsSearchingState
    {
        False,
        True,
    }

    public class UIExpansionBindingPanelSettings
    {
        private float _panelOffsetX;

        private float _panelOffsetY;

        private BindingPanelWorkTypeState _workType = BindingPanelWorkTypeState.Edit;

        private BindingPanelShowTypeState _showType = BindingPanelShowTypeState.Detail;

        private BindingTreeItemState _showState = BindingTreeItemState.Total;

        private BindingPanelIsSearchingState _isSearching = BindingPanelIsSearchingState.False;

        private int _columnSelectedGOId = 0;

        private string _binderType = "";

        private string _searchStr = "";

        public static string PrefabPath = "Assets/ResourcesAssets/UI/Common/Modules";

        public static string LuaFilePath = "ui/modulePages";

        public float PanelOffsetX { get => _panelOffsetX; set => _panelOffsetX = value; }
        public float PanelOffsetY { get => _panelOffsetY; set => _panelOffsetY = value; }

        public string SearchStr
        {
            get => _searchStr;
            set => _searchStr = value;
        }

        public BindingPanelShowTypeState ShowType
        {
            get => _showType;
            set
            {
                _showType = value;
                if (_showType == BindingPanelShowTypeState.Detail)
                {
                    UIExpansionManager.Instance.CurBindingWrapper.RebuildShowTreeList();
                }
                else
                {
                    _columnSelectedGOId = 0;
                    _binderType = "";
                    UIExpansionManager.Instance.CurBindingWrapper.RebuildColumnGOTreeList();
                }
                UIExpansionManager.Instance.NeedRepaint = true;
            }
        }

        public int ColumnSelectedGOId
        {
            get => _columnSelectedGOId;
            set
            {
                _columnSelectedGOId = value;
                _binderType = "";
                UIExpansionManager.Instance.NeedRepaint = true;
            }
        }

        public string BinderType
        {
            get => _binderType;
            set
            {
                _binderType = value;
                UIExpansionManager.Instance.NeedRepaint = true;
            }
        }

        public BindingTreeItemState ShowState
        {
            get => _showState;
            set
            {
                _showState = value;
                if (_showType == BindingPanelShowTypeState.Detail)
                {
                    UIExpansionManager.Instance.CurBindingWrapper.RebuildShowTreeList();
                }
                else
                {
                    _columnSelectedGOId = 0;
                    _binderType = "";
                    UIExpansionManager.Instance.CurBindingWrapper.RebuildColumnGOTreeList();
                }
                UIExpansionManager.Instance.NeedRepaint = true;
            }
        }

        public BindingPanelWorkTypeState WorkType
        {
            get => _workType;
            set
            {
                if (_workType == value)
                {
                    return;
                }
                _workType = value;
                if (_workType == BindingPanelWorkTypeState.Edit)
                {
                    _showState = BindingTreeItemState.Total;
                    ShowType = BindingPanelShowTypeState.Column;
                }
                else if (_workType == BindingPanelWorkTypeState.Display)
                {
                    _showState = BindingTreeItemState.Use;
                    UIExpansionManager.Instance.CurBindingWrapper.ImplicitSetfoldoutValue(true);
                    ShowType = BindingPanelShowTypeState.Detail;
                }
            }
        }

        public BindingPanelIsSearchingState IsSearching
        {
            get => _isSearching;
            set => _isSearching = value;
        }
    }
}