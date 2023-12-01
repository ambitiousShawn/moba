using ShawnFramework.ShawMath;
using System;

public class TowerLogic : MainLogicUnit
{
    public int towerID;
    public int towerIndex;


    public TowerLogic(TowerData ud) : base(ud)
    {
        towerID = ud.towerID;
        towerIndex = ud.towerIndex;
        unitType = EUnitType.Tower;
        pathPrefix = "tower";
    }

    public override void LogicTick()
    {
        base.LogicTick();
        TickAI();
    }

    int AIIntervel = 200;
    int AIIntervelCounter = 0;
    void TickAI()
    {
        AIIntervelCounter += ClientConfig.ClientLogicFrameDeltaTimeMS;
        if (AIIntervelCounter >= AIIntervel)
        {
            AIIntervelCounter -= AIIntervel;
            MainLogicUnit unit = CalcSkillSelectTarget.FindSingleTargetByConfig(this, skillArr[0].config.targetConf, ShawVector3.zero);
            if (unit != null)
            {
                // 检测到存在能攻击的单位
                mainViewUnit.SetAtkSkillRange(true, skillArr[0].config.targetConf.selectRange);
                if (CanReleaseSkill(unitData.unitCfg.skillArr[0]))
                {
                    skillArr[0].ReleaseSkill(ShawVector3.zero);
                }
            }
            else
            {
                mainViewUnit.SetAtkSkillRange(false);
            }
        }
    }
}