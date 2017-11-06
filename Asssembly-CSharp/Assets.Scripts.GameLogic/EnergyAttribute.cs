using System;

namespace Assets.Scripts.GameLogic
{
	public class EnergyAttribute : Attribute
	{
		public int attributeType;

		public EnergyAttribute(EnergyType _type)
		{
			this.attributeType = (int)_type;
		}
	}
}
