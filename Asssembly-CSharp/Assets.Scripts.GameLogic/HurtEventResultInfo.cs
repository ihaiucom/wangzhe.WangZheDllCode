using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct HurtEventResultInfo
	{
		public PoolObjHandle<ActorRoot> src;

		public PoolObjHandle<ActorRoot> atker;

		public HurtDataInfo hurtInfo;

		public int hurtTotal;

		public int hpChanged;

		public int critValue;

		public HurtEventResultInfo(PoolObjHandle<ActorRoot> _src, PoolObjHandle<ActorRoot> _atker, HurtDataInfo _hurtInfo, int _hurtTotal, int _hpChanged, int _critValue)
		{
			this.src = _src;
			this.atker = _atker;
			this.hurtInfo = _hurtInfo;
			this.hurtTotal = _hurtTotal;
			this.hpChanged = _hpChanged;
			this.critValue = _critValue;
		}
	}
}
