/// <summary>
/// ”¢–€ ”Õº≤„
/// </summary>
public class HeroView : MainViewUnit
{

    HeroLogic heroLogic;
    public override void Init(BaseLogicUnit logicUnit)
    {
        base.Init(logicUnit);

        heroLogic = logicUnit as HeroLogic;
    }
}