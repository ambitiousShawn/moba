public class BulletView : BaseViewUnit
{
    public override void Init(BaseLogicUnit logicUnit)
    {
        base.Init(logicUnit);
    }


    public void DestroyBullet()
    {
        Destroy(gameObject, 0.1f);
    }
}