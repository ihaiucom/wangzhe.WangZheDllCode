using Assets.Scripts.Common;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[StarConditionAttrContext(7)]
	internal class StarConditionDataStat : StarCondition
	{
		private int Score;

		private bool bCheckResults = true;

		private PoolObjHandle<ActorRoot> CachedSource;

		private PoolObjHandle<ActorRoot> CachedAttacker;

		public override int[] values
		{
			get
			{
				return new int[]
				{
					this.Score
				};
			}
		}

		public RES_STAR_CONDITION_DATA_SUB_TYPE DataSubType
		{
			get
			{
				return (RES_STAR_CONDITION_DATA_SUB_TYPE)base.ConditionInfo.KeyDetail[1];
			}
		}

		public int TargetScore
		{
			get
			{
				return base.ConditionInfo.ValueDetail[0];
			}
		}

		private int TargetCamp
		{
			get
			{
				return base.ConditionInfo.KeyDetail[2];
			}
		}

		public override StarEvaluationStatus status
		{
			get
			{
				return this.bCheckResults ? StarEvaluationStatus.Success : StarEvaluationStatus.Failure;
			}
		}

		public override void Initialize(ResDT_ConditionInfo InConditionInfo)
		{
			base.Initialize(InConditionInfo);
			this.bCheckResults = SmartCompare.Compare<int>(this.Score, this.TargetScore, this.operation);
		}

		public override void Dispose()
		{
			base.Dispose();
		}

		public override void OnCampScoreUpdated(ref SCampScoreUpdateParam prm)
		{
			if (!this.ShouldCare(ref prm))
			{
				return;
			}
			this.CachedSource = prm.src;
			this.CachedAttacker = prm.atker;
			if (this.DataSubType == RES_STAR_CONDITION_DATA_SUB_TYPE.RES_STAR_CONDITION_DATA_HEAD_POINTS && prm.HeadPoints >= 0)
			{
				this.Score = prm.HeadPoints;
			}
			else if (this.DataSubType == RES_STAR_CONDITION_DATA_SUB_TYPE.RES_STAR_CONDITION_DATA_HEADS && prm.HeadCount >= 0)
			{
				this.Score = prm.HeadCount;
			}
			bool flag = SmartCompare.Compare<int>(this.Score, this.TargetScore, this.operation);
			if (this.bCheckResults != flag)
			{
				this.bCheckResults = flag;
				this.TriggerChangedEvent();
			}
		}

		private bool ShouldCare(ref SCampScoreUpdateParam prm)
		{
			if (prm.CampType == COM_PLAYERCAMP.COM_PLAYERCAMP_MID || prm.CampType >= COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT)
			{
				return false;
			}
			if (this.TargetCamp == 0)
			{
				return prm.CampType == Singleton<GamePlayerCenter>.instance.hostPlayerCamp;
			}
			return prm.CampType != Singleton<GamePlayerCenter>.instance.hostPlayerCamp;
		}

		public override bool GetActorRef(out PoolObjHandle<ActorRoot> OutSource, out PoolObjHandle<ActorRoot> OutAttacker)
		{
			OutSource = this.CachedSource;
			OutAttacker = this.CachedAttacker;
			return true;
		}
	}
}
