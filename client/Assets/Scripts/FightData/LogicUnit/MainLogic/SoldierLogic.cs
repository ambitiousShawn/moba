using ShawnFramework.ShawMath;
/// <summary>
/// 英雄逻辑层
/// </summary>
public class SoldierLogic : MainLogicUnit
{
    public int soldierID; // 小兵ID
    public int waveIndex; // 波次数
    public int orderIndex; // 当前波次的第几个
    public string soldierName; 

    public SoldierLogic(SoldierData ud) : base(ud)
    {
        soldierID = ud.soldierID;
        waveIndex = ud.waveIndex;
        orderIndex = ud.orderIndex;

        unitType = EUnitType.Soldier;
        unitName = $"{ud.unitCfg.unitName}_w:{waveIndex}_o:{orderIndex}";
        pathPrefix = "charactor";
    }


    ShawInt sqrSearchDis; // 搜索距离的平方
    TargetConfig cfg;        // 目标配置信息
    public override void LogicInit()
    {
        base.LogicInit();
        sqrSearchDis = (ShawInt)skillArr[0].config.targetConf.searchDis * (ShawInt)skillArr[0].config.targetConf.searchDis;
        cfg = skillArr[0].config.targetConf;    //  获得普通攻击的技能配置
        
        // 开局按照默认规则行进
        if (IsTeam(ETeamType.Blue))
        {
            InputDir = ShawVector3.right;
        }
        else
        {
            InputDir = ShawVector3.left;
        }
    }

    private int AITickInterval = 5;
    private int AITickIntervalCounter = 0;
    public override void LogicTick()
    {
        base.LogicTick();
        
        if (AITickIntervalCounter < AITickInterval)
        {
            AITickIntervalCounter += 1;
            return;
        }
        AITickIntervalCounter = 0;

        // 每5个逻辑帧执行一次AI
        if (CanReleaseSkill(unitData.unitCfg.skillArr[0]))
        {
            MainLogicUnit target = CalcSkillSelectTarget.FindSingleTargetByConfig(this, cfg, ShawVector3.zero);
            
            if (target != null)
            {
                // 存在目标，开始普通攻击
                InputDir = ShawVector3.zero;    // 先停下来
                skillArr[0].ReleaseSkill(ShawVector3.zero);
            }
            else
            {
                // 不存在目标
                target = CalcSkillSelectTarget.FindMinDistanceEnemyTarget(this, cfg);
                if (target != null)
                {
                    // 向当前距离最近的单位移动
                    ShawVector3 offset = target.LogicPos - LogicPos;
                    ShawInt sqrDistance = offset.sqrMagnitude;
                    if (sqrDistance < sqrSearchDis)
                    {
                        // 找到最近目标，向着目标移动
                        InputDir = offset.normalized;
                    }
                    else
                    {
                        // 未找到最近目标，顺着面前移动
                        if (IsTeam(ETeamType.Blue))
                        {
                            InputDir = ShawVector3.right;
                        }
                        else
                        {
                            InputDir = ShawVector3.left;
                        }
                    }
                }
            }
        }
    }
}