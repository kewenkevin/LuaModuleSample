using System;
using System.Collections.Generic;
using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;

namespace ND.UI
{
    /*
    public enum BinderTypeState : int
    {
        // UIExpansion
        UIExpansion,

        // GameObject
        GameObject,

        // Component
        RectTransform,
        Text,
        Image,
        Button,
        UMTList,
        InputField,
        Toggle,
    }*/

    public enum LinkerValueState : int
    {
        Vector2 = 0,
        Vector3 = 1,
        Quaternion = 2,
        Boolean = 3,
        Int32 = 4,
        String = 5,
        Single = 6,
        Color = 7,
        Sprite = 8,
        Char = 9,
        Rect = 10,
        SystemObject = 11,

        UnityEvent = 100,
        UnityEventBoolean = 101,
        UnityEventSingle = 102,
        UnityEventInt32 = 103,
        UnityEventString = 104,
        UnityEventVector2 = 105,

        UnityEvent2 = 130,
        UnityEvent2Boolean = 131,
        UnityEvent2Single = 132,
        UnityEvent2Int32 = 133,
        UnityEvent2String = 134,
        UnityEvent2Vector2 = 135,
        
        SystemEventActionIntAndObject = 201,
        SystemEventActionIntAndBool = 202,
        SystemEventActionObject = 203,
        DelegateInt = 301,
        
    }

    public enum EaseType : int
    {
        Linear = 0,
        SineIn = 1,
        SineOut= 2,
        SineInOut= 3,
        QuadIn= 4,
        QuadOut= 5,
        QuadInOut= 6,
        CubicIn= 7,
        CubicOut= 8,
        CubicInOut= 9,
        QuartIn= 10,
        QuartOut= 11,
        QuartInOut= 12,
        QuintIn= 13,
        QuintOut= 14,
        QuintInOut= 15,
        ExpoIn= 16,
        ExpoOut= 17,
        ExpoInOut= 18,
        CircIn= 19,
        CircOut= 20,
        CircInOut= 21,
        ElasticIn= 22,
        ElasticOut= 23,
        ElasticInOut= 24,
        BackIn= 25,
        BackOut= 26,
        BackInOut= 27,
        BounceIn= 28,
        BounceOut= 29,
        BounceInOut= 30,
        Custom= 31,
        None = 100,
    }

    public class UIExpansionUtility
    {
        public delegate GearBase GenerateGearFunction(Controller parent, GearConfig config);
        
        public const ushort TrueValue = 1;

        public const ushort FalseValue = 0;
        
        private static Dictionary<string, System.Type> m_RegisterBinderDic = null;
        private static Dictionary<int, System.Type> m_RegisterGearDic = null;
        private static RectTransform m_RootCanvas;

        public static Dictionary<int, System.Type> RegisterGearDic
        {
            get
            {
                if (m_RegisterGearDic == null)
                {
                    m_RegisterGearDic = new Dictionary<int, System.Type>()
                    {
                        {(int) GearTypeState.Controller, typeof(ControllerGear)},
                        {(int) GearTypeState.Active, typeof(ActiveGear)},
                        {(int) GearTypeState.OverallAlpha, typeof(OverallAlphaGear)},
                        {(int) GearTypeState.Position, typeof(PositionGear)},
                        {(int) GearTypeState.PercentPosition, typeof(PercentPositionGear)},
                        {(int) GearTypeState.SizeDelta, typeof(SizeDeltaGear)},
                        {(int) GearTypeState.Rotation, typeof(RotationGear)},
                        {(int) GearTypeState.Scale, typeof(ScaleGear)},
                        {(int) GearTypeState.TextStr, typeof(TextStrGear)},
                        {(int) GearTypeState.TextColor, typeof(TextColorGear)},
                        {(int) GearTypeState.ImageSprite, typeof(ImageSpriteGear)},
                        {(int) GearTypeState.ImageColor, typeof(ImageColorGear)},
                        {(int) GearTypeState.ImageMaterial, typeof(ImageMaterialGear)},
                        {(int) GearTypeState.RawImageColor, typeof(RawImageColorGear)},
                        {(int) GearTypeState.TextFontStyle, typeof(TextFontStyleGear)},
                        {(int) GearTypeState.TextColorStyle, typeof(TextColorStyleGear)},
                        {(int) GearTypeState.LocalizationKey, typeof(LocalizationKeyGear)},
                        {(int) GearTypeState.RatingCurrent, typeof(RatingCurrentGear)},
                        {(int) GearTypeState.RatingTotal, typeof(RatingTotalGear)},
                    };
                    System.Type type = Type.GetType("UIGear,Assembly-CSharp");
                    if (type != null)
                    {
                        var registerFunc = type.GetMethod("Register");
                        if (registerFunc != null)
                        {
                            registerFunc.Invoke(null, null);
                        }
                    }
                }
                return m_RegisterGearDic;
            }
        }

