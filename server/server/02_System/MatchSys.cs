

using GameProtocol;

namespace GameServer
{
    public class MatchSys : SystemRoot<MatchSys>
    {
        public override void Init()
        {
            base.Init();

            que1v1 = new Queue<ServerSession>();
            que2v2 = new Queue<ServerSession>();
            que5v5 = new Queue<ServerSession>();

            this.Log("MatchSys Init Done!");
        }
        public override void Update()
        {
            base.Update();

            while (que1v1.Count >= 2)
            {
                ServerSession[] serverSessions = new ServerSession[2];
                for (int i = 0; i < serverSessions.Length; i++)
                {
                    serverSessions[i] = que1v1.Dequeue();
                }
                RoomSys.Instance.AddPVPRoom(serverSessions, PVPType._1v1);
            }

            while (que1v1.Count >= 4)
            {
                ServerSession[] serverSessions = new ServerSession[4];
                for (int i = 0; i < serverSessions.Length; i++)
                {
                    serverSessions[i] = que2v2.Dequeue();
                }
                RoomSys.Instance.AddPVPRoom(serverSessions, PVPType._1v1);
            }

            while (que1v1.Count >= 10)
            {
                ServerSession[] serverSessions = new ServerSession[10];
                for (int i = 0; i < serverSessions.Length; i++)
                {
                    serverSessions[i] = que5v5.Dequeue();
                }
                RoomSys.Instance.AddPVPRoom(serverSessions, PVPType._1v1);
            }
        }

        // 匹配队列
        private Queue<ServerSession> que1v1 = null;
        private Queue<ServerSession> que2v2 = null;
        private Queue<ServerSession> que5v5 = null;

        public void ReqMatch(MsgPack msgPack)
        {
            ReqMatch data = msgPack.msg.reqMatch;
            PVPType pvpEnum = data.pvpType;
            switch (pvpEnum) 
            {
                case PVPType._1v1:
                    que1v1.Enqueue(msgPack.session);
                    break;
                case PVPType._2v2:
                    que2v2.Enqueue(msgPack.session);
                    break;
                case PVPType._5v5:
                    que5v5.Enqueue(msgPack.session);
                    break;
                default:
                    break;
            }
            GameMsg msg = new GameMsg
            {
                cmd = CMD.RspMatch,
                rspMatch = new RspMatch
                {
                    predictTime = GetPredictTime(pvpEnum),
                }
            };
            msgPack.session.SendMsg(msg);
        }

        private int GetPredictTime(PVPType pvpType)
        {
            int waitCount = 0;
            switch(pvpType)
            {
                case PVPType._1v1:
                    waitCount = 2 - que1v1.Count;
                    if (waitCount < 0)
                    {
                        waitCount = 0;
                    }
                    return waitCount * 10 + 5;
                case PVPType._2v2:
                    waitCount = 4 - que2v2.Count;
                    if (waitCount < 0)
                    {
                        waitCount = 0;
                    }
                    return waitCount * 10 + 5;
                case PVPType._5v5:
                    waitCount = 10  - que5v5.Count;
                    if (waitCount < 0)
                    {
                        waitCount = 0;
                    }
                    return waitCount * 10 + 5;
            }
            return waitCount;
        }
    }
}
