using ShawnFramework.ShawMath;
using XLua;
/// <summary>
/// 基础逻辑单元数据结构
/// </summary>
public class LogicUnitData
{
    public ETeamType teamType;
    public ShawVector3 bornPos;
    public UnitConfig unitCfg;
}

public class HeroData : LogicUnitData
{
    public int heroID;
    public int posIndex;
    public string userName;
}

public class SoldierData : LogicUnitData
{
    public int soldierID;
    public int waveIndex;
    public int orderIndex;
    public string soldierName;
}

public class TowerData : LogicUnitData
{
    public int towerID;
    public int towerIndex;
    public string towerName;
}

// 队伍类型
[LuaCallCSharp]
public enum ETeamType
{
    None,
    Blue,
    Red,
    Neutral, // 中立
}

// 单元类型
[LuaCallCSharp]
public enum EUnitType
{
    Hero,    // 英雄
    Soldier, // 小兵
    Tower,   // 防御塔
}

[LuaCallCSharp]
public enum EUnitStateType
{
    Alive,  // 活跃
    Dead,   // 阵亡
}

public enum EAbnormalState
{
    None,
    Silenced,       // 沉默
    Knockup,        // 击飞
    Stunned,        // 眩晕

    Invincible,     //
    Restricted,
}
