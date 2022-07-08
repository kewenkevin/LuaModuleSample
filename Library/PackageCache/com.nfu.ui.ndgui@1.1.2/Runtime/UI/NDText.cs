using System;
using System.Reflection;
using ND.UI.Core;
using ND.UI.I18n;
using ND.UI.NDUI.CoroutineTween;
using ND.UI.NDEvents;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ND.UI.NDUI
{
    [ExecuteAlways]
    [AddComponentMenu("NDUI/NDText", 10)]
    public class NDText : Text , IColorStyleUseAble , ITextStyleUseAble , ILocalizationable
    {
        public static Func<string, string> localizationProvider;

        [SerializeField]
        protected string m_LocalizationKey = String.Empty;

        public string LocalizationKey
        {
            get { return m_LocalizationKey; }
            set
            {
                m_LocalizationKey = value;
                UpdateLocalization();
            }
        }

        public int LocalizationGearType => (int)GearTypeState.TextStr;

        public bool IsLocalized
        {
            get
            {
                return !string.IsNullOrEmpty(m_LocalizationKey);
            }
        }

#if UNITY_EDITOR
        private string rawText;

        private bool isLocalizationUpdated;
#endif

        public override string text
        {
            get { return base.text; }
            set
            {
                base.text = value;
            }
        }

        private string localizationText
        {
            set
            {
                if (Application.isPlaying)
                {
                    if (String.IsNullOrEmpty(value))
                    {
                        if (String.IsNullOrEmpty(m_Text))
                            return;
                        m_Text = "";
                        SetVerticesDirty();
                    }
                    else if (m_Text != value)
                    {
                        m_Text = value;
                        SetVerticesDirty();
                        SetLayoutDirty();
                    }
                }
            }
        }

        [SerializeField] 
        public float m_LetterSpacing = 0;

        public float letterSpacing
        {
            get { return m_LetterSpacing; }
            set
            {
                m_LetterSpacing = value;
                TextSpacing ts = GetComponent<TextSpacing>();
                if (m_LetterSpacing ==0 && ts==null)
                {
                    return;
                }
                if (ts == null)
                {
                    ts = this.gameObject.AddComponent<TextSpacing>();
                }

                ts.Spacing = m_LetterSpacing;

            }
        }

        [SerializeField]
        bool m_NumberMode = false;

        public bool numberMode
        {
            get { return m_NumberMode; }
            set { m_NumberMode = value; }
        }

        [SerializeField]
        string m_Format = "{0:#}";

        [SerializeField]
        float m_Duration = 0.5f;
        public float duration
        {
            get { return m_Duration; }
            set { m_Duration = value; }
        }

        [Serializable]
        public class TweenCompleteEvent : UnityEvent2 { }

        [SerializeField]
        TweenCompleteEvent m_OnTweenComplete = new TweenCompleteEvent();


        public TweenCompleteEvent onTweenComplete
        {
            get { return m_OnTweenComplete; }
            set { m_OnTweenComplete = value; }
        }


        float m_Number = 0;
        bool inited = false;

        public float number
        {
            get { return m_Number; }
            set { Set(value); }
        }

        protected virtual void Set(float value)
        {
            if (!inited)
            {
                inited = true;
                m_Number = value;
                text = string.Format(m_Format, value);
                return;
            }
            if (Mathf.Approximately(m_Number, value))
            {
                return;
            }
            if (numberMode)
            {
                var tween = new IndFloatTween();
                tween.startValue = m_Number;
                tween.targetValue = value;
                tween.duration = m_Duration;
                tween.AddOnChangedCallback((val, finish) =>
                {
                    text = string.Format(m_Format, val);
                    if (finish)
                    {
                        onTweenComplete?.Invoke();
                    }
                });
                m_TweenRunner.StartTween(tween);
            }else
            {
                text = string.Format(m_Format, value);
            }
            m_Number = value;
        }

        NDTweenRunner<IndFloatTween> m_TweenRunner;

        protected NDText()
        {
            if(m_TweenRunner == null)
                m_TweenRunner = new NDTweenRunner<IndFloatTween>();
            m_TweenRunner.Init(this);
            
        }

        [SerializeField]
        private NDTextStyle m_Style;
        
        [SerializeField]
        private NDTextColorStyle m_StyleColor;

        public TextStyleBase style
        {
            get { return m_Style; }
            set
            {
                if (m_Style == value) return;
                m_Style = value as NDTextStyle;
                UpdateStyle();
            }
        }

        public ColorStyleBase colorStyle 
        {
            get { return m_StyleColor; }
            set 
            {
                if (m_StyleColor == value)
                    return;
                m_StyleColor = value as NDTextColorStyle;
                UpdateColorStyle();
            }
        }

        protected override void Awake()
        {
            base.Awake();
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                bool isDebugMode = EditorPrefs.GetBool("EditorPrefs_LocalizationDebugMode", false);

                if (isDebugMode)
                {
                    rawText = text;
                    text = string.Empty;
                }
            }
#endif
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (Application.isPlaying)
            {
                UpdateLocalization();
#if UNITY_EDITOR
                isLocalizationUpdated = true;
#endif
            }
        }

        public void UpdateLocalization()
        {
            if (string.IsNullOrEmpty(m_LocalizationKey)) return;
            if (localizationProvider != null)
            {
                var t = localizationProvider.Invoke(m_LocalizationKey);
                localizationText = t;
            }

            if (m_Style != null)
            {
                m_Style.UpdateLocalizationFont();
            }
        }

        protected override void Start()
        {
            base.Start();
            UpdateStyle();
            UpdateColorStyle();
        }

        private void UpdateStyle()
        {
            if (m_Style)
            {
                m_Style.Apply(this);
            }
        }

        private void UpdateColorStyle() 
        {
            if (m_StyleColor)
            {
                m_StyleColor.Apply(this);
            }
        }


#if UNITY_EDITOR
        private static Type mAnimationWindowType => Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.AnimationWindow");
        
        private static bool AnimationWindowIsActive()
        {
            if (mAnimationWindowType == null)
            {
                return false;
            }
            
            return EditorWindow.mouseOverWindow && EditorWindow.mouseOverWindow.GetType() == mAnimationWindowType;
        }
        
        protected override void OnValidate()
        {
            if (!IsActive())
            {
                base.OnValidate();
                return;
            }
            
            UpdateStyle();
            
            if(!AnimationWindowIsActive()) {
                UpdateColorStyle();
            }

            base.OnValidate();
        }

#endif // if UNITY_EDITOR
        internal void AssignDefaultFont()
        {
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        internal NDTextStyle LoadDefautStyle() 
        {
            NDTextStyle t = Resources.Load<NDTextStyle>("FontStyle/DefaultFontStyle");
            return t;
        }

        internal NDTextColorStyle LoadDefautColorStyle()
        {
            NDTextColorStyle t = Resources.Load<NDTextColorStyle>("FontStyle/DefaultColorStyle");
            return t;
        }
    }
}
 