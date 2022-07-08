using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ND.UI.NDUI
{
    [AddComponentMenu("NDUI/NDScrollRect")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public abstract class NDScrollRect : UIBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, ICanvasElement, ILayoutGroup
    {
        public enum Direction
        {
            Horizontal = 0,
            Vertical = 1,
        }

        protected bool m_NeedReload;

        protected bool m_NeedRefresh;

        public CellOnUseDelegate cellOnUseDelegate;
        public CellOnUnUseDelegate cellOnUnuseDelegate;

        [SerializeField]
        public Direction direction = Direction.Vertical;

        [Tooltip("Total count, negative means INFINITE mode")]
        public int totalCount;

        protected float threshold = 0;
        [Tooltip("Reverse direction for dragging")]
        public bool reverseDirection = false;
        [Tooltip("Rubber scale for outside")]
        public float rubberScale = 1;

        protected int itemTypeStart = 0;
        protected int itemTypeEnd = 0;


        protected abstract float GetSize(NDTableViewCell item);

        protected float GetDimension(Vector2 vector)
        {
            if (direction == Direction.Horizontal) return -vector.x;
            if (direction == Direction.Vertical) return vector.y;
            return 0;
        }


        protected Vector2 GetVector(float value)
        {
            if (direction == Direction.Vertical) return new Vector2(0, value);
            if (direction == Direction.Horizontal) return new Vector2(-value, 0);
            return Vector2.zero;
        }

        protected virtual float contentSpacing
        {
            get
            {
                return 0;
            }
        }

        protected virtual int contentConstraintCount
        {
            get
            {
                return 1;
            }
        }

        // the first line
        int StartLine
        {
            get
            {
                return Mathf.CeilToInt((float)(itemTypeStart) / contentConstraintCount);
            }
        }

        // how many lines we have for now
        int CurrentLines
        {
            get
            {
                return Mathf.CeilToInt((float)(itemTypeEnd - itemTypeStart) / contentConstraintCount);
            }
        }

        // how many lines we have in total
        int TotalLines
        {
            get
            {
                return Mathf.CeilToInt((float)(totalCount) / contentConstraintCount);
            }
        }

        protected virtual bool UpdateItems(Bounds viewBounds, Bounds contentBounds)
        {
            bool changed = false;


            if (direction == Direction.Horizontal)
            {
                if (viewBounds.max.x < contentBounds.max.x - threshold)
                {
                    float size = DeleteItemAtEnd(), totalSize = size;
                    while (size > 0 && viewBounds.max.x < contentBounds.max.x - threshold - totalSize)
                    {
                        size = DeleteItemAtEnd();
                        totalSize += size;
                    }
                    if (totalSize > 0)
                        changed = true;
                }

                if (viewBounds.min.x > contentBounds.min.x + threshold)
                {
                    float size = DeleteItemAtStart(), totalSize = size;
                    while (size > 0 && viewBounds.min.x > contentBounds.min.x + threshold + totalSize)
                    {
                        size = DeleteItemAtStart();
                        totalSize += size;
                    }
                    if (totalSize > 0)
                        changed = true;
                }

                if (viewBounds.max.x > contentBounds.max.x)
                {
                    float size = NewItemAtEnd(), totalSize = size;
                    while (size > 0 && viewBounds.max.x > contentBounds.max.x + totalSize)
                    {
                        size = NewItemAtEnd();
                        totalSize += size;
                    }
                    if (totalSize > 0)
                        changed = true;
                }

                if (viewBounds.min.x < contentBounds.min.x)
                {
                    float size = NewItemAtStart(), totalSize = size;
                    while (size > 0 && viewBounds.min.x < contentBounds.min.x - totalSize)
                    {
                        size = NewItemAtStart();
                        totalSize += size;
                    }
                    if (totalSize > 0)
                        changed = true;
                }
            }

            if (direction == Direction.Vertical)
            {
                if (viewBounds.min.y > contentBounds.min.y + threshold)
                {
                    float size = DeleteItemAtEnd(), totalSize = size;
                    while (size > 0 && viewBounds.min.y > contentBounds.min.y + threshold + totalSize)
                    {
                        size = DeleteItemAtEnd();
                        totalSize += size;
                    }
                    if (totalSize > 0)
                        changed = true;
                }

                if (viewBounds.max.y < contentBounds.max.y - threshold)
                {
                    float size = DeleteItemAtStart(), totalSize = size;
                    while (size > 0 && viewBounds.max.y < contentBounds.max.y - threshold - totalSize)
                    {
                        size = DeleteItemAtStart();
                        totalSize += size;
                    }
                    if (totalSize > 0)
                        changed = true;
                }

                if (viewBounds.min.y < contentBounds.min.y)
                {
                    float size = NewItemAtEnd(), totalSize = size;
                    while (size > 0 && viewBounds.min.y < contentBounds.min.y - totalSize)
                    {
                        size = NewItemAtEnd();
                        totalSize += size;
                    }
                    if (totalSize > 0)
                        changed = true;
                }

                if (viewBounds.max.y > contentBounds.max.y)
                {
                    float size = NewItemAtStart(), totalSize = size;
                    while (size > 0 && viewBounds.max.y > contentBounds.max.y + totalSize)
                    {
                        size = NewItemAtStart();
                        totalSize += size;
                    }
                    if (totalSize > 0)
                        changed = true;
                }
            }


            return changed;
        }
        //==========TableView2==========

        public enum MovementType
        {
            Unrestricted, // Unrestricted movement -- can scroll forever
            Elastic, // Restricted but flexible -- can go past the edges, but springs back in place
            Clamped, // Restricted movement where it's not possible to go past the edges
        }

        public enum ScrollbarVisibility
        {
            Permanent,
            AutoHide,
        }

        [Serializable]
        public class ScrollRectEvent : UnityEvent<Vector2> { }

        [SerializeField]
        private RectTransform m_Content;
        public RectTransform content { get { return m_Content; } set { m_Content = value; } }

        [SerializeField]
        private MovementType m_MovementType = MovementType.Elastic;
        public MovementType movementType { get { return m_MovementType; } set { m_MovementType = value; } }

        [SerializeField]
        private float m_Elasticity = 0.1f; // Only used for MovementType.Elastic
        public float elasticity { get { return m_Elasticity; } set { m_Elasticity = value; } }

        [SerializeField]
        private bool m_CanScroll = true;
        public bool canScroll { get { return m_CanScroll; } set { m_CanScroll = value; } }
        
        [SerializeField]
        private bool m_Inertia = true;
        public bool inertia { get { return m_Inertia; } set { m_Inertia = value; } }

        [SerializeField]
        private bool m_Snap = false;
        public bool snap { get { return m_Snap; } set { m_Snap = value; } }

        [SerializeField]
        private float m_SnapLerp = 200;

        [SerializeField]
        private float m_DecelerationRate = 0.135f; // Only used when inertia is enabled
        public float decelerationRate { get { return m_DecelerationRate; } set { m_DecelerationRate = value; } }

        [SerializeField]
        private float m_ScrollSensitivity = 1.0f;
        public float scrollSensitivity { get { return m_ScrollSensitivity; } set { m_ScrollSensitivity = value; } }

        [SerializeField]
        private RectTransform m_Viewport;
        public RectTransform viewport { get { return m_Viewport; } set { m_Viewport = value; SetDirtyCaching(); } }

        [SerializeField]
        private NDScrollbar m_Scrollbar;
        public NDScrollbar scrollbar
        {
            get
            {
                return m_Scrollbar;
            }
            set
            {
                if (m_Scrollbar)
                    m_Scrollbar.onValueChanged.RemoveListener(SetNormalizedPosition);
                m_Scrollbar = value;
                if (m_Scrollbar)
                    m_Scrollbar.onValueChanged.AddListener(SetNormalizedPosition);
                SetDirtyCaching();
            }
        }


        [SerializeField]
        private ScrollbarVisibility m_ScrollbarVisibility;
        public ScrollbarVisibility scrollbarVisibility { get { return m_ScrollbarVisibility; } set { m_ScrollbarVisibility = value; SetDirtyCaching(); } }


        [SerializeField]
        private ScrollRectEvent m_OnValueChanged = new ScrollRectEvent();
        public ScrollRectEvent onValueChanged { get { return m_OnValueChanged; } set { m_OnValueChanged = value; } }

        // The offset from handle position to mouse down position
        private Vector2 m_PointerStartLocalCursor = Vector2.zero;
        private Vector2 m_ContentStartPosition = Vector2.zero;

        private RectTransform m_ViewRect;

        protected RectTransform viewRect
        {
            get
            {
                if (m_ViewRect == null)
                    m_ViewRect = m_Viewport;
                if (m_ViewRect == null)
                    m_ViewRect = (RectTransform)transform;
                return m_ViewRect;
            }
        }

        private Bounds m_ContentBounds;
        private Bounds m_ViewBounds;

        private Vector2 m_Velocity;
        public Vector2 velocity { get { return m_Velocity; } set { m_Velocity = value; } }

        private bool m_Dragging;

        private Vector2 m_PrevPosition = Vector2.zero;
        private Bounds m_PrevContentBounds;
        private Bounds m_PrevViewBounds;
        [NonSerialized]
        private bool m_HasRebuiltLayout = false;

        [System.NonSerialized]
        private RectTransform m_Rect;
        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        private RectTransform m_reusableCellContainer;
        private Dictionary<string, LinkedList<NDTableViewCell>> m_reusableCells;

        protected NDScrollRect()
        {
        }
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
        }
#endif

        protected override void Awake()
        {
            base.Awake();
            m_reusableCellContainer = new GameObject("ReusableCells", typeof(RectTransform)).GetComponent<RectTransform>();
            m_reusableCellContainer.SetParent(this.transform, false);
            m_reusableCellContainer.gameObject.SetActive(false);
            m_reusableCells = new Dictionary<string, LinkedList<NDTableViewCell>>();
            Vector2 pivot = Vector2.one * 0.5f;
            if(m_Content!=null)
            {
                pivot[(int)direction] = direction == Direction.Horizontal ? 0 : 1; //橫的是0 竪的是1  reverseDirection？
                m_Content.pivot = pivot;
            }
            CheckLayout();
        }

        //==========TableView2==========

        public abstract void CheckLayout();

        public void ClearCells()
        {
            if (Application.isPlaying)
            {
                itemTypeStart = 0;
                itemTypeEnd = 0;
                totalCount = 0;
                for (int i = content.childCount - 1; i >= 0; i--)
                {
                    StoreCellForReuse(content.GetChild(i).GetComponent<NDTableViewCell>());
                }
            }
        }

        public void ScrollToCell(int index, float speed)
        {
            if (totalCount >= 0 && (index < 0 || index >= totalCount))
            {
                Debug.LogWarningFormat("invalid index {0}", index);
                return;
            }
            if (speed <= 0)
            {
                Debug.LogWarningFormat("invalid speed {0}", speed);
                return;
            }
            StopMovement();
            StartCoroutine(ScrollToCellCoroutine(index, speed));
        }

        IEnumerator ScrollToCellCoroutine(int index, float speed)
        {
            bool needMoving = true;
            while (needMoving)
            {
                yield return null;
                if (!m_Dragging)
                {
                    float move = 0;
                    if (index < itemTypeStart)
                    {
                        move = -Time.deltaTime * speed;
                    }
                    else if (index >= itemTypeEnd)
                    {
                        move = Time.deltaTime * speed;
                    }
                    else
                    {
                        m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
                        var m_ItemBounds = GetBounds4Item(index);
                        var offset = 0.0f;
                        if (direction == Direction.Vertical)
                            offset = reverseDirection ? (m_ViewBounds.min.y - m_ItemBounds.min.y) : (m_ViewBounds.max.y - m_ItemBounds.max.y);
                        else if (direction == Direction.Horizontal)
                            offset = reverseDirection ? (m_ItemBounds.max.x - m_ViewBounds.max.x) : (m_ItemBounds.min.x - m_ViewBounds.min.x);
                        // check if we cannot move on
                        if (totalCount >= 0)
                        {
                            if (offset > 0 && itemTypeEnd == totalCount && !reverseDirection)
                            {
                                m_ItemBounds = GetBounds4Item(totalCount - 1);
                                // reach bottom
                                if ((direction == Direction.Vertical && m_ItemBounds.min.y > m_ViewBounds.min.y) ||
                                    (direction == Direction.Horizontal && m_ItemBounds.max.x < m_ViewBounds.max.x))
                                {
                                    needMoving = false;
                                    break;
                                }
                            }
                            else if (offset < 0 && itemTypeStart == 0 && reverseDirection)
                            {
                                m_ItemBounds = GetBounds4Item(0);
                                if ((direction == Direction.Vertical && m_ItemBounds.max.y < m_ViewBounds.max.y) ||
                                    (direction == Direction.Horizontal && m_ItemBounds.min.x > m_ViewBounds.min.x))
                                {
                                    needMoving = false;
                                    break;
                                }
                            }
                        }

                        float maxMove = Time.deltaTime * speed;
                        if (Mathf.Abs(offset) < maxMove)
                        {
                            needMoving = false;
                            move = offset;
                        }
                        else
                            move = Mathf.Sign(offset) * maxMove;
                    }
                    if (move != 0)
                    {
                        Vector2 offset = GetVector(move);
                        content.anchoredPosition += offset;
                        m_PrevPosition += offset;
                        m_ContentStartPosition += offset;
                    }
                }
            }
            StopMovement();
            UpdatePrevData();
        }


        public void RefreshCells()
        {
            if (!this.IsActive())
            {
                m_NeedRefresh = true;
                return;
            }

            if (totalCount > 0)
                itemTypeStart = Mathf.Clamp(itemTypeStart, 0, totalCount - 1);

            if (totalCount == 0) itemTypeStart = 0;

            StopMovement();
            //UpdateBounds();
            itemTypeEnd = itemTypeStart;

            if (totalCount >= 0 && itemTypeStart % contentConstraintCount != 0)
                Debug.LogWarning("Grid will become strange since we can't fill items in the first line");

            // Don't `Canvas.ForceUpdateCanvases();` here, or it will new/delete cells to change itemTypeStart/End
            for (int i = m_Content.childCount - 1; i >= 0; i--)
            {
                StoreCellForReuse(m_Content.GetChild(i).GetComponent<NDTableViewCell>());
            }


            var axis = (int)direction;

            float sizeToFill = 0, sizeFilled = 0;
            // m_ViewBounds may be not ready when ReloadData on Start
            sizeToFill = Mathf.Max(m_ContentBounds.size[axis], viewRect.rect.size[axis] + contentSpacing);

            while (sizeToFill > sizeFilled)
            {
                float size = reverseDirection ? NewItemAtStart() : NewItemAtEnd();
                if (size <= 0) break;
                sizeFilled += size;
            }

            if (sizeFilled < sizeToFill)
            {
                //calculate how many items can be added above the offset, so it still is visible in the view
                int itemsToAddCount = 0;
                while (sizeFilled < sizeToFill)
                {
                    float size = reverseDirection ? NewItemAtEnd() : NewItemAtStart(); //朝着反方向排
                    if (size <= 0) break;
                    sizeFilled += size;
                    ++itemsToAddCount;
                }
                Vector2 pos = m_Content.anchoredPosition;

                m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
                m_ContentBounds = GetBounds();
                if (direction == Direction.Vertical)
                {
                    pos[axis] = m_ContentBounds.max[axis] - m_ViewBounds.max[axis] - contentSpacing;
                }
                if (direction == Direction.Horizontal)
                {
                    pos[axis] = m_ContentBounds.max[axis] - m_ViewBounds.max[axis] + contentSpacing;
                }
                m_Content.anchoredPosition = pos;
                UpdateBounds();
                UpdateScrollbars(Vector2.zero);
            }
            else
            {
                UpdateScrollbars(Vector2.zero);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            EnsureLayoutHasRebuilt();
        }

        /// <summary>
        /// 刷新列表
        /// </summary>
        /// <param name="offset">直接定位哪一条</param>
        /// <param name="fillViewRect">比如说只有1000条，要定位到999条 那么下面就会超出滚动区域 为true的时候重新计算偏移位置 修正显示</param>
        public void ReloadData(int offset = 0, bool fillViewRect = true)
        {
            if(!this.IsActive()){
                m_NeedReload = true;
                return;
            }

            if (totalCount > 0)
                offset = Mathf.Clamp(offset, 0, totalCount-1);

            if (totalCount == 0) itemTypeStart = 0;

            StopMovement();
            itemTypeStart = reverseDirection ? totalCount - offset : offset;
            itemTypeEnd = itemTypeStart;

            if (totalCount >= 0 && itemTypeStart % contentConstraintCount != 0)
                Debug.LogWarning("Grid will become strange since we can't fill items in the first line");

            // Don't `Canvas.ForceUpdateCanvases();` here, or it will new/delete cells to change itemTypeStart/End
            for (int i = m_Content.childCount - 1; i >= 0; i--)
            {
                StoreCellForReuse(m_Content.GetChild(i).GetComponent<NDTableViewCell>());
            }

            var axis = (int)direction;

            float sizeToFill = 0, sizeFilled = 0;
            // m_ViewBounds may be not ready when ReloadData on Start
            sizeToFill = viewRect.rect.size[axis] + contentSpacing;

            while (sizeToFill > sizeFilled)
            {
                float size = reverseDirection ? NewItemAtStart() : NewItemAtEnd();
                if (size <= 0) break;
                sizeFilled += size;
            }

            if (fillViewRect && sizeFilled < sizeToFill)
            {
                //calculate how many items can be added above the offset, so it still is visible in the view
                int itemsToAddCount = 0;
                while (sizeFilled < sizeToFill)
                {
                    float size = reverseDirection ? NewItemAtEnd() : NewItemAtStart(); //朝着反方向排
                    if (size <= 0) break;
                    sizeFilled += size;
                    ++itemsToAddCount;
                }      
                int newOffset = offset - itemsToAddCount;
                if (newOffset < 0) newOffset = 0;
                if (newOffset != offset)
                {

                    if (m_Snap)
                    {
                        ReloadData(newOffset, false);                 //refill again, with the new offset value, and now with fillViewRect disabled.
                    }
                    else
                    {
                        Vector2 pos = m_Content.anchoredPosition;

                        m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
                        m_ContentBounds = GetBounds();
                        if(direction == Direction.Vertical)
                        { 
                            pos[axis] = m_ContentBounds.max[axis] - m_ViewBounds.max[axis] - contentSpacing;
                        }
                        if (direction == Direction.Horizontal)
                        {
                            pos[axis] = m_ContentBounds.max[axis] - m_ViewBounds.max[axis] + contentSpacing;
                        }
                        m_Content.anchoredPosition = pos;
                        UpdateBounds();
                        UpdateScrollbars(Vector2.zero);
                    }
                }
            }
            else
            {
                Vector2 pos = m_Content.anchoredPosition;
                pos[axis] = 0;
                m_Content.anchoredPosition = pos;
                UpdateScrollbars(Vector2.zero);
            }
        }

        protected float NewItemAtStart()
        {
            if (totalCount >= 0 && itemTypeStart - contentConstraintCount < 0)
            {
                return 0;
            }
            float size = 0;
            for (int i = 0; i < contentConstraintCount; i++)
            {
                itemTypeStart--;
                var newItem = InstantiateNextItem(itemTypeStart);
                newItem.rectTransform.SetAsFirstSibling();
                size = Mathf.Max(GetSize(newItem), size);
            }
            threshold = Mathf.Max(threshold, size * 1.5f);

            if (!reverseDirection)
            {
                Vector2 offset = GetVector(size);
                content.anchoredPosition += offset;
                m_PrevPosition += offset;
                m_ContentStartPosition += offset;
            }

            return size;
        }

        protected float DeleteItemAtStart()
        {
            // special case: when moving or dragging, we cannot simply delete start when we've reached the end
            if (((m_Dragging || m_Velocity != Vector2.zero) && totalCount >= 0 && itemTypeEnd >= totalCount - 1)
                || content.childCount == 0)
            {
                return 0;
            }

            float size = 0;
            for (int i = 0; i < contentConstraintCount; i++)
            {
                var oldItem = content.GetChild(0).GetComponent<NDTableViewCell>();
                size = Mathf.Max(GetSize(oldItem), size);
                StoreCellForReuse(oldItem.GetComponent<NDTableViewCell>());

                itemTypeStart++;

                if (content.childCount == 0)
                {
                    break;
                }
            }

            if (!reverseDirection)
            {
                Vector2 offset = GetVector(size);
                content.anchoredPosition -= offset;
                m_PrevPosition -= offset;
                m_ContentStartPosition -= offset;
            }
            return size;
        }


        protected float NewItemAtEnd()
        {
            if (totalCount >= 0 && itemTypeEnd >= totalCount)
            {
                return 0;
            }
            float size = 0;
            // issue 4: fill lines to end first
            int count = contentConstraintCount - (content.childCount % contentConstraintCount);
            for (int i = 0; i < count; i++)
            {
                var newItem = InstantiateNextItem(itemTypeEnd);
                size = Mathf.Max(GetSize(newItem), size);
                itemTypeEnd++;
                if (totalCount >= 0 && itemTypeEnd >= totalCount)
                {
                    break;
                }
            }
            threshold = Mathf.Max(threshold, size * 1.5f);

            if (reverseDirection)
            {
                Vector2 offset = GetVector(size);
                content.anchoredPosition -= offset;
                m_PrevPosition -= offset;
                m_ContentStartPosition -= offset;
            }

            return size;
        }

        protected float DeleteItemAtEnd()
        {
            if (((m_Dragging || m_Velocity != Vector2.zero) && totalCount >= 0 && itemTypeStart < contentConstraintCount)
                || content.childCount == 0)
            {
                return 0;
            }

            float size = 0;
            for (int i = 0; i < contentConstraintCount; i++)
            {
                var oldItem = content.GetChild(content.childCount - 1).GetComponent<NDTableViewCell>();
                size = Mathf.Max(GetSize(oldItem), size);
                StoreCellForReuse(oldItem.GetComponent<NDTableViewCell>());

                itemTypeEnd--;
                if (itemTypeEnd % contentConstraintCount == 0 || content.childCount == 0)
                {
                    break;  //just delete the whole row
                }
            }

            if (reverseDirection)
            {
                Vector2 offset = GetVector(size);
                content.anchoredPosition += offset;
                m_PrevPosition += offset;
                m_ContentStartPosition += offset;
            }
            return size;
        }

        protected abstract NDTableViewCell InstantiateNextItem(int itemIdx);


        private void StoreCellForReuse(NDTableViewCell cell)
        {
//#if UNITY_EDITOR
//            //if (!Application.isPlaying)
//            //{
//            //    GameObject.DestroyImmediate(cell.gameObject);
//            //    return;
//            //}
//#endif
            string reuseIdentifier = cell.reuseIdentifier;

            // if (string.IsNullOrEmpty(reuseIdentifier))
            if (reuseIdentifier == null)
            {
                reuseIdentifier = "";
            }
            InitReusableCells();
            if (!m_reusableCells.ContainsKey(reuseIdentifier))
            {
                m_reusableCells.Add(reuseIdentifier, new LinkedList<NDTableViewCell>());
            }
            m_reusableCells[reuseIdentifier].AddLast(cell);
            cell.transform.SetParent(m_reusableCellContainer, false);
            cellOnUnuseDelegate?.Invoke(cell);
            cell.Unused(this);
        }

        private void InitReusableCells()
        {
#if UNITY_EDITOR
            if(Application.isPlaying)
            {
                return;
            }
            if (m_reusableCells == null)
            {
                m_reusableCells = new Dictionary<string, LinkedList<NDTableViewCell>>();
            }
            if(m_reusableCellContainer == null)
            {
                m_reusableCellContainer = new GameObject("ReusableCells", typeof(RectTransform)).GetComponent<RectTransform>();
                m_reusableCellContainer.SetParent(this.transform, false);
                m_reusableCellContainer.gameObject.SetActive(false);
                m_reusableCellContainer.gameObject.hideFlags = HideFlags.DontSave;
            }
            
#endif
        }

        public NDTableViewCell GetReusableCell(string reuseIdentifier)
        {
            if (reuseIdentifier == null)
            {
                reuseIdentifier = "";
            }
            LinkedList<NDTableViewCell> cells;
            InitReusableCells();
            if (!m_reusableCells.TryGetValue(reuseIdentifier, out cells))
            {
                return null;
            }
            if (cells.Count == 0)
            {
                return null;
            }
            NDTableViewCell cell = cells.First.Value;
            cells.RemoveFirst();
            return cell;
        }


        //==========TableView2==========

        public virtual void Rebuild(CanvasUpdate executing)
        {
            if (executing == CanvasUpdate.Prelayout)
            {
                UpdateCachedData();
            }

            if (executing == CanvasUpdate.PostLayout)
            {
                UpdateBounds();
                UpdateScrollbars(Vector2.zero);
                UpdatePrevData();

                m_HasRebuiltLayout = true;
            }
        }

        public virtual void LayoutComplete()
        { }

        public virtual void GraphicUpdateComplete()
        { }

        void UpdateCachedData()
        {

        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (m_Scrollbar)
                m_Scrollbar.onValueChanged.AddListener(SetNormalizedPosition);

            CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
            if(m_NeedReload)
            {
                ReloadData();
            }else
            {
                if(m_NeedRefresh)
                {
                    RefreshCells();
                }
            }
            m_NeedReload = false;
            m_NeedRefresh = false;
        }

        protected override void OnDisable()
        {
            CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);

            if (m_Scrollbar)
                m_Scrollbar.onValueChanged.RemoveListener(SetNormalizedPosition);

            m_HasRebuiltLayout = false;
            m_Velocity = Vector2.zero;
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        public override bool IsActive()
        {
            return base.IsActive() && m_Content != null;
        }

        private void EnsureLayoutHasRebuilt()
        {
            if (!m_HasRebuiltLayout && !CanvasUpdateRegistry.IsRebuildingLayout())
                Canvas.ForceUpdateCanvases();
        }

        public virtual void StopMovement()
        {
            m_Velocity = Vector2.zero;
            StopAllCoroutines();
        }

        public virtual void OnScroll(PointerEventData data)
        {
            if (!IsActive())
                return;
            
            if (!m_CanScroll)
            {
                return;
            }

            EnsureLayoutHasRebuilt();
            UpdateBounds();

            Vector2 delta = data.scrollDelta;
            // Down is positive for scroll events, while in UI system up is positive.
            delta.y *= -1;
            if (direction == Direction.Vertical)
            {
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    delta.y = delta.x;
                delta.x = 0;
            }
            if (direction == Direction.Horizontal)
            {
                if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
                    delta.x = delta.y;
                delta.y = 0;
            }

            Vector2 position = m_Content.anchoredPosition;
            position += delta * m_ScrollSensitivity;
            if (m_MovementType == MovementType.Clamped)
                position += CalculateOffset(position - m_Content.anchoredPosition);

            SetContentAnchoredPosition(position);
            UpdateBounds();
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            m_Velocity = Vector2.zero;
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsActive())
                return;
            StopMovement();
            UpdateBounds();

            m_PointerStartLocalCursor = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, eventData.position, eventData.pressEventCamera, out m_PointerStartLocalCursor);
            m_ContentStartPosition = m_Content.anchoredPosition;
            m_Dragging = true;
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            m_Dragging = false;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsActive())
                return;

            if (!m_CanScroll)
            {
                return;
            }

            Vector2 localCursor;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, eventData.position, eventData.pressEventCamera, out localCursor))
                return;

            UpdateBounds();

            var pointerDelta = localCursor - m_PointerStartLocalCursor;
            Vector2 position = m_ContentStartPosition + pointerDelta;

            // Offset to get content into place in the view.
            Vector2 offset = CalculateOffset(position - m_Content.anchoredPosition);
            position += offset;
            if (m_MovementType == MovementType.Elastic)
            {
                //==========TableView2==========
                if (offset.x != 0)
                    position.x = position.x - RubberDelta(offset.x, m_ViewBounds.size.x) * rubberScale;
                if (offset.y != 0)
                    position.y = position.y - RubberDelta(offset.y, m_ViewBounds.size.y) * rubberScale;
                //==========TableView2==========
            }

            SetContentAnchoredPosition(position);
        }

        protected virtual void SetContentAnchoredPosition(Vector2 position)
        {
            if (direction == Direction.Vertical)
                position.x = m_Content.anchoredPosition.x;
            if (direction == Direction.Horizontal)
                position.y = m_Content.anchoredPosition.y;

            if (position != m_Content.anchoredPosition)
            {
                m_Content.anchoredPosition = position;
                UpdateBounds(true);
            }
        }

        public void EditorUpdate()
        {
#if UNITY_EDITOR
            SetContentAnchoredPosition(Vector2.zero);
#endif
        }

        protected virtual void LateUpdate()
        {
            if (!m_Content)
                return;

            EnsureLayoutHasRebuilt();
            UpdateScrollbarVisibility();
            UpdateBounds();
            float deltaTime = Time.unscaledDeltaTime;
            Vector2 offset = CalculateOffset(Vector2.zero);

            // if (!m_CanScroll)
            // {
            //     StopMovement();
            //     offset = Vector2.zero;
            //     return;
            // }
            
            if (!m_Dragging && (offset != Vector2.zero || m_Velocity != Vector2.zero))
            {
                Vector2 position = m_Content.anchoredPosition;
                for (int axis = 0; axis < 2; axis++)
                {
                    // Apply spring physics if movement is elastic and content has an offset from the view.
                    if (m_MovementType == MovementType.Elastic && offset[axis] != 0)
                    {
                        float speed = m_Velocity[axis];
                        position[axis] = Mathf.SmoothDamp(m_Content.anchoredPosition[axis], m_Content.anchoredPosition[axis] + offset[axis], ref speed, m_Elasticity, Mathf.Infinity, deltaTime);
                        m_Velocity[axis] = speed;
                    }
                    // Else move content according to velocity with deceleration applied.
                    else if (m_Inertia)
                    {
                        m_Velocity[axis] *= Mathf.Pow(m_DecelerationRate, deltaTime);
                        if (Mathf.Abs(m_Velocity[axis]) < 1)
                        {
                            m_Velocity[axis] = 0;
                        }
                        position[axis] += m_Velocity[axis] * deltaTime;
                    }
                    // If we have neither elaticity or friction, there shouldn't be any velocity.
                    else
                    {
                        m_Velocity[axis] = 0;
                    }
                }


                if (m_Snap && Mathf.Abs(m_Velocity[(int)direction]) < 10)
                {
                    SnapToCell();
                }

                if (m_Velocity != Vector2.zero)
                {
                    if (m_MovementType == MovementType.Clamped)
                    {
                        offset = CalculateOffset(position - m_Content.anchoredPosition);
                        position += offset;
                    }

                    SetContentAnchoredPosition(position);
                }
            }

            if (m_Dragging && m_Inertia)
            {
                Vector3 newVelocity = (m_Content.anchoredPosition - m_PrevPosition) / deltaTime;
                m_Velocity = Vector3.Lerp(m_Velocity, newVelocity, deltaTime * 10);
            }

            if (m_ViewBounds != m_PrevViewBounds || m_ContentBounds != m_PrevContentBounds || m_Content.anchoredPosition != m_PrevPosition)
            {
                UpdateScrollbars(offset);
                m_OnValueChanged.Invoke(normalizedPosition);
                UpdatePrevData();
            }
        }

        //private Vector3 FindClosestFrom(Vector3 start)
        //{
        //    Vector3 closest = Vector3.zero;
        //    float distance = Mathf.Infinity;

        //    foreach (Vector3 position in contentPositions)
        //    {
        //        if (Vector3.Distance(start, position) < distance)
        //        {
        //            distance = Vector3.Distance(start, position);
        //            closest = position;
        //        }
        //    }
        //    return closest;
        //}

        private void SnapToCell()
        {

            m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
            var dis = float.MaxValue;
            int index = 0;
            for (index = itemTypeStart; index < itemTypeEnd; index++)
            {
                var m_ItemBounds = GetBounds4Item(index);

                var offset = 0.0f;
                if (direction == Direction.Vertical)
                    offset = reverseDirection ? (m_ViewBounds.min.y - m_ItemBounds.min.y) : (m_ViewBounds.max.y - m_ItemBounds.max.y);
                else if (direction == Direction.Horizontal)
                    offset = reverseDirection ? (m_ItemBounds.max.x - m_ViewBounds.max.x) : (m_ItemBounds.min.x - m_ViewBounds.min.x);
                offset = Mathf.Abs(offset);
                if(offset < dis)
                {
                    dis = offset;
                }else
                {
                    --index;
                    break;
                }
            }
            ScrollToCell(index, m_SnapLerp);
        }

        private void UpdatePrevData()
        {
            if (m_Content == null)
                m_PrevPosition = Vector2.zero;
            else
                m_PrevPosition = m_Content.anchoredPosition;
            m_PrevViewBounds = m_ViewBounds;
            m_PrevContentBounds = m_ContentBounds;
        }

        protected void UpdateScrollbars(Vector2 offset)
        {
            if (m_Scrollbar)
            {
                m_Scrollbar.onValueChanged.RemoveListener(SetNormalizedPosition);
                if (direction == Direction.Horizontal)
                {
                    //==========TableView2==========
                    if (m_ContentBounds.size.x > 0 && totalCount > 0)
                    {
                        m_Scrollbar.size = Mathf.Clamp01((m_ViewBounds.size.x - Mathf.Abs(offset.x)) / m_ContentBounds.size.x * CurrentLines / TotalLines);
                    }
                    //==========TableView2==========
                    else
                        m_Scrollbar.size = 1;

                    m_Scrollbar.value = horizontalNormalizedPosition;
                }
                if (direction == Direction.Vertical)
                {
                    //==========TableView2==========
                    if (m_ContentBounds.size.y > 0 && totalCount > 0)
                    {
                        m_Scrollbar.size = Mathf.Clamp01((m_ViewBounds.size.y - Mathf.Abs(offset.y)) / m_ContentBounds.size.y * CurrentLines / TotalLines);
                    }
                    //==========TableView2==========
                    else
                        m_Scrollbar.size = 1;

                    m_Scrollbar.value = verticalNormalizedPosition;
                }
                m_Scrollbar.onValueChanged.AddListener(SetNormalizedPosition);
            }
        }

        public Vector2 normalizedPosition
        {
            get
            {
                return new Vector2(horizontalNormalizedPosition, verticalNormalizedPosition);
            }
            set
            {
                SetNormalizedPosition(value.x, Direction.Horizontal);
                SetNormalizedPosition(value.y, Direction.Vertical);
            }
        }

        public float horizontalNormalizedPosition
        {
            get
            {
                UpdateBounds();
                //==========TableView2==========
                if (totalCount > 0 && itemTypeEnd > itemTypeStart)
                {
                    //TODO: consider contentSpacing
                    float elementSize = m_ContentBounds.size.x / CurrentLines;
                    float totalSize = elementSize * TotalLines;
                    float offset = m_ContentBounds.min.x - elementSize * StartLine;

                    if (totalSize <= m_ViewBounds.size.x)
                        return (m_ViewBounds.min.x > offset) ? 1 : 0;
                    return (m_ViewBounds.min.x - offset) / (totalSize - m_ViewBounds.size.x);
                }
                else
                    return 0.5f;
                //==========TableView2==========
            }
            set
            {
                SetNormalizedPosition(value, Direction.Horizontal);
            }
        }

        public float verticalNormalizedPosition
        {
            get
            {
                UpdateBounds();
                //==========TableView2==========
                if (totalCount > 0 && itemTypeEnd > itemTypeStart)
                {
                    //TODO: consider contentSpacinge
                    float elementSize = m_ContentBounds.size.y / CurrentLines;
                    float totalSize = elementSize * TotalLines;
                    float offset = m_ContentBounds.max.y + elementSize * StartLine;

                    if (totalSize <= m_ViewBounds.size.y)
                        return (offset > m_ViewBounds.max.y) ? 1 : 0;
                    return (offset - m_ViewBounds.max.y) / (totalSize - m_ViewBounds.size.y);
                }
                else
                    return 0.5f;
                //==========TableView2==========
            }
            set
            {
                SetNormalizedPosition(value, Direction.Vertical);
            }
        }

        private void SetNormalizedPosition(float value)
        {
            SetNormalizedPosition(value, direction);
        }

        private void SetNormalizedPosition(float value, Direction direction)
        {
            if (direction != this.direction) return;
            //==========TableView2==========
            //快速定位 大幅度滚动会产生大量无用cell
            var axis = (int)direction;

            float elementSize = m_ContentBounds.size[axis] / CurrentLines;
            var t = TotalLines - Mathf.FloorToInt(m_ViewBounds.size[axis] / elementSize);

            if (totalCount <= 0 || itemTypeEnd <= itemTypeStart)
                return;

            if (reverseDirection)
                ReloadData(Mathf.FloorToInt(t * (1 -value)) * contentConstraintCount, true);
            else
                ReloadData(Mathf.FloorToInt(t * value) * contentConstraintCount, true);
            
            //==========TableView2==========


            //EnsureLayoutHasRebuilt();
            //UpdateBounds();

            ////==========TableView2==========
            //var axis = (int)direction;
            //Vector3 localPosition = m_Content.localPosition;
            //float newLocalPosition = localPosition[axis];
            //float elementSize = m_ContentBounds.size[axis] / CurrentLines;
            //float totalSize = elementSize * TotalLines;
            //if (direction == Direction.Horizontal)
            //{
            //    float offset = m_ContentBounds.min.x - elementSize * StartLine;
            //    newLocalPosition += m_ViewBounds.min.x - value * (totalSize - m_ViewBounds.size.x) - offset;
            //}
            //else if (direction == Direction.Vertical)
            //{
            //    float offset = m_ContentBounds.max.y + elementSize * StartLine;
            //    newLocalPosition -= offset - value * (totalSize - m_ViewBounds.size.y) - m_ViewBounds.max.y;
            //}
            ////==========TableView2==========

            //if (Mathf.Abs(localPosition[axis] - newLocalPosition) > 0.01f)
            //{
            //    localPosition[axis] = newLocalPosition;
            //    m_Content.localPosition = localPosition;
            //    m_Velocity[axis] = 0;
            //    UpdateBounds(true);
            //}
        }

        private static float RubberDelta(float overStretching, float viewSize)
        {
            return (1 - (1 / ((Mathf.Abs(overStretching) * 0.55f / viewSize) + 1))) * viewSize * Mathf.Sign(overStretching);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }

        public bool scrollingNeeded
        {
            get
            {
                if (Application.isPlaying)
                {
                    if (CurrentLines < TotalLines) return true;
                    
                    if (direction == Direction.Horizontal)
                        return m_ContentBounds.size.x > m_ViewBounds.size.x + 0.01f;

                    if (direction == Direction.Vertical)
                        return m_ContentBounds.size.y > m_ViewBounds.size.y + 0.01f;
                }
                return true;
            }
        }

        public virtual void SetLayoutHorizontal()
        {
        }

        public virtual void SetLayoutVertical()
        {
            m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
            m_ContentBounds = GetBounds();
        }

        void UpdateScrollbarVisibility()
        {
            if (m_Scrollbar && m_ScrollbarVisibility != ScrollbarVisibility.Permanent && m_Scrollbar.gameObject.activeSelf != scrollingNeeded)
                m_Scrollbar.gameObject.SetActive(scrollingNeeded);
        }

        private void UpdateBounds(bool updateItems = false)
        {
            m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
            m_ContentBounds = GetBounds();

            if (m_Content == null)
                return;

            // ============TableView2============
            // Don't do this in Rebuild
            if (Application.isPlaying && updateItems && UpdateItems(m_ViewBounds, m_ContentBounds))
            {
                Canvas.ForceUpdateCanvases();
                m_ContentBounds = GetBounds();
            }
            // ============TableView2============

            // Make sure content bounds are at least as large as view by adding padding if not.
            // One might think at first that if the content is smaller than the view, scrolling should be allowed.
            // However, that's not how scroll views normally work.
            // Scrolling is *only* possible when content is *larger* than view.
            // We use the pivot of the content rect to decide in which directions the content bounds should be expanded.
            // E.g. if pivot is at top, bounds are expanded downwards.
            // This also works nicely when ContentSizeFitter is used on the content.
            Vector3 contentSize = m_ContentBounds.size;
            Vector3 contentPos = m_ContentBounds.center;
            Vector3 excess = m_ViewBounds.size - contentSize;
            if (excess.x > 0)
            {
                contentPos.x -= excess.x * (m_Content.pivot.x - 0.5f);
                contentSize.x = m_ViewBounds.size.x;
            }
            if (excess.y > 0)
            {
                contentPos.y -= excess.y * (m_Content.pivot.y - 0.5f);
                contentSize.y = m_ViewBounds.size.y;
            }

            m_ContentBounds.size = contentSize;
            m_ContentBounds.center = contentPos;
        }

        private readonly Vector3[] m_Corners = new Vector3[4];
        private Bounds GetBounds()
        {
            if (m_Content == null)
                return new Bounds();

            var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            var toLocal = viewRect.worldToLocalMatrix;
            m_Content.GetWorldCorners(m_Corners);
            for (int j = 0; j < 4; j++)
            {
                Vector3 v = toLocal.MultiplyPoint3x4(m_Corners[j]);
                vMin = Vector3.Min(v, vMin);
                vMax = Vector3.Max(v, vMax);
            }

            var bounds = new Bounds(vMin, Vector3.zero);
            bounds.Encapsulate(vMax);
            return bounds;
        }

        private Bounds GetBounds4Item(int index)
        {
            if (m_Content == null)
                return new Bounds();

            var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            var toLocal = viewRect.worldToLocalMatrix;
            int offset = index - itemTypeStart;
            if (offset < 0 || offset >= m_Content.childCount)
                return new Bounds();
            var rt = m_Content.GetChild(offset) as RectTransform;
            if (rt == null)
                return new Bounds();
            rt.GetWorldCorners(m_Corners);
            for (int j = 0; j < 4; j++)
            {
                Vector3 v = toLocal.MultiplyPoint3x4(m_Corners[j]);
                vMin = Vector3.Min(v, vMin);
                vMax = Vector3.Max(v, vMax);
            }

            var bounds = new Bounds(vMin, Vector3.zero);
            bounds.Encapsulate(vMax);
            return bounds;
        }

        private Vector2 CalculateOffset(Vector2 delta)
        {
            Vector2 offset = Vector2.zero;
            if (m_MovementType == MovementType.Unrestricted)
                return offset;
            if (m_MovementType == MovementType.Clamped)
            {
                if (totalCount < 0)
                    return offset;
                if (GetDimension(delta) < 0 && itemTypeStart > 0)
                    return offset;
                if (GetDimension(delta) > 0 && itemTypeEnd < totalCount)
                    return offset;
            }

            Vector2 min = m_ContentBounds.min;
            Vector2 max = m_ContentBounds.max;

            if (direction == Direction.Horizontal)
            {
                min.x += delta.x;
                max.x += delta.x;
                if (min.x > m_ViewBounds.min.x)
                    offset.x = m_ViewBounds.min.x - min.x;
                else if (max.x < m_ViewBounds.max.x)
                    offset.x = m_ViewBounds.max.x - max.x;
            }

            if (direction == Direction.Vertical)
            {
                min.y += delta.y;
                max.y += delta.y;
                if (max.y < m_ViewBounds.max.y)
                    offset.y = m_ViewBounds.max.y - max.y;
                else if (min.y > m_ViewBounds.min.y)
                    offset.y = m_ViewBounds.min.y - min.y;
            }

            return offset;
        }

        protected void SetDirty()
        {
            if (!IsActive())
                return;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

        protected void SetDirtyCaching()
        {
            if (!IsActive())
                return;

            CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirtyCaching();
        }
#endif
    }
}