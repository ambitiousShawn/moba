using ShawnFramework.CommonModule;
using ShawnFramework.ShawLog;
using ShawnFramework.ShawMath;
using System;
using System.Diagnostics;
/// <summary>
/// 技能单位
/// </summary>
public class Skill
{
    public int skillID;
    public SkillConfig config;
    public ShawVector3 skillArgs;
    public MainLogicUnit lockTarget;
    public ESkillState skillState = ESkillState.Before;

    public ShawInt spellTime; // 前摇时间
    public ShawInt skillTime; // 技能总时间

    public MainLogicUnit owner;

    // 技能释放成功后的回调
    public Action<Skill> SpellSuccCallback;

    public Skill(int skillID, MainLogicUnit owner)
    {
        this.skillID = skillID;

        config = AssetsSvc.Instance.GetSkillConfigByID(skillID);
        spellTime = config.spellTime;
        skillTime = config.skillTime;

        if (config.isNormalAttack)
        {
            owner.InitAttackSpeedRate(1000 / skillTime);
        }
        this.owner = owner;
    }

    /// <summary>
    /// 释放技能
    /// </summary>
    /// <param name="skillArgs"></param>
    public void ReleaseSkill(ShawVector3 skillArgs)
    {
        this.skillArgs = skillArgs;
        // 指向性技能
        if (config.targetConf != null && config.targetConf.skillTargetType != ESkillTargetType.Dynamic)
        {
            lockTarget = CalcSkillSelectTarget.FindSingleTargetByConfig(owner, config.targetConf, skillArgs);
            if (lockTarget != null)
            {
                ShawVector3 dir = lockTarget.LogicPos - owner.LogicPos;
                SkillSpellBefore(dir);
            
                void SkillWork()
                {
                    // 技能释放过程
                    SkillSpellAfter();
                }

                if (spellTime == 0)
                {
                    LogCore.Log("瞬发技能，立即生效");
                    SkillWork();
                }
                else
                {
                    void DelaySkillWork()
                    {
                        lockTarget = CalcSkillSelectTarget.FindSingleTargetByConfig(owner, config.targetConf, skillArgs);
                        if (lockTarget != null)
                        {
                            SkillWork();
                        }
                        else
                        {
                            SkillEnd();
                        }
                    }

                    //定时处理
                    owner.CreateLogicTimer(DelaySkillWork, spellTime);
                }
            }
            else
            {
                LogCore.Warn("没有符合条件的技能目标");
                SkillEnd();
            }
        }
        else
        {
            SkillSpellBefore(skillArgs);
            // void DirectionBullet()
            // {
            //     //非目标弹道技能
            //     DirectionBullet bullet = owner.CreateSkillBullet(owner, null, this) as DirectionBullet;
            //     bullet.hitTargetCB = (MainLogicUnit target, object[] args) => {
            //         this.Log("路径上击中目标：" + target.unitName);
            //         HitTarget(target, args);
            //     };
            //     bullet.ReachPosCB = () => {
            //         this.Log("子弹达到最终位置");
            //     };
            // }
            // if (spellTime == 0)
            // {
            //     if (cfg.bulletCfg != null)
            //     {
            //         DirectionBullet();
            //     }
            //     AttachSkillBuffToCaster();
            //     SkillSpellAfter();
            // }
            // else
            // {
            //     owner.CreateLogicTimer(() => {
            //         if (cfg.bulletCfg != null)
            //         {
            //             DirectionBullet();
            //         }
            //         AttachSkillBuffToCaster();
            //         SkillSpellAfter();
            //     }, spellTime);
            // }
        }
    }

    // 进入施法前摇
    void SkillSpellBefore(ShawVector3 dir)
    {
        skillState = ESkillState.SpellBefore;
        if (dir != ShawVector3.zero)
        {
            owner.mainViewUnit.UpdateSkillRotation(dir);
        }

    }

    // 进入施法后摇
    void SkillSpellAfter()
    {
        skillState = ESkillState.SpellAfter;
        if (owner.IsPlayerSelf() && !config.isNormalAttack)
        {
            // 进入技能CD
            FightManager.Instance.EnterCDState(skillID, config.cdTime);
        }
        SpellSuccCallback?.Invoke(this);
    
        // 后摇完成时技能状态重置为None
        // 配置的技能时间必须大于施法时间
        if (skillTime > spellTime)
        {
            owner.CreateLogicTimer(SkillEnd, skillTime - spellTime);
        }
        else
        {
            SkillEnd();
        }
    }

    /// <summary>
    /// 施法后摇动作完成,角色切换到idle状态
    /// </summary>
    void SkillEnd()
    {
        if (skillState == ESkillState.None || skillState == ESkillState.SpellBefore)
        {
            if (owner.IsPlayerSelf())
            {
                if (config.targetConf != null
                    && config.targetConf.skillTargetType == ESkillTargetType.Enemy
                    && config.targetConf.searchDis > 0)
                {
                    // Buff mf = owner.GetBuffByID(ClientConfig.CommonMoveAttackBuffID);
                    // if (mf != null)
                    // {
                    //     mf.unitState = SubUnitState.End;
                    // }

                    LogCore.Log("技能未施放成功，添加通用移动攻击buff.");
                    // owner.CreateSkillBuff(owner, this, ClientConfig.CommonMoveAttackBuffID);
                }
            }
        }

        // if (FreeAniCallback != null)
        // {
        //     FreeAniCallback();
        //     FreeAniCallback = null;
        // }
        skillState = ESkillState.None;
        lockTarget = null;
    }
}

public enum ESkillState
{
    None,
    Before, // Before
    SpellBefore, // 施法前摇
    SpellAfter, // 施法后摇
}