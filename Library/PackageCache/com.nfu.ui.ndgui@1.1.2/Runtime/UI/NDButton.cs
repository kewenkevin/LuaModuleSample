using System;
using System.Collections;
using System.Collections.Generic;
using ND.UI.NDUI.CoroutineTween;
using ND.UI.NDEvents;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace ND.UI.NDUI
{
    [AddComponentMenu("NDUI/NDButton", 30)]
    public class NDButton : Selectable, IPointerClickHandler, ISubmitHandler
    {
        [SerializeField]
        private float m_DoubleTime = 0.3f;
        [SerializeField]
        private float m_LongClickTime = 0.4f;
        [SerializeField]
        private float m_LongPressTime = 0.4f;
        [SerializeField]
        private float m_LongIntervalTime = 0.1f;

        [Serializable]
        public class ButtonClickedEvent : UnityEvent2 { }

        [SerializeField]
        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

        [Serializable]
        public class ButtonDoubleClickedEvent : UnityEvent2 { }

        [SerializeField]
        private ButtonDoubleClickedEvent m_OnDoubleClick = new ButtonDoubleClickedEvent();

        [Serializable]
        public class ButtonLongClickedEvent : UnityEvent2 { }

        [SerializeField]
        private ButtonLongClickedEvent m_OnLongClick = new ButtonLongClickedEvent();

        [Serializable]
        public class ButtonLongPressedEvent : UnityEvent2 { }

        [SerializeField]
        private ButtonLongPressedEvent m_OnLongPress = new ButtonLongPressedEvent();

        [Serializable]
        public class ButtonDownEvent : UnityEvent2 { }

        [SerializeField]
        private ButtonDownEvent m_OnDown = new ButtonDownEvent();

        [Serializable]
        public class ButtonUpEvent : UnityEvent2 { }

        [SerializeField]
        private ButtonUpEvent m_OnUp = new ButtonUpEvent();
        
        [Serializable]
        public class ButtonInteractiveChangeEvent : UnityEvent2 { }

        [SerializeField]
        private ButtonInteractiveChangeEvent m_OnInteractiveChange = new ButtonInteractiveChangeEvent();

        [SerializeField]
        public class ButtonPointerExitEvent : UnityEvent2 { }
        
        [SerializeField]
        private ButtonPointerExitEvent m_OnPointerExit = new ButtonPointerExitEvent();
        
        [SerializeField]
        public class ButtonPointerEnterEvent : UnityEvent2 { }
        
        [SerializeField]
        private ButtonPointerEnterEvent m_OnPointerEnter = new ButtonPointerEnterEvent();

        private Vector3 rawScale;
        private Color rawColor;


        [SerializeField] 
        private bool m_EnableButtonStateTextColor;
        
        [SerializeField] 
        private NDTextColorStyle[] m_ButtonStateTextColorStyles;
        
        [Serializable]
        private struct ButtonTextControlData
        {
            public bool Enable;
            public NDTextColorStyle ColorStyle;
        }
        
        protected override void Awake()
        {
            base.Awake();
            rawScale = transform.localScale;
            if (targetGraphic != null)
            {
                rawColor = targetGraphic.color;
            }
        }

        private bool lastIsInteractable;
        public override bool IsInteractable()
        {
            var isInteractable = base.IsInteractable();
            if (lastIsInteractable != isInteractable)
            {
                lastIsInteractable = isInteractable;
            }

            return isInteractable;
        }

        protected virtual void OnInteractableChange(bool isInteractable)
        {
            RefreshButtonTextStyle(YButtonState.Up);
            m_OnInteractiveChange?.Invoke();
        }
        
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            RefreshButtonTextStyle(YButtonState.Up);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StopAllCoroutines();
            if(m_ScaleTweenInfo.shouldTween) 
            {
                transform.localScale = rawScale;
            }
            
        }

        [Serializable]
        public struct ScaleTweenInfo
        {
            public bool shouldTween;
            public AnimationCurve downScaleCurve;
            public AnimationCurve UpScaleCurve;
        }

        [SerializeField]
        private ScaleTweenInfo m_ScaleTweenInfo;

        private NDTweenRunner<IndAnimationCurveTween> scaleTweenRunner;

        protected NDButton()
        {
            scaleTweenRunner = new NDTweenRunner<IndAnimationCurveTween>();
            scaleTweenRunner.Init(this);
        }

        public ButtonClickedEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }

        public ButtonDoubleClickedEvent onDoubleClick
        {
            get { return m_OnDoubleClick; }
            set { m_OnDoubleClick = value; }
        }

        public ButtonLongClickedEvent onLongClick
        {
            get { return m_OnLongClick; }
            set { m_OnLongClick = value; }
        }

        public ButtonLongPressedEvent onLongPress
        {
            get { return m_OnLongPress; }
            set { m_OnLongPress = value; }
        }

        public ButtonDownEvent onDown
        {
            get { return m_OnDown; }
            set { m_OnDown = value; }
        }

        public ButtonUpEvent onUp
        {
            get { return m_OnUp; }
            set { m_OnUp = value; }
        }

        public ButtonInteractiveChangeEvent onInteractiveChange
        {
            get { return m_OnInteractiveChange; }
            set { m_OnInteractiveChange = value; }
        }
        
        public ButtonPointerExitEvent onPointerExit
        {
            get { return m_OnPointerExit;}
            set { m_OnPointerExit = value; }
        }
        
        public ButtonPointerEnterEvent onPointerEnter
        {
            get { return m_OnPointerEnter;}
            set { m_OnPointerEnter = value; }
        }

        protected float m_firstClickTime = 0;
        protected float m_secondClickTime = 0;
        protected float m_firstPressTimeForClick = 0;
        protected bool m_isLongClick = false;
        protected float m_firstPressTimeForPress = 0;
        protected int m_pressCount = 0;
        protected bool m_isFouces = false;
        protected bool m_isInBtn = true;

        protected bool shouldClick { get { return m_OnClick.GetPersistentEventCount() > 0 || m_OnClick.GetListenerCount() > 0; } }
        protected bool shouldDoubleClick { get { return (m_OnDoubleClick.GetPersistentEventCount() > 0 || m_OnDoubleClick.GetListenerCount() > 0) && m_DoubleTime > 0; } }
        protected bool shouldLongClick { get { return (m_OnLongClick.GetPersistentEventCount() > 0 || m_OnLongClick.GetListenerCount() > 0) && m_LongClickTime > 0; } }
        protected bool shouldLongPress { get { return (m_OnLongPress.GetPersistentEventCount() > 0 || m_OnLongPress.GetListenerCount() > 0) && m_LongPressTime > 0; } }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            RefreshButtonTextStyle(YButtonState.Over);
            
            base.OnPointerEnter(eventData);
            m_isInBtn = true;
            if (m_pressCount > 0)
            {
                m_isFouces = true;
            }
            onPointerEnter.Invoke();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            RefreshButtonTextStyle(YButtonState.Up);
            
            base.OnPointerExit(eventData);
            m_isInBtn = false;
            if (m_pressCount > 0)
            {
                m_isFouces = false;
            }
            onPointerExit.Invoke();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (!IsActive() || !IsInteractable())
                return;
            if (shouldLongClick)
            {
                m_firstPressTimeForClick = Time.unscaledTime;
            }
            if (shouldLongPress)
            {
                m_firstPressTimeForPress = Time.unscaledTime;
                m_isFouces = true;
            }
            OnDown();
            //Tween
            ScaleTween(true);
            
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (m_firstPressTimeForClick != 0)
            {
                m_firstPressTimeForClick = 0;
                if (m_isLongClick)
                {
                    m_isLongClick = !m_isLongClick;
                }
            }
            if (m_firstPressTimeForPress != 0)
            {
                m_firstPressTimeForPress = 0;
                if (m_pressCount != 0)
                {
                    m_pressCount = 0;
                }
                m_isFouces = false;
            }
            OnUp();
            //Tween
            ScaleTween(false);
            
        }

        private void ScaleTween(bool down)
        {
            if (!IsInteractable() || !m_ScaleTweenInfo.shouldTween)
            {
                return;
            }
            
            var tween = new IndAnimationCurveTween();
            tween.AddOnChangedCallback(SetScale);
            
            if(down)
            {
                tween.animationCurve = m_ScaleTweenInfo.downScaleCurve;
            }
            else
            {
                tween.animationCurve = m_ScaleTweenInfo.UpScaleCurve;
            }
            
            scaleTweenRunner.StartTween(tween);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            if (!IsActive() || !IsInteractable())
                return;
            if (m_firstClickTime > 0)
            {
                m_secondClickTime = eventData.clickTime;
                if (m_secondClickTime - m_firstClickTime <= m_DoubleTime)
                {
                    OnDoubleClick();
                    m_firstClickTime = 0;
                    m_secondClickTime = 0;
                    return;
                }
                m_firstClickTime = 0;
                m_secondClickTime = 0;
            }
            if (shouldDoubleClick)
            {
                m_firstClickTime = eventData.clickTime;
            }
            else
            {
                if (m_isLongClick)
                {
                    OnLongClick();
                    m_isLongClick = false;
                }
                else
                {
                    OnClick();
                }
            }
            //Pass(eventData, ExecuteEvents.pointerClickHandler);
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

        private void Update()
        {
            //双击
            if (m_firstClickTime > 0)
            {
                float interval = Time.unscaledTime - m_firstClickTime;
                if (interval > m_DoubleTime)
                {
                    //双击失败  点击或长点击
                    if (m_isLongClick)
                    {
                        OnLongClick();
                        m_isLongClick = false;
                    }
                    else
                    {
                        OnClick();
                    }
                    m_firstClickTime = 0;
                }
            }
            //长点击
            if (m_firstPressTimeForClick > 0)
            {
                float interval = Time.unscaledTime - m_firstPressTimeForClick;
                if (interval >= m_LongClickTime)
                {
                    m_isLongClick = true;
                    m_firstPressTimeForClick = 0;
                }
            }
            //长按
            if (m_firstPressTimeForPress > 0)
            {
                float interval = Time.unscaledTime - m_firstPressTimeForPress;
                if (m_pressCount == 0)
                {
                    if (interval >= m_LongPressTime)
                    {
                        OnLongPress();
                        m_pressCount++;
                    }
                }
                else
                { 
                    if (interval >= m_LongPressTime + m_LongIntervalTime * m_pressCount)
                    {
                        Debug.Log(m_pressCount);
                        //长按 连续触发
                        if (m_isFouces)
                        {
                            OnLongPress();
                        }
                        m_pressCount++;
                    }
                }
            }
        }

        private void OnClick()
        {
            m_OnClick.Invoke();
        }

        private void OnDoubleClick()
        {
            m_OnDoubleClick.Invoke();
        }

        private void OnLongClick()
        {
            m_OnLongClick.Invoke();
        }

        private void OnLongPress()
        {
            m_OnLongPress.Invoke();
        }

        private void OnDown()
        {
            RefreshButtonTextStyle(YButtonState.Down);
            m_OnDown.Invoke();
        }

        private void OnUp()
        {
            RefreshButtonTextStyle(YButtonState.Over);
            m_OnUp.Invoke();
        }

        private void SetScale(float f)
        {
            transform.localScale = new Vector3(rawScale.x * f, rawScale.y * f, 1);
        }

        //---------------------------------------------------------------------------------------------
        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();
            if (!IsActive() || !IsInteractable())
                return;

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
        }

        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            m_OnClick.Invoke();
        }

        private IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;
            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }
            DoStateTransition(currentSelectionState, false);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            rawScale = transform.localScale;
            RefreshButtonTextStyle(YButtonState.Up);
        }
        
        
