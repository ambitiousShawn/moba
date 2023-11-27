using GameProtocol;
using ShawnFramework.ShawLog;

namespace GameServer
{
    /// <summary>
    /// 英雄选择状态
    /// </summary>
    public class RoomStateSelect : RoomStateBase
    {
        private SelectData[] selectArr = null;
        private int checkTaskID = -1;
        private bool isAllSelected = false;

        public RoomStateSelect(PVPRoom room) : base(room) { }

        public override void Enter()
        {
            int len = room.sessionArr.Length;
            selectArr = new SelectData[len];
            for (int i = 0; i < len; i++)
            {
                selectArr[i] = new SelectData()
                {
                    selectID = 0,
                    selectDone = false
                };
            }
            GameMsg msg = new GameMsg
            {
                cmd = CMD.NtfSelect,
            };
            room.BroadcastMsg(msg);

            checkTaskID = TimerSvc.Instance.AddTask(ServerConfig.SelectCountDown * 1000 + 2000, ReachTimeLimit);
        }

        void ReachTimeLimit(int tid)
        {
            if (isAllSelected)
            {
                return;
            }
            else
            {
                LogCore.Warn($"RoomID:{room.roomID} 玩家超时未确认选择");
                for (int i = 0;i < selectArr.Length;i++)
                {
                    if (!selectArr[i].selectDone)
                    {
                        selectArr[i].selectID = GetDefaultHeroSelect(i);
                        selectArr[i].selectDone = true;
                    }
                }

                room.SelectHeroArr = selectArr;
                room.SwitchRoomState(ERoomStateType.Load);
            }
        }

        int GetDefaultHeroSelect(int posIndex)
        {
            UserData ud = CacheSvc.Instance.GetUserDataBySession(room.sessionArr[posIndex]);
            if (ud != null)
            {
                return ud.heroDatas[0].heroId;
            }
            return 0;
        }

        void CheckSelectState()
        {
            for (int i = 0; i < selectArr.Length;i++)
            {
                if (!selectArr[i].selectDone)
                {
                    return;
                }
            }
            isAllSelected = true;
        }

        public void UpdateHeroSelect(int posIndex, int heroID)
        {
            selectArr[posIndex].selectID = heroID;
            selectArr[posIndex].selectDone = true;

            CheckSelectState();
            if (isAllSelected)
            {
                // 进入Load状态
                if (TimerSvc.Instance.RemoveTask(checkTaskID))
                {
                    LogCore.ColorLog($"RoomID:{room.roomID} 所有玩家加载完成", ELogColor.Green);
                }
                else
                {
                    LogCore.Warn("Remove CheckTaskID Failed!");
                }

                room.SelectHeroArr = selectArr;
                room.SwitchRoomState(ERoomStateType.Load);
            }
        }

        public override void Update()
        {
           
        }

        public override void Exit()
        {
            selectArr = null;
            checkTaskID = -1;
            isAllSelected = false;
        }

        
    }
}