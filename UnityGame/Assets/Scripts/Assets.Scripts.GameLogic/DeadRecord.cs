using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct DeadRecord
	{
		public COM_PLAYERCAMP camp;

		public ActorTypeDef actorType;

		public int cfgId;

		public int deadTime;

		public COM_PLAYERCAMP killerCamp;

		public uint AttackPlayerID;

		public ActorTypeDef killerActorType;

		public byte actorSubType;

		public byte actorSubSoliderType;

		public int iOrder;

		public int fightTime;

		public DeadRecord(COM_PLAYERCAMP camp, ActorTypeDef actorType, int cfgId, int deadTime, COM_PLAYERCAMP killerCamp, uint plyaerID, ActorTypeDef killerActorType)
		{
			this.camp = camp;
			this.actorType = actorType;
			this.cfgId = cfgId;
			this.deadTime = deadTime;
			this.killerCamp = killerCamp;
			this.killerActorType = killerActorType;
			this.AttackPlayerID = plyaerID;
			this.actorSubType = 0;
			this.actorSubSoliderType = 0;
			this.iOrder = 0;
			this.fightTime = 0;
		}
	}
}
