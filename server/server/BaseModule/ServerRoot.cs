using PEUtils;

namespace GameServer
{
    public class ServerRoot : Singleton<ServerRoot>
    {

        public override void Init() 
        {
            base.Init();

            LogConfig config = new LogConfig
            {
                enableLog = true,
                logPrefix = "#",
                enableTime = true,
                logSeparate = ">>",
                enableThreadID = false,
                enableTrace = false,
                enableSave = false,
                enableCover = false,
                loggerType = LoggerType.Console,
            };

            PELog.InitSettings(config);

            // 服务
            CacheSvc.Instance.Init();
            TimerSvc.Instance.Init();
            NetSvc.Instance.Init();
            DBSvc.Instance.Init();

            // 业务
            LoginSys.Instance.Init();
            MatchSys.Instance.Init();
            RoomSys.Instance.Init();
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
