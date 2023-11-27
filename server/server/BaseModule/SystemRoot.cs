

namespace GameServer
{
    public abstract class SystemRoot<T> : Singleton<T> where T : new ()
    {
        protected NetSvc netSvc = null;
        protected TimerSvc timerSvc = null;
        protected CacheSvc cacheSvc = null;
        public override void Init() 
        {
            base.Init();

            netSvc = NetSvc.Instance;
            timerSvc = TimerSvc.Instance;
            cacheSvc = CacheSvc.Instance;
        }

        public override void Update() 
        {
            base.Update();
        }
    }
}
