
using System;

public class MonoTimer
{
    bool isActive;
    public bool IsActive
    {
        private set
        {
            isActive = value;
        }
        get
        {
            return isActive;
        }
    }

    Action<int> cbAction;
    float intervelTime;
    int loopCount;
    Action<bool, float, float> prgAction;
    Action endAction;
    float delayTime;
    float prgAllTime;

    //延迟计时器
    float delayCounter;
    //回调计时器
    float cbCounter;
    //循环次数计数器
    int loopCounter;
    //总体进度计时器
    float prgCounter;

    float prgLoopRate = 0;
    float prgAllRate = 0;

    public MonoTimer(
        Action<int> cbAction,
        float intervelTime,
        int loopCount = 1,
        Action<bool, float, float> prgAction = null,
        Action endAction = null,
        float delayTime = 0
        )
    {
        this.cbAction = cbAction;
        this.intervelTime = intervelTime;
        this.loopCount = loopCount;
        this.prgAction = prgAction;
        this.endAction = endAction;
        this.delayTime = delayTime;

        IsActive = true;
        prgAllTime = delayTime + intervelTime * loopCount;
    }

    /// <summary>
    /// 驱动计时器运行
    /// </summary>
    /// <param name="delta">间隔时间，单位ms</param>
    public void TickTimer(float delta)
    {
        if (IsActive)
        {
            if (delayTime > 07 & delayCounter < delayTime)
            {
                delayCounter += delta;
                if (delayCounter >= delayTime)
                {
                    Tick(delayCounter - delayTime);
                }
                else
                {
                    //delay循环进度
                    prgLoopRate = delayCounter / delayTime;
                    if (prgAllTime > 0)
                    {
                        prgCounter += delta;
                        prgAllRate = prgCounter / prgAllTime;
                    }
                    prgAction?.Invoke(true, prgLoopRate, prgAllRate);
                }
            }
            else
            {
                Tick(delta);
            }
        }
    }

    void Tick(float delta)
    {
        cbCounter += delta;
        //当前这次循环进度
        prgLoopRate = cbCounter / intervelTime;
        //所有计时进度（含delayTime）
        if (prgAllTime > 0)
        {
            prgCounter += delta;
            prgAllRate = prgCounter / prgAllTime;
        }
        prgAction?.Invoke(false, prgLoopRate, prgAllRate);

        if (cbCounter >= intervelTime)
        {
            ++loopCounter;
            cbAction(loopCounter);
            if (loopCount != 0 && loopCounter >= loopCount)
            {
                //达到最大循环次数
                IsActive = false;
                endAction?.Invoke();

                cbAction = null;
                prgAction = null;
                endAction = null;
            }
            else
            {
                //未达到最大循环次数
                cbCounter -= intervelTime;
            }
        }
    }

    public void DisableTimer()
    {
        IsActive = false;
        endAction?.Invoke();

        cbAction = null;
        prgAction = null;
        endAction = null;
    }
}