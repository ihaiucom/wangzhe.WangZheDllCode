using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class PassiveConditionAttribute : PassivetAttribute
	{
		public PassiveConditionAttribute(PassiveConditionType _type)
		{
			this.attributeType = (int)_type;
		}
	}
}
