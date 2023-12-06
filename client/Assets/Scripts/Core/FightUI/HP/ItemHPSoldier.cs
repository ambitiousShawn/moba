using ShawnFramework.CommonModule;
using UnityEngine;
using UnityEngine.UI;

public class ItemHPSoldier : SceneHPItem
{
    public Image ImgState;
    public override void InitItem(MainLogicUnit unit, Transform root, int hp)
    {
        base.InitItem(unit, root, hp);
        ImgState.gameObject.SetActive(false);
        if (IsFriend)
        {
            ImgPrg.sprite = AssetsSvc.Instance.LoadSprite("hero", "selfteamhpfg", 1);
        }
        else
        {
            ImgPrg.sprite = AssetsSvc.Instance.LoadSprite("hero", "enemyteamhpfg", 1);
        }
    }

    public override void SetStateIcon(EAbnormalState state, bool show)
    {
        base.SetStateIcon(state, show);
        if (!show)
        {
            ImgState.gameObject.SetActive(false);
        }
        else
        {
            //血条下方图标显示
            switch (state)
            {
                case EAbnormalState.Silenced:
                    ImgState.sprite = AssetsSvc.Instance.LoadSprite("hero", "silenceIcon", 1);
                    break;
                case EAbnormalState.Knockup:
                    ImgState.sprite = AssetsSvc.Instance.LoadSprite("hero", "stunIcon", 1);
                    break;
                case EAbnormalState.Stunned:
                    ImgState.sprite = AssetsSvc.Instance.LoadSprite("hero", "stunIcon", 1);
                    break;
                case EAbnormalState.Invincible:
                case EAbnormalState.Restricted:
                case EAbnormalState.None:
                default:
                    break;
            }

            ImgState.gameObject.SetActive(true);
            ImgState.SetNativeSize();
        }
    }
}