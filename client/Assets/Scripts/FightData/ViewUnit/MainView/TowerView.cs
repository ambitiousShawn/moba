public class TowerView : MainViewUnit
{
    TowerLogic tower;

    public override void Init(BaseLogicUnit logicUnit)
    {
        base.Init(logicUnit);
        tower = logicUnit as TowerLogic;
    }

    // ��ͬ�����ı�ը
    public void DestoryTower()
    {
        Destroy(gameObject, 1f);
    }
}