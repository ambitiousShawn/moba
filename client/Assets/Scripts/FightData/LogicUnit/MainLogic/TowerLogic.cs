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
                // ��⵽�����ܹ����ĵ�λ
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
            // ���������ݻ�
            MainLogicUnit selfUnit = FightManager.Instance.GetSelfHero(Launcher.Instance.SelfIndex);
            Action<bool> req_battle_end = LuaManager.Instance.GlobalLuaEnv.Global.Get<Action<bool>>("EndBattle"); // ����lua��ȫ����Ӧ����
            if (towerID == 1002)
            {
                LogCore.ColorLog("�췽ʤ��", ELogColor.Cyan);
                // ֪ͨBattleSystem����ս���߼�
                req_battle_end?.Invoke(!selfUnit.IsTeam(ETeamType.Blue));
            }
            else if (towerID == 2002)
            {
                LogCore.ColorLog("����ʤ��", ELogColor.Cyan);
                // ֪ͨBattleSystem����ս���߼�
                req_battle_end?.Invoke(!selfUnit.IsTeam(ETeamType.Red));
            }

            TowerView view = mainViewUnit as TowerView;
            view.DestoryTower(selfUnit);
        }
    }
}