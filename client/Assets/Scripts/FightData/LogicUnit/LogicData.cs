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
/// 逻辑单元基础接口
/// </summary>
public interface ILogic
{
    void LogicInit();
    void LogicTick();
    void LogicUninit();
}

/// <summary>
/// 基本逻辑单元数据类型基类
/// </summary>
public abstract class BaseLogicUnit : ILogic
{
    public string unitName;

    #region 逻辑属性(位置，速度，方向)
    // 逻辑位置是否改变
    public bool isPosChanged = false;

    // 逻辑位置
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

    // 逻辑方向是否改变
    public bool IsDirChanged = false;

    // 逻辑方向
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

    // 逻辑速度
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
    // 基础速度
    public ShawInt baseMoveSpeed;

    #endregion

    public abstract void LogicInit();

    public abstract void LogicTick();

    public abstract void LogicUninit();
}

/// <summary>
/// 主要逻辑模块(英雄，小兵，防御塔)
/// </summary>
public abstract class MainLogicUnit : BaseLogicUnit
{
    public LogicUnitData unitData;  // 持有一份逻辑模块数据
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
        InitProperties(); // 属性初始化(生命值,攻击力等)
        InitSkill();      // 技能初始化
        InitMove();       // 移动初始化

        // 关闭小兵生成
        if (!Launcher.Instance.EnableSoldier && unitType == EUnitType.Soldier)
        {
            return;
        }

        // 表现层初始化
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

    #region 属性模块
    // 一些事件监听
    public Action OnHurt;                 // 受到伤害时的回调
    public Action<MainLogicUnit> OnDeath; // 死亡时的回调
    public Action<EAbnormalState, bool> OnStateChanged; // 异常状态修改的回调

    // 生命值
    private ShawInt hp;
    public ShawInt Hp
    {
        get { return hp; }
        private set
        {
            hp = value;
            // LogCore.ColorLog($"{unitData.unitCfg.unitName}血量变动，当前血量:{hp}", ELogColor.Yellow);
        }
    }

    // 防御力
    private ShawInt defense;
    public ShawInt Defense
    {
        get { return defense; }
        private set { defense = value; }
    }

    // 攻击力
    private ShawInt attack;
    public ShawInt Attack
    {
        get { return attack; }
        private set { attack = value; }
    }

    // 一些异常状态
    int silenceCount;   // 沉默计数
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
    // 是否被沉默
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

    // 攻击速度
    public ShawInt AttackSpeedRateBase; // 初始攻速
    private ShawInt attackSpeedRate; // 当前攻速
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
    /// 初始化攻速
    /// </summary>
    /// <param name="rate"></param>
    public void InitAttackSpeedRate(ShawInt rate)
    {
        AttackSpeedRateBase = rate;
        attackSpeedRate = rate; // 每秒钟进行多少次攻击
    }
    #endregion

    #region 技能模块
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

        // 添加被动Buff
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
    // 创建一个逻辑帧计时器
    public void CreateLogicTimer(Action cb, ShawInt waitTime)
    {
        LogicTimer timer = new LogicTimer(cb, waitTime);
        timerLst.Add(timer);
    }

    // 创建技能子弹
    public BulletLogic CreateSkillBullet(MainLogicUnit source, MainLogicUnit target, Skill skill)
    {
        BulletLogic bullet = AssetsSvc.Instance.CreateBullet(source, target, skill);
        bullet.LogicInit();
        FightManager.Instance.AddBullet(bullet);
        return bullet ;
    }

    // 创建技能Buff
    public void CreateSkillBuff(MainLogicUnit source, Skill skill, int buffID, object[] args = null)
    {
        BuffLogic buff = AssetsSvc.Instance.CreateBuff(source, this, skill, buffID, args);
        buff.LogicInit();
        buffLst.Add(buff);
        LogCore.ColorLog($"{source.unitData.unitCfg.unitName}对{this.unitData.unitCfg.unitName}施加了Buff:{buff.config.buffName}", ELogColor.Yellow);
    }

    /// <summary>
    /// 拿到当前逻辑单元的普通攻击配置
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
    /// 是否可以施放技能
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

    #region 移动模块
    public ShawCylinderCollider selfCollider; // 自身碰撞体
    List<ShawColliderBase> envColliLst;       // 环境碰撞体

    private ShawVector3 inputDir; // 输入方向
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
        // LogCore.ColorLog($"{unitData.unitCfg.unitName}碰撞体已初始化,位置:{LogicPos},宽高:{selfCollider.mRadius}", ELogColor.Cyan);
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
    /// 判断是否是某队
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool IsTeam(ETeamType type)
    {
        return unitData.teamType == type;
    }

    // 当前posIndex是否为该客户端玩家
    public virtual bool IsPlayerSelf()
    {
        return false;
    }

    // 受到来自技能的伤害
    public void GetDamageBySkill(ShawInt damage, Skill skill)
    {
        OnHurt?.Invoke();
        ShawInt hurt = damage - defense;    // 实际受到的伤害
        if (hurt > 0)
        {
            Hp -= hurt;
            if (Hp <= 0)
            {
                // 逻辑单元死亡
                Hp = 0;
                stateType = EUnitStateType.Dead;
                InputDir = ShawVector3.zero;
                OnDeath?.Invoke(skill.owner);
                LogCore.Log($"{unitName} hp = 0,Died.");
            }
        }
    }

    // 受到来自Buff的伤害
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

    // 受到来自Buff的治疗效果
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
            // 治疗溢出
            trueCure -= (Hp - unitData.unitCfg.hp);
            Hp = unitData.unitCfg .hp;
        }
    }

    // 通过ID获得当前逻辑实体身上的Buff
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
    
    // 更改逻辑实体的移动速度
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