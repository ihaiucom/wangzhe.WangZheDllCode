using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct BuffChangeEventParam
	{
		public bool bIsAdd;

		public PoolObjHandle<ActorRoot> target;

		public PoolObjHandle<BuffSkill> stBuffSkill;

		public BuffChangeEventParam(bool _bIsAdd, PoolObjHandle<ActorRoot> _target, BuffSkill _stBuffSkill)
		{
			this.bIsAdd = _bIsAdd;
			this.target = _target;
			this.stBuffSkill = new PoolObjHandle<BuffSkill>(_stBuffSkill);
		}
	}
}
