using GameProtocol;
using ShawnFramework.ShawLog;
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

    public override void LogicUninit()
    {
        base.LogicUninit();

        if (stateType == EUnitStateType.Dead)
        {
            // 防御塔被摧毁
            MainLogicUnit selfUnit = FightManager.Instance.GetSelfHero(Launcher.Instance.SelfIndex);
            Action<bool> req_battle_end = LuaManager.Instance.GlobalLuaEnv.Global.Get<Action<bool>>("EndBattle"); // 调用lua的全局响应函数
            if (towerID == 1002)
            {
                LogCore.ColorLog("红方胜利", ELogColor.Cyan);
                // 通知BattleSystem结束战斗逻辑
                req_battle_end?.Invoke(!selfUnit.IsTeam(ETeamType.Blue));
            }
            else if (towerID == 2002)
            {
                LogCore.ColorLog("蓝方胜利", ELogColor.Cyan);
                // 通知BattleSystem结束战斗逻辑
                req_battle_end?.Invoke(!selfUnit.IsTeam(ETeamType.Red));
            }

            TowerView view = mainViewUnit as TowerView;
            view.DestoryTower(selfUnit);
        }
    }
}