

using GameProtocol;
using PENet;
using PEUtils;

namespace GameServer
{
    public class ServerSession : KCPSession<GameMsg>
    {
        protected override void OnConnected()
        {
            this.ColorLog(LogColor.Green, "Client Online:sid:{0}", m_sid);
        }

        protected override void OnDisConnected()
        {
            this.Warn("Client Offline:sid:{0}", m_sid);
        }

        protected override void OnReciveMsg(GameMsg msg)
        {
            this.Log("RevPack CMD:{0}", msg.cmd.ToString());
            NetSvc.Instance.AddToMsgQueue(this, msg);
        }

        protected override void OnUpdate(DateTime now)
        {
            
        }
    }
}
