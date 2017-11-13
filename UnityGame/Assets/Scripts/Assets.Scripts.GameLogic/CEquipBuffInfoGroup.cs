using System;

namespace Assets.Scripts.GameLogic
{
	public class CEquipBuffInfoGroup
	{
		public ushort m_groupID;

		public bool m_isChanged;

		public ListView<CEquipBuffInfo> m_equipBuffInfos = new ListView<CEquipBuffInfo>();

		public CEquipBuffInfoGroup(ushort groupID)
		{
			this.m_groupID = groupID;
			this.m_isChanged = false;
		}
	}
}
