// 辅助逻辑单元
using ShawnFramework.CommonModule;
using ShawnFramework.ShawMath;
using UnityEngine;

public abstract class SubLogicUnit : BaseLogicUnit
{
    public MainLogicUnit source; // 来源角色逻辑单元
    protected Skill skill;       // 所属技能
    protected int delayTime;     // 延迟生效时间
    protected int delayCounter;  // 延迟时间计数
    public SubUnitState state;   // 辅助单元状态

    public SubLogicUnit(MainLogicUnit source, Skill skill)
    {
        this.source = source;
        this.skill = skill;
    }

    public override void LogicInit()
    {
        if (delayTime == 0)
        {
            // 瞬发
            state = SubUnitState.Start;
        }
        else
        {
            // 延时触发
            delayCounter = delayTime;
            state = SubUnitState.Delay;
        }
    }

    public override void LogicTick()
    {
        switch (state)
        {
            case SubUnitState.Delay:
                delayCounter -= ClientConfig.ClientLogicFrameDeltaTimeMS;
                if (delayCounter <= 0)
                {
                    // 倒计时结束
                    state = SubUnitState.Start;
                }
                break;
            case SubUnitState.End:
                End();
                state = SubUnitState.None;
                break;
        }
    }

    public override void LogicUninit()  { }

    protected abstract void Start();
    protected abstract void Tick();
    protected abstract void End();
}

public enum SubUnitState
{
    None,
    Delay,
    Start,
    Tick,
    End,
}