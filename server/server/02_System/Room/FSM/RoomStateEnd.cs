using GameProtocol;

namespace GameServer
{
    /// <summary>
    /// 战斗结算状态
    /// </summary>
    public class RoomStateEnd : RoomStateBase
    {
        public RoomStateEnd(PVPRoom room) : base(room) { }

        public override void Enter()
        {
            GameMsg msg = new GameMsg
            {
                cmd = CMD.RspBattleEnd,
                rspBattleEnd = new RspBattleEnd
                {
                    //TOADD
                }
            };

            room.BroadcastMsg(msg);
            Exit();
        }

        public override void Update()
        {
           
        }

        public override void Exit()
        {
            RoomSys.Instance.DestroyRoom(room.roomID);
        }

        
    }
}