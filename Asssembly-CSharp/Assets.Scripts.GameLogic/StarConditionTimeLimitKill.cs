using Assets.Scripts.Framework;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[StarConditionAttrContext(8)]
	internal class StarConditionTimeLimitKill : StarConditionKillBase
	{
		protected int KillCntWhenTimeout;

		protected ulong TimeoutFlag;

		protected bool bIsTimeout;

		protected ulong StartTime;

		public override StarEvaluationStatus status
		{
			get
			{
				if (this.bIsTimeout)
				{
					return StarEvaluationStatus.Failure;
				}
				return base.status;
			}
		}

		private int timeOperation
		{
			get
			{
				return base.ConditionInfo.ComparetorDetail[1];
			}
		}

		private int limitMSeconds
		{
			get
			{
				return base.ConditionInfo.ValueDetail[1];
			}
		}

		public override string description
		{
			get
			{
				if (this.bIsTimeout)
				{
					return string.Format("[{0}/{1}]", this.KillCntWhenTimeout, this.targetCount);
				}
				return base.description;
			}
		}

		public override int[] values
		{
			get
			{
				DebugHelper.Assert(this.TimeoutFlag >= this.StartTime);
				ulong num = this.TimeoutFlag - this.StartTime;
				return new int[]
				{
					base.killCnt,
					(int)num
				};
			}
		}

		protected override ActorTypeDef targetType
		{
			get
			{
				return (ActorTypeDef)base.ConditionInfo.KeyDetail[1];
			}
		}

		protected override int targetID
		{
			get
			{
				return base.ConditionInfo.KeyDetail[2];
			}
		}

		protected override bool isSelfCamp
		{
			get
			{
				return base.ConditionInfo.KeyDetail[3] == 0;
			}
		}

		protected override int targetCount
		{
			get
			{
				return base.ConditionInfo.ValueDetail[0];
			}
		}

		public override void Initialize(ResDT_ConditionInfo InConditionInfo)
		{
			base.Initialize(InConditionInfo);
		}

		public override void Start()
		{
			base.Start();
			this.KillCntWhenTimeout = 0;
			this.TimeoutFlag = (this.StartTime = 0uL);
		}

		private bool CheckTimeout()
		{
			ulong inFirst = Singleton<FrameSynchr>.instance.LogicFrameTick - this.StartTime;
			return !SmartCompare.Compare<ulong>(inFirst, (ulong)((long)this.limitMSeconds), this.timeOperation);
		}

		public override void OnActorDeath(ref GameDeadEventParam prm)
		{
			this.CachedSource = prm.src;
			this.CachedAttacker = prm.orignalAtker;
			if (!this.bIsTimeout && !this.bCachedResult && this.CheckTimeout())
			{
				this.bIsTimeout = true;
				this.KillCntWhenTimeout = base.killCnt;
				this.TriggerChangedEvent();
			}
			base.OnActorDeath(ref prm);
		}

		protected override void OnResultStateChanged()
		{
			this.TimeoutFlag = Singleton<FrameSynchr>.instance.LogicFrameTick;
			if (!this.bIsTimeout)
			{
				base.OnResultStateChanged();
			}
		}

		protected override void OnStatChanged()
		{
			if (!this.bIsTimeout)
			{
				this.TriggerChangedEvent();
			}
		}
	}
}
