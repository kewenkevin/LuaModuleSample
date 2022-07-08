using UnityEngine;
using UnityEngine.UI;

namespace ND.UI.NDUI
{
    public class NDGridView : NDScrollRect
    {
        [SerializeField]
        private NDTableViewCell m_Prefab;

        [SerializeField]
        private Vector2 m_GridSpacing;

        [SerializeField]
        private int m_ConstraintCount = 1;

        public override void CheckLayout()
        {
            if(content == null) return;
            // RectTransformExtensions.SetAnchor(content,AnchorPresets.MiddleCenter);
            // RectTransformExtensions.SetPivot(content,PivotPresets.TopLeft);
            GridLayoutGroup layout = content.GetComponent<GridLayoutGroup>();
            if (layout == null)
            {
                layout = content.gameObject.AddComponent<GridLayoutGroup>();
            }
            if (direction == Direction.Vertical)
            {
                layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                layout.startAxis = GridLayoutGroup.Axis.Horizontal;
            }
            if (direction == Direction.Horizontal)
            {
                layout.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                layout.startAxis = GridLayoutGroup.Axis.Vertical;
            }
            if(m_Prefab!=null)
            {
                layout.cellSize = m_Prefab.GetSize(this);
            }
            layout.spacing = m_GridSpacing;
            layout.constraintCount = m_ConstraintCount;
            layout.hideFlags = HideFlags.DontSaveInEditor;


            ContentSizeFitter sizeFitter = content.GetComponent<ContentSizeFitter>();
            if (sizeFitter==null)
                sizeFitter = content.gameObject.AddComponent<ContentSizeFitter>();

            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            // sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            // sizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        }

        protected override float GetSize(NDTableViewCell item)
        {
            float size = contentSpacing;
            var axis = (int)direction;
            size += m_Prefab.GetSize(this)[axis];
            return size;
        }

        protected override NDTableViewCell InstantiateNextItem(int itemIdx)
        {
            NDTableViewCell nextItem = GetReusableCell(m_Prefab.reuseIdentifier);

            if (nextItem == null)
                nextItem = Instantiate(m_Prefab.gameObject).GetComponent<NDTableViewCell>();

            nextItem.transform.SetParent(content, false);
            nextItem.gameObject.SetActive(true);
            nextItem.SetData(this, itemIdx);
            cellOnUseDelegate?.Invoke(nextItem);
            return nextItem;
        }

        protected override float contentSpacing
        {
            get
            {
                return m_GridSpacing[(int)direction];
            }
        }

        protected int m_ContentConstraintCount = 0;

        protected override int contentConstraintCount
        {
            get
            {
                return m_ConstraintCount;
            }
        }

    }
}