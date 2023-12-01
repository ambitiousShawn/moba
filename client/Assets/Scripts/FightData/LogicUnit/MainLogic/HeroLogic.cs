/// <summary>
/// Ó¢ÐÛÂß¼­²ã
/// </summary>
public class HeroLogic : MainLogicUnit
{
    public int heroID;
    public int posIndex;
    public string userName;

    public HeroLogic(HeroData ud) : base(ud)
    {
        heroID = ud.heroID;
        posIndex = ud.posIndex;
        userName = ud.userName;

        unitType = EUnitType.Hero;
        unitName = ud.unitCfg.unitName + "_" + userName;
        pathPrefix = "charactor";
    }
}