namespace ShawnFramework.ShawTimer
{
    public abstract class ShawTimer
    {
        /// <summary>
        /// 创建一个定时任务
        /// </summary>
        /// <param name="delay">定时任务时间</param>
        /// <param name="taskCB">定时任务回调</param>
        /// <param name="cancelCB">取消时的回调</param>
        /// <param name="count">任务重复次数</param>
        /// <returns></returns>
        public abstract int AddTimer(uint delay, Action<int> taskCB, Action<int> cancelCB, int count = 1);

        /// <summary>
        /// 移除一个定时任务
        /// </summary>
        /// <param name="tid">定时任务ID</param>
        /// <returns></returns>
        public abstract bool RemoveTimer(int tid);

        /// <summary>
        /// 重置定时器
        /// </summary>
        public abstract void Reset();

        protected int tid = 0;
        protected abstract int GeneratorTid();
    }
}
