using System;

namespace Assets.Scripts.GameLogic
{
	public class CEquipPassiveSkillInfo : IComparable
	{
		public ushort m_equipID;

		public CrypticInt32 m_equipBuyPrice;

		public uint m_passiveSkillID;

		public ushort m_passiveSkillGroupID;

		public uint m_sequence;

		public bool m_isEnabled;

		public uint[] m_passiveSkillRelatedFuncIDs;

		public bool m_isNeedRemoved;

		public CEquipPassiveSkillInfo(ushort equipID, uint equipBuyPrice, uint passiveSkillID, ushort passiveSkillGroupID, uint[] passiveSkillRelatedFuncIDs, uint sequence)
		{
			this.m_equipID = equipID;
			this.m_equipBuyPrice = (int)equipBuyPrice;
			this.m_passiveSkillID = passiveSkillID;
			this.m_passiveSkillGroupID = passiveSkillGroupID;
			this.m_passiveSkillRelatedFuncIDs = passiveSkillRelatedFuncIDs;
			this.m_sequence = sequence;
			this.m_isEnabled = false;
			this.m_isNeedRemoved = false;
		}

		public int CompareTo(object obj)
		{
			CEquipPassiveSkillInfo cEquipPassiveSkillInfo = obj as CEquipPassiveSkillInfo;
			if (this.m_passiveSkillID == cEquipPassiveSkillInfo.m_passiveSkillID && this.m_isEnabled != cEquipPassiveSkillInfo.m_isEnabled)
			{
				return (!this.m_isEnabled) ? -1 : 1;
			}
			if ((uint)this.m_equipBuyPrice > (uint)cEquipPassiveSkillInfo.m_equipBuyPrice)
			{
				return 1;
			}
			if ((uint)this.m_equipBuyPrice != (uint)cEquipPassiveSkillInfo.m_equipBuyPrice)
			{
				return -1;
			}
			if (this.m_sequence > cEquipPassiveSkillInfo.m_sequence)
			{
				return 1;
			}
			if (this.m_sequence == cEquipPassiveSkillInfo.m_sequence)
			{
				return 0;
			}
			return -1;
		}

		public bool IsEqual(uint passiveSkillID, ushort passiveSkillGroupID, uint[] passiveSkillRelatedFuncIDs)
		{
			if (this.m_passiveSkillID == passiveSkillID && this.m_passiveSkillGroupID == passiveSkillGroupID)
			{
				if (this.m_passiveSkillRelatedFuncIDs == null && passiveSkillRelatedFuncIDs == null)
				{
					return true;
				}
				if (this.m_passiveSkillRelatedFuncIDs != null && passiveSkillRelatedFuncIDs != null && this.m_passiveSkillRelatedFuncIDs.Length == passiveSkillRelatedFuncIDs.Length)
				{
					for (int i = 0; i < this.m_passiveSkillRelatedFuncIDs.Length; i++)
					{
						if (this.m_passiveSkillRelatedFuncIDs[i] != passiveSkillRelatedFuncIDs[i])
						{
							return false;
						}
					}
					return true;
				}
			}
			return false;
		}
	}
}