#endif
        
        [SerializeField]
        private NDText m_Text;
        
        private void RefreshButtonTextStyle(YButtonState state)
        {
            var text = m_Text;

            if (!m_EnableButtonStateTextColor || text == null)
            {
                return;
            }

            if (!IsInteractable())
            {
                //禁用交互状态下使用禁用交互态的样式
                var disableStyle = GetButtonStateColorStyle(YButtonState.DisableInteractive, false);
                if (disableStyle != null)
                {
                    ApplyButtonTextColor(disableStyle, m_Text);
                    return;
                }

                //如果没有禁用交互态，则使用正态样式
                state = YButtonState.Up;
            }
            
            var colorStyle = GetButtonStateColorStyle(state, true);
            ApplyButtonTextColor(colorStyle, m_Text);
        }

        private void ApplyButtonTextColor(NDTextColorStyle colorStyle, NDText text)
        {
            if (colorStyle != null)
            {
                text.colorStyle = null;
                colorStyle.Apply(text);
            }
        }

        private NDTextColorStyle GetButtonStateColorStyle(YButtonState state, bool lookUpAValidStyle)
        {
            var index = Math.Max(0, Math.Min((int) state, m_ButtonStateTextColorStyles.Length));

            if (!lookUpAValidStyle)
            {
                return m_ButtonStateTextColorStyles[index];
            }
            
            for (var i = index; i >= 0; i--)
            {
                if (m_ButtonStateTextColorStyles[i] != null)
                {
                    return m_ButtonStateTextColorStyles[i];
                }
            }

            return null;
        }
        
        private enum YButtonState
        {
            Up = 0,
            Over = 1,
            Down = 2,
            DisableInteractive = 3
            
        }
        
    }
}
