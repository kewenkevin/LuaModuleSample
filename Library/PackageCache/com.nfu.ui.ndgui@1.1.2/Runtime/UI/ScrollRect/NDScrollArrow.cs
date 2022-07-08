using UnityEngine;
using UnityEngine.Serialization;

namespace ND.UI.NDUI
{
    public class NDScrollArrow : MonoBehaviour
    {
        [SerializeField]
        public GameObject start;

        [SerializeField]
        public GameObject end;

        [FormerlySerializedAs("ndScrollRect")] [FormerlySerializedAs("scrollRect")] [SerializeField]
        public NDScrollRect m_ndScrollRect;

        private void Awake()
        {
            if (m_ndScrollRect == null) m_ndScrollRect = this.gameObject.GetComponentInParent<NDScrollRect>();

            OnScroll(Vector2.zero);
        }

        private void OnEnable()
        {
            if (m_ndScrollRect)
                m_ndScrollRect.onValueChanged.AddListener(OnScroll);
        }

        private void OnDisable()
        {
            if (m_ndScrollRect)
                m_ndScrollRect.onValueChanged.RemoveListener(OnScroll);
        }
        private void OnScroll(Vector2 change)
        {
            if(m_ndScrollRect == null) return;
            if (!m_ndScrollRect.scrollingNeeded)
            {
                start.SetActive(false);
                end.SetActive(false);
                return;
            }

            float normalizedPosition = m_ndScrollRect.normalizedPosition[(int)m_ndScrollRect.direction];
            start.SetActive(normalizedPosition > 0);
            end.SetActive(normalizedPosition < 1);
        }
    }
}