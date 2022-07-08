using System;
using ND.UI.NDEvents;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace ND.UI.NDUI
{
    /// <summary>
    /// A standard toggle that has an on / off state.
    /// </summary>
    /// <remarks>
    /// The toggle component is a Selectable that controls a child graphic which displays the on / off state.
    /// When a toggle event occurs a callback is sent to any registered listeners of UI.Toggle._onValueChanged.
    /// </remarks>
    [AddComponentMenu("NDUI/NDToggle", 31)]
    [RequireComponent(typeof(RectTransform))]
    public class NDToggle : Selectable, IPointerClickHandler, ISubmitHandler, ICanvasElement
    {

        [Serializable]
        /// <summary>
        /// UnityEvent callback for when a toggle is toggled.
        /// </summary>
        public class ToggleEvent : UnityEvent2<bool>
        { }

        /// <summary>
        /// Graphic the toggle should be working with.
        /// </summary>
        public GameObject graphic;

        public GameObject inversGraphic;

        [SerializeField]
        private NDToggleGroup m_Group;

        /// <summary>
        /// Group the toggle belongs to.
        /// </summary>
        public NDToggleGroup group
        {
            get { return m_Group; }
            set
            {
                SetToggleGroup(value, true);
                UpdateStatus();
            }
        }

        /// <summary>
        /// Allow for delegate-based subscriptions for faster events than 'eventReceiver', and allowing for multiple receivers.
        /// </summary>
        /// <example>
        /// <code>
        /// //Attach this script to a Toggle GameObject. To do this, go to Create>UI>Toggle.
        /// //Set your own Text in the Inspector window
        ///
        /// using UnityEngine;
        /// using UnityEngine.UI;
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     Toggle m_Toggle;
        ///     public Text m_Text;
        ///
        ///     void Start()
        ///     {
        ///         //Fetch the Toggle GameObject
        ///         m_Toggle = GetComponent<Toggle>();
        ///         //Add listener for when the state of the Toggle changes, to take action
        ///         m_Toggle.onValueChanged.AddListener(delegate {
        ///                 ToggleValueChanged(m_Toggle);
        ///             });
        ///
        ///         //Initialise the Text to say the first state of the Toggle
        ///         m_Text.text = "First Value : " + m_Toggle.isOn;
        ///     }
        ///
        ///     //Output the new state of the Toggle into Text
        ///     void ToggleValueChanged(Toggle change)
        ///     {
        ///         m_Text.text =  "New Value : " + m_Toggle.isOn;
        ///     }
        /// }
        /// </code>
        /// </example>
        public ToggleEvent onValueChanged = new ToggleEvent();

        // Whether the toggle is on
        [Tooltip("Is the toggle currently on or off?")]
        [SerializeField]
        private bool m_IsOn;


        public UnityEvent2 on = new UnityEvent2();
        public UnityEvent2 off = new UnityEvent2();

        protected NDToggle()
        { }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this) && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

#endif // if UNITY_EDITOR

        public virtual void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
                onValueChanged.Invoke(m_IsOn);
#endif
        }

        public virtual void LayoutComplete()
        { }

        public virtual void GraphicUpdateComplete()
        { }

        protected override void OnDestroy()
        {
            if (m_Group != null)
                m_Group.EnsureValidState();
            base.OnDestroy();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetToggleGroup(m_Group, false);
            UpdateStatus();
        }

        protected override void OnDisable()
        {
            SetToggleGroup(null, false);
            base.OnDisable();
        }

        private void SetToggleGroup(NDToggleGroup newGroup, bool setMemberValue)
        {
            bool isNewGroup = m_Group != newGroup;
            // Sometimes IsActive returns false in OnDisable so don't check for it.
            // Rather remove the toggle too often than too little.
            if (m_Group != null)
                m_Group.UnregisterToggle(this);

            // At runtime the group variable should be set but not when calling this method from OnEnable or OnDisable.
            // That's why we use the setMemberValue parameter.
            if (setMemberValue)
                m_Group = newGroup;

            // Only register to the new group if this Toggle is active.
            if (newGroup != null && IsActive())
                newGroup.RegisterToggle(this);

            // If we are in a new group, and this toggle is on, notify group.
            // Note: Don't refer to m_Group here as it's not guaranteed to have been set.
            if (newGroup != null && isOn && IsActive())
                newGroup.NotifyToggleOn(this, isNewGroup);
        }

        /// <summary>
        /// Whether the toggle is currently active.
        /// </summary>
        /// <example>
        /// <code>
        /// /Attach this script to a Toggle GameObject. To do this, go to Create>UI>Toggle.
        /// //Set your own Text in the Inspector window
        ///
        /// using UnityEngine;
        /// using UnityEngine.UI;
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     Toggle m_Toggle;
        ///     public Text m_Text;
        ///
        ///     void Start()
        ///     {
        ///         //Fetch the Toggle GameObject
        ///         m_Toggle = GetComponent<Toggle>();
        ///         //Add listener for when the state of the Toggle changes, and output the state
        ///         m_Toggle.onValueChanged.AddListener(delegate {
        ///                 ToggleValueChanged(m_Toggle);
        ///             });
        ///
        ///         //Initialize the Text to say whether the Toggle is in a positive or negative state
        ///         m_Text.text = "Toggle is : " + m_Toggle.isOn;
        ///     }
        ///
        ///     //Output the new state of the Toggle into Text when the user uses the Toggle
        ///     void ToggleValueChanged(Toggle change)
        ///     {
        ///         m_Text.text =  "Toggle is : " + m_Toggle.isOn;
        ///     }
        /// }
        /// </code>
        /// </example>

        public bool isOn
        {
            get { return m_IsOn; }

            set
            {
                Set(value, false);
            }
        }

        /// <summary>
        /// Set isOn without invoking onValueChanged callback.
        /// </summary>
        /// <param name="value">New Value for isOn.</param>
        public void SetIsOnWithoutNotify(bool value)
        {
            Set(value, false);
        }

        public void Set(bool value, bool sendCallback = true)
        {
            if (m_IsOn == value)
                return;

            // if we are in a group and set to true, do group logic
            m_IsOn = value;
            if (m_Group != null && IsActive())
            {
                if (m_IsOn || (!m_Group.AnyTogglesOn() && !m_Group.allowSwitchOff))
                {
                    m_IsOn = true;
                    m_Group.NotifyToggleOn(this, sendCallback);
                }
            }

            UpdateStatus();

            // Always send event when toggle is clicked, even if value didn't change
            // due to already active toggle in a toggle group being clicked.
            // Controls like Dropdown rely on this.
            // It's up to the user to ignore a selection being set to the same value it already was, if desired.
            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("Toggle.value", this);
                onValueChanged.Invoke(m_IsOn);
            }
        }

        public void UpdateStatus()
        {
            if (m_IsOn)
            {
                on.Invoke();
            }else
            {
                off.Invoke();
            }

            if(graphic!=null)
                graphic.SetActive(m_IsOn);

            if(inversGraphic!=null)
                inversGraphic.SetActive(!m_IsOn);
        }

        /// <summary>
        /// Assume the correct visual state.
        /// </summary>
        protected override void Start()
        {
            UpdateStatus();
        }

        private void InternalToggle()
        {
            if (!IsActive() || !IsInteractable())
                return;

            Set(!isOn);
        }

        /// <summary>
        /// React to clicks.
        /// </summary>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            InternalToggle();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            InternalToggle();
        }
    }
}
