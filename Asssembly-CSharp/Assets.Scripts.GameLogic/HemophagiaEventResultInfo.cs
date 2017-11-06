using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct HemophagiaEventResultInfo
	{
		public PoolObjHandle<ActorRoot> src;

		public int hpChanged;

		public HemophagiaEventResultInfo(PoolObjHandle<ActorRoot> _src, int _hpChanged)
		{
			this.src = _src;
			this.hpChanged = _hpChanged;
		}
	}
}
