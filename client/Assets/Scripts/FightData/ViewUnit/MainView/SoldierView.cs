using System;

public class SoldierView : MainViewUnit
{
    SoldierLogic soldier;
    public override void ViewInit(BaseLogicUnit logicUnit)
    {
        base.ViewInit(logicUnit);
        soldier = logicUnit as SoldierLogic;
    }

    protected override void Update()
    {
        base.Update();

        if (soldier.stateType == EUnitStateType.Dead)
        {
            Destroy(gameObject, 1f);
            RemoveUIItem();
        }
    }
}