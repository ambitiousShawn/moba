using ShawnFramework.ShawMath;

public class HPCureBuffConfig : BuffConfig
{
    public int cureHPpercent;
}

public class HPCureBuff : BuffLogic
{
    public ShawInt currHPpercent;

    public HPCureBuff(MainLogicUnit source, MainLogicUnit owner, Skill skill, int buffID, object[] args = null) : base(source, owner, skill, buffID, args)
    {
    }

    public override void LogicInit()
    {
        base.LogicInit();

        HPCureBuffConfig hcbc = config as HPCureBuffConfig;
        currHPpercent = hcbc.cureHPpercent;
    }

    protected override void Tick()
    {
        base.Tick();
        if (owner.stateType == EUnitStateType.Alive)
        {
            owner.GetCureByBuff(owner.unitData.unitCfg.hp * currHPpercent / 100, this);
        }
    }
}