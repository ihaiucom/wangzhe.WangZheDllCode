using Assets.Scripts.UI;
using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class CTrophyRewardInfo
	{
		public int Index;

		public ResTrophyLvl Cfg;

		public TrophyState State;

		public uint MinPoint;

		public uint MaxPoint;

		public CTrophyRewardInfo(ResTrophyLvl resTrophy, TrophyState state = TrophyState.UnFinish, int index = 0, uint minPoint = 0u)
		{
			this.Cfg = resTrophy;
			this.State = state;
			this.Index = index;
			this.MinPoint = minPoint;
			this.MaxPoint = this.Cfg.dwTrophyScore;
		}

		public uint GetPointStep()
		{
			return this.MaxPoint - this.MinPoint;
		}

		public bool IsFinish()
		{
			return this.State == TrophyState.Finished || this.State == TrophyState.GotRewards;
		}

		public bool HasGotAward()
		{
			return this.State == TrophyState.GotRewards;
		}

		public bool IsRewardConfiged()
		{
			uint num = 0u;
			for (int i = 0; i < 3; i++)
			{
				if (this.Cfg.astReqReward[i].dwRewardNum != 0u)
				{
					num += 1u;
				}
			}
			return num > 0u;
		}

		public string GetTrophyImagePath()
		{
			return string.Format("{0}{1}", CUIUtility.s_Sprite_System_BattleEquip_Dir, this.Cfg.dwImage);
		}

		public CUseable[] GetTrophyRewards()
		{
			ListView<CUseable> listView = new ListView<CUseable>();
			for (int i = 0; i < this.Cfg.astReqReward.Length; i++)
			{
				if (this.Cfg.astReqReward[i].dwRewardNum > 0u)
				{
					listView.Add(CUseableManager.CreateUsableByServerType((RES_REWARDS_TYPE)this.Cfg.astReqReward[i].bRewardType, (int)this.Cfg.astReqReward[i].dwRewardNum, this.Cfg.astReqReward[i].dwRewardID));
				}
			}
			CUseable[] array = new CUseable[listView.Count];
			for (int j = 0; j < listView.Count; j++)
			{
				array[j] = listView[j];
			}
			return array;
		}
	}
}
