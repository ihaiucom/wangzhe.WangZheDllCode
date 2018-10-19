using System;

namespace Assets.Scripts.GameSystem
{
	public class TeamMember
	{
		public PlayerUniqueID uID;

		public uint dwPosOfTeam;

		public string MemberName;

		public uint dwMemberLevel;

		public uint dwMemberHeadId;

		public byte bGradeOfRank;

		public string snsHeadUrl;

		public TeamMember()
		{
			this.uID = new PlayerUniqueID();
		}
	}
}
