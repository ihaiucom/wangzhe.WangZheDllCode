using AGE;
using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public class TriggerActionPauseGame : TriggerActionBase
	{
		private int timer = -1;

		public TriggerActionPauseGame(TriggerActionWrapper inWrapper, int inTriggerId) : base(inWrapper, inTriggerId)
		{
		}

		public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
		{
			if (this.timer != -1)
			{
				return null;
			}
			this.DoPauseGame(true);
			if (this.TotalTime > 0)
			{
				this.timer = Singleton<CTimerManager>.GetInstance().AddTimer(this.TotalTime, 1, new CTimer.OnTimeUpHandler(this.OnTimeUp), true);
			}
			return null;
		}

		private void OnTimeUp(int timersequence)
		{
			this.DoPauseGame(false);
			if (this.timer != -1)
			{
				Singleton<CTimerManager>.GetInstance().RemoveTimer(this.timer);
				this.timer = -1;
			}
		}

		private void DoPauseGame(bool bPause)
		{
			if (bPause)
			{
				Singleton<CBattleGuideManager>.instance.PauseGame(this, true);
			}
			else
			{
				Singleton<CBattleGuideManager>.instance.ResumeGame(this);
			}
		}

		public override void TriggerUpdate(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
		{
		}

		public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
		{
			if (this.bStopWhenLeaving)
			{
				this.DoPauseGame(false);
			}
		}
	}
}
