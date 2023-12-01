using ShawnFramework.CommonModule;
using ShawnFramework.ShawMath;
/// <summary>
/// 技能单位
/// </summary>
public class Skill
{
    public int skillID;
    public SkillConfig config;
    public ShawVector3 skillArgs;
    public MainLogicUnit lockTarget;
    public ESkillState skillState = ESkillState.Before;

    public ShawInt spellTime; // 前摇时间
    public ShawInt skillTime; // 技能总时间

    public MainLogicUnit owner;

    public Skill(int skillID, MainLogicUnit owner)
    {
        this.skillID = skillID;

        config = AssetsSvc.Instance.GetSkillConfigByID(skillID);
        spellTime = config.spellTime;
        skillTime = config.skillTime;

        if (config.isNormalAttack)
        {
            owner.InitAttackSpeedRate(1000 / skillTime);
        }
        this.owner = owner;
    }

    /// <summary>
    /// 释放技能
    /// </summary>
    /// <param name="skillArgs"></param>
    public void ReleaseSkill(ShawVector3 skillArgs)
    {

    }
}

public enum ESkillState
{
    None,
    Before, // Before
    SpellBefore, // 施法前摇
    SpellAfter, // 施法后摇
}