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

/// <summary>
/// 英雄与玩家绑定数据
/// </summary>
public class HeroData : LogicUnitData
{
    public int heroID;
    public int posIndex;
    public string userName;
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
