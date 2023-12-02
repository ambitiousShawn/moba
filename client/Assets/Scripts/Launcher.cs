// Demo������
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

    [Header("������Ϸ��Դ�ȸ���(ȷ����Դ������˳�����У�����)")]
    public bool EnableHotUpdate;

    // ��ʱ��
    List<MonoTimer> tempTimerLst;
    List<MonoTimer> timerLst;

    #region ��������
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

        // ��ʼ����־
        LogConfig config = new LogConfig()
        {
            EnableLog = true,
            EnableThread = false,   
            EnableSaveToFile = true,
            Type = EShawLogType.Unity,
        };
        LogCore.InitSettings(config);

        // ��ʼ����Դ����
        AssetBundleMgr.InitManager(this);

        // ��ʼ���ȸ�������
        if (EnableHotUpdate)
        {
            HotUpdateMgr.InitManager(this);
            HotUpdateMgr.Instance.HotUpdate((isOver) =>
            {
                if (isOver)
                    LogCore.ColorLog("�ʲ��ȸ�������ɣ�", ELogColor.Orange);
                else
                    LogCore.ColorLog("�ʲ��ȸ���ʧ�ܣ�", ELogColor.Orange);
                // ��ʼ��Luaģ��
                LuaManager.CreateSingletonInstance();
                LuaManager.Instance.GlobalLuaEnv.DoString("require 'lua_enter'");
            }, (info, progress) =>
            {
                LogCore.ColorLog($"{info},����: {progress * 100} %", ELogColor.Cyan);
            });
        }
        
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        //��ʱ��
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
    /// ������ʾ��
    /// </summary>
    /// <param name="info"></param>
    /// 
    public void AddTips(string info)
    {
        TipPanel.AddTipToQueue(info);
    }

    /// <summary>
    /// ���Mono��ʱ��
    /// </summary>
    /// <param name="timer"></param>
    public void AddMonoTimer(MonoTimer timer)
    {
        tempTimerLst.Add(timer);
    }
}