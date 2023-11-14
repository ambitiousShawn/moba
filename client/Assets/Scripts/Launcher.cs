// Demo启动类
using ShawnFramework.CommonModule;
using ShawnFramework.ShawLog;
using UnityEngine;
using XLua;

public class Launcher : MonoBehaviour
{
    public static Launcher Instance;

    public Transform UIRoot;
    public UGUI_Tip TipPanel;

    [Header("免登录测试")]
    public bool SkipLogin;

    void Awake()
    {
        Instance = this;

        // 初始化日志
        LogConfig config = new LogConfig()
        {
            EnableLog = true,
            EnableSaveToFile = false,
            Type = EShawLogType.Unity,
        };
        LogCore.InitSettings(config);

        // 初始化Lua模块
        LuaManager.CreateSingletonInstance();
        LuaManager.Instance.GlobalLuaEnv.DoString("require 'lua_enter'");
    }

    private void Start()
    {
        InitService();
        InitSystem();

        //_loginSys.EnterLogin(); // 进入登录系统
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
    /// 唤起提示框
    /// </summary>
    /// <param name="info"></param>
    /// 
    public void AddTips(string info)
    {
        TipPanel.AddTipToQueue(info);
    }
}