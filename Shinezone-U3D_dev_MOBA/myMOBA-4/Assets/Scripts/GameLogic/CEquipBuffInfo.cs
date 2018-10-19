using System;

namespace Assets.Scripts.GameLogic
{
	public class CEquipBuffInfo : IComparable
	{
		public ushort m_equipID;

		public CrypticInt32 m_equipBuyPrice;

		public uint m_buffID;

		public ushort m_buffGroupID;

		public uint m_sequence;

		public bool m_isEnabled;

		public bool m_isNeedRemoved;

		public CEquipBuffInfo(ushort equipID, uint equipBuyPrice, uint buffID, ushort buffGroupID, uint sequence)
		{
			this.m_equipID = equipID;
			this.m_equipBuyPrice = (int)equipBuyPrice;
			this.m_buffID = buffID;
			this.m_buffGroupID = buffGroupID;
			this.m_sequence = sequence;
			this.m_isEnabled = false;
			this.m_isNeedRemoved = false;
		}

		public int CompareTo(object obj)
		{
			CEquipBuffInfo cEquipBuffInfo = obj as CEquipBuffInfo;
			if (this.m_buffID == cEquipBuffInfo.m_buffID && this.m_isEnabled != cEquipBuffInfo.m_isEnabled)
			{
				return (!this.m_isEnabled) ? -1 : 1;
			}
			if ((uint)this.m_equipBuyPrice > (uint)cEquipBuffInfo.m_equipBuyPrice)
			{
				return 1;
			}
			if ((uint)this.m_equipBuyPrice != (uint)cEquipBuffInfo.m_equipBuyPrice)
			{
				return -1;
			}
			if (this.m_sequence > cEquipBuffInfo.m_sequence)
			{
				return 1;
			}
			if (this.m_sequence == cEquipBuffInfo.m_sequence)
			{
				return 0;
			}
			return -1;
		}

		public bool IsEqual(uint buffID, ushort buffGroupID)
		{
			return this.m_buffID == buffID && this.m_buffGroupID == buffGroupID;
		}
	}
}
