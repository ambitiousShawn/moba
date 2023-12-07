using PEUtils;
using System.Collections.Concurrent;

namespace ShawnFramework.ShawTimer
{
    public class TickTimer : ShawTimer
    {
        class TickTaskPack
        {
            public int tid;
            public Action<int> callback;

            public TickTaskPack(int tid, Action<int> callback)
            {
                this.tid = tid;
                this.callback = callback;
            }
        }

        private readonly DateTime startDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        private readonly ConcurrentDictionary<int, TickTaskUnit> taskDic;
        private readonly ConcurrentQueue<TickTaskPack> packQue;
        private readonly bool setHandler;
        private const string TIDLock = "ticktimer_lock";

        private readonly Thread timerThread;

        // 传入0，视为通过外部帧函数驱动
        public TickTimer(int interval = 0, bool setHandler = true)
        {
            taskDic = new ConcurrentDictionary<int, TickTaskUnit>();
            this.setHandler = setHandler;

            if (setHandler )
            {
                packQue = new ConcurrentQueue<TickTaskPack>();
            }
            
            if (interval != 0)
            {
                timerThread = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        while (true)
                        {
                            UpdateTask();
                            Thread.Sleep(interval);
                        }
                    }
                    catch (ThreadAbortException e)
                    {
                        PELog.Warn($"Tick Thread Abort:{e}");
                    }
                }));
                timerThread.Start();
            }
        }

        public void UpdateTask()
        {
            double nowTime = GetUTCMilliseconds();
            foreach(var item in taskDic)
            {
                TickTaskUnit task = item.Value;
                if (nowTime < task.destTime)
                {
                    continue;
                }
                task.loopIndex++;
                // 运行有限次数
                if (task.count > 0)
                {
                    --task.count;
                    if (task.count == 0)
                    {
                        // 结束任务
                        if (taskDic.TryRemove(tid, out TickTaskUnit taskh))
                        {
                            task.taskCB?.Invoke(tid);
                            task.taskCB = null;
                        }
                        else
                        {
                            PELog.Warn($"Remove tid:{tid} task in Dic Failed!");
                        }
                    }
                    else
                    {
                        // task.destTime += task.delay;
                        // 避免大循环次数时浮点数累加误差过大
                        task.destTime = task.startTime + task.delay * (task.loopIndex + 1);
                        CallTaskCB(task.tid, task.taskCB);
                    }
                }
                else
                {
                    task.destTime = task.startTime + task.delay * (task.loopIndex + 1);
                    CallTaskCB(task.tid, task.taskCB);
                }
            }
        }

        public override int AddTask(uint delay, Action<int> taskCB, Action<int> cancelCB, int count = 1)
        {
            int tid = GeneratorTid();
            double startTime = GetUTCMilliseconds();
            double destTime = startTime + delay;
            TickTaskUnit unit = new TickTaskUnit(tid, delay, count, destTime, taskCB, cancelCB, startTime);
            if (taskDic.TryAdd(tid, unit))
            {
                return tid;
            }
            else
            {
                PELog.Warn($"KEY:{tid} already exist!");
                return -1;
            }
        }

        public override bool RemoveTask(int tid)
        {
            if (taskDic.TryRemove(tid, out TickTaskUnit task))
            {
                if (setHandler && task.cancelCB != null)
                {
                    packQue.Enqueue(new TickTaskPack(tid, task.cancelCB));
                }
                else
                {
                    task.cancelCB?.Invoke(tid);
                }
                return true;
            }
            else
            {
                PELog.Warn($"tid:{tid} remove failed.");
                return false;
            }
        }

        public override void Reset()
        {
            if (!packQue.IsEmpty)
            {
                PELog.Warn($"Callback Queue is not Empty!");
            }
            taskDic.Clear();
            if (timerThread != null)
            {
                timerThread.Abort();
            }
        }

        public void HandleTask()
        {
            while (packQue != null && packQue.Count > 0)
            {
                if (packQue.TryDequeue(out TickTaskPack pack))
                {
                    pack.callback?.Invoke(pack.tid);
                }
                else 
                {
                    PELog.Error($"packQue Dequeue Data Error!");
                }
            }
        }

        void CallTaskCB(int tid, Action<int> taskCB)
        {
            if (setHandler)
            {
                packQue.Enqueue(new TickTaskPack(tid, taskCB));
            }
            else
            {
                taskCB?.Invoke(tid);
            }
        }

        protected override int GeneratorTid()
        {
            lock(TIDLock)
            {
                while (true)
                {
                    ++tid;
                    if (tid == int.MaxValue)
                    {
                        tid = 0;
                    }
                    if (!taskDic.ContainsKey(tid))
                    {
                        return tid;
                    }
                }
            }
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
            public ulong loopIndex; // 循环次数

            public TickTaskUnit(int tid, uint delay, int count, double destTime, Action<int> taskCB, Action<int> cancelCB, double startTime)
            {
                this.tid = tid;
                this.delay = delay;
                this.count = count;
                this.destTime = destTime;
                this.taskCB = taskCB;
                this.cancelCB = cancelCB;
                this.startTime = startTime;
                this.loopIndex = 0;
            }
        }
    }
}
