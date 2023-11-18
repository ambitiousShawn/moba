using ShawnFramework.ShawMath;
using ShawnFramework.ShawnPhysics;

public class HeroConfig
{
    // base
    public int heroID;
    public string heroName;
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