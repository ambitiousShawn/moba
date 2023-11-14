﻿

using PEUtils;

namespace GameServer
{
    public class ServerRoot : Singleton<ServerRoot>
    {

        public override void Init() 
        {
            base.Init();

            PELog.InitSettings();

            // 服务
            CacheSvc.Instance.Init();
            TimerSvc.Instance.Init();
            NetSvc.Instance.Init();

            // 业务
            LoginSys.Instance.Init();
            MatchSys.Instance.Init();
            RoomSys.Instance.Init();

            this.ColorLog(LogColor.Green, "ServerRoot Init Done!");
        }

        public override void Update() 
        {
            base.Update();

            // 服务
            CacheSvc.Instance.Update();
            TimerSvc.Instance.Update();
            NetSvc.Instance.Update();

            // 业务
            LoginSys.Instance.Update();
            MatchSys.Instance.Update();
            RoomSys.Instance.Update();
        }
    }
}
