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
    public class NDUINDUINDRichTextWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(ND.UI.NDUI.NDRichText);
			Utils.BeginObjectRegister(type, L, translator, 0, 2, 5, 5);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "DrawTag", _m_DrawTag);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnPointerClick", _m_OnPointerClick);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "onClick", _g_get_onClick);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "lineAlignment", _g_get_lineAlignment);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "richText", _g_get_richText);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnGetPrefab", _g_get_OnGetPrefab);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnReleasePrefab", _g_get_OnReleasePrefab);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "onClick", _s_set_onClick);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "lineAlignment", _s_set_lineAlignment);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "richText", _s_set_richText);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnGetPrefab", _s_set_OnGetPrefab);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnReleasePrefab", _s_set_OnReleasePrefab);
            
			
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
					
					var gen_ret = new ND.UI.NDUI.NDRichText();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to ND.UI.NDUI.NDRichText constructor!");
            
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstanceNew(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 0)
				{
					
					var gen_ret = new ND.UI.NDUI.NDRichText();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to ND.UI.NDUI.NDRichText constructor!");
            
        }

        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DrawTag(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.NDUI.NDRichText gen_to_be_invoked = (ND.UI.NDUI.NDRichText)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.DrawTag(  );
                    
                    
                    
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
            
            
                ND.UI.NDUI.NDRichText gen_to_be_invoked = (ND.UI.NDUI.NDRichText)translator.FastGetCSObj(L, 1);
            
            
                
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
        static int _g_get_onClick(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRichText gen_to_be_invoked = (ND.UI.NDUI.NDRichText)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.onClick);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_lineAlignment(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRichText gen_to_be_invoked = (ND.UI.NDUI.NDRichText)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.lineAlignment);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_richText(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRichText gen_to_be_invoked = (ND.UI.NDUI.NDRichText)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushstring(L, gen_to_be_invoked.richText);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnGetPrefab(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRichText gen_to_be_invoked = (ND.UI.NDUI.NDRichText)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnGetPrefab);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnReleasePrefab(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRichText gen_to_be_invoked = (ND.UI.NDUI.NDRichText)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnReleasePrefab);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_onClick(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRichText gen_to_be_invoked = (ND.UI.NDUI.NDRichText)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.onClick = (ND.UI.NDUI.NDRichText.RichTextHrefEvent)translator.GetObject(L, 2, typeof(ND.UI.NDUI.NDRichText.RichTextHrefEvent));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_lineAlignment(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRichText gen_to_be_invoked = (ND.UI.NDUI.NDRichText)translator.FastGetCSObj(L, 1);
                ND.UI.NDUI.NDRichText.LineAlignment gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.lineAlignment = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_richText(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRichText gen_to_be_invoked = (ND.UI.NDUI.NDRichText)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.richText = LuaAPI.lua_tostring(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnGetPrefab(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRichText gen_to_be_invoked = (ND.UI.NDUI.NDRichText)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnGetPrefab = translator.GetDelegate<ND.UI.NDUI.NDRichText.GetPrefab>(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnReleasePrefab(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDRichText gen_to_be_invoked = (ND.UI.NDUI.NDRichText)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnReleasePrefab = translator.GetDelegate<ND.UI.NDUI.NDRichText.ReleasePrefab>(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
