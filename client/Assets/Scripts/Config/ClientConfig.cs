using ShawnFramework.ShawMath;
using ShawnFramework.ShawnPhysics;

/// <summary>
/// 客户端通用配置
/// </summary>
public static class ClientConfig
{
    // 客户端逻辑帧时间间隔
    public const float ClientLogicFrameDeltaTimeSecond = 0.066f;
    public const int ClientLogicFrameDeltaTimeMS = 66;

    public const int ScreenStandardWidth = 1920;
    public const int ScreenStandardHeight = 1080;
    public const int ScreenOPDis = 135;
    public const int SkillOPDis = 125;
    public const int SkillCancelDis = 500;

    public const int CommonMoveAttackBuffID = 90000;
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
    public ShawInt hitHeight;

    // core
    public int hp;
    public int defense;
    public int attack;
    public int moveSpeed;

    // 碰撞体
    public ColliderConfig colliCfg;

    // 技能ID数组
    public int[] pasvBuff;
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

    // 防御塔相关配置
    public int[] towerIDArr;
    public ShawVector3[] towerPosArr;

    // 小兵相关配置
    public int soldierBornDelay;
    public int soldierBornInterval;
    public int soldierWaveInterval;
    public int[] blueSoldierIDArr;
    public ShawVector3[] blueSoldierPosArr;
    public int[] redSoldierIDArr;
    public ShawVector3[] redSoldierPosArr;
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

    public bool isNormalAttack; // 是否未普通攻击

    public BulletConfig bulletConf; // 弹道设置
    public int[] buffIDArr; // 添加Buff

    //音效相关
    public string audio_start;//施法开始
    public string audio_work;//施法成功
    public string audio_hit;//施法命中
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

#region 弹道配置与Buff

// 弹道配置
public class BulletConfig
{
    public EBulletType bulletType;
    public string bulletName;
    public string resPath;
    public float bulletSpeed;
    public float bulletSize;
    public float bulletHeight;
    public float bulletOffset;
    public int bulletDelay;//ms
    public bool canBlock;

    public TargetConfig impacter;
    public int bulletDuration;
}

public enum EBulletType
{
    UIDirection,//ui指定方向
    UIPosition,//ui指定位置
    SkillTarget,//当前技能目标
    BuffSearch,
    //TODO
}

public class BuffConfig
{
    public int buffID;
    public string buffName;
    public EBuffType buffType;
    public EAttachType attacher; // buff附着目标
    public TargetConfig impacter; // buff作用目标

    public int buffDelay;
    public int buffInterval;
    public int buffDuration;//（不包含delay）0：生效1次，-1：永久生效
    public EStaticPosType staticPosType;

    public string buffAudio;
    public string buffEffect;
    public string hitTickAudio;
}

public enum EBuffType
{
    None,
    HPCure,//治疗

    ModifySkill,
    MoveSpeedUp,//单体加速buff
    ArthurMark,//Arthur1技能的标记伤害Buff
    Silense,//沉默
    TargetFlashMove,
    DirectionFlashMove,//TODO
    ExecuteDamage,
    Knockup_Group,//群体击飞

    Stun_Single_DynamicTime,

    //houyi专区buff
    HouyiActiveSkillModify,//Houyi主动技能修改buff
    Scatter,

    HouyiPasvAttackSpeed,//Houyi被动攻速加成buff
    HouyiPasvSkillModify,//Houyi被动技能修改Buff
    HouyiPasvMultiArrow,//Houyi被动技能多重射击Buff
    HouyiMixedMultiScatter,//混合多重射击与散射


    MoveSpeed_DynamicGroup,//动态群体移速Buff
    MoveSpeed_StaticGroup,//静态群体移速buff
    Damage_DynamicGroup,//动态群体伤害
    Damage_StaticGroup,
    MoveAttack,//移动攻击
}

public enum EStaticPosType
{
    None,
    SkillCasterPos,//Buff所属技能施放者的位置
    SkillLockTargetPos,//Buff所属技能锁定目标的位置
    BulletHitTargetPos,//子弹命中目标的位置
    UIInputPos,//UI输入位置信息
}

public enum EAttachType
{
    None,
    Caster,//Arthur的1技能加速buff
    Target,//Arthur的1技能沉默buff

    Indie,//Arthur大招产生的持续范围伤害

    Bullet,//Houyi大招命中目标时产生的范围伤害
}
#endregion