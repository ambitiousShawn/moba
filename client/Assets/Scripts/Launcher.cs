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

    [Header("开启游戏资源热更新(确保资源服务器顺利运行！！！)")]
    public bool EnableHotUpdate;

    // 定时器
    List<MonoTimer> tempTimerLst;
    List<MonoTimer> timerLst;

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

    private int mapID;
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

         // Screen.SetResolution(960, 540, false);

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
        if (EnableHotUpdate)
        {
            HotUpdateMgr.InitManager(this);
            HotUpdateMgr.Instance.HotUpdate((isOver) =>
            {
                if (isOver)
                    LogCore.ColorLog("资产热更新已完成！", ELogColor.Orange);
                else
                    LogCore.ColorLog("资产热更新失败！", ELogColor.Orange);
                // 初始化Lua模块
                LuaManager.CreateSingletonInstance();
                LuaManager.Instance.GlobalLuaEnv.DoString("require 'lua_enter'");
            }, (info, progress) =>
            {
                LogCore.ColorLog($"{info},进度: {progress * 100} %", ELogColor.Cyan);
            });
        }
        
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        //计时器
        timerLst = new List<MonoTimer>();
        tempTimerLst = new List<MonoTimer>();
        InitService();
    }

    private void Update()
    {
        if (tempTimerLst.Count > 0)
        {
            timerLst.AddRange(tempTimerLst);
            tempTimerLst.Clear();
        }

        for (int i = timerLst.Count - 1; i >= 0; --i)
        {
            MonoTimer timer = timerLst[i];
            if (timer.IsActive)
            {
                timer.TickTimer(Time.deltaTime * 1000);
            }
            else
            {
                timerLst.RemoveAt(i);
            }
        }
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

    /// <summary>
    /// 添加Mono定时器
    /// </summary>
    /// <param name="timer"></param>
    public void AddMonoTimer(MonoTimer timer)
    {
        tempTimerLst.Add(timer);
    }
}