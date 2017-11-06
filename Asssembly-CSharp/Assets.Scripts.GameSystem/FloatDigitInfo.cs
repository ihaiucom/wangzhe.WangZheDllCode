using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace Assets.Scripts.GameSystem
{
	public struct FloatDigitInfo
	{
		public PoolObjHandle<ActorRoot> m_attacker;

		public PoolObjHandle<ActorRoot> m_target;

		public DIGIT_TYPE m_digitType;

		public int m_value;

		public FloatDigitInfo(PoolObjHandle<ActorRoot> attacker, PoolObjHandle<ActorRoot> target, DIGIT_TYPE digitType, int value)
		{
			this.m_attacker = attacker;
			this.m_target = target;
			this.m_digitType = digitType;
			this.m_value = value;
		}
	}
}
