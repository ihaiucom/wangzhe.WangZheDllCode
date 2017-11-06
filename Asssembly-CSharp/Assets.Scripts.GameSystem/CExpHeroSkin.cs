using System;

namespace Assets.Scripts.GameSystem
{
	public class CExpHeroSkin : CHeroSkin
	{
		public uint expDays;

		public CExpHeroSkin(ulong objID, uint baseID, uint expDays, int stackCount = 0, int addTime = 0) : base(objID, baseID, stackCount, addTime)
		{
			this.expDays = expDays;
		}
	}
}
