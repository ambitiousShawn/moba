using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using server.CommonTools.ShawKCPNet;
using ShawnFramework.ShawLog;

namespace ShawnFramework.ShawKCPNet
{
    [Serializable]
    public abstract class KCPMsg { }

    public class KCPNet<T, K>
        where T : KCPSession<K>, new()
        where K : KCPMsg, new()
    {
        UdpClient udp;
        IPEndPoint remotePoint;

        private CancellationTokenSource cts;
        private CancellationToken ct;

        public KCPNet()
        {
            cts = new CancellationTokenSource();
            ct = cts.Token;
        }

        #region Server
        private Dictionary<uint, T> sessionDic = null;
        public void StartAsServer(string ip, int port)
        {
            sessionDic = new Dictionary<uint, T>();

            udp = new UdpClient(new IPEndPoint(IPAddress.Parse(ip), port));
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                udp.Client.IOControl((IOControlCode)(-1744830452), new byte[] { 0, 0, 0, 0 }, null);
            }
            remotePoint = new IPEndPoint(IPAddress.Parse(ip), port);
            // LogCore.ColorLog("Server Start...", ELogColor.Green);
            Task.Run(ServerRecive, ct);
        }
        async void ServerRecive()
        {
            UdpReceiveResult result;
            while (true)
            {
                try
                {
                    if (ct.IsCancellationRequested)
                    {
                        LogCore.ColorLog("SeverRecive Task is Cancelled.", ELogColor.Cyan);
                        break;
                    }
                    result = await udp.ReceiveAsync();
                    uint sid = BitConverter.ToUInt32(result.Buffer, 0);
                    if (sid == 0)
                    {
                        sid = GenerateUniqueSessionID();
                        byte[] sid_bytes = BitConverter.GetBytes(sid);
                        byte[] conv_bytes = new byte[8];
                        Array.Copy(sid_bytes, 0, conv_bytes, 4, 4);
                        SendUDPMsg(conv_bytes, result.RemoteEndPoint);
                    }
                    else
                    {
                        if (!sessionDic.TryGetValue(sid, out T session))
                        {
                            session = new T();
                            session.InitSession(sid, SendUDPMsg, result.RemoteEndPoint);
                            session.OnSessionClose = OnServerSessionClose;
                            lock (sessionDic)
                            {
                                sessionDic.Add(sid, session);
                            }
                        }
                        else
                        {
                            session = sessionDic[sid];
                        }
                        session.ReciveData(result.Buffer);
                    }
                }
                catch (Exception e)
                {
                    LogCore.Warn($"Server Udp Recive Data Exception:{e.ToString()}");
                }
            }
        }
        void OnServerSessionClose(uint sid)
        {
            if (sessionDic.ContainsKey(sid))
            {
                lock (sessionDic)
                {
                    sessionDic.Remove(sid);
                    LogCore.Warn($"Session:{sid} remove from sessionDic.");
                }
            }
            else
            {
                LogCore.Error($"Session:{sid} cannot find in sessionDic");
            }
        }
        public void CloseServer()
        {
            foreach (var item in sessionDic)
            {
                item.Value.CloseSession();
            }
            sessionDic = null;

            if (udp != null)
            {
                udp.Close();
                udp = null;
                cts.Cancel();
            }
        }
        #endregion

        #region Client
        public T clientSession;
        public void StartAsClient(string ip, int port)
        {
            udp = new UdpClient(0);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                udp.Client.IOControl((IOControlCode)(-1744830452), new byte[] { 0, 0, 0, 0 }, null);
            }
            remotePoint = new IPEndPoint(IPAddress.Parse(ip), port);
            LogCore.ColorLog("Client Start...", ELogColor.Green);

            Task.Run(ClientRecive, ct);
        }
        public Task<bool> ConnectServer(int interval, int maxintervalSum = 5000)
        {
            SendUDPMsg(new byte[4], remotePoint);
            int checkTimes = 0;
            Task<bool> task = Task.Run(async () => {
                while (true)
                {
                    await Task.Delay(interval);
                    checkTimes += interval;
                    if (clientSession != null && clientSession.IsConnected())
                    {
                        return true;
                    }
                    else
                    {
                        if (checkTimes > maxintervalSum)
                        {
                            return false;
                        }
                    }
                }
            });
            return task;
        }
        async void ClientRecive()
        {
            UdpReceiveResult result;
            while (true)
            {
                try
                {
                    if (ct.IsCancellationRequested)
                    {
                        LogCore.ColorLog("ClientRecive Task is Cancelled.", ELogColor.Cyan);
                        break;
                    }
                    result = await udp.ReceiveAsync();

                    if (Equals(remotePoint, result.RemoteEndPoint))
                    {
                        uint sid = BitConverter.ToUInt32(result.Buffer, 0);
                        if (sid == 0)
                        {
                            //sid 数据
                            if (clientSession != null && clientSession.IsConnected())
                            {
                                //已经建立连接，初始化完成了，收到了多的sid,直接丢弃。
                                LogCore.Warn("Client is Init Done,Sid Surplus.");
                            }
                            else
                            {
                                //未初始化，收到服务器分配的sid数据，初始化一个客户端session
                                sid = BitConverter.ToUInt32(result.Buffer, 4);
                                LogCore.ColorLog($"UDP Request Conv Sid:{sid}", ELogColor.Green);

                                //会话处理
                                clientSession = new T();
                                clientSession.InitSession(sid, SendUDPMsg, remotePoint);
                                clientSession.OnSessionClose = OnClientSessionClose;
                            }
                        }
                        else
                        {
                            //处理业务逻辑
                            if (clientSession != null && clientSession.IsConnected())
                            {
                                clientSession.ReciveData(result.Buffer);
                            }
                            else
                            {
                                //没初始化且sid!=0，数据消息提前到了，直接丢弃消息，直到初
                                //始化完成，kcp重传再开始处理。
                                LogCore.Warn("Client is Initing...");
                            }
                        }
                    }
                    else
                    {
                        LogCore.Warn("Client Udp Recive illegal target Data.");
                    }
                }
                catch (Exception e)
                {
                    LogCore.Warn($"Client Udp Recive Data Exception:{e.ToString()}");
                }
            }
        }
        void OnClientSessionClose(uint sid)
        {
            cts.Cancel();
            if (udp != null)
            {
                udp.Close();
                udp = null;
            }
            LogCore.Warn($"Client Session Close,sid:{sid}");
        }
        public void CloseClient()
        {
            if (clientSession != null)
            {
                clientSession.CloseSession();
            }
        }
        #endregion

        void SendUDPMsg(byte[] bytes, IPEndPoint remotePoint)
        {
            if (udp != null)
            {
                udp.SendAsync(bytes, bytes.Length, remotePoint);
            }
        }
        public void BroadCastMsg(K msg)
        {
            byte[] bytes = KCPTools.Serialize<K>(msg);
            foreach (var item in sessionDic)
            {
                item.Value.SendMsg(bytes);
            }
        }
        private uint sid = 0;
        public uint GenerateUniqueSessionID()
        {
            lock (sessionDic)
            {
                while (true)
                {
                    ++sid;
                    if (sid == uint.MaxValue)
                    {
                        sid = 1;
                    }
                    if (!sessionDic.ContainsKey(sid))
                    {
                        break;
                    }
                }
            }
            return sid;
        }
    }
}
