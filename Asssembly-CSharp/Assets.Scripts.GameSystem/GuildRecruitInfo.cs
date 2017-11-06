using System;

namespace Assets.Scripts.GameSystem
{
	public struct GuildRecruitInfo
	{
		public ulong senderUid;

		public byte senderLevel;

		public ulong guildId;

		public int guildLogicWorldId;

		public uint sendTime;

		public string guildName;

		public string senderName;

		public string senderHeadUrl;

		public byte limitGrade;

		public byte limitLevel;
	}
}
