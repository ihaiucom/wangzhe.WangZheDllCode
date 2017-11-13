using Assets.Scripts.Common;
using Assets.Scripts.GameLogic.GameKernal;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[StarConditionAttrContext(4)]
	internal class StarConditionRestrictHero : StarCondition
	{
		private struct Info
		{
			public int Key;

			public int Value;
		}

		private ListView<StarConditionRestrictHero.Info> TeamStat = new ListView<StarConditionRestrictHero.Info>();

		private bool bCheckResults;

		private int targetID
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
				return this.bCheckResults ? StarEvaluationStatus.Success : StarEvaluationStatus.Failure;
			}
		}

		public override int[] values
		{
			get
			{
				return new int[]
				{
					this.bCheckResults ? 1 : 0
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
			bool flag = this.CheckResults();
			if (this.bCheckResults != flag)
			{
				this.bCheckResults = flag;
				this.TriggerChangedEvent();
			}
		}

		private bool CheckResults()
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer == null)
			{
				return false;
			}
			ReadonlyContext<PoolObjHandle<ActorRoot>>.Enumerator enumerator = hostPlayer.GetAllHeroes().GetEnumerator();
			bool flag;
			while (enumerator.MoveNext())
			{
				PoolObjHandle<ActorRoot> current = enumerator.Current;
				int configId = current.handle.TheActorMeta.ConfigId;
				flag = false;
				for (int i = 0; i < this.TeamStat.Count; i++)
				{
					if (this.TeamStat[i].Key == configId)
					{
						flag = true;
						StarConditionRestrictHero.Info info;
						this.TeamStat[i].Value = info.Value + 1;
						break;
					}
				}
				if (!flag)
				{
					StarConditionRestrictHero.Info item = new StarConditionRestrictHero.Info
					{
						Key = configId,
						Value = 1
					};
					this.TeamStat.Add(item);
				}
			}
			int inFirst = 0;
			flag = false;
			for (int j = 0; j < this.TeamStat.Count; j++)
			{
				if (this.TeamStat[j].Key == this.targetID)
				{
					inFirst = this.TeamStat[j].Value;
					flag = true;
					break;
				}
			}
			return flag && SmartCompare.Compare<int>(inFirst, this.targetCount, this.operation);
		}
	}
}
