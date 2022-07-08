using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


namespace ND.UI
{
    public enum UIExpansionGameObjectType : int
    {
        None,
        PrefabAsset,
        PrefabInstance,
        NormalGameObject,
    }

    public class UIExpansionEditorUtility 
    {
        /// <summary>
        /// 编辑器最小宽度
        /// </summary>
        public const float WINDOW_MIN_WIDTH = 400f;

        /// <summary>
        /// 编辑器最小高度
        /// </summary>
        public const float WINDOW_MIN_HEIGHT = 240f;

        /// <summary>
        /// 编辑器GUI刷新时间
        /// </summary>
        public const double WINDOW_REPAINT_TIME = 1.0 / 30.0;

        /// <summary>
        /// 标准行高
        /// </summary>
        public static float SINGLELINE_HEIGHT = 20;

        /// <summary>
        /// 标准行高间隔
        /// </summary>
        public static float SINGLELINE_SPACING = SINGLELINE_HEIGHT + 1;

        /// <summary>
        /// 分割栏宽度
        /// </summary>
        public const float BORDER_CONTROL_WIDTH = 2f;

        /// <summary>
        /// 拖动条的宽度
        /// </summary>
        public const float SLIDER_BAR_WIDTH = 16f;

        /// <summary>
        /// 控制器单页宽度
        /// </summary>
        public const float CONTROLLER_PAGE_WIDTH = 220;

        /// <summary>
        /// 
        /// </summary>
        public const float TRANSITION_TIMELINE_MARGIN_LEFT = 20f;

        /// <summary>
        /// 
        /// </summary>
        public const float TRANSITION_TIMELINE_MARGIN_RIGHT = 40f;

        /// <summary>
        /// 编辑器资源根目录
        /// </summary>
        //public const string EDITOR_ASSETS_PATH = "Packages/com.Yoozoo.ui/Editor Resources/Image/";
        public const string EDITOR_ASSETS_PATH = "Packages/com.nfu.ui.expansion/Editor Resources/Image/";

        
        private static string s_GeneratePath = Application.dataPath + "/UIBinderGenerate";
        

        public static string[] IgnoreExportNameArray =
        {
            "useGUILayout",         // MonoBehaviour
            "runInEditMode",        // MonoBehaviour
            "tag",                  // Component
            "name",                 // Object
        };

        public static Dictionary<System.Type, int> TypeValueDic = new Dictionary<System.Type, int>()
        {
            {typeof(UnityEngine.Vector2),               (int)LinkerValueState.Vector2 },
            {typeof(UnityEngine.Vector3),               (int)LinkerValueState.Vector3 },
            {typeof(UnityEngine.Quaternion),            (int)LinkerValueState.Quaternion },
            {typeof(System.Boolean),                    (int)LinkerValueState.Boolean },
            {typeof(System.Int32),                      (int)LinkerValueState.Int32 },
            {typeof(System.String),                     (int)LinkerValueState.String },
            {typeof(System.Single),                     (int)LinkerValueState.Single },
            {typeof(UnityEngine.Color),                 (int)LinkerValueState.Color },
            {typeof(UnityEngine.Sprite),                (int)LinkerValueState.Sprite },
            {typeof(System.Char),                       (int)LinkerValueState.Char },
            {typeof(UnityEngine.Rect),                  (int)LinkerValueState.Rect },
            {typeof(System.Object),                     (int)LinkerValueState.SystemObject },

            {typeof(UnityEngine.Events.UnityEvent),                    (int)LinkerValueState.UnityEvent },
            {typeof(UnityEngine.Events.UnityEvent<System.Boolean>),    (int)LinkerValueState.UnityEventBoolean },
            {typeof(UnityEngine.Events.UnityEvent<System.Single>),     (int)LinkerValueState.UnityEventSingle },
            {typeof(UnityEngine.Events.UnityEvent<System.Int32>),      (int)LinkerValueState.UnityEventInt32 },
            {typeof(UnityEngine.Events.UnityEvent<System.String>),     (int)LinkerValueState.UnityEventString },
            {typeof(UnityEngine.Events.UnityEvent<Vector2>),           (int)LinkerValueState.UnityEventVector2 },
            {typeof(ND.UI.NDEvents.UnityEvent2),                    (int)LinkerValueState.UnityEvent2 },
            {typeof(ND.UI.NDEvents.UnityEvent2<System.Boolean>),     (int)LinkerValueState.UnityEvent2Boolean },
            {typeof(ND.UI.NDEvents.UnityEvent2<System.Single>),     (int)LinkerValueState.UnityEvent2Single },
            {typeof(ND.UI.NDEvents.UnityEvent2<System.Int32>),      (int)LinkerValueState.UnityEvent2Int32 },
            {typeof(ND.UI.NDEvents.UnityEvent2<System.String>),     (int)LinkerValueState.UnityEvent2String },
            {typeof(ND.UI.NDEvents.UnityEvent2<Vector2>),           (int)LinkerValueState.UnityEvent2Vector2 },
        };
        
