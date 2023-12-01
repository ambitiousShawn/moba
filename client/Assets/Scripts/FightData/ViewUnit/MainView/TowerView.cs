public class TowerView : MainViewUnit
{
    TowerLogic tower;

    public override void Init(BaseLogicUnit logicUnit)
    {
        base.Init(logicUnit);
        tower = logicUnit as TowerLogic;
    }

    // 不同建筑的爆炸
    public void DestoryTower()
    {
        Destroy(gameObject, 1f);
    }
}