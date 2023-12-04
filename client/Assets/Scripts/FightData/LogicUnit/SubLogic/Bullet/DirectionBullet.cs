using ShawnFramework.ShawLog;
using ShawnFramework.ShawMath;
using ShawnFramework.ShawnPhysics;
using System;
using System.Collections.Generic;

public class DirectionBullet : BulletLogic
{
    ShawVector3 targetPos;                  // 目标位置
    ShawCylinderCollider targetCollider;    // 目标碰撞体

    int flyTime;    // 飞行时间

    public Action<MainLogicUnit, object[]> hitTargetCB; // 击中目标后的回调
    public Action ReachPosCB;                           // 到达目标位置后的回调

    public DirectionBullet(MainLogicUnit source, Skill skill) : base(source, skill)
    {
    }

    public override void LogicInit()
    {
        base.LogicInit();

        EBulletType bulletType = config.bulletType;
        // 指定方向飞行的子弹
        if (bulletType == EBulletType.UIDirection)
        {
            if (skill.skillArgs == ShawVector3.zero)
            {
                LogCore.Error("input skill direction is vector2.zero.");
            }
            else
            {
                LogicDir = skill.skillArgs;
            }
        }
        // 指定位置飞行的子弹
        else if (bulletType == EBulletType.UIPosition)
        {
            targetPos = source.LogicPos + skill.skillArgs + new ShawVector3(0, (ShawInt)config.bulletHeight, 0);
            ColliderConfig targetColliderConfig = new ColliderConfig
            {
                mPos = targetPos,
                mType = ColliderType.Cylinder,
                mRadius = bulletSize,
            };
            targetCollider = new ShawCylinderCollider(targetColliderConfig);
            LogicDir = (targetPos - LogicPos).normalized;
        }
        else
        {
            LogCore.Log("Unknow Bullet Type Enum.");
        }

        LogicPos += LogicDir * (ShawInt)config.bulletOffset; // 子弹发射位置
    }

    protected override void Tick()
    {
        base.Tick();

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

        List<MainLogicUnit> hitLst = new List<MainLogicUnit>();
        List<MainLogicUnit> selectLst = CalcSkillSelectTarget.FindMultipleTargetByConfig(source, config.impacter, ShawVector3.zero);
        for (int i = 0; i < selectLst.Count; i++)
        {
            ShawVector3 normal = ShawVector3.zero;
            ShawVector3 adj = ShawVector3.zero;
            if (selectLst[i].selfCollider.CollisionDetect(bulletCollider, ref normal, ref adj))
            {
                hitLst.Add(selectLst[i]);
            }
        }

        if (config.canBlock)
        {
            // 可以被阻挡
            if (hitLst.Count > 0)
            {
                MainLogicUnit hitTarget = CalcSkillSelectTarget.FindMinDistanceTargetInPos(lastPos, hitLst.ToArray());
                hitTargetCB?.Invoke(hitTarget, new object[] { flyTime, hitTarget.LogicPos });
                state = SubUnitState.End;
            }
        }
        else
        {
            // 不可阻挡，穿透性技能
            for (int i = 0; i < hitLst.Count; i++)
            {
                hitTargetCB?.Invoke(hitLst[i], new object[] { flyTime, hitLst[i].LogicPos });
            }
        }

        // 是否到达目标位置
        if (config.bulletType == EBulletType.UIPosition)
        {
            ShawVector3 normal = ShawVector3.zero;
            ShawVector3 adj = ShawVector3.zero;
            if (targetCollider.CollisionDetect(bulletCollider, ref normal, ref adj) )
            {
                state = SubUnitState.End;
            }
        }
        else if (config.bulletType == EBulletType.UIDirection)
        {
            flyTime += ClientConfig.ClientLogicFrameDeltaTimeMS;
            if (flyTime >= config.bulletDuration)
            {
                state = SubUnitState.End;
            }
        }
        lastPos = LogicPos;
    }

    protected override void End()
    {
        base.End();
        ReachPosCB?.Invoke();
    }
}