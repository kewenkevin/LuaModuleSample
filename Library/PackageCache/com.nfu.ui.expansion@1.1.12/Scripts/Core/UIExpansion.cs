using System;
using System.Collections.Generic;
using ND.UI.Core;
using ND.UI.Core.Model;
using UnityEngine;

namespace ND.UI
{

    public class UIExpansion : MonoBehaviour , IUIExpansion
    {
        private bool _ready;

        private Controller[] _controllers;

        private Binding _bindingInfo;

        private Transition[] _transitions;

        private Animation _animation;

        [SerializeField] private bool isPureMode;
        
        [SerializeField]
        private UnityEngine.Object[] _bindedObjects;
        
        [SerializeField]
        private GameObject[] _storedGameObjects;

        [SerializeField]
        private Sprite[] _storedSprites;

        [SerializeField]
        private Material[] _storedMaterials;
        
        [SerializeField]
        private TextStyleBase[] _storedTextFontStyles;
        
        [SerializeField]
        private ColorStyleBase[] _storedTextColorStyles;

        [SerializeField]
        private float[] _storedFloats;

        [SerializeField]
        private int[] _storedInts;

        [SerializeField]
        private string[] _storedStrings;
        
        [SerializeField]
        private AnimationCurve[] _storedAnimationCurves;

        [SerializeField]
        private ControllerConfig[] _controllerConfigs;

        [SerializeField]
        private TransitionConfig[] _transitionConfigs;

        [SerializeField]
        private BindingConfig _bindingConfig;
        
        [SerializeField]
        private string _luaBindPath;

        private Tweener _animationEventtweener;
        
        public UnityEngine.Object[] BindedObjects
        {
            get
            {
                return _bindedObjects;
            }

            set
            {
                _bindedObjects = value;
            }
        }

        private Dictionary<string, bool> m_bindObjectToggleGroup = new Dictionary<string, bool>();
        public Dictionary<string,bool> BindObjectToggleGroup
        {
            get => m_bindObjectToggleGroup;

            set => m_bindObjectToggleGroup = value;
        }
        


        public UnityEngine.Object GetBindObject(int index)
        {
            return BindedObjects[index];
        }

        public string LuaBindPath
        {
            get
            {
                return _luaBindPath;
            }

            set
            {
                _luaBindPath = value;
            }
        }
        
        public GameObject[] StoredGameObjects
        {
            get
            {
                return _storedGameObjects;
            }

            set
            {
                _storedGameObjects = value;
            }
        }

        public Sprite[] StoredSprites
        {
            get
            {
                return _storedSprites;
            }

            set
            {
                _storedSprites = value;
            }
        }

        public Material[] StoredMaterials
        {
            get
            {
                return _storedMaterials;
            }
            set
            {
                _storedMaterials = value;
            }
        }
        
        public TextStyleBase[] StoredTextFontStyles
        {
            get
            {
                return _storedTextFontStyles;
            }
            set
            {
                _storedTextFontStyles = value;
            }
        }
        
        public ColorStyleBase[] StoredTextColorStyles
        {
            get
            {
                return _storedTextColorStyles;
            }
            set
            {
                _storedTextColorStyles = value;
            }
        }

        public float[] StoredFloats
        {
            get
            {
                return _storedFloats;
            }

            set
            {
                _storedFloats = value;
            }
        }

        public int[] StoredInts
        {
            get
            {
                return _storedInts;
            }

            set
            {
                _storedInts = value;
            }
        }

        public string[] StoredStrings
        {
            get
            {
                return _storedStrings;
            }

            set
            {
                _storedStrings = value;
            }
        }
        
        public AnimationCurve[] StoredAnimationCurves
        {
            get
            {
                return _storedAnimationCurves;
            }

            set
            {
                _storedAnimationCurves = value;
            }
        }

        public ControllerConfig[] ControllerConfigs
        {
            get
            {
                return _controllerConfigs;
            }

            set
            {
                _controllerConfigs = value;
            }
        }

        public TransitionConfig[] TransitionConfigs
        {
            get
            {
                return _transitionConfigs;
            }

            set
            {
                _transitionConfigs = value;
            }
        }

        public BindingConfig BindingConfig
        {
            get
            {
                return _bindingConfig;
            }

            set
            {
                _bindingConfig = value;
            }
        }

        public bool Ready
        {
            get
            {
                return _ready;
            }
        }
        
        public bool IsPureMode
        {
            get
            {
                return isPureMode;
            }
            set
            {
                isPureMode = value;
            }
        }

        public Controller[] Controllers
        {
            get
            {
                return _controllers;
            }
        }

        public Binding BindingInfo 
        { 
            get
            {
                Init();
                return _bindingInfo;
            }
        }

        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            if (_transitions != null)
            {
                foreach(var transition in _transitions)
                {
                    transition.CheckAutoPlay();
                }
            }
        
        }

