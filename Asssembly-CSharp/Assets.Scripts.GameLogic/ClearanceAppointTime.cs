using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[StarConditionAttrContext(14)]
	internal class ClearanceAppointTime : StarCondition
	{
		private bool bTimeOut;

		public int timeMSeconds
		{
			get
			{
				return base.ConditionInfo.ValueDetail[0];
			}
		}

		public override StarEvaluationStatus status
		{
			get
			{
				return this.bTimeOut ? StarEvaluationStatus.Success : StarEvaluationStatus.Failure;
			}
		}

		public override int[] values
		{
			get
			{
				return new int[]
				{
					this.bTimeOut ? (this.timeMSeconds + 1) : Math.Max(0, this.timeMSeconds - 1000)
				};
			}
		}

		public override void Initialize(ResDT_ConditionInfo InConditionInfo)
		{
			base.Initialize(InConditionInfo);
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.AddTimer));
		}

		public void AddTimer(ref DefaultGameEventParam prm)
		{
			Singleton<CTimerManager>.instance.RemoveTimer(new CTimer.OnTimeUpHandler(this.OnAppointTimeFinished));
			Singleton<CTimerManager>.instance.AddTimer(this.timeMSeconds + 1, -1, new CTimer.OnTimeUpHandler(this.OnAppointTimeFinished));
		}

		public override void Dispose()
		{
			Singleton<CTimerManager>.instance.RemoveTimer(new CTimer.OnTimeUpHandler(this.OnAppointTimeFinished));
			base.Dispose();
		}

		protected void OnAppointTimeFinished(int seq)
		{
			if (!this.bTimeOut)
			{
				this.bTimeOut = true;
				this.TriggerChangedEvent();
			}
		}
	}
}
