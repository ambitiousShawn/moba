using ShawnFramework.CommonModule;
using ShawnFramework.ShawMath;
/// <summary>
/// ���ܵ�λ
/// </summary>
public class Skill
{
    public int skillID;
    public SkillConfig config;
    public ShawVector3 skillArgs;
    public MainLogicUnit lockTarget;
    public ESkillState skillState = ESkillState.Before;

    public ShawInt spellTime; // ǰҡʱ��
    public ShawInt skillTime; // ������ʱ��

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
    /// �ͷż���
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
    SpellBefore, // ʩ��ǰҡ
    SpellAfter, // ʩ����ҡ
}