using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ND.UI.NDUI
{
[ExecuteInEditMode]
    public class ListView : MonoBehaviour
    {
        #region Enum Group

        public enum ListSelectionMode
        {   
            None = 1,
            
            Single = 2,

            Multiple = 3,
        }
        
        public enum ListLayoutType
        {
            SingleColumn = 1,
            SingleRow = 2,
            FlowHorizontal = 3,
            FlowVertical = 4,
            Pagination = 5
        }
        
        public enum VertAlignType
        {
            Top = 1,
            Middle = 2,
            Bottom = 3
        }
        
        public enum AlignType
        {
            Left = 1,
            Center = 2,
            Right = 3
        }
        
        public enum ChildrenRenderOrder
        {
            Ascent = 1,
            Descent = 2,
            Arch = 3,//暂时不支持
        }

        #endregion

        #region Members

         public class ItemInfo
        {
            public ListViewItem item;

            public string identifyName;

            public bool isSelected;

            public Vector2 size;

            public uint updateFlag;

            // public object widget;
        }

        [SerializeField] public GameObject[] m_ViewCells; //Cell显示对象的引用库

        [SerializeField] public RectTransform contentComponent; //列表content组件

        [SerializeField] public ScrollRect scrollRectComponent; //列表滚动组件
        
        [SerializeField] bool m_isVirtual = true; //是否为虚列表 默认开启

        [SerializeField] ListSelectionMode m_selectType = ListSelectionMode.Single; //选择类型

        [SerializeField]  int m_constraintCount; //固定的行数或列数，若为0则根据窗口大小自动计算行列数

        [SerializeField] Vector2 m_itemSpace; //item间距

        [SerializeField] private int totalCount; //总Cell数量
        
        [SerializeField] private ListLayoutType _layout = ListLayoutType.SingleColumn;
        
        [SerializeField] VertAlignType _verticalAlign = VertAlignType.Top;
        
        [SerializeField] AlignType _align = AlignType.Left;
        
        /*[SerializeField]*/ ChildrenRenderOrder _childrenRenderOrder;

        public GameObject defaultItemPrefab { get; set; } //默认item的GameObject
        
        public bool isVirtual => m_isVirtual; //是否是虚拟列表开关 默认开启
        
        // public delegate string ListItemProvider(int index); //确定渲染Item的实际对象

        // public ListItemProvider ItemProvider; //显示对象回调函数，用来混插显示不同对象
        public Func<int,string> ItemProvider; //显示对象回调函数，用来混插显示不同对象
        
        public Action<int, ListViewItem> onItemRender; //用于刷新item UI
        
        public Action<object> onItemClicked; //item被点击时调用

        public GameObjectPool pool = new GameObjectPool(); //对象池

        bool m_isBoundsDirty;
        int _lastSelectedIndex;
        int _lineCount;
        int _columnCount;
        int _lineGap = 0;
        int _columnGap = 0;
        bool _autoResizeItem = false;
        bool _boundsChanged;
        Vector2 _alignOffset;
        
        //Virtual Support
        bool _loop;
        int _realNumItems;
        int _firstIndex; //the top left index
        int _curLineItemCount; //item count in one line
        int _curLineItemCount2; //只用在页面模式，表示垂直方向的项目数
        Vector2 _itemSize;
        int _virtualListChanged = 2; //1-content changed, 2-size changed
        uint itemInfoVer; //用来标志item是否在本次处理中已经被重用了
        
        RectTransform m_rectTransform;
        ScrollRect m_scrollRect;
        List<ItemInfo> _virtualItems; //用做虚列表逻辑对象
        List<ListViewItem> m_displayItemList; //显示对象列表

        public int layout
        {
            get { return (int)_layout; }
            set
            {
                if (value == 0)
                {
                    return;
                }
                if (value < 0 || value > 5)
                {
                    Debug.LogError($"Layout input value is illegal: {value}, SingleColumn = 1,SingleRow = 2,FlowHorizontal = 3,FlowVertical = 4,Pagination = 5");
                    return;
                }

                if (_layout != (ListLayoutType)value)
                {
                    _layout = (ListLayoutType)value;
                    SetBoundsChangedFlag();
                    if (m_isVirtual)
                        SetVirtualListChangedFlag(true);
                }
            }
        }
        
        public int verticalAlign
        {
            get { return (int)_verticalAlign; }
            set
            {
                if (value == 0)
                {
                    return;
                }
                if (value < 0 || value > 3)
                {
                    Debug.LogError($"Vertical Align input value is illegal: {value}, Top = 1,Middle = 2,Bottom = 3");
                    return;
                }

                if (_verticalAlign != (VertAlignType)value)
                {
                    _verticalAlign = (VertAlignType)value;
                    SetBoundsChangedFlag();
                    if (m_isVirtual)
                        SetVirtualListChangedFlag(true);
                }
            }
        }
        
        public int align
        {
            get { return (int)_align; }
            set
            {
                if (value == 0)
                {
                    return;
                }
                if (value < 0 || value > 3)
                {
                    Debug.LogError($"Align input value is illegal: {value}, Left = 1,Center = 2,Right = 3");
                    return;
                }

                if (_align != (AlignType)value)
                {
                    _align = (AlignType)value;
                    SetBoundsChangedFlag();
                    if (m_isVirtual)
                        SetVirtualListChangedFlag(true);
                }
            }
        }

        public int numChildren
        {
            get
            {
                if (m_displayItemList == null)
                {
                    return 0;
                }

                return m_displayItemList.Count;
            }
        }
        
        public ChildrenRenderOrder childrenRenderOrder
        {
            get { return _childrenRenderOrder; }
            set
            {
                if (_childrenRenderOrder != value)
                {
                    _childrenRenderOrder = value;
                    // BuildNativeDisplayList();
                }
            }
        }

        public Vector2 ItemSpace
        {
            get => m_itemSpace;
            set
            { 
                m_itemSpace = value;
                _lineGap = (int)m_itemSpace.x;
                _columnGap = (int)m_itemSpace.y;
                SetBoundsChangedFlag();
            }
        }

        public int selectType
        {
            get => (int)m_selectType;
            set
            {
                if (value == 0)
                {
                    return;
                }
                if (value < 0 || value > 3)
                {
                    Debug.LogError($"Selection Type input value is illegal: {value}, None = 1,Single = 2,Multiple = 3,");
                    return;
                }

                if (m_selectType != (ListSelectionMode)value)
                {
                    m_selectType = (ListSelectionMode)value;
                    //切换选择模式后，必定重置选择项
                    ClearSelection();
                }
                
            }
        }

        public int itemCount
        {
            get
            {
                if (m_isVirtual)
                    return totalCount;
                else
                    return m_displayItemList.Count;
            }
            set
            {
                if (m_isVirtual)
                {
                    // if (Application.isPlaying && onItemRender == null)
                    //     throw new Exception("Set itemRenderer first!");

                    totalCount = value;
                    if (_loop)
                        _realNumItems = totalCount * 6;//设置6倍数量，用于循环滚动
                    else
                        _realNumItems = totalCount;

                    //_virtualItems的设计是只增不减的
                    int oldCount = _virtualItems.Count;
                    if (_realNumItems > oldCount)
                    {
                        _itemSize = defaultItemPrefab.GetComponent<RectTransform>().rect.size;
                        for (int i = oldCount; i < _realNumItems; i++)
                        {
                            ItemInfo ii = new ItemInfo();
                            if (ItemProvider == null)
                            {
                                ii.size = _itemSize;
                                ii.identifyName = defaultItemPrefab.GetComponent<ListViewItem>().identifyName;
                            }
                            else
                            {
                                string identifyName = ItemProvider.Invoke(i);
                                GameObject item = pool.Get(identifyName);
                                ii.size = item.GetComponent<RectTransform>().rect.size;
                                ii.identifyName = identifyName;
                                pool.Put(identifyName,item);
                            }
                            // ii.size = _itemSize;
                            // ii.identifyName = defaultItemPrefab.GetComponent<ListViewItem>().identifyName;
                            _virtualItems.Add(ii);
                        }
                    }
                    else
                    {
                        for (int i = _realNumItems; i < oldCount; i++)
                        {
                            _virtualItems[i].isSelected = false;
                            if (_virtualItems[i].item!=null)
                            {
                                _virtualItems[i].item.selectType = m_selectType;
                                _virtualItems[i].item.isSelected = false;
                            }
                        }
                            
                    }

                    // if (_virtualListChanged != 0)
                    //     Timers.inst.Remove(this.RefreshVirtualList);
                    //立即刷新
                    this.RefreshVirtualList(null);
                }
                else
                {
                    totalCount = value;
                    _realNumItems = totalCount;
                    int cnt = m_displayItemList.Count;
                    if (value > cnt)
                    {
                        ListViewItem item;
                        for (int i = cnt; i < value; i++)
                        {
                            if (ItemProvider == null)
                                item = AddItemFromPool();
                            else
                                item = AddItemFromPool(ItemProvider.Invoke(i));
                            ItemInfo ii = new ItemInfo();
                            ii.item = item;
                            ii.size = item.size;
                            ii.identifyName = item.identifyName;
                            _virtualItems.Add(ii);
                        }
                    }
                    else
                    {
                        RemoveChildrenToPool(value, cnt);
                        _virtualItems.Clear();
                        for (int i = 0; i < m_displayItemList.Count; i++)
                        {
                            ItemInfo ii = new ItemInfo();
                            ii.item = m_displayItemList[i];
                            ii.size = m_displayItemList[i].size;
                            ii.identifyName = m_displayItemList[i].identifyName;
                            _virtualItems.Add(ii);
                        }
                    }
                    this.RefreshVirtualList(null);
                    // if (onItemRender != null)
                    // {
                    //     for (int i = 0; i < value; i++)
                    //         onItemRender.Invoke(i, GetChildAt(i));
                    // }
                }
            }
        }
        
        public int constraintCount
        {
            get
            {
                return m_constraintCount;
            }
            set
            {
                if (m_constraintCount != value)
                {
                    m_constraintCount = value;
                    SetBoundsChangedFlag();
                    if (m_isVirtual)
                        SetVirtualListChangedFlag(true);
                }
            }
        }

        #endregion
        
        #region Unity Lifetime

        void Awake()
        {
            Init();
        }
        
        void Update()
        {
            if (_boundsChanged)
            {
                StopMovement();
                if (m_isVirtual)
                {
                    RefreshVirtualList(); 
                }

                _boundsChanged = false;
            }
                
            if (m_scrollRect?.velocity != Vector2.zero)
            {
                StopAllCoroutines();
            }
        }

        void OnDestroy()
        {
            StopMovement();
            ClearAll();
        }

        #endregion

        #region Init
        
        public void Init()
        {
            if (m_ViewCells.Length == 0)
            {
                throw new Exception("List Default View Cell must not be empty");
            }
            InitData();
            CreatPool();
        }
        
        void InitData()
        {
            m_rectTransform = contentComponent;
            m_rectTransform.pivot = Vector2.up;
            m_rectTransform.anchorMax = Vector2.up;
            m_rectTransform.anchorMin = Vector2.up;
            m_displayItemList = new List<ListViewItem>();
            _virtualItems = new List<ItemInfo>();
            m_scrollRect = scrollRectComponent;
            
            _lineGap = (int)ItemSpace.x;
            _columnGap = (int)ItemSpace.y;
            
            if (m_scrollRect == null)
            {
                Debug.LogError("ListView can not find ScrollRect");
                return;
            }
            m_scrollRect.onValueChanged.AddListener(OnScroll);
        }

        void CreatPool()
        {
            var prefabList = m_ViewCells.ToList();
            for (int i = 0; i < prefabList.Count; i++)
            {
                var prb = prefabList[i].GetComponent<ListViewItem>();
                pool.CreatePool(m_rectTransform, prefabList[i], prb.identifyName);
            }
            defaultItemPrefab = prefabList[0];
            _itemSize = defaultItemPrefab.GetComponent<RectTransform>().rect.size;
        }

        #endregion

        #region List Algorithm

        void OnScroll(Vector2 position)
        {
            HandleScroll(false);
        }

        /// <summary>
        /// 触发重绘
        /// </summary>
        public void SetBoundsChangedFlag()
        {
            // if (scrollPane == null && !_trackBounds)
            if (scrollRectComponent == null)
                return;

            _boundsChanged = true;
        }
        
        void SetVirtualListChangedFlag(bool layoutChanged)
        {
            if (layoutChanged)
                _virtualListChanged = 2;
            else if (_virtualListChanged == 0)
                _virtualListChanged = 1;

            // Timers.inst.CallLater(RefreshVirtualList);
            RefreshVirtualList(true);
        }

        void RefreshVirtualList(object param = null)
        {
            bool layoutChanged = _virtualListChanged == 2;
            _virtualListChanged = 0;
            // bool layoutChanged = _virtualListChanged == 0;
            // _virtualListChanged = 10;
            // _miscFlags |= 1;
            var viewWidth = m_scrollRect.viewport.rect.width;
            var viewHeight = m_scrollRect.viewport.rect.height;
            if (layoutChanged)
            {
                if (_layout == ListLayoutType.SingleColumn || _layout == ListLayoutType.SingleRow)
                    _curLineItemCount = 1;
                else if (_layout == ListLayoutType.FlowHorizontal)
                {
                    // if (_columnCount > 0)
                    //     _curLineItemCount = _columnCount;
                    if (m_constraintCount > 0)
                        _curLineItemCount = m_constraintCount;
                    else
                    {
                        _curLineItemCount = Mathf.FloorToInt((viewWidth + _columnGap) / (_itemSize.x + _columnGap));
                        if (_curLineItemCount <= 0)
                            _curLineItemCount = 1;
                    }
                }
                else if (_layout == ListLayoutType.FlowVertical)
                {
                    // if (_lineCount > 0)
                    //     _curLineItemCount = _lineCount;
                    if (m_constraintCount > 0)
                        _curLineItemCount = m_constraintCount;
                    else
                    {
                        _curLineItemCount = Mathf.FloorToInt((viewHeight + _lineGap) / (_itemSize.y + _lineGap));
                        if (_curLineItemCount <= 0)
                            _curLineItemCount = 1;
                    }
                }
                else //pagination
                {
                    // if (_columnCount > 0)
                    //     _curLineItemCount = _columnCount;
                    if (m_constraintCount > 0)
                        _curLineItemCount = m_constraintCount;
                    else
                    {
                        _curLineItemCount = Mathf.FloorToInt((viewWidth + _columnGap) / (_itemSize.x + _columnGap));
                        if (_curLineItemCount <= 0)
                            _curLineItemCount = 1;
                    }

                    // if (_lineCount > 0)
                    //     _curLineItemCount2 = _lineCount;
                    if (m_constraintCount > 0)
                        _curLineItemCount2 = m_constraintCount;
                    else
                    {
                        _curLineItemCount2 = Mathf.FloorToInt((viewHeight + _lineGap) / (_itemSize.y + _lineGap));
                        if (_curLineItemCount2 <= 0)
                            _curLineItemCount2 = 1;
                    }
                }
            }

            float ch = 0, cw = 0;
            if (_realNumItems > 0)
            {
                int len = Mathf.CeilToInt((float)_realNumItems / _curLineItemCount) * _curLineItemCount;
                int len2 = Math.Min(_curLineItemCount, _realNumItems);
                if (_layout == ListLayoutType.SingleColumn || _layout == ListLayoutType.FlowHorizontal)
                {
                    for (int i = 0; i < len; i += _curLineItemCount)
                        ch += _virtualItems[i].size.y + _lineGap;
                    if (ch > 0)
                        ch -= _lineGap;

                    if (_autoResizeItem)
                        cw = viewWidth;
                    else
                    {
                        for (int i = 0; i < len2; i++)
                            cw += _virtualItems[i].size.x + _columnGap;
                        if (cw > 0)
                            cw -= _columnGap;
                    }
                }
                else if (_layout == ListLayoutType.SingleRow || _layout == ListLayoutType.FlowVertical)
                {
                    for (int i = 0; i < len; i += _curLineItemCount)
                        cw += _virtualItems[i].size.x + _columnGap;
                    if (cw > 0)
                        cw -= _columnGap;

                    if (_autoResizeItem)
                        ch = viewHeight;
                    else
                    {
                        for (int i = 0; i < len2; i++)
                            ch += _virtualItems[i].size.y + _lineGap;
                        if (ch > 0)
                            ch -= _lineGap;
                    }
                }
                else
                {
                    int pageCount = Mathf.CeilToInt((float)len / (_curLineItemCount * _curLineItemCount2));
                    cw = pageCount * viewWidth;
                    ch = viewHeight;
                }
            }

            HandleAlign(cw, ch);
            SetContentSize(cw, ch);

            // _miscFlags &= 0xFE;

            HandleScroll(true);
        }

        private void SetContentSize(float aWidth, float aHeight)
        {
            if (Mathf.Approximately(contentComponent.sizeDelta.x, aWidth) && Mathf.Approximately(contentComponent.sizeDelta.y, aHeight))
                return;
            // contentComponent.sizeDelta.Set(aWidth, aHeight);
            contentComponent.sizeDelta = new Vector2(aWidth, aHeight);
            //TODO 是否要重新调整content的位置
        }
        
        private void HandleAlign(float contentWidth, float contentHeight)
        {
            Vector2 newOffset = Vector2.zero;
            var viewWidth = m_scrollRect.viewport.rect.width;
            var viewHeight = m_scrollRect.viewport.rect.height;
            if (contentHeight < viewHeight)
            {
                if (_verticalAlign == VertAlignType.Middle)
                {
                    newOffset.y = -(int)((viewHeight - contentHeight) / 2);
                    scrollRectComponent.vertical = false;
                }
                else if (_verticalAlign == VertAlignType.Bottom)
                {
                    newOffset.y = -(viewHeight - contentHeight);
                    scrollRectComponent.vertical = false;
                }
                else
                {
                    scrollRectComponent.vertical = true;
                }
                    
            }

            if (contentWidth < viewWidth)
            {
                if (_align == AlignType.Center)
                {
                    newOffset.x = (int)((viewWidth - contentWidth) / 2);
                    scrollRectComponent.horizontal = false;
                }
                else if (_align == AlignType.Right)
                {
                    newOffset.x = viewWidth - contentWidth;
                    scrollRectComponent.horizontal = false;
                }
                else
                {
                    scrollRectComponent.horizontal = true; 
                }
            }

            if (newOffset != _alignOffset)
            {
                _alignOffset = newOffset;
                contentComponent.localPosition = new Vector3(_alignOffset.x, _alignOffset.y, 0);
            }
        }

        private void HandleScroll(bool forceUpdate)
        {
            // if ((_miscFlags & 1) != 0)
            //     return;
            if (_layout == ListLayoutType.SingleColumn || _layout == ListLayoutType.FlowHorizontal)
            {
                int enterCounter = 0;
                while (HandleScrollColumAndFlowHorizontal(forceUpdate))
                {
                    //可能会因为ITEM资源改变导致ITEM大小发生改变，所有出现最后一页填不满的情况，这时要反复尝试填满。
                    enterCounter++;
                    forceUpdate = false;
                    if (enterCounter > 20)
                    {
                        Debug.Log("ListView: list will never be filled as the item renderer function always returns a different size.");
                        break;
                    }
                }
                
                //TODO 拱门排列
                // HandleArchOrder1();
            }
            else if (_layout == ListLayoutType.SingleRow || _layout == ListLayoutType.FlowVertical)
            {
                int enterCounter = 0;
                while (HandleScrollRowAndFlowVertical(forceUpdate))
                {
                    enterCounter++;
                    forceUpdate = false;
                    if (enterCounter > 20)
                    {
                        Debug.Log("ListView: list will never be filled as the item renderer function always returns a different size.");
                        break;
                    }
                }

                //TODO 拱门排列
                // HandleArchOrder2();
            }
            else
            {
                HandleScrollPagenation(forceUpdate);
            }

            _boundsChanged = false;
        }
        
        bool HandleScrollColumAndFlowHorizontal(bool forceUpdate)
        {
            // scrollRectComponent
            var viewWidth = m_scrollRect.viewport.rect.width;
            var viewHeight = m_scrollRect.viewport.rect.height;
            var contentHeight = contentComponent.sizeDelta.y;
            float pos = Mathf.CeilToInt(contentComponent.anchoredPosition.y);
            // Debug.Log("Content Pos: "+pos);
            float max = pos + viewHeight;
            bool end = max == contentHeight;//这个标志表示当前需要滚动到最末，无论内容变化大小

            //寻找当前位置的第一条项目
            int newFirstIndex = GetIndexOnPos1(ref pos, forceUpdate);
            // Debug.Log(newFirstIndex);
            if (newFirstIndex == _firstIndex && !forceUpdate)
                return false;

            int oldFirstIndex = _firstIndex;
            _firstIndex = newFirstIndex;
            int curIndex = newFirstIndex;
            bool forward = oldFirstIndex > newFirstIndex;
            int childCount = this.numChildren;
            int lastIndex = oldFirstIndex + childCount - 1;
            int reuseIndex = forward ? lastIndex : oldFirstIndex;
            float curX = 0, curY = pos;
            bool needRender;
            float deltaSize = 0;
            float firstItemDeltaSize = 0;
            string identifyName = defaultItemPrefab.GetComponent<ListViewItem>().identifyName;
            string defaultIdentifyName = identifyName;
            int partSize = (int)((viewWidth - _columnGap * (_curLineItemCount - 1)) / _curLineItemCount);

            itemInfoVer++;
            // Debug.Log(itemInfoVer);
            while (curIndex < _realNumItems && (end || curY < max))
            {
                ItemInfo ii = _virtualItems[curIndex];

                if (ii.item == null || forceUpdate)
                {
                    if (ItemProvider != null)
                    {
                        identifyName = ItemProvider(curIndex % totalCount);
                        if (identifyName == null)
                            identifyName = defaultIdentifyName;
                        //TODO 动态加载特殊对象重新定义identifyName
                    }

                    if (ii.item != null && ii.item.identifyName != identifyName)
                    {
                        ii.item.selectType = m_selectType;
                        ii.isSelected = ii.item.isSelected;
                        RemoveChildToPool(ii.item);
                        ii.item = null;
                    }
                }

                if (ii.item == null)
                {
                    //搜索最适合的重用item，保证每次刷新需要新建或者重新render的item最少
                    if (forward)
                    {
                        for (int j = reuseIndex; j >= oldFirstIndex; j--)
                        {
                            ItemInfo ii2 = _virtualItems[j];
                            if (ii2.item != null && ii2.updateFlag != itemInfoVer && ii2.item.identifyName == identifyName)
                            {
                                ii2.item.selectType = m_selectType;
                                ii2.isSelected = ii2.item.isSelected;
                                ii.item = ii2.item;
                                ii2.item = null;
                                if (j == reuseIndex)
                                    reuseIndex--;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int j = reuseIndex; j <= lastIndex; j++)
                        {
                            ItemInfo ii2 = _virtualItems[j];
                            if (ii2.item != null && ii2.updateFlag != itemInfoVer && ii2.item.identifyName == identifyName)
                            {
                                ii2.item.selectType = m_selectType;
                                ii2.isSelected = ii2.item.isSelected;
                                ii.item = ii2.item;
                                ii2.item = null;
                                if (j == reuseIndex)
                                    reuseIndex++;
                                break;
                            }
                        }
                    }

                    if (ii.item != null)
                    {
                        SetChildIndex(ii.item, forward ? curIndex - newFirstIndex : numChildren);
                    }
                    else
                    {
                        ii.item = pool.Get(identifyName).gameObject.GetComponent<ListViewItem>();
                        if (forward)
                            this.AddChildAt(ii.item, curIndex - newFirstIndex);
                        else
                            this.AddChild(ii.item);
                    }
                    ii.item.selectType = m_selectType;
                    ii.item.isSelected = ii.isSelected;
                    needRender = true;
                }
                else
                    needRender = forceUpdate;

                if (needRender)
                {
                    if (_autoResizeItem && (_layout == ListLayoutType.SingleColumn || _columnCount > 0))
                        ii.item.SetSize(partSize, ii.item.size.y, true);

                    onItemRender?.Invoke(curIndex % totalCount, ii.item);
                    if (curIndex % _curLineItemCount == 0)
                    {
                        deltaSize += Mathf.CeilToInt(ii.item.size.y) - ii.size.y;
                        if (curIndex == newFirstIndex && oldFirstIndex > newFirstIndex)
                        {
                            //当内容向下滚动时，如果新出现的项目大小发生变化，需要做一个位置补偿，才不会导致滚动跳动
                            firstItemDeltaSize = Mathf.CeilToInt(ii.item.size.y) - ii.size.y;
                        }
                    }
                    ii.size.x = Mathf.CeilToInt(ii.item.size.x);
                    ii.size.y = Mathf.CeilToInt(ii.item.size.y);
                }

                ii.updateFlag = itemInfoVer;
                ii.item.SetXY(curX, curY);
                if (curIndex == newFirstIndex) //要显示多一条才不会穿帮
                    max += ii.size.y;

                curX += ii.size.x + _columnGap;

                if (curIndex % _curLineItemCount == _curLineItemCount - 1)
                {
                    curX = 0;
                    curY += ii.size.y + _lineGap;
                }
                curIndex++;
            }

            for (int i = 0; i < childCount; i++)
            {
                ItemInfo ii = _virtualItems[oldFirstIndex + i];
                if (ii.updateFlag != itemInfoVer && ii.item != null)
                {
                    ii.item.selectType = m_selectType;
                    ii.isSelected = ii.item.isSelected;
                    RemoveChildToPool(ii.item);
                    ii.item = null;
                }
            }

            childCount = m_displayItemList.Count;
            for (int i = 0; i < childCount; i++)
            {
                ListViewItem obj = _virtualItems[newFirstIndex + i].item;
                if (m_displayItemList[i] != obj)
                    SetChildIndex(obj, i);
            }
            
            //TODO 看看是否需要动态调整Content大小
            // if (deltaSize != 0 || firstItemDeltaSize != 0)
            //     this.scrollPane.ChangeContentSizeOnScrolling(0, deltaSize, 0, firstItemDeltaSize);
            
            if (curIndex > 0 && this.numChildren > 0 && contentComponent.localPosition.y <= 0 && GetChildAt(0).Position.y > -contentComponent.localPosition.y)//最后一页没填满！
                return true;
            else
                return false;
        }
        
        bool HandleScrollRowAndFlowVertical(bool forceUpdate)
        {
            var viewWidth = m_scrollRect.viewport.rect.width;
            var viewHeight = m_scrollRect.viewport.rect.height;
            var contentWidth = contentComponent.sizeDelta.x;
            float pos = -Mathf.CeilToInt(contentComponent.anchoredPosition.x);
            float max = pos + viewWidth;
            bool end = pos == contentWidth;//这个标志表示当前需要滚动到最末，无论内容变化大小

            //寻找当前位置的第一条项目
            int newFirstIndex = GetIndexOnPos2(ref pos, forceUpdate);
            // Debug.Log(newFirstIndex);
            if (newFirstIndex == _firstIndex && !forceUpdate)
                return false;

            int oldFirstIndex = _firstIndex;
            _firstIndex = newFirstIndex;
            int curIndex = newFirstIndex;
            bool forward = oldFirstIndex > newFirstIndex;
            int childCount = this.numChildren;
            int lastIndex = oldFirstIndex + childCount - 1;
            int reuseIndex = forward ? lastIndex : oldFirstIndex;
            float curX = pos, curY = 0;
            bool needRender;
            float deltaSize = 0;
            float firstItemDeltaSize = 0;
            string identifyName = defaultItemPrefab.GetComponent<ListViewItem>().identifyName;
            string defaultIdentifyName = identifyName;
            int partSize = (int)((viewHeight - _lineGap * (_curLineItemCount - 1)) / _curLineItemCount);

            itemInfoVer++;
            while (curIndex < _realNumItems && (end || curX < max))
            {
                ItemInfo ii = _virtualItems[curIndex];

                if (ii.item == null || forceUpdate)
                {
                    if (ItemProvider != null)
                    {
                        identifyName = ItemProvider(curIndex % totalCount);
                        if (identifyName == null)
                            identifyName = defaultIdentifyName;
                    }

                    if (ii.item != null && ii.item.identifyName != identifyName)
                    {
                        ii.item.selectType = m_selectType;
                        ii.isSelected = ii.item.isSelected;
                        RemoveChildToPool(ii.item);
                        ii.item = null;
                    }
                }

                if (ii.item == null)
                {
                    if (forward)
                    {
                        for (int j = reuseIndex; j >= oldFirstIndex; j--)
                        {
                            ItemInfo ii2 = _virtualItems[j];
                            if (ii2.item != null && ii2.updateFlag != itemInfoVer && ii2.item.identifyName == identifyName)
                            {
                                ii2.item.selectType = m_selectType;
                                ii2.isSelected = ii2.item.isSelected;
                                ii.item = ii2.item;
                                ii2.item = null;
                                if (j == reuseIndex)
                                    reuseIndex--;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int j = reuseIndex; j <= lastIndex; j++)
                        {
                            ItemInfo ii2 = _virtualItems[j];
                            if (ii2.item != null && ii2.updateFlag != itemInfoVer && ii2.item.identifyName == identifyName)
                            {
                                ii2.item.selectType = m_selectType;
                                ii2.isSelected = ii2.item.isSelected;
                                ii.item = ii2.item;
                                ii2.item = null;
                                if (j == reuseIndex)
                                    reuseIndex++;
                                break;
                            }
                        }
                    }

                    if (ii.item != null)
                    {
                        SetChildIndex(ii.item, forward ? curIndex - newFirstIndex : numChildren);
                    }
                    else
                    {
                        ii.item = pool.Get(identifyName).GetComponent<ListViewItem>();
                        if (forward)
                            this.AddChildAt(ii.item, curIndex - newFirstIndex);
                        else
                            this.AddChild(ii.item);
                    }
                    ii.item.selectType = m_selectType;
                    ii.item.isSelected = ii.isSelected;
                    needRender = true;
                }
                else
                    needRender = forceUpdate;

                if (needRender)
                {
                    if (_autoResizeItem && (_layout == ListLayoutType.SingleRow || _lineCount > 0))
                        ii.item.SetSize(ii.item.size.x, partSize, true);

                    onItemRender?.Invoke(curIndex % totalCount, ii.item);
                    if (curIndex % _curLineItemCount == 0)
                    {
                        deltaSize += Mathf.CeilToInt(ii.item.size.x) - ii.size.x;
                        if (curIndex == newFirstIndex && oldFirstIndex > newFirstIndex)
                        {
                            //当内容向下滚动时，如果新出现的一个项目大小发生变化，需要做一个位置补偿，才不会导致滚动跳动
                            firstItemDeltaSize = Mathf.CeilToInt(ii.item.size.x) - ii.size.x;
                        }
                    }
                    ii.size.x = Mathf.CeilToInt(ii.item.size.x);
                    ii.size.y = Mathf.CeilToInt(ii.item.size.y);
                }

                ii.updateFlag = itemInfoVer;
                ii.item.SetXY(curX, curY);
                if (curIndex == newFirstIndex) //要显示多一条才不会穿帮
                    max += ii.size.x;

                curY += ii.size.y + _lineGap;

                if (curIndex % _curLineItemCount == _curLineItemCount - 1)
                {
                    curY = 0;
                    curX += ii.size.x + _columnGap;
                }
                curIndex++;
            }

            for (int i = 0; i < childCount; i++)
            {
                ItemInfo ii = _virtualItems[oldFirstIndex + i];
                if (ii.updateFlag != itemInfoVer && ii.item != null)
                {
                    ii.item.selectType = m_selectType;
                    ii.isSelected = ii.item.isSelected;
                    RemoveChildToPool(ii.item);
                    ii.item = null;
                }
            }

            childCount = m_displayItemList.Count;
            for (int i = 0; i < childCount; i++)
            {
                ListViewItem obj = _virtualItems[newFirstIndex + i].item;
                if (m_displayItemList[i] != obj)
                    SetChildIndex(obj, i);
            }

            // if (deltaSize != 0 || firstItemDeltaSize != 0)
            //     this.scrollPane.ChangeContentSizeOnScrolling(deltaSize, 0, firstItemDeltaSize, 0);

            //滚动x轴取反
            if (curIndex > 0 && this.numChildren > 0 && -contentComponent.localPosition.x <= 0 && GetChildAt(0).Position.x > contentComponent.localPosition.x)//最后一页没填满！
                return true;
            else
                return false;
        }
        
        void HandleScrollPagenation(bool forceUpdate)
        {
            var viewWidth = m_scrollRect.viewport.rect.width;
            var viewHeight = m_scrollRect.viewport.rect.height;
            float pos = contentComponent.anchoredPosition.x;

            //寻找当前位置的第一条项目
            int newFirstIndex = GetIndexOnPos3(ref pos, forceUpdate);
            if (newFirstIndex == _firstIndex && !forceUpdate)
                return;

            int oldFirstIndex = _firstIndex;
            _firstIndex = newFirstIndex;

            //分页模式不支持不等高，所以渲染满一页就好了

            int reuseIndex = oldFirstIndex;
            int virtualItemCount = _virtualItems.Count;
            int pageSize = _curLineItemCount * _curLineItemCount2;
            int startCol = newFirstIndex % _curLineItemCount;
            
            int page = (int)(newFirstIndex / pageSize);
            int startIndex = page * pageSize;
            int lastIndex = startIndex + pageSize * 2; //测试两页
            bool needRender;
            string identifyName = defaultItemPrefab.GetComponent<ListViewItem>().identifyName;
            string defaultIdentifyName = identifyName;
            int partWidth = (int)((viewWidth - _columnGap * (_curLineItemCount - 1)) / _curLineItemCount);
            int partHeight = (int)((viewHeight - _lineGap * (_curLineItemCount2 - 1)) / _curLineItemCount2);
            itemInfoVer++;

            //先标记这次要用到的项目
            for (int i = startIndex; i < lastIndex; i++)
            {
                if (i >= _realNumItems)
                    continue;

                int col = i % _curLineItemCount;
                if (i - startIndex < pageSize)
                {
                    if (col < startCol)
                        continue;
                }
                else
                {
                    if (col > startCol)
                        continue;
                }

                ItemInfo ii = _virtualItems[i];
                ii.updateFlag = itemInfoVer;
            }

            ListViewItem lastObj = null;
            int insertIndex = 0;
            for (int i = startIndex; i < lastIndex; i++)
            {
                if (i >= _realNumItems)
                    continue;

                ItemInfo ii = _virtualItems[i];
                if (ii.updateFlag != itemInfoVer)
                    continue;

                if (ii.item == null)
                {
                    //寻找看有没有可重用的
                    while (reuseIndex < virtualItemCount)
                    {
                        ItemInfo ii2 = _virtualItems[reuseIndex];
                        if (ii2.item != null && ii2.updateFlag != itemInfoVer)
                        {
                            ii2.item.selectType = m_selectType;
                            ii2.isSelected = ii2.item.isSelected;
                            ii.item = ii2.item;
                            ii2.item = null;
                            break;
                        }
                        reuseIndex++;
                    }

                    if (insertIndex == -1)
                        insertIndex = GetChildIndex(lastObj) + 1;

                    if (ii.item == null)
                    {
                        if (ItemProvider != null)
                        {
                            identifyName = ItemProvider.Invoke(i % totalCount);
                            if (identifyName == null)
                                identifyName = defaultIdentifyName;
                        }

                        ii.item = pool.Get(identifyName).gameObject.GetComponent<ListViewItem>();
                        this.AddChildAt(ii.item, insertIndex);
                    }
                    else
                    {
                        insertIndex = SetChildIndexBefore(ii.item, insertIndex);
                    }
                    insertIndex++;

                    ii.item.selectType = m_selectType;
                    ii.item.isSelected = ii.isSelected;
                    needRender = true;
                }
                else
                {
                    needRender = forceUpdate;
                    insertIndex = -1;
                    lastObj = ii.item;
                }

                if (needRender)
                {
                    if (_autoResizeItem)
                    {
                        if (_curLineItemCount == _columnCount && _curLineItemCount2 == _lineCount)
                            ii.item.SetSize(partWidth, partHeight, true);
                        else if (_curLineItemCount == _columnCount)
                            ii.item.SetSize(partWidth, ii.item.size.y, true);
                        else if (_curLineItemCount2 == _lineCount)
                            ii.item.SetSize(ii.item.size.x, partHeight, true);
                    }

                    onItemRender?.Invoke(i % totalCount, ii.item);
                    ii.size.x = Mathf.CeilToInt(ii.item.size.x);
                    ii.size.y = Mathf.CeilToInt(ii.item.size.y);
                }
            }

            //排列item
            float borderX = (startIndex / pageSize) * viewWidth;
            float xx = borderX;
            float yy = 0;
            float lineHeight = 0;
            for (int i = startIndex; i < lastIndex; i++)
            {
                if (i >= _realNumItems)
                    continue;

                ItemInfo ii = _virtualItems[i];
                if (ii.updateFlag == itemInfoVer)
                    ii.item.SetXY(xx, yy);

                if (ii.size.y > lineHeight)
                    lineHeight = ii.size.y;
                if (i % _curLineItemCount == _curLineItemCount - 1)
                {
                    xx = borderX;
                    yy += lineHeight + _lineGap;
                    lineHeight = 0;

                    if (i == startIndex + pageSize - 1)
                    {
                        borderX += viewWidth;
                        xx = borderX;
                        yy = 0;
                    }
                }
                else
                    xx += ii.size.x + _columnGap;
            }

            //释放未使用的
            for (int i = reuseIndex; i < virtualItemCount; i++)
            {
                ItemInfo ii = _virtualItems[i];
                if (ii.updateFlag != itemInfoVer && ii.item != null)
                {
                    ii.item.selectType = m_selectType;
                    ii.isSelected = ii.item.isSelected;
                    RemoveChildToPool(ii.item);
                    ii.item = null;
                }
            }
        }
        
        int GetIndexOnPos1(ref float pos, bool forceUpdate)
        {
            if (_realNumItems < _curLineItemCount)
            {
                pos = 0;
                return 0;
            }

            if (numChildren > 0 && !forceUpdate)
            {
                float pos2 = Mathf.CeilToInt(this.GetChildAt(0).Position.y) ;
                // Debug.Log(pos2);
                if (pos2 + (_lineGap > 0 ? 0 : -_lineGap) > pos)
                {
                    for (int i = _firstIndex - _curLineItemCount; i >= 0; i -= _curLineItemCount)
                    {
                        pos2 -= (_virtualItems[i].size.y + _lineGap);
                        if (pos2 <= pos)
                        {
                            pos = pos2;
                            return i;
                        }
                    }

                    pos = 0;
                    return 0;
                }
                else
                {
                    float testGap = _lineGap > 0 ? _lineGap : 0;
                    for (int i = _firstIndex; i < _realNumItems; i += _curLineItemCount)
                    {
                        float pos3 = pos2 + _virtualItems[i].size.y;
                        if (pos3 + testGap > pos)
                        {
                            pos = pos2;
                            return i;
                        }
                        pos2 = pos3 + _lineGap;
                    }

                    pos = pos2;
                    return _realNumItems - _curLineItemCount;
                }
            }
            else
            {
                float pos2 = 0;
                float testGap = _lineGap > 0 ? _lineGap : 0;
                for (int i = 0; i < _realNumItems; i += _curLineItemCount)
                {
                    float pos3 = pos2 + _virtualItems[i].size.y;
                    if (pos3 + testGap > pos)
                    {
                        pos = pos2;
                        return i;
                    }
                    pos2 = pos3 + _lineGap;
                }

                pos = pos2;
                return _realNumItems - _curLineItemCount;
            }
        }
        
        int GetIndexOnPos2(ref float pos, bool forceUpdate)
        {
            if (_realNumItems < _curLineItemCount)
            {
                pos = 0;
                return 0;
            }

            if (numChildren > 0 && !forceUpdate)
            {
                float pos2 = this.GetChildAt(0).Position.x;
                
                if (pos2 + (_columnGap > 0 ? 0 : -_columnGap) > pos)
                {
                    for (int i = _firstIndex - _curLineItemCount; i >= 0; i -= _curLineItemCount)
                    {
                        pos2 -= (_virtualItems[i].size.x + _columnGap);
                        if (pos2 <= pos)
                        {
                            pos = pos2;
                            return i;
                        }
                    }

                    pos = 0;
                    return 0;
                }
                else
                {
                    float testGap = _columnGap > 0 ? _columnGap : 0;
                    for (int i = _firstIndex; i < _realNumItems; i += _curLineItemCount)
                    {
                        float pos3 = pos2 + _virtualItems[i].size.x;
                        if (pos3 + testGap > pos)
                        {
                            pos = pos2;
                            return i;
                        }
                        pos2 = pos3 + _columnGap;
                    }

                    pos = pos2;
                    return _realNumItems - _curLineItemCount;
                }
            }
            else
            {
                float pos2 = 0;
                float testGap = _columnGap > 0 ? _columnGap : 0;
                for (int i = 0; i < _realNumItems; i += _curLineItemCount)
                {
                    float pos3 = pos2 + _virtualItems[i].size.x;
                    if (pos3 + testGap > pos)
                    {
                        pos = pos2;
                        return i;
                    }
                    pos2 = pos3 + _columnGap;
                }

                pos = pos2;
                return _realNumItems - _curLineItemCount;
            }
        }

        int GetIndexOnPos3(ref float pos, bool forceUpdate)
        {
            pos = Mathf.Abs(pos);
            if (_realNumItems < _curLineItemCount)
            {
                pos = 0;
                return 0;
            }

            var viewWidth = m_scrollRect.viewport.rect.width;
            var viewHeight = m_scrollRect.viewport.rect.height;
            int page = Mathf.FloorToInt(pos / viewWidth);
            int startIndex = page * (_curLineItemCount * _curLineItemCount2);
            float pos2 = page * viewWidth;
            float testGap = _columnGap > 0 ? _columnGap : 0;
            for (int i = 0; i < _curLineItemCount; i++)
            {
                float pos3 = pos2 + _virtualItems[startIndex + i].size.x;
                if (pos3 + testGap > pos)
                {
                    pos = pos2;
                    return startIndex + i;
                }
                pos2 = pos3 + _columnGap;
            }

            pos = pos2;
            return startIndex + _curLineItemCount - 1;
        }
        
        public ListViewItem AddItemFromPool()
        {
            string identifyName = defaultItemPrefab.GetComponent<ListViewItem>().identifyName;
            ListViewItem obj = pool.Get(identifyName).GetComponent<ListViewItem>();

            return AddChild(obj);
        }
        
        public ListViewItem AddItemFromPool(string identifyName)
        {
            ListViewItem obj = pool.Get(identifyName).GetComponent<ListViewItem>();

            return AddChild(obj);
        }
        
        public ListViewItem AddChild(ListViewItem child)
        {
            AddChildAt(child, m_displayItemList.Count);
            return child;
        }
        
        public ListViewItem AddChildAt(ListViewItem child, int index)
        {
            InternalAddChildAt(child, index);
            return child;
        }

        private ListViewItem InternalAddChildAt(ListViewItem child, int index)
        {
            if (index >= 0 && index <= m_displayItemList.Count)
            {
                if (m_displayItemList.Contains(child))
                {
                    SetChildIndex(child, index);
                }
                else
                {
                    GameObject obj = child.gameObject;
                    obj.transform.SetParent(contentComponent);
                    //TODO 暂时初始化对象缩放大小为1 后期可定制化调整
                    obj.transform.localScale = Vector3.one;
                    m_displayItemList.Add(child);
                    child.Init(m_selectType, OnValueChanged, onItemClicked);
                    obj.SetActive(true);
                    SetBoundsChangedFlag();
                    return child;
                }
                return child;
            }
            else
            {
                throw new Exception("Invalid child index: " + index + ">" + m_displayItemList.Count);
            }
        }
        
        public void SetChildIndex(ListViewItem child, int index)
        {
            int oldIndex = m_displayItemList.IndexOf(child);
            if (oldIndex == index) return;
            if (oldIndex == -1) throw new ArgumentException("Not a child for this List");
            m_displayItemList.RemoveAt(oldIndex);
            if (index >= m_displayItemList.Count)
                m_displayItemList.Add(child);
            else
                m_displayItemList.Insert(index, child);
            //TODO 合批处理
            // InvalidateBatchingState(true);
        }
        
        public int SetChildIndexBefore(ListViewItem child, int index)
        {
            int oldIndex = m_displayItemList.IndexOf(child);
            if (oldIndex == -1)
                throw new ArgumentException("Not a child of this container");

            if (oldIndex < index)
                return _SetChildIndex(child, oldIndex, index - 1);
            else
                return _SetChildIndex(child, oldIndex, index);
        }
        
        int _SetChildIndex(ListViewItem child, int oldIndex, int index)
        {
            int cnt = m_displayItemList.Count;
            if (index > cnt)
                index = cnt;

            if (oldIndex == index)
                return oldIndex;

            m_displayItemList.RemoveAt(oldIndex);
            if (index >= cnt)
                m_displayItemList.Add(child);
            else
                m_displayItemList.Insert(index, child);
            
            int displayIndex = 0;
            if (_childrenRenderOrder == ChildrenRenderOrder.Ascent)
            {
                for (int i = 0; i < index; i++)
                {
                    ListViewItem g = m_displayItemList[i];
                    displayIndex++;
                }
                SetChildIndex(child, displayIndex);
            }
            else if (_childrenRenderOrder == ChildrenRenderOrder.Descent)
            {
                for (int i = cnt - 1; i > index; i--)
                {
                    ListViewItem g = m_displayItemList[i];
                    displayIndex++;
                }
                SetChildIndex(child, displayIndex);
            }

            SetBoundsChangedFlag();
            return index;
        }

        public ListViewItem GetChildAt(int index)
        {
            if (index >= 0 && index < numChildren)
            {
                return m_displayItemList[index];
            }
            else
            {
                throw new Exception("Invalid child index: " + index + ">" + numChildren);
            }
        }
        
        public int GetChildIndex(ListViewItem child)
        {
            return m_displayItemList.IndexOf(child);
        }
        
        public void RemoveChildrenToPool(int beginIndex, int endIndex)
        {
            if (endIndex < 0 || endIndex >= m_displayItemList.Count)
                endIndex = m_displayItemList.Count - 1;

            for (int i = beginIndex; i <= endIndex; ++i)
                RemoveChildToPoolAt(beginIndex);
        }
        
        public void RemoveChildToPoolAt(int index)
        {
            ListViewItem item = m_displayItemList[index];
            RemoveChild(item, false);
            pool.Put(item.identifyName, item.gameObject);
        }
        
        public void RemoveChildToPool(ListViewItem child)
        {
            if (child == null)
            {
                throw new Exception("Try to Remove Invalid child: Can not remove null");
            }
            RemoveChild(child, false);
            pool.Put(child.identifyName, child.gameObject);
        }

        public void RemoveChild(ListViewItem child, bool destroy)
        {
            int childIndex = m_displayItemList.IndexOf(child);
            if (childIndex != -1)
            {
                RemoveChildAt(childIndex, destroy);
            }
        }

        public ListViewItem RemoveChildAt(int index, bool destroy)
        {
            if (index >= 0 && index < numChildren)
            {
                ListViewItem child = m_displayItemList[index];
                if (destroy)
                {
                    Object.Destroy(child.gameObject);
                }
                m_displayItemList.RemoveAt(index);
                SetBoundsChangedFlag();

                return child;
            }
            else
                throw new Exception("Invalid child index: " + index + ">" + numChildren);
        }

        void CheckVirtualList()
        {
            if (_virtualListChanged != 0)
            {
                RefreshVirtualList(null);
            }
        }

        public List<int> GetSelection()
        {
            return GetSelection(null);
        }

        public List<int> GetSelection(List<int> result)
        {
            if (result == null)
                result = new List<int>();
            if (m_isVirtual)
            {
                int cnt = _realNumItems;
                for (int i = 0; i < cnt; i++)
                {
                    ItemInfo item = _virtualItems[i];
                    if (item.isSelected)
                    {
                        int j = i;
                        if (_loop)
                        {
                            j = i % totalCount;
                            if (result.Contains(j))
                                continue;
                        }
                        result.Add(j);
                    }
                }
            }
            else
            {
                int cnt = m_displayItemList.Count;
                for (int i = 0; i < cnt; i++)
                {
                    if (m_displayItemList[i] != null && m_displayItemList[i].isSelected)
                    {
                        result.Add(i);
                    }
                }
            }

            return result;
        }

        public void AddSelection(int index, bool scrollItToView)
        {
            if (m_selectType == ListSelectionMode.None)
            {
                Debug.LogWarning("Current Selection Type is [None]");
                return;
            }
            if (index >= _virtualItems.Count || index < 0)
            {
                Debug.LogError("Input Selection Index invalid");
                return;
            }

            CheckVirtualList();

            if (m_selectType == ListSelectionMode.Single)
                ClearSelection();

            //滚动至显示
            if (scrollItToView)
                ScrollToView(index);

            _lastSelectedIndex = index;
            if (m_isVirtual)
            {
                ItemInfo ii = _virtualItems[index];
                if (ii.item != null)
                {
                    ii.item.isSelected = true;
                }
                ii.isSelected = true;
            }
            else
            {
                var obj = GetChildAt(index);
                if (obj != null && !obj.isSelected)
                {
                    obj.isSelected = true;
                }
            }
        }
        
        public void RemoveSelection(int index)
        {
            if (m_selectType == ListSelectionMode.None)
                return;

            if (m_isVirtual)
            {
                ItemInfo ii = _virtualItems[index];
                if (ii.item != null)
                {
                    ii.item.isSelected = false;
                }
                ii.isSelected = false;
            }
            else
            {
                var obj = GetChildAt(index);

                if (obj != null)
                    obj.isSelected = false;
            }

        }

        public void ClearSelection()
        {
            if (m_isVirtual)
            {
                int cnt = _realNumItems;
                for (int i = 0; i < cnt; i++)
                {
                    ItemInfo ii = _virtualItems[i];
                    if (ii.item != null)
                    {
                        ii.item.selectType = m_selectType;
                        ii.item.isSelected = false;
                    }
                    ii.isSelected = false;
                }
            }
            else
            {
                int cnt = m_displayItemList.Count;
                for (int i = 0; i < cnt; i++)
                {
                    var obj = m_displayItemList[i];
                    if (obj != null)
                    {
                        obj.selectType = m_selectType;
                        obj.isSelected = false;
                    }
                }
            }
        }
        
        void OnValueChanged(ListViewItem item)
        {
            if (m_isVirtual)
            {
                if (item.isSelected)
                {
                    if (m_selectType == ListSelectionMode.Single)
                    {
                        for (int i = 0; i < itemCount; i++)
                        {
                            //找到对应项，设置为选中
                            if (_virtualItems[i].item == item)
                            {
                                _virtualItems[i].isSelected = true;
                                continue;
                            }

                            //取消之前的选中状态
                            if (_virtualItems[i].isSelected)
                            {
                                _virtualItems[i].isSelected = false;
                                if (_virtualItems[i].item != null)
                                    _virtualItems[i].item.isSelected = false;
                            }
                        }
                    }
                    else if (m_selectType == ListSelectionMode.Multiple)
                    {
                        //找到对应项，设置为选中
                        for (int i = 0; i < itemCount; i++)
                        {
                            if (_virtualItems[i].item == item)
                            {
                                _virtualItems[i].isSelected = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < itemCount; i++)
                        {
                            if (_virtualItems[i].item == item)
                            {
                                _virtualItems[i].isSelected = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    //找到对应项，设置为未选中
                    for (int i = 0; i < itemCount; i++)
                    {
                        if (_virtualItems[i].item == item)
                        {
                            _virtualItems[i].isSelected = false;
                            break;
                        }
                    }
                }
            }
        }

        public void ScrollToView(int index)
        {
            ScrollToView(index, false);
        }

        public void ScrollToView(int index, bool ani)
        {
            ScrollToView(index, ani, false);
        }

        public void ScrollToView(int index, bool ani, bool setFirst)
        {
            StopMovement();
            var viewWidth = m_scrollRect.viewport.rect.width;
            var viewHeight = m_scrollRect.viewport.rect.height;
            Vector2 dest = new Vector2();
            if (m_isVirtual)
            {
                if (totalCount <= 0)
                    return;
                CheckVirtualList();
                if (index >= _virtualItems.Count)
                    throw new Exception("Invalid child index: " + index + ">" + _virtualItems.Count);
                if (_loop)
                    index = Mathf.FloorToInt((float)_firstIndex / totalCount) * totalCount + index;
                
                
                ItemInfo ii = _virtualItems[index];
                if (_layout == ListLayoutType.SingleColumn || _layout == ListLayoutType.FlowHorizontal)
                {
                    float pos = 0;
                    for (int i = _curLineItemCount - 1; i < index; i += _curLineItemCount)
                        pos += _virtualItems[i].size.y + _lineGap;
                    dest.Set(0, pos);
                }
                else if (_layout == ListLayoutType.SingleRow || _layout == ListLayoutType.FlowVertical)
                {
                    float pos = 0;
                    for (int i = _curLineItemCount - 1; i < index; i += _curLineItemCount)
                        pos += _virtualItems[i].size.x + _columnGap;
                    dest.Set(pos, 0);
                }
                else
                {
                    int page = index / (_curLineItemCount * _curLineItemCount2);
                    dest.Set(page * viewWidth + (index % _curLineItemCount) * (ii.size.x + _columnGap),
                        (index / _curLineItemCount) % _curLineItemCount2 * (ii.size.y + _lineGap));
                }
            }
            else
            {
                ListViewItem item = GetChildAt(index);
                dest = item.Position;
            }
            
            if (scrollRectComponent != null)
            {
                if (dest.x != 0 && dest.x > (contentComponent.sizeDelta.x - viewWidth))
                {
                    dest.x = contentComponent.sizeDelta.x - viewWidth;
                }

                if (dest.y != 0 && dest.y > (contentComponent.sizeDelta.y - viewHeight))
                {
                    dest.y = contentComponent.sizeDelta.y - viewHeight;
                }

                if (ani)
                {
                    StartCoroutine(ScrollToWithAni(new Vector2(dest.x, dest.y)));
                }
                else
                {
                    contentComponent.anchoredPosition = new Vector2(-dest.x,dest.y); 
                }
            }
        }
        
        float m_scrollAniTime = 0.75f;
        public float ScrollAniTime
        {
            get { return m_scrollAniTime;}
            set { m_scrollAniTime = value; }
        }

        IEnumerator ScrollToWithAni(Vector2 dest)
        {
            bool needMoving = true;
            float distance = GetDistance(dest);;
            float move = Mathf.Abs(distance) * (Time.deltaTime/m_scrollAniTime);
            float direct = GetDirection(dest);
            if (direct<0)
            {
                move = -move;
            }
            
            while (needMoving)
            {
                yield return null;
                
                float offset = GetDistance(dest);
                Debug.Log(offset);
                if ((direct < 0 && offset <= 0)||(direct > 0 && offset >= 0))
                {
                    SetFinalPos(dest);
                    needMoving = false;
                    break;
                }
                
                var a = GetVector(move);
                contentComponent.anchoredPosition += a;
            }
            StopMovement();
        }

        void SetFinalPos(Vector2 dest)
        {
            dest.x = -dest.x;
            if (layout == (int)ListLayoutType.SingleRow || layout == (int)ListLayoutType.FlowVertical)
            {
                dest.y = 0;
                contentComponent.anchoredPosition = dest;
            }
            else
            {
                dest.x = 0;
                contentComponent.anchoredPosition = dest;
            }
        }

        int GetDirection(Vector2 dest)
        {
            if (layout == (int)ListLayoutType.SingleRow || layout == (int)ListLayoutType.FlowVertical)
            {
                if (-dest.x > contentComponent.anchoredPosition.x)
                    return 1;
                else
                    return -1;
            }
            else
            {
                if (dest.y > contentComponent.anchoredPosition.y)
                    return 1;
                else
                    return -1;
            }
        }

        float GetDistance(Vector2 dest)
        {
            if (layout == (int)ListLayoutType.SingleRow || layout == (int)ListLayoutType.FlowVertical)
            {
                return contentComponent.anchoredPosition.x + dest.x;
            }
            else
            {
                return contentComponent.anchoredPosition.y - dest.y;
            }
        }
        
        Vector2 GetVector(float value)
        {
            if (layout == (int)ListLayoutType.SingleRow || layout == (int)ListLayoutType.FlowVertical)
            {
                return new Vector2(value, 0);
            }
            else
            {
                return new Vector2(0, value);
            }
            return Vector2.zero;
        }

        public void RefreshVirtualList()
        {
            if (!m_isVirtual)
                throw new Exception("ListView: not virtual list");
            SetVirtualListChangedFlag(false);
        }
        
        public void ClearAll()
        {
            pool?.Clear();
            m_displayItemList?.Clear();
            _virtualItems?.Clear();
            pool = null;
            m_displayItemList = null;
            _virtualItems = null;
        }
        
        void StopMovement()
        {
            scrollRectComponent.StopMovement();
            StopAllCoroutines();
        }
        
        #endregion

        #region Editor Support

        public void EditorUpdate()
        {
#if UNITY_EDITOR
            EditorClearDirtyChildren();
            Init();
            itemCount = totalCount;
#endif
        }

        public void EditorClean()
        {
#if UNITY_EDITOR
            totalCount = 0;
            pool = new GameObjectPool();
            EditorClearDirtyChildren();
#endif
        }

        void EditorClearDirtyChildren()
        {
#if UNITY_EDITOR
            ClearAll();
            if (contentComponent!=null)
            {
                var children = contentComponent.GetComponentsInChildren<ListViewItem>();
                for (int i = 0; i < children.Length; i++)
                {
                    DestroyImmediate(children[i].gameObject);
                }
            }
#endif
        }

        #endregion
    }
}
