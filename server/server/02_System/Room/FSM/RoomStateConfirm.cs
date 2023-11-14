﻿using GameProtocol;

namespace GameServer
{
    /// <summary>
    /// 等待确认状态
    /// </summary>
    public class RoomStateConfirm : RoomStateBase
    {
        private ConfirmData[] confirmArr = null;
        private int checkTastID = -1;
        private bool isAllConfirmed = false; // 是否全部确认

        public RoomStateConfirm(PVPRoom room) : base(room) { }

        public override void Enter()
        {
            int len = room.sessionArr.Length;
            confirmArr = new ConfirmData[len];
            for (int i = 0; i < len; i++)
            {
                confirmArr[i] = new ConfirmData()
                {
                    iconIndex = i,
                    confirmDone = false,
                };
            }
            GameMsg msg = new GameMsg
            {
                cmd = CMD.NtfConfirm,
                ntfConfirm = new NtfConfirm
                {
                    roomID = room.roomID,
                    dissmiss = false,
                    confirmArr = confirmArr,
                }
            };
            room.BroadcastMsg(msg);

            checkTastID = TimerSvc.Instance.AddTask(
                ServerConfig.ConfirmCountDown * 1000
                ,  ReachTimeLimit
                );
        }
        
        void ReachTimeLimit(int tid)
        {
            if (isAllConfirmed)
            {
                return;
            }
            else
            {
                this.ColorLog(PEUtils.LogColor.Yellow, "RoomID:{0} 确认超时，解散房间。", room.roomID);
                GameMsg msg = new GameMsg
                {
                    cmd = CMD.NtfConfirm,
                    ntfConfirm = new NtfConfirm
                    {
                        roomID = room.roomID,
                        dissmiss = true,
                    }
                };

                room.BroadcastMsg (msg);
                room.SwitchRoomState(ERoomStateType.End);
            }
        }

        /// <summary>
        /// 检测是否所有玩家都确认
        /// </summary>
        void CheckConfirmState()
        {
            for (int i = 0;i < confirmArr.Length;i++)
            {
                if (!confirmArr[i].confirmDone)
                {
                    return;
                }
            }
            isAllConfirmed = true;
        }

        public void UpdateConfirmState(int posIndex)
        {
            confirmArr[posIndex].confirmDone = true;
            CheckConfirmState();
            if (isAllConfirmed)
            {
                // 已经全部确认
                if (TimerSvc.Instance.DeleteTask(checkTastID))
                {
                    this.ColorLog(PEUtils.LogColor.Green, "RoomID:{0} 所有玩家确认完成！", room.roomID);
                }
                else
                {
                    this.Warn("Remove CheckTaskID Failed.");
                }
                room.SwitchRoomState(ERoomStateType.Select);
            }
            else
            {
                // 同步数据刷新表现
                GameMsg msg = new GameMsg
                {
                    cmd = CMD.NtfConfirm,
                    ntfConfirm = new NtfConfirm
                    {
                        roomID = room.roomID,
                        dissmiss = false,
                        confirmArr = confirmArr,
                    }
                };
                room.BroadcastMsg(msg);
            }
        }

        public override void Update()
        {
           
        }

        public override void Exit()
        {
            confirmArr = null;
            checkTastID = -1;
            isAllConfirmed = false;
        }
    }
}