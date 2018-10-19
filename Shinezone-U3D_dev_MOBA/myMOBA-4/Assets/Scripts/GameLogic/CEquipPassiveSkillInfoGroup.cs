using System;

namespace Assets.Scripts.GameLogic
{
	public class CEquipPassiveSkillInfoGroup
	{
		public ushort m_groupID;

		public bool m_isChanged;

		public ListView<CEquipPassiveSkillInfo> m_equipPassiveSkillInfos = new ListView<CEquipPassiveSkillInfo>();

		public CEquipPassiveSkillInfoGroup(ushort groupID)
		{
			this.m_groupID = groupID;
			this.m_isChanged = false;
		}
	}
}
