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
    public class NDUINDUINDToggleWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(ND.UI.NDUI.NDToggle);
			Utils.BeginObjectRegister(type, L, translator, 0, 8, 7, 7);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Rebuild", _m_Rebuild);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LayoutComplete", _m_LayoutComplete);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GraphicUpdateComplete", _m_GraphicUpdateComplete);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetIsOnWithoutNotify", _m_SetIsOnWithoutNotify);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Set", _m_Set);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "UpdateStatus", _m_UpdateStatus);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnPointerClick", _m_OnPointerClick);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnSubmit", _m_OnSubmit);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "group", _g_get_group);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "isOn", _g_get_isOn);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "graphic", _g_get_graphic);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "inversGraphic", _g_get_inversGraphic);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "onValueChanged", _g_get_onValueChanged);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "on", _g_get_on);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "off", _g_get_off);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "group", _s_set_group);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "isOn", _s_set_isOn);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "graphic", _s_set_graphic);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "inversGraphic", _s_set_inversGraphic);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "onValueChanged", _s_set_onValueChanged);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "on", _s_set_on);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "off", _s_set_off);
            
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
            Utils.RegisterFunc(L, Utils.CLS_IDX, "New", __CreateInstanceNew);
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "ND.UI.NDUI.NDToggle does not have a constructor!");
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstanceNew(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "ND.UI.NDUI.NDToggle does not have a constructor!");
        }

        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Rebuild(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.NDUI.NDToggle gen_to_be_invoked = (ND.UI.NDUI.NDToggle)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.UI.CanvasUpdate _executing;translator.Get(L, 2, out _executing);
                    
                    gen_to_be_invoked.Rebuild( _executing );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LayoutComplete(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.NDUI.NDToggle gen_to_be_invoked = (ND.UI.NDUI.NDToggle)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.LayoutComplete(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GraphicUpdateComplete(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.NDUI.NDToggle gen_to_be_invoked = (ND.UI.NDUI.NDToggle)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.GraphicUpdateComplete(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetIsOnWithoutNotify(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.NDUI.NDToggle gen_to_be_invoked = (ND.UI.NDUI.NDToggle)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    bool _value = LuaAPI.lua_toboolean(L, 2);
                    
                    gen_to_be_invoked.SetIsOnWithoutNotify( _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Set(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.NDUI.NDToggle gen_to_be_invoked = (ND.UI.NDUI.NDToggle)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 3)) 
                {
                    bool _value = LuaAPI.lua_toboolean(L, 2);
                    bool _sendCallback = LuaAPI.lua_toboolean(L, 3);
                    
                    gen_to_be_invoked.Set( _value, _sendCallback );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 2)) 
                {
                    bool _value = LuaAPI.lua_toboolean(L, 2);
                    
                    gen_to_be_invoked.Set( _value );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ND.UI.NDUI.NDToggle.Set!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UpdateStatus(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.NDUI.NDToggle gen_to_be_invoked = (ND.UI.NDUI.NDToggle)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.UpdateStatus(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnPointerClick(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.NDUI.NDToggle gen_to_be_invoked = (ND.UI.NDUI.NDToggle)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.EventSystems.PointerEventData _eventData = (UnityEngine.EventSystems.PointerEventData)translator.GetObject(L, 2, typeof(UnityEngine.EventSystems.PointerEventData));
                    
                    gen_to_be_invoked.OnPointerClick( _eventData );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnSubmit(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.NDUI.NDToggle gen_to_be_invoked = (ND.UI.NDUI.NDToggle)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.EventSystems.BaseEventData _eventData = (UnityEngine.EventSystems.BaseEventData)translator.GetObject(L, 2, typeof(UnityEngine.EventSystems.BaseEventData));
                    
                    gen_to_be_invoked.OnSubmit( _eventData );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_group(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDToggle gen_to_be_invoked = (ND.UI.NDUI.NDToggle)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.group);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_isOn(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDToggle gen_to_be_invoked = (ND.UI.NDUI.NDToggle)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.isOn);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_graphic(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDToggle gen_to_be_invoked = (ND.UI.NDUI.NDToggle)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.graphic);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_inversGraphic(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDToggle gen_to_be_invoked = (ND.UI.NDUI.NDToggle)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.inversGraphic);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_onValueChanged(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDToggle gen_to_be_invoked = (ND.UI.NDUI.NDToggle)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.onValueChanged);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_on(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDToggle gen_to_be_invoked = (ND.UI.NDUI.NDToggle)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.on);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_off(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDToggle gen_to_be_invoked = (ND.UI.NDUI.NDToggle)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.off);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_group(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDToggle gen_to_be_invoked = (ND.UI.NDUI.NDToggle)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.group = (ND.UI.NDUI.NDToggleGroup)translator.GetObject(L, 2, typeof(ND.UI.NDUI.NDToggleGroup));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_isOn(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDToggle gen_to_be_invoked = (ND.UI.NDUI.NDToggle)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.isOn = LuaAPI.lua_toboolean(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_graphic(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDToggle gen_to_be_invoked = (ND.UI.NDUI.NDToggle)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.graphic = (UnityEngine.GameObject)translator.GetObject(L, 2, typeof(UnityEngine.GameObject));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_inversGraphic(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDToggle gen_to_be_invoked = (ND.UI.NDUI.NDToggle)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.inversGraphic = (UnityEngine.GameObject)translator.GetObject(L, 2, typeof(UnityEngine.GameObject));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_onValueChanged(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDToggle gen_to_be_invoked = (ND.UI.NDUI.NDToggle)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.onValueChanged = (ND.UI.NDUI.NDToggle.ToggleEvent)translator.GetObject(L, 2, typeof(ND.UI.NDUI.NDToggle.ToggleEvent));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_on(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDToggle gen_to_be_invoked = (ND.UI.NDUI.NDToggle)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.on = (ND.UI.NDEvents.UnityEvent2)translator.GetObject(L, 2, typeof(ND.UI.NDEvents.UnityEvent2));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_off(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDToggle gen_to_be_invoked = (ND.UI.NDUI.NDToggle)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.off = (ND.UI.NDEvents.UnityEvent2)translator.GetObject(L, 2, typeof(ND.UI.NDEvents.UnityEvent2));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
