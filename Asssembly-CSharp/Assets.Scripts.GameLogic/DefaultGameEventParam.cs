using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct DefaultGameEventParam
	{
		public PoolObjHandle<ActorRoot> src;

		public PoolObjHandle<ActorRoot> atker;

		public PoolObjHandle<ActorRoot> orignalAtker;

		public PoolObjHandle<ActorRoot> logicAtker;

		public DefaultGameEventParam(PoolObjHandle<ActorRoot> _src, PoolObjHandle<ActorRoot> _atker)
		{
			this.src = _src;
			this.atker = _atker;
			this.orignalAtker = _atker;
			this.logicAtker = _atker;
		}

		public DefaultGameEventParam(PoolObjHandle<ActorRoot> _src, PoolObjHandle<ActorRoot> _atker, ref PoolObjHandle<ActorRoot> _orignalAtker)
		{
			this.src = _src;
			this.atker = _atker;
			this.orignalAtker = _orignalAtker;
			this.logicAtker = _orignalAtker;
		}

		public DefaultGameEventParam(PoolObjHandle<ActorRoot> _src, PoolObjHandle<ActorRoot> _atker, ref PoolObjHandle<ActorRoot> _orignalAtker, ref PoolObjHandle<ActorRoot> _logicAtker)
		{
			this.src = _src;
			this.atker = _atker;
			this.orignalAtker = _orignalAtker;
			this.logicAtker = _logicAtker;
		}
	}
}
