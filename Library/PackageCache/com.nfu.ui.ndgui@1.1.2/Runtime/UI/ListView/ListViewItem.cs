using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ND.UI.NDUI
{
    [ExecuteInEditMode]
    public class ListViewItem : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] GameObject m_selectedGameObject;

        [SerializeField] string m_identifyName;

        public string identifyName
        {
            get { return m_identifyName; }
            set { m_identifyName = value; }
        }

        public int id { get; private set; }

        public Vector2 size
        {
            get
            {
                return m_rectTransform.sizeDelta;
            }
        }

        public void SetSize(float w, float h, bool ignorePivot = true)
        {
            m_rectTransform.sizeDelta = new Vector2(w, h);
        }
        
        /// <summary>
        /// 注意：此Position以物体左上原点为锚点计算，并且坐标系使用FGUI标准计算，即y轴朝下为正轴，x轴朝右为正轴
        /// </summary>
        public Vector3 Position
        {
            get
            {
                var x = m_rectTransform.localPosition.x - size.x / 2;
                var y = - (m_rectTransform.localPosition.y + size.y / 2);
                return new Vector3(x,y,m_rectTransform.localPosition.z);
            }
        }

        public void SetXY(float x, float y)
        {
            y = y + size.y / 2;
            y = -y;
            x = x + size.x / 2;
            m_rectTransform.localPosition = new Vector3(x, y, 0);
        }

        public ListView.ListSelectionMode selectType { get; set; }

        // public GameObjectPool.Item item;

        Action<ListViewItem> m_onValueChanged;

        Action<ListViewItem> m_onClicked; //适用于只在Item被单击时做操作的情况

        RectTransform m_rectTransform;

        NDButton m_button;
        NDImage m_raycastTarget;

        bool m_isSelected;

        public bool isSelected
        {
            get => m_isSelected;
            set
            {
                if (m_isSelected != value)
                {
                    m_isSelected = value;
                    UpdateSelectedUI();
                }
            }
        }

        void Awake()
        {
            id = GetInstanceID();

            CheckHasRayCastTarget();
            
            isSelected = false;
            m_rectTransform = GetComponent<RectTransform>();
            m_rectTransform.anchorMin = Vector2.up;
            m_rectTransform.anchorMax = Vector2.up;
            m_rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        public void Init(ListView.ListSelectionMode type, Action<ListViewItem> onValueChanged, Action<ListViewItem> onClicked)
        {
            selectType = type;
            m_onValueChanged = onValueChanged;
            m_onClicked = onClicked;
        }

        private void CheckHasRayCastTarget()
        {
            m_button = GetComponent<NDButton>();
            m_raycastTarget = GetComponent<NDImage>();
            if (m_button != null)
            {
                m_button.onClick.AddListener(OnClicked);
            }
            else if(m_raycastTarget == null || m_raycastTarget.raycastTarget == false)
            {
                NDHitArea hitArea = GetComponent<NDHitArea>();
                if (hitArea == null)
                {
                    hitArea = this.gameObject.AddComponent<NDHitArea>();
                }

                hitArea.raycastTarget = true;
            }
        }

        void OnClicked()
        {
            bool isValueChange = false;
            if (selectType == ListView.ListSelectionMode.Single)
            {
                if (!isSelected)
                    isValueChange = true;
                isSelected = true;
            }
            else if(selectType == ListView.ListSelectionMode.Multiple)
            {
                isValueChange = true;
                isSelected = !isSelected;
            }
            else if(selectType == ListView.ListSelectionMode.None)
            {
                isValueChange = true;
                isSelected = false;
            }

            if (isValueChange)
                m_onValueChanged?.Invoke(this);
            m_onClicked?.Invoke(this);
        }

        void ClearSelected()
        {
            isSelected = false;
        }

        protected virtual void UpdateSelectedUI()
        {
            if (m_selectedGameObject != null)
            {
                m_selectedGameObject.SetActive(isSelected);
            }
        }

        private void OnDestroy()
        {
            // m_onValueChanged = null;
            if(m_button!=null)
                m_button.onClick.RemoveListener(OnClicked);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_button!=null)
            {
                return;
            }
            OnClicked();
            // Debug.Log("On Click");
            // Pass(eventData, ExecuteEvents.pointerClickHandler);
        }

        private void Pass<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function)
            where T : IEventSystemHandler
        {
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data, results);
            var current = data.pointerCurrentRaycast.gameObject;
            for (int i = 0; i < results.Count; i++)
            {
                //判断穿透对象是否是需要要点击的对象
                if (current != results[i].gameObject)
                {
                    ExecuteEvents.Execute(results[i].gameObject, data, function);
                }
            }
        }
    }
}