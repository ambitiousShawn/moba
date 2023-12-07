

using GameProtocol;
using PENet;
using PEUtils;

namespace GameServer
{
    public class ServerSession : KCPSession<GameMsg>
    {
        protected override void OnConnected()
        {
            PELog.ColorLog(LogColor.Green, $"Client Online:sid:{m_sid}");
        }

        protected override void OnDisConnected()
        {
            PELog.Warn($"Client Offline:sid:{m_sid}");
        }

        protected override void OnReciveMsg(GameMsg msg)
        {
            PELog.Log("RevPack CMD:{0}", msg.cmd.ToString());
            NetSvc.Instance.AddToMsgQueue(this, msg);
        }

        protected override void OnUpdate(DateTime now)
        {
            
        }
    }
}
