using GameProtocol;
using PENet;
using ShawnFramework.ShawLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using XLua;

namespace ShawnFramework.CommonModule
{

    public class NetSvc : MonoBehaviour
    {
        public static NetSvc Instance;
        public static readonly string msgqueue_lock = "msgqueue_lock";

        private KCPNet<ClientSession, GameMsg> client;

        private Task<bool> checkTask = null; // 断线重连检测

        public void InitService()
        {
            Instance = this;
            client = new KCPNet<ClientSession, GameMsg>();
            msgPackQueue = new Queue<GameMsg>();

            KCPTool.LogFunc = LogCore.Log;
            KCPTool.WarnFunc = LogCore.Warn;
            KCPTool.ErrorFunc = LogCore.Error;
            KCPTool.ColorLogFunc = (color, msg) =>
            {
                LogCore.ColorLog(msg, (ELogColor)color);
            };
            
            string srvIP = ServerConfig.LocalDevInnerIP;

            client.StartAsClient(srvIP, ServerConfig.UdpPort);
            checkTask = client.ConnectServer(100);
        }

        private Queue<GameMsg> msgPackQueue;

        public void AddToMsgQueue(GameMsg msg)
        {
            lock (msgqueue_lock)
            {
                msgPackQueue.Enqueue(msg);
            }
        }

        private int reconnectedCount = 0;
        public void Update()
        {
            if (checkTask != null && checkTask.IsCompleted)
            {
                if (checkTask.Result)
                {
                    checkTask = null;
                }
                else
                {
                    ++reconnectedCount;
                    if (reconnectedCount > 4)
                    {
                        LogCore.Error(string.Format("Connect Failed {0} Times, Check Your Network Connection.", reconnectedCount));
                        Launcher.Instance.AddTips("无法连接服务器，请检查网络状况");
                        checkTask = null;
                    }
                    else
                    {
                        LogCore.ColorLog(string.Format("Connect Failed {0} Times,Retry...", reconnectedCount), ELogColor.Yellow);
                        checkTask = client.ConnectServer(100);
                    }
                }
            }

            if (client != null && client.clientSession != null)
            {
                if (msgPackQueue.Count > 0)
                {
                    lock (msgqueue_lock)
                    {
                        GameMsg msg = msgPackQueue.Dequeue();
                        HandoutMsg(msg);
                    }
                }
                return;
            }

            // if (GMSys.Instance.IsActive)
            // {
            //     if (msgPackQueue.Count > 0)
            //     {
            //         lock (msgqueue_lock)
            //         {
            //             GameMsg msg = msgPackQueue.Dequeue();
            //             HandoutMsg(msg);
            //         }
            //     }
            // }
        }

        public void SendMsg(GameMsg msg, Action<bool> callback = null)
        {
            // 测试接收消息
            // if (GMSys.Instance.IsActive)
            // {
            //     GMSys.Instance.SimulateServerRevMsg(msg);
            //     callback?.Invoke(true);
            //     return;
            // }

            if (client.clientSession != null && client.clientSession.IsConnected())
            {
                client.clientSession.SendMsg(msg);
                callback?.Invoke(true);
            }
            else
            {
                Launcher.Instance.AddTips("服务器未连接!");
                LogCore.ColorLog("服务器未连接！", ELogColor.Red);
                callback?.Invoke(false);
            }
        }
        

        /// <summary>
        /// 分发消息
        /// </summary>
        private void HandoutMsg(GameMsg msg)
        {
            if (msg.errorCode != ErrorCode.None)
            {
                switch (msg.errorCode)
                {
                    case ErrorCode.AccountIsOnLine:
                        Launcher.Instance.AddTips("当前账号已经上线！");
                        break;
                    default:
                        break;
                }
                return;
            }
            switch (msg.cmd)
            {
                case CMD.RspLogin:
                    Action<GameMsg> rsp_login = LuaManager.Instance.GlobalLuaEnv.Global.Get<Action<GameMsg>>("RspLoginCallBack"); // 调用lua的全局响应函数
                    rsp_login?.Invoke(msg);
                    break;
                case CMD.RspMatch:
                    Action<GameMsg> rsp_match = LuaManager.Instance.GlobalLuaEnv.Global.Get<Action<GameMsg>>("RspMatchCallBack"); // 调用lua的全局响应函数
                    rsp_match?.Invoke(msg);
                    break;
                case CMD.NtfConfirm:
                    Action<GameMsg> ntf_confirm = LuaManager.Instance.GlobalLuaEnv.Global.Get<Action<GameMsg>>("NtfConfirmCallBack"); // 调用lua的全局响应函数
                    ntf_confirm?.Invoke(msg);
                    break;
                case CMD.NtfSelect:
                    Action<GameMsg> ntf_select = LuaManager.Instance.GlobalLuaEnv.Global.Get<Action<GameMsg>>("NtfSelectCallBack"); // 调用lua的全局响应函数
                    ntf_select?.Invoke(msg); 
                    break;
                case CMD.NtfLoadRes:
                    Action<GameMsg> ntf_loadres = LuaManager.Instance.GlobalLuaEnv.Global.Get<Action<GameMsg>>("NtfLoadResCallBack"); // 调用lua的全局响应函数
                    ntf_loadres?.Invoke(msg);
                    break;
                case CMD.SndLoadPrg:
                    Action<GameMsg> ntf_loadprg = LuaManager.Instance.GlobalLuaEnv.Global.Get<Action<GameMsg>>("NtfLoadPrgCallBack"); // 调用lua的全局响应函数
                    ntf_loadprg?.Invoke(msg);
                    break;
                case CMD.RspBattleStart:
                    Action<GameMsg> rsp_battlestart = LuaManager.Instance.GlobalLuaEnv.Global.Get<Action<GameMsg>>("RspBattleStart"); // 调用lua的全局响应函数
                    rsp_battlestart?.Invoke(msg);
                    break;
                // case CMD.NtfOpKey:
                //     BattleSys.Instance.NtfOpKey(msg);
                //     break;
                default:
                    break;
            }
        }
    }


    /// <summary>
    /// 客户端网络通信会话
    /// </summary>
    public class ClientSession : KCPSession<GameMsg>
    {
        protected override void OnConnected()
        {
            Launcher.Instance.AddTips("连接服务器成功！");
        }

        protected override void OnDisConnected()
        {
            Launcher.Instance.AddTips("断开服务器连接。");
        }

        protected override void OnReciveMsg(GameMsg msg)
        {
            NetSvc.Instance.AddToMsgQueue(msg);
        }

        protected override void OnUpdate(DateTime now)
        {

        }
    }
}