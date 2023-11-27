

using GameProtocol;
using ShawnFramework.ShawLog;

namespace GameServer
{
    public class RoomSys : SystemRoot<RoomSys>
    {
        public List<PVPRoom> pvpRoomList = null;
        public Dictionary<uint, PVPRoom> pvpRoomDic = null;

        public override void Init()
        {
            base.Init();

            pvpRoomList = new List<PVPRoom>();
            pvpRoomDic = new Dictionary<uint, PVPRoom>();

            LogCore.ColorLog("[Room] 房间系统初始化完成！", ELogColor.Cyan);

            TimerSvc.Instance.AddTask(5000, CheckStatus, null, 0);
        }

        void CheckStatus(int id)
        {
            LogCore.ColorLog($"对战房间负载：{pvpRoomList.Count}个", ELogColor.Magenta);
        }

        public void AddPVPRoom(ServerSession[] serverSessions, PVPType type)
        {
            uint roomID = GetUniqueRoomID();
            PVPRoom room = new PVPRoom(roomID, type, serverSessions);
            pvpRoomList.Add(room);
            pvpRoomDic.Add(roomID, room);
        }

        public override void Update()
        {
            base.Update();
        }

        public void SndConfirm(MsgPack pack)
        {
            SndConfirm req = pack.msg.sndConfirm;
            if (pvpRoomDic.TryGetValue(req.roomID, out PVPRoom room))
            {
                room.SndConfirm(pack.session);
            }
            else
            {
                LogCore.Warn("PVPRoom ID:" + req.roomID + "is destroyed.");
            }
        }

        public void SndSelect(MsgPack pack)
        {
            SndSelect req = pack.msg.sndSelect;
            if (pvpRoomDic.TryGetValue(req.roomID, out PVPRoom room))
            {
                room.SndSelect(pack.session, req.heroID);
            }
            else
            {
                LogCore.Warn("PVPRoom ID:" + req.roomID + "is destroyed.");
            }
        }

        public void SndLoadPrg(MsgPack pack)
        {
            SndLoadPrg req = pack.msg.sndLoadPrg;
            if (pvpRoomDic.TryGetValue(req.roomID, out PVPRoom room))
            {
                room.SndLoadPrg(pack.session, req.percent);
            }
            else
            {
                LogCore.Warn("PVPRoom ID:" + req.roomID + "is destroyed.");
            }
        }

        public void ReqBattleStart(MsgPack pack)
        {
            ReqBattleStart req = pack.msg.reqBattleStart;
            if (pvpRoomDic.TryGetValue(req.roomID, out PVPRoom room))
            {
                room.ReqBattleStart(pack.session);
            }
            else
            {
                LogCore.Warn("PVPRoom ID:" + req.roomID + "is destroyed.");
            }
        }

        public void SndOpKey(MsgPack pack)
        {
            SndOpKey snd = pack.msg.sndOpKey;
            if (pvpRoomDic.TryGetValue(snd.roomID, out PVPRoom room))
            {
                room.SndOpKey(snd.opKey);
            }
            else
            {
                LogCore.Warn("PVPRoom ID:" + snd.roomID + " is not exist.");
            }
        }

        public void SndChat(MsgPack pack)
        {
            SndChat snd = pack.msg.sndChat;
            if (pvpRoomDic.TryGetValue(snd.roomID, out PVPRoom room))
            {
                room.SndChat(snd.chatMsg);
            }
            else
            {
                LogCore.Warn("PVPRoom ID:" + snd.roomID + " is not exist.");
            }
        }

        public void ReqBattleEnd(MsgPack pack)
        {
            ReqBattleEnd snd = pack.msg.reqBattleEnd;
            if (pvpRoomDic.TryGetValue(snd.roomID, out PVPRoom room))
            {
                room.ReqBattleEnd(pack.session);
            }
            else
            {
                LogCore.Warn("PVPRoom ID:" + snd.roomID + " is not exist.");
            }
        }

        uint roomID = 0;
        public uint GetUniqueRoomID()
        {
            return ++roomID;
        }

        //clear room
        public void DestroyRoom(uint roomID)
        {
            if (pvpRoomDic.TryGetValue(roomID, out PVPRoom room))
            {
                room.Clear();

                int index = -1;
                for (int i = 0; i < pvpRoomList.Count; i++)
                {
                    if (pvpRoomList[i].roomID == roomID)
                    {
                        index = i;
                        break;
                    }
                }
                if (index >= 0)
                {
                    pvpRoomList.RemoveAt(index);
                }
                pvpRoomDic.Remove(roomID);
            }
            else
            {
                LogCore.Error("PVPRoom is not exist ID:" + roomID);
            }
        }
    }
}
