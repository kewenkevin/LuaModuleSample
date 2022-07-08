using ND.UI.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace ND.UI.NDUI
{
    [AddComponentMenu("NDUI/NDRating", 11)]
    public class NDRating : UIBehaviour, IRating
    {
        [SerializeField]
        private Sprite m_BackgroundImage;

        [SerializeField]
        public Sprite backgroundImage
        {
            get { return m_BackgroundImage; }
            set { if (m_BackgroundImage == value) return; UpdateSprite(value, m_Background.transform); }
        }

        [SerializeField]
        private Sprite m_HighlightImage;

        [SerializeField]
        public Sprite highlightImage
        {
            get { return m_HighlightImage; }
            set { if (m_HighlightImage == value) return; UpdateSprite(value, m_Highlight.transform); }
        }

        [SerializeField]
        private HorizontalOrVerticalLayoutGroup m_Background;

        public HorizontalOrVerticalLayoutGroup background
        {
            get { return m_Background; }
            set { m_Background = value; }
        }
        

        [SerializeField]
        private HorizontalOrVerticalLayoutGroup m_Highlight;

        public HorizontalOrVerticalLayoutGroup highlight
        {
            get { return m_Highlight; }
            set { m_Highlight = value; }
        }

        [SerializeField]
        private float m_Spacing;

        public float spacing
        {
            get {
                if (m_Background != null) return m_Background.spacing;
                if (m_Highlight != null) return m_Highlight.spacing;
                return m_Spacing;
            }
            set
            {
                if (m_Background != null) m_Background.spacing = value;
                if (m_Highlight != null) m_Highlight.spacing = value;
                m_Spacing = value;
            }
        }

        [SerializeField]
        private int m_Total;

        public int total
        {
            get { return m_Total; }
            set
            {
                if (m_Total == value) return;
                m_Total = Mathf.Max(0, value);
                UpdateDisplay();
            }
        }

        public bool canModifyTotalByCode => true;

        [SerializeField]
        private float m_Current;

        public float current
        {
            get { return m_Current; }
            set
            {
                if (Mathf.Approximately(m_Current, value)) return;
                m_Current = Mathf.Clamp(value, 0, total);
                UpdateCurrent();
            }
        }

        protected override void Awake()
        {
            UpdateSpacing();
            UpdateDisplay();
        }

        private void UpdateStar(Sprite sprite, Transform container)
        {
            
            while (total < container.childCount)
            {
                Transform temp = container.GetChild(0);
                temp.SetParent(null);

#if UNITY_EDITOR
                if(Application.isPlaying)
                    Destroy(temp.gameObject);
#endif
            }

            while (total > container.childCount)
            {
                var img = new GameObject("Star", typeof(Image)).GetComponent<Image>();
                img.transform.SetParent(container, false);
                img.fillMethod = Image.FillMethod.Horizontal;
                img.type = Image.Type.Filled;
                img.sprite = sprite;
                img.gameObject.hideFlags = HideFlags.HideAndDontSave;
            }
        }    

        private void UpdateDisplay()
        {
            if(m_Background == null || m_Highlight == null) return;
            UpdateStar(m_BackgroundImage, m_Background.transform);
            UpdateStar(m_BackgroundImage, m_Highlight.transform);
            UpdateCurrent();
        }

        private void UpdateCurrent()
        {
            var container = m_Highlight.transform;
            for (int i = 0; i < container.childCount; i++)
            {
                var img = container.GetChild(i).GetComponent<Image>();
                var index = i + 1;
                if(index < current)
                {
                    img.fillAmount = 1;
                    img.enabled = true;
                }
                else
                {
                    if(index - current < 1)
                    {
                        img.type = Image.Type.Filled;
                        if(background is HorizontalLayoutGroup)
                        {
                            img.fillMethod = Image.FillMethod.Horizontal;
                            img.fillOrigin = 0;
                        }
                        else
                        {
                            img.fillMethod = Image.FillMethod.Vertical;
                            img.fillOrigin = 1;
                        }
                        
                        img.fillAmount = 1 + current - index;
                        img.enabled = true;
                    }else
                    {
                        img.enabled = false;
                    }
                }
            }
        }

        private void UpdateSprite(Sprite sprite, Transform container)
        {
            for (int i = 0; i < container.childCount; i++)
            {
                var img = container.GetChild(i).GetComponent<Image>();
                img.sprite = sprite;
                img.SetNativeSize();
            }
        }

        private void UpdateSpacing()
        {
            if (m_Background != null) m_Background.spacing = m_Spacing;
            if (m_Highlight != null) m_Highlight.spacing = m_Spacing;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            m_Current = Mathf.Clamp(m_Current, 0, total);

            UpdateSpacing();

            UpdateDisplay();
            
            if(m_Highlight)
                UpdateSprite(m_HighlightImage, m_Highlight.transform);

            if(m_Background)
                UpdateSprite(m_BackgroundImage, m_Background.transform);
        }

#endif
    }
}
