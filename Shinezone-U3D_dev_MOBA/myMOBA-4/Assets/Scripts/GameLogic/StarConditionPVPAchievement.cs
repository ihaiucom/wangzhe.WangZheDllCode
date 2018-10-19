using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[StarConditionAttrContext(13)]
	internal class StarConditionPVPAchievement : StarCondition
	{
		private int CompleteCount;

		private bool bHasComplete;

		private KillDetailInfoType targetAchievementType
		{
			get
			{
				return (KillDetailInfoType)base.ConditionInfo.KeyDetail[1];
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
				bool flag = SmartCompare.Compare<int>(this.CompleteCount, this.targetCount, this.operation);
				return (!flag) ? StarEvaluationStatus.InProgressing : StarEvaluationStatus.Success;
			}
		}

		public override int[] values
		{
			get
			{
				return new int[]
				{
					this.CompleteCount
				};
			}
		}

		public override string description
		{
			get
			{
				return string.Format("[{0}/{1}]", this.CompleteCount, this.targetCount);
			}
		}

		public override void Initialize(ResDT_ConditionInfo InConditionInfo)
		{
			base.Initialize(InConditionInfo);
			Singleton<EventRouter>.instance.AddEventHandler<KillDetailInfo>(EventID.AchievementRecorderEvent, new Action<KillDetailInfo>(this.OnAchievementEvent));
		}

		public override void Dispose()
		{
			Singleton<EventRouter>.instance.RemoveEventHandler<KillDetailInfo>(EventID.AchievementRecorderEvent, new Action<KillDetailInfo>(this.OnAchievementEvent));
			base.Dispose();
		}

		private void OnAchievementEvent(KillDetailInfo KillDetail)
		{
			if (KillDetail.Killer && KillDetail.Killer.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && ActorHelper.IsHostCtrlActor(ref KillDetail.Killer) && (KillDetail.HeroContiKillType == this.targetAchievementType || KillDetail.HeroMultiKillType == this.targetAchievementType))
			{
				this.CompleteCount++;
				if (!this.bHasComplete && this.status == StarEvaluationStatus.Success)
				{
					this.bHasComplete = true;
					this.TriggerChangedEvent();
				}
			}
		}
	}
}
