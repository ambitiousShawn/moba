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
    ///  �����¼ϵͳ / ����
    /// </summary>
    public void EnterLogin()
    {
        loginPanel.SetActive(true);
    }
}
