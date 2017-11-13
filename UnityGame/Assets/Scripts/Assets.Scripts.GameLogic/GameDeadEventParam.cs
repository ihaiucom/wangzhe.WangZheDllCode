using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct GameDeadEventParam
	{
		public PoolObjHandle<ActorRoot> src;

		public PoolObjHandle<ActorRoot> atker;

		public PoolObjHandle<ActorRoot> orignalAtker;

		public PoolObjHandle<ActorRoot> logicAtker;

		public bool bImmediateRevive;

		public bool bSuicide;

		public SpawnPoint spawnPoint;

		public bool bIsPassiveSkillRevive;

		public GameDeadEventParam(PoolObjHandle<ActorRoot> _src, PoolObjHandle<ActorRoot> _atker, PoolObjHandle<ActorRoot> _orignalAtker, PoolObjHandle<ActorRoot> _logicAtker, bool inImmediateRevive, bool inSuicide, SpawnPoint _spawnPoint = null, bool _bIsPassiveSkillRevive = true)
		{
			this.src = _src;
			this.atker = _atker;
			this.orignalAtker = _orignalAtker;
			this.logicAtker = _logicAtker;
			this.bImmediateRevive = inImmediateRevive;
			this.bSuicide = inSuicide;
			this.spawnPoint = _spawnPoint;
			this.bIsPassiveSkillRevive = _bIsPassiveSkillRevive;
		}
	}
}
