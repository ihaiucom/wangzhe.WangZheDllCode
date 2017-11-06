using Assets.Scripts.Common;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class KillDetailInfo
	{
		public PoolObjHandle<ActorRoot> Victim;

		public PoolObjHandle<ActorRoot> Killer;

		public List<uint> assistList;

		public KillDetailInfoType Type;

		public KillDetailInfoType HeroMultiKillType;

		public KillDetailInfoType HeroContiKillType;

		public bool bSelfCamp;

		public bool bAllDead;

		public bool bPlayerSelf_KillOrKilled;

		public bool bSuicide;
	}
}