        public void Init()
        {
            if (_ready)
            {
                return;
            }

            if (_animation==null)
            {
                _animation = GetComponent<Animation>();
            }
            if (_controllerConfigs != null && _controllerConfigs.Length > 0)
            {
                _controllers = new Controller[_controllerConfigs.Length];
                for (int i = 0; i < _controllerConfigs.Length; i++)
                {
                    _controllers[i] = new Controller(this, _controllerConfigs[i]);
                }
            }
            if (_transitionConfigs != null && _transitionConfigs.Length > 0)
            {
                _transitions = new Transition[_transitionConfigs.Length];
                for(int i = 0; i< _transitionConfigs.Length; i++)
                {
                    _transitions[i] = new Transition(this, _transitionConfigs[i]);
                }
            }
            if (_bindingConfig != null)
            {
                _bindingInfo = new Binding(this,_bindingConfig);
            }
            _ready = true;
        }

        public void ClearStoredDatas()
        {
            _storedGameObjects = null;
            _storedSprites = null;
            _storedFloats = null;
            _storedInts = null;
            _storedStrings = null;
            _storedAnimationCurves = null;

            _controllerConfigs = null;
            _transitionConfigs = null;
            _bindingConfig = null;
        }

        public GameObject GetStoredGameObject(ushort index)
        {
            if (_storedGameObjects == null || index >= _storedGameObjects.Length)
            {
                return null;
            }
            return _storedGameObjects[index];
        }
        
        public IController GetController(int index)
        {
            if (_controllers == null || index < 0 || index >= _controllers.Length)
            {
                return null;
            }
            return _controllers[index];
        }

        public IController GetController(string name)
        {
            if (_controllers == null || _controllers.Length == 0)
            {
                return null; 
            }
            foreach (var controller in _controllers)
            {
                if (controller.Name == name)
                {
                    return controller;
                }
            }
            return null;
        }

        public void EditorChangeControllerSelectedIndex(string controllerName, int index)
        {
            if (_controllerConfigs != null && _controllerConfigs.Length > 0)
            {
                for (int i = 0; i < _controllerConfigs.Length; i++)
                {
                    if (_controllerConfigs[i].name == controllerName)
                    {
                        Controller controller = new Controller(this, _controllerConfigs[i]);
                        controller.EditorApply(index);
                        break;
                    }
                }
            }
        }

        public ControllerConfig EditorGetControllerConfig(string name)
        {
            if (_controllerConfigs == null || _controllerConfigs.Length == 0)
            {
                return null;
            }
            foreach (var controllerConfig in _controllerConfigs)
            {
                if (controllerConfig.name == name)
                {
                    return controllerConfig;
                }
            }
            return null;
        }

        public ITransition GetTransition(int index)
        {
            if (_transitions == null || index < 0 || index >= _transitions.Length)
            {
                return null;
            }
            return _transitions[index];
        }

        public ITransition GetTransition(string name)
        {
            if(_transitions == null || _transitions.Length == 0)
            {
                UnityEngine.Debug.LogWarning("dont has this GetTransiton: "+name);
                return null;
            }
            foreach (var transiton in _transitions)
            {
                if (transiton.Name == name)
                {
                    return transiton;
                }
            }
            return null;
        }

        public void PlayAnimation(string animName, UnityEngine.Events.UnityAction onComplete = null)
        {
            if (_animation==null)
            {
                onComplete?.Invoke();
                UnityEngine.Debug.LogWarning("dont has animation:"+animName);
                return;
            }

            if (_animation[animName] != null)
            {
                bool bPlay = _animation.Play(animName);
                if (bPlay)
                {
                    if (onComplete != null)
                    {
                        _animationEventtweener?.Kill();
                        float delay = _animation.GetClip(animName).length;
                        _animationEventtweener = TweenManager.Instance.CreateTweener().SetDelay(delay)
                            .OnComplete(() => { onComplete(); });
                    }
                }
                else
                {
                    onComplete?.Invoke();
                }
            }
            else
            {
                Debug.LogWarning(string.Format("<color=#ffff00>播放动画[{0}]失败: 未能找到对应的Animation</color>", animName));
                onComplete?.Invoke();
            }
        }
        public void StopAnimation(string animName)
        {
            if (_animation==null)
            {
                UnityEngine.Debug.LogWarning("dont has animation:"+animName);
                return;
            }
            _animation.Stop(animName);
            _animationEventtweener?.Kill();
        }

