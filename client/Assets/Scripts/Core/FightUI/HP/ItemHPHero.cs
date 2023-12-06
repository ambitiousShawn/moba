using ShawnFramework.CommonModule;
using UnityEngine;
using UnityEngine.UI;

public class ItemHPHero : ItemHPSoldier
{
    public Text TextName;
    public Transform hpMarkRoot;
    public int markCount;

    public override void InitItem(MainLogicUnit unit, Transform root, int hp)
    {
        base.InitItem(unit, root, hp);

        ImgState.gameObject.SetActive(false);
        TextName.text = unit.unitName;
        TextName.gameObject.SetActive(true);

        SetHPMark(hp);
    }

    void SetHPMark(int hp)
    {
        int count = hp / markCount;
        int childCount = hpMarkRoot.childCount;
        for (int i = 0; i < childCount; i++)
        {
            if (i < count)
            {
                hpMarkRoot.GetChild(i).gameObject.SetActive(true) ;
            }
            else
            {
                hpMarkRoot.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public override void SetStateIcon(EAbnormalState state, bool show)
    {
        base.SetStateIcon(state, show);

        if (!show)
        {
            TextName.gameObject.SetActive(true);
            ImgState.gameObject.SetActive(false);
        }
        else
        {
            switch (state)
            {
                case EAbnormalState.Silenced:
                    ImgState.sprite = AssetsSvc.Instance.LoadSprite("hero", "silenceState", 1);
                    break;
                case EAbnormalState.Knockup:
                    ImgState.sprite = AssetsSvc.Instance.LoadSprite("hero", "knockState", 1);
                    break;
                case EAbnormalState.Stunned:
                    ImgState.sprite = AssetsSvc.Instance.LoadSprite("hero", "stunState", 1);
                    break;
                //TODO
                case EAbnormalState.Invincible:
                case EAbnormalState.Restricted:
                case EAbnormalState.None:
                default:
                    break;
            }

            TextName.gameObject.SetActive(false);
            ImgState.gameObject.SetActive(true);
            ImgState.SetNativeSize();
        }
    }
}