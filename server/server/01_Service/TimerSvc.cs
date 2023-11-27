// using PETimer;
using ShawnFramework.ShawLog;
using ShawnFramework.ShawTimer;

namespace GameServer
{
    public class TimerSvc : Singleton<TimerSvc>
    {
        TickTimer timer = new TickTimer(0, false);
        public override void Init()
        {
            base.Init();

            LogCore.ColorLog("[Timer] 定时器服务初始化完成！", ELogColor.Cyan);
        }

        public override void Update()
        {
            base.Update();
            timer.UpdateTask();
        }

        public int AddTask(uint delay, Action<int> taskCB, Action<int> cancelCB = null, int count = 1)
        {
            return timer.AddTask(delay,taskCB, cancelCB, count);
        }
        public bool RemoveTask(int tid)
        {
            return timer.RemoveTask(tid);
        }
    }
}
