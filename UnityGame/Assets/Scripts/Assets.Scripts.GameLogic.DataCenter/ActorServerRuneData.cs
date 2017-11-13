using System;

namespace Assets.Scripts.GameLogic.DataCenter
{
	[Serializable]
	public struct ActorServerRuneData
	{
		public ActorMeta TheActorMeta;

		public ActorRunelSlot RuneSlot;

		public uint RuneId;
	}
}