        /// <summary>
        /// Use to play all transitions which are contained by UIExpansion
        /// </summary>
        public void PlayAllTransitions()
        {
            if (_transitionConfigs==null || _transitionConfigs.Length<=0)
            {
                return;
            }

            if (_transitions == null || _transitions.Length <= 0 || _transitions.Length != _transitionConfigs.Length)
            {
                _transitions = null;
                _transitions = new Transition[_transitionConfigs.Length];
                for(int i = 0; i< _transitionConfigs.Length; i++)
                {
                    _transitions[i] = new Transition(this, _transitionConfigs[i]);
                }
            }
            for (int i = 0; i < _transitionConfigs.Length; i++)
            {
                PlayTransition(_transitionConfigs[i].name);
            }
        }
        
        public void PlayTransition(string transitionName, int times = 1, float delay = 0, UnityEngine.Events.UnityAction onComplete = null, bool reverse = false)
        {
            ITransition transition = GetTransition(transitionName);
            if (transition == null)
            {
                UnityEngine.Debug.LogWarning("dont has this Transition: "+transitionName);
            }
            else
            {
                transition.Play(times, delay, onComplete, reverse);
            }
        }

        // 预留
        public void PauseTransition(string transitionName, bool pauseState)
        {
            ITransition transition = GetTransition(transitionName);
            if (transition == null)
            {
                UnityEngine.Debug.LogWarning("dont has this Transition: "+transitionName);
                return;
            }
            transition.SetPaused(pauseState);
        }

        public void StopTransition(string transitionName)
        {
            ITransition transition = GetTransition(transitionName);
            if (transition == null)
            {
                UnityEngine.Debug.LogWarning("dont has this Transition: "+transitionName);
                return;
            }
            transition.Stop();
        }


        #region BindingFunc

        public void RemoveAllAction()
        {
            if (_bindingInfo == null)
            {
                return;
            }
            _bindingInfo.RemoveAllAction();
        }
        public void RemoveAction(string label)
        {
            if (_bindingInfo == null)
            {
                return;
            }
            _bindingInfo.RemoveAction(label);
        }

        public ModuleData[] GetModuleDatas()
        {
            if (_bindingConfig == null)
            {
                return null;
            }
            return _bindingConfig.GetModuleDataArray(this);
        }

        public LinkerData[] GetLinkerDatas()
        {
            if (_bindingConfig == null)
            {
                return null;
            }
            return _bindingConfig.GetLinkerDataArray(this);

        }
        
        public LinkerData[] GetModuleContainerLinkerDatas()
        {
            if (_bindingConfig == null)
            {
                return null;
            }
            return _bindingConfig.GetModuleContainerDataArray(this);
        }

        public void SetController(string na,int index)
        {
            var control = GetController(na);
            if (control!=null)
            {
                control.SelectedIndex = index;
            }
            else
            {
                UnityEngine.Debug.LogWarning("dont has this controller:  "+na);
            }
        }
        
        public int GetControllerSelectedIndex(string na)
        {
            var control = GetController(na);
            if (control != null)
            {
                return control.SelectedIndex;
            }
            else
            {
                UnityEngine.Debug.LogWarning("dont has this controller:  " + na);
            }
            return -1;
        }

