public class SoldierView : MainViewUnit
{
    SoldierLogic soldier;
    public override void Init(BaseLogicUnit logicUnit)
    {
        base.Init(logicUnit);
        soldier = logicUnit as SoldierLogic;
    }

    protected override void Update()
    {
        base.Update();

        if (soldier.stateType == EUnitStateType.Dead)
        {
            Destroy(gameObject, 3f);
        }
    }
}