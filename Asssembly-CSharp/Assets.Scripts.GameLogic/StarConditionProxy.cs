using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public abstract class StarConditionProxy : StarCondition
	{
		protected IStarCondition ContextProxy;

		public override StarEvaluationStatus status
		{
			get
			{
				return (this.ContextProxy != null) ? this.ContextProxy.status : StarEvaluationStatus.InProgressing;
			}
		}

		public override int[] values
		{
			get
			{
				return (this.ContextProxy != null) ? this.ContextProxy.values : null;
			}
		}

		public override void Initialize(ResDT_ConditionInfo InConditionInfo)
		{
			base.Initialize(InConditionInfo);
			this.ContextProxy = this.CreateStarCondition();
			StarCondition starCondition = this.ContextProxy as StarCondition;
			if (starCondition != null)
			{
				starCondition.OnStarConditionChanged += new OnStarConditionChangedDelegate(this.OnProxyChanged);
			}
		}

		public override void Start()
		{
			base.Start();
			if (this.ContextProxy != null)
			{
				this.ContextProxy.Start();
			}
		}

		private void OnProxyChanged(IStarCondition InStarCondition)
		{
			DebugHelper.Assert(InStarCondition == this.ContextProxy);
			this.TriggerChangedEvent();
		}

		public virtual IStarCondition CreateStarCondition()
		{
			return null;
		}

		public override void Dispose()
		{
			if (this.ContextProxy != null)
			{
				StarCondition starCondition = this.ContextProxy as StarCondition;
				if (starCondition != null)
				{
					starCondition.OnStarConditionChanged -= new OnStarConditionChangedDelegate(this.OnProxyChanged);
				}
				this.ContextProxy.Dispose();
			}
		}

		public override bool GetActorRef(out PoolObjHandle<ActorRoot> OutSource, out PoolObjHandle<ActorRoot> OutAttacker)
		{
			if (this.ContextProxy != null)
			{
				return this.ContextProxy.GetActorRef(out OutSource, out OutAttacker);
			}
			OutSource = new PoolObjHandle<ActorRoot>(null);
			OutAttacker = new PoolObjHandle<ActorRoot>(null);
			return false;
		}

		public override void OnActorDeath(ref GameDeadEventParam prm)
		{
			if (this.ContextProxy != null)
			{
				this.ContextProxy.OnActorDeath(ref prm);
			}
		}

		public override void OnCampScoreUpdated(ref SCampScoreUpdateParam prm)
		{
			if (this.ContextProxy != null)
			{
				this.ContextProxy.OnCampScoreUpdated(ref prm);
			}
		}
	}
}
