using ND.UI.NDEvents;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ND.UI.NDUI
{
    [AddComponentMenu("NDUI/NDDragButton", 38)]
    public class NDDragButton : NDButton, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        public class ButtonDragEvent : UnityEvent2<Vector2> { }
        
        [SerializeField]
        private ButtonDragEvent m_OnDrag = new ButtonDragEvent();
        
        [SerializeField]
        private ButtonDragEvent m_OnDragOut = new ButtonDragEvent();
        
        [SerializeField]
        private ButtonDragEvent m_OnDragBegin = new ButtonDragEvent();
        
        [SerializeField]
        private ButtonDragEvent m_OnDragEnd = new ButtonDragEvent();
        
        [SerializeField]
        private ButtonDragEvent m_OnDragOutEnd = new ButtonDragEvent();
        
        public ButtonDragEvent onDrag
        {
            get { return m_OnDrag; }
            set { m_OnDrag = value; }
        }
        
        public ButtonDragEvent onDragOut
        {
            get { return m_OnDragOut; }
            set { m_OnDragOut = value; }
        }
        
        public ButtonDragEvent onDragBegin
        {
            get { return m_OnDragBegin; }
            set { m_OnDragBegin = value; }
        }
        
        public ButtonDragEvent onDragEnd
        {
            get { return m_OnDragEnd; }
            set { m_OnDragEnd = value; }
        }
        
        public ButtonDragEvent onDragOutEnd
        {
            get { return m_OnDragOutEnd; }
            set { m_OnDragOutEnd = value; }
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (!m_isInBtn)
            {
                onDragOut.Invoke(eventData.position);
            }
            onDrag.Invoke(eventData.position);
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            onDragBegin.Invoke(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!m_isInBtn)
            {
                onDragOutEnd.Invoke(eventData.position);
            }
            onDragEnd.Invoke(eventData.position);
        }
    }
}