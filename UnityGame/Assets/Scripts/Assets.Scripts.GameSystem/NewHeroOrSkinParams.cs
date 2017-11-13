using Assets.Scripts.UI;
using CSProtocol;
using System;

namespace Assets.Scripts.GameSystem
{
	public struct NewHeroOrSkinParams
	{
		public uint heroId;

		public uint skinId;

		public enUIEventID eventID;

		public bool bInitAnima;

		public COM_REWARDS_TYPE rewardType;

		public bool interactableTransition;

		public string prefabPath;

		public enFormPriority priority;

		public uint convertCoin;

		public int experienceDays;
	}
}
