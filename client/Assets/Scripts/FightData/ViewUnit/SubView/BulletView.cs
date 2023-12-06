public class BulletView : BaseViewUnit
{
    public override void ViewInit(BaseLogicUnit logicUnit)
    {
        base.ViewInit(logicUnit);
    }


    public void DestroyBullet()
    {
        Destroy(gameObject, 0.1f);
    }
}