        public static Dictionary<int, System.Type> ValueTypeDic = new Dictionary<int, System.Type>()
        {
            {(int)LinkerValueState.Vector2,typeof(UnityEngine.Vector2)},
            {(int)LinkerValueState.Vector3,typeof(UnityEngine.Vector3)},
            {(int)LinkerValueState.Quaternion,typeof(UnityEngine.Quaternion)},
            {(int)LinkerValueState.Boolean,typeof(System.Boolean)},
            {(int)LinkerValueState.Int32,typeof(System.Int32)},
            {(int)LinkerValueState.String,typeof(System.String)},
            {(int)LinkerValueState.Single,typeof(System.Single)},
            {(int)LinkerValueState.Color,typeof(UnityEngine.Color)},
            {(int)LinkerValueState.Sprite,typeof(UnityEngine.Sprite)},
            {(int)LinkerValueState.Char,typeof(System.Char)},
            {(int)LinkerValueState.Rect,typeof(UnityEngine.Rect)},
            {(int)LinkerValueState.SystemObject,typeof(System.Object)},
            {(int)LinkerValueState.UnityEvent,typeof(UnityEngine.Events.UnityEvent)},
            {(int)LinkerValueState.UnityEventBoolean,typeof(UnityEngine.Events.UnityEvent<System.Boolean>)},
            {(int)LinkerValueState.UnityEventSingle,typeof(UnityEngine.Events.UnityEvent<System.Single>)},
            {(int)LinkerValueState.UnityEventInt32,typeof(UnityEngine.Events.UnityEvent<System.Int32>)},
            {(int)LinkerValueState.UnityEventString,typeof(UnityEngine.Events.UnityEvent<System.String>)},
            {(int)LinkerValueState.UnityEventVector2,typeof(UnityEngine.Events.UnityEvent<Vector2>)},
            {(int)LinkerValueState.UnityEvent2,typeof(ND.UI.NDEvents.UnityEvent2)},
            {(int)LinkerValueState.UnityEvent2Boolean,typeof(ND.UI.NDEvents.UnityEvent2<System.Boolean>)},
            {(int)LinkerValueState.UnityEvent2Single,typeof(ND.UI.NDEvents.UnityEvent2<System.Single>)},
            {(int)LinkerValueState.UnityEvent2Int32,typeof(ND.UI.NDEvents.UnityEvent2<System.Int32>)},
            {(int)LinkerValueState.UnityEvent2String,typeof(ND.UI.NDEvents.UnityEvent2<System.String>)},
            {(int)LinkerValueState.UnityEvent2Vector2,typeof(ND.UI.NDEvents.UnityEvent2<Vector2>)},
        };

