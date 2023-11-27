using GameProtocol;
using ShawnFramework.ShawLog;

namespace GameServer
{
    /// <summary>
    /// 战斗状态
    /// </summary>
    public class RoomStateFight : RoomStateBase
    {
        uint frameID = 0;
        List<OpKey> opkeyLst = new List<OpKey>();
        int checkTaskID;

        private bool[] endArr;

        public RoomStateFight(PVPRoom room) : base(room) 
        {
            int len = room.sessionArr.Length;
            endArr = new bool[len];
        }

        public override void Enter()
        {
            opkeyLst.Clear();
            // checkTaskID = TimerSvc.Instance.AddTask(ServerConfig.ServerLogicFrameIntervelMs, SyncLogicFrame, null, 0);
        }

        void SyncLogicFrame(int tid)
        {
            ++frameID;
            GameMsg msg = new()
            {
                cmd = CMD.NtfOpKey,
                isEmpty = true,
                ntfOpKey = new NtfOpKey
                {
                    frameID = frameID,
                    keyList = new List<OpKey>()
                }
            };

            int count = opkeyLst.Count;
            if (count > 0)
            {
                msg.isEmpty = false;
                msg.ntfOpKey.keyList.AddRange(opkeyLst);
            }
            opkeyLst.Clear();
            room.BroadcastMsg(msg);
        }

        public override void Update()
        {
           
        }

        public override void Exit()
        {
            checkTaskID = 0;
            opkeyLst.Clear();
            endArr = null;
        }

        public void UpdateOpKey(OpKey key)
        {
            opkeyLst.Add(key);
        }

        public void UpdateEndState(int posIndex)
        {
            endArr[posIndex] = true;

            // if (TimerSvc.Instance.DeleteTask(checkTaskID))
            // {
            //     LogCore.ColorLog("Delete Sync Task Success.", ELogColor.Green);
            // }
            // else
            // {
            //     LogCore.Warn("Delete Sync Task Failed.");
            // }
            room.SwitchRoomState(ERoomStateType.End);
        }
    }
}