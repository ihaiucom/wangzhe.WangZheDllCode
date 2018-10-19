using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public abstract class StarConditionKillBase : StarCondition
	{
		protected bool bCachedResult;

		protected PoolObjHandle<ActorRoot> CachedSource;

		protected PoolObjHandle<ActorRoot> CachedAttacker;

		public int killCnt
		{
			get;
			protected set;
		}

		public override StarEvaluationStatus status
		{
			get
			{
				return (!this.bCachedResult) ? StarEvaluationStatus.Failure : StarEvaluationStatus.Success;
			}
		}

		protected abstract ActorTypeDef targetType
		{
			get;
		}

		protected abstract int targetID
		{
			get;
		}

		protected abstract bool isSelfCamp
		{
			get;
		}

		protected abstract int targetCount
		{
			get;
		}

		public override string description
		{
			get
			{
				return string.Format("[{0}/{1}]", (this.killCnt <= this.targetCount) ? this.killCnt : this.targetCount, this.targetCount);
			}
		}

		public override int[] values
		{
			get
			{
				return new int[]
				{
					this.killCnt
				};
			}
		}

		public override void Initialize(ResDT_ConditionInfo InConditionInfo)
		{
			base.Initialize(InConditionInfo);
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_BeginFightOver, new RefAction<DefaultGameEventParam>(this.onFightOver));
			this.killCnt = 0;
			this.bCachedResult = SmartCompare.Compare<int>(this.killCnt, this.targetCount, this.operation);
		}

		public override void Dispose()
		{
			this.DetachEventListener();
			this.killCnt = 0;
			base.Dispose();
		}

		protected void DetachEventListener()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_BeginFightOver, new RefAction<DefaultGameEventParam>(this.onFightOver));
		}

		protected virtual bool ShouldCare(ActorRoot src)
		{
			if (this.isSelfCamp)
			{
				return src.IsHostCamp();
			}
			return !src.IsHostCamp();
		}

		protected virtual void onFightOver(ref DefaultGameEventParam prm)
		{
			this.CheckResult();
		}

		public override void OnActorDeath(ref GameDeadEventParam prm)
		{
			this.CachedSource = prm.src;
			this.CachedAttacker = prm.orignalAtker;
			if (prm.src && prm.orignalAtker && this.ShouldCare(prm.src.handle) && prm.src.handle.TheActorMeta.ActorType == this.targetType && (this.targetID == 0 || prm.src.handle.TheActorMeta.ConfigId == this.targetID))
			{
				this.killCnt++;
				this.CheckResult();
				this.OnStatChanged();
			}
		}

		protected virtual void OnStatChanged()
		{
		}

		protected virtual void OnResultStateChanged()
		{
		}

		protected virtual bool CheckResult()
		{
			bool flag = SmartCompare.Compare<int>(this.killCnt, this.targetCount, this.operation);
			if (flag != this.bCachedResult)
			{
				this.bCachedResult = flag;
				this.OnResultStateChanged();
				return true;
			}
			return false;
		}

		public override bool GetActorRef(out PoolObjHandle<ActorRoot> OutSource, out PoolObjHandle<ActorRoot> OutAttacker)
		{
			OutSource = this.CachedSource;
			OutAttacker = this.CachedAttacker;
			return true;
		}
	}
}
