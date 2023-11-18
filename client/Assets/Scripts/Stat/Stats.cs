using ShawnFramework.ShawMath;
using XLua;
/// <summary>
/// 基础逻辑单元数据结构
/// </summary>
public class LogicUnitData
{
    public ETeamType teamType;
    public ShawVector3 bornPos;
    public HeroConfig heroCfg;
}

/// <summary>
/// 英雄逻辑单位的数据
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
    Hero,
    Soldier,
    Tower,
}
