#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class NDUIUIExpansionWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(ND.UI.UIExpansion);
			Utils.BeginObjectRegister(type, L, translator, 0, 50, 19, 16);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetBindObject", _m_GetBindObject);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Init", _m_Init);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ClearStoredDatas", _m_ClearStoredDatas);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetStoredGameObject", _m_GetStoredGameObject);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetController", _m_GetController);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "EditorChangeControllerSelectedIndex", _m_EditorChangeControllerSelectedIndex);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "EditorGetControllerConfig", _m_EditorGetControllerConfig);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetTransition", _m_GetTransition);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "PlayAnimation", _m_PlayAnimation);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "StopAnimation", _m_StopAnimation);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "PlayAllTransitions", _m_PlayAllTransitions);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "PlayTransition", _m_PlayTransition);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "PauseTransition", _m_PauseTransition);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "StopTransition", _m_StopTransition);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "RemoveAllAction", _m_RemoveAllAction);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "RemoveAction", _m_RemoveAction);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetModuleDatas", _m_GetModuleDatas);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetLinkerDatas", _m_GetLinkerDatas);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetModuleContainerLinkerDatas", _m_GetModuleContainerLinkerDatas);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetController", _m_SetController);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetControllerSelectedIndex", _m_GetControllerSelectedIndex);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetControllerSelectedPageName", _m_GetControllerSelectedPageName);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetQuaternion", _m_LinkerSetQuaternion);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetString", _m_LinkerSetString);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetSingle", _m_LinkerSetSingle);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetVector2", _m_LinkerSetVector2);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetVector3", _m_LinkerSetVector3);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetColor", _m_LinkerSetColor);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetSystemObject", _m_LinkerSetSystemObject);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetAction", _m_LinkerSetAction);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetActionInt32", _m_LinkerSetActionInt32);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetActionSingle", _m_LinkerSetActionSingle);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetActionString", _m_LinkerSetActionString);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetInt32", _m_LinkerSetInt32);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetBoolean", _m_LinkerSetBoolean);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetSprite", _m_LinkerSetSprite);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetChar", _m_LinkerSetChar);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetRect", _m_LinkerSetRect);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetActionBoolean", _m_LinkerSetActionBoolean);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetActionVector2", _m_LinkerSetActionVector2);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetAction2", _m_LinkerSetAction2);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetAction2Boolean", _m_LinkerSetAction2Boolean);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetAction2Single", _m_LinkerSetAction2Single);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetAction2Int32", _m_LinkerSetAction2Int32);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetAction2String", _m_LinkerSetAction2String);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetAction2Vector2", _m_LinkerSetAction2Vector2);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetSystemActionIntAndObject", _m_LinkerSetSystemActionIntAndObject);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetSystemActionIntAndBool", _m_LinkerSetSystemActionIntAndBool);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetSystemActionObject", _m_LinkerSetSystemActionObject);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LinkerSetDelegateInt", _m_LinkerSetDelegateInt);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "BindedObjects", _g_get_BindedObjects);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "BindObjectToggleGroup", _g_get_BindObjectToggleGroup);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "LuaBindPath", _g_get_LuaBindPath);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "StoredGameObjects", _g_get_StoredGameObjects);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "StoredSprites", _g_get_StoredSprites);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "StoredMaterials", _g_get_StoredMaterials);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "StoredTextFontStyles", _g_get_StoredTextFontStyles);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "StoredTextColorStyles", _g_get_StoredTextColorStyles);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "StoredFloats", _g_get_StoredFloats);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "StoredInts", _g_get_StoredInts);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "StoredStrings", _g_get_StoredStrings);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "StoredAnimationCurves", _g_get_StoredAnimationCurves);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "ControllerConfigs", _g_get_ControllerConfigs);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "TransitionConfigs", _g_get_TransitionConfigs);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "BindingConfig", _g_get_BindingConfig);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Ready", _g_get_Ready);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "IsPureMode", _g_get_IsPureMode);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Controllers", _g_get_Controllers);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "BindingInfo", _g_get_BindingInfo);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "BindedObjects", _s_set_BindedObjects);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "BindObjectToggleGroup", _s_set_BindObjectToggleGroup);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "LuaBindPath", _s_set_LuaBindPath);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "StoredGameObjects", _s_set_StoredGameObjects);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "StoredSprites", _s_set_StoredSprites);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "StoredMaterials", _s_set_StoredMaterials);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "StoredTextFontStyles", _s_set_StoredTextFontStyles);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "StoredTextColorStyles", _s_set_StoredTextColorStyles);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "StoredFloats", _s_set_StoredFloats);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "StoredInts", _s_set_StoredInts);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "StoredStrings", _s_set_StoredStrings);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "StoredAnimationCurves", _s_set_StoredAnimationCurves);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "ControllerConfigs", _s_set_ControllerConfigs);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "TransitionConfigs", _s_set_TransitionConfigs);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "BindingConfig", _s_set_BindingConfig);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "IsPureMode", _s_set_IsPureMode);
            
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
            Utils.RegisterFunc(L, Utils.CLS_IDX, "New", __CreateInstanceNew);
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					var gen_ret = new ND.UI.UIExpansion();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to ND.UI.UIExpansion constructor!");
            
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstanceNew(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 0)
				{
					
					var gen_ret = new ND.UI.UIExpansion();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to ND.UI.UIExpansion constructor!");
            
        }

        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetBindObject(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _index = LuaAPI.xlua_tointeger(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.GetBindObject( _index );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Init(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.Init(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ClearStoredDatas(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.ClearStoredDatas(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetStoredGameObject(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    ushort _index = (ushort)LuaAPI.xlua_tointeger(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.GetStoredGameObject( _index );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetController(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    int _index = LuaAPI.xlua_tointeger(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.GetController( _index );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)) 
                {
                    string _name = LuaAPI.lua_tostring(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.GetController( _name );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ND.UI.UIExpansion.GetController!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_EditorChangeControllerSelectedIndex(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _controllerName = LuaAPI.lua_tostring(L, 2);
                    int _index = LuaAPI.xlua_tointeger(L, 3);
                    
                    gen_to_be_invoked.EditorChangeControllerSelectedIndex( _controllerName, _index );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_EditorGetControllerConfig(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.EditorGetControllerConfig( _name );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetTransition(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    int _index = LuaAPI.xlua_tointeger(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.GetTransition( _index );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)) 
                {
                    string _name = LuaAPI.lua_tostring(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.GetTransition( _name );
                        translator.PushAny(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ND.UI.UIExpansion.GetTransition!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_PlayAnimation(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& translator.Assignable<UnityEngine.Events.UnityAction>(L, 3)) 
                {
                    string _animName = LuaAPI.lua_tostring(L, 2);
                    UnityEngine.Events.UnityAction _onComplete = translator.GetDelegate<UnityEngine.Events.UnityAction>(L, 3);
                    
                    gen_to_be_invoked.PlayAnimation( _animName, _onComplete );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)) 
                {
                    string _animName = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.PlayAnimation( _animName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ND.UI.UIExpansion.PlayAnimation!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_StopAnimation(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _animName = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.StopAnimation( _animName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_PlayAllTransitions(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.PlayAllTransitions(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_PlayTransition(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 6&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)&& translator.Assignable<UnityEngine.Events.UnityAction>(L, 5)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 6)) 
                {
                    string _transitionName = LuaAPI.lua_tostring(L, 2);
                    int _times = LuaAPI.xlua_tointeger(L, 3);
                    float _delay = (float)LuaAPI.lua_tonumber(L, 4);
                    UnityEngine.Events.UnityAction _onComplete = translator.GetDelegate<UnityEngine.Events.UnityAction>(L, 5);
                    bool _reverse = LuaAPI.lua_toboolean(L, 6);
                    
                    gen_to_be_invoked.PlayTransition( _transitionName, _times, _delay, _onComplete, _reverse );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 5&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)&& translator.Assignable<UnityEngine.Events.UnityAction>(L, 5)) 
                {
                    string _transitionName = LuaAPI.lua_tostring(L, 2);
                    int _times = LuaAPI.xlua_tointeger(L, 3);
                    float _delay = (float)LuaAPI.lua_tonumber(L, 4);
                    UnityEngine.Events.UnityAction _onComplete = translator.GetDelegate<UnityEngine.Events.UnityAction>(L, 5);
                    
                    gen_to_be_invoked.PlayTransition( _transitionName, _times, _delay, _onComplete );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)) 
                {
                    string _transitionName = LuaAPI.lua_tostring(L, 2);
                    int _times = LuaAPI.xlua_tointeger(L, 3);
                    float _delay = (float)LuaAPI.lua_tonumber(L, 4);
                    
                    gen_to_be_invoked.PlayTransition( _transitionName, _times, _delay );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    string _transitionName = LuaAPI.lua_tostring(L, 2);
                    int _times = LuaAPI.xlua_tointeger(L, 3);
                    
                    gen_to_be_invoked.PlayTransition( _transitionName, _times );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)) 
                {
                    string _transitionName = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.PlayTransition( _transitionName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ND.UI.UIExpansion.PlayTransition!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_PauseTransition(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _transitionName = LuaAPI.lua_tostring(L, 2);
                    bool _pauseState = LuaAPI.lua_toboolean(L, 3);
                    
                    gen_to_be_invoked.PauseTransition( _transitionName, _pauseState );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_StopTransition(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _transitionName = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.StopTransition( _transitionName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RemoveAllAction(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.RemoveAllAction(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RemoveAction(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.RemoveAction( _label );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetModuleDatas(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.GetModuleDatas(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetLinkerDatas(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.GetLinkerDatas(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetModuleContainerLinkerDatas(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.GetModuleContainerLinkerDatas(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetController(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _na = LuaAPI.lua_tostring(L, 2);
                    int _index = LuaAPI.xlua_tointeger(L, 3);
                    
                    gen_to_be_invoked.SetController( _na, _index );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetControllerSelectedIndex(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _na = LuaAPI.lua_tostring(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.GetControllerSelectedIndex( _na );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetControllerSelectedPageName(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _na = LuaAPI.lua_tostring(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.GetControllerSelectedPageName( _na );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetQuaternion(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    UnityEngine.Quaternion _value;translator.Get(L, 3, out _value);
                    
                    gen_to_be_invoked.LinkerSetQuaternion( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetString(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    string _value = LuaAPI.lua_tostring(L, 3);
                    
                    gen_to_be_invoked.LinkerSetString( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetSingle(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    float _value = (float)LuaAPI.lua_tonumber(L, 3);
                    
                    gen_to_be_invoked.LinkerSetSingle( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetVector2(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    UnityEngine.Vector2 _value;translator.Get(L, 3, out _value);
                    
                    gen_to_be_invoked.LinkerSetVector2( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetVector3(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    UnityEngine.Vector3 _value;translator.Get(L, 3, out _value);
                    
                    gen_to_be_invoked.LinkerSetVector3( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetColor(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    UnityEngine.Color _value;translator.Get(L, 3, out _value);
                    
                    gen_to_be_invoked.LinkerSetColor( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetSystemObject(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    object _value = translator.GetObject(L, 3, typeof(object));
                    
                    gen_to_be_invoked.LinkerSetSystemObject( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetAction(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    UnityEngine.Events.UnityAction _value = translator.GetDelegate<UnityEngine.Events.UnityAction>(L, 3);
                    
                    gen_to_be_invoked.LinkerSetAction( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetActionInt32(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    UnityEngine.Events.UnityAction<int> _value = translator.GetDelegate<UnityEngine.Events.UnityAction<int>>(L, 3);
                    
                    gen_to_be_invoked.LinkerSetActionInt32( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetActionSingle(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    UnityEngine.Events.UnityAction<float> _value = translator.GetDelegate<UnityEngine.Events.UnityAction<float>>(L, 3);
                    
                    gen_to_be_invoked.LinkerSetActionSingle( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetActionString(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    UnityEngine.Events.UnityAction<string> _value = translator.GetDelegate<UnityEngine.Events.UnityAction<string>>(L, 3);
                    
                    gen_to_be_invoked.LinkerSetActionString( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetInt32(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    int _value = LuaAPI.xlua_tointeger(L, 3);
                    
                    gen_to_be_invoked.LinkerSetInt32( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetBoolean(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    bool _value = LuaAPI.lua_toboolean(L, 3);
                    
                    gen_to_be_invoked.LinkerSetBoolean( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetSprite(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    UnityEngine.Sprite _value = (UnityEngine.Sprite)translator.GetObject(L, 3, typeof(UnityEngine.Sprite));
                    
                    gen_to_be_invoked.LinkerSetSprite( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetChar(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    char _value = (char)LuaAPI.xlua_tointeger(L, 3);
                    
                    gen_to_be_invoked.LinkerSetChar( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetRect(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    UnityEngine.Rect _value;translator.Get(L, 3, out _value);
                    
                    gen_to_be_invoked.LinkerSetRect( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetActionBoolean(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    UnityEngine.Events.UnityAction<bool> _value = translator.GetDelegate<UnityEngine.Events.UnityAction<bool>>(L, 3);
                    
                    gen_to_be_invoked.LinkerSetActionBoolean( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetActionVector2(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    UnityEngine.Events.UnityAction<UnityEngine.Vector2> _value = translator.GetDelegate<UnityEngine.Events.UnityAction<UnityEngine.Vector2>>(L, 3);
                    
                    gen_to_be_invoked.LinkerSetActionVector2( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetAction2(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    UnityEngine.Events.UnityAction _value = translator.GetDelegate<UnityEngine.Events.UnityAction>(L, 3);
                    
                    gen_to_be_invoked.LinkerSetAction2( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetAction2Boolean(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    UnityEngine.Events.UnityAction<bool> _value = translator.GetDelegate<UnityEngine.Events.UnityAction<bool>>(L, 3);
                    
                    gen_to_be_invoked.LinkerSetAction2Boolean( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetAction2Single(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    UnityEngine.Events.UnityAction<float> _value = translator.GetDelegate<UnityEngine.Events.UnityAction<float>>(L, 3);
                    
                    gen_to_be_invoked.LinkerSetAction2Single( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetAction2Int32(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    UnityEngine.Events.UnityAction<int> _value = translator.GetDelegate<UnityEngine.Events.UnityAction<int>>(L, 3);
                    
                    gen_to_be_invoked.LinkerSetAction2Int32( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetAction2String(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    UnityEngine.Events.UnityAction<string> _value = translator.GetDelegate<UnityEngine.Events.UnityAction<string>>(L, 3);
                    
                    gen_to_be_invoked.LinkerSetAction2String( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetAction2Vector2(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    UnityEngine.Events.UnityAction<UnityEngine.Vector2> _value = translator.GetDelegate<UnityEngine.Events.UnityAction<UnityEngine.Vector2>>(L, 3);
                    
                    gen_to_be_invoked.LinkerSetAction2Vector2( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetSystemActionIntAndObject(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    System.Action<int, object> _value = translator.GetDelegate<System.Action<int, object>>(L, 3);
                    
                    gen_to_be_invoked.LinkerSetSystemActionIntAndObject( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetSystemActionIntAndBool(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    System.Action<int, bool> _value = translator.GetDelegate<System.Action<int, bool>>(L, 3);
                    
                    gen_to_be_invoked.LinkerSetSystemActionIntAndBool( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetSystemActionObject(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    System.Action<object> _value = translator.GetDelegate<System.Action<object>>(L, 3);
                    
                    gen_to_be_invoked.LinkerSetSystemActionObject( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LinkerSetDelegateInt(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _label = LuaAPI.lua_tostring(L, 2);
                    System.Func<int, string> _value = translator.GetDelegate<System.Func<int, string>>(L, 3);
                    
                    gen_to_be_invoked.LinkerSetDelegateInt( _label, _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_BindedObjects(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.BindedObjects);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_BindObjectToggleGroup(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.BindObjectToggleGroup);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_LuaBindPath(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushstring(L, gen_to_be_invoked.LuaBindPath);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_StoredGameObjects(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.StoredGameObjects);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_StoredSprites(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.StoredSprites);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_StoredMaterials(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.StoredMaterials);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_StoredTextFontStyles(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.StoredTextFontStyles);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_StoredTextColorStyles(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.StoredTextColorStyles);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_StoredFloats(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.StoredFloats);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_StoredInts(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.StoredInts);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_StoredStrings(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.StoredStrings);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_StoredAnimationCurves(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.StoredAnimationCurves);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_ControllerConfigs(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.ControllerConfigs);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_TransitionConfigs(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.TransitionConfigs);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_BindingConfig(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.BindingConfig);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Ready(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.Ready);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_IsPureMode(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.IsPureMode);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Controllers(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.Controllers);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_BindingInfo(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.BindingInfo);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_BindedObjects(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.BindedObjects = (UnityEngine.Object[])translator.GetObject(L, 2, typeof(UnityEngine.Object[]));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_BindObjectToggleGroup(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.BindObjectToggleGroup = (System.Collections.Generic.Dictionary<string, bool>)translator.GetObject(L, 2, typeof(System.Collections.Generic.Dictionary<string, bool>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_LuaBindPath(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.LuaBindPath = LuaAPI.lua_tostring(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_StoredGameObjects(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.StoredGameObjects = (UnityEngine.GameObject[])translator.GetObject(L, 2, typeof(UnityEngine.GameObject[]));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_StoredSprites(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.StoredSprites = (UnityEngine.Sprite[])translator.GetObject(L, 2, typeof(UnityEngine.Sprite[]));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_StoredMaterials(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.StoredMaterials = (UnityEngine.Material[])translator.GetObject(L, 2, typeof(UnityEngine.Material[]));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_StoredTextFontStyles(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.StoredTextFontStyles = (ND.UI.Core.TextStyleBase[])translator.GetObject(L, 2, typeof(ND.UI.Core.TextStyleBase[]));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_StoredTextColorStyles(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.StoredTextColorStyles = (ND.UI.Core.ColorStyleBase[])translator.GetObject(L, 2, typeof(ND.UI.Core.ColorStyleBase[]));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_StoredFloats(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.StoredFloats = (float[])translator.GetObject(L, 2, typeof(float[]));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_StoredInts(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.StoredInts = (int[])translator.GetObject(L, 2, typeof(int[]));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_StoredStrings(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.StoredStrings = (string[])translator.GetObject(L, 2, typeof(string[]));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_StoredAnimationCurves(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.StoredAnimationCurves = (UnityEngine.AnimationCurve[])translator.GetObject(L, 2, typeof(UnityEngine.AnimationCurve[]));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_ControllerConfigs(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.ControllerConfigs = (ND.UI.Core.Model.ControllerConfig[])translator.GetObject(L, 2, typeof(ND.UI.Core.Model.ControllerConfig[]));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_TransitionConfigs(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.TransitionConfigs = (ND.UI.Core.Model.TransitionConfig[])translator.GetObject(L, 2, typeof(ND.UI.Core.Model.TransitionConfig[]));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_BindingConfig(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.BindingConfig = (ND.UI.BindingConfig)translator.GetObject(L, 2, typeof(ND.UI.BindingConfig));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_IsPureMode(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.UIExpansion gen_to_be_invoked = (ND.UI.UIExpansion)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.IsPureMode = LuaAPI.lua_toboolean(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
