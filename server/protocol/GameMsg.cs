/*
新功能模块添加通用步骤：
服务端：
    1. 添加新的协议信息
        - 建立新的可序列化类数据
        - 在CMD枚举中增加通信类型
        - 在GameMsg类中添加对应的加载数据
    2. 在状态机的对应阶段添加细节
        - 封装请求数据，并向客户端广播同步数据

客户端：
    3. 部署好UI脚本，同步至各个模块
        - 创建UGUI面板预制体以及相关脚本
        - NetSvc网络交互层添加新的处理分支
        - 在对应的系统层添加相应处理逻辑（保存数据，弹出 / 隐藏界面等）（LobbySys）
        - 必要数据存入GameRoot（地图编号，房间ID等）
        - 系统层完毕后书写界面层，通过服务端数据刷新UI界面
        
*/


//网络通信数据协议
using PENet;

namespace GameProtocol
{
    /// <summary>
    /// 通信协议命令号
    /// </summary>
    public enum CMD
    {
        None = 0,
        ReqLogin = 1, // 登录相关
        RspLogin = 2,

        ReqMatch = 3, // 匹配相关
        RspMatch = 4,

        NtfConfirm = 5, // 确认相关
        SndConfirm = 6,

        NtfSelect = 7, // 选择相关
        SndSelect = 8,

        NtfLoadRes = 9, // 加载资源
        SndLoadPrg = 10,
        NtfLoadPrg = 11,

        ReqBattleStart = 12, // 核心战斗
        RspBattleStart = 13,

        SndOpKey = 100, // 玩家操作相关
        NtfOpKey = 101,

        //结算
        ReqBattleEnd = 201,
        RspBattleEnd = 202,

        //聊天
        SndChat = 203,
        NtfChat = 204
    }
    /// <summary>
    /// 通信错误码
    /// </summary>
    public enum ErrorCode
    {
        None,
        AccountIsOnLine, // 账号已经在线
        ErrorActOrPsd,   // 账号或密码输入错误
    }
    [Serializable]
    public class GameMsg : KCPMsg
    {
        public CMD cmd;
        public ErrorCode errorCode;
        public int error;
        public bool isEmpty;
        public ReqLogin reqLogin;
        public RspLogin rspLogin;

        public ReqMatch reqMatch;
        public RspMatch rspMatch;

        public NtfConfirm ntfConfirm;
        public SndConfirm sndConfirm;

        public NtfSelect ntfSelect;
        public SndSelect sndSelect;

        public NtfLoadRes ntfLoadRes;
        public SndLoadPrg sndLoadPrg;
        public NtfLoadPrg ntfLoadPrg;

        public ReqBattleStart reqBattleStart;
        public RspBattleStart rspBattleStart;

        public SndOpKey sndOpKey;
        public NtfOpKey ntfOpKey;

        public ReqBattleEnd reqBattleEnd;
        public RspBattleEnd rspBattleEnd;

        public SndChat sndChat;
        public NtfChat ntfChat;

        public ReqPing reqPing;
        public RspPing rspPing;
    }

    #region 登录相关
    [Serializable]
    public class ReqLogin
    {
        public string account;
        public string password;
    }

    [Serializable]
    public class RspLogin
    {
        public UserData userData;
    }

    [Serializable]
    public class UserData
    {
        public uint id;
        public string name;
        public int level;
        public int exp;
        public int coin;
        public int diamond;
        public int ticket;
        public List<HeroData> heroDatas;
    }


    [Serializable]
    public class HeroData
    {
        public int heroId;
    }
    #endregion

    #region 匹配确认相关
    [Serializable]
    public enum PVPType
    {
        None = 0,
        _1v1 = 1,
        _2v2 = 2,
        _5v5 = 3,
    }

    [Serializable]
    public class ReqMatch
    {
        public PVPType pvpType; // 匹配类型
    }
    [Serializable]
    public class RspMatch
    {
        public int predictTime; // 预计匹配时长
    }

    [Serializable]
    public class NtfConfirm
    {
        public uint roomID;
        public bool dissmiss; // 解散
        public ConfirmData[] confirmArr;
    }

    [Serializable]
    public class ConfirmData
    {
        public int iconIndex;
        public bool confirmDone;
    }

    [Serializable]
    public class SndConfirm
    {
        public uint roomID;
    }
    #endregion

    #region 选择与加载
    [Serializable]
    public class NtfSelect
    {

    }

    [Serializable]
    public class SelectData
    {
        public int selectID;
        public bool selectDone;
    }
    [Serializable]
    public class SndSelect
    {
        public uint roomID;
        public int heroID;
    }

    [Serializable]
    public class NtfLoadRes
    {
        public int mapID; // 地图ID
        public List<BattleHeroData> battleHeroDatas; // 战斗英雄的数据列表
        public int posIndex; // 玩家个人的位置
    }

    [Serializable]
    public class BattleHeroData
    {
        public string userName; // 玩家名字
        public int heroID; // 选择的英雄ID
        // 皮肤ID，边框，称号
    }

    [Serializable]
    public class SndLoadPrg
    {
        public uint roomID;
        public int percent;
    }
    [Serializable]
    public class NtfLoadPrg
    {
        public List<int> percentList;
    }
    #endregion

    #region 核心战斗模块
    [Serializable]
    public class ReqBattleStart
    {
        public uint roomID;
    }
    [Serializable]
    public class RspBattleStart
    {

    }

    [Serializable]
    public class SndOpKey
    {
        public uint roomID;
        public OpKey opKey; // 操作数据
        // public 
    }
    [Serializable]
    public class NtfOpKey
    {
        public uint frameID;
        public List<OpKey> keyList;
    }

    [Serializable]
    public class ReqBattleEnd
    {
        public uint roomID;
    }
    [Serializable]
    public class RspBattleEnd
    {
        //结算数据
    }

    [Serializable]
    public class SndChat
    {
        public uint roomID;
        public string chatMsg;
    }
    [Serializable]
    public class NtfChat
    {
        public string chatMsg;
    }

    [Serializable]
    public class ReqPing
    {
        public uint pingID;
        public ulong sendTime;
        public ulong backTime;
    }
    [Serializable]
    public class RspPing
    {
        public uint pingID;
    }
#endregion
}


