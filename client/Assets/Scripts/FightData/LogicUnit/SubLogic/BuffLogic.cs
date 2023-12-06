using ShawnFramework.CommonModule;
using ShawnFramework.ShawLog;
using System.Collections.Generic;
using UnityEngine;

public class BuffLogic : SubLogicUnit
{
    public MainLogicUnit owner; // Buff附着单位
    protected int buffID;
    protected object[] args;

    protected int buffDuration;
    int tickCount = 0;          // 计数
    int durationCount = 0;      // 计时时长
    public BuffConfig config;

    protected List<MainLogicUnit> targetList; // 群体目标列表
    BuffView buffView;

    public BuffLogic(MainLogicUnit source, MainLogicUnit owner, Skill skill, int buffID, object[] args = null) : base(source, skill)
    {
        this.owner = owner;
        this.buffID = buffID;
        this.args = args;
    }

    public override void LogicInit()
    {
        config = AssetsSvc.Instance.GetBuffConfigByID(buffID);
        buffDuration = config.buffDuration;
        delayTime = config.buffDelay;

        base.LogicInit();
    }

    public override void LogicTick()
    {
        base.LogicTick();
        switch (state)
        {
            case SubUnitState.Start:
                Start();
                if (buffDuration > 0 || buffDuration == -1)
                {
                    // 循环次数 >1或无限循环
                    state = SubUnitState.Tick;
                }
                else
                {
                    state = SubUnitState.End;
                }
                break;
            case SubUnitState.Tick:
                if (config.buffInterval > 0)
                {
                    tickCount += ClientConfig.ClientLogicFrameDeltaTimeMS;
                    if (tickCount >= config.buffInterval)
                    {
                        tickCount -= config.buffInterval;
                        Tick();
                    }
                }
                durationCount += ClientConfig.ClientLogicFrameDeltaTimeMS;
                if (durationCount >= buffDuration &&  buffDuration != -1)
                {
                    state = SubUnitState.End;
                }
                break;
        }
    }

    protected override void Start()
    {
        // Buff初始位置
        if (config.staticPosType == EStaticPosType.None)
        {
            LogicPos = owner.LogicPos;
        }
        if (config.buffEffect != null)
        {
            GameObject go = AssetsSvc.Instance.LoadPrefab("effect", config.buffEffect, 1);
            go.name = $"{source.unitName}_{config.buffName}";
            buffView = go.AddComponent<BuffView>();
            if (buffView == null)
            {
                LogCore.Error($"Get BuffView Error:{unitName}");
            }
            if (config.staticPosType == EStaticPosType.None)
            {
                // buff所属设置与跟随
                owner.mainViewUnit.SetBuffFollower(buffView);
            }
            buffView.ViewInit(this) ;

            // TODO:音效
        }
    }
    protected override void Tick()
    {
        // TODO:音效
    }
    protected override void End()
    {
        if (config.buffEffect != null)
        {
            buffView.DestroyBuff();
        }
    }

    public override void LogicUninit() { }
}