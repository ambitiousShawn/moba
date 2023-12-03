public class BuffView : BaseViewUnit
{
    BuffLogic buff;

    public override void Init(BaseLogicUnit logicUnit)
    {
        base.Init(logicUnit);
        this.buff = logicUnit as BuffLogic;

        if (buff.config.staticPosType != EStaticPosType.None )
        {
            // 固定位置Buff
            transform.position = buff.LogicPos.ConvertViewVector3();
            transform.rotation = CalcRotation(buff.LogicDir.ConvertViewVector3());
        }
    }

    // 空函数覆盖位置与方向的更新
    protected override void Update() { }

    public void DestroyBuff()
    {
        Destroy(gameObject, 0.1f);
    }
}