        public static Dictionary<string, System.Type> RegisterBinderDic
        {
            get
            {
                if (m_RegisterBinderDic == null)
                {
                    m_RegisterBinderDic = new Dictionary<string, System.Type>()
                    {
                        {"GameObject", typeof(GameObjectBinder)},
                        {"UIExpansion", typeof(UIExpansionBinder)},
                    };
                    System.Type type = Type.GetType("UIBinder,Assembly-CSharp");
                    if (type!=null)
                    {
                        var registerFunc = type.GetMethod("Register");
                        if (registerFunc!=null)
                        {
                            registerFunc.Invoke(null, null);   
                        }
                    }
                }
                return m_RegisterBinderDic;
            }
        }

        // public static Dictionary<string, System.Type> RegisterBinderDic = new Dictionary<string, System.Type>()
        // {
        //     { "GameObject",             typeof(GameObjectBinder) },
        //     // { "Image",                  typeof(ImageBinder) },
        //     // { "RectTransform",          typeof(RectTransformBinder) },
        //     // { "Text",                   typeof(TextBinder) },
        //     // { "Button",                 typeof(ButtonBinder) },
        //     // { "UMTList",                typeof(ListBinder) },
        //     // { "InputField",             typeof(InputFieldBinder) },
        //     // { "Toggle",                 typeof(ToggleBinder) },
        //     { "UIExpansion",            typeof(UIExpansionBinder) },
        // };

        private static List<string> _registerBinderKeyList = null;

        public static List<string> RegisterBinderKeyList
        {
            get
            {
                if(_registerBinderKeyList == null)
                {
                    _registerBinderKeyList = new List<string>();
                    foreach (string key in RegisterBinderDic.Keys)
                    {
                        _registerBinderKeyList.Add(key);
                    }
                }
                return _registerBinderKeyList;
            }
        }

        /// <summary>
        /// 获取当前场景canvas的大小
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void GetCanvasSize(out float width, out float height)
        {
            if (m_RootCanvas == null)
            {
                var uiRoot = UnityEngine.GameObject.Find("UIRoot");
                if (uiRoot != null)
                {
                    //遍历UIRoot下的Canvas
                    Canvas canvas = uiRoot.GetComponentInChildren(typeof(Canvas),true) as Canvas;

                    if (canvas != null)
                    {
                        m_RootCanvas = canvas.GetComponent<RectTransform>();
                    }
                }
            }
          
            
            if (m_RootCanvas != null)
            {
                var rect = m_RootCanvas.rect;
                width = rect.width;
                height = rect.height;
            }
            else
            {
                Debug.LogWarning("当前场景Canvas未找到！");
                width = 1920;
                height = 1080;
            }
        }