        public static Dictionary<int, List<string>> TypeFuncHeaderDic = new Dictionary<int, List<string>>()
        {
            {
                (int)LinkerValueState.Vector2,
                new List<string>
                {
                    "public override void SetVector2(Vector2 value)",
                    "_target.{0} = value;",
                    "base.SetVector2(value);"
                }
            },

            {
                (int)LinkerValueState.Vector3,
                new List<string>
                {
                    "public override void SetVector3(Vector3 value)",
                    "_target.{0} = value;",
                    "base.SetVector3(value);"
                }
            },

            {
                (int)LinkerValueState.Quaternion,
                new List<string>
                {
                    "public override void SetQuaternion(Quaternion value)",
                    "_target.{0} = value;",
                    "base.SetQuaternion(value);"
                }
            },

            {
                (int)LinkerValueState.Boolean,
                new List<string>
                {
                    "public override void SetBoolean(bool value)",
                    "_target.{0} = value;",
                    "base.SetBoolean(value);"
                }
            },

            {
                (int)LinkerValueState.Int32,
                new List<string>
                {
                    "public override void SetInt32(int value)",
                    "_target.{0} = value;",
                    "base.SetInt32(value);"
                }
            },

            {
                (int)LinkerValueState.String,
                new List<string>
                {
                    "public override void SetString(string value)",
                    "_target.{0} = value;",
                    "base.SetString(value);"
                }
            },

            {
                (int)LinkerValueState.Single,
                new List<string>
                {
                    "public override void SetSingle(float value)",
                    "_target.{0} = value;",
                    "base.SetSingle(value);"
                }
            },

            {
                (int)LinkerValueState.Color,
                new List<string>
                {
                    "public override void SetColor(Color value)",
                    "_target.{0} = value;",
                    "base.SetColor(value);"
                }
            },

            {
                (int)LinkerValueState.Sprite,
                new List<string>
                {
                    "public override void SetSprite(Sprite value)",
                    "_target.{0} = value;",
                    "base.SetSprite(value);"
                }
            },

            {
                (int)LinkerValueState.Char,
                new List<string>
                {
                    "public override void SetChar(char value)",
                    "_target.{0} = value;",
                    "base.SetChar(value);"
                }
            },

            {
                (int)LinkerValueState.Rect,
                new List<string>
                {
                    "public override void SetRect(Rect value)",
                    "_target.{0} = value;",
                    "base.SetRect(value);"
                }
            },

            {
                (int)LinkerValueState.SystemObject,
                new List<string>
                {
                    "public override void SetSystemObject(System.Object value)",
                    "_target.{0} = value;",
                    "base.SetSystemObject(value);"
                }
            },

            {
                (int)LinkerValueState.UnityEvent,
                new List<string>
                {
                    // add
                    "public override void SetAction(UnityAction action)",
                    "_target.{0}.AddListener(action);",
                    "base.SetAction(action);",
                    //remove
                    "public override void RemoveAction(UnityAction action)",
                    "_target.{0}.RemoveListener(action);",
                    "base.RemoveAction(action);",
                    //remove All
                    "public override void RemoveAllAction()",
                    "_target.{0}.RemoveAllListeners();",
                    "base.RemoveAllAction();",
                }
            },

            {
                (int)LinkerValueState.UnityEventBoolean,
                new List<string>
                {
                    "public override void SetActionBoolean(UnityAction<bool> action)",
                    "_target.{0}.AddListener(action);",
                    "base.SetActionBoolean(action);",

                    //remove
                    "public override void RemoveActionBoolean(UnityAction<bool> action)",
                    "_target.{0}.RemoveListener(action);",
                    "base.RemoveActionBoolean(action);",

                    //remove All
                    "public override void RemoveAllActionBoolean()",
                    "_target.{0}.RemoveAllListeners();",
                    "base.RemoveAllActionBoolean();",
                }
            },

            {
                (int)LinkerValueState.UnityEventSingle,
                new List<string>
                {
                    "public override void SetActionSingle(UnityAction<float> action)",
                    "_target.{0}.AddListener(action);",
                    "base.SetActionSingle(action);",

                    //remove
                    "public override void RemoveActionSingle(UnityAction<float> action)",
                    "_target.{0}.RemoveListener(action);",
                    "base.RemoveActionSingle(action);",

                    //remove All
                    "public override void RemoveAllActionSingle()",
                    "_target.{0}.RemoveAllListeners();",
                    "base.RemoveAllActionBoolean();",
                }
            },

            {
                (int)LinkerValueState.UnityEventInt32,
                new List<string>
                {
                    "public override void SetActionInt32(UnityAction<int> action)",
                    "_target.{0}.AddListener(action);",
                    "base.SetActionInt32(action);",
                    //remove
                    "public override void RemoveActionInt32(UnityAction<int> action)",
                    "_target.{0}.RemoveListener(action);",
                    "base.RemoveActionInt32(action);",

                    //remove All
                    "public override void RemoveAllActionInt32()",
                    "_target.{0}.RemoveAllListeners();",
                    "base.RemoveAllActionInt32();",
                }
            },

            {
                (int)LinkerValueState.UnityEventString,
                new List<string>
                {
                    "public override void SetActionString(UnityAction<string> action)",
                    "_target.{0}.AddListener(action);",
                    "base.SetActionString(action);",
                    //remove
                    "public override void RemoveActionString(UnityAction<string> action)",
                    "_target.{0}.RemoveListener(action);",
                    "base.RemoveActionString(action);",
                    
                    //remove All
                    "public override void RemoveAllActionString()",
                    "_target.{0}.RemoveAllListeners();",
                    "base.RemoveAllActionString();",
                }
            },

            {
                (int)LinkerValueState.UnityEventVector2,
                new List<string>
                {
                    "public override void SetActionVector2(UnityAction<Vector2> action)",
                    "_target.{0}.AddListener(action);",
                    "base.SetActionVector2(action);",
                    //remove
                    "public override void RemoveActionVector2(UnityAction<Vector2> action)",
                    "_target.{0}.RemoveListener(action);",
                    "base.RemoveActionVector2(action);",

                    //remove All
                    "public override void RemoveAllActionVector2()",
                    "_target.{0}.RemoveAllListeners();",
                    "base.RemoveAllActionVector2();",
                }
            },

            {
                (int)LinkerValueState.UnityEvent2,
                new List<string>
                {
                    "public override void SetAction2(UnityAction action)",
                    "_target.{0}.AddListener(action);",
                    "base.SetAction2(action);",
                    //remove
                    "public override void RemoveAction2(UnityAction action)",
                    "_target.{0}.RemoveListener(action);",
                    "base.RemoveAction2(action);",

                         //remove All
                    "public override void RemoveAllAction2()",
                    "_target.{0}.RemoveAllListeners();",
                    "base.RemoveAllAction2();",
                }
            },
            {
                (int)LinkerValueState.UnityEvent2Boolean,
                new List<string>
                {
                    "public override void SetAction2Boolean(UnityAction<bool> action)",
                    "_target.{0}.AddListener(action);",
                    "base.SetAction2Boolean(action);",

                    //remove
                    "public override void RemoveAction2Boolean(UnityAction<bool> action)",
                    "_target.{0}.RemoveListener(action);",
                    "base.RemoveAction2Boolean(action);",

                    //remove All
                    "public override void RemoveAllActionBoolean()",
                    "_target.{0}.RemoveAllListeners();",
                    "base.RemoveAllActionBoolean();",
                }
            },
            {
                (int)LinkerValueState.UnityEvent2Single,
                new List<string>
                {
                    "public override void SetAction2Single(UnityAction<float> action)",
                    "_target.{0}.AddListener(action);",
                    "base.SetAction2Single(action);",

                    //remove
                    "public override void RemoveAction2Single(UnityAction<float> action)",
                    "_target.{0}.RemoveListener(action);",
                    "base.RemoveAction2Single(action);",

                    //remove All
                    "public override void RemoveAllActionSingle()",
                    "_target.{0}.RemoveAllListeners();",
                    "base.RemoveAllActionBoolean();",
                }
            },

            {
                (int)LinkerValueState.UnityEvent2Int32,
                new List<string>
                {
                    "public override void SetAction2Int32(UnityAction<int> action)",
                    "_target.{0}.AddListener(action);",
                    "base.SetAction2Int32(action);",
                    //remove
                    "public override void RemoveAction2Int32(UnityAction<int> action)",
                    "_target.{0}.RemoveListener(action);",
                    "base.RemoveAction2Int32(action);",

                    //remove All
                    "public override void RemoveAllActionInt32()",
                    "_target.{0}.RemoveAllListeners();",
                    "base.RemoveAllActionInt32();",
                }
            },

            {
                (int)LinkerValueState.UnityEvent2String,
                new List<string>
                {
                    "public override void SetAction2String(UnityAction<string> action)",
                    "_target.{0}.AddListener(action);",
                    "base.SetAction2String(action);",
                    //remove
                    "public override void RemoveAction2String(UnityAction<string> action)",
                    "_target.{0}.RemoveListener(action);",
                    "base.RemoveAction2String(action);",
                    
                    //remove All
                    "public override void RemoveAllActionString()",
                    "_target.{0}.RemoveAllListeners();",
                    "base.RemoveAllActionString();",
                }
            },

            {
                (int)LinkerValueState.UnityEvent2Vector2,
                new List<string>
                {
                    "public override void SetAction2Vector2(UnityAction<Vector2> action)",
                    "_target.{0}.AddListener(action);",
                    "base.SetAction2Vector2(action);",
                    //remove
                    "public override void RemoveActionVector2(UnityAction<Vector2> action)",
                    "_target.{0}.RemoveListener(action);",
                    "base.RemoveAction2Vector2(action);",

                    //remove All
                    "public override void RemoveAllActionVector2()",
                    "_target.{0}.RemoveAllListeners();",
                    "base.RemoveAllActionVector2();",
                }
            },
        };


