// Demo启动类
using GameProtocol;
using ShawnFramework.CommonModule;
using ShawnFramework.ShawHotUpdate;
using ShawnFramework.ShawLog;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public class Launcher : MonoBehaviour
{
    public static Launcher Instance;

    public Transform UIRoot;
    public Popup_Tip TipPanel;

    [Header("免登录测试")]
    public bool SkipLogin;

    #region 数据区域
    UserData userData;
    public UserData UserData
    {
        set { userData = value; }
        get { return userData; }
    }

    private uint roomID;
    public uint RoomID
    {
        set { roomID = value; }
        get { return roomID; }
    }

    public int mapID = 101;
    public int MapID
    {
        set { mapID = value; }
        get { return mapID; }
    }

    private List<BattleHeroData> battleHeroDatas;
    public List<BattleHeroData> BattleHeroDatas
    {
        set { battleHeroDatas = value; }
        get { return battleHeroDatas; }
    }

    private int selfIndex;
    public int SelfIndex
    {
        set { selfIndex = value; }
        get { return selfIndex; }
    }
    #endregion

    void Awake()
    {
        Instance = this;

        // 初始化日志
        LogConfig config = new LogConfig()
        {
            EnableLog = true,
            EnableThread = false,   
            EnableSaveToFile = true,
            Type = EShawLogType.Unity,
        };
        LogCore.InitSettings(config);

        // 初始化资源管理
        AssetBundleMgr.InitManager(this);
        // 初始化热更新流程
        HotUpdateMgr.InitManager(this);
        HotUpdateMgr.Instance.HotUpdate((isOver) =>
        {
            LogCore.ColorLog("资产热更新已完成！", ELogColor.Orange);
        }, (info) =>
        {

        });

        DontDestroyOnLoad(this);

        // 初始化Lua模块
        LuaManager.CreateSingletonInstance();
    }

    private void Start()
    {
        InitService();
        LuaManager.Instance.GlobalLuaEnv.DoString("require 'lua_enter'");
    }

    private AssetsSvc _assetsSvc;
    public AssetsSvc AssetsSvc { get => _assetsSvc; }

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