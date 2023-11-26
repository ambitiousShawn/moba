using ShawnFramework.ShawMath;
using ShawnFramework.ShawnPhysics;

/// <summary>
/// �ͻ���ͨ������
/// </summary>
public static class ClientConfig
{
    // �ͻ����߼�֡ʱ����
    public const float ClientLogicFrameDeltaTimeSecond = 0.066f;

    public const int ScreenStandardWidth = 1920;
    public const int ScreenStandardHeight = 1080;
    public const int ScreenOPDis = 135;
    public const int SkillOPDis = 125;
    public const int SkillCancelDis = 500;
}

/// <summary>
/// ��Ϸ�������������
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

    // ��ײ��
    public ColliderConfig colliCfg;

    // ����ID����
    public int[] skillArr;
}

/// <summary>
/// ��ͼ����
/// </summary>
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


/// <summary>
/// ��������
/// </summary>
public class SkillConfig
{
    public int skillID;
    public string iconName;
    public string animName;
    public EReleaseModeType releaseModeType;
    public TargetConfig targetConf; // Ŀ������
    public int cdTime; // ��ȴʱ��
    public int spellTime; // ʩ��ǰҡ������ʱ�� ��ms��
    public int skillTime; // ����ȫ��ʱ�䣨ǰҡ + �ͷ� + ��ҡ��
    public int damage; // �����˺�ֵ
    public int[] buffIDArr;

    public bool isNormalAttack; // �Ƿ�δ��ͨ����
}

#region �����ͷ����
/// <summary>
/// �����ͷŵķ�ʽ
/// </summary>
public enum EReleaseModeType
{
    None,
    Click,
    Position,
    Direction,
}

/// <summary>
/// Ŀ������������
/// </summary>
public class TargetConfig
{
    public ESkillTargetType skillTargetType; // ����Ŀ��ѡ������
    public ESelectRuleType selectRuleType;
    public EUnitType[] targetUnits; // ������Ŀ��

    // -------- �������� --------
    public float selectRange; // ����Ŀ�귶Χ����
    public float searchDis; // �ƶ�������������
}

/// <summary>
/// ��������ѡ�����
/// </summary>
public enum ESelectRuleType
{
    None,

    // ����Ŀ��ѡ�����
    MinHPValue, // ���Ѫ��
    MinHPPercent, // ��Ͱٷֱ�Ѫ��
    TargetClosestSingle, // ����Ŀ���ɫ����ĵ���ѡ��
    PositionClosestSingle, // ����ĳ��λ�õĵ���ѡ��

    // ���Ŀ��ѡ�����
    TargetClosestMultiple, // ����Ŀ���ɫ����Ķ��ѡ��
    PositionClosestMultiple, // ����ĳ��λ�õĶ��ѡ��

    AllHero, // ����Ӣ�۵�λ
}

public enum ESkillTargetType
{
    Dynamic, // ��ָ����
    Friend, // ���Ѿ��ͷ�
    Enemy, // �Եз��ͷ�
}
#endregion