

using GameProtocol;

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

            this.Log("RoomSys Init Done!");
        }

        public void AddPVPRoom(ServerSession[] serverSessions, PVPType type)
        {
            uint roomID = GetUniqueRoomID();
            PVPRoom room = new PVPRoom(roomID, type, serverSessions);
            pvpRoomList.Add(room);
            pvpRoomDic.Add(roomID, room);
        }

        uint roomID = 0;
        public uint GetUniqueRoomID()
        {
            return ++roomID;
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
                this.Warn("PVPRoom ID:" + req.roomID + "is destroyed.");
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
                this.Warn("PVPRoom ID:" + req.roomID + "is destroyed.");
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
                this.Warn("PVPRoom ID:" + req.roomID + "is destroyed.");
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
                this.Warn("PVPRoom ID:" + req.roomID + "is destroyed.");
            }
        }
    }
}
