using System;
using UnityEngine;
using UnityEngine.UI;

namespace ND.UI.NDUI
{
    [RequireComponent(typeof(LayoutElement))]
    public class NDTableViewCell : MonoBehaviour
    {
        [HideInInspector, NonSerialized]
        public RectTransform rectTransform;

        protected LayoutElement layoutElement;

        [SerializeField]
        private string m_ReuseIdentifier;
        

        [HideInInspector,NonSerialized]
        public int id = -1;

        private void Awake()
        {
            layoutElement = this.GetComponent<LayoutElement>();
            rectTransform = this.transform as RectTransform;
        }

        public virtual string reuseIdentifier
        {
            get
            {
                if (string.IsNullOrEmpty(m_ReuseIdentifier))
                {
                    m_ReuseIdentifier = this.name;
                }
                return m_ReuseIdentifier;
            }
        }

        public void UpdateCellSize()
        {
            if (Application.isPlaying)
            {
                return;
            }
            if (layoutElement == null)
            {
                layoutElement = this.GetComponent<LayoutElement>();
            }
            //layoutElement.hideFlags = HideFlags.NotEditable;
            if (rectTransform == null)
            {
                rectTransform = this.transform as RectTransform;
            }
            layoutElement.preferredWidth = this.rectTransform.sizeDelta.x;
            layoutElement.preferredHeight = this.rectTransform.sizeDelta.y;
        }


        public virtual Vector2 GetSize(NDScrollRect tableView)
        {
            if (layoutElement == null) layoutElement = this.GetComponent<LayoutElement>();  //这里有可能直接访问没有实例化的prefab作为配置
            return new Vector2(layoutElement.preferredWidth, layoutElement.preferredHeight);
        }

        public virtual void SetData(NDScrollRect tableView, int id)
        {
            this.id = id;
        }

        public virtual void Unused(NDScrollRect tableView)
        {
            this.id = -1;
        }
    }
}