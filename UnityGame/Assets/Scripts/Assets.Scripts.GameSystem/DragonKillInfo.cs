using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace Assets.Scripts.GameSystem
{
	public class DragonKillInfo
	{
		public int time;

		public PoolObjHandle<ActorRoot> killer;

		public int dragonType;
	}
}
