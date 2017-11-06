using System;

namespace Assets.Scripts.GameLogic
{
	public struct HURT_INFO
	{
		public HurtTypeDef hurtType;

		public int iValue;

		public string strIconName;

		public string strName;

		public SKILL_USE_FROM_TYPE SkillUseFrom;

		public HURT_INFO(HurtTypeDef _hurtType, string _strIconName, string _strName, int _iValue, SKILL_USE_FROM_TYPE _skillUseFrom)
		{
			this.hurtType = _hurtType;
			this.strIconName = _strIconName;
			this.strName = _strName;
			this.iValue = _iValue;
			this.SkillUseFrom = _skillUseFrom;
		}
	}
}
