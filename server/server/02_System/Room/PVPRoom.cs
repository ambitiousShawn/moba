

using GameProtocol;
using PENet;

namespace GameServer
{
    public class PVPRoom 
    {
        public uint roomID;
        public PVPType pvpType = PVPType.None;
        public ServerSession[] sessionArr;

        private Dictionary<ERoomStateType, RoomStateBase> FSM = new Dictionary<ERoomStateType, RoomStateBase>();
        private ERoomStateType currentType = ERoomStateType.None;

        private SelectData[] selectHeroArr = null;
        public SelectData[] SelectHeroArr
        {
            set
            {
                selectHeroArr = value;
            }
            get
            {
                return selectHeroArr;
            }
        }

        public PVPRoom(uint roomID, PVPType pvpType, ServerSession[] sessionArr)
        {
            this.roomID = roomID;
            this.pvpType = pvpType;
            this.sessionArr = sessionArr;

            FSM.Add(ERoomStateType.Confirm, new RoomStateConfirm(this));
            FSM.Add(ERoomStateType.Select, new RoomStateSelect(this));
            FSM.Add(ERoomStateType.Load, new RoomStateLoad(this));
            FSM.Add(ERoomStateType.Fight, new RoomStateFight(this));
            FSM.Add(ERoomStateType.End, new RoomStateEnd(this));

            SwitchRoomState(ERoomStateType.Confirm);
        }

        public void SwitchRoomState(ERoomStateType newType)
        {
            if (currentType == newType) return;
            if (FSM.ContainsKey(newType))
            {
                if (currentType != ERoomStateType.None)
                {
                    FSM[currentType].Exit();
                }
                currentType = newType;
                FSM[currentType].Enter();
            }
        }

        public void BroadcastMsg(GameMsg msg)
        {
            // 由于每个人广播消息都一样，所以可以优先序列化，否则需要序列化相同消息很多次
            byte[] bytes = KCPTool.Serialize(msg);
            if (bytes != null)
            {
                for (int i = 0; i < sessionArr.Length; i++)
                {
                    sessionArr[i].SendMsg(msg);
                }
            }
        }
        int GetPosIndex(ServerSession session)
        {
            int posIndex = 0;
            for (int i = 0;i < sessionArr.Length;i++)
            {
                if (sessionArr[i].Equals(session))
                {
                    posIndex = i;
                }
            }
            return posIndex;
        }

        public void SndConfirm(ServerSession session)
        {
            if (currentType == ERoomStateType.Confirm)
            {
                if (FSM[currentType] is RoomStateConfirm state) 
                {
                    state.UpdateConfirmState(GetPosIndex(session));
                }
            }
        }

        public void SndSelect(ServerSession session, int heroID)
        {
            if (currentType == ERoomStateType.Select)
            {
                if (FSM[currentType] is RoomStateSelect state)
                {
                    state.UpdateHeroSelect(GetPosIndex(session), heroID);
                }
            }
        }

        public void SndLoadPrg(ServerSession session, int percent)
        {
            if (currentType == ERoomStateType.Load)
            {
                if (FSM[currentType] is RoomStateLoad state)
                {
                    state.UpdateLoadState(GetPosIndex(session), percent);
                }
            }
        }

        public void ReqBattleStart(ServerSession session)
        {
            if (currentType == ERoomStateType.Load)
            {
                if (FSM[currentType] is RoomStateLoad state)
                {
                    state.UpdateLoadDone(GetPosIndex(session));
                }
            }
        }

        public void SndOpKey(OpKey opKey)
        {
            if (currentType == ERoomStateType.Fight)
            {
                if (FSM[currentType] is RoomStateFight state)
                {
                    state.UpdateOpKey(opKey);
                }
            }
        }

        public void SndChat(string chatMsg)
        {
            GameMsg msg = new GameMsg
            {
                cmd = CMD.NtfChat,
                ntfChat = new NtfChat
                {
                    chatMsg = chatMsg
                }
            };
            BroadcastMsg(msg);
        }

        public void ReqBattleEnd(ServerSession session)
        {
            if (currentType == ERoomStateType.Fight)
            {
                if (FSM[currentType] is RoomStateFight state)
                {
                    state.UpdateEndState(GetPosIndex(session));
                }
            }
        }

        public void Clear()
        {
            selectHeroArr = null;
            sessionArr = null;
            FSM = null;
        }
    }
}
