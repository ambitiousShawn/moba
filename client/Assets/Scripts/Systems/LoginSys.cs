using ShawnFramework.CommonModule;
using UnityEngine;
using XLua;

public class LoginSys : SystemRoot
{
    public static LoginSys Instance;
    private LuaEnv luaEnv; // lua环境

    public override void InitSystem()
    {
        Instance = this;
        luaEnv = LuaManager.Instance.GlobalLuaEnv;
    }

    /// <summary>
    ///  进入登录系统 / 界面
    /// </summary>
    public void EnterLogin()
    {
        // 注册login + 打开login窗口
        luaEnv.DoString("require 'ui.login.ugui_loginpanel' \n local window_manager = require 'system.window_manager' \n window_manager:open('ugui_loginpanel', nil)");
    }
}
