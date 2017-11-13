using System;

namespace Assets.Scripts.GameLogic
{
	public class PassivetAttribute : Attribute
	{
		public int attributeType;

		public PassivetAttribute()
		{
		}

		public PassivetAttribute(int _type)
		{
			this.attributeType = _type;
		}
	}
}
