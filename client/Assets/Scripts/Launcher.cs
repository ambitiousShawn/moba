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

        LogCore.ColorLog("启动成功！", ELogColor.Cyan);
    }

    private void Start()
    {
        InitUIRoot();
        InitService();
        InitSystem();

        GameStart();
    }

    void InitUIRoot()
    {
        // for (int i = 0; i < UIRoot.childCount; i++)
        // {
        //     Transform child = UIRoot.GetChild(i);
        //     child.gameObject.SetActive(false); 
        // }
        // TipPanel.SetActive(true);
    }

    private AssetsSvc _assetsSvc;
    private AudioSvc _audioSvc;
    private NetSvc _netSvc;
    void InitService()
    {
        _assetsSvc = this.GetComponent<AssetsSvc>();
        _audioSvc = this.GetComponent<AudioSvc>();
        _netSvc = this.GetComponent<NetSvc>();

        _assetsSvc.InitService();
        _audioSvc.InitService();
        _netSvc.InitService();
    }

    private LoginSys _loginSys;
    private LobbySys _lobbySys;
    private BattleSys _battleSys;
    void InitSystem()
    {
        _loginSys = this.GetComponent<LoginSys>();
        _lobbySys = this.GetComponent<LobbySys>();
        _battleSys = this.GetComponent<BattleSys>();

       // _loginSys.InitSystem();
        _lobbySys.InitSystem();
        _battleSys.InitSystem();
    }

    void GameStart()
    {
        // _loginSys.EnterLogin();
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