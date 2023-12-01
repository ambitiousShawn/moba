using ShawnFramework.ShawLog;
using ShawnFramework.ShawMath;
using System;
using System.Collections.Generic;

public static class CalcSkillSelectTarget
{
    public static HeroLogic[] blueTeamHero;
    public static HeroLogic[] redTeamHero;
    public static TowerLogic[] blueTeamTower;
    public static TowerLogic[] redTeamTower;
    public static List<SoldierLogic> blueTeamSoldier = new List<SoldierLogic>();
    public static List<SoldierLogic> redTeamSoldier = new List<SoldierLogic>();

    /// <summary>
    /// 寻找附近最近的敌人
    /// </summary>
    /// <param name="self"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static MainLogicUnit FindMinDisEnemyTarget(MainLogicUnit self, TargetConfig config)
    {
        MainLogicUnit target = null;
        List<MainLogicUnit> targetTeam = GetTargetTeam(self, config);

        int count = targetTeam.Count;
        ShawVector3 selfPos = self.LogicPos;
        ShawInt len = 0;
        for (int i = 0; i < count; i++)
        {
            ShawInt sumRaius = targetTeam[i].unitData.unitCfg.colliCfg.mRadius + self.unitData.unitCfg.colliCfg.mRadius;
            ShawInt tempLen = (targetTeam[i].LogicPos - selfPos).magnitude - sumRaius;
            if (len == 0 || tempLen < len)
            {
                len = tempLen;
                target = targetTeam[i];
            }
        }
        return target;
    }

    /// <summary>
    /// 通过技能选择配置规则选择单个目标单位
    /// </summary>
    /// <param name="self"></param>
    /// <param name="config"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static MainLogicUnit FindSingleTargetByConfig(MainLogicUnit self, TargetConfig config, ShawVector3 pos)
    {
        List<MainLogicUnit> searchUnits = GetTargetTeam(self, config);
        switch (config.selectRuleType)
        {
            case ESelectRuleType.MinHPValue:
                break;
            case ESelectRuleType.MinHPPercent:
                break;
            case ESelectRuleType.TargetClosestSingle:
                return FindMinDistanceTargetInUnits(self, searchUnits, (ShawInt)config.selectRange);
            case ESelectRuleType.PositionClosestSingle:
                return FindMinDisTargetInPos(pos, searchUnits, (ShawInt)config.selectRange);
            default:
                LogCore.Warn("select target error, check you target config.");
                break;
        }
        return null;
    }

    // 得到所有符合当前选择规则的目标逻辑单元
    static List<MainLogicUnit> GetTargetTeam(MainLogicUnit self, TargetConfig config)
    {
        List<MainLogicUnit> targetLst = new List<MainLogicUnit>();
        if (self.IsTeam(ETeamType.Blue))
        {
            if (config.skillTargetType == ESkillTargetType.Friend)
            {
                //blue
                if (ContainTargetType(config, EUnitType.Hero))
                {
                    targetLst.AddRange(blueTeamHero);
                }
                if (ContainTargetType(config, EUnitType.Tower))
                {
                    targetLst.AddRange(blueTeamTower);
                }
                if (ContainTargetType(config, EUnitType.Soldier))
                {
                    targetLst.AddRange(blueTeamSoldier);
                }
            }
            else if (config.skillTargetType == ESkillTargetType.Enemy)
            {
                //red
                if (ContainTargetType(config, EUnitType.Hero))
                {
                    targetLst.AddRange(redTeamHero);
                }
                if (ContainTargetType(config, EUnitType.Tower))
                {
                    targetLst.AddRange(redTeamTower);
                }
                if (ContainTargetType(config, EUnitType.Soldier))
                {
                    targetLst.AddRange(redTeamSoldier);
                }
            }
            else
            {
                LogCore.Warn("TargetTeamEnum is Unknow.");
            }
        }
        else if (self.IsTeam(ETeamType.Red))
        {
            if (config.skillTargetType == ESkillTargetType.Friend)
            {
                //red
                if (ContainTargetType(config, EUnitType.Hero))
                {
                    targetLst.AddRange(redTeamHero);
                }
                if (ContainTargetType(config, EUnitType.Tower))
                {
                    targetLst.AddRange(redTeamTower);
                }
                if (ContainTargetType(config, EUnitType.Soldier))
                {
                    targetLst.AddRange(redTeamSoldier);
                }
            }
            else if (config.skillTargetType == ESkillTargetType.Enemy)
            {
                //blue
                if (ContainTargetType(config, EUnitType.Hero))
                {
                    targetLst.AddRange(blueTeamHero);
                }
                if (ContainTargetType(config, EUnitType.Tower))
                {
                    targetLst.AddRange(blueTeamTower);
                }
                if (ContainTargetType(config, EUnitType.Soldier))
                {
                    targetLst.AddRange(blueTeamSoldier);
                }
            }
            else
            {
                LogCore.Warn("TargetTeamEnum is Unknow.");
            }
        }
        else
        {
            LogCore.Warn("Self Hero TeamEnum is Unknow.");
        }

        //过滤掉死亡单位
        for (int i = targetLst.Count - 1; i >= 0; --i)
        {
            // TODO:测试阶段的Bug，当场上存在两名以上玩家会没问题
            if (targetLst[i] == null || targetLst[i].stateType == EUnitStateType.Dead)
            {
                targetLst.RemoveAt(i);
            }
        }
        return targetLst;
    }

    // 判断目标配置中是否包含对应类型单位
    static bool ContainTargetType(TargetConfig config, EUnitType targetType)
    {
        for (int i = 0; i < config.targetUnits.Length; i++)
        {
            if (config.targetUnits[i] == targetType)
            {
                return true;
            }
        }
        return false;
    }

    // 在List目标实体中查找距离最近的单个逻辑单元
    static MainLogicUnit FindMinDistanceTargetInUnits(MainLogicUnit self, List<MainLogicUnit> searchUnits, ShawInt selectRange)
    {
        if (searchUnits == null || selectRange < 0)
        {
            return null;
        }
        MainLogicUnit target = null;
        int count = searchUnits.Count;
        ShawVector3 selfPos = self.LogicPos;
        ShawInt distance = 0;
        for (int i = 0; i < count; i++)
        {
            ShawInt sumRadius = searchUnits[i].unitData.unitCfg.colliCfg.mRadius + self.unitData.unitCfg.colliCfg.mRadius;
            ShawInt tempDis = (searchUnits[i].LogicPos - selfPos).magnitude - sumRadius;
            if (distance == 0 || tempDis < distance)
            {
                distance = tempDis;
                target = searchUnits[i];
            }
        }
        // 在目标范围内
        if (distance < selectRange)
        {
            return target;
        }
        return null;
    }

    // 在List目标实体中查找距离最近的单个逻辑单元
    static MainLogicUnit FindMinDisTargetInPos(ShawVector3 pos, List<MainLogicUnit> targetTeam, ShawInt range)
    {
        if (targetTeam == null)
        {
            return null;
        }
        MainLogicUnit target = null;
        int count = targetTeam.Count;
        ShawInt distance = 0;
        for (int i = 0; i < count; i++)
        {
            ShawInt radius = targetTeam[i].unitData.unitCfg.colliCfg.mRadius;
            ShawInt tempLen = (targetTeam[i].LogicPos - pos).magnitude - radius;
            if (distance == 0 || tempLen < distance)
            {
                distance = tempLen;
                target = targetTeam[i];
            }
        }

        if (distance < range)
        {
            return target;
        }
        return null;
    }
}