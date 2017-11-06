using System;

namespace Assets.Scripts.GameLogic
{
	public struct CampJudgeRecord
	{
		public ushort killNum;

		public ushort deadNum;

		public uint gainCoin;

		public uint hurtToHero;

		public uint sufferHero;

		public void Reset()
		{
			this.killNum = 0;
			this.deadNum = 0;
			this.gainCoin = 0u;
			this.hurtToHero = 0u;
			this.sufferHero = 0u;
		}
	}
}
