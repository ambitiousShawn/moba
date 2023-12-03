using ShawnFramework.ShawMath;
using System;

public class TargetBullet : BulletLogic
{
    protected MainLogicUnit target; // 追踪目标
    protected ShawVector3 curveDir = ShawVector3.zero;

    public Action<MainLogicUnit, object[]> HitTargetCB;

    public TargetBullet(MainLogicUnit source, MainLogicUnit target, Skill skill) : base(source, skill)
    {
        this.target = target;
    }

    public override void LogicInit()
    {
        base.LogicInit();

        ShawVector3 targetPos = target.LogicPos + new ShawVector3(0, target.unitData.unitCfg.hitHeight, 0);
        LogicDir = (targetPos - LogicPos).normalized;
        LogicPos += LogicDir * (ShawInt)config.bulletOffset;
    }

    protected override void Tick()
    {
        base.Tick();

        LogicDir = (target.LogicPos + new ShawVector3(0, target.unitData.unitCfg.hitHeight, 0) - LogicPos).normalized;
        if (LogicDir == ShawVector3.zero)
        {
            state = SubUnitState.End;
            return;
        }        

        if (target.stateType == EUnitStateType.Dead)
        {
            state = SubUnitState.End;
            return;
        }
    }
}