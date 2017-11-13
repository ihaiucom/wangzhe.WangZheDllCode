using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct AddSoulExpEventParam
	{
		public PoolObjHandle<ActorRoot> src;

		public PoolObjHandle<ActorRoot> atker;

		public int iAddExpValue;

		public AddSoulExpEventParam(PoolObjHandle<ActorRoot> _src, PoolObjHandle<ActorRoot> _atker, int _iAddExpValue)
		{
			this.src = _src;
			this.atker = _atker;
			this.iAddExpValue = _iAddExpValue;
		}
	}
}
