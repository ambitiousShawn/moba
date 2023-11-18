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

    // ��ײ��
    public ColliderConfig colliCfg;

    // ����ID����
    public int[] skillArr;
}

public class MapConfig
{
    public int mapID;

    public ShawVector3 blueBornPos;
    public ShawVector3 redBornPos;

    // С���������
    public int soldierBornDelay;
    public int soldierBornInterval;
    public int soldierWaveInterval;
}