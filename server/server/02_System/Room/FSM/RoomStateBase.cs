

using GameProtocol;

namespace GameServer
{
    public interface IRoomState
    {
        void Enter();
        void Update();
        void Exit();
    }

    public enum ERoomStateType
    {
        None = 0,
        Confirm = 1,    // 确认
        Select = 2,     // 选择
        Load = 3,       // 加载
        Fight = 4,      // 战斗
        End = 5,        // 结算
    }

    /// <summary>
    /// 房间的状态基类
    /// </summary>
    public abstract class RoomStateBase : IRoomState
    {
        public PVPRoom room;
        public RoomStateBase(PVPRoom room)
        {
            this.room = room;
        }

        public abstract void Enter();

        public abstract void Update();

        public abstract void Exit();
    }
}