        public string GetControllerSelectedPageName(string na)
        {
            var control = GetController(na);
            if (control != null && control.PageNum > 0)
            {
                foreach (var controllerConfig in _controllerConfigs)
                {
                    if (controllerConfig.name == na)
                    {
                        return controllerConfig.pageNames[control.SelectedIndex];
                    }
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning("dont has this controller:  " + na);
            }
            return null;
        }

        private void LinkerSet(string label,Action<BinderBase> onBinderSetFunc)
        {
            var dic = BindingInfo.GetLabel(label);
            if (dic == null) return;

            foreach (var kv in dic)
            {
                foreach (var md in kv.Value)
                {
                    var binder = BindingInfo.GetBinder(kv.Key, md.Key);
                    if (binder == null) continue;
                    for (int i = 0; i <  md.Value.Count; i++)
                    {
                        binder.linkerType = md.Value[i];
                        // binder.SetString(value);
                        onBinderSetFunc(binder);
                    }
                }
            }
        }


        public void LinkerSetQuaternion(string label, Quaternion value)
        {
            LinkerSet(label, o => o.SetQuaternion(value));
            // var linker = BindingInfo.GetLinker(label);
            // if (linker == null)
            // {
            //     return;
            // }
            // linker.SetQuaternion(value);
        }

        public void LinkerSetString(string label, string value)
        {
            LinkerSet(label, o => o.SetString(value));
            // var linker = BindingInfo.GetLinker(label);
            // if (linker == null)
            // {
            //     return;
            // }
            // linker.SetString(value);
            
            
            // var dic = BindingInfo.GetLabel(label);
            // if (dic == null) return;
            //
            // foreach (var kv in dic)
            // {
            //     foreach (var md in kv.Value)
            //     {
            //         var binder = BindingInfo.GetBinder(kv.Key, md.Key);
            //         if (binder == null) continue;
            //         for (int i = 0; i <  md.Value.Count; i++)
            //         {
            //             binder.linkerType = md.Value[i];
            //             binder.SetString(value);
            //         }
            //     }
            // }
        }

        public void LinkerSetSingle(string label, float value)
        {
            LinkerSet(label, o => o.SetSingle(value));
        }

        public void LinkerSetVector2(string label, Vector2 value)
        {
            LinkerSet(label, o => o.SetVector2(value));
        }

        public void LinkerSetVector3(string label, Vector3 value)
        {
            LinkerSet(label, o => o.SetVector3(value));
        }

        public void LinkerSetColor(string label, Color value)
        {
            LinkerSet(label, o => o.SetColor(value));
        }

        public void LinkerSetSystemObject(string label, System.Object value)
        {
            LinkerSet(label, o => o.SetSystemObject(value));
        }

        public void LinkerSetAction(string label, UnityEngine.Events.UnityAction value)
        {
            LinkerSet(label, o => o.SetAction(value));
        }

        public void LinkerSetActionInt32(string label, UnityEngine.Events.UnityAction<int> value)
        {
            LinkerSet(label, o => o.SetActionInt32(value));
        }

        public void LinkerSetActionSingle(string label, UnityEngine.Events.UnityAction<float> value)
        {
            LinkerSet(label, o => o.SetActionSingle(value));
        }

        public void LinkerSetActionString(string label, UnityEngine.Events.UnityAction<string> value)
        {
            LinkerSet(label, o => o.SetActionString(value));
        }

        public void LinkerSetInt32(string label, int value)
        {
            LinkerSet(label, o => o.SetInt32(value));
        }

        public void LinkerSetBoolean(string label, bool value)
        {
            LinkerSet(label, o => o.SetBoolean(value));
        }

        public void LinkerSetSprite(string label, Sprite value)
        {
            LinkerSet(label, o => o.SetSprite(value));
        }

        public void LinkerSetChar(string label, char value)
        {
            LinkerSet(label, o => o.SetChar(value));
        }

        public void LinkerSetRect(string label, Rect value)
        {
            LinkerSet(label, o => o.SetRect(value));
        }

        public void LinkerSetActionBoolean(string label, UnityEngine.Events.UnityAction<bool> value)
        {
            LinkerSet(label, o => o.SetActionBoolean(value));
        }

        public void LinkerSetActionVector2(string label, UnityEngine.Events.UnityAction<Vector2> value)
        {
            LinkerSet(label, o => o.SetActionVector2(value));
        }
        public void LinkerSetAction2(string label, UnityEngine.Events.UnityAction value)
        {
            LinkerSet(label, o => o.SetAction2(value));
        }
        public void LinkerSetAction2Boolean(string label, UnityEngine.Events.UnityAction<bool> value)
        {
            LinkerSet(label, o => o.SetAction2Boolean(value));
        }
        
        public void LinkerSetAction2Single(string label, UnityEngine.Events.UnityAction<Single> value)
        {
            LinkerSet(label, o => o.SetAction2Single(value));
        }
        
        public void LinkerSetAction2Int32(string label, UnityEngine.Events.UnityAction<Int32> value)
        {
            LinkerSet(label, o => o.SetAction2Int32(value));
        }
        
        public void LinkerSetAction2String(string label, UnityEngine.Events.UnityAction<string> value)
        {
            LinkerSet(label, o => o.SetAction2String(value));
        }
        
        public void LinkerSetAction2Vector2(string label, UnityEngine.Events.UnityAction<Vector2> value)
        {
            LinkerSet(label, o => o.SetAction2Vector2(value));
        }

        public void LinkerSetSystemActionIntAndObject(string label, System.Action<int, object> value)
        {
            LinkerSet(label, o => o.SetSystemActionIntAndObject(value));
        }
        
        public void LinkerSetSystemActionIntAndBool(string label, System.Action<int, bool> value)
        {
            LinkerSet(label, o => o.SetSystemActionIntAndBool(value));
        }
        
        public void LinkerSetSystemActionObject(string label, System.Action<object> value)
        {
            LinkerSet(label, o => o.SetSystemActionObject(value));
        }
        
        // public void LinkerSetDelegateInt(string label, Yoozoo.UI.YGUI.ListView.ListItemProvider value)
        // {
        //     LinkerSet(label, o => o.SetDelegateInt(value));
        // }
        //
        public void LinkerSetDelegateInt(string label, Func<int,string> value)
        {
            LinkerSet(label, o => o.SetDelegateInt(value));
        }

        private void OnDestroy()
        {
            if(_bindingInfo!=null)
                _bindingInfo.Dispose();
            if(_animationEventtweener != null)
                _animationEventtweener.Kill();
        }

        #endregion
    }
}