using ShawnFramework.CommonModule;
using UnityEngine;

public class LoginSys : SystemRoot
{
    public static LoginSys Instance;

    public UGUI_LoginPanel loginPanel;
    public override void InitSystem()
    {
        Instance = this;
    }

    /// <summary>
    ///  进入登录系统 / 界面
    /// </summary>
    public void EnterLogin()
    {
        loginPanel.SetActive(true);
    }
}
