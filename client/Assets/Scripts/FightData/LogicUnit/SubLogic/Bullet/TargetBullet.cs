using ShawnFramework.ShawMath;
using ShawnFramework.ShawnPhysics;
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
        // 目标已经死亡
        if (target.stateType == EUnitStateType.Dead)
        {
            state = SubUnitState.End;
            return;
        }

        LogicPos += LogicDir * LogicMoveSpeed;
        // SweepVolume 体积扫描算法
        ShawVector3 centerPos = (LogicPos + lastPos) / 2;   // 相邻两帧位置的中心点
        ShawVector3 deltaOffset = LogicPos - lastPos;       // 相邻两帧位置向量差
        ColliderConfig bulletConfig = new ColliderConfig
        {
            mType = ColliderType.Box,
            mPos = centerPos,
            mSize = new ShawVector3
            {
                x = deltaOffset.magnitude / 2,
                y = 0,
                z = bulletSize,
            },
            mAxis = new ShawVector3[3],
        };
        bulletConfig.mAxis[0] = deltaOffset.normalized;
        bulletConfig.mAxis[1] = ShawVector3.up;
        bulletConfig.mAxis[2] = ShawVector3.Cross(deltaOffset, ShawVector3.up).normalized;

        ShawBoxCollider bulletCollider = new ShawBoxCollider(bulletConfig);

        lastPos = LogicPos;     // 记录上一帧的位置信息

        // 与场景逻辑单元碰撞检测
        ShawVector3 normal = ShawVector3.zero;
        ShawVector3 adj = ShawVector3.zero;
        if (target.selfCollider.CollisionDetect(bulletCollider, ref normal, ref adj))
        {
            // 碰撞检测到了，直接销毁子弹
            state = SubUnitState.End;
        }
    }

    protected override void End()
    {
        base.End();

        // 命中目标，产生效果
        if (target.stateType != EUnitStateType.Dead)
        {
            HitTargetCB?.Invoke(target, null);
        }
    }
}