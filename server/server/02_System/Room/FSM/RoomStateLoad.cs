using GameProtocol;
using PEUtils;

namespace GameServer
{
    /// <summary>
    /// 加载界面状态
    /// </summary>
    public class RoomStateLoad : RoomStateBase
    {
        private int[] percentArr;
        private bool[] loadArr;

        public RoomStateLoad(PVPRoom room) : base(room) { }

        public override void Enter()
        {
            int len = room.sessionArr.Length;
            percentArr = new int[len];
            loadArr = new bool[len];

            GameMsg msg = new GameMsg
            {
                cmd = CMD.NtfLoadRes,
                ntfLoadRes = new NtfLoadRes
                {
                    mapID = 101,
                    battleHeroDatas = new List<BattleHeroData>(),
                }
            };

            for (int i = 0; i < room.SelectHeroArr.Length; i++)
            {
                SelectData sd = room.SelectHeroArr[i];
                BattleHeroData hero = new BattleHeroData
                {
                    heroID = sd.selectID,
                    userName = GetDefaultHeroSelect(i),
                };
                msg.ntfLoadRes.battleHeroDatas.Add(hero);
            }

            for (int i = 0; i < len; i++)
            {
                msg.ntfLoadRes.posIndex = i;
                room.sessionArr[i].SendMsg(msg);
            }
        }

        string GetDefaultHeroSelect(int posIndex)
        {
            UserData ud = CacheSvc.Instance.GetUserDataBySession(room.sessionArr[posIndex]);
            if (ud != null)
            {
                return ud.name;
            }
            return "";
        }

        public override void Update()
        {

        }

        public override void Exit()
        {
            percentArr = null;
            loadArr = null;
        }

        public void UpdateLoadState(int posIndex, int percent)
        {
            percentArr[posIndex] = percent;
            GameMsg msg = new GameMsg
            {
                cmd = CMD.NtfLoadPrg,
                ntfLoadPrg = new NtfLoadPrg
                {
                    percentList = new List<int>()
                }
            };

            for (int i = 0; i < percentArr.Length; i++)
            {
                msg.ntfLoadPrg.percentList.Add(percentArr[i]);
            }
            room.BroadcastMsg(msg);
            
        }
    
        public void UpdateLoadDone(int posIndex)
        {
            loadArr[posIndex] = true;
            for (int i = 0; i < loadArr.Length; i++)
            {
                if (loadArr[i] == false)
                {
                    return;
                }
            }

            // 所有用户加载完成
            GameMsg msg = new GameMsg
            {
                cmd = CMD.RspBattleStart,
            };
            room.BroadcastMsg (msg);

            room.SwitchRoomState(ERoomStateType.Fight);
            PELog.ColorLog(LogColor.Green, $"RoomID:{room.roomID} 所有玩家加载完成");
        }
    }
}