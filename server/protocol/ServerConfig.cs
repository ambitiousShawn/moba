using PENet;

// 网络协议配置
namespace GameProtocol
{
    public class ServerConfig
    {
        // 确认匹配倒计时：15s
        public const int ConfirmCountDown = 15;
        // 选择英雄倒计时：60s
        public const int SelectCountDown = 30;

        public const string LocalDevInnerIP = "127.0.0.1";
        public const int UdpPort = 17666;
    }

    public class Config
    {
        public const float ClientLogicFrameDeltaTimeSec = 0.066f;
    }
}