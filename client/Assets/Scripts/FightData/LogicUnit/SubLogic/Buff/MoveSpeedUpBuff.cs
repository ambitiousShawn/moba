
using ShawnFramework.ShawMath;

public class MoveSpeedBuffConfig : BuffConfig
{
    public int amount;//速度变化百分比
}

public class MoveSpeedUpBuff : BuffLogic
{
    private ShawInt speedOffset;

    public MoveSpeedUpBuff(MainLogicUnit source, MainLogicUnit owner, Skill skill, int buffID, object[] args = null)
        : base(source, owner, skill, buffID, args)
    {
    }

    public override void LogicInit()
    {
        base.LogicInit();

        MoveSpeedBuffConfig msbc = config as MoveSpeedBuffConfig;
        speedOffset = owner.baseMoveSpeed * ((ShawInt)msbc.amount / 100);
    }

    protected override void Start()
    {
        base.Start();
        owner.ModifyMoveSpeed(speedOffset, this, true);
    }

    protected override void End()
    {
        base.End();
        owner.ModifyMoveSpeed(-speedOffset, this, false);
    }
}