        public static GearBase GetGear(Controller parent, GearConfig config)
        {
            System.Type gearType = null;
            if (RegisterGearDic.TryGetValue((int)config.gearType, out gearType))
            {
                return (GearBase)System.Activator.CreateInstance(gearType, new object[] { parent, config });
            }
            else
            {
                UnityEngine.Debug.LogError("RegisterBinder Failue Dont has this binderName: "+(int)config.gearType);
                return null;
            }
        }

        public static void RegisterBinder(System.Type binderType)
        {
            RegisterBinderDic[binderType.Name.Replace("Binder","")] = binderType;
            _registerBinderKeyList = null;
        }
        
        public static void RegisterGear(int gearType,System.Type binderType)
        {
            RegisterGearDic[gearType] = binderType;
        }


        public static BinderBase GetLinker(UIExpansion owner, LinkerConfig config)
        {
            string binderName = owner.StoredStrings[config.BinderTypeIndex];
            if (string.IsNullOrEmpty(binderName))
            {
                UnityEngine.Debug.LogError("GetLinker dont has bunderName:"+binderName);
                return null;
            }
            System.Type binderType = null;
            if (RegisterBinderDic.TryGetValue(binderName, out binderType))
            {
                return (BinderBase)System.Activator.CreateInstance(binderType, new object[] { owner, config });
            }
            else
            {
                UnityEngine.Debug.LogError("RegisterBinder Failue Dont has this binderName: "+binderName);
            }
            return null;
        }
        
        
        public static Type GetLinkerType(UIExpansion owner, LinkerConfig config)
        {
            string binderName = owner.StoredStrings[config.BinderTypeIndex];
            if (string.IsNullOrEmpty(binderName))
            {
                UnityEngine.Debug.LogError("GetLinker dont has bunderName:"+binderName);
                return null;
            }
            System.Type binderType = null;
            if (RegisterBinderDic.TryGetValue(binderName, out binderType))
            {
                return binderType;
            }
            else
            {
                UnityEngine.Debug.LogError("RegisterBinder Failue Dont has this binderName: "+binderName);
            }
            return null;
        }
        
        /*
        public static BinderBase GetLinker(UIExpansion owner, LinkerConfig config)
        {
            switch ((BinderTypeState)owner.StoredInts[config.BinderTypeIndex])
            {
                case BinderTypeState.GameObject:
                    return new GameObjectBinder(owner, config);
                case BinderTypeState.Image:
                    return new ImageBinder(owner, config);
                case BinderTypeState.RectTransform:
                    return new RectTransformBinder(owner, config);
                case BinderTypeState.Text:
                    return new TextBinder(owner, config);
                case BinderTypeState.Button:
                    return new ButtonBinder(owner, config);
                case BinderTypeState.UMTList:
                    return new ListBinder(owner, config);
                case BinderTypeState.InputField:
                    return new InputFieldBinder(owner, config);
                case BinderTypeState.Toggle:
                    return new ToggleBinder(owner, config);
                default:
                    return null;
            }
        }
        */

        public static KeyFrameBase GetKeyFrame(Transition parent, KeyFrameConfig config)
        {
            switch (config.lineType)
            {
                case LineTypeState.Position:
                    return new PositionKeyFrame(parent, config);
                case LineTypeState.Active:
                    return new ActiveKeyFrame(parent, config);
                case LineTypeState.Event:
                    return new EventKeyFrame(parent, config);
                case LineTypeState.ImageColor:
                    return new ImageColorKeyFrame(parent, config);
                case LineTypeState.ImageSprite:
                    return new ImageSpriteKeyFrame(parent, config);
                case LineTypeState.Rotation:
                    return new RotationKeyFrame(parent, config);
                case LineTypeState.Scale:
                    return new ScaleKeyFrame(parent, config);
                case LineTypeState.SizeDelta:
                    return new SizeDeltaKeyFrame(parent, config);
                case LineTypeState.TextColor:
                    return new TextColorKeyFrame(parent, config);
                default:
                    return null;
            }
        }
    }
}