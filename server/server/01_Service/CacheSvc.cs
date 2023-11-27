using GameProtocol;
using ShawnFramework.ShawLog;

namespace GameServer
{
    public class CacheSvc : Singleton<CacheSvc>
    {
        public override void Init()
        {
            base.Init();
            LogCore.ColorLog("[Cache] 缓存服务初始化完成！", ELogColor.Cyan);
        }

        public override void Update()
        {
            base.Update();
        }

        // account-session
        private Dictionary<string, ServerSession> onLineAccountDic = new Dictionary<string, ServerSession>();
        // session-userdata
        private Dictionary<ServerSession, UserData> onLineSessionDic = new Dictionary<ServerSession, UserData>();
        /// <summary>
        /// 缓存已上线账号信息
        /// </summary>
        /// <returns></returns>
        public bool IsAccountOnline(string account)
        {
            return onLineAccountDic.ContainsKey(account);
        }

        public void AccountOnline(string account, ServerSession session, UserData playerData)
        {
            onLineAccountDic.Add(account, session);
            onLineSessionDic.Add(session, playerData);
        }
   
        public UserData GetUserDataBySession(ServerSession session)
        {
            if (onLineSessionDic.TryGetValue(session, out UserData userData))
            {
                return userData;
            }
            else
            {
                return null;
            }
        }
    }
}
