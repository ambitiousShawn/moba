using System;
/*
    服务器启动入口
 */
namespace GameServer
{
    class Launcher
    {
        static void Main(string[] args)
        {
            ServerRoot.Instance.Init();
            while (true)
            {
                ServerRoot.Instance.Update();
                Thread.Sleep(10);
            }
        }
    }
}