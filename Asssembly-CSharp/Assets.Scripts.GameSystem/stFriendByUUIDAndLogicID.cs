using System;

namespace Assets.Scripts.GameSystem
{
	public struct stFriendByUUIDAndLogicID
	{
		public ulong ullUid;

		public uint dwLogicWorldID;

		public CFriendModel.FriendType friendType;

		public stFriendByUUIDAndLogicID(ulong uuid, uint logicWorldID, CFriendModel.FriendType type)
		{
			this.ullUid = uuid;
			this.dwLogicWorldID = logicWorldID;
			this.friendType = type;
		}
	}
}
