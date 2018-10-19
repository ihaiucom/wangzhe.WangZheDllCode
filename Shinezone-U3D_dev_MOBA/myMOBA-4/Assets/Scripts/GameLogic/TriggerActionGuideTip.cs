using AGE;
using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public class TriggerActionGuideTip : TriggerActionBase
	{
		private int timer = -1;

		private int GuideTipId;

		public TriggerActionGuideTip(TriggerActionWrapper inWrapper, int inTriggerId) : base(inWrapper, inTriggerId)
		{
		}

		public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
		{
			if (this.timer != -1)
			{
				return null;
			}
			this.GuideTipId = this.EnterUniqueId;
			ActorRoot inSrc = (!src) ? null : src.handle;
			ActorRoot inAtker = (!atker) ? null : atker.handle;
			if (this.bEnable)
			{
				Singleton<TipProcessor>.GetInstance().PlayDrama(this.GuideTipId, inSrc, inAtker);
			}
			else
			{
				Singleton<TipProcessor>.GetInstance().EndDrama(this.GuideTipId);
			}
			if (this.TotalTime > 0)
			{
				this.timer = Singleton<CTimerManager>.GetInstance().AddTimer(this.TotalTime, 1, new CTimer.OnTimeUpHandler(this.OnTimeUp), true);
			}
			return null;
		}

		private void OnTimeUp(int timersequence)
		{
			Singleton<TipProcessor>.GetInstance().EndDrama(this.GuideTipId);
			if (this.timer != -1)
			{
				Singleton<CTimerManager>.GetInstance().RemoveTimer(this.timer);
				this.timer = -1;
			}
		}

		public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
		{
			int leaveUniqueId = this.LeaveUniqueId;
			ActorRoot inSrc = (!src) ? null : src.handle;
			ActorRoot inAtker = null;
			if (this.bEnable)
			{
				Singleton<TipProcessor>.GetInstance().PlayDrama(leaveUniqueId, inSrc, inAtker);
			}
			else
			{
				Singleton<TipProcessor>.GetInstance().EndDrama(leaveUniqueId);
			}
		}
	}
}
