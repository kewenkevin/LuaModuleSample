using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ND.UI.NDUI
{
    [AddComponentMenu("NDUI/NDTableView")]
    public class NDTableView : NDScrollRect
    {
        public GetSizeForCellDelegate getCellSizeDelegate;
        public GetCellReuseIdentifierDelegate getCellReuseIdentifierDelegate;

        [SerializeField]
        private float m_CellSpacing;

        [SerializeField]
        private NDTableViewCell[] m_TableViewCells;

        private Dictionary<string, NDTableViewCell> m_TableViewCellMap;

        string m_DefaultReuseIdentifier;
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            ResetTableView();
        }
#endif
        private void ResetTableView()
        {
            //
            Mask mask = this.transform.GetComponentInChildren<Mask>();
            if(mask == null)
            {
                return;
            }
            Transform viewport = mask.transform;
            //Transform viewport = this.transform.Find("Viewport");
            //if(viewport == null)
            //{
            //    return;
            //}
            

            RectTransform viewportRT = viewport.GetComponent<RectTransform>();
            viewportRT.anchorMin = Vector2.zero;
            viewportRT.anchorMax = Vector2.one;
            viewportRT.sizeDelta = Vector2.zero;
            viewportRT.pivot = Vector2.up;
            this.viewport = viewportRT;

            ContentSizeFitter sizeFitter = this.transform.GetComponentInChildren<ContentSizeFitter>();
            if (sizeFitter == null)
            {
                return;
            }
            Transform content = sizeFitter.transform;
            //if(content == null)
            //{
            //    return;
            //}
            while(content.childCount > 0)
            {
                GameObject.DestroyImmediate(content.GetChild(0).gameObject);
            }
            RectTransform contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0, 0.5f);
            contentRT.anchorMax = new Vector2(0, 0.5f);
            contentRT.sizeDelta = new Vector2(200, 200);
            contentRT.pivot = new Vector2(0, 0.5f);
            this.content = contentRT;

            Scrollbar scrollbar = this.transform.GetComponentInChildren<Scrollbar>();
            if (scrollbar == null)
            {
                return;
            }
            NDScrollRect.Direction dir = NDScrollRect.Direction.Horizontal;
            if (scrollbar.direction == Scrollbar.Direction.LeftToRight || scrollbar.direction == Scrollbar.Direction.RightToLeft)
            {
                dir = NDScrollRect.Direction.Horizontal;
            }
            else
            {
                dir = NDScrollRect.Direction.Vertical;
            }
            //Transform scrollbar = this.transform.Find("Scrollbar Horizontal");
            
            //if (scrollbar == null)
            //{
            //    scrollbar = this.transform.Find("Scrollbar Vertical");
            //    dir = YScrollRect.Direction.Vertical;
            //    if (scrollbar == null)
            //    {
            //        return;
            //    }
            //}
            this.scrollbar = scrollbar.GetComponent<NDScrollbar>();
            this.direction = dir;
            this.scrollbarVisibility = NDScrollRect.ScrollbarVisibility.Permanent;

        }

        protected override void Awake()
        {
            base.Awake();
            m_TableViewCellMap = new Dictionary<string, NDTableViewCell>();
            foreach (var cell in m_TableViewCells)
            {
                if(m_TableViewCellMap.ContainsKey(cell.reuseIdentifier))
                {
                    Debug.LogError("多个CellPrefab reuseIdentifier重复，请检查CellPrefab reuseIdentifier设置");
                    continue;
                }
                m_TableViewCellMap.Add(cell.reuseIdentifier, cell);
            }
            if (m_TableViewCells.Length > 0)
            {
                m_DefaultReuseIdentifier = m_TableViewCells[0].reuseIdentifier;
            }
        }

        protected override float GetSize(NDTableViewCell item)
        {
            float size = contentSpacing;
            if (getCellSizeDelegate != null)
                size += getCellSizeDelegate.Invoke(this, item);
            else
                size += item.GetSize(this)[(int)direction];
            return size;
        }

        private void InitInEditor()
        {
            if(m_TableViewCellMap != null)
            {
                return;
            }
            m_TableViewCellMap = new Dictionary<string, NDTableViewCell>();
            foreach (var cell in m_TableViewCells)
            {
                if (m_TableViewCellMap.ContainsKey(cell.reuseIdentifier))
                {
                    Debug.LogError("多个CellPrefab reuseIdentifier重复，请检查CellPrefab reuseIdentifier设置");
                    continue;
                }
                m_TableViewCellMap.Add(cell.reuseIdentifier, cell);
            }
            if (m_TableViewCells.Length > 0)
            {
                m_DefaultReuseIdentifier = m_TableViewCells[0].reuseIdentifier;
            }
        }

        protected override NDTableViewCell InstantiateNextItem(int itemIdx)
        {
#if UNITY_EDITOR
            if(!Application.isPlaying)
            {
                InitInEditor();
            }
#endif
            NDTableViewCell nextItem = null;
            string id = m_DefaultReuseIdentifier;
            if (getCellReuseIdentifierDelegate != null)
            {
                id = getCellReuseIdentifierDelegate.Invoke(itemIdx);
                if(string.IsNullOrEmpty(id))
                {
                    id = m_DefaultReuseIdentifier;
                }
            }

            nextItem = GetReusableCell(id);


            if (nextItem == null)
            {
                if (m_TableViewCellMap.TryGetValue(id, out var prefab))
                {
                    // nextItem = Instantiate(prefab.gameObject).GetComponent<YTableViewCell>();
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        nextItem = (UnityEditor.PrefabUtility.InstantiatePrefab(prefab.gameObject, this.transform) as GameObject).GetComponent<NDTableViewCell>();
                    }
#endif
                    if (Application.isPlaying)
                    {
                        nextItem = Instantiate(prefab.gameObject).GetComponent<NDTableViewCell>();
                    }
                    
                    nextItem.gameObject.hideFlags = HideFlags.DontSave;
                }
                else
                {
                    Debug.LogError("找不到对应的Cell,请检查reuseIdentifier设置是否正确或者CellPrefab是否存在");
                    return null;
                }
            }

            nextItem.transform.SetParent(content, false);
            nextItem.gameObject.SetActive(true);
            nextItem.SetData(this, itemIdx);
            cellOnUseDelegate?.Invoke(nextItem);
            return nextItem;
        }


        public override void CheckLayout() {

            if(content == null) return;
            HorizontalOrVerticalLayoutGroup layout = content.GetComponent<HorizontalOrVerticalLayoutGroup>();

            if(layout != null)
            {
                if ((direction == Direction.Vertical && layout is HorizontalLayoutGroup) 
                    || (direction == Direction.Horizontal && layout is VerticalLayoutGroup))
                {
                    DestroyImmediate(layout);
                    layout = null;
                }
                
            }

            ContentSizeFitter sizeFitter = content.GetComponent<ContentSizeFitter>();
            if (sizeFitter == null)
                sizeFitter = content.gameObject.AddComponent<ContentSizeFitter>();

            if (direction == Direction.Vertical)
            {
                if(layout == null)
                    layout = content.gameObject.AddComponent<VerticalLayoutGroup>();

                layout.childAlignment = TextAnchor.UpperCenter;

                sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }

            if (direction == Direction.Horizontal)
            {
                if (layout == null)
                    layout = content.gameObject.AddComponent<HorizontalLayoutGroup>();

                layout.childAlignment = TextAnchor.MiddleLeft;

                sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                sizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            }
            layout.childControlWidth = layout.childControlHeight = false;
            layout.childForceExpandHeight = layout.childForceExpandWidth = false;
            layout.spacing = m_CellSpacing;
            //layout.hideFlags = HideFlags.DontSaveInEditor | HideFlags.HideInInspector;
            layout.hideFlags = HideFlags.NotEditable;
            sizeFitter.hideFlags = HideFlags.NotEditable;
        }

        protected override float contentSpacing
        {
            get
            {
                return m_CellSpacing;
            }
        }
    }
}