using System;

namespace Assets.Scripts.GameLogic
{
	public class CEquipPassiveCdInfo
	{
		public uint m_passiveSkillId;

		public int m_passiveCd;

		public CEquipPassiveCdInfo(uint passiveSkillID, int cd)
		{
			this.m_passiveSkillId = passiveSkillID;
			this.m_passiveCd = cd;
		}
	}
}
