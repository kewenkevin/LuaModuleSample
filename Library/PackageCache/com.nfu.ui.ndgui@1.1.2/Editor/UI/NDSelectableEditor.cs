using ND.UI.NDUI.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.UI;
using UnityEngine.UI;
using UnityEngine;

namespace ND.UI.NDUI
{
    [CanEditMultipleObjects]
    public class NDSelectableEditor : SelectableEditor
    {
        private SerializedProperty m_InteractableProperty;
        
        private SerializedProperty m_TargetGraphicProperty;
        private SerializedProperty m_TransitionProperty;
        
        private SerializedProperty m_ColorBlockProperty;
        private SerializedProperty m_SpriteStateProperty;
        private SerializedProperty m_AnimTriggerProperty;
        
        private AnimBool m_ShowColorTint2 = new AnimBool();
        private AnimBool m_ShowSpriteTrasition2 = new AnimBool();
        private AnimBool m_ShowAnimTransition2 = new AnimBool();
        
        protected virtual void OnEnable()
        {
            m_InteractableProperty  = serializedObject.FindProperty("m_Interactable");
            
            m_TargetGraphicProperty = serializedObject.FindProperty("m_TargetGraphic");
            m_TransitionProperty    = serializedObject.FindProperty("m_Transition");
            
            m_ColorBlockProperty    = serializedObject.FindProperty("m_Colors");
            m_SpriteStateProperty   = serializedObject.FindProperty("m_SpriteState");
            m_AnimTriggerProperty   = serializedObject.FindProperty("m_AnimationTriggers");
            
        }

        public override void OnInspectorGUI()
        {
            var isPrefabInstance = UIPrefabUtils.IsPrefabInstance(serializedObject.targetObject as MonoBehaviour);
            if(isPrefabInstance) 
            {
                EditorGUILayout.HelpBox("由于UI编辑规范限制，请打开Prefab后再编辑相关属性", MessageType.Info, true);
                if (GUILayout.Button("打开Prefab"))
                {
                    UIPrefabUtils.OpenPrefab(serializedObject.targetObject as MonoBehaviour);
                }
            }
            
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_InteractableProperty);
            serializedObject.ApplyModifiedProperties();
            
            EditorGUI.BeginDisabledGroup(isPrefabInstance);
            DrawTransition();
            EditorGUI.EndDisabledGroup();
            
            
        }


        protected void DrawTransition()
        {
            serializedObject.Update();
            
            EditorGUILayout.Space();
            
            var trans = GetTransition(m_TransitionProperty);

            var graphic = m_TargetGraphicProperty.objectReferenceValue as Graphic;
            if (graphic == null)
                graphic = (target as Selectable).GetComponent<Graphic>();

            var animator = (target as Selectable).GetComponent<Animator>();
            m_ShowColorTint2.target = (!m_TransitionProperty.hasMultipleDifferentValues && trans == Button.Transition.ColorTint);
            m_ShowSpriteTrasition2.target = (!m_TransitionProperty.hasMultipleDifferentValues && trans == Button.Transition.SpriteSwap);
            m_ShowAnimTransition2.target = (!m_TransitionProperty.hasMultipleDifferentValues && trans == Button.Transition.Animation);

            EditorGUILayout.PropertyField(m_TransitionProperty);

            ++EditorGUI.indentLevel;
            {
                if (trans == Selectable.Transition.ColorTint || trans == Selectable.Transition.SpriteSwap)
                {
                    EditorGUILayout.PropertyField(m_TargetGraphicProperty);
                }

                switch (trans)
                {
                    case Selectable.Transition.ColorTint:
                        if (graphic == null)
                            EditorGUILayout.HelpBox("You must have a Graphic target in order to use a color transition.", MessageType.Warning);
                        break;

                    case Selectable.Transition.SpriteSwap:
                        if (graphic as Image == null)
                            EditorGUILayout.HelpBox("You must have a Image target in order to use a sprite swap transition.", MessageType.Warning);
                        break;
                }

                if (EditorGUILayout.BeginFadeGroup(m_ShowColorTint2.faded))
                {
                    EditorGUILayout.PropertyField(m_ColorBlockProperty);
                }
                EditorGUILayout.EndFadeGroup();

                if (EditorGUILayout.BeginFadeGroup(m_ShowSpriteTrasition2.faded))
                {
                    EditorGUILayout.PropertyField(m_SpriteStateProperty);
                }
                EditorGUILayout.EndFadeGroup();

                if (EditorGUILayout.BeginFadeGroup(m_ShowAnimTransition2.faded))
                {
                    EditorGUILayout.PropertyField(m_AnimTriggerProperty);

                    if (animator == null || animator.runtimeAnimatorController == null)
                    {
                        Rect buttonRect = EditorGUILayout.GetControlRect();
                        buttonRect.xMin += EditorGUIUtility.labelWidth;
                        if (GUI.Button(buttonRect, "Auto Generate Animation", EditorStyles.miniButton))
                        {
                            var controller = GenerateSelectableAnimatorContoller((target as Selectable).animationTriggers, target as Selectable);
                            if (controller != null)
                            {
                                if (animator == null)
                                    animator = (target as Selectable).gameObject.AddComponent<Animator>();

                                UnityEditor.Animations.AnimatorController.SetAnimatorController(animator, controller);
                            }
                        }
                    }
                }
                EditorGUILayout.EndFadeGroup();
            }
            --EditorGUI.indentLevel;
            serializedObject.ApplyModifiedProperties();
        }
        
