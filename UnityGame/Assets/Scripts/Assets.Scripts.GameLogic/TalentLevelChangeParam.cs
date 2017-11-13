using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct TalentLevelChangeParam
	{
		public PoolObjHandle<ActorRoot> src;

		public int SoulLevel;

		public int TalentLevel;
	}
}
