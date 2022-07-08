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
    public class NDManagersResourceMgrRuntimeNFUResourceWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(ND.Managers.ResourceMgr.Runtime.NFUResource);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 23, 5, 1);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "Initialize", _m_Initialize_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "HotUpdateStart", _m_HotUpdateStart_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Shutdown", _m_Shutdown_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "HasAsset", _m_HasAsset_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetResourceName", _m_GetResourceName_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetResourceFullName", _m_GetResourceFullName_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetResourceFileName", _m_GetResourceFileName_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetResourcePath", _m_GetResourcePath_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "InstantiateAsync", _m_InstantiateAsync_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadAssetAsync", _m_LoadAssetAsync_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadAssetImmediate", _m_LoadAssetImmediate_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Release", _m_Release_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadBinaryFromFileSystem", _m_LoadBinaryFromFileSystem_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadSceneAsset", _m_LoadSceneAsset_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "UnloadSceneAsset", _m_UnloadSceneAsset_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "UnloadScene", _m_UnloadScene_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetBinaryPath", _m_GetBinaryPath_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetBinaryLength", _m_GetBinaryLength_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadBinary", _m_LoadBinary_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetResourceAllAssets", _m_GetResourceAllAssets_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetAllSubAssets", _m_GetAllSubAssets_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetSubAssets", _m_GetSubAssets_xlua_st_);
            
			
            
			Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "Resource", _g_get_Resource);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "WebRequest", _g_get_WebRequest);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "Initializated", _g_get_Initializated);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "isEditorMode", _g_get_isEditorMode);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "currentVariant", _g_get_currentVariant);
            
			Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "currentVariant", _s_set_currentVariant);
            
			
            Utils.RegisterFunc(L, Utils.CLS_IDX, "New", __CreateInstanceNew);
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "ND.Managers.ResourceMgr.Runtime.NFUResource does not have a constructor!");
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstanceNew(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "ND.Managers.ResourceMgr.Runtime.NFUResource does not have a constructor!");
        }

        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Initialize_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    ND.Managers.ResourceMgr.Runtime.NFUResource.Initialize(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_HotUpdateStart_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                    ND.Managers.ResourceMgr.Runtime.NFUResource.HotUpdateStart(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Shutdown_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    ND.Managers.ResourceMgr.Runtime.ShutdownType _type;translator.Get(L, 1, out _type);
                    
                    ND.Managers.ResourceMgr.Runtime.NFUResource.Shutdown( _type );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_HasAsset_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _assetName = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = ND.Managers.ResourceMgr.Runtime.NFUResource.HasAsset( _assetName );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetResourceName_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _assetName = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = ND.Managers.ResourceMgr.Runtime.NFUResource.GetResourceName( _assetName );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetResourceFullName_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _assetName = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = ND.Managers.ResourceMgr.Runtime.NFUResource.GetResourceFullName( _assetName );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetResourceFileName_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _assetName = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = ND.Managers.ResourceMgr.Runtime.NFUResource.GetResourceFileName( _assetName );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetResourcePath_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _assetName = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = ND.Managers.ResourceMgr.Runtime.NFUResource.GetResourcePath( _assetName );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_InstantiateAsync_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<ND.Managers.ResourceMgr.Framework.Resource.LoadAssetSuccessCallback>(L, 2)&& translator.Assignable<ND.Managers.ResourceMgr.Framework.Resource.LoadAssetFailureCallback>(L, 3)) 
                {
                    string _assetName = LuaAPI.lua_tostring(L, 1);
                    ND.Managers.ResourceMgr.Framework.Resource.LoadAssetSuccessCallback _loadAssetSuccessCallback = translator.GetDelegate<ND.Managers.ResourceMgr.Framework.Resource.LoadAssetSuccessCallback>(L, 2);
                    ND.Managers.ResourceMgr.Framework.Resource.LoadAssetFailureCallback _loadAssetFailureCallback = translator.GetDelegate<ND.Managers.ResourceMgr.Framework.Resource.LoadAssetFailureCallback>(L, 3);
                    
                        var gen_ret = ND.Managers.ResourceMgr.Runtime.NFUResource.InstantiateAsync( _assetName, _loadAssetSuccessCallback, _loadAssetFailureCallback );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<ND.Managers.ResourceMgr.Framework.Resource.LoadAssetSuccessCallback>(L, 2)) 
                {
                    string _assetName = LuaAPI.lua_tostring(L, 1);
                    ND.Managers.ResourceMgr.Framework.Resource.LoadAssetSuccessCallback _loadAssetSuccessCallback = translator.GetDelegate<ND.Managers.ResourceMgr.Framework.Resource.LoadAssetSuccessCallback>(L, 2);
                    
                        var gen_ret = ND.Managers.ResourceMgr.Runtime.NFUResource.InstantiateAsync( _assetName, _loadAssetSuccessCallback );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string _assetName = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = ND.Managers.ResourceMgr.Runtime.NFUResource.InstantiateAsync( _assetName );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ND.Managers.ResourceMgr.Runtime.NFUResource.InstantiateAsync!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadAssetAsync_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 5&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Type>(L, 2)&& translator.Assignable<ND.Managers.ResourceMgr.Framework.Resource.LoadAssetSuccessCallback>(L, 3)&& translator.Assignable<ND.Managers.ResourceMgr.Framework.Resource.LoadAssetFailureCallback>(L, 4)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 5)) 
                {
                    string _assetName = LuaAPI.lua_tostring(L, 1);
                    System.Type _assetType = (System.Type)translator.GetObject(L, 2, typeof(System.Type));
                    ND.Managers.ResourceMgr.Framework.Resource.LoadAssetSuccessCallback _loadAssetSuccessCallback = translator.GetDelegate<ND.Managers.ResourceMgr.Framework.Resource.LoadAssetSuccessCallback>(L, 3);
                    ND.Managers.ResourceMgr.Framework.Resource.LoadAssetFailureCallback _loadAssetFailureCallback = translator.GetDelegate<ND.Managers.ResourceMgr.Framework.Resource.LoadAssetFailureCallback>(L, 4);
                    int _priority = LuaAPI.xlua_tointeger(L, 5);
                    
                        var gen_ret = ND.Managers.ResourceMgr.Runtime.NFUResource.LoadAssetAsync( _assetName, _assetType, _loadAssetSuccessCallback, _loadAssetFailureCallback, _priority );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Type>(L, 2)&& translator.Assignable<ND.Managers.ResourceMgr.Framework.Resource.LoadAssetSuccessCallback>(L, 3)&& translator.Assignable<ND.Managers.ResourceMgr.Framework.Resource.LoadAssetFailureCallback>(L, 4)) 
                {
                    string _assetName = LuaAPI.lua_tostring(L, 1);
                    System.Type _assetType = (System.Type)translator.GetObject(L, 2, typeof(System.Type));
                    ND.Managers.ResourceMgr.Framework.Resource.LoadAssetSuccessCallback _loadAssetSuccessCallback = translator.GetDelegate<ND.Managers.ResourceMgr.Framework.Resource.LoadAssetSuccessCallback>(L, 3);
                    ND.Managers.ResourceMgr.Framework.Resource.LoadAssetFailureCallback _loadAssetFailureCallback = translator.GetDelegate<ND.Managers.ResourceMgr.Framework.Resource.LoadAssetFailureCallback>(L, 4);
                    
                        var gen_ret = ND.Managers.ResourceMgr.Runtime.NFUResource.LoadAssetAsync( _assetName, _assetType, _loadAssetSuccessCallback, _loadAssetFailureCallback );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Type>(L, 2)&& translator.Assignable<ND.Managers.ResourceMgr.Framework.Resource.LoadAssetSuccessCallback>(L, 3)) 
                {
                    string _assetName = LuaAPI.lua_tostring(L, 1);
                    System.Type _assetType = (System.Type)translator.GetObject(L, 2, typeof(System.Type));
                    ND.Managers.ResourceMgr.Framework.Resource.LoadAssetSuccessCallback _loadAssetSuccessCallback = translator.GetDelegate<ND.Managers.ResourceMgr.Framework.Resource.LoadAssetSuccessCallback>(L, 3);
                    
                        var gen_ret = ND.Managers.ResourceMgr.Runtime.NFUResource.LoadAssetAsync( _assetName, _assetType, _loadAssetSuccessCallback );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Type>(L, 2)) 
                {
                    string _assetName = LuaAPI.lua_tostring(L, 1);
                    System.Type _assetType = (System.Type)translator.GetObject(L, 2, typeof(System.Type));
                    
                        var gen_ret = ND.Managers.ResourceMgr.Runtime.NFUResource.LoadAssetAsync( _assetName, _assetType );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ND.Managers.ResourceMgr.Runtime.NFUResource.LoadAssetAsync!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadAssetImmediate_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _assetName = LuaAPI.lua_tostring(L, 1);
                    System.Type _assetType = (System.Type)translator.GetObject(L, 2, typeof(System.Type));
                    
                        var gen_ret = ND.Managers.ResourceMgr.Runtime.NFUResource.LoadAssetImmediate( _assetName, _assetType );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Release_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 1&& translator.Assignable<object>(L, 1)) 
                {
                    object _asset = translator.GetObject(L, 1, typeof(object));
                    
                    ND.Managers.ResourceMgr.Runtime.NFUResource.Release( _asset );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Type>(L, 2)) 
                {
                    string _assetName = LuaAPI.lua_tostring(L, 1);
                    System.Type _assetType = (System.Type)translator.GetObject(L, 2, typeof(System.Type));
                    
                    ND.Managers.ResourceMgr.Runtime.NFUResource.Release( _assetName, _assetType );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string _assetName = LuaAPI.lua_tostring(L, 1);
                    
                    ND.Managers.ResourceMgr.Runtime.NFUResource.Release( _assetName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ND.Managers.ResourceMgr.Runtime.NFUResource.Release!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadBinaryFromFileSystem_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _binaryAssetName = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = ND.Managers.ResourceMgr.Runtime.NFUResource.LoadBinaryFromFileSystem( _binaryAssetName );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadSceneAsset_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 4&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<ND.Managers.ResourceMgr.Framework.Resource.LoadSceneCallbacks>(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& translator.Assignable<UnityEngine.Object>(L, 4)) 
                {
                    string _sceneAssetName = LuaAPI.lua_tostring(L, 1);
                    ND.Managers.ResourceMgr.Framework.Resource.LoadSceneCallbacks _loadSceneCallbacks = (ND.Managers.ResourceMgr.Framework.Resource.LoadSceneCallbacks)translator.GetObject(L, 2, typeof(ND.Managers.ResourceMgr.Framework.Resource.LoadSceneCallbacks));
                    int _priority = LuaAPI.xlua_tointeger(L, 3);
                    UnityEngine.Object _userData = (UnityEngine.Object)translator.GetObject(L, 4, typeof(UnityEngine.Object));
                    
                    ND.Managers.ResourceMgr.Runtime.NFUResource.LoadSceneAsset( _sceneAssetName, _loadSceneCallbacks, _priority, _userData );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<ND.Managers.ResourceMgr.Framework.Resource.LoadSceneCallbacks>(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    string _sceneAssetName = LuaAPI.lua_tostring(L, 1);
                    ND.Managers.ResourceMgr.Framework.Resource.LoadSceneCallbacks _loadSceneCallbacks = (ND.Managers.ResourceMgr.Framework.Resource.LoadSceneCallbacks)translator.GetObject(L, 2, typeof(ND.Managers.ResourceMgr.Framework.Resource.LoadSceneCallbacks));
                    int _priority = LuaAPI.xlua_tointeger(L, 3);
                    
                    ND.Managers.ResourceMgr.Runtime.NFUResource.LoadSceneAsset( _sceneAssetName, _loadSceneCallbacks, _priority );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<ND.Managers.ResourceMgr.Framework.Resource.LoadSceneCallbacks>(L, 2)) 
                {
                    string _sceneAssetName = LuaAPI.lua_tostring(L, 1);
                    ND.Managers.ResourceMgr.Framework.Resource.LoadSceneCallbacks _loadSceneCallbacks = (ND.Managers.ResourceMgr.Framework.Resource.LoadSceneCallbacks)translator.GetObject(L, 2, typeof(ND.Managers.ResourceMgr.Framework.Resource.LoadSceneCallbacks));
                    
                    ND.Managers.ResourceMgr.Runtime.NFUResource.LoadSceneAsset( _sceneAssetName, _loadSceneCallbacks );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string _sceneAssetName = LuaAPI.lua_tostring(L, 1);
                    
                    ND.Managers.ResourceMgr.Runtime.NFUResource.LoadSceneAsset( _sceneAssetName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ND.Managers.ResourceMgr.Runtime.NFUResource.LoadSceneAsset!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UnloadSceneAsset_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _sceneAssetName = LuaAPI.lua_tostring(L, 1);
                    ND.Managers.ResourceMgr.Framework.Resource.UnloadSceneCallbacks _unloadSceneCallbacks = (ND.Managers.ResourceMgr.Framework.Resource.UnloadSceneCallbacks)translator.GetObject(L, 2, typeof(ND.Managers.ResourceMgr.Framework.Resource.UnloadSceneCallbacks));
                    
                    ND.Managers.ResourceMgr.Runtime.NFUResource.UnloadSceneAsset( _sceneAssetName, _unloadSceneCallbacks );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UnloadScene_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _sceneAssetName = LuaAPI.lua_tostring(L, 1);
                    ND.Managers.ResourceMgr.Framework.Resource.UnloadSceneCallbacks _unloadSceneCallbacks = (ND.Managers.ResourceMgr.Framework.Resource.UnloadSceneCallbacks)translator.GetObject(L, 2, typeof(ND.Managers.ResourceMgr.Framework.Resource.UnloadSceneCallbacks));
                    object _userData = translator.GetObject(L, 3, typeof(object));
                    
                    ND.Managers.ResourceMgr.Runtime.NFUResource.UnloadScene( _sceneAssetName, _unloadSceneCallbacks, _userData );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetBinaryPath_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string _binaryAssetName = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = ND.Managers.ResourceMgr.Runtime.NFUResource.GetBinaryPath( _binaryAssetName );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string _binaryAssetName = LuaAPI.lua_tostring(L, 1);
                    bool _storageInReadOnly;
                    bool _storageInFileSystem;
                    string _relativePath;
                    string _fileName;
                    
                        var gen_ret = ND.Managers.ResourceMgr.Runtime.NFUResource.GetBinaryPath( _binaryAssetName, out _storageInReadOnly, out _storageInFileSystem, out _relativePath, out _fileName );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    LuaAPI.lua_pushboolean(L, _storageInReadOnly);
                        
                    LuaAPI.lua_pushboolean(L, _storageInFileSystem);
                        
                    LuaAPI.lua_pushstring(L, _relativePath);
                        
                    LuaAPI.lua_pushstring(L, _fileName);
                        
                    
                    
                    
                    return 5;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ND.Managers.ResourceMgr.Runtime.NFUResource.GetBinaryPath!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetBinaryLength_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _binaryAssetName = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = ND.Managers.ResourceMgr.Runtime.NFUResource.GetBinaryLength( _binaryAssetName );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadBinary_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<ND.Managers.ResourceMgr.Framework.Resource.LoadBinaryCallbacks>(L, 2)) 
                {
                    string _binaryAssetName = LuaAPI.lua_tostring(L, 1);
                    ND.Managers.ResourceMgr.Framework.Resource.LoadBinaryCallbacks _loadBinaryCallbacks = (ND.Managers.ResourceMgr.Framework.Resource.LoadBinaryCallbacks)translator.GetObject(L, 2, typeof(ND.Managers.ResourceMgr.Framework.Resource.LoadBinaryCallbacks));
                    
                    ND.Managers.ResourceMgr.Runtime.NFUResource.LoadBinary( _binaryAssetName, _loadBinaryCallbacks );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<ND.Managers.ResourceMgr.Framework.Resource.LoadBinaryCallbacks>(L, 2)&& translator.Assignable<object>(L, 3)) 
                {
                    string _binaryAssetName = LuaAPI.lua_tostring(L, 1);
                    ND.Managers.ResourceMgr.Framework.Resource.LoadBinaryCallbacks _loadBinaryCallbacks = (ND.Managers.ResourceMgr.Framework.Resource.LoadBinaryCallbacks)translator.GetObject(L, 2, typeof(ND.Managers.ResourceMgr.Framework.Resource.LoadBinaryCallbacks));
                    object _userData = translator.GetObject(L, 3, typeof(object));
                    
                    ND.Managers.ResourceMgr.Runtime.NFUResource.LoadBinary( _binaryAssetName, _loadBinaryCallbacks, _userData );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to ND.Managers.ResourceMgr.Runtime.NFUResource.LoadBinary!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetResourceAllAssets_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _resourceName = LuaAPI.lua_tostring(L, 1);
                    
                        var gen_ret = ND.Managers.ResourceMgr.Runtime.NFUResource.GetResourceAllAssets( _resourceName );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetAllSubAssets_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    object _rootAsset = translator.GetObject(L, 1, typeof(object));
                    
                        var gen_ret = ND.Managers.ResourceMgr.Runtime.NFUResource.GetAllSubAssets( _rootAsset );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetSubAssets_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    object _rootAsset = translator.GetObject(L, 1, typeof(object));
                    System.Type _type = (System.Type)translator.GetObject(L, 2, typeof(System.Type));
                    
                        var gen_ret = ND.Managers.ResourceMgr.Runtime.NFUResource.GetSubAssets( _rootAsset, _type );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Resource(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, ND.Managers.ResourceMgr.Runtime.NFUResource.Resource);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_WebRequest(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, ND.Managers.ResourceMgr.Runtime.NFUResource.WebRequest);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Initializated(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushboolean(L, ND.Managers.ResourceMgr.Runtime.NFUResource.Initializated);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_isEditorMode(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushboolean(L, ND.Managers.ResourceMgr.Runtime.NFUResource.isEditorMode);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_currentVariant(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushstring(L, ND.Managers.ResourceMgr.Runtime.NFUResource.currentVariant);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_currentVariant(RealStatePtr L)
        {
		    try {
                
			    ND.Managers.ResourceMgr.Runtime.NFUResource.currentVariant = LuaAPI.lua_tostring(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