        static Selectable.Transition GetTransition(SerializedProperty transition)
        {
            return (Selectable.Transition)transition.enumValueIndex;
        }
        private static UnityEditor.Animations.AnimatorController GenerateSelectableAnimatorContoller(AnimationTriggers animationTriggers, Selectable target)
        {
            if (target == null)
                return null;

            // Where should we create the controller?
            var path = GetSaveControllerPath(target);
            if (string.IsNullOrEmpty(path))
                return null;

            // figure out clip names
            var normalName = string.IsNullOrEmpty(animationTriggers.normalTrigger) ? "Normal" : animationTriggers.normalTrigger;
            var highlightedName = string.IsNullOrEmpty(animationTriggers.highlightedTrigger) ? "Highlighted" : animationTriggers.highlightedTrigger;
            var pressedName = string.IsNullOrEmpty(animationTriggers.pressedTrigger) ? "Pressed" : animationTriggers.pressedTrigger;

            #if UNITY_2019_1_OR_NEWER           
            var selectedName = string.IsNullOrEmpty(animationTriggers.selectedTrigger) ? "Selected" : animationTriggers.selectedTrigger;
            #endif
            var disabledName = string.IsNullOrEmpty(animationTriggers.disabledTrigger) ? "Disabled" : animationTriggers.disabledTrigger;

            // Create controller and hook up transitions.
            var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(path);
            GenerateTriggerableTransition(normalName, controller);
            GenerateTriggerableTransition(highlightedName, controller);
            GenerateTriggerableTransition(pressedName, controller);
            #if UNITY_2019_1_OR_NEWER
            GenerateTriggerableTransition(selectedName, controller);
            #endif 
            GenerateTriggerableTransition(disabledName, controller);

            AssetDatabase.ImportAsset(path);

            return controller;
        }
        
        private static string GetSaveControllerPath(Selectable target)
        {
            var defaultName = target.gameObject.name;
            var message = string.Format("Create a new animator for the game object '{0}':", defaultName);
            return EditorUtility.SaveFilePanelInProject("New Animation Contoller", defaultName, "controller", message);
        }
        
        private static AnimationClip GenerateTriggerableTransition(string name, UnityEditor.Animations.AnimatorController controller)
        {
            // Create the clip
            var clip = UnityEditor.Animations.AnimatorController.AllocateAnimatorClip(name);
            AssetDatabase.AddObjectToAsset(clip, controller);

            // Create a state in the animatior controller for this clip
            var state = controller.AddMotion(clip);

            // Add a transition property
            controller.AddParameter(name, AnimatorControllerParameterType.Trigger);

            // Add an any state transition
            var stateMachine = controller.layers[0].stateMachine;
            var transition = stateMachine.AddAnyStateTransition(state);
            transition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, name);
            return clip;
        }
    }
}
