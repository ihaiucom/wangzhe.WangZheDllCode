using Assets.Scripts.Common;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[StarConditionAttrContext(12)]
	internal class StarConditionTeamExtinct : StarCondition
	{
		private bool bCheckResult;

		private int EncounterCount;

		private bool bCanStat = true;

		protected PoolObjHandle<ActorRoot> CachedSource;

		protected PoolObjHandle<ActorRoot> CachedAttacker;

		private int targetCamp
		{
			get
			{
				return base.ConditionInfo.KeyDetail[3];
			}
		}

		private int targetCount
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
				return (!this.bCheckResult) ? StarEvaluationStatus.Failure : StarEvaluationStatus.Success;
			}
		}

		public override int[] values
		{
			get
			{
				return new int[]
				{
					this.EncounterCount
				};
			}
		}

		public override void Initialize(ResDT_ConditionInfo InConditionInfo)
		{
			base.Initialize(InConditionInfo);
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.OnActorRevive));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.OnActorRevive));
		}

		public override void Dispose()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.OnActorRevive));
			base.Dispose();
		}

		private bool ShouldCare(ActorRoot src)
		{
			if (src.ActorControl == null || !(src.ActorControl is HeroWrapper))
			{
				return false;
			}
			if (this.targetCamp == 0)
			{
				return src.TheActorMeta.ActorCamp == Singleton<GamePlayerCenter>.instance.hostPlayerCamp;
			}
			return src.TheActorMeta.ActorCamp != Singleton<GamePlayerCenter>.instance.hostPlayerCamp;
		}

		private void OnActorRevive(ref DefaultGameEventParam prm)
		{
			if (prm.src && this.ShouldCare(prm.src.handle))
			{
				this.bCanStat = true;
			}
		}

		public override void OnActorDeath(ref GameDeadEventParam prm)
		{
			this.CachedSource = prm.src;
			this.CachedAttacker = prm.orignalAtker;
			if (prm.src && this.bCanStat && this.ShouldCare(prm.src.handle))
			{
				bool flag = true;
				Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref prm.src);
				DebugHelper.Assert(ownerPlayer != null, "咦，怎么会取不到ActorRoot对应的Player呢？");
				ReadonlyContext<PoolObjHandle<ActorRoot>>.Enumerator enumerator = ownerPlayer.GetAllHeroes().GetEnumerator();
				while (enumerator.MoveNext())
				{
					PoolObjHandle<ActorRoot> current = enumerator.Current;
					if (!current.handle.ActorControl.IsDeadState)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					this.EncounterCount++;
					this.bCanStat = false;
					bool flag2 = this.CheckResult();
					if (flag2 != this.bCheckResult)
					{
						this.bCheckResult = flag2;
						this.TriggerAllDeathEvent();
					}
				}
			}
		}

		private void TriggerAllDeathEvent()
		{
			if (!Singleton<PVEReviveHeros>.instance.CheckAndPopReviveForm(new PVEReviveHeros.SetTriggerAllDeathResult(this.GetTriggerAllDeathResult), true))
			{
				this.TriggerChangedEvent();
			}
		}

		private void GetTriggerAllDeathResult(bool bReviveOk)
		{
			if (bReviveOk)
			{
				this.bCheckResult = false;
				this.EncounterCount = 0;
				this.bCanStat = true;
			}
			else
			{
				this.TriggerChangedEvent();
			}
		}

		protected bool CheckResult()
		{
			return SmartCompare.Compare<int>(this.EncounterCount, this.targetCount, this.operation);
		}

		public override bool GetActorRef(out PoolObjHandle<ActorRoot> OutSource, out PoolObjHandle<ActorRoot> OutAttacker)
		{
			OutSource = this.CachedSource;
			OutAttacker = this.CachedAttacker;
			return true;
		}
	}
}
