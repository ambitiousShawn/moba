using ShawnFramework.CommonModule;
using ShawnFramework.ShawLog;
using ShawnFramework.ShawMath;
using System;
using System.Diagnostics;
/// <summary>
/// ���ܵ�λ
/// </summary>
public class Skill
{
    public int skillID;
    public SkillConfig config;
    public ShawVector3 skillArgs;
    public MainLogicUnit lockTarget;
    public ESkillState skillState = ESkillState.Before;

    public ShawInt spellTime; // ǰҡʱ��
    public ShawInt skillTime; // ������ʱ��

    public MainLogicUnit owner;

    // �����ͷųɹ���Ļص�
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
    /// �ͷż���
    /// </summary>
    /// <param name="skillArgs"></param>
    public void ReleaseSkill(ShawVector3 skillArgs)
    {
        this.skillArgs = skillArgs;
        // ָ���Լ���
        if (config.targetConf != null && config.targetConf.skillTargetType != ESkillTargetType.Dynamic)
        {
            lockTarget = CalcSkillSelectTarget.FindSingleTargetByConfig(owner, config.targetConf, skillArgs);
            if (lockTarget != null)
            {
                ShawVector3 dir = lockTarget.LogicPos - owner.LogicPos;
                SkillSpellBefore(dir);
            
                void SkillWork()
                {
                    // �����ͷŹ���
                    SkillTakeEffect(lockTarget);  // �����ӵ�Buffʵ��
                    AttachSkillBuffToCaster();
                    SkillSpellAfter();            // ʩ����ҡ 
                }

                if (spellTime == 0)
                {
                    LogCore.Log("˲�����ܣ�������Ч");
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

                    //��ʱ����
                    owner.CreateLogicTimer(DelaySkillWork, spellTime);
                }
            }
            else
            {
                LogCore.Warn("û�з��������ļ���Ŀ��");
                SkillEnd();
            }
        }
        else
        {
            SkillSpellBefore(skillArgs);
            void DirectionBullet()
            {
                //��Ŀ�굯������
               // DirectionBullet bullet = owner.CreateSkillBullet(owner, null, this) as DirectionBullet;
               // bullet.hitTargetCB = (MainLogicUnit target, object[] args) => {
               //     this.Log("·���ϻ���Ŀ�꣺" + target.unitName);
               //     HitTarget(target, args);
               // };
               // bullet.ReachPosCB = () => {
               //     this.Log("�ӵ��ﵽ����λ��");
               // };
            }
            if (spellTime == 0)
            {
                if (config.bulletConf != null)
                {
                    DirectionBullet();
                }
                AttachSkillBuffToCaster();
                SkillSpellAfter();
            }
            else
            {
                owner.CreateLogicTimer(() => {
                    if (config.bulletConf != null)
                    {
                        DirectionBullet();
                    }
                    AttachSkillBuffToCaster();
                    SkillSpellAfter();
                }, spellTime);
            }
        }
    }

    // ����ʩ��ǰҡ
    void SkillSpellBefore(ShawVector3 dir)
    {
        skillState = ESkillState.SpellBefore;
        if (dir != ShawVector3.zero)
        {
            owner.mainViewUnit.UpdateSkillRotation(dir);
        }
        // TODO:���Ŷ���
        if (config.animName != null)
        {
            owner.InputDir = dir;
        }
    }

    // ������Ч
    void SkillTakeEffect(MainLogicUnit lockTarget)
    {
        if (config.bulletConf != null)
        {
            // ���ڵ�����Ϣ
            TargetBullet bullet = owner.CreateSkillBullet(owner, lockTarget, this) as TargetBullet;
            bullet.HitTargetCB = HitTarget;
        }
        else
        {
            // �����ڵ�����Ϣ��ֱ�ӻ���Ŀ��
            HitTarget(lockTarget);
        }
    }

    // ����Ŀ����߼�
    void HitTarget(MainLogicUnit lockTarget, object[] args = null)
    {
        if (config.damage != 0)
        {
            ShawInt damage = config.damage;
            lockTarget.GetDamageBySkill(damage, this);
        }
        // ����Buff����
        if (config.buffIDArr == null)
        {
            return;
        }
        for (int i = 0; i <  config.buffIDArr.Length; i++)
        {
            int buffID = config.buffIDArr[i];
            if (buffID == 0)
            {
                LogCore.Warn($"SkillID:{config.skillID} exist buffID == 0,check your buffID Configs");
                continue;
            }
            BuffConfig buffConfig = AssetsSvc.Instance.GetBuffConfigByID(buffID);
            if (buffConfig.attacher == EAttachType.Target || buffConfig.attacher == EAttachType.Bullet)
            {
                lockTarget.CreateSkillBuff(owner, this, buffID, args);
            }
        }
    }

    void AttachSkillBuffToCaster()
    {
        if (config.buffIDArr == null)
        {
            return;
        }

        for (int i = 0; i < config.buffIDArr.Length; i++)
        {
            int buffID = config.buffIDArr[i];
            if (buffID == 0)
            {
                LogCore.Warn(string.Format("SkillID:{0} exist: buffID ==0,Check your buffID Configs.", config.skillID));
                continue;
            }

            BuffConfig buffCfg = AssetsSvc.Instance.GetBuffConfigByID(buffID);
            if (buffCfg.attacher == EAttachType.Caster || buffCfg.attacher == EAttachType.Indie)
            {
                owner.CreateSkillBuff(owner, this, buffID);
            }
        }
    }

    // ����ʩ����ҡ
    void SkillSpellAfter()
    {
        skillState = ESkillState.SpellAfter;
        if (owner.IsPlayerSelf() && !config.isNormalAttack)
        {
            // ���뼼��CD
            FightManager.Instance.EnterCDState(skillID, config.cdTime);
        }
        SpellSuccCallback?.Invoke(this);
    
        // ��ҡ���ʱ����״̬����ΪNone
        // ���õļ���ʱ��������ʩ��ʱ��
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
    /// ʩ����ҡ�������,��ɫ�л���idle״̬
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
                    BuffLogic mf = owner.GetBuffByID(ClientConfig.CommonMoveAttackBuffID);
                    if (mf != null)
                    {
                        mf.state = SubUnitState.End;
                    }

                    LogCore.Log("����δʩ�ųɹ������ͨ���ƶ�����buff.");
                    owner.CreateSkillBuff(owner, this, ClientConfig.CommonMoveAttackBuffID);
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

    int tempSkillID;
    public int TempSkillID
    {
        set
        {
            tempSkillID = value;
            LogCore.Log("Set TempSkillID:" + value);
        }
        get
        {
            return tempSkillID;
        }
    }

    //�����滻
    public void ReplaceSkillConfig(int replaceID)
    {
        if (skillID == replaceID)
        {
            TempSkillID = 0;
        }
        else
        {
            TempSkillID = replaceID;
        }

        config = AssetsSvc.Instance.GetSkillConfigByID(replaceID);
    }
}

public enum ESkillState
{
    None,
    Before, // Before
    SpellBefore, // ʩ��ǰҡ
    SpellAfter, // ʩ����ҡ
}