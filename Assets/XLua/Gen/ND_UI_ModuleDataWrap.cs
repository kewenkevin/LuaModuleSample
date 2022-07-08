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
    public class NDUIModuleDataWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(ND.UI.ModuleData);
			Utils.BeginObjectRegister(type, L, translator, 0, 1, 3, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Init", _m_Init);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "Label", _g_get_Label);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "PrefabPath", _g_get_PrefabPath);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "UI", _g_get_UI);
            
			
			
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
					
					var gen_ret = new ND.UI.ModuleData();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				if(LuaAPI.lua_gettop(L) == 3 && translator.Assignable<ND.UI.UIExpansion>(L, 2) && translator.Assignable<ND.UI.LinkerConfig>(L, 3))
				{
					ND.UI.UIExpansion _owner = (ND.UI.UIExpansion)translator.GetObject(L, 2, typeof(ND.UI.UIExpansion));
					ND.UI.LinkerConfig _config = (ND.UI.LinkerConfig)translator.GetObject(L, 3, typeof(ND.UI.LinkerConfig));
					
					var gen_ret = new ND.UI.ModuleData(_owner, _config);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				if(LuaAPI.lua_gettop(L) == 3 && translator.Assignable<ND.UI.UIExpansion>(L, 2) && (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING))
				{
					ND.UI.UIExpansion _ui = (ND.UI.UIExpansion)translator.GetObject(L, 2, typeof(ND.UI.UIExpansion));
					string _prefabPath = LuaAPI.lua_tostring(L, 3);
					
					var gen_ret = new ND.UI.ModuleData(_ui, _prefabPath);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to ND.UI.ModuleData constructor!");
            
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstanceNew(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 0)
				{
					
					var gen_ret = new ND.UI.ModuleData();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				if(LuaAPI.lua_gettop(L) == 2 && translator.Assignable<ND.UI.UIExpansion>(L, 1) && translator.Assignable<ND.UI.LinkerConfig>(L, 2))
				{
					ND.UI.UIExpansion _owner = (ND.UI.UIExpansion)translator.GetObject(L, 1, typeof(ND.UI.UIExpansion));
					ND.UI.LinkerConfig _config = (ND.UI.LinkerConfig)translator.GetObject(L, 2, typeof(ND.UI.LinkerConfig));
					
					var gen_ret = new ND.UI.ModuleData(_owner, _config);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				if(LuaAPI.lua_gettop(L) == 2 && translator.Assignable<ND.UI.UIExpansion>(L, 1) && (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING))
				{
					ND.UI.UIExpansion _ui = (ND.UI.UIExpansion)translator.GetObject(L, 1, typeof(ND.UI.UIExpansion));
					string _prefabPath = LuaAPI.lua_tostring(L, 2);
					
					var gen_ret = new ND.UI.ModuleData(_ui, _prefabPath);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to ND.UI.ModuleData constructor!");
            
        }

        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Init(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ND.UI.ModuleData gen_to_be_invoked = (ND.UI.ModuleData)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    ND.UI.UIExpansion _owner = (ND.UI.UIExpansion)translator.GetObject(L, 2, typeof(ND.UI.UIExpansion));
                    ND.UI.LinkerConfig _config = (ND.UI.LinkerConfig)translator.GetObject(L, 3, typeof(ND.UI.LinkerConfig));
                    
                        var gen_ret = gen_to_be_invoked.Init( _owner, _config );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Label(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.ModuleData gen_to_be_invoked = (ND.UI.ModuleData)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushstring(L, gen_to_be_invoked.Label);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_PrefabPath(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.ModuleData gen_to_be_invoked = (ND.UI.ModuleData)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushstring(L, gen_to_be_invoked.PrefabPath);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_UI(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                ND.UI.ModuleData gen_to_be_invoked = (ND.UI.ModuleData)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.UI);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
		
		
		
		
    }
}
