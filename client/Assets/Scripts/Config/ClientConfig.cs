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