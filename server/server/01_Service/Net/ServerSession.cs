

using GameProtocol;
using server.CommonTools.ShawKCPNet;
using ShawnFramework.ShawLog;

namespace GameServer
{
    public class ServerSession : KCPSession<GameMsg>
    {
        protected override void OnConnected()
        {
            LogCore.ColorLog($"Client Online:sid:{m_sid}", ELogColor.Green);
        }

        protected override void OnDisConnected()
        {
            LogCore.Warn($"Client Offline:sid:{m_sid}");
        }

        protected override void OnReciveMsg(GameMsg msg)
        {
            LogCore.Log("RevPack CMD:{0}", msg.cmd.ToString());
            NetSvc.Instance.AddToMsgQueue(this, msg);
        }

        protected override void OnUpdate(DateTime now)
        {
            
        }
    }
}
