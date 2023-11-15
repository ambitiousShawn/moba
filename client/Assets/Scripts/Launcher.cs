// Demo������
using GameProtocol;
using ShawnFramework.CommonModule;
using ShawnFramework.ShawLog;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public class Launcher : MonoBehaviour
{
    public static Launcher Instance;

    public Transform UIRoot;
    public UGUI_Tip TipPanel;

    [Header("���¼����")]
    public bool SkipLogin;

    // ��������
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

    private BattleSys _battleSys;
    void InitSystem()
    {
        _battleSys = GetComponent<BattleSys>();

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