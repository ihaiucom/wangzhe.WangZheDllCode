using System;

namespace Assets.Scripts.GameSystem
{
	public class CDEntity
	{
		public int cd_time;

		private bool bInCDState;

		private int timer_index = -1;

		public CDEntity(int cd_time, int count = 0)
		{
			this.cd_time = cd_time;
			if (this.cd_time > 0)
			{
				this.timer_index = Singleton<CTimerManager>.instance.AddTimer(cd_time, 0, new CTimer.OnTimeUpHandler(this.On_Timer_End));
				Singleton<CTimerManager>.instance.PauseTimer(this.timer_index);
			}
		}

		public virtual void Clear()
		{
			if (this.timer_index != -1)
			{
				Singleton<CTimerManager>.instance.RemoveTimer(this.timer_index);
			}
			this.timer_index = -1;
		}

		public virtual void On_Timer_End(int timer)
		{
			this.bInCDState = false;
			if (this.timer_index != -1)
			{
				Singleton<CTimerManager>.instance.PauseTimer(this.timer_index);
			}
		}

		public virtual void Start()
		{
			this.bInCDState = true;
			if (this.timer_index != -1)
			{
				Singleton<CTimerManager>.instance.ResetTimer(this.timer_index);
				Singleton<CTimerManager>.instance.ResumeTimer(this.timer_index);
			}
		}
	}
}
