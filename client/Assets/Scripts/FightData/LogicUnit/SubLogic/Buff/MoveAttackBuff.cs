using ShawnFramework.CommonModule;
using ShawnFramework.ShawLog;
using ShawnFramework.ShawMath;
using System;
using UnityEngine;

public class MoveAttackBuff : BuffLogic
{
    MainLogicUnit moveTarget;
    SkillConfig skillConfig;
    ShawInt selectRange;
    ShawInt searchDistance;

    bool activeSkill;

    public MoveAttackBuff(MainLogicUnit source, MainLogicUnit owner, Skill skill, int buffID, object[] args = null) : base(source, owner, skill, buffID, args)
    {
    }

    public override void LogicInit()
    {
        base.LogicInit();
        skillConfig = AssetsSvc.Instance.GetSkillConfigByID(skill.skillID);
        selectRange = (ShawInt)skillConfig.targetConf.selectRange;
        searchDistance = (ShawInt)skillConfig.targetConf.searchDis;
        activeSkill = false;
    }

    protected override void Start()
    {
        base.Start();
        MoveToTarget();
    }

    protected override void Tick()
    {
        base.Tick();
        MoveToTarget();
    }

    void MoveToTarget()
    {
        moveTarget = CalcSkillSelectTarget.FindMinDistanceEnemyTarget(owner, skill.config.targetConf);
        if (moveTarget == null)
        {
            return;
        }

        ShawVector3 offsetDir = moveTarget.LogicPos - owner.LogicPos;
        ShawInt sqrDistance = offsetDir.sqrMagnitude;
        ShawInt sumRadius = owner.unitData.unitCfg.colliCfg.mRadius + moveTarget.unitData.unitCfg.colliCfg.mRadius;
        if (sqrDistance < (selectRange + sumRadius) * (selectRange + sumRadius))
        {
            activeSkill = true;
            FightManager.Instance.SendMoveOperation(ShawVector3.zero);
            state = SubUnitState.End;
        }
        else
        {
            if (sqrDistance < (searchDistance + sumRadius) * (searchDistance + sumRadius))
            {
                if (FightManager.Instance.CheckUIInput())
                {
                    // ÓÐUIÊäÈë£¬ÖÐ¶ÏÒÆ¶¯¹¥»÷
                    state = SubUnitState.End;
                }
                else
                {
                    FightManager.Instance.SendMoveOperation(offsetDir.normalized);
                }
            }
            else
            {
                LogCore.Log("³¬³öËÑË÷¾àÀë");
                FightManager.Instance.SendMoveOperation(ShawVector3.zero);
                state = SubUnitState.End;
            }
        }
    }

    protected override void End()
    {
        base.End();
        if (activeSkill)
        {
            activeSkill = false;
            LogCore.Log("Send ReActive Skill Msg");
            FightManager.Instance.SendSkillOperation(skill.skillID, Vector3.zero);
        }
    }
}