using ShawnFramework.ShawMath;
using ShawnFramework.ShawnPhysics;

/// <summary>
/// 客户端通用配置
/// </summary>
public static class ClientConfig
{
    // 客户端逻辑帧时间间隔
    public const float ClientLogicFrameDeltaTimeSecond = 0.066f;

    public const int ScreenStandardWidth = 1920;
    public const int ScreenStandardHeight = 1080;
    public const int ScreenOPDis = 135;
    public const int SkillOPDis = 125;
    public const int SkillCancelDis = 500;
}

/// <summary>
/// 游戏中物体的配置类
/// </summary>
public class UnitConfig
{
    // base
    public int unitID;
    public string unitName;
    public string resName;

    // core
    public int hp;
    public int defense;
    public int attack;
    public int moveSpeed;

    // 碰撞体
    public ColliderConfig colliCfg;

    // 技能ID数组
    public int[] skillArr;
}

/// <summary>
/// 地图配置
/// </summary>
public class MapConfig
{
    public int mapID;

    public ShawVector3 blueBornPos;
    public ShawVector3 redBornPos;

    // 小兵相关配置
    public int soldierBornDelay;
    public int soldierBornInterval;
    public int soldierWaveInterval;
}


/// <summary>
/// 技能配置
/// </summary>
public class SkillConfig
{
    public int skillID;
    public string iconName;
    public string animName;
    public EReleaseModeType releaseModeType;
    public TargetConfig targetConf; // 目标配置
    public int cdTime; // 冷却时间
    public int spellTime; // 施法前摇（引导时间 ：ms）
    public int skillTime; // 技能全长时间（前摇 + 释放 + 后摇）
    public int damage; // 基础伤害值
    public int[] buffIDArr;

    public bool isNormalAttack; // 是否未普通攻击
}

#region 技能释放相关
/// <summary>
/// 技能释放的方式
/// </summary>
public enum EReleaseModeType
{
    None,
    Click,
    Position,
    Direction,
}

/// <summary>
/// 目标查找相关配置
/// </summary>
public class TargetConfig
{
    public ESkillTargetType skillTargetType; // 技能目标选择类型
    public ESelectRuleType selectRuleType;
    public EUnitType[] targetUnits; // 可作用目标

    // -------- 辅助参数 --------
    public float selectRange; // 查找目标范围距离
    public float searchDis; // 移动攻击搜索距离
}

/// <summary>
/// 技能优先选择规则
/// </summary>
public enum ESelectRuleType
{
    None,

    // 单个目标选择规则
    MinHPValue, // 最低血量
    MinHPPercent, // 最低百分比血量
    TargetClosestSingle, // 靠近目标角色最近的单个选择
    PositionClosestSingle, // 靠近某个位置的单个选择

    // 多个目标选择规则
    TargetClosestMultiple, // 靠近目标角色最近的多个选择
    PositionClosestMultiple, // 靠近某个位置的多个选择

    AllHero, // 所有英雄单位
}

public enum ESkillTargetType
{
    Dynamic, // 非指向性
    Friend, // 对友军释放
    Enemy, // 对敌方释放
}
#endregion