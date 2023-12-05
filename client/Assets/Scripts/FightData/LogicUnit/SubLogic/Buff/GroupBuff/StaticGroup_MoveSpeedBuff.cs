using System.Collections.Generic;
using ShawnFramework.ShawMath;
using ShawnFramework.ShawLog;

public class StaticGroup_MoveSpeedBuff : BuffLogic
{
    ShawInt speedOffset;

    public StaticGroup_MoveSpeedBuff(MainLogicUnit source, MainLogicUnit owner, Skill skill, int buffID, object[] args = null)
        : base(source, owner, skill, buffID, args)
    {
    }

    public override void LogicInit()
    {
        base.LogicInit();

        MoveSpeedBuffConfig msbc = config as MoveSpeedBuffConfig;
        speedOffset = msbc.amount;

        targetList = new List<MainLogicUnit>();

        switch (msbc.staticPosType)
        {
            case EStaticPosType.SkillCasterPos:
                LogicPos = source.LogicPos;
                break;
            case EStaticPosType.SkillLockTargetPos:
                LogicPos = skill.lockTarget.LogicPos;
                break;
            case EStaticPosType.BulletHitTargetPos:
                LogicPos = (ShawVector3)args[1];
                break;
            case EStaticPosType.UIInputPos:
                LogicPos = source.LogicPos + skill.skillArgs;
                break;
            case EStaticPosType.None:
            default:
                LogCore.Error("static buff pos error.");
                break;
        }
    }

    protected override void Start()
    {
        base.Start();

        targetList.AddRange(CalcSkillSelectTarget.FindMultipleTargetByConfig(source, config.impacter, LogicPos));
        ModifyGroupMoveSpeed(speedOffset, true);
    }

    protected override void Tick()
    {
        base.Tick();
        ModifyGroupMoveSpeed(-speedOffset);

        targetList.Clear();
        targetList.AddRange(CalcSkillSelectTarget.FindMultipleTargetByConfig(source, config.impacter, LogicPos));
        ModifyGroupMoveSpeed(speedOffset, false);
    }

    protected override void End()
    {
        base.End();

        ModifyGroupMoveSpeed(-speedOffset);
    }


    void ModifyGroupMoveSpeed(ShawInt offset, bool showJump = false)
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            ShawInt value = targetList[i].baseMoveSpeed * (offset / 100);
            targetList[i].ModifyMoveSpeed(value, this, showJump);
        }
    }
}
