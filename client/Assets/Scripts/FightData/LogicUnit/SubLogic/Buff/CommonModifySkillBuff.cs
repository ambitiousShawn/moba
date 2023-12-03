using static UnityEngine.UI.GridLayoutGroup;
using UnityEditor;

public class CommonModifySkillBuffConfig : BuffConfig
{
    public int originalID;
    public int replaceID;
}

public class CommonModifySkillBuff : BuffLogic
{
    public int originalID;
    public int replaceID;
    private Skill modifySkill;

    public CommonModifySkillBuff(MainLogicUnit source, MainLogicUnit owner, Skill skill, int buffID, object[] args = null)
        : base(source, owner, skill, buffID, args)
    {
    }

    public override void LogicInit()
    {
        base.LogicInit();

        CommonModifySkillBuffConfig mabc = config as CommonModifySkillBuffConfig;
        originalID = mabc.originalID;
        replaceID = mabc.replaceID;
        modifySkill = owner.GetSkillByID(originalID);
    }

    protected override void Start()
    {
        base.Start();

        modifySkill.ReplaceSkillConfig(replaceID);
        modifySkill.SpellSuccCallback += ReplaceSkillReleaseDone;
    }

    void ReplaceSkillReleaseDone(Skill skillReleased)
    {
        if (skillReleased.config.isNormalAttack)
        {
            state = SubUnitState.End;
        }
    }

    protected override void End()
    {
        base.End();
        modifySkill.ReplaceSkillConfig(originalID);
        modifySkill.SpellSuccCallback -= ReplaceSkillReleaseDone;
    }
}
