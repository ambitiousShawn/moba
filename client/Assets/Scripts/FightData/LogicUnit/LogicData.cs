using GameProtocol;
using ShawnFramework.CommonModule;
using ShawnFramework.ShawLog;
using ShawnFramework.ShawMath;
using ShawnFramework.ShawnPhysics;
using System;
using System.Collections.Generic;
using System.Numerics;
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
        set
        {
            logicMoveSpeed = value;
            LogCore.Log($"Speed has Changed:{logicMoveSpeed}");
        }
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

        // �ر�С������
        if (!Launcher.Instance.EnableSoldier && unitType == EUnitType.Soldier)
        {
            return;
        }

        // ���ֲ��ʼ��
        GameObject go = AssetsSvc.Instance.LoadPrefab(pathPrefix, unitData.unitCfg.resName, 1);
        if (unitType == EUnitType.Hero)
        {
            mainViewUnit = go.AddComponent<HeroView>();
            mainViewUnit.skillRange = go.transform.Find("skillRange");
        }
        else if (unitType == EUnitType.Soldier)
        {
            mainViewUnit = go.AddComponent<SoldierView>();
        }
        else if (unitType == EUnitType.Tower)
        {
            mainViewUnit = go.AddComponent<TowerView>();
            mainViewUnit.skillRange = go.transform.Find("skillRange");
            go.transform.SetParent(FightManager.Instance.transEnvRoot);
        }
        
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
                InputSkillKey(key.skillKey);
                break;
        }
    }

    #region ����ģ��
    // һЩ�¼�����
    public Action OnHurt;                 // �ܵ��˺�ʱ�Ļص�
    public Action<MainLogicUnit> OnDeath; // ����ʱ�Ļص�
    public Action<EAbnormalState, bool> OnStateChanged; // �쳣״̬�޸ĵĻص�

    // ����ֵ
    private ShawInt hp;
    public ShawInt Hp
    {
        get { return hp; }
        private set
        {
            hp = value;
            // LogCore.ColorLog($"{unitData.unitCfg.unitName}Ѫ���䶯����ǰѪ��:{hp}", ELogColor.Yellow);
        }
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

    // һЩ�쳣״̬
    int silenceCount;   // ��Ĭ����
    public int SilenceCount
    {
        get
        {
            return silenceCount;
        }
        set
        {
            silenceCount = value;
            OnStateChanged?.Invoke(EAbnormalState.Silenced, IsSilenced());
        }
    }
    // �Ƿ񱻳�Ĭ
    bool IsSilenced()
    {
        return silenceCount != 0;
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
    List<LogicTimer> timerLst;
    List<BuffLogic> buffLst;

    void InitSkill()
    {
        int len = unitData.unitCfg.skillArr.Length;
        skillArr = new Skill[len];
        for (int i = 0; i < len; i++)
        {
            skillArr[i] = new Skill(unitData.unitCfg.skillArr[i], this);
        }
        timerLst = new List<LogicTimer>();
        buffLst = new List<BuffLogic>();

        // ��ӱ���Buff
        int[] pasvBuffArr = unitData.unitCfg.pasvBuff;
        if (pasvBuffArr != null)
        {
            for (int i = 0;i < pasvBuffArr.Length;i++)
            {
                CreateSkillBuff(this, null, pasvBuffArr[i], null);
            }
        }
    }

    void TickSkill()
    {

        //Buff tick
        for (int i = buffLst.Count - 1; i >= 0; --i)
        {
            if (buffLst[i].state == SubUnitState.None)
            {
                buffLst[i].LogicUninit();
                buffLst.RemoveAt(i);
            }
            else
            {
                buffLst[i].LogicTick();
            }
        }

        //timer tick
        for (int i = timerLst.Count - 1; i >= 0; --i)
        {
            LogicTimer timer = timerLst[i];
            if (timer.Enable)
            {
                timer.TickTimer();
            }
            else
            {
                timerLst.RemoveAt(i);
            }
        }
    }

    void UnInitSkill()
    {

    }

    void InputSkillKey(SkillKey key)
    {
        for (int i = 0; i < skillArr.Length; i++)
        {
            if (skillArr[i].skillID == key.skillID)
            {
                ShawInt x = ShawInt.zero;
                ShawInt z = ShawInt.zero;
                x.ScaledValue = key.x;
                z.ScaledValue = key.z;
                ShawVector3 skillArgs = new ShawVector3(x, 0, z);
                skillArr[i].ReleaseSkill(skillArgs);
                return;
            }
        }
        LogCore.Error($"skillID:{key.skillID} is not exist.");
    }
    // ����һ���߼�֡��ʱ��
    public void CreateLogicTimer(Action cb, ShawInt waitTime)
    {
        LogicTimer timer = new LogicTimer(cb, waitTime);
        timerLst.Add(timer);
    }

    // ���������ӵ�
    public BulletLogic CreateSkillBullet(MainLogicUnit source, MainLogicUnit target, Skill skill)
    {
        BulletLogic bullet = AssetsSvc.Instance.CreateBullet(source, target, skill);
        bullet.LogicInit();
        FightManager.Instance.AddBullet(bullet);
        return bullet ;
    }

    // ��������Buff
    public void CreateSkillBuff(MainLogicUnit source, Skill skill, int buffID, object[] args = null)
    {
        BuffLogic buff = AssetsSvc.Instance.CreateBuff(source, this, skill, buffID, args);
        buff.LogicInit();
        buffLst.Add(buff);
        LogCore.ColorLog($"{source.unitData.unitCfg.unitName}��{this.unitData.unitCfg.unitName}ʩ����Buff:{buff.config.buffName}", ELogColor.Yellow);
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

    /// <summary>
    /// �Ƿ����ʩ�ż���
    /// </summary>
    /// <param name="skillID"></param>
    /// <returns></returns>
    public bool CanReleaseSkill(int skillID)
    {
        return 
            // IsSilenced() == false &&
            // IsStunned() == false &&
            // IsKnockup() == false &&
            // IsSkillSpelling() == false &&
            IsSkillReady(skillID);
    }

    bool IsSkillReady(int skillID)
    {
        for (int i = 0; i < skillArr.Length; i++)
        {
            if (skillArr[i].skillID == skillID)
            {
                return skillArr[i].skillState == ESkillState.None;
            }
        }
        LogCore.Warn("skill id config error!");
        return false;
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
        // LogCore.ColorLog($"{unitData.unitCfg.unitName}��ײ���ѳ�ʼ��,λ��:{LogicPos},���:{selfCollider.mRadius}", ELogColor.Cyan);
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
        // LogCore.ColorLog(selfCollider.mPos.x + " " + selfCollider.mPos.z, ELogColor.Green);
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

    #region API Func

    /// <summary>
    /// �ж��Ƿ���ĳ��
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool IsTeam(ETeamType type)
    {
        return unitData.teamType == type;
    }

    // ��ǰposIndex�Ƿ�Ϊ�ÿͻ������
    public virtual bool IsPlayerSelf()
    {
        return false;
    }

    // �ܵ����Լ��ܵ��˺�
    public void GetDamageBySkill(ShawInt damage, Skill skill)
    {
        OnHurt?.Invoke();
        ShawInt hurt = damage - defense;    // ʵ���ܵ����˺�
        if (hurt > 0)
        {
            Hp -= hurt;
            if (Hp <= 0)
            {
                // �߼���Ԫ����
                Hp = 0;
                stateType = EUnitStateType.Dead;
                InputDir = ShawVector3.zero;
                OnDeath?.Invoke(skill.owner);
                LogCore.Log($"{unitName} hp = 0,Died.");
            }
        }
    }

    // �ܵ�����Buff���˺�
    public void GetDamageByBuff(ShawInt damage, BuffLogic buff, bool calcCB = true)
    {
        if (calcCB)
        {
            OnHurt?.Invoke();
        }
        ShawInt hurt = damage - defense;
        if (hurt > 0)
        {
            Hp -= hurt;
            if (Hp <= 0)
            {
                Hp = 0;
                stateType = EUnitStateType.Dead;
                InputDir = ShawVector3.zero;
                OnDeath?.Invoke(buff.source);
            }
        }
    }

    // �ܵ�����Buff������Ч��
    public void GetCureByBuff(ShawInt cure, BuffLogic buff)
    {
        if (Hp >= unitData.unitCfg.hp)
        {
            return;
        }

        Hp += cure;
        ShawInt trueCure = cure;
        if (Hp > unitData.unitCfg.hp)
        {
            // �������
            trueCure -= (Hp - unitData.unitCfg.hp);
            Hp = unitData.unitCfg .hp;
        }
    }

    // ͨ��ID��õ�ǰ�߼�ʵ�����ϵ�Buff
    public BuffLogic GetBuffByID(int buffID)
    {
        for (int i = 0; i < buffLst.Count; i++)
        {
            if (buffLst[i].config.buffID == buffID)
            {
                return buffLst[i];
            }
        }
        return null;
    }
    
    // �����߼�ʵ����ƶ��ٶ�
    public void ModifyMoveSpeed(ShawInt value, BuffLogic buff, bool jumpInfo)
    {
        LogicMoveSpeed += value;
    }

    public Skill GetSkillByID(int skillID)
    {
        for (int i = 0; i < skillArr.Length; i++)
        {
            if (skillArr[i].skillID == skillID)
            {
                return skillArr[i];
            }
        }
        LogCore.Error($"SkillID:{skillID} is not exist");
        return null;
    }
    #endregion
}