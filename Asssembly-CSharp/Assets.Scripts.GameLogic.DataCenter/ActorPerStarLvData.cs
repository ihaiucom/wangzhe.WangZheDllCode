using System;

namespace Assets.Scripts.GameLogic.DataCenter
{
	[Serializable]
	public struct ActorPerStarLvData
	{
		public ActorMeta TheActorMeta;

		public ActorStarLv StarLv;

		public int PerLvHp;

		public int PerLvAd;

		public int PerLvAp;

		public int PerLvDef;

		public int PerLvRes;
	}
}
