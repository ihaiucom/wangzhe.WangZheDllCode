using AGE;
using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public class TriggerActionSkillHud : TriggerActionBase
	{
		private int timer = -1;

		private int startTimer = -1;

		public TriggerActionSkillHud(TriggerActionWrapper inWrapper, int inTriggerId) : base(inWrapper, inTriggerId)
		{
		}

		public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
		{
			if (this.timer != -1 || this.startTimer != -1)
			{
				return null;
			}
			if (this.ActiveTime > 0)
			{
				this.startTimer = Singleton<CTimerManager>.GetInstance().AddTimer(this.ActiveTime, 1, new CTimer.OnTimeUpHandler(this.OnActivating), true);
			}
			else
			{
				this.DoHighlight(this.bEnable);
				if (this.TotalTime > 0)
				{
					this.timer = Singleton<CTimerManager>.GetInstance().AddTimer(this.TotalTime, 1, new CTimer.OnTimeUpHandler(this.OnTimeUp), true);
				}
			}
			return null;
		}

		private void OnActivating(int timersequence)
		{
			this.DoHighlight(this.bEnable);
			if (this.startTimer != -1)
			{
				Singleton<CTimerManager>.GetInstance().RemoveTimer(this.startTimer);
				this.startTimer = -1;
			}
			if (this.TotalTime > 0)
			{
				this.timer = Singleton<CTimerManager>.GetInstance().AddTimer(this.TotalTime, 1, new CTimer.OnTimeUpHandler(this.OnTimeUp), true);
			}
		}

		private void OnTimeUp(int timersequence)
		{
			this.DoHighlight(!this.bEnable);
			if (this.timer != -1)
			{
				Singleton<CTimerManager>.GetInstance().RemoveTimer(this.timer);
				this.timer = -1;
			}
		}

		private void DoHighlight(bool bYes)
		{
			bool flag = this.EnterUniqueId > 0;
			SkillSlotType enterUniqueId = (SkillSlotType)this.EnterUniqueId;
			bool bAll = enterUniqueId == SkillSlotType.SLOT_SKILL_COUNT;
			if (flag)
			{
				Singleton<BattleSkillHudControl>.GetInstance().Highlight(enterUniqueId, bYes, bAll, false, false, false, 0);
			}
		}

		public override void TriggerUpdate(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
		{
		}

		public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
		{
			if (this.bStopWhenLeaving)
			{
				this.DoHighlight(!this.bEnable);
			}
		}
	}
}
