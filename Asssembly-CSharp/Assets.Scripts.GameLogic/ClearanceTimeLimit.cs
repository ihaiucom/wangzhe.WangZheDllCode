using Assets.Scripts.Framework;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[StarConditionAttrContext(11)]
	internal class ClearanceTimeLimit : StarCondition
	{
		private ulong StartTime;

		private ulong EndTime;

		private ulong limitMSeconds
		{
			get
			{
				return (ulong)((long)base.ConditionInfo.ValueDetail[0]);
			}
		}

		public override StarEvaluationStatus status
		{
			get
			{
				ulong num = this.EndTime - this.StartTime;
				if (num == 0uL)
				{
					num = Singleton<FrameSynchr>.instance.LogicFrameTick - this.StartTime;
					return (num > this.limitMSeconds) ? StarEvaluationStatus.Failure : StarEvaluationStatus.InProgressing;
				}
				if (num > this.limitMSeconds)
				{
					return StarEvaluationStatus.Failure;
				}
				return StarEvaluationStatus.Success;
			}
		}

		public override int[] values
		{
			get
			{
				ulong num = this.EndTime - this.StartTime;
				return new int[]
				{
					(int)num
				};
			}
		}

		public override void Initialize(ResDT_ConditionInfo InConditionInfo)
		{
			base.Initialize(InConditionInfo);
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_BeginFightOver, new RefAction<DefaultGameEventParam>(this.onFightOver));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_BeginFightOver, new RefAction<DefaultGameEventParam>(this.onFightOver));
		}

		public override void Start()
		{
			base.Start();
			this.EndTime = (this.StartTime = 0uL);
		}

		private void onFightOver(ref DefaultGameEventParam prm)
		{
			this.EndTime = Singleton<FrameSynchr>.instance.LogicFrameTick;
			if (this.status == StarEvaluationStatus.Success)
			{
				this.TriggerChangedEvent();
			}
		}
	}
}
