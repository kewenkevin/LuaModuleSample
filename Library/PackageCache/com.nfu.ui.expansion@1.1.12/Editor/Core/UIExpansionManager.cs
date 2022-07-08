using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ND.UI
{
    public class UIExpansionManager 
    {
        private static UIExpansionManager _instance;

        public static UIExpansionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UIExpansionManager();
                }
                return _instance;
            }
        }

        public UIExpansionControllerPanelSettings ControllerSettings
        {
            get { return _windowSettings.ControllerSettings; }
        }

        public UIExpansionControllerWrapper CurControllerWrapper
        {
            get
            {
                if(ControllerSettings.CurControllerIndex < 0)
                {
                    return null;
                }
                return _curUIExpansionWrapper.ControllerWrapperList[ControllerSettings.CurControllerIndex];
            }
        }

        public UIExpansionBindingPanelSettings BindingSettings
        {
            get
            {
                if (_windowSettings == null)
                {
                    return null;
                }
                return _windowSettings.BindingSettings;
            }
        }

        public UIExpansionBindingWrapper CurBindingWrapper
        {
            get
            {
                return _curUIExpansionWrapper.BindingWrapper;
            }
        }

        public UIExpansionTransitionPanelSettings TransitionSettings
        {
            get
            {
                if (_windowSettings == null)
                {
                    return null;
                }
                return _windowSettings.TransitionSettings;
            }
        }

        public UIExpansionTransitionWrapper CurTransitionWrapper
        {
            get
            {
                if (TransitionSettings == null)
                {
                    return null;
                }
                if (TransitionSettings.CurTransitionIndex < 0)
                {
                    return null;
                }
                return _curUIExpansionWrapper.TransitionWrapperList[TransitionSettings.CurTransitionIndex];
            }
        }

        private UIExpansion _curUIExpansion;

        private UIExpansionWrapper _curUIExpansionWrapper;

        private UIExpansionWindowSettings _windowSettings;

        private double _accumulatedTime;

        private DateTime _previousTime;

        private bool _needRepaint;

        private bool _needReInit;

        private bool _autoSave;

        private bool m_isWindowOpen = false;

        public UIExpansion CurUIExpansion
        {
            get
            {
                return _curUIExpansion;
            }
            set
            {
                if (_curUIExpansion == value)
                {
                  return;
                }
                SaveCurUIExpansion(value);
                _curUIExpansion = value;
                Init();
            }
        }

        public bool AutoSave
        {
            get
            {
                return _autoSave;
            }

            set
            {
                _autoSave = value;
            }
        }

        public UIExpansionWrapper CurUIExpansionWrapper
        {
            get
            {
                return _curUIExpansionWrapper;
            }

            set
            {
                _curUIExpansionWrapper = value;
            }
        }

        public UIExpansionWindowSettings WindowSettings
        {
            get
            {
                return _windowSettings;
            }

            set
            {
                _windowSettings = value;
            }
        }

        public bool NeedRepaint
        {
            get
            {
                return _needRepaint;
            }

            set
            {
                _needRepaint = value;
            }
        }

        public bool IsWindowOpen
        {
            get
            {
                return m_isWindowOpen;
            }
            set
            {
                m_isWindowOpen = value;
            }
        }

        public bool NeedReInit { get => _needReInit; set => _needReInit = value; }

        private void SaveCurUIExpansion(UIExpansion uiExpansion)
        {
            if (uiExpansion == null)
            {
                EditorPrefs.SetInt("UIExpansion.UIExpansionGameObjectType", (int)UIExpansionGameObjectType.None);
            }
            else
            {
                if (PrefabUtility.GetPrefabInstanceStatus(uiExpansion.gameObject) == PrefabInstanceStatus.Connected)
                {
                    Debug.Log("是一个prefab的Instance");
                    UnityEngine.Object parentObject = PrefabUtility.GetCorrespondingObjectFromSource(uiExpansion.gameObject);
                    string path = AssetDatabase.GetAssetPath(parentObject);
                    EditorPrefs.SetInt("UIExpansion.UIExpansionGameObjectType", (int)UIExpansionGameObjectType.PrefabInstance);
                    EditorPrefs.SetInt("UIExpansion.UIExpansionGameObjectInstanceID", uiExpansion.gameObject.GetInstanceID());
                }
                else if (PrefabUtility.GetPrefabAssetType(uiExpansion.gameObject) == PrefabAssetType.Regular ||
                             PrefabUtility.GetPrefabAssetType(uiExpansion.gameObject) == PrefabAssetType.Variant)
                {
                    Debug.Log("是一个prefab的本体");
                    string path = AssetDatabase.GetAssetPath(uiExpansion.gameObject);
                    EditorPrefs.SetInt("UIExpansion.UIExpansionGameObjectType", (int)UIExpansionGameObjectType.PrefabAsset);
                    EditorPrefs.SetString("UIExpansion.UIExpansionGameObjectGUID", AssetDatabase.AssetPathToGUID(path));
                }
                else
                {
                    Debug.Log("是一个普通的GameObject");
                    EditorPrefs.SetInt("UIExpansion.UIExpansionGameObjectType", (int)UIExpansionGameObjectType.NormalGameObject);
                    EditorPrefs.SetInt("UIExpansion.UIExpansionGameObjectInstanceID", uiExpansion.gameObject.GetInstanceID());
                }

            }
        }

        public void LoadLastUIExpansion()
        {
            if (EditorPrefs.HasKey("UIExpansion.UIExpansionGameObjectType"))
            {
                UIExpansionGameObjectType type = (UIExpansionGameObjectType)EditorPrefs.GetInt("UIExpansion.UIExpansionGameObjectType");
                if (type == UIExpansionGameObjectType.PrefabInstance ||
                    type == UIExpansionGameObjectType.NormalGameObject)
                {
                    if (EditorPrefs.HasKey("UIExpansion.UIExpansionGameObjectInstanceID"))
                    {
                        int targetInstanceID = EditorPrefs.GetInt("UIExpansion.UIExpansionGameObjectInstanceID");
                        GameObject[] gos = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
                        foreach (GameObject go in gos)
                        {
                            if (go.gameObject.GetInstanceID() == 42860)
                            {
                                if (go.GetComponent<UIExpansion>() != null)
                                {
                                    // Debug.Log(go.name);
                                }
                            }
                            if (go.gameObject.GetInstanceID() == targetInstanceID && go.GetComponent<UIExpansion>() != null)
                            {
                                Debug.Log(go.name);
                                _curUIExpansion = go.GetComponent<UIExpansion>();
                                Init();
                            }
                        }
                    }
                }
                else if (type == UIExpansionGameObjectType.PrefabAsset)
                {
                    if (EditorPrefs.HasKey("UIExpansion.UIExpansionGameObjectGUID"))
                    {
                        string path = AssetDatabase.GUIDToAssetPath(EditorPrefs.GetString("UIExpansion.UIExpansionGameObjectGUID"));
                        if (!string.IsNullOrEmpty(path))
                        {
                            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                            if (go != null && go.GetComponent<UIExpansion>() != null)
                            {
                                _curUIExpansion = go.GetComponent<UIExpansion>();
                                Init();
                            }
                        }
                    }
                }
            }
        }

        public void Init()
        {
            _previousTime = DateTime.Now;
            _accumulatedTime = 0;
            if (_curUIExpansion != null)
            {
                _curUIExpansionWrapper = new UIExpansionWrapper(_curUIExpansion);
                _windowSettings = new UIExpansionWindowSettings();
            }
            else
            {
                _curUIExpansionWrapper = null;
                _windowSettings = null;
            }
         
        }

        public void OnWindowUpdate()
        {
            if (_curUIExpansion == null)
            {
                return;
            }
            double delta = (DateTime.Now - _previousTime).TotalSeconds;
            _previousTime = DateTime.Now;
            if (delta <= 0)
            {
                return;
            }
            _accumulatedTime += delta;
            if (_accumulatedTime > UIExpansionEditorUtility.WINDOW_REPAINT_TIME)
            {
                _needRepaint = true;
                _accumulatedTime -= UIExpansionEditorUtility.WINDOW_REPAINT_TIME;
            } 
            OnUpdate((float)delta);
        }

        private void OnUpdate(float deltaTime)
        {
            // Debug.Log(deltaTime); 
            if (CurUIExpansion.IsPureMode || UIExpansionEditorConfigSettingsProvider.IsPureMode())
            {
                _windowSettings.CurPanelIndex = 0; //强行设置成Controller
            }
            _windowSettings.Panels[_windowSettings.CurPanelIndex].OnUpdate(deltaTime);
        }

        public void OnDispose()
        {
            _instance = null;
        }

        #region Controller Action
        public void CreateNewControllerWrapper(string wrapperName, List<string> pageNameList, List<string> pageTipsList)
        {
            UIExpansionControllerWrapper controllerWrapper = new UIExpansionControllerWrapper(wrapperName, pageNameList, pageTipsList);
            _curUIExpansionWrapper.ControllerWrapperList.Add(controllerWrapper);
            ControllerSettings.CurControllerIndex = _curUIExpansionWrapper.ControllerWrapperList.Count - 1;
        }

        public void DeleteCurControllerWrapper()
        {
            int needDeleteIndex = ControllerSettings.CurControllerIndex;
            _curUIExpansionWrapper.ControllerWrapperList.RemoveAt(needDeleteIndex);
            ControllerSettings.CurControllerIndex = _curUIExpansionWrapper.ControllerWrapperList.Count > 0 ? 0 : -1;
        }

        public void DeleteControllerPage(int index)
        {
            ControllerSettings.CurChangeNamePageIndex = -1;
            CurControllerWrapper.RemovePage(index);
        }
        #endregion

        #region Transition Action

        public void CreateNewTransitionWrapper(string name)
        {
            UIExpansionTransitionWrapper transitionWrapper = new UIExpansionTransitionWrapper(name);
            _curUIExpansionWrapper.TransitionWrapperList.Add(transitionWrapper);
            TransitionSettings.CurTransitionIndex = _curUIExpansionWrapper.TransitionWrapperList.Count - 1;
        }

        public void DeleteCurTransitionWrapper()
        {
            int needDeleteIndex = TransitionSettings.CurTransitionIndex;
            _curUIExpansionWrapper.TransitionWrapperList.RemoveAt(needDeleteIndex);
            TransitionSettings.CurTransitionIndex = _curUIExpansionWrapper.TransitionWrapperList.Count > 0 ? 0 : -1;
        }

        #endregion
    }
}