using System;

namespace Assets.Scripts.GameSystem
{
	public class VoiceInteractionAttribute : AutoRegisterAttribute, IIdentifierAttribute<int>
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

		public VoiceInteractionAttribute(int InKeyType)
		{
			this.KeyType = InKeyType;
		}
	}
}