        public static void Init()
        {
            LoadEditorTexture();
        }

    

        private static void LoadEditorTexture()
        {
            AddIconTex = EditorGUIUtility.Load(EDITOR_ASSETS_PATH + "AddIcon.png") as Texture;
            EditIconTex = EditorGUIUtility.Load(EDITOR_ASSETS_PATH + "EditIcon.png") as Texture;
            MinusIconTex = EditorGUIUtility.Load(EDITOR_ASSETS_PATH + "MinusIcon.png") as Texture;
            ArrowTexture = EditorGUIUtility.Load(EDITOR_ASSETS_PATH + "Playhead.png") as Texture;
            PlayButtonTexture = EditorGUIUtility.Load(EDITOR_ASSETS_PATH + "PlayIcon.png") as Texture;
            PauseButtonTexture = EditorGUIUtility.Load(EDITOR_ASSETS_PATH + "PauseIcon.png") as Texture;
            StopButtonTexture = EditorGUIUtility.Load(EDITOR_ASSETS_PATH + "StopIcon.png") as Texture;
            FrameBackTexture = EditorGUIUtility.Load(EDITOR_ASSETS_PATH + "FrameBackwardIcon.png") as Texture;
            FrameForwardTexture = EditorGUIUtility.Load(EDITOR_ASSETS_PATH + "FrameForwardIcon.png") as Texture;
            RenameTexture = EditorGUIUtility.Load(EDITOR_ASSETS_PATH + "EditIcon.png") as Texture;
            if (EditorGUIUtility.isProSkin)
            {
                CutlineTex = EditorGUIUtility.Load(EDITOR_ASSETS_PATH + "Cutline_Pro.png") as Texture;
                TreeItemBG1 = EditorGUIUtility.Load(EDITOR_ASSETS_PATH + "TreeItemBG1_Pro.png") as Texture;
                TreeItemBG2 = EditorGUIUtility.Load(EDITOR_ASSETS_PATH + "TreeItemBG2_Pro.png") as Texture;
                TreeItemSelectedBG = EditorGUIUtility.Load(EDITOR_ASSETS_PATH + "TreeItemBG_Selected_Pro.png") as Texture;
            }
            else
            {
                CutlineTex = EditorGUIUtility.Load(EDITOR_ASSETS_PATH + "Cutline_Personal.png") as Texture;
                TreeItemBG1 = EditorGUIUtility.Load(EDITOR_ASSETS_PATH + "TreeItemBG1_Personal.png") as Texture;
                TreeItemBG2 = EditorGUIUtility.Load(EDITOR_ASSETS_PATH + "TreeItemBG2_Personal.png") as Texture;
                TreeItemSelectedBG = EditorGUIUtility.Load(EDITOR_ASSETS_PATH + "TreeItemBG_Selected_Personal.png") as Texture;
            }
        }


        #region Editor Texture

        public static Texture CutlineTex;

        public static Texture AddIconTex;

        public static Texture MinusIconTex;

        public static Texture EditIconTex;

        public static Texture TreeItemBG1;

        public static Texture TreeItemBG2;

        public static Texture TreeItemSelectedBG;

        public static Texture ArrowTexture;

        public static Texture PlayButtonTexture;

        public static Texture PauseButtonTexture;

        public static Texture StopButtonTexture;

        public static Texture FrameBackTexture;

        public static Texture FrameForwardTexture;

        public static Texture AddItemTexture;

        public static Texture RenameTexture;

        #endregion
    }
}