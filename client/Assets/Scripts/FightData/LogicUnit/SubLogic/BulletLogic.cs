// 子弹逻辑单元
using ShawnFramework.CommonModule;
using ShawnFramework.ShawLog;
using ShawnFramework.ShawMath;
using UnityEngine;

public abstract class BulletLogic : SubLogicUnit
{
    protected ShawVector3 lastPos; // 上一个逻辑帧的位置(sweepvolume检测)
    protected ShawInt bulletSize;  // 子弹半径
    protected BulletConfig config; // 子弹配置

    BulletView bulletView; 

    protected BulletLogic(MainLogicUnit source, Skill skill) : base(source, skill)
    {
    }

    public override void LogicInit()
    {
        base.LogicInit();

        config = skill.config.bulletConf;
        bulletSize = (ShawInt)config.bulletSize;
        LogicMoveSpeed = (ShawInt)config.bulletSpeed;

        LogicPos = source.LogicPos + new ShawVector3(0, (ShawInt)config.bulletHeight, 0);
        lastPos = LogicPos;
        delayTime = config.bulletDelay;
    }

    public override void LogicTick()
    {
        base.LogicTick();
        switch (state)
        {
            case SubUnitState.Start:
                Start();
                state = SubUnitState.Tick;
                break;
            case SubUnitState.Tick:
                Tick();
                break;
            default:
                break;
        }
    }

    protected override void Start()
    {
        GameObject go = AssetsSvc.Instance.LoadPrefab("bullet", config.resPath, 1); ;
        go.name = $"{source.unitName}_{config.bulletName}";
        bulletView = go.AddComponent<BulletView>();
        if (bulletView == null)
        {
            LogCore.Error($"Get bulletview error:{unitName}");
        }
        else
        {
            bulletView.Init(this);
        }
    }

    protected override void Tick() { }

    protected override void End()
    {
        bulletView.DestroyBullet();
    }
}