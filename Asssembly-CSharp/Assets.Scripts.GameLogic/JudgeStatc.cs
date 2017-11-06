using System;

namespace Assets.Scripts.GameLogic
{
	public struct JudgeStatc
	{
		public ushort KillNum;

		public ushort DeadNum;

		public ushort AssitNum;

		public ushort GainCoin;

		public uint HurtToHero;

		public uint HurtToAll;

		public uint SufferHero;

		public void Reset()
		{
			this.KillNum = 0;
			this.DeadNum = 0;
			this.AssitNum = 0;
			this.GainCoin = 0;
			this.HurtToHero = 0u;
			this.HurtToAll = 0u;
			this.SufferHero = 0u;
		}
	}
}
