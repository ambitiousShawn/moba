using ShawnFramework.CommonModule;
using UnityEngine;

public class ItemHPTower : SceneHPItem
{
    public override void InitItem(MainLogicUnit unit, Transform root, int hp)
    {
        base.InitItem(unit, root, hp);
        if (IsFriend)
        {
            ImgPrg.sprite = AssetsSvc.Instance.LoadSprite("tower", "selftowerhpfg", 1) ;
        }
        else
        {
            ImgPrg.sprite = AssetsSvc.Instance.LoadSprite("tower", "enemytowerhpfg", 1);
        }
    }
}