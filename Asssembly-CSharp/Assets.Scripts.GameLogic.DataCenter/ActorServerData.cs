using Assets.Scripts.GameSystem;
using System;

namespace Assets.Scripts.GameLogic.DataCenter
{
	[Serializable]
	public struct ActorServerData
	{
		public struct QualityInfo
		{
			public uint Quality;

			public uint SubQuality;
		}

		public struct ProficiencyInfo
		{
			public uint Proficiency;

			public uint Level;
		}

		public struct BurnInfo
		{
			public uint HeroRemainingHp;

			public bool IsDead;
		}

		public struct ExtraInfo
		{
			public int BornPointIndex;
		}

		public ActorMeta TheActorMeta;

		public uint Level;

		public uint Star;

		public uint Exp;

		public uint SkinId;

		public uint[] SymbolID;

		public stRcmdEquipListInfo m_customRecommendEquips;

		public ActorServerData.QualityInfo TheQualityInfo;

		public ActorServerData.ProficiencyInfo TheProficiencyInfo;

		public ActorServerData.BurnInfo TheBurnInfo;

		public ActorServerData.ExtraInfo TheExtraInfo;
	}
}
