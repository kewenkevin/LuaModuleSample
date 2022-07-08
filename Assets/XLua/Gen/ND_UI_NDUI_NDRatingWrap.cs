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
    public class NDUINDUINDRatingWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(ND.UI.NDUI.NDRating);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 8, 7);
			
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "backgroundImage", _g_get_backgroundImage);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "highlightImage", _g_get_highlightImage);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "background", _g_get_background);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "highlight", _g_get_highlight);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "spacing", _g_get_spacing);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "total", _g_get_total);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "canModifyTotalByCode", _g_get_canModifyTotalByCode);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "current", _g_get_current);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "backgroundImage", _s_set_backgroundImage);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "highlightImage", _s_set_highlightImage);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "background", _s_set_background);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "highlight", _s_set_highlight);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "spacing", _s_set_spacing);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "total", _s_set_total);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "current", _s_set_current);
            
			
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
					
					var gen_ret = new ND.UI.NDUI.NDRating();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to ND.UI.NDUI.NDRating constructor!");
            
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstanceNew(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 0)
				{
					
					var gen_ret = new ND.UI.NDUI.NDRating();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to ND.UI.NDUI.NDRating constructor!");
            
        }

        
		
        
		
        
        
        
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_backgroundImage(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRating gen_to_be_invoked = (ND.UI.NDUI.NDRating)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.backgroundImage);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_highlightImage(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRating gen_to_be_invoked = (ND.UI.NDUI.NDRating)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.highlightImage);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_background(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRating gen_to_be_invoked = (ND.UI.NDUI.NDRating)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.background);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_highlight(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRating gen_to_be_invoked = (ND.UI.NDUI.NDRating)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.highlight);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_spacing(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRating gen_to_be_invoked = (ND.UI.NDUI.NDRating)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushnumber(L, gen_to_be_invoked.spacing);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_total(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRating gen_to_be_invoked = (ND.UI.NDUI.NDRating)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.total);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_canModifyTotalByCode(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRating gen_to_be_invoked = (ND.UI.NDUI.NDRating)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.canModifyTotalByCode);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_current(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRating gen_to_be_invoked = (ND.UI.NDUI.NDRating)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushnumber(L, gen_to_be_invoked.current);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_backgroundImage(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRating gen_to_be_invoked = (ND.UI.NDUI.NDRating)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.backgroundImage = (UnityEngine.Sprite)translator.GetObject(L, 2, typeof(UnityEngine.Sprite));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_highlightImage(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRating gen_to_be_invoked = (ND.UI.NDUI.NDRating)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.highlightImage = (UnityEngine.Sprite)translator.GetObject(L, 2, typeof(UnityEngine.Sprite));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_background(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRating gen_to_be_invoked = (ND.UI.NDUI.NDRating)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.background = (UnityEngine.UI.HorizontalOrVerticalLayoutGroup)translator.GetObject(L, 2, typeof(UnityEngine.UI.HorizontalOrVerticalLayoutGroup));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_highlight(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRating gen_to_be_invoked = (ND.UI.NDUI.NDRating)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.highlight = (UnityEngine.UI.HorizontalOrVerticalLayoutGroup)translator.GetObject(L, 2, typeof(UnityEngine.UI.HorizontalOrVerticalLayoutGroup));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_spacing(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRating gen_to_be_invoked = (ND.UI.NDUI.NDRating)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.spacing = (float)LuaAPI.lua_tonumber(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_total(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRating gen_to_be_invoked = (ND.UI.NDUI.NDRating)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.total = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_current(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRating gen_to_be_invoked = (ND.UI.NDUI.NDRating)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.current = (float)LuaAPI.lua_tonumber(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
