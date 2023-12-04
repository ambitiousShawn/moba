using ShawnFramework.ShawLog;
using ShawnFramework.ShawMath;
using ShawnFramework.ShawnPhysics;
using System;
using System.Collections.Generic;

public class DirectionBullet : BulletLogic
{
    ShawVector3 targetPos;                  // Ŀ��λ��
    ShawCylinderCollider targetCollider;    // Ŀ����ײ��

    int flyTime;    // ����ʱ��

    public Action<MainLogicUnit, object[]> hitTargetCB; // ����Ŀ���Ļص�
    public Action ReachPosCB;                           // ����Ŀ��λ�ú�Ļص�

    public DirectionBullet(MainLogicUnit source, Skill skill) : base(source, skill)
    {
    }

    public override void LogicInit()
    {
        base.LogicInit();

        EBulletType bulletType = config.bulletType;
        // ָ��������е��ӵ�
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
        // ָ��λ�÷��е��ӵ�
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

        LogicPos += LogicDir * (ShawInt)config.bulletOffset; // �ӵ�����λ��
    }

    protected override void Tick()
    {
        base.Tick();

        LogicPos += LogicDir * LogicMoveSpeed;
        // SweepVolume ���ɨ���㷨
        ShawVector3 centerPos = (LogicPos + lastPos) / 2;   // ������֡λ�õ����ĵ�
        ShawVector3 deltaOffset = LogicPos - lastPos;       // ������֡λ��������
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

        lastPos = LogicPos;     // ��¼��һ֡��λ����Ϣ

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
            // ���Ա��赲
            if (hitLst.Count > 0)
            {
                MainLogicUnit hitTarget = CalcSkillSelectTarget.FindMinDistanceTargetInPos(lastPos, hitLst.ToArray());
                hitTargetCB?.Invoke(hitTarget, new object[] { flyTime, hitTarget.LogicPos });
                state = SubUnitState.End;
            }
        }
        else
        {
            // �����赲����͸�Լ���
            for (int i = 0; i < hitLst.Count; i++)
            {
                hitTargetCB?.Invoke(hitLst[i], new object[] { flyTime, hitLst[i].LogicPos });
            }
        }

        // �Ƿ񵽴�Ŀ��λ��
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