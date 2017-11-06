using System;

namespace Assets.Scripts.GameLogic
{
	public class StarConditionAttrContextAttribute : AutoRegisterAttribute, IIdentifierAttribute<int>
	{
		public int KeyType;

		public int ID
		{
			get
			{
				return this.KeyType;
			}
		}

		public int[] AdditionalIdList
		{
			get
			{
				return null;
			}
		}

		public StarConditionAttrContextAttribute(int InKeyType)
		{
			this.KeyType = InKeyType;
		}
	}
}
