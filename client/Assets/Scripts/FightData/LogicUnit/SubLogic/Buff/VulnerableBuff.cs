// Ò×ËðBuff£¬ÔöÉË
using ShawnFramework.ShawMath;
using System;

public class VulnerableBuffConfig : BuffConfig
{
    public int damagePct;
}

public class VulnerableBuff : BuffLogic
{
    ShawInt damagePct;
    MainLogicUnit target;

    public VulnerableBuff(MainLogicUnit source, MainLogicUnit owner, Skill skill, int buffID, object[] args = null) : base(source, owner, skill, buffID, args)
    {
    }

    public override void LogicInit()
    {
        base.LogicInit();

        VulnerableBuffConfig vbc = config as VulnerableBuffConfig;
        damagePct = vbc.damagePct;
        target = skill.lockTarget;
    }

    protected override void Start()
    {
        base.Start();
        target.OnHurt += GetHurt;
    }

    void GetHurt()
    {
        target.GetDamageByBuff(damagePct / 100 * target.unitData.unitCfg.hp, this, false);
    }

    protected override void End()
    {
        base.End();
        target.OnHurt -= GetHurt;
    }
}