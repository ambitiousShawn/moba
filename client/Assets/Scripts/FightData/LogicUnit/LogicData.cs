using GameProtocol;
using ShawnFramework.CommonModule;
using ShawnFramework.ShawLog;
using ShawnFramework.ShawMath;
using ShawnFramework.ShawnPhysics;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �߼���Ԫ�����ӿ�
/// </summary>
public interface ILogic
{
    void LogicInit();
    void LogicTick();
    void LogicUninit();
}

/// <summary>
/// �����߼���Ԫ�������ͻ���
/// </summary>
public abstract class BaseLogicUnit : ILogic
{
    public string unitName;

    #region �߼�����(λ�ã��ٶȣ�����)
    // �߼�λ���Ƿ�ı�
    public bool isPosChanged = false;

    // �߼�λ��
    ShawVector3 logicPos;
    public ShawVector3 LogicPos
    {
        get { return logicPos; }
        set
        {
            logicPos = value;
            isPosChanged = true;
        }
    }

    // �߼������Ƿ�ı�
    public bool IsDirChanged = false;

    // �߼�����
    ShawVector3 logicDir;
    public ShawVector3 LogicDir
    {
        get { return logicDir; }
        set
        {
            logicDir = value;
            IsDirChanged = true;
        }
    }

    // �߼��ٶ�
    ShawInt logicMoveSpeed;
    public ShawInt LogicMoveSpeed
    {
        get { return logicMoveSpeed; }
        set { logicMoveSpeed = value; }
    }
    // �����ٶ�
    public ShawInt baseMoveSpeed;

    #endregion

    public abstract void LogicInit();

    public abstract void LogicTick();

    public abstract void LogicUninit();
}

/// <summary>
/// ��Ҫ�߼�ģ��(Ӣ�ۣ�С����������)
/// </summary>
public abstract class MainLogicUnit : BaseLogicUnit
{
    public LogicUnitData unitData;  // ����һ���߼�ģ������
    public EUnitStateType stateType;
    public EUnitType unitType;

    protected string pathPrefix = "";
    public MainViewUnit mainViewUnit;

    public MainLogicUnit(LogicUnitData ud)
    {
        unitData = ud;
        unitName = ud.unitCfg.unitName;
    }

    public override void LogicInit()
    {
        InitProperties(); // ���Գ�ʼ��(����ֵ,��������)
        InitSkill();      // ���ܳ�ʼ��
        InitMove();       // �ƶ���ʼ��

        // ���ֲ��ʼ��
        GameObject go = AssetsSvc.Instance.LoadPrefab(pathPrefix, unitData.unitCfg.resName, 1);
        mainViewUnit = go.AddComponent<HeroView>();
        if (mainViewUnit == null )
        {
            LogCore.Error("Get MainViewUnit Error:" + unitName);
        }
        mainViewUnit.Init(this);
        stateType = EUnitStateType.Alive;
    }

    public override void LogicTick()
    {
        TickSkill();
        TickMove();
    }

    public override void LogicUninit()
    {
        UnInitSkill();
        UnInitMove();
    }

    public void InputKey(OpKey key)
    {
        switch (key.keyType)
        {
            case EKeyType.Move:
                ShawInt x = ShawInt.zero;
                x.ScaledValue = key.moveKey.x;
                ShawInt z = ShawInt.zero;
                z.ScaledValue = key.moveKey.z;
                InputMoveKey(new ShawVector3(x, 0, z));
                break;
            case EKeyType.Skill:
                break;
        }
    }

    #region ����ģ��
    // ����ֵ
    private ShawInt hp;
    public ShawInt Hp
    {
        get { return hp; }
        private set { hp = value; }
    }

    // ������
    private ShawInt defense;
    public ShawInt Defense
    {
        get { return defense; }
        private set { defense = value; }
    }

    // ������
    private ShawInt attack;
    public ShawInt Attack
    {
        get { return attack; }
        private set { attack = value; }
    }

    void InitProperties()
    {
        Hp = unitData.unitCfg.hp;
        Defense = unitData.unitCfg.defense;
        Attack = unitData.unitCfg.attack;
    }

    // �����ٶ�
    public ShawInt AttackSpeedRateBase; // ��ʼ����
    private ShawInt attackSpeedRate; // ��ǰ����
    public ShawInt AttackSpeedRate
    {
        private set
        {
            attackSpeedRate = value;

            Skill skill = GetNormalSkill();
            if (skill != null)
            {
                skill.skillTime = skill.config.skillTime * AttackSpeedRateBase / attackSpeedRate;
                skill.skillTime = skill.config.spellTime * AttackSpeedRateBase / attackSpeedRate;
            }
        }
        get => attackSpeedRate;
    }

    /// <summary>
    /// ��ʼ������
    /// </summary>
    /// <param name="rate"></param>
    public void InitAttackSpeedRate(ShawInt rate)
    {
        AttackSpeedRateBase = rate;
        attackSpeedRate = rate; // ÿ���ӽ��ж��ٴι���
    }
    #endregion

    #region ����ģ��
    protected Skill[] skillArr;

    void InitSkill()
    {
        int len = unitData.unitCfg.skillArr.Length;
        skillArr = new Skill[len];
        for (int i = 0; i < len; i++)
        {
            skillArr[i] = new Skill(unitData.unitCfg.skillArr[i], this);
        }
    }

    void TickSkill()
    {

    }

    void UnInitSkill()
    {

    }

    /// <summary>
    /// �õ���ǰ�߼���Ԫ����ͨ��������
    /// </summary>
    /// <returns></returns>
    public Skill GetNormalSkill()
    {
        if (skillArr != null && skillArr[0] != null)
        {
            return skillArr[0];
        }
        return null;
    }
    #endregion

    #region �ƶ�ģ��
    public ShawCylinderCollider selfCollider; // ������ײ��
    List<ShawColliderBase> envColliLst;       // ������ײ��

    private ShawVector3 inputDir; // ���뷽��
    public ShawVector3 InputDir
    {
        get => inputDir;
        set => inputDir = value;
    }

    void InitMove()
    {
        LogicPos = unitData.bornPos;
        baseMoveSpeed = unitData.unitCfg.moveSpeed;
        LogicMoveSpeed = unitData.unitCfg.moveSpeed;

        envColliLst = FightManager.Instance.GetAllEnvColliders();

        selfCollider = new ShawCylinderCollider(unitData.unitCfg.colliCfg)
        {
            mPos = LogicPos,
        };
    }

    void TickMove()
    {
        ShawVector3 moveDir = InputDir;
        selfCollider.mPos += moveDir * LogicMoveSpeed * (ShawInt)ClientConfig.ClientLogicFrameDeltaTimeSecond;

        ShawVector3 adj = ShawVector3.zero;
        selfCollider.CalcColliderInteraction(envColliLst, ref moveDir, ref adj);
        if (LogicDir != moveDir)
        {
            LogicDir = moveDir;
        }
        if (LogicDir != ShawVector3.zero)
        {
            LogicPos = selfCollider.mPos + adj;
        }
        selfCollider.mPos = LogicPos;
        LogCore.ColorLog(selfCollider.mPos.x + " " + selfCollider.mPos.z, ELogColor.Green);
    }

    void UnInitMove()
    {

    }

    public void InputMoveKey(ShawVector3 dir)
    {
        InputDir = dir;
        // LogCore.ColorLog(InputDir.ConvertViewVector3().ToString(), ELogColor.Green) ;
    }
    #endregion
}