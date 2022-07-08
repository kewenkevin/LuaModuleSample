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
    public class NDUINDUINDDragButtonWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(ND.UI.NDUI.NDDragButton);
			Utils.BeginObjectRegister(type, L, translator, 0, 3, 5, 5);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnDrag", _m_OnDrag);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnBeginDrag", _m_OnBeginDrag);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnEndDrag", _m_OnEndDrag);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "onDrag", _g_get_onDrag);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "onDragOut", _g_get_onDragOut);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "onDragBegin", _g_get_onDragBegin);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "onDragEnd", _g_get_onDragEnd);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "onDragOutEnd", _g_get_onDragOutEnd);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "onDrag", _s_set_onDrag);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "onDragOut", _s_set_onDragOut);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "onDragBegin", _s_set_onDragBegin);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "onDragEnd", _s_set_onDragEnd);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "onDragOutEnd", _s_set_onDragOutEnd);
            
			
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
					
					var gen_ret = new ND.UI.NDUI.NDDragButton();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to ND.UI.NDUI.NDDragButton constructor!");
            
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstanceNew(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 0)
				{
					
					var gen_ret = new ND.UI.NDUI.NDDragButton();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to ND.UI.NDUI.NDDragButton constructor!");
            
        }

        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnDrag(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.NDUI.NDDragButton gen_to_be_invoked = (ND.UI.NDUI.NDDragButton)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.EventSystems.PointerEventData _eventData = (UnityEngine.EventSystems.PointerEventData)translator.GetObject(L, 2, typeof(UnityEngine.EventSystems.PointerEventData));
                    
                    gen_to_be_invoked.OnDrag( _eventData );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnBeginDrag(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.NDUI.NDDragButton gen_to_be_invoked = (ND.UI.NDUI.NDDragButton)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.EventSystems.PointerEventData _eventData = (UnityEngine.EventSystems.PointerEventData)translator.GetObject(L, 2, typeof(UnityEngine.EventSystems.PointerEventData));
                    
                    gen_to_be_invoked.OnBeginDrag( _eventData );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnEndDrag(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.NDUI.NDDragButton gen_to_be_invoked = (ND.UI.NDUI.NDDragButton)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.EventSystems.PointerEventData _eventData = (UnityEngine.EventSystems.PointerEventData)translator.GetObject(L, 2, typeof(UnityEngine.EventSystems.PointerEventData));
                    
                    gen_to_be_invoked.OnEndDrag( _eventData );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_onDrag(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDDragButton gen_to_be_invoked = (ND.UI.NDUI.NDDragButton)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.onDrag);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_onDragOut(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDDragButton gen_to_be_invoked = (ND.UI.NDUI.NDDragButton)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.onDragOut);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_onDragBegin(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDDragButton gen_to_be_invoked = (ND.UI.NDUI.NDDragButton)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.onDragBegin);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_onDragEnd(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDDragButton gen_to_be_invoked = (ND.UI.NDUI.NDDragButton)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.onDragEnd);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_onDragOutEnd(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDDragButton gen_to_be_invoked = (ND.UI.NDUI.NDDragButton)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.onDragOutEnd);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_onDrag(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDDragButton gen_to_be_invoked = (ND.UI.NDUI.NDDragButton)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.onDrag = (ND.UI.NDUI.NDDragButton.ButtonDragEvent)translator.GetObject(L, 2, typeof(ND.UI.NDUI.NDDragButton.ButtonDragEvent));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_onDragOut(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDDragButton gen_to_be_invoked = (ND.UI.NDUI.NDDragButton)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.onDragOut = (ND.UI.NDUI.NDDragButton.ButtonDragEvent)translator.GetObject(L, 2, typeof(ND.UI.NDUI.NDDragButton.ButtonDragEvent));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_onDragBegin(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDDragButton gen_to_be_invoked = (ND.UI.NDUI.NDDragButton)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.onDragBegin = (ND.UI.NDUI.NDDragButton.ButtonDragEvent)translator.GetObject(L, 2, typeof(ND.UI.NDUI.NDDragButton.ButtonDragEvent));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_onDragEnd(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDDragButton gen_to_be_invoked = (ND.UI.NDUI.NDDragButton)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.onDragEnd = (ND.UI.NDUI.NDDragButton.ButtonDragEvent)translator.GetObject(L, 2, typeof(ND.UI.NDUI.NDDragButton.ButtonDragEvent));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_onDragOutEnd(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDDragButton gen_to_be_invoked = (ND.UI.NDUI.NDDragButton)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.onDragOutEnd = (ND.UI.NDUI.NDDragButton.ButtonDragEvent)translator.GetObject(L, 2, typeof(ND.UI.NDUI.NDDragButton.ButtonDragEvent));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
