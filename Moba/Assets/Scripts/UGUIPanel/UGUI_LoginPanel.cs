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

    // ��¼��ť�¼�
    public void BtnLoginClickEvent()
    {
        if (Launcher.Instance.SkipLogin)
        {
            // ��ת
            root.AddTips("���ܵ�¼�ɹ���");
            return;
        }

        // ��¼�ж��߼�
        if (Input_password.text.Length < 6 || Input_username.text.Length < 6)
        {
            // ���Ȳ����Ϲ淶
        }
    }
}