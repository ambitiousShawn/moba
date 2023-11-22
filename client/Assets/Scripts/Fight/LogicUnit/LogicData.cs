using ShawnFramework.CommonModule;
using ShawnFramework.ShawLog;
using ShawnFramework.ShawMath;
using ShawnFramework.ShawnPhysics;
using System.Collections.Generic;
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
        set { logicMoveSpeed = value; }
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

        // 表现层初始化
        GameObject go = AssetsSvc.Instance.LoadPrefab(pathPrefix + "/" + unitData.unitCfg.resName);
        // mainViewUnit = go.AddComponent<HeroView>();
        // if (mainViewUnit == null )
        // {
        //     LogCore.Error("Get MainViewUnit Error:" + unitName);
        // }
        // mainViewUnit.Init(this);
        // stateType = EUnitStateType.Alive;
    }

    public override void LogicTick()
    {
        TickMove();
    }

    #region 属性模块
    // 生命值
    private ShawInt hp;
    public ShawInt Hp
    {
        get { return hp; }
        private set { hp = value; }
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

    void InitSkill()
    {
        int len = unitData.unitCfg.skillArr.Length;
        skillArr = new Skill[len];
        for (int i = 0; i < len; i++)
        {
            skillArr[i] = new Skill(unitData.unitCfg.skillArr[i], this);
        }
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
    #endregion

    #region 移动模块
    public ShawCylinderCollider selfCollider; // 自身碰撞体
    List<ShawColliderBase> envColliLst;       // 环境碰撞体

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

    public ShawVector3 InputDir {  get; set; }
    void TickMove()
    {
        ShawVector3 moveDir;
    }
    #endregion
}