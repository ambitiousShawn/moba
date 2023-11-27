using ShawnFramework.ShawKCPNet;
using ShawnFramework.ShawLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets.Kcp;
using System.Text;
using System.Threading.Tasks;

namespace server.CommonTools.ShawKCPNet
{
    public enum SessionState
    {
        None,
        Connected,
        DisConnected
    }

    public abstract class KCPSession<T>
        where T : KCPMsg, new()
    {
        protected uint m_sid;
        Action<byte[], IPEndPoint> m_udpSender;
        private IPEndPoint m_remotePoint;
        protected SessionState m_sessionState = SessionState.None;
        public Action<uint> OnSessionClose;

        public KCPHandle m_handle;
        public Kcp m_kcp;
        private CancellationTokenSource cts;
        private CancellationToken ct;

        public void InitSession(uint sid, Action<byte[], IPEndPoint> udpSender, IPEndPoint remotePoint)
        {
            m_sid = sid;
            m_udpSender = udpSender;
            m_remotePoint = remotePoint;
            m_sessionState = SessionState.Connected;

            m_handle = new KCPHandle();
            m_kcp = new Kcp(sid, m_handle);
            m_kcp.NoDelay(1, 10, 2, 1);
            m_kcp.WndSize(64, 64);
            m_kcp.SetMtu(512);

            m_handle.Out = (Memory<byte> buffer) => {
                byte[] bytes = buffer.ToArray();
                m_udpSender(bytes, m_remotePoint);
            };

            m_handle.Recv = (byte[] buffer) => {
                buffer = KCPTools.DeCompress(buffer);
                T msg = KCPTools.DeSerialize<T>(buffer);
                if (msg != null)
                {
                    OnReciveMsg(msg);
                }
            };

            OnConnected();

            cts = new CancellationTokenSource();
            ct = cts.Token;
            Task.Run(Update, ct);
        }
        public void ReciveData(byte[] buffer)
        {
            m_kcp.Input(buffer.AsSpan());
        }

        public void SendMsg(T msg)
        {
            if (IsConnected())
            {
                byte[] bytes = KCPTools.Serialize(msg);
                if (bytes != null)
                {
                    SendMsg(bytes);
                }
            }
            else
            {
                LogCore.Warn("Session Disconnected.Can not send msg.");
            }
        }
        public void SendMsg(byte[] msg_bytes)
        {
            if (IsConnected())
            {
                msg_bytes = KCPTools.Compress(msg_bytes);
                m_kcp.Send(msg_bytes.AsSpan());
            }
            else
            {
                LogCore.Warn("Session Disconnected.Can not send msg.");
            }
        }
        public void CloseSession()
        {
            cts.Cancel();
            OnDisConnected();

            OnSessionClose?.Invoke(m_sid);
            OnSessionClose = null;

            m_sessionState = SessionState.DisConnected;
            m_remotePoint = null;
            m_udpSender = null;
            m_sid = 0;

            m_handle = null;
            m_kcp = null;
            cts = null;
        }

        async void Update()
        {
            try
            {
                while (true)
                {
                    DateTime now = DateTime.UtcNow;
                    OnUpdate(now);
                    if (ct.IsCancellationRequested)
                    {
                        LogCore.ColorLog("SessionUpdate Task is Cancelled.", ELogColor.Cyan);
                        break;
                    }
                    else
                    {
                        m_kcp.Update(now);
                        int len;
                        while ((len = m_kcp.PeekSize()) > 0)
                        {
                            var buffer = new byte[len];
                            if (m_kcp.Recv(buffer) >= 0)
                            {
                                m_handle.Recive(buffer);
                            }
                        }
                        await Task.Delay(10);
                    }
                }
            }
            catch (Exception e)
            {
                LogCore.Warn($"Session Update Exception:{e.ToString()}");
            }
        }

        protected abstract void OnUpdate(DateTime now);
        protected abstract void OnConnected();
        protected abstract void OnReciveMsg(T msg);
        protected abstract void OnDisConnected();


        public override bool Equals(object obj)
        {
            if (obj is KCPSession<T>)
            {
                KCPSession<T> us = obj as KCPSession<T>;
                return m_sid == us.m_sid;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return m_sid.GetHashCode();
        }
        public uint GetSessionID()
        {
            return m_sid;
        }

        public bool IsConnected()
        {
            return m_sessionState == SessionState.Connected;
        }
    }
}
