// Demo������
using ShawnFramework.CommonModule;
using ShawnFramework.ShawLog;
using UnityEngine;
using XLua;

public class Launcher : MonoBehaviour
{
    public static Launcher Instance;

    public Transform UIRoot;
    public UGUI_Tip TipPanel;

    [Header("���¼����")]
    public bool SkipLogin;

    void Awake()
    {
        Instance = this;

        // ��ʼ����־
        LogConfig config = new LogConfig()
        {
            EnableLog = true,
            EnableSaveToFile = false,
            Type = EShawLogType.Unity,
        };
        LogCore.InitSettings(config);

        // ��ʼ��Luaģ��
        LuaManager.CreateSingletonInstance();
        LuaManager.Instance.GlobalLuaEnv.DoString("require 'lua_enter'");
    }

    private void Start()
    {
        InitService();
        InitSystem();

        //_loginSys.EnterLogin(); // �����¼ϵͳ
        LuaManager.Instance.GlobalLuaEnv.DoString("require 'ui.lobby.ugui_lobbypanel' \n local window_manager = require 'system.window_manager' \n window_manager:open('ugui_lobbypanel', nil)");
    }

    private AssetsSvc _assetsSvc;
    private AudioSvc _audioSvc;
    private NetSvc _netSvc;
    void InitService()
    {
        _assetsSvc = GetComponent<AssetsSvc>();
        _audioSvc = GetComponent<AudioSvc>();
        _netSvc = GetComponent<NetSvc>();

        _assetsSvc.InitService();
        _audioSvc.InitService();
        _netSvc.InitService();
    }

    private LoginSys _loginSys;
    private LobbySys _lobbySys;
    private BattleSys _battleSys;
    void InitSystem()
    {
        _loginSys = GetComponent<LoginSys>();
        _lobbySys = GetComponent<LobbySys>();
        _battleSys = GetComponent<BattleSys>();

        _loginSys.InitSystem();
        _lobbySys.InitSystem();
        _battleSys.InitSystem();
    }

    /// <summary>
    /// ������ʾ��
    /// </summary>
    /// <param name="info"></param>
    /// 
    public void AddTips(string info)
    {
        TipPanel.AddTipToQueue(info);
    }
}