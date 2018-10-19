using Assets.Scripts.Framework;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	public class GameTaskKill : GameTask
	{
		protected COM_PLAYERCAMP SubjCamp
		{
			get
			{
				return (COM_PLAYERCAMP)base.Config.iParam1;
			}
		}

		protected override void OnInitial()
		{
		}

		protected override void OnDestroy()
		{
			this.OnClose();
		}

		protected override void OnStart()
		{
			DictionaryView<uint, CampInfo> campStat = Singleton<BattleLogic>.GetInstance().battleStat.GetCampStat();
			if (campStat.ContainsKey((uint)this.SubjCamp))
			{
				CampInfo campInfo = campStat[(uint)this.SubjCamp];
				campInfo.onCampScoreChanged += new CampInfo.CampInfoValueChanged(this.ValidateKill);
				base.Current = campInfo.campScore;
			}
		}

		protected override void OnClose()
		{
			DictionaryView<uint, CampInfo> campStat = Singleton<BattleLogic>.GetInstance().battleStat.GetCampStat();
			if (campStat.ContainsKey((uint)this.SubjCamp))
			{
				CampInfo campInfo = campStat[(uint)this.SubjCamp];
				campInfo.onCampScoreChanged -= new CampInfo.CampInfoValueChanged(this.ValidateKill);
			}
		}

		private void ValidateKill(COM_PLAYERCAMP campType, int inCampScore, int inHeadPts)
		{
			if (inCampScore >= 0)
			{
				base.Current = inCampScore;
			}
		}
	}
}
