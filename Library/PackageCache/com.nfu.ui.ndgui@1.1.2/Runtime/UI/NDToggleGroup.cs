using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using ND.UI.NDEvents;
using UnityEngine.EventSystems;
using UnityEngine;

namespace ND.UI.NDUI
{
    [AddComponentMenu("NDUI/NDToggle Group", 32)]
    [DisallowMultipleComponent]
    /// <summary>
    /// A component that represents a group of UI.Toggles.
    /// </summary>
    /// <remarks>
    /// When using a group reference the group from a UI.Toggle. Only one member of a group can be active at a time.
    /// </remarks>
    public class NDToggleGroup : UIBehaviour
    {
        [Serializable]
        /// <summary>
        /// UnityEvent callback for when a toggle is toggled.
        /// </summary>
        public class ToggleGroupEvent : UnityEvent2<int>
        { }
        
        public ToggleGroupEvent onValueChanged = new ToggleGroupEvent();
        
        [SerializeField] private bool m_AllowSwitchOff = false;

        /// <summary>
        /// Is it allowed that no toggle is switched on?
        /// </summary>
        /// <remarks>
        /// If this setting is enabled, pressing the toggle that is currently switched on will switch it off, so that no toggle is switched on. If this setting is disabled, pressing the toggle that is currently switched on will not change its state.
        /// Note that even if allowSwitchOff is false, the Toggle Group will not enforce its constraint right away if no toggles in the group are switched on when the scene is loaded or when the group is instantiated. It will only prevent the user from switching a toggle off.
        /// </remarks>
        public bool allowSwitchOff { get { return m_AllowSwitchOff; } set { m_AllowSwitchOff = value; } }

        protected List<NDToggle> m_Toggles = new List<NDToggle>();

        private int m_Selected = 0;

        public int selected
        {
            get => m_Selected;
            set { 
                if(m_Selected == value) return;
                Debug.Assert(value >= 0 && value < m_Toggles.Count, "Invalid Index");
                m_Selected = value;
                SetAllTogglesOff(false);
                m_Toggles[value].SetIsOnWithoutNotify(true);
            }
        }


        protected NDToggleGroup()
        { }

        // /// <summary>
        // /// Because all the Toggles have registered themselves in the OnEnabled, Start should check to
        // /// make sure at least one Toggle is active in groups that do not AllowSwitchOff
        // /// </summary>
        // protected override void Start()
        // {
        //     base.Start();
        // }

        private void ValidateToggleIsInGroup(NDToggle toggle)
        {
            if (toggle == null || !m_Toggles.Contains(toggle))
                throw new ArgumentException(string.Format("Toggle {0} is not part of ToggleGroup {1}", new object[] { toggle, this }));
        }

        public void NotifyToggleOn(int index, bool sendCallback = true)
        {
            Debug.Assert(index>=0 && index<m_Toggles.Count,"Invalid Index");
            if (m_Toggles[index].isOn) return;
            NotifyToggleOn(m_Toggles[index], sendCallback);
        }

        /// <summary>
        /// Notify the group that the given toggle is enabled.
        /// </summary>
        /// <param name="toggle">The toggle that got triggered on.</param>
        /// <param name="sendCallback">If other toggles should send onValueChanged.</param>
        public void NotifyToggleOn(NDToggle toggle, bool sendCallback = true)
        {
            ValidateToggleIsInGroup(toggle);
            // disable all toggles in the group
            for (var i = 0; i < m_Toggles.Count; i++)
            {
                if (m_Toggles[i] == toggle)
                {
                    if (sendCallback)
                    {
                        m_Selected = i;
                        onValueChanged.Invoke(i);
                    }
                    continue;
                }

                if (sendCallback)
                    m_Toggles[i].Set(false);
                else
                    m_Toggles[i].SetIsOnWithoutNotify(false);
            }
        }

        /// <summary>
        /// Unregister a toggle from the group.
        /// </summary>
        /// <param name="toggle">The toggle to remove.</param>
        public void UnregisterToggle(NDToggle toggle)
        {
            if (m_Toggles.Contains(toggle))
                m_Toggles.Remove(toggle);
        }

        /// <summary>
        /// Register a toggle with the toggle group so it is watched for changes and notified if another toggle in the group changes.
        /// </summary>
        /// <param name="toggle">The toggle to register with the group.</param>
        public void RegisterToggle(NDToggle toggle)
        {
            if (!m_Toggles.Contains(toggle))
                m_Toggles.Add(toggle);
            StartCoroutine(SortToggle());
        }

        IEnumerator SortToggle()
        {
            yield return new WaitForEndOfFrame();
            m_Toggles.Sort((a, b) => { return a.transform.GetSiblingIndex() == b.transform.GetSiblingIndex() ? 0 : a.transform.GetSiblingIndex() < b.transform.GetSiblingIndex() ? -1 : 1; });
            EnsureValidState();
            var oldSelected = m_Selected;
            for (var i = 0; i < m_Toggles.Count; i++)
            {
                if (m_Toggles[i].isOn)
                {
                    m_Selected = i;
                    break;
                }
            }
            if (oldSelected != m_Selected)
            {
                onValueChanged.Invoke(m_Selected);
            }
        }

        /// <summary>
        /// Ensure that the toggle group still has a valid state. This is only relevant when a ToggleGroup is Started
        /// or a Toggle has been deleted from the group.
        /// </summary>
        public void EnsureValidState()
        {
            if (!allowSwitchOff && !AnyTogglesOn() && m_Toggles.Count != 0)
            {
                m_Toggles[0].Set(true);
                NotifyToggleOn(m_Toggles[0]);
            }
        }

        /// <summary>
        /// Are any of the toggles on?
        /// </summary>
        /// <returns>Are and of the toggles on?</returns>
        public bool AnyTogglesOn()
        {
            return m_Toggles.Find(x => x.isOn) != null;
        }

        /// <summary>
        /// Returns the toggles in this group that are active.
        /// </summary>
        /// <returns>The active toggles in the group.</returns>
        /// <remarks>
        /// Toggles belonging to this group but are not active either because their GameObject is inactive or because the Toggle component is disabled, are not returned as part of the list.
        /// </remarks>
        public IEnumerable<NDToggle> ActiveToggles()
        {
            return m_Toggles.Where(x => x.isOn);
        }

        /// <summary>
        /// Switch all toggles off.
        /// </summary>
        /// <remarks>
        /// This method can be used to switch all toggles off, regardless of whether the allowSwitchOff property is enabled or not.
        /// </remarks>
        public void SetAllTogglesOff(bool sendCallback = true)
        {
            bool oldAllowSwitchOff = m_AllowSwitchOff;
            m_AllowSwitchOff = true;

            if (sendCallback)
            {
                for (var i = 0; i < m_Toggles.Count; i++)
                    m_Toggles[i].Set(false);
            }
            else
            {
                for (var i = 0; i < m_Toggles.Count; i++)
                    m_Toggles[i].SetIsOnWithoutNotify(false);
            }

            m_AllowSwitchOff = oldAllowSwitchOff;
        }
    }
}
