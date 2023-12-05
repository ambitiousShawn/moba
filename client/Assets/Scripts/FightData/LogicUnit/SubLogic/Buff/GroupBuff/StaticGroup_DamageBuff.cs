// 静态位置群体伤害
using ShawnFramework.ShawLog;
using ShawnFramework.ShawMath;
using System;
using System.Collections.Generic;

public class StaticGroup_DamageBuffConfig : BuffConfig
{
    public int damage;
}

public class StaticGroup_DamageBuff : BuffLogic
{
    ShawInt damage;
    public StaticGroup_DamageBuff(MainLogicUnit source, MainLogicUnit owner, Skill skill, int buffID, object[] args = null) : base(source, owner, skill, buffID, args)
    {
    }

    public override void LogicInit()
    {
        base.LogicInit();
        targetList = new List<MainLogicUnit>();
        StaticGroup_DamageBuffConfig sgdbc = config as StaticGroup_DamageBuffConfig;
        damage = sgdbc.damage;

        switch (sgdbc.staticPosType)
        {
            case EStaticPosType.SkillCasterPos:
                LogicPos = source.LogicPos; // 释放者的位置
                break;
            case EStaticPosType.SkillLockTargetPos:
                LogicPos = skill.lockTarget.LogicPos;
                break;
            case EStaticPosType.BulletHitTargetPos:
                LogicPos = (ShawVector3)args[1];
                break;
            case EStaticPosType.UIInputPos:
                LogicPos = source.LogicPos + skill.skillArgs;
                break;
            default:
                LogCore.Error("static buff pos error!");
                break;
        }
    }

    protected override void Start()
    {
        base.Start();
        TriggerGroupDamage();
    }

    protected override void Tick()
    {
        base.Tick();
        TriggerGroupDamage();
    }

    private void TriggerGroupDamage()
    {
        targetList.Clear();
        targetList.AddRange(CalcSkillSelectTarget.FindMultipleTargetByConfig(owner, config.impacter, LogicPos));
        for (int i = 0; i < targetList.Count; i++)
        {
            MainLogicUnit target = targetList[i];
            target.GetDamageByBuff(damage, this);
        }
    }
}