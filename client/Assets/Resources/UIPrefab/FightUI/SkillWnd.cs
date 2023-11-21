

using GameProtocol;
using ShawnFramework.CommonModule;
using UnityEngine;

public partial class UGUI_PlayWnd : CommonListenerRoot
{
    public SkillItem skillItemA;
    public SkillItem skillItem1;
    public SkillItem skillItem2;
    public SkillItem skillItem3;

    public Transform ImgInfoRoot;
    public void InitSkillInfo()
    {
        BattleHeroData self = Launcher.Instance.BattleHeroDatas[Launcher.Instance.SelfIndex];
        UnitConfig heroCfg = AssetsSvc.Instance.GetHeroConfigByID(self.heroID);
        int[] skillArr = heroCfg.skillArr; // 拿到技能配置

        skillItemA.InitSkillItem(AssetsSvc.Instance.GetSkillConfigByID(skillArr[0]), 0);
        skillItem1.InitSkillItem(AssetsSvc.Instance.GetSkillConfigByID(skillArr[1]), 1);
        skillItem2.InitSkillItem(AssetsSvc.Instance.GetSkillConfigByID(skillArr[2]), 2);
        skillItem3.InitSkillItem(AssetsSvc.Instance.GetSkillConfigByID(skillArr[3]), 3);

        SetForbidState(false);
        ImgInfoRoot.gameObject.SetActive(false);
    }

    /// <summary>
    /// 被控制时无法释放技能
    /// </summary>
    /// <param name="state"></param>
    void SetForbidState(bool state)
    {
        skillItem1.SetForbidState(state);
        skillItem2.SetForbidState(state);
        skillItem3.SetForbidState(state);
    }
}
