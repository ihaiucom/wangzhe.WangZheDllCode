using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	public class NONHERO_STATISTIC_INFO
	{
		public ActorTypeDef ActorType;

		public COM_PLAYERCAMP ActorCamp;

		public uint uiTotalSpawnNum;

		public uint uiTotalDeadNum;

		public uint uiTotalAttackNum;

		public uint uiTotalHurtCount;

		public uint uiHurtMax;

		public uint uiHurtMin;

		public uint uiTotalBeAttackedNum;

		public uint uiTotalBeHurtCount;

		public uint uiBeHurtMax;

		public uint uiBeHurtMin;

		public uint uiAttackDistanceMax;

		public uint uiAttackDistanceMin;

		public uint uiHpMax;

		public uint uiHpMin;

		public uint uiFirstBeAttackTime;

		public NONHERO_STATISTIC_INFO()
		{
			this.ActorType = ActorTypeDef.Actor_Type_Monster;
			this.ActorCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
			this.uiTotalSpawnNum = 0u;
			this.uiTotalDeadNum = 0u;
			this.uiTotalAttackNum = 0u;
			this.uiTotalHurtCount = 0u;
			this.uiTotalBeAttackedNum = 0u;
			this.uiTotalBeHurtCount = 0u;
			this.uiAttackDistanceMax = 0u;
			this.uiAttackDistanceMin = 4294967295u;
			this.uiHurtMax = 0u;
			this.uiHurtMin = 4294967295u;
			this.uiBeHurtMax = 0u;
			this.uiBeHurtMin = 4294967295u;
			this.uiHpMax = 0u;
			this.uiHpMin = 4294967295u;
			this.uiFirstBeAttackTime = 0u;
		}
	}
}
