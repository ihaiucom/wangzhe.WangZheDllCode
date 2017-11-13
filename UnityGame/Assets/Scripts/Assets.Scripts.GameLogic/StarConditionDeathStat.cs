using Assets.Scripts.Common;
using Assets.Scripts.GameLogic.GameKernal;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[StarConditionAttrContext(3)]
	internal class StarConditionDeathStat : StarCondition
	{
		private int DealthCount;

		private bool bCheckResults = true;

		protected PoolObjHandle<ActorRoot> CachedSource;

		protected PoolObjHandle<ActorRoot> CachedAttacker;

		private int targetCamp
		{
			get
			{
				return base.ConditionInfo.KeyDetail[3];
			}
		}

		public ActorTypeDef targetType
		{
			get
			{
				return (ActorTypeDef)base.ConditionInfo.KeyDetail[1];
			}
		}

		public int targetID
		{
			get
			{
				return base.ConditionInfo.KeyDetail[2];
			}
		}

		public int targetCount
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
				return this.bCheckResults ? StarEvaluationStatus.Success : StarEvaluationStatus.Failure;
			}
		}

		public override int[] values
		{
			get
			{
				return new int[]
				{
					this.DealthCount
				};
			}
		}

		public override void Initialize(ResDT_ConditionInfo InConditionInfo)
		{
			base.Initialize(InConditionInfo);
			this.bCheckResults = SmartCompare.Compare<int>(this.DealthCount, this.targetCount, this.operation);
		}

		public override void Dispose()
		{
			base.Dispose();
		}

		private bool ShouldCare(ActorRoot src)
		{
			if (this.targetCamp == 0)
			{
				return src.TheActorMeta.ActorCamp == Singleton<GamePlayerCenter>.instance.hostPlayerCamp;
			}
			return src.TheActorMeta.ActorCamp != Singleton<GamePlayerCenter>.instance.hostPlayerCamp;
		}

		public override void OnActorDeath(ref GameDeadEventParam prm)
		{
			if (prm.src && prm.src.handle.TheActorMeta.ActorType == this.targetType && (this.targetID == 0 || this.targetID == prm.src.handle.TheActorMeta.ConfigId) && this.ShouldCare(prm.src.handle))
			{
				this.DealthCount++;
				bool flag = SmartCompare.Compare<int>(this.DealthCount, this.targetCount, this.operation);
				if (this.bCheckResults != flag)
				{
					this.bCheckResults = flag;
					this.CachedSource = prm.src;
					this.CachedAttacker = prm.orignalAtker;
					this.TriggerChangedEvent();
				}
			}
		}

		public override bool GetActorRef(out PoolObjHandle<ActorRoot> OutSource, out PoolObjHandle<ActorRoot> OutAttacker)
		{
			OutSource = this.CachedSource;
			OutAttacker = this.CachedAttacker;
			return true;
		}
	}
}
