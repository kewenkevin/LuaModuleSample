using System.Collections;
using System.Collections.Generic;
using ND.UI.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace ND.UI.NDUI
{
    [AddComponentMenu("NDUI/NDRawRating", 11)]
    public class NDRawRating : UIBehaviour, IRating
    {
        [SerializeField] private GameObject[] m_Highlight;

        public GameObject[] Highlight
        {
            get => m_Highlight;
            set => m_Highlight = value;
        }

        public GameObject[] Background
        {
            get => m_Background;
            set => m_Background = value;
        }

        [SerializeField] private GameObject[] m_Background;

        [SerializeField] [Range(0, 30)] private int m_Total;

        public int total
        {
            get { return m_Total; }
            set
            {
                if (m_Total == value) return;
                m_Total = 0;
                if (Background != null)
                {
                    m_Total = Mathf.Clamp(value, 0, Background.Length);
                }

                UpdateBackground();
                if (m_Current > m_Total)
                {
                    m_Current = Mathf.Clamp(m_Current, 0, value);
                    UpdateCurrent();
                }
            }
        }

        public bool canModifyTotalByCode => false;

        [SerializeField] [Range(0, 30)] private int m_Current;

        public float current
        {
            get { return m_Current; }
            set
            {
                if (m_Current == value) return;
                m_Current = (int)Mathf.Clamp(value, 0, total);
                UpdateCurrent();
            }
        }


        protected override void Awake()
        {
            Debug.Assert(Background.Length == Highlight.Length);
            m_Total = Mathf.Clamp(m_Total, 0, Background.Length);
            m_Current = Mathf.Clamp(m_Current, 0, m_Total);
            UpdateBackground();
            UpdateCurrent();
        }

        private void UpdateBackground()
        {
            if (Background == null) return;
            for (int i = 0; i < Background.Length; i++)
            {
                if (Background[i] != null)
                    Background[i].SetActive(i < total);
            }
        }

        private void UpdateCurrent()
        {
            if (Highlight == null) return;
            for (int i = 0; i < Highlight.Length; i++)
            {
                if (Highlight[i] != null)
                    Highlight[i].SetActive(i < current);
            }
        }


#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (Background != null)
            {
                m_Total = Mathf.Clamp(m_Total, 0, Background.Length);
            }
            else
            {
                m_Total = 0;
            }

            m_Current = (int)Mathf.Clamp(m_Current, 0, total);
            UpdateBackground();
            UpdateCurrent();
        }

#endif
    }
}