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
    public class ShawnFrameworkShawHotUpdateAssetBundleMgrWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(ShawnFramework.ShawHotUpdate.AssetBundleMgr);
			Utils.BeginObjectRegister(type, L, translator, 0, 2, 0, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "UnLoadAB", _m_UnLoadAB);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ClearAB", _m_ClearAB);
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 2, 4, 2);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "InitManager", _m_InitManager_xlua_st_);
            
			
            
			Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "Instance", _g_get_Instance);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "RootFolderName", _g_get_RootFolderName);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "SteamingAssetsPath", _g_get_SteamingAssetsPath);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "PersistentDataPath", _g_get_PersistentDataPath);
            
			Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "SteamingAssetsPath", _s_set_SteamingAssetsPath);
            Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "PersistentDataPath", _s_set_PersistentDataPath);
            
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					var gen_ret = new ShawnFramework.ShawHotUpdate.AssetBundleMgr();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to ShawnFramework.ShawHotUpdate.AssetBundleMgr constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_InitManager_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.MonoBehaviour _mono = (UnityEngine.MonoBehaviour)translator.GetObject(L, 1, typeof(UnityEngine.MonoBehaviour));
                    
                    ShawnFramework.ShawHotUpdate.AssetBundleMgr.InitManager( _mono );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UnLoadAB(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ShawnFramework.ShawHotUpdate.AssetBundleMgr gen_to_be_invoked = (ShawnFramework.ShawHotUpdate.AssetBundleMgr)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _name = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.UnLoadAB( _name );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ClearAB(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                ShawnFramework.ShawHotUpdate.AssetBundleMgr gen_to_be_invoked = (ShawnFramework.ShawHotUpdate.AssetBundleMgr)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.ClearAB(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Instance(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, ShawnFramework.ShawHotUpdate.AssetBundleMgr.Instance);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_RootFolderName(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushstring(L, ShawnFramework.ShawHotUpdate.AssetBundleMgr.RootFolderName);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_SteamingAssetsPath(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushstring(L, ShawnFramework.ShawHotUpdate.AssetBundleMgr.SteamingAssetsPath);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_PersistentDataPath(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushstring(L, ShawnFramework.ShawHotUpdate.AssetBundleMgr.PersistentDataPath);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_SteamingAssetsPath(RealStatePtr L)
        {
		    try {
                
			    ShawnFramework.ShawHotUpdate.AssetBundleMgr.SteamingAssetsPath = LuaAPI.lua_tostring(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_PersistentDataPath(RealStatePtr L)
        {
		    try {
                
			    ShawnFramework.ShawHotUpdate.AssetBundleMgr.PersistentDataPath = LuaAPI.lua_tostring(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
