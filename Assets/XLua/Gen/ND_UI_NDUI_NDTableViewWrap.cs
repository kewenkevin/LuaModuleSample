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
    public class NDUINDUINDTableViewWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(ND.UI.NDUI.NDTableView);
			Utils.BeginObjectRegister(type, L, translator, 0, 1, 2, 2);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "CheckLayout", _m_CheckLayout);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "getCellSizeDelegate", _g_get_getCellSizeDelegate);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "getCellReuseIdentifierDelegate", _g_get_getCellReuseIdentifierDelegate);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "getCellSizeDelegate", _s_set_getCellSizeDelegate);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "getCellReuseIdentifierDelegate", _s_set_getCellReuseIdentifierDelegate);
            
			
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
					
					var gen_ret = new ND.UI.NDUI.NDTableView();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to ND.UI.NDUI.NDTableView constructor!");
            
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstanceNew(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 0)
				{
					
					var gen_ret = new ND.UI.NDUI.NDTableView();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to ND.UI.NDUI.NDTableView constructor!");
            
        }

        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CheckLayout(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.NDUI.NDTableView gen_to_be_invoked = (ND.UI.NDUI.NDTableView)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.CheckLayout(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_getCellSizeDelegate(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDTableView gen_to_be_invoked = (ND.UI.NDUI.NDTableView)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.getCellSizeDelegate);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_getCellReuseIdentifierDelegate(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDTableView gen_to_be_invoked = (ND.UI.NDUI.NDTableView)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.getCellReuseIdentifierDelegate);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_getCellSizeDelegate(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDTableView gen_to_be_invoked = (ND.UI.NDUI.NDTableView)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.getCellSizeDelegate = translator.GetDelegate<ND.UI.NDUI.GetSizeForCellDelegate>(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_getCellReuseIdentifierDelegate(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.NDUI.NDTableView gen_to_be_invoked = (ND.UI.NDUI.NDTableView)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.getCellReuseIdentifierDelegate = translator.GetDelegate<ND.UI.NDUI.GetCellReuseIdentifierDelegate>(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
