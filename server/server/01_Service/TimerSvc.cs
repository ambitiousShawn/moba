using PETimer;

namespace GameServer
{
    public class TimerSvc : Singleton<TimerSvc>
    {
        TickTimer timer = new TickTimer(0, false);
        public override void Init()
        {
            base.Init();

            timer.LogFunc = this.Log;
            timer.WarnFunc = this.Warn;
            timer.ErrorFunc = this.Error;

            this.Log("TimerSvc Init Done!");
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
        public bool DeleteTask(int tid)
        {
            return timer.DeleteTask(tid);
        }
    }
}
