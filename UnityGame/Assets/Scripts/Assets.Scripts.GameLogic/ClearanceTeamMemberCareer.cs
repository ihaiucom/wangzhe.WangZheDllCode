using Assets.Scripts.Common;
using Assets.Scripts.GameLogic.GameKernal;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[StarConditionAttrContext(9)]
	internal class ClearanceTeamMemberCareer : StarCondition
	{
		private StarEvaluationStatus CachedStatus;

		private int CareerCount;

		private int targetCareer
		{
			get
			{
				return base.ConditionInfo.KeyDetail[1];
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
				return this.CachedStatus;
			}
		}

		public override int[] values
		{
			get
			{
				return new int[]
				{
					this.CareerCount
				};
			}
		}

		public override void Initialize(ResDT_ConditionInfo InConditionInfo)
		{
			base.Initialize(InConditionInfo);
		}

		public override void Start()
		{
			base.Start();
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			DebugHelper.Assert(hostPlayer != null);
			ReadonlyContext<PoolObjHandle<ActorRoot>>.Enumerator enumerator = hostPlayer.GetAllHeroes().GetEnumerator();
			this.CareerCount = 0;
			int targetCareer = this.targetCareer;
			while (enumerator.MoveNext())
			{
				PoolObjHandle<ActorRoot> current = enumerator.Current;
				if (current.handle.TheActorMeta.ActorType == (ActorTypeDef)targetCareer)
				{
					this.CareerCount++;
				}
			}
			this.CachedStatus = (SmartCompare.Compare<int>(this.CareerCount, this.targetCount, this.operation) ? StarEvaluationStatus.Success : StarEvaluationStatus.Failure);
		}
	}
}
