

using GameProtocol;
using ShawnFramework.ShawKCPNet;
using ShawnFramework.ShawLog;

namespace GameServer
{
    /// <summary>
    /// 消息包裹
    /// </summary>
    public class MsgPack
    {
        public ServerSession session;
        public GameMsg msg;
        public MsgPack(ServerSession session, GameMsg msg)
        {
            this.session = session;
            this.msg = msg;
        }
    }

    public class NetSvc : Singleton<NetSvc>
    {
        public static readonly string msgqueue_lock = "msgqueue_lock";

        private KCPNet<ServerSession, GameMsg> server = new();

        public override void Init()
        {
            base.Init();

            server.StartAsServer(ServerConfig.LocalDevInnerIP, ServerConfig.UdpPort);

            LogCore.ColorLog("[Net] 网络服务初始化完成！", ELogColor.Cyan);
        }

        private Queue<MsgPack> msgPackQueue = new Queue<MsgPack>();

        public void AddToMsgQueue(ServerSession session, GameMsg msg)
        {
            lock (msgqueue_lock)
            {
                msgPackQueue.Enqueue(new MsgPack(session, msg));
            }
        }


        public override void Update()
        {
            base.Update();

            if (msgPackQueue.Count > 0)
            {
                lock (msgqueue_lock)
                {
                    MsgPack msg = msgPackQueue.Dequeue();
                    HandoutMsg(msg);
                }
            }
        }
        /// <summary>
        /// 分发消息
        /// </summary>
        private void HandoutMsg(MsgPack pack)
        {
            switch (pack.msg.cmd)
            {
                case CMD.ReqLogin:
                    LoginSys.Instance.ReqLogin(pack);
                    break;
                case CMD.ReqMatch:
                    MatchSys.Instance.ReqMatch(pack);
                    break;
                case CMD.SndConfirm:
                    RoomSys.Instance.SndConfirm(pack);
                    break;
                case CMD.SndSelect:
                    RoomSys.Instance.SndSelect(pack);
                    break;
                case CMD.SndLoadPrg:
                    RoomSys.Instance.SndLoadPrg(pack);
                    break;
                case CMD.ReqBattleStart:
                    RoomSys.Instance.ReqBattleStart(pack);
                    break;
                case CMD.SndOpKey:
                    RoomSys.Instance.SndOpKey(pack);
                    break;
                default:
                    break;
            }
        }
    }
}
