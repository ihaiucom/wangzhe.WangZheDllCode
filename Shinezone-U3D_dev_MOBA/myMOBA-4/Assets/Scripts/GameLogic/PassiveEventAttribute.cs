using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class PassiveEventAttribute : PassivetAttribute
	{
		public PassiveEventAttribute(PassiveEventType _type)
		{
			this.attributeType = (int)_type;
		}
	}
}
