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
    public class FightManagerWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(FightManager);
			Utils.BeginObjectRegister(type, L, translator, 0, 11, 4, 3);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "InitAll", _m_InitAll);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "InitCamera", _m_InitCamera);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Tick", _m_Tick);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "InputKey", _m_InputKey);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SendMoveOperation", _m_SendMoveOperation);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SendSkillOperation", _m_SendSkillOperation);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "EnterCDState", _m_EnterCDState);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "UnInit", _m_UnInit);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetSelfHero", _m_GetSelfHero);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetAllEnvColliders", _m_GetAllEnvColliders);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddBullet", _m_AddBullet);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "KeyID", _g_get_KeyID);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "playWnd", _g_get_playWnd);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "SkillDisMultipler", _g_get_SkillDisMultipler);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "transEnvRoot", _g_get_transEnvRoot);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "playWnd", _s_set_playWnd);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "SkillDisMultipler", _s_set_SkillDisMultipler);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "transEnvRoot", _s_set_transEnvRoot);
            
			
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
					
					var gen_ret = new FightManager();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to FightManager constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_InitAll(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                FightManager gen_to_be_invoked = (FightManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    System.Collections.Generic.List<GameProtocol.BattleHeroData> _battleHeroDatas = (System.Collections.Generic.List<GameProtocol.BattleHeroData>)translator.GetObject(L, 2, typeof(System.Collections.Generic.List<GameProtocol.BattleHeroData>));
                    MapConfig _config = (MapConfig)translator.GetObject(L, 3, typeof(MapConfig));
                    
                    gen_to_be_invoked.InitAll( _battleHeroDatas, _config );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_InitCamera(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                FightManager gen_to_be_invoked = (FightManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _posIndex = LuaAPI.xlua_tointeger(L, 2);
                    
                    gen_to_be_invoked.InitCamera( _posIndex );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Tick(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                FightManager gen_to_be_invoked = (FightManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.Tick(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_InputKey(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                FightManager gen_to_be_invoked = (FightManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    System.Collections.Generic.List<GameProtocol.OpKey> _keyList = (System.Collections.Generic.List<GameProtocol.OpKey>)translator.GetObject(L, 2, typeof(System.Collections.Generic.List<GameProtocol.OpKey>));
                    
                    gen_to_be_invoked.InputKey( _keyList );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SendMoveOperation(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                FightManager gen_to_be_invoked = (FightManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    ShawnFramework.ShawMath.ShawVector3 _logicDir;translator.Get(L, 2, out _logicDir);
                    
                    gen_to_be_invoked.SendMoveOperation( _logicDir );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SendSkillOperation(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                FightManager gen_to_be_invoked = (FightManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _skillID = LuaAPI.xlua_tointeger(L, 2);
                    UnityEngine.Vector3 _vec;translator.Get(L, 3, out _vec);
                    
                    gen_to_be_invoked.SendSkillOperation( _skillID, _vec );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_EnterCDState(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                FightManager gen_to_be_invoked = (FightManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _skilldID = LuaAPI.xlua_tointeger(L, 2);
                    int _cdTime = LuaAPI.xlua_tointeger(L, 3);
                    
                    gen_to_be_invoked.EnterCDState( _skilldID, _cdTime );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UnInit(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                FightManager gen_to_be_invoked = (FightManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.UnInit(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetSelfHero(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                FightManager gen_to_be_invoked = (FightManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _posIndex = LuaAPI.xlua_tointeger(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.GetSelfHero( _posIndex );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetAllEnvColliders(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                FightManager gen_to_be_invoked = (FightManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        var gen_ret = gen_to_be_invoked.GetAllEnvColliders(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddBullet(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                FightManager gen_to_be_invoked = (FightManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    BulletLogic _bullet = (BulletLogic)translator.GetObject(L, 2, typeof(BulletLogic));
                    
                    gen_to_be_invoked.AddBullet( _bullet );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_KeyID(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                FightManager gen_to_be_invoked = (FightManager)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushuint(L, gen_to_be_invoked.KeyID);
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
			    translator.Push(L, FightManager.Instance);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_playWnd(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                FightManager gen_to_be_invoked = (FightManager)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.playWnd);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_SkillDisMultipler(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                FightManager gen_to_be_invoked = (FightManager)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushnumber(L, gen_to_be_invoked.SkillDisMultipler);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_transEnvRoot(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                FightManager gen_to_be_invoked = (FightManager)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.transEnvRoot);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Instance(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    FightManager.Instance = (FightManager)translator.GetObject(L, 1, typeof(FightManager));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_playWnd(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                FightManager gen_to_be_invoked = (FightManager)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.playWnd = (UGUI_PlayPanel)translator.GetObject(L, 2, typeof(UGUI_PlayPanel));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_SkillDisMultipler(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                FightManager gen_to_be_invoked = (FightManager)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.SkillDisMultipler = (float)LuaAPI.lua_tonumber(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_transEnvRoot(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                FightManager gen_to_be_invoked = (FightManager)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.transEnvRoot = (UnityEngine.Transform)translator.GetObject(L, 2, typeof(UnityEngine.Transform));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
