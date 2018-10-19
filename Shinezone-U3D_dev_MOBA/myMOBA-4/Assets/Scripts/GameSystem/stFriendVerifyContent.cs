using CSProtocol;
using System;

namespace Assets.Scripts.GameSystem
{
	public class stFriendVerifyContent
	{
		public ulong ullUid;

		public uint dwLogicWorldID;

		public string content;

		public COMDT_FRIEND_SOURCE friendSource;

		public int mentorType;

		public stFriendVerifyContent(ulong ullUid, uint dwLogicWorldID, string content, COMDT_FRIEND_SOURCE friendSource, int mentor_type = 0)
		{
			this.ullUid = ullUid;
			this.dwLogicWorldID = dwLogicWorldID;
			this.content = content;
			this.friendSource = friendSource;
			this.mentorType = mentor_type;
		}
	}
}
