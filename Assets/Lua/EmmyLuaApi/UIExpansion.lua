---@class UIExpansion : MonoBehaviour
---@field public BindedObjects Object[]
---@field public LuaBindPath string
---@field public StoredGameObjects GameObject[]
---@field public StoredSprites Sprite[]
---@field public StoredMaterials Material[]
---@field public StoredTextFontStyles TextStyleBase[]
---@field public StoredTextColorStyles ColorStyleBase[]
---@field public StoredFloats Single[]
---@field public StoredInts Int32[]
---@field public StoredStrings String[]
---@field public StoredAnimationCurves AnimationCurve[]
---@field public ControllerConfigs ControllerConfig[]
---@field public TransitionConfigs TransitionConfig[]
---@field public BindingConfig BindingConfig
---@field public Ready bool
---@field public IsPureMode bool
---@field public Controllers Controller[]
---@field public BindingInfo Binding
local UIExpansion={ }
---@public
---@param index number
---@return Object
function UIExpansion:GetBindObject(index) end
---@public
---@return void
function UIExpansion:Init() end
---@public
---@return void
function UIExpansion:ClearStoredDatas() end
---@public
---@param index number
---@return GameObject
function UIExpansion:GetStoredGameObject(index) end
---@public
---@param index number
---@return IController
function UIExpansion:GetController(index) end
---@public
---@param name string
---@return IController
function UIExpansion:GetController(name) end
---@public
---@param controllerName string
---@param index number
---@return void
function UIExpansion:EditorChangeControllerSelectedIndex(controllerName, index) end
---@public
---@param name string
---@return ControllerConfig
function UIExpansion:EditorGetControllerConfig(name) end
---@public
---@param index number
---@return ITransition
function UIExpansion:GetTransition(index) end
---@public
---@param name string
---@return ITransition
function UIExpansion:GetTransition(name) end
---@public
---@param animName string
---@param onComplete UnityAction
---@return void
function UIExpansion:PlayAnimation(animName, onComplete) end
---@public
---@param animName string
---@return void
function UIExpansion:StopAnimation(animName) end
---@public
---@return void
function UIExpansion:PlayAllTransitions() end
---@public
---@param transitionName string
---@param times number
---@param delay number
---@param onComplete UnityAction
---@param reverse bool
---@return void
function UIExpansion:PlayTransition(transitionName, times, delay, onComplete, reverse) end
---@public
---@param transitionName string
---@param pauseState bool
---@return void
function UIExpansion:PauseTransition(transitionName, pauseState) end
---@public
---@param transitionName string
---@return void
function UIExpansion:StopTransition(transitionName) end
---@public
---@return void
function UIExpansion:RemoveAllAction() end
---@public
---@param label string
---@return void
function UIExpansion:RemoveAction(label) end
---@public
---@return ModuleData[]
function UIExpansion:GetModuleDatas() end
---@public
---@return LinkerData[]
function UIExpansion:GetLinkerDatas() end
---@public
---@return LinkerData[]
function UIExpansion:GetModuleContainerLinkerDatas() end
---@public
---@param na string
---@param index number
---@return void
function UIExpansion:SetController(na, index) end
---@public
---@param na string
---@return number
function UIExpansion:GetControllerSelectedIndex(na) end
---@public
---@param na string
---@return string
function UIExpansion:GetControllerSelectedPageName(na) end
---@public
---@param label string
---@param value Quaternion
---@return void
function UIExpansion:LinkerSetQuaternion(label, value) end
---@public
---@param label string
---@param value string
---@return void
function UIExpansion:LinkerSetString(label, value) end
---@public
---@param label string
---@param value number
---@return void
function UIExpansion:LinkerSetSingle(label, value) end
---@public
---@param label string
---@param value Vector2
---@return void
function UIExpansion:LinkerSetVector2(label, value) end
---@public
---@param label string
---@param value Vector3
---@return void
function UIExpansion:LinkerSetVector3(label, value) end
---@public
---@param label string
---@param value Color
---@return void
function UIExpansion:LinkerSetColor(label, value) end
---@public
---@param label string
---@param value Object
---@return void
function UIExpansion:LinkerSetSystemObject(label, value) end
---@public
---@param label string
---@param value UnityAction
---@return void
function UIExpansion:LinkerSetAction(label, value) end
---@public
---@param label string
---@param value UnityAction`1
---@return void
function UIExpansion:LinkerSetActionInt32(label, value) end
---@public
---@param label string
---@param value UnityAction`1
---@return void
function UIExpansion:LinkerSetActionSingle(label, value) end
---@public
---@param label string
---@param value UnityAction`1
---@return void
function UIExpansion:LinkerSetActionString(label, value) end
---@public
---@param label string
---@param value number
---@return void
function UIExpansion:LinkerSetInt32(label, value) end
---@public
---@param label string
---@param value bool
---@return void
function UIExpansion:LinkerSetBoolean(label, value) end
---@public
---@param label string
---@param value Sprite
---@return void
function UIExpansion:LinkerSetSprite(label, value) end
---@public
---@param label string
---@param value Char
---@return void
function UIExpansion:LinkerSetChar(label, value) end
---@public
---@param label string
---@param value Rect
---@return void
function UIExpansion:LinkerSetRect(label, value) end
---@public
---@param label string
---@param value UnityAction`1
---@return void
function UIExpansion:LinkerSetActionBoolean(label, value) end
---@public
---@param label string
---@param value UnityAction`1
---@return void
function UIExpansion:LinkerSetActionVector2(label, value) end
---@public
---@param label string
---@param value UnityAction
---@return void
function UIExpansion:LinkerSetAction2(label, value) end
---@public
---@param label string
---@param value UnityAction`1
---@return void
function UIExpansion:LinkerSetAction2Boolean(label, value) end
---@public
---@param label string
---@param value UnityAction`1
---@return void
function UIExpansion:LinkerSetAction2Single(label, value) end
---@public
---@param label string
---@param value UnityAction`1
---@return void
function UIExpansion:LinkerSetAction2Int32(label, value) end
---@public
---@param label string
---@param value UnityAction`1
---@return void
function UIExpansion:LinkerSetAction2String(label, value) end
---@public
---@param label string
---@param value UnityAction`1
---@return void
function UIExpansion:LinkerSetAction2Vector2(label, value) end
---@public
---@param label string
---@param value Action`2
---@return void
function UIExpansion:LinkerSetSystemActionIntAndObject(label, value) end
---@public
---@param label string
---@param value Action`2
---@return void
function UIExpansion:LinkerSetSystemActionIntAndBool(label, value) end
---@public
---@param label string
---@param value Action`1
---@return void
function UIExpansion:LinkerSetSystemActionObject(label, value) end
---@public
---@param label string
---@param value Func`2
---@return void
function UIExpansion:LinkerSetDelegateInt(label, value) end
ND.UI.UIExpansion = UIExpansion