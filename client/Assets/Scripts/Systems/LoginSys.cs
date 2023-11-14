using ShawnFramework.CommonModule;
using UnityEngine;
using XLua;

public class LoginSys : SystemRoot
{
    public static LoginSys Instance;
    private LuaEnv luaEnv; // lua����

    public override void InitSystem()
    {
        Instance = this;
        luaEnv = LuaManager.Instance.GlobalLuaEnv;
    }

    /// <summary>
    ///  �����¼ϵͳ / ����
    /// </summary>
    public void EnterLogin()
    {
        // ע��login + ��login����
        luaEnv.DoString("require 'ui.login.ugui_loginpanel' \n local window_manager = require 'system.window_manager' \n window_manager:open('ugui_loginpanel', nil)");
    }
}
