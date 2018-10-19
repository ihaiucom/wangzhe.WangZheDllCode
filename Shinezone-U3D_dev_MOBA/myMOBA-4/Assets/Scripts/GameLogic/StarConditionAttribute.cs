using System;

namespace Assets.Scripts.GameLogic
{
	public class StarConditionAttribute : AutoRegisterAttribute, IIdentifierAttribute<int>
	{
		public int CondType;

		public int ID
		{
			get
			{
				return this.CondType;
			}
		}

		public int[] AdditionalIdList
		{
			get
			{
				return null;
			}
		}

		public StarConditionAttribute(int InCondType)
		{
			this.CondType = InCondType;
		}
	}
}
