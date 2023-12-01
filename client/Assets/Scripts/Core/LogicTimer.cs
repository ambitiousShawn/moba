using ShawnFramework.ShawMath;
using System;

public class LogicTimer
{
	bool enable;
	public bool Enable
	{
		get { return enable; }
		set { enable = value; }
	}

	ShawInt delayTime;
	ShawInt loopTime;
	ShawInt delta;
	ShawInt callbackCount;
	Action callback;

	public LogicTimer(Action callback, ShawInt delayTime, int loopTime = 0)
	{
		this.callback = callback;
		this.delayTime = delayTime;
		this.loopTime = loopTime;
		delta = ClientConfig.ClientLogicFrameDeltaTimeMS; // 15֡
		Enable = true;
	}

	public void TickTimer()
	{
		callbackCount += delta;
		if (callbackCount >= delayTime && callback != null) 
		{
			callback();
			if (loopTime == 0)
			{
				Enable = false;
				callback = null;
			}
			else
			{
				callbackCount -= delayTime;
				delayTime = loopTime;
			}
		}
	}
}