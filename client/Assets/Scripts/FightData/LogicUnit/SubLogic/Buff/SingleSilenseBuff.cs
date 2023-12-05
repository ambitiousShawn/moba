// µ¥Ìå³ÁÄ¬
public class SingleSilenseBuff : BuffLogic
{
    public SingleSilenseBuff(MainLogicUnit source, MainLogicUnit owner, Skill skill, int buffID, object[] args = null) : base(source, owner, skill, buffID, args)
    {
    }

    protected override void Start()
    {
        base.Start();
        owner.SilenceCount += 1;
    }

    protected override void End()
    {
        base.End();
        owner.SilenceCount -= 1;
    }
}