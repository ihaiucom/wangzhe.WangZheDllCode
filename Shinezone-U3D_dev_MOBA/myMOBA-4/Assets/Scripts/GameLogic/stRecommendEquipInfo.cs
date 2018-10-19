using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct stRecommendEquipInfo
	{
		public ushort m_equipID;

		public ResEquipInBattle m_resEquipInBattle;

		public bool m_hasBeenBought;

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object other)
		{
			return other != null && base.GetType() == other.GetType() && this.IsEquals((stRecommendEquipInfo)other);
		}

		private bool IsEquals(stRecommendEquipInfo rhs)
		{
			return this.m_equipID == rhs.m_equipID && this.m_resEquipInBattle == rhs.m_resEquipInBattle && this.m_hasBeenBought == rhs.m_hasBeenBought;
		}
	}
}
