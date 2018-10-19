using System;

namespace Assets.Scripts.GameLogic.DataCenter
{
	[Serializable]
	public struct ActorServerEquipData
	{
		public struct EquipDetailInfo
		{
			public int StackCount;
		}

		public ActorMeta TheActorMeta;

		public ActorEquiplSlot EquipSlot;

		public uint EquipCfgId;

		public ulong EquipUid;

		public ActorServerEquipData.EquipDetailInfo TheDetailInfo;
	}
}
