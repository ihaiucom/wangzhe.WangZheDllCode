using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic.DataCenter
{
	[Serializable]
	public struct ActorMeta
	{
		public int ConfigId;

		public int HostConfigId;

		public uint PlayerId;

		public ActorTypeDef ActorType;

		public COM_PLAYERCAMP ActorCamp;

		public CrypticInt32 EnCId;

		public uint SkinID;

		public byte Difficuty;

		public bool Invincible;

		public bool NotMovable;
	}
}
