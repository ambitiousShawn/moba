using ShawnFramework.ShawMath;
using ShawnFramework.ShawnPhysics;

/// <summary>
/// �ͻ���ͨ������
/// </summary>
public static class ClientConfig
{
    // �ͻ����߼�֡ʱ����
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
/// ��Ϸ�������������
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

    // ��ײ��
    public ColliderConfig colliCfg;

    // ����ID����
    public int[] pasvBuff;
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

    // �������������
    public int[] towerIDArr;
    public ShawVector3[] towerPosArr;

    // С���������
    public int soldierBornDelay;
    public int soldierBornInterval;
    public int soldierWaveInterval;
    public int[] blueSoldierIDArr;
    public ShawVector3[] blueSoldierPosArr;
    public int[] redSoldierIDArr;
    public ShawVector3[] redSoldierPosArr;
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

    public bool isNormalAttack; // �Ƿ�δ��ͨ����

    public BulletConfig bulletConf; // ��������
    public int[] buffIDArr; // ���Buff

    //��Ч���
    public string audio_start;//ʩ����ʼ
    public string audio_work;//ʩ���ɹ�
    public string audio_hit;//ʩ������
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

#region ����������Buff

// ��������
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
    UIDirection,//uiָ������
    UIPosition,//uiָ��λ��
    SkillTarget,//��ǰ����Ŀ��
    BuffSearch,
    //TODO
}

public class BuffConfig
{
    public int buffID;
    public string buffName;
    public EBuffType buffType;
    public EAttachType attacher; // buff����Ŀ��
    public TargetConfig impacter; // buff����Ŀ��

    public int buffDelay;
    public int buffInterval;
    public int buffDuration;//��������delay��0����Ч1�Σ�-1��������Ч
    public EStaticPosType staticPosType;

    public string buffAudio;
    public string buffEffect;
    public string hitTickAudio;
}

public enum EBuffType
{
    None,
    HPCure,//����

    ModifySkill,
    MoveSpeedUp,//�������buff
    ArthurMark,//Arthur1���ܵı���˺�Buff
    Silense,//��Ĭ
    TargetFlashMove,
    DirectionFlashMove,//TODO
    ExecuteDamage,
    Knockup_Group,//Ⱥ�����

    Stun_Single_DynamicTime,

    //houyiר��buff
    HouyiActiveSkillModify,//Houyi���������޸�buff
    Scatter,

    HouyiPasvAttackSpeed,//Houyi�������ټӳ�buff
    HouyiPasvSkillModify,//Houyi���������޸�Buff
    HouyiPasvMultiArrow,//Houyi�������ܶ������Buff
    HouyiMixedMultiScatter,//��϶��������ɢ��


    MoveSpeed_DynamicGroup,//��̬Ⱥ������Buff
    MoveSpeed_StaticGroup,//��̬Ⱥ������buff
    Damage_DynamicGroup,//��̬Ⱥ���˺�
    Damage_StaticGroup,
    MoveAttack,//�ƶ�����
}

public enum EStaticPosType
{
    None,
    SkillCasterPos,//Buff��������ʩ���ߵ�λ��
    SkillLockTargetPos,//Buff������������Ŀ���λ��
    BulletHitTargetPos,//�ӵ�����Ŀ���λ��
    UIInputPos,//UI����λ����Ϣ
}

public enum EAttachType
{
    None,
    Caster,//Arthur��1���ܼ���buff
    Target,//Arthur��1���ܳ�Ĭbuff

    Indie,//Arthur���в����ĳ�����Χ�˺�

    Bullet,//Houyi��������Ŀ��ʱ�����ķ�Χ�˺�
}
#endregion