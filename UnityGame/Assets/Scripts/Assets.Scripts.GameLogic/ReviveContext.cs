using System;

namespace Assets.Scripts.GameLogic
{
	public struct ReviveContext
	{
		public bool bEnable;

		public int ReviveTime;

		public int ReviveLife;

		public int ReviveEnergy;

		public bool AutoReset;

		public bool bBaseRevive;

		public bool bCDReset;

		public int iReviveBuffId;

		public uint uiBuffObjId;

		public bool bIsPassiveSkill;

		public void Reset()
		{
			this.ReviveTime = 0;
			this.ReviveLife = 10000;
			this.ReviveEnergy = 10000;
			this.AutoReset = false;
			this.bBaseRevive = true;
			this.bCDReset = false;
			this.bEnable = false;
			this.iReviveBuffId = 0;
			this.bIsPassiveSkill = true;
			this.uiBuffObjId = 0u;
		}
	}
}
