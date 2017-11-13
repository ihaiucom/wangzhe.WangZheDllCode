using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[StarConditionAttrContext(15)]
	internal class StarConditionMonsterTrigger : StarCondition
	{
		public bool bCachedResults
		{
			get;
			protected set;
		}

		public int TriggerCount
		{
			get;
			protected set;
		}

		public int targetMonsterID
		{
			get
			{
				return base.ConditionInfo.KeyDetail[1];
			}
		}

		public int targetTriggerID
		{
			get
			{
				return base.ConditionInfo.KeyDetail[2];
			}
		}

		public bool isTriggerd
		{
			get
			{
				return base.ConditionInfo.KeyDetail[3] == 1;
			}
		}

		private int targetCount
		{
			get
			{
				return base.ConditionInfo.ValueDetail[0];
			}
		}

		public override int[] values
		{
			get
			{
				return new int[]
				{
					this.TriggerCount
				};
			}
		}

		public override void Initialize(ResDT_ConditionInfo InConditionInfo)
		{
			base.Initialize(InConditionInfo);
			this.TriggerCount = 0;
			Singleton<TriggerEventSys>.instance.OnActorEnter += new TriggerEventDelegate(this.OnActorEnter);
			Singleton<TriggerEventSys>.instance.OnActorLeave += new TriggerEventDelegate(this.OnActorLeave);
			this.bCachedResults = SmartCompare.Compare<int>(this.TriggerCount, this.targetCount, this.operation);
		}

		public override void Dispose()
		{
			Singleton<TriggerEventSys>.instance.OnActorEnter -= new TriggerEventDelegate(this.OnActorEnter);
			Singleton<TriggerEventSys>.instance.OnActorLeave -= new TriggerEventDelegate(this.OnActorLeave);
			base.Dispose();
		}

		private void OnActorEnter(AreaEventTrigger sourceTrigger, object param)
		{
			ActorRoot actorRoot = param as ActorRoot;
			if (actorRoot != null && sourceTrigger != null && sourceTrigger.ID == this.targetTriggerID && actorRoot.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster && actorRoot.TheActorMeta.ConfigId == this.targetMonsterID)
			{
				this.TriggerCount++;
				this.CheckResult();
			}
		}

		private void CheckResult()
		{
			bool flag = SmartCompare.Compare<int>(this.TriggerCount, this.targetCount, this.operation);
			if (flag != this.bCachedResults)
			{
				this.bCachedResults = flag;
				this.TriggerChangedEvent();
			}
		}

		private void OnActorLeave(AreaEventTrigger sourceTrigger, object param)
		{
		}
	}
}
