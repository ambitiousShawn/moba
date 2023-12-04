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
    public class LauncherWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(Launcher);
			Utils.BeginObjectRegister(type, L, translator, 0, 1, 9, 8);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddMonoTimer", _m_AddMonoTimer);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "UserData", _g_get_UserData);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "RoomID", _g_get_RoomID);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "MapID", _g_get_MapID);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "BattleHeroDatas", _g_get_BattleHeroDatas);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "SelfIndex", _g_get_SelfIndex);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "AssetsSvc", _g_get_AssetsSvc);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "UIRoot", _g_get_UIRoot);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "EnableHotUpdate", _g_get_EnableHotUpdate);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "EnableSoldier", _g_get_EnableSoldier);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "UserData", _s_set_UserData);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "RoomID", _s_set_RoomID);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "MapID", _s_set_MapID);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "BattleHeroDatas", _s_set_BattleHeroDatas);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "SelfIndex", _s_set_SelfIndex);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "UIRoot", _s_set_UIRoot);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "EnableHotUpdate", _s_set_EnableHotUpdate);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "EnableSoldier", _s_set_EnableSoldier);
            
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 1, 1);
			
			
            
			Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "Instance", _g_get_Instance);
            
			Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "Instance", _s_set_Instance);
            
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					var gen_ret = new Launcher();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to Launcher constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddMonoTimer(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Launcher gen_to_be_invoked = (Launcher)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    MonoTimer _timer = (MonoTimer)translator.GetObject(L, 2, typeof(MonoTimer));
                    
                    gen_to_be_invoked.AddMonoTimer( _timer );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_UserData(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Launcher gen_to_be_invoked = (Launcher)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.UserData);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_RoomID(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Launcher gen_to_be_invoked = (Launcher)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushuint(L, gen_to_be_invoked.RoomID);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_MapID(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Launcher gen_to_be_invoked = (Launcher)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.MapID);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_BattleHeroDatas(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Launcher gen_to_be_invoked = (Launcher)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.BattleHeroDatas);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_SelfIndex(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Launcher gen_to_be_invoked = (Launcher)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.SelfIndex);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_AssetsSvc(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Launcher gen_to_be_invoked = (Launcher)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.AssetsSvc);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Instance(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, Launcher.Instance);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_UIRoot(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Launcher gen_to_be_invoked = (Launcher)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.UIRoot);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_EnableHotUpdate(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Launcher gen_to_be_invoked = (Launcher)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.EnableHotUpdate);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_EnableSoldier(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Launcher gen_to_be_invoked = (Launcher)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.EnableSoldier);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_UserData(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Launcher gen_to_be_invoked = (Launcher)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.UserData = (GameProtocol.UserData)translator.GetObject(L, 2, typeof(GameProtocol.UserData));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_RoomID(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Launcher gen_to_be_invoked = (Launcher)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.RoomID = LuaAPI.xlua_touint(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_MapID(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Launcher gen_to_be_invoked = (Launcher)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.MapID = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_BattleHeroDatas(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Launcher gen_to_be_invoked = (Launcher)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.BattleHeroDatas = (System.Collections.Generic.List<GameProtocol.BattleHeroData>)translator.GetObject(L, 2, typeof(System.Collections.Generic.List<GameProtocol.BattleHeroData>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_SelfIndex(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Launcher gen_to_be_invoked = (Launcher)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.SelfIndex = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Instance(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    Launcher.Instance = (Launcher)translator.GetObject(L, 1, typeof(Launcher));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_UIRoot(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Launcher gen_to_be_invoked = (Launcher)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.UIRoot = (UnityEngine.Transform)translator.GetObject(L, 2, typeof(UnityEngine.Transform));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_EnableHotUpdate(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Launcher gen_to_be_invoked = (Launcher)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.EnableHotUpdate = LuaAPI.lua_toboolean(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_EnableSoldier(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Launcher gen_to_be_invoked = (Launcher)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.EnableSoldier = LuaAPI.lua_toboolean(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
