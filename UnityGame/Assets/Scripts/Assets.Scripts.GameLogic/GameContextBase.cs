using System;

namespace Assets.Scripts.GameLogic
{
	public class GameContextBase
	{
		protected SLevelContext LevelContext;

		protected int RewardCount;

		public SLevelContext levelContext
		{
			get
			{
				return this.LevelContext;
			}
		}

		public int rewardCount
		{
			get
			{
				return this.RewardCount;
			}
		}

		public virtual GameInfoBase CreateGameInfo()
		{
			return null;
		}

		public virtual void PrepareStartup()
		{
		}
	}
}
