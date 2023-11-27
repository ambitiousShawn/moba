namespace ShawnFramework.ShawTimer
{
    public class TickTimer : ShawTimer
    {
        private readonly DateTime startDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public override int AddTimer(uint delay, Action<int> taskCB, Action<int> cancelCB, int count = 1)
        {
            return 0;
        }

        public override bool RemoveTimer(int tid)
        {
            return false;
        }

        public override void Reset()
        {
            
        }

        protected override int GeneratorTid()
        {
            return tid;
        }

        private double GetUTCMilliseconds()
        {
            TimeSpan span = DateTime.UtcNow - startDateTime;
            return span.TotalMilliseconds;
        }

        // 一个任务单元
        class TickTaskUnit
        {
            public int tid;
            public uint delay;
            public int count;
            public double destTime;
            public Action<int> taskCB;
            public Action<int> cancelCB;
            public double startTime;

            public TickTaskUnit(int tid, uint delay, int count, double destTime, Action<int> taskCB, Action<int> cancelCB, double startTime)
            {
                this.tid = tid;
                this.delay = delay;
                this.count = count;
                this.destTime = destTime;
                this.taskCB = taskCB;
                this.cancelCB = cancelCB;
                this.startTime = startTime;
            }
        }
    }
}
