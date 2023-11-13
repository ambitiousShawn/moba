using ShawnFramework.UGUI;
using UnityEngine;
using UnityEngine.UI;

public class UGUI_LoginPanel : PanelRoot
{
    public InputField Input_username;
    public InputField Input_password;
    public Button Btn_Login;

    protected override void InitPanel()
    {
        base.InitPanel();

        System.Random rd = new System.Random();
        Input_username.text = rd.Next(100000, 999999).ToString();
        Input_password.text = rd.Next(100000, 999999).ToString();
    }

    // 登录按钮事件
    public void BtnLoginClickEvent()
    {
        if (Launcher.Instance.SkipLogin)
        {
            // 跳转
            root.AddTips("免密登录成功！");
            return;
        }

        // 登录判定逻辑
        if (Input_password.text.Length < 6 || Input_username.text.Length < 6)
        {
            // 长度不符合规范
        }
    }
}