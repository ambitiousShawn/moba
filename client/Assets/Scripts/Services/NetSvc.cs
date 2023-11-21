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

        private Task<bool> checkTask = null; // �����������

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
                        Launcher.Instance.AddTips("�޷����ӷ���������������״��");
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
            // ���Խ�����Ϣ
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
                Launcher.Instance.AddTips("������δ����!");
                LogCore.ColorLog("������δ���ӣ�", ELogColor.Red);
                callback?.Invoke(false);
            }
        }
        

        /// <summary>
        /// �ַ���Ϣ
        /// </summary>
        private void HandoutMsg(GameMsg msg)
        {
            if (msg.errorCode != ErrorCode.None)
            {
                switch (msg.errorCode)
                {
                    case ErrorCode.AccountIsOnLine:
                        Launcher.Instance.AddTips("��ǰ�˺��Ѿ����ߣ�");
                        break;
                    default:
                        break;
                }
                return;
            }
            switch (msg.cmd)
            {
                case CMD.RspLogin:
                    Action<GameMsg> rsp_login = LuaManager.Instance.GlobalLuaEnv.Global.Get<Action<GameMsg>>("RspLoginCallBack"); // ����lua��ȫ����Ӧ����
                    rsp_login?.Invoke(msg);
                    break;
                case CMD.RspMatch:
                    Action<GameMsg> rsp_match = LuaManager.Instance.GlobalLuaEnv.Global.Get<Action<GameMsg>>("RspMatchCallBack"); // ����lua��ȫ����Ӧ����
                    rsp_match?.Invoke(msg);
                    break;
                case CMD.NtfConfirm:
                    Action<GameMsg> ntf_confirm = LuaManager.Instance.GlobalLuaEnv.Global.Get<Action<GameMsg>>("NtfConfirmCallBack"); // ����lua��ȫ����Ӧ����
                    ntf_confirm?.Invoke(msg);
                    break;
                case CMD.NtfSelect:
                    Action<GameMsg> ntf_select = LuaManager.Instance.GlobalLuaEnv.Global.Get<Action<GameMsg>>("NtfSelectCallBack"); // ����lua��ȫ����Ӧ����
                    ntf_select?.Invoke(msg); 
                    break;
                case CMD.NtfLoadRes:
                    Action<GameMsg> ntf_loadres = LuaManager.Instance.GlobalLuaEnv.Global.Get<Action<GameMsg>>("NtfLoadResCallBack"); // ����lua��ȫ����Ӧ����
                    ntf_loadres?.Invoke(msg);
                    break;
                case CMD.SndLoadPrg:
                    Action<GameMsg> ntf_loadprg = LuaManager.Instance.GlobalLuaEnv.Global.Get<Action<GameMsg>>("NtfLoadPrgCallBack"); // ����lua��ȫ����Ӧ����
                    ntf_loadprg?.Invoke(msg);
                    break;
                case CMD.RspBattleStart:
                    Action<GameMsg> rsp_battlestart = LuaManager.Instance.GlobalLuaEnv.Global.Get<Action<GameMsg>>("RspBattleStart"); // ����lua��ȫ����Ӧ����
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
    /// �ͻ�������ͨ�ŻỰ
    /// </summary>
    public class ClientSession : KCPSession<GameMsg>
    {
        protected override void OnConnected()
        {
            Launcher.Instance.AddTips("���ӷ������ɹ���");
        }

        protected override void OnDisConnected()
        {
            Launcher.Instance.AddTips("�Ͽ����������ӡ�");
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