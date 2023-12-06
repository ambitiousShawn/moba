public class TowerView : MainViewUnit
{
    TowerLogic tower;

    public override void ViewInit(BaseLogicUnit logicUnit)
    {
        base.ViewInit(logicUnit);
        tower = logicUnit as TowerLogic;
    }

    // 不同建筑的爆炸
    public void DestoryTower(MainLogicUnit selfUnit)
    {
        RemoveUIItem();
        Destroy(gameObject, 1f);
    }
}