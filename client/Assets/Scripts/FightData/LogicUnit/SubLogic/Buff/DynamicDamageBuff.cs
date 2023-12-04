// ¸úËæÊ½ÉËº¦¼¼ÄÜ
using ShawnFramework.ShawMath;
using System.Collections.Generic;

public class DynamicDamageBuffConfig : BuffConfig
{
    public int damage;
}

public class DynamicDamageBuff : BuffLogic
{
    ShawInt damage;
    public DynamicDamageBuff(MainLogicUnit source, MainLogicUnit owner, Skill skill, int buffID, object[] args = null) : base(source, owner, skill, buffID, args)
    {
    }

    public override void LogicInit()
    {
        base.LogicInit();

        targetList = new List<MainLogicUnit>();
        DynamicDamageBuffConfig ddbc = config as DynamicDamageBuffConfig;
        damage = ddbc.damage;
    }
    
    protected override void Tick()
    {
        base.Tick();
        targetList.Clear();
        List<MainLogicUnit> units = CalcSkillSelectTarget.FindMultipleTargetByConfig(owner, config.impacter, ShawVector3.zero);
        targetList.AddRange(units);
        for (int i = 0; i < targetList.Count; i++)
        {
            targetList[i].GetDamageByBuff(damage, this);
        }
    